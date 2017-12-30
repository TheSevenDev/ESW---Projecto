using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CIMOB_IPS.Models;
using CIMOB_IPS.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

namespace CIMOB_IPS.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly CIMOB_IPS_DBContext _context;

        public ApplicationController(CIMOB_IPS_DBContext context)
        {
            _context = context;
        }

        public int GetCurrentUserID()
        {
            return int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        public async Task<IActionResult> New()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Index", "Home");

            ViewData["app_form"] = "NewApplication";
            ViewData["submit_form"] = "NewApplicationMob";
            

            int userID = GetCurrentUserID();

            ApplicationViewModel viewModel = new ApplicationViewModel { Account = new Account(), Application = new Application { IdStudentNavigation = GetStudentById(userID) } };
            viewModel.Account.Email = GetEmail(userID);
            var program = await _context.Program.Include(p => p.IdProgramTypeNavigation).Include(p => p.IdStateNavigation).Include(p => p.InstitutionProgram).FirstOrDefaultAsync(p => p.IdProgram == 1); 

            foreach(var ip in program.InstitutionProgram)
            {
                ip.IdOutgoingInstitutionNavigation = await _context.Institution
                    .Include(i => i.IdNationalityNavigation)
                    .SingleOrDefaultAsync(i => i.IdInstitution == ip.IdOutgoingInstitution);
            }


            viewModel.Institutions = program.InstitutionProgram.ToList();
            viewModel.Nationalities = PopulateNationalities();
            viewModel.Application.IdProgramNavigation = _context.Program.Where(p => p.IdProgram == 1).FirstOrDefault(); //ERASMUS
            return View(viewModel);
        }

        public Student GetStudentById(int intId)
        {
            return _context.Student.Where(s => s.IdAccount == intId).FirstOrDefault();
        }

        public string GetEmail(int intId)
        {
            return _context.Account.Where(s => s.IdAccount == intId).FirstOrDefault().Email;
        }

      
        [HttpPost]
        public IActionResult RegisterApplication(ApplicationViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Index", "Home");


            /*Application app = new Application
            {
                ApplicationDate = DateTime.Now,
                HasScholarship = model.Application.HasScholarship,
            };*/


            //_context.Application.Add(app);

            return RedirectToAction("MyApplications", "Application");
            
        }

        private IEnumerable<SelectListItem> PopulateNationalities()
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                List<SelectListItem> lisNationalities = new List<SelectListItem>();

                var listNationalities = context.Nationality.OrderBy(x => x.Description).ToList();

                foreach (Nationality n in listNationalities)
                {
                    lisNationalities.Add(new SelectListItem { Value = n.IdNationality.ToString(), Text = n.Description });
                }

                return lisNationalities;
            }
        }

        public async Task<IActionResult> MyApplications()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Index", "Home");

            AccountController ac = new AccountController();
            ProfileController pc = new ProfileController();

            int lngCurrentUserId = GetCurrentUserID();

            if (!ac.IsStudent(lngCurrentUserId))
                return RedirectToAction("Index", "Home");

            long studentId = ac.GetStudentId(lngCurrentUserId);

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var lisApplications = await context.Application.Where(a => a.IdStudent == studentId)
                    .Include(a => a.IdStateNavigation)
                    .Include(a => a.IdProgramNavigation)
                    .Include(a => a.ApplicationInstitutions)
                    .ToListAsync();

                foreach(Application app in lisApplications)
                {
                    app.ApplicationInstitutions = await context.ApplicationInstitutions
                        .Include(ai => ai.IdInstitutionNavigation).OrderBy(ai => ai.InstitutionOrder).Where(i=>i.IdApplication == app.IdApplication).ToListAsync();

                    app.IdProgramNavigation.IdProgramTypeNavigation = await context.ProgramType.Where(p => p.IdProgramType == app.IdProgramNavigation.IdProgramType).SingleOrDefaultAsync();
                }

                return View(lisApplications);
            }
        }

        public IActionResult FileTest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FileTest([Bind("File")] FileViewModel viewModel)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var testFile = new TestFile {  };
                
                using (var memoryStream = new MemoryStream())
                {
                    await viewModel.File.CopyToAsync(memoryStream);
                    testFile.FileTest = memoryStream.ToArray();
                }

                context.Add(testFile);
                await context.SaveChangesAsync();
            }

            return RedirectToAction("ShowFiles", "Application");
        }

        public async Task<IActionResult> ShowFiles()
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var files = await context.TestFile.ToListAsync();

                return View(files);
            }
        }

        public async Task<FileResult> OpenFile(int fileId)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var file = await context.TestFile.SingleOrDefaultAsync(f => f.IdFile == fileId);

                return File(file.FileTest, "application/pdf", "teste.pdf");
            }
        }

        public async Task<FileResult> Export()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var student = await context.Student
                    .Include(s => s.IdAccountNavigation)
                    .Include(s => s.IdCourseNavigation)
                    .SingleOrDefaultAsync(s => s.IdAccount == GetCurrentUserID());

                var strMes = DateTime.Now.ToString("MMMM");
                strMes = strMes.First().ToString().ToUpper() + strMes.Substring(1);


                StringBuilder strbHtml = new StringBuilder();
                strbHtml.AppendLine("<h2 style='text-align: center;'>Declaração de candidatura a mobilidade</h2>");
                //incluir morada
                strbHtml.AppendLine("<br><br><p>Eu, " + student.Name + ", portador(a) do n.º de cartão de cidadão " + student.Cc + ", nascido(a) na data " + student.BirthDate.ToString("dd/MM/yyyy"));
                strbHtml.AppendLine(", declaro que, no presente dia " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ", me candidato ao seguinte programa de mobilidade, ");
                strbHtml.AppendLine("tendo o perfeito conhecimento dos regulamentos associados com o mesmo, bem como os meus deveres e direitos.</p>");
                strbHtml.AppendLine("<br><br><br><p><b>O estudante</b></p><br><br>________________________________________ ");
                strbHtml.AppendLine(DateTime.Now.ToString("dd") + " " + strMes + " de " + DateTime.Now.ToString("yyyy"));

                MemoryStream ms = new MemoryStream();
                TextReader txtReader = new StringReader(strbHtml.ToString());

                // 1: create object of a itextsharp document class  
                Document doc = new Document(PageSize.A4, 25, 25, 25, 25);

                // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file  
                PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);

                // 3: we create a worker parse the document  
                HTMLWorker htmlWorker = new HTMLWorker(doc);

                // 4: we open document and start the worker on the document  
                doc.Open();
                htmlWorker.StartDocument();

                // 5: parse the html into the document  
                htmlWorker.Parse(txtReader);

                // 6: close the document and the worker  
                htmlWorker.EndDocument();
                htmlWorker.Close();
                doc.Close();

                return File(ms.ToArray(), "application/pdf", "teste.pdf");
            }
        }

        /*
        public FileResult CreatePdf()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("SamplePdf" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout = new PdfPTable(5);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF   
            doc.Add(Add_Content_To_PDF(tableLayout));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout)
        {

            float[] headers = { 50, 24, 45, 35, 50 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  

            List<Employee> employees = _context.employees.ToList<Employee>();



            tableLayout.AddCell(new PdfPCell(new Phrase("Creating Pdf using ItextSharp", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) {
                Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER
            });


            ////Add header  
            AddCellToHeader(tableLayout, "EmployeeId");
            AddCellToHeader(tableLayout, "Name");
            AddCellToHeader(tableLayout, "Gender");
            AddCellToHeader(tableLayout, "City");
            AddCellToHeader(tableLayout, "Hire Date");

            ////Add body  

            foreach (var emp in employees)
            {

                AddCellToBody(tableLayout, emp.EmployeeId.ToString());
                AddCellToBody(tableLayout, emp.Name);
                AddCellToBody(tableLayout, emp.Gender);
                AddCellToBody(tableLayout, emp.City);
                AddCellToBody(tableLayout, emp.Hire_Date.ToString());

            }

            return tableLayout;
        }
        // Method to add single cell to the Header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, BaseColor.YELLOW)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(128, 0, 0)
            });
        }

        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK)))
             {
                HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
        });
        }
        */
    }
}