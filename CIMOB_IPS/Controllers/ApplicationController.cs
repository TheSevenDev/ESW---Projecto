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
using System.Text;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;

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

            int userID = GetCurrentUserID();

            Student student = GetStudentById(userID);
            var app = _context.Application.Where(ap => ap.IdStudent == student.IdStudent);

            if(app.Count() >= 3)
              return RedirectToAction("Index", "Home");


            ViewData["app_form"] = "NewApplication";
            ViewData["submit_form"] = "NewApplicationMob";
                

            ApplicationViewModel viewModel = new ApplicationViewModel { Account = new Account(), Application = new Application { IdStudentNavigation = student } };
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

            var postalCode = student.PostalCode;

            viewModel.PostalCode1 = postalCode.Substring(0, 4);
            viewModel.PostalCode2 = postalCode.Substring(5, 3);


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
        public async Task<IActionResult> RegisterApplication(ApplicationViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Index", "Home");

            Application app = model.Application;
            app.IdProgramNavigation =  _context.Program.Where(p => p.IdProgram == 1).FirstOrDefault(); //ERASMUS MUDAR PROX FASE
            app.IdStateNavigation = _context.State.Where(s => s.Description == "Em Avaliação").FirstOrDefault();
            app.IdStudentNavigation = GetStudentById(GetCurrentUserID());
            app.ApplicationDate = DateTime.Now;
            app.SignedAppFile = await CreateSignedApplication(model);

            _context.Application.Add(model.Application);
            _context.SaveChanges();

            AddApplicationNotification();

            return RedirectToAction("MyApplications", "Application");
        }

        [HttpPost]
        public IActionResult RegisterApplicationInstitutions([FromQuery] string inst1,[FromQuery] string inst2,[FromQuery] string inst3)
        {
            var applicationID = GetNewApplicationID();
            if (!String.IsNullOrEmpty(inst1))
            {
                //Console.WriteLine("===========================" + inst1); 
                using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
                {
                    scnConnection.Open();
                    string strQuery = "INSERT INTO Application_Institutions Values(@Application, @Institution, 1)";

                    SqlCommand scmCommand = new SqlCommand(strQuery, scnConnection);
                    scmCommand.Parameters.AddWithValue("@Application", applicationID);
                    scmCommand.Parameters.AddWithValue("@Institution", GetIdByInstitution(inst1));
                    scmCommand.ExecuteNonQuery();
                }
            }

            if (!String.IsNullOrEmpty(inst2))
            {
                //Console.WriteLine("===========================" + inst2);
                using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
                {
                    scnConnection.Open();
                    string strQuery = "INSERT INTO Application_Institutions Values(@Application, @Institution, 2)";

                    SqlCommand scmCommand = new SqlCommand(strQuery, scnConnection);
                    scmCommand.Parameters.AddWithValue("@Application", applicationID);
                    scmCommand.Parameters.AddWithValue("@Institution", GetIdByInstitution(inst2));
                    scmCommand.ExecuteNonQuery();
                }
            }

            if (!String.IsNullOrEmpty(inst3))
            {
                //Console.WriteLine("===========================" + inst3);

                using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
                {
                    scnConnection.Open();
                    string strQuery = "INSERT INTO Application_Institutions Values(@Application, @Institution, 3)";

                    SqlCommand scmCommand = new SqlCommand(strQuery, scnConnection);
                    scmCommand.Parameters.AddWithValue("@Application", applicationID);
                    scmCommand.Parameters.AddWithValue("@Institution", GetIdByInstitution(inst3));
                    scmCommand.ExecuteNonQuery();
                }
            }

            return Json("Sucess");
        }

        private long GetIdByInstitution(string institutionName)
        {
            return _context.Institution.Where(i => i.Name == institutionName).FirstOrDefault().IdInstitution;
        }

        private long GetNewApplicationID()
        {
            return _context.Application.Max(i => i.IdApplication) + 1;
        }


        private void AddApplicationNotification()
        {
            Notification not = new Notification
            {
                ReadNotification = false,
                Description = "Candidatura a mobilidade submetida a " + DateTime.Now.ToString("dd/MM/yyyy"),
                ControllerName = "Application",
                ActionName = "MyApplications",
                NotificationDate = DateTime.Now,
                IdAccountNavigation = _context.Account.Where(a => a.IdAccount == GetCurrentUserID()).FirstOrDefault()
            };
            _context.Notification.Add(not);
            _context.SaveChanges();
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

        public async Task<FileResult> OpenSignedAppFile(int fileId)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var file = await context.Application.Where(a => a.IdApplication == fileId).Select(a => a.SignedAppFile).SingleOrDefaultAsync();

                return File(file, "application/pdf", "ComprovativoDeCandidatura.pdf");
            }
        }

        [HttpPost]
        public async Task<byte[]> CreateSignedApplication(ApplicationViewModel viewModel)
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var img = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(viewModel.Signature));

            Image image = Image.GetInstance(viewModel.Signature);

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var student = await context.Student
                    .Include(s => s.IdAccountNavigation)
                    .Include(s => s.IdCourseNavigation)
                    .SingleOrDefaultAsync(s => s.IdAccount == GetCurrentUserID());

                var programType = await context.ProgramType
                    .SingleOrDefaultAsync(s => s.IdProgramType == 1); //erasmus, mudar prox fase

                var strMes = DateTime.Now.ToString("MMMM");
                strMes = strMes.First().ToString().ToUpper() + strMes.Substring(1);

                StringBuilder strbHtml = new StringBuilder();
                strbHtml.AppendLine("<h2 style='text-align: center;'>Declaração de candidatura a mobilidade</h2>");
                //incluir morada
                strbHtml.AppendLine("<br><br><p>Eu, " + student.Name + ", portador(a) do n.º de cartão de cidadão " + student.Cc + ", nascido(a) na data " + student.BirthDate.ToString("dd/MM/yyyy"));
                strbHtml.AppendLine(", declaro que, no presente dia " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ", me candidato ao seguinte programa de mobilidade" + programType.Name + ", ");
                strbHtml.AppendLine("tendo o perfeito conhecimento dos regulamentos associados com o mesmo, bem como os meus deveres e direitos.</p>");
                strbHtml.AppendLine("<br><br><br><p><b>O estudante</b></p><br><br>");
                //strbHtml.AppendLine("<img src='" + String.Format("data:image/gif;base64,{0}", Convert.ToBase64String(signature.MySignature)).Replace("/", "//") + "' />");

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

                doc.Add(image);

                strbHtml = new StringBuilder();
                strbHtml.AppendLine(DateTime.Now.ToString("dd") + " " + strMes + " de " + DateTime.Now.ToString("yyyy"));
                txtReader = new StringReader(strbHtml.ToString());
                htmlWorker.Parse(txtReader);

                // 6: close the document and the worker  
                htmlWorker.EndDocument();
                htmlWorker.Close();
                doc.Close();

                //return File(ms.ToArray(), "application/pdf", "teste.pdf");
                return ms.ToArray();
            }
        }
    }
}