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

            ProfileController pc = new ProfileController();
            int intEcts = await pc.GetCurrentStudentECTS(User);

            var program = await _context.Program.Include(p => p.IdProgramTypeNavigation).Include(p => p.IdStateNavigation).Include(p => p.InstitutionProgram).FirstOrDefaultAsync(p => p.IdProgram == 1);

            int userID = GetCurrentUserID();

            Student student = GetStudentById(userID);
            var app = _context.Application.Where(ap => ap.IdStudent == student.IdStudent);

            if (app.Count() >= 3 || program.Vacancies <= 0 || !(program.IdStateNavigation.Description == "Aberto") || intEcts < 45)
                return RedirectToAction("Index", "Home");

            ViewData["app_form"] = "NewApplication";
            ViewData["submit_form"] = "NewApplicationMob";

            ApplicationViewModel viewModel = new ApplicationViewModel { Account = new Account(), Application = new Application { IdStudentNavigation = student } };
            viewModel.Account.Email = GetEmail(userID);

            foreach (var ip in program.InstitutionProgram)
            {
                ip.IdOutgoingInstitutionNavigation = await _context.Institution
                    .Include(i => i.IdNationalityNavigation)
                    .SingleOrDefaultAsync(i => i.IdInstitution == ip.IdOutgoingInstitution);
            }

            viewModel.Institutions = program.InstitutionProgram.ToList();
            viewModel.Nationalities = PopulateNationalities();

            var postalCode = student.IdAddressNavigation.PostalCode;

            viewModel.PostalCode1 = postalCode.Substring(0, 4);
            viewModel.PostalCode2 = postalCode.Substring(5, 3);


            return View(viewModel);
        }

        public Student GetStudentById(int intId)
        {
            return _context.Student.Where(s => s.IdAccount == intId)
                .Include(s => s.IdAddressNavigation)
                .FirstOrDefault();
        }

        public string GetEmail(int intId)
        {
            var account = _context.Account.Where(s => s.IdAccount == intId).FirstOrDefault();

            var email = "";

            if (account != null)
            {
                email = _context.Account.Where(s => s.IdAccount == intId).FirstOrDefault().Email;
            }

            return email;
        }

        public async Task<int> GetNumberPendentApplications(ClaimsPrincipal user)
        {
            var intCurrentId = int.Parse(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                return await context.Application
                    .CountAsync(a => a.IdStudent == GetStudentById(intCurrentId).IdStudent
                    && a.IdState == (from s in context.State where s.Description == "Em avaliação" select s.IdState).SingleOrDefault());
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterApplication(ApplicationViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Index", "Home");

            Application app = model.Application;
            app.IdProgramNavigation = _context.Program.Where(p => p.IdProgram == 1).FirstOrDefault(); //ERASMUS MUDAR PROX FASE
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
        public IActionResult RegisterApplicationInstitutions([FromQuery] string inst1, [FromQuery] string inst2, [FromQuery] string inst3)
        {
            var applicationID = GetNewApplicationID();
            if (!String.IsNullOrEmpty(inst1))
            {

                _context.ApplicationInstitutions.Add(new ApplicationInstitutions { IdApplication = applicationID, IdInstitution = GetIdByInstitution(inst1), InstitutionOrder = 1 });
            }

            if (!String.IsNullOrEmpty(inst2))
            {
                _context.ApplicationInstitutions.Add(new ApplicationInstitutions { IdApplication = applicationID, IdInstitution = GetIdByInstitution(inst2), InstitutionOrder = 2 });
            }

            if (!String.IsNullOrEmpty(inst3))
            {
                _context.ApplicationInstitutions.Add(new ApplicationInstitutions { IdApplication = applicationID, IdInstitution = GetIdByInstitution(inst3), InstitutionOrder = 3 });
            }

            _context.SaveChanges();

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

                foreach (Application app in lisApplications)
                {
                    app.ApplicationInstitutions = await context.ApplicationInstitutions
                        .Include(ai => ai.IdInstitutionNavigation).OrderBy(ai => ai.InstitutionOrder).Where(i => i.IdApplication == app.IdApplication).ToListAsync();

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
                var testFile = new TestFile { };

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
                strbHtml.AppendLine(", declaro que, no presente dia " + DateTime.Now.Date.ToString("dd/MM/yyyy") + ", me candidato ao programa de mobilidade " + programType.Name + ", ");
                strbHtml.AppendLine("tendo o perfeito conhecimento dos regulamentos associados com o mesmo, bem como os meus deveres e direitos.</p>");
                strbHtml.AppendLine("<br><br><br><p><b>O estudante,</b></p><br><br>");
                //strbHtml.AppendLine("<img src='" + String.Format("data:image/gif;base64,{0}", Convert.ToBase64String(signature.MySignature)).Replace("/", "//") + "' />");

                MemoryStream ms = new MemoryStream();
                TextReader txtReader = new StringReader(strbHtml.ToString());

                Document doc = new Document(PageSize.A4, 25, 25, 25, 25);


                PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);

                HTMLWorker htmlWorker = new HTMLWorker(doc);

                doc.Open();
                htmlWorker.StartDocument();

                htmlWorker.Parse(txtReader);

                doc.Add(image);

                strbHtml = new StringBuilder();
                strbHtml.AppendLine(DateTime.Now.ToString("dd") + " " + strMes + " de " + DateTime.Now.ToString("yyyy"));
                txtReader = new StringReader(strbHtml.ToString());
                htmlWorker.Parse(txtReader);
                htmlWorker.EndDocument();
                htmlWorker.Close();
                doc.Close();

                //return File(ms.ToArray(), "application/pdf", "teste.pdf");
                return ms.ToArray();
            }
        }

        public async Task<IActionResult> Index(int? pageApplication, string search_by)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!(new AccountController().IsTechnician(GetCurrentUserID())))
                return RedirectToAction("Index", "Home");

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                int intPageSize = 10;
                int intPageApplications = (pageApplication ?? 1);

                
                if (String.IsNullOrEmpty(search_by))
                {
                    var applications = (from a in context.Application orderby a.ApplicationDate select a).OrderBy(a => a.ApplicationDate)
                        .Include(a => a.IdStateNavigation)
                        .Include(a => a.IdStudentNavigation)
                        .Include(a => a.IdStudentNavigation.IdCourseNavigation)
                        .Include(a => a.IdStudentNavigation.IdCourseNavigation.IdInstitutionNavigation)
                        .Include(a => a.IdProgramNavigation)
                        .Where(a => a.IdStateNavigation.IdState == 1)
                        ;

                    var paginatedApplications = await PaginatedList<Application>.CreateAsync(applications.AsNoTracking(), intPageApplications, intPageSize);
                    ViewData["search-by"] = "";
                    return View(paginatedApplications);
                }
                else
                {
                   var applications = (from a in context.Application orderby a.ApplicationDate select a).OrderBy(a => a.ApplicationDate)
                        .Include(a => a.IdStateNavigation)
                        .Include(a => a.IdStudentNavigation)
                        .Include(a => a.IdProgramNavigation)
                        .Include(a => a.IdStudentNavigation.IdCourseNavigation)
                        .Include(a => a.IdStudentNavigation.IdCourseNavigation.IdInstitutionNavigation)
                        .Where(a => a.IdStateNavigation.IdState == 1)
                        .Where(a=> a.IdStudentNavigation.Name.Contains(search_by) || a.IdStudentNavigation.StudentNum.ToString().Contains(search_by))
                        ;

                    ViewData["search-by"] = search_by.ToString();
                    var paginatedApplications = await PaginatedList<Application>.CreateAsync(applications.AsNoTracking(), intPageApplications, intPageSize);

                    return View(paginatedApplications);
                }    
            }
        }

        [HttpPost]
        public IActionResult DeleteApplication()
        {
            AccountController ac = new AccountController();
            ProfileController pc = new ProfileController();

            int lngCurrentUserId = GetCurrentUserID();
            int applicationId = Int32.Parse(Request.Form["idApplication"]);

            if (!ac.IsStudent(lngCurrentUserId))
                return RedirectToAction("Index", "Home");

            long studentId = ac.GetStudentId(lngCurrentUserId);

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                try
                {
                    var application = context.Application.Where(app => app.IdApplication == applicationId).FirstOrDefault();
                    if (application != null)
                        context.Application.Remove(application);

                    context.SaveChanges();
                }
                catch
                {

                }

                return RedirectToAction("MyApplications", "Application");
            }
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!(User.IsInRole("tecnico") || User.IsInRole("tecnico_admin")))
                return RedirectToAction("Index", "Home");

            Console.WriteLine("===================== ================" + id);


            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var application = context.Application.Where(a => a.IdApplication == Int32.Parse(id))
                    .Include(a => a.IdStateNavigation)
                    .Include(a => a.IdProgramNavigation)
                    .Include(a => a.ApplicationInstitutions)
                    .FirstOrDefault();

               
                if (application == null)
                    return RedirectToAction("Index", "Application");
                else
                {
                    application.ApplicationInstitutions = context.ApplicationInstitutions
                        .Include(ai => ai.IdInstitutionNavigation).OrderBy(ai => ai.InstitutionOrder).Where(i => i.IdApplication == application.IdApplication).ToList();

                    application.IdProgramNavigation.IdProgramTypeNavigation = context.ProgramType.Where(p => p.IdProgramType == application.IdProgramNavigation.IdProgramType).SingleOrDefault();

                    return PartialView("_ApplicationDetails", application);
                }
            }
        }

        [HttpGet]
        public IActionResult Evaluate(int appId, bool? bolTechError)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!(User.IsInRole("tecnico") || User.IsInRole("tecnico_admin")))
                return RedirectToAction("Index", "Home");

            ViewData["applicationID"] = appId;

            if(bolTechError != null)
            {
                if ((bool)bolTechError)
                    ViewData["technicians-error"] = "É necessário escolher um técnico responsável pela mobilidade do(a) aluno(a).";
            }

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var app = context.Application.Where(a => a.IdApplication == appId)
                    .Include(a => a.IdStudentNavigation)
                    .FirstOrDefault();
                ViewData["application-student-name"] = app.IdStudentNavigation.Name;
                ViewData["application-student-number"] = app.IdStudentNavigation.StudentNum.ToString();
                ViewData["application-student-credits"] = app.IdStudentNavigation.Credits.ToString();
                ViewData["application-student-motivation-card"] = app.MotivationCard.ToString();
            }
            return View(new ApplicationEvaluationViewModel { IdApplication = appId, Technicians = PopulateTechnicians(), FinalEvalution = 0.00 });
        }

        [HttpPost]
        public IActionResult EvaluateApplication(ApplicationEvaluationViewModel viewModel)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!(User.IsInRole("tecnico") || User.IsInRole("tecnico_admin")))
                return RedirectToAction("Index", "Home");

            var appId = viewModel.IdApplication;
            var evaluationResult = Request.Form["final_classification"].ToString();

            double dblResult = Double.Parse(evaluationResult.Replace(".", ","));

            if(dblResult >= 50 && viewModel.IdTechnician == 0)
                return RedirectToAction("Evaluate", "Application", new { appId = viewModel.IdApplication, bolTechError = true });

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var application = context.Application.Where(a => a.IdApplication == viewModel.IdApplication).FirstOrDefault();
                var student = context.Student.Where(s => s.IdStudent == application.IdStudent)
                    .Include(s => s.IdAccountNavigation)
                    .FirstOrDefault();
                var program = context.Program.Where(p => p.IdProgram == application.IdProgram)
                    .Include(p => p.IdProgramTypeNavigation)
                    .SingleOrDefault();

                application.FinalEvaluation = (short)dblResult;

                bool bolApproved = dblResult >= 50;
                
                if(dblResult >= 50)
                {
                    application.IdState = context.State.Where(s => s.Description == "Aceite").Select(a => a.IdState).SingleOrDefault();

                    Mobility mobility = new Mobility
                    {
                        IdApplication = appId,
                        BeginDate = program.MobilityDate,
                        IdOutgoingInstitution = 2, //MUDAR ISTO MUDAR ISTO MUDAR ISTO
                        IdResponsibleTechnician = viewModel.IdTechnician,
                        IdState = 12 //MUDAR ISTO MUDAR ISTO MUDAR ISTO
                    };

                    context.Add(mobility);

                    //caso tecnico n seja actual, enviar notificação tambem

                    long intResponsibleTechnicianId = context.Technician.Where(t => t.IdTechnician == viewModel.IdTechnician).Select(t => t.IdAccount).SingleOrDefault();

                    if(intResponsibleTechnicianId != GetCurrentUserID())
                    {
                        Notification notificationTechnician = new Notification
                        {
                            ReadNotification = false,
                            Description = "Foi posta uma nova mobilidade a seu cargo.",
                            ControllerName = "Index", //MUDAR ISTO MUDAR ISTO MUDAR ISTO
                            ActionName = "Home",
                            NotificationDate = DateTime.Now,
                            IdAccount = context.Technician.Where(t => t.IdTechnician == viewModel.IdTechnician).Select(t => t.IdAccount).SingleOrDefault()
                        };

                        context.Notification.Add(notificationTechnician);
                    }
                }
                else
                {
                    application.IdState = context.State.Where(s => s.Description == "Negada").Select(a => a.IdState).SingleOrDefault();
                }

                context.Update(application);

                //email ao aluno
                string strLink = "http://cimob-ips.azurewebsites.net/MyApplications";

                var strbBody = new StringBuilder();
                strbBody.AppendLine("Caro estudante,<br><br>");
                strbBody.AppendFormat(@"Informamos que a sua candidatura para mobilidade, feita a " + application.ApplicationDate.Date);
                strbBody.AppendFormat(" para o programa " + program.IdProgramTypeNavigation.Name);

                if (bolApproved)
                {
                    strbBody.AppendFormat(" foi aprovada.<br/> <br/>");
                    strbBody.AppendLine("Clique <a href=\"" + strLink + "\">aqui</a> ou aceda a 'Minhas Candidaturas' para confirmar a mobilidade.<br/>");
                    strbBody.AppendLine("Note que este passo é <b>vital</b> para o início de mobilidade, sem ele, não poderá ser contactado pelos técnicos do CIMOB-IPS.<br/><br/>");
                }
                else
                {
                    strbBody.AppendFormat("não foi aprovada.<br/> <br/>");
                    strbBody.AppendLine("Clique <a href=\"" + strLink + "\">aqui</a> ou aceda a 'Minhas Candidaturas' para consultar o estado das suas candidaturas.<br/><br/>");
                }

                strbBody.AppendLine("Cumprimentos, <br> A Equipa do CIMOB-IPS.");

                Email.SendEmail(student.IdAccountNavigation.Email, "Alteração de estado de candidatura", strbBody.ToString());

                //notificação ao aluno
                Notification notificationStudent = new Notification
                {
                    ReadNotification = false,
                    Description = "Foi alterada o estado de uma das suas candidaturas.",
                    ControllerName = "Application",
                    ActionName = "MyApplications",
                    NotificationDate = DateTime.Now,
                    IdAccount = student.IdAccount
                };

                context.Notification.Add(notificationStudent);

                context.SaveChanges();
            }

            return View();
        }

        private IEnumerable<SelectListItem> PopulateTechnicians()
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                List<SelectListItem> lisTechnicians = new List<SelectListItem>();

                var listTechnicians = context.Technician.Where(x => x.Active == true).OrderBy(x => x.Name).ToList();

                foreach (Technician n in listTechnicians)
                {
                    lisTechnicians.Add(new SelectListItem { Value = n.IdTechnician.ToString(), Text = n.Name });
                }

                return lisTechnicians;
            }
        }

        public async Task<IActionResult> Approved(int? pageApplication)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                int intPageSize = 10;
                int intPageApplications = (pageApplication ?? 1);

                var applications = (from a in context.Application
                                    select a)
                    .OrderBy(a => a.IdStudentNavigation.StudentNum)
                    .Include(a => a.IdStateNavigation)
                    .Include(a => a.IdStudentNavigation)
                    .Include(a => a.IdProgramNavigation)
                    .Where(a => a.IdProgramNavigation.IdState == 1 && a.FinalEvaluation >= 50);

                var paginatedApplications = await PaginatedList<Application>.CreateAsync(applications.AsNoTracking(), intPageApplications, intPageSize);

                return View(paginatedApplications);
            }
        }
    }
}
