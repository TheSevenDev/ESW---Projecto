using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using CIMOB_IPS.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using CIMOB_IPS.Models.ViewModels;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System.Xml;
using System.Net;
using System.IO;

namespace CIMOB_IPS.Controllers
{
    /// <summary>
    /// Controlador para as ações relacionadas com a Conta de utilizador.
    /// Contem métodos para efetuar um registo, autenticação e recuperação da palavra-passe.
    /// </summary>
    /// <remarks></remarks>
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        #region Register

        /// <summary>
        ///  Retorna a view para o registo na aplicação. O tipo de registo por defeito é o pré-registo de um estudante na aplicação (student-preregister)
        /// </summary>
        /// <returns>View Register/Index</returns>
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["register-type"] = "student-preregister";
            return View();
        }

        /// <summary>
        /// Retorna a vista com as informações paginadas dos técnicos registados na aplicação. 
        /// </summary>
        /// <param name="pagePending">Paginação da tabela dos convites pendentes</param>
        /// <param name="pageTechnicians">Paginação da tabela dos técnicos</param>
        /// <returns>View Technicians</returns>
        /// <remarks></remarks>
        [ActionName("Technicians")]
        public IActionResult Technicians(int? pagePending, int? pageTechnicians)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            int intPageSize = 10;
            int intPagePendingNumber = (pagePending ?? 1);
            int intPageTechniciansNumber = (pageTechnicians ?? 1);

            return View("Technicians", PopulateTechnicians(intPagePendingNumber, intPageTechniciansNumber, intPageSize));
        }


        /// <summary>
        /// Método que retorna o documento XML correspondente ao pedido de morada à API dos CTT.
        /// </summary>
        /// <param name="code">Código Postal</param>
        /// <returns>Documento XML com informaçõs da morada com base no código postal</returns>
        /// <remarks></remarks>
        public ContentResult Address([FromQuery] string code)
        {
            if (!String.IsNullOrEmpty(code))
            {
                var url = "http://www.ctt.pt/pdcp/xml_pdcp?incodpos=" + code;
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(url);
                return Content(XmlToString(xmlDoc), "application/xml", Encoding.UTF8);
            }
            return Content("", "");
        }

        /// <summary>
        /// Converte um documento XML numa string.
        /// </summary>
        /// <param name="doc">Documento XML a converter</param>
        /// <returns>Documento XML em string</returns>
        /// <remarks></remarks>
        private string XmlToString(XmlDocument doc)
        {
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                doc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Popula o viewmodel com os objetos dos técnicos, provenientes da base de dados. Retorna os objetos incluídos nas páginas passadas como argumento do método.
        /// </summary>
        /// <param name="intPendingPageNumber">Número da página dos Técnicos Pendentes</param>
        /// <param name="intTechniciansPageNumber">Número da página dos Técnicos</param>
        /// <param name="intPendingPageSize">Tamanho da página dos técnicos pendentes</param>
        /// <returns>ViewModel com as listas atualizadas</returns>
        /// <remarks></remarks>
        private TechnicianManagementViewModel PopulateTechnicians(int intPendingPageNumber, int intTechniciansPageNumber, int intPendingPageSize)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var lisIdAdmin = from s in context.Technician where s.IdAccount == GetCurrentUserID() && s.IdTechnician == 1 select s;

                var lisPendingAccounts = GetPendingAccountsPaginated(intPendingPageNumber, intPendingPageSize);

                TechnicianManagementViewModel viewModel = new TechnicianManagementViewModel { PendingAccounts = lisPendingAccounts.Result };

                if (lisIdAdmin != null)
                {
                    viewModel.Technicians = GetTechniciansPaginated(intTechniciansPageNumber, intPendingPageSize).Result;
                }

                return viewModel;
            }
        }

        /// <summary>
        /// Retorna uma lista paginada com todos os técnicos do CIMOB com contas pendentes.
        /// </summary>
        /// <param name="intPendingPageNumber">Número da página</param>
        /// <param name="intPendingPageSize">Tamanho da página</param>
        /// <returns>Lista paginada com os técnicos pendentes.</returns>
        /// <remarks></remarks>
        private async Task<PaginatedList<PendingAccount>> GetPendingAccountsPaginated(int intPendingPageNumber, int intPendingPageSize)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var pendingAccounts = from s in context.PendingAccount
                                      select s;

                return await PaginatedList<PendingAccount>.CreateAsync(pendingAccounts.AsNoTracking(), intPendingPageNumber, intPendingPageSize);
            }
        }

        /// <summary>
        /// Retorna uma lista paginada com os técnicos do CIMOB registados na aplicação.
        /// </summary>
        /// <param name="intTechniciansPageNumber">Número da página</param>
        /// <param name="intPendingPageSize">Tamanho da página</param>
        /// <returns>Lista preenchida com os técnicos.</returns>
        /// <remarks></remarks>
        private async Task<PaginatedList<Technician>> GetTechniciansPaginated(int intTechniciansPageNumber, int intPendingPageSize)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var technicians = (from s in context.Technician
                                   select new Technician
                                   {
                                       Name = s.Name,
                                       IdAccountNavigation = new Account { Email = s.IdAccountNavigation.Email },
                                       Telephone = s.Telephone,
                                       IsAdmin = s.IsAdmin
                                   });

                return await PaginatedList<Technician>.CreateAsync(technicians.AsNoTracking(), intTechniciansPageNumber, intPendingPageSize);
            }
        }

        /// <summary>
        /// Regista um novo estudante na aplicação. 
        /// Caso exista um erro na validação da data de nascimento, é novamente carregada mesma página com uma mensagem de erro na operação.
        /// Caso contrário, redirecciona o utilizador para a página com o formulário de autenticação.
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>View Account/Login</returns>
        /// <remarks></remarks>
        [HttpPost]
        public IActionResult RegisterStudent(RegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                string strEmail = model.EmailView;
                string strPassword = model.PasswordView;

                long lngIdAccount = 0;

                lngIdAccount = InsertAccountGetId(strEmail, strPassword);

                DeletePendingAccount(strEmail);

                long lngStudentNum = model.Student.StudentNum;
                string strName = model.Student.Name;
                long lngIdCourse = model.Student.IdCourse;
                long lngCcNum = model.Student.Cc;
                long lngTelephone = model.Student.Telephone;
                long lngIdNationality = model.Student.IdNationality;
                int intCredits = model.Student.Credits;
                DateTime dtBithDate = model.Student.BirthDate;
                string strGender = model.Student.Gender;
                string postalCode = "" + model.PostalCode1 + "-" + model.PostalCode2;
                string strAddress = model.Student.IdAddressNavigation.AddressDesc;

                Address addressAux = new Address
                {
                    PostalCode = postalCode,
                    AddressDesc = strAddress,
                    DoorNumber = model.Student.IdAddressNavigation.DoorNumber,
                    Floor = model.Student.IdAddressNavigation.Floor
                };

                InsertAddress(addressAux);

                Student student = new Student
                {
                    IdAccount = lngIdAccount,
                    IdCourse = lngIdCourse,
                    Name = strName,
                    Cc = lngCcNum,
                    Telephone = lngTelephone,
                    IdNationality = lngIdNationality,
                    Credits = intCredits,
                    StudentNum = lngStudentNum,
                    BirthDate = dtBithDate,
                    Gender = strGender,
                    IdAddress = addressAux.IdAddress
                };

                InsertStudent(student, strEmail);

                WelcomeEmail(strEmail);
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (model.Student.BirthDate.Date >= DateTime.Now.Date)
                    return RedirectToAction("RegisterStudent", "Account", new { account_id = model.RegisterGUID, birthDateError = true });

                return RedirectToAction("RegisterStudent", "Account", new { account_id = model.RegisterGUID });
            }
        }

        /// <summary>
        /// Regista um novo técnico na aplicação.
        /// </summary>
        /// <param name="model">Modelo</param>
        /// <returns>Retorna a View com uma mensagem de erro ou sucesso na operação.</returns>
        /// <remarks></remarks>
        [HttpPost]
        public IActionResult RegisterTechnician(RegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            string strEmail = model.EmailView;
            string strPassword = model.PasswordView;

            long lngIdAccount = InsertAccountGetId(strEmail, strPassword);

            DeletePendingAccount(strEmail);

            bool bolIsAdmin = model.Technician.IsAdmin;
            string strName = model.Technician.Name;
            long lngTelephone = model.Technician.Telephone;

            Technician technician = new Technician { IdAccount = lngIdAccount, Name = strName, Telephone = lngTelephone, IsAdmin = bolIsAdmin, Active = true };

            InsertTechnician(technician, strEmail);

            WelcomeEmail(strEmail);
            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Adiciona um novo pré-registo na aplicação.
        /// </summary>
        /// <param name="model">RegisterViewModel modelo</param>
        /// <returns>Retorna a própria vista com uma mensagem de sucesso ou erro na operação.</returns>
        /// <remarks></remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PreRegister(RegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["register-type"] = "student-preregister";

            long lngStudentNumber = model.Student.StudentNum;
            String strStudentEmail = lngStudentNumber.ToString() + "@estudantes.ips.pt";

            try
            {
                bool success = InsertPendingAccount(strStudentEmail, EnumAccountType.STUDENT, 0);
                if (success)
                {
                    ViewData["message"] = "Número registado. Verifique o seu e-mail de estudante.";
                    ViewData["error-message"] = "";
                }
                else
                {
                    ViewData["error-message"] = "Número já registado.";
                    ViewData["message"] = "";
                }

                return View("Register");
            }
            catch (SqlException e)
            {
                ViewData["error-message"] = "Conexão Falhada.";
            }

            return View("Register");
        }      

        /// <summary>
        /// Retorna a vista com o formulário para a criação de um registo de um estudante.
        /// </summary>
        /// <param name="account_id">Chave primária da conta</param>
        /// <param name="birthDateError">Verifica se houve um erro na escolha da idade, caso exista mostra uma mensagem.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IActionResult RegisterStudent([FromQuery] string account_id, bool? birthDateError)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["register-type"] = "student-register";

            if (birthDateError != null)
                if ((bool)birthDateError)
                    ViewData["BirthDateError"] = "A data de nascimento tem de ser anterior a hoje.";

            List<SelectListItem> lisNationalities = new List<SelectListItem>();

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var email = context.PendingAccount.Where(pa => pa.Guid == account_id).Select(p => p.Email).FirstOrDefault();

                if (email == null)
                    return RedirectToAction("Index", "Home");
                else
                {
                    string strEmail = email.ToString();
                    string strStudentNumber = email.ToString().Substring(0, 9); 
                    ViewData["student-email"] = strEmail;
                    ViewData["student-number"] = strStudentNumber;
                }
            }

            return View("Register", new RegisterViewModel { Nationalities = PopulateNationalities(), Courses = PopulateCourses(), RegisterGUID = account_id });
        }

        /// <summary>
        /// Popula uma lista com várias nacionalidades para serem usadas ao criar uma nova conta de estudante.
        /// </summary>
        /// <returns>Lista preenchida com as nacionalidades.</returns>
        /// <remarks></remarks>
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

        /// <summary>
        /// Insere cursos numa lista para serem escolhidos pelo estudante ao criar uma nova conta. Só são tidos em conta cursos portugueses e da instituição IPS.
        /// </summary>
        /// <returns>Lista preenchida com os cursos.</returns>
        /// <remarks></remarks>
        private IEnumerable<SelectListItem> PopulateCourses()
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                List<SelectListItem> lisCourses = new List<SelectListItem>();

                var listCourses = context.Course
                    .Where(x => (from i in context.Institution
                                 where i.IdNationality ==
   (from n in context.Nationality where n.Description == "PORTUGAL" select n.IdNationality).FirstOrDefault()
                                 select i.IdInstitution).Contains(x.IdInstitution))
                    .OrderBy(x => x.Name).ToList();

                foreach (Course n in listCourses)
                {
                    lisCourses.Add(new SelectListItem { Value = n.IdCourse.ToString(), Text = n.Name });
                }

                return lisCourses;
            }
        }

        /// <summary>
        /// Retorna a vista com um formulário para o registo de um novo técnico.
        /// </summary>
        /// <param name="account_id">Chave primária da conta que será associada ao registo</param>
        /// <returns>View Account/Register</returns>
        /// <remarks></remarks>
        public IActionResult RegisterTechnician([FromQuery] string account_id)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["register-type"] = "technician";

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {

                PendingAccount pendingAccount = context.PendingAccount.Where(pa => pa.Guid == account_id).FirstOrDefault();

                if (pendingAccount == null)
                    return RedirectToAction("Index", "Home");
                else
                {
                    string strEmail = pendingAccount.Email.ToString();
                    ViewData["technician-email"] = strEmail;
                    ViewData["technician-isAdmin"] = pendingAccount.IsAdmin;
                }

                return View("Register", new RegisterViewModel { EmailView = ViewData["technician-email"].ToString(), Technician = new Technician { IsAdmin = (bool)ViewData["technician-isAdmin"] } });
            }
        }

        /// <summary>
        /// Retorna a vista para os convites de Técnicos do CIMOB
        /// </summary>
        /// <returns>View InviteTechnician</returns>
        /// <remarks></remarks>
        public IActionResult InviteTec()
        {
            return View("InviteTechnician");
        }

        /// <summary>
        /// Executa um convite a um técnico para efetuar um registo na aplicação.
        /// </summary>
        /// <param name="model">Técnico</param>
        /// <returns>View Technicians com as mensagens com o sucesso ou erro da operação.</returns>
        /// <remarks></remarks>
        [HttpPost]
        public IActionResult InviteTec(TechnicianManagementViewModel model)
        {
            string strDestination = model.EmailView;
            int intIsAdmin = Convert.ToInt32(model.IsAdmin);
            Guid guid;
            guid = Guid.NewGuid();

            try
            {
                bool bolSuccess = InsertPendingAccount(strDestination, EnumAccountType.TECHNICIAN, intIsAdmin);
                if (bolSuccess)
                {
                    ViewData["message"] = "Email enviado.";
                    ViewData["invite-tech-display"] = "block";
                    ViewData["error-message"] = "";
                }
                else
                {
                    ViewData["error-message"] = "Email já registado.";
                    ViewData["invite-tech-display"] = "block";
                    ViewData["invite-tech-email"] = strDestination;
                    ViewData["message"] = "";
                }

                return View("Technicians", PopulateTechnicians(1, 1, 10));
            }
            catch (SqlException e)
            {
                ViewData["invite-tech-display"] = "block";
                ViewData["error-message"] = "Conexão Falhada.";
            }

            return View("Technicians", PopulateTechnicians(1, 1, 10));
        }

        /// <summary>
        /// Elimina uma conta pendende.
        /// </summary>
        /// <param name="strEmail">Email da conta a eliminar.</param>
        /// <remarks></remarks>
        public void DeletePendingAccount(string strEmail)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                PendingAccount pendingAccount = context.PendingAccount.SingleOrDefault(p => p.Email == strEmail);

                context.PendingAccount.Remove(pendingAccount);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Insere uma nova conta na aplicação e retorna a chave primária associada à mesma.
        /// </summary>
        /// <param name="strEmail">Email</param>
        /// <param name="strPassword">Passowrd</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public long InsertAccountGetId(string strEmail, string strPassword)
        {
            Account account = new Account { Email = strEmail, Password = StrToArrByte(strPassword), AvatarUrl = "/images/avatars/user1.png" };

            InsertAccount(account);

            return account.IdAccount;
        }

        /// <summary>
        /// Adiciona um novo registo de uma morada.
        /// </summary>
        /// <param name="address">Morada</param>
        /// <remarks></remarks>
        public void InsertAddress(Address address)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                context.Add(address);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Cria uma nova conta.
        /// </summary>
        /// <param name="account">Conta</param>
        /// <remarks></remarks>
        public async void InsertAccount(Account account)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                context.Add(account);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Insere uma conta pendende na base de dados.
        /// </summary>
        /// <param name="strEmail">Email</param>
        /// <param name="enUserType">Tipo de Conta</param>
        /// <param name="intIsAdmin">Valor lógico que indica se é administrador, no caso de se tratar de uma conta tipo técnico do CIMOB</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool InsertPendingAccount(string strEmail, EnumAccountType enUserType, int intIsAdmin)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                if (context.PendingAccount.Where(p => p.Email == strEmail).Any())
                    return false;

                if (context.Account.Where(p => p.Email == strEmail).Any())
                    return false;

                Guid guid = Guid.NewGuid();

                PendingAccount pendingAccount = new PendingAccount { Email = strEmail, Guid = guid.ToString(), IsAdmin = Convert.ToBoolean(intIsAdmin) };

                context.Add(pendingAccount);
                context.SaveChanges();

                if (enUserType == EnumAccountType.STUDENT)
                {
                    SendEmailToStudent(strEmail, guid.ToString());
                }
                else
                {
                    SendEmailToTec(strEmail, guid.ToString());
                }

                return true;
            }
        }

        /// <summary>
        /// Insere um novo estudante na aplicação.
        /// </summary>
        /// <param name="student">EStudante</param>
        /// <param name="strEmail">Email do Estudante</param>
        /// <remarks></remarks>
        private async void InsertStudent(Student student, string strEmail)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                long lngId = await context.Account.Where(a => a.Email == strEmail).Select(a => a.IdAccount).SingleOrDefaultAsync();

                student.IdAccount = lngId;

                context.Add(student);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Insere um novo técnico do CIMOB na aplicação.
        /// </summary>
        /// <param name="technician">Técnico</param>
        /// <param name="strEmail">Email do Técnico</param>
        /// <remarks></remarks>
        private async void InsertTechnician(Technician technician, string strEmail)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                long lngId = await context.Account.Where(a => a.Email == strEmail).Select(a => a.IdAccount).SingleOrDefaultAsync();

                technician.IdAccount = lngId;

                context.Add(technician);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Envia um email ao técnico do CIMOB com a confirmação do pré-registo na aplicação com um link gerado aleatoriamente para a página de completação do registo.
        /// </summary>
        /// <param name="strEmailTec">Email do Técnico do CIMOB</param>
        /// <param name="strGuid">GUID para o link aleatoriamente gerado para a página de completação do registo.</param>
        /// <remarks></remarks>
        private void SendEmailToTec(string strEmailTec, string strGuid)
        {
            string strSubject = "[CIMOB-IPS] Registo no CIMOB-IPS";
            string strLink = "http://cimob-ips.azurewebsites.net/RegisterTechnician?account_id=" + strGuid;

            var strbBody = new StringBuilder();
            strbBody.AppendLine("Caro utilizador,<br><br>");
            strbBody.AppendFormat(@"O seu pedido de registo na plataforma do CIMOB-IPS foi aprovado.<br><br>");
            strbBody.AppendLine("Clique <a href=\"" + strLink + "\">aqui</a> para completar a criação da conta.<br>");
            strbBody.AppendLine("Caso não tenha efetuado nenhum pedido de ciração de conta, por favor, contacte a equipa do CIMOB-IPS");


            strbBody.AppendLine("Cumprimentos, <br> A Equipa do CIMOB-IPS.");
            Email.SendEmail(strEmailTec, strSubject, strbBody.ToString());
        }

        /// <summary>
        /// Envia um email com uma mensagem de boas vindas.
        /// </summary>
        /// <param name="strTargetEmail">Email de destino</param>
        /// <remarks></remarks>
        private void WelcomeEmail(string strTargetEmail)
        {
            string strSubject = "[CIMOB-IPS] Bem-Vindo ao CIMOB-IPS";
            string strLink = "http://cimob-ips.azurewebsites.net/Login";

            var strbBody = new StringBuilder();
            strbBody.AppendLine("Caro utilizador,<br><br>");
            strbBody.AppendFormat(@"O seu registo na plataforma do CIMOB-IPS foi efetuado com sucesso.<br><br>");
            strbBody.AppendLine("Pode entrar na aplicação através do seguinte <a href=\"" + strLink + "\">link</a>.");

            strbBody.AppendLine("Cumprimentos, <br> A Equipa do CIMOB-IPS.");
            Email.SendEmail(strTargetEmail, strSubject, strbBody.ToString());
        }

        /// <summary>
        /// Envia o email com a confirmação do pedido de registo na aplicação com um link gerado aleatoriamente para a página de completação do registo.
        /// </summary>
        /// <param name="strEmailStudent">Email do estudante</param>
        /// <param name="strGuid">GUID para o link aleatoriamente gerado para a página de completação do registo.</param>
        /// <remarks></remarks>
        private void SendEmailToStudent(string strEmailStudent, string strGuid)
        {
            string strSubject = "[CIMOB-IPS] Registo no CIMOB-IPS";
            string strLink = "http://cimob-ips.azurewebsites.net/RegisterStudent?account_id=" + strGuid;

            var strbBody = new StringBuilder();
            strbBody.AppendLine("Caro utilizador,<br><br>");
            strbBody.AppendFormat(@"O seu pedido de registo na plataforma do CIMOB-IPS foi aprovado.<br><br>");
            strbBody.AppendLine("Clique <a href=\"" + strLink + "\">aqui</a> para completar a criação da conta.<br>");
            strbBody.AppendLine("Caso não tenha efetuado nenhum pedido de criação de conta, por favor, contacte a equipa do CIMOB-IPS");


            strbBody.AppendLine("Cumprimentos, <br> A Equipa do CIMOB-IPS.");
            Email.SendEmail(strEmailStudent, strSubject, strbBody.ToString());
        }

        #endregion

        #region Login
        /// <summary>
        /// Retorna a view para a autenticação na aplicação.
        /// </summary>
        /// <returns>View Account/Login</returns>
        /// <remarks></remarks>
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["fyp-initial-display"] = "none";
            ViewData["initial-email"] = "";

            return View();
        }

        /// <summary>
        /// Método que executa uma auntenticação na aplicação. Redirecciona o utilizador para a página principal, caso o login seja efetuado com sucesso.
        /// </summary>
        /// <param name="model">LoginViewModel</param>
        /// <returns>View Account/Login ou View Home/Index</returns>
        /// <remarks></remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            string strEmail = model.Email;
            string strPassword = model.Password;

            if (ModelState.IsValid)
            {
                LoginState state = IsRegistered(strEmail, strPassword);

                if (state == LoginState.EMAIL_NOTFOUND || state == LoginState.CONNECTION_FAILED || state == LoginState.WRONG_PASSWORD) //Email não encontrado, ou password inválida
                {
                    ViewData["Login-Message"] = state.GetMessage();
                    ViewData["fyp-initial-display"] = "none";
                    ViewData["initial-email"] = strEmail;

                    return View("Login");
                }
                else
                {
                    string strAccountId = AccountID(strEmail);
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, strAccountId));
                    identity.AddClaim(new Claim(ClaimTypes.Name, AccountName(strAccountId)));

                    if (state == LoginState.CONNECTED_STUDENT)// Caso Estudante
                        identity.AddClaim(new Claim(ClaimTypes.Role, "estudante"));
                    else
                    {

                        if (IsAdmin(strAccountId) == "True") // Técnico do CIMOB Administrador
                            identity.AddClaim(new Claim(ClaimTypes.Role, "tecnico_admin"));
                        else // Técnico do CIMOB normal
                            identity.AddClaim(new Claim(ClaimTypes.Role, "tecnico"));
                    }

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = model.RememberMe });
                    identity.AddClaim(new Claim(ClaimTypes.GivenName, GetAvatarUrl(strAccountId)));

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewData["initial-email"] = strEmail;
            return View(model);
        }

        /// <summary>
        /// Retorna o caminho para a imagem de perfil de um utilizador.
        /// </summary>
        /// <param name="account_id">Chave primária de uma conta de utilizador</param>
        /// <returns>Caminho para a imagem de perfil de um utilizador</returns>
        /// <remarks></remarks>
        public string GetAvatarUrl(string account_id)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                return context.Account.Where(a => a.IdAccount.ToString() == account_id).FirstOrDefault().AvatarUrl;
            }
        }

        /// <summary>
        /// Executa um logout, redireciconando o utilizador para a página principal da aplicação.
        /// </summary>
        /// <returns>View Home/Index</returns>
        /// <remarks></remarks>
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region FYP
        /// <summary>
        /// Retorna a view com o formulário para resgatar a palavra-passe, em caso de esquecimento. 
        /// </summary>
        /// <param name="form">Formulário da view</param>
        /// <returns>View Login, com o formulário para resgatar a palavra-passe visível.</returns>
        /// <remarks></remarks>
        public IActionResult ForgotYourPassword(IFormCollection form)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            string strEmail = Convert.ToString(form["email"]);
            LoginState state = IsRegistered(strEmail, "");

            if (state == LoginState.EMAIL_NOTFOUND || state == LoginState.CONNECTION_FAILED)
            {
                ViewData["FYP-Message-Error"] = state.GetMessage();
                ViewData["FYP-Message"] = "";
                ViewData["fyp-initial-display"] = "block";
                ViewData["initial-email-fyp"] = strEmail;
                return View("Login");
            }
            else
            {
                SendFYPEmail(strEmail);
                ViewData["FYP-Message"] = "Password renovada. <br>Verifique a sua caixa de correio.";
                ViewData["FYP-Message-Error"] = "";
                ViewData["fyp-initial-display"] = "block";
                return View("Login");
            }
        }

        /// <summary>
        /// Atualiza o model da conta com a nova palavra-passe
        /// </summary>
        /// <param name="strEmail">Email</param>
        /// <param name="strPassword">Nova palavra-passe</param>
        /// <remarks></remarks>
        private async void ChangePassword(string strEmail, string strPassword)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                Account account = context.Account.SingleOrDefault(a => a.Email == strEmail);

                if (account != null)
                {
                    account.Password = StrToArrByte(strPassword);
                    context.Update(account);
                    await context.SaveChangesAsync();
                }
            }
        }

        private const int NEW_PW_MAX_LENGTH = 8;
        /// <summary>
        /// Gera uma nova palavra-passe com 8 caracteres aleatórios.
        /// </summary>
        /// <returns>Palavra-passe gerada convertida para string</returns>
        /// <remarks></remarks>
        private string GenerateNewPassword()
        {
            RNGCryptoServiceProvider newpw = new RNGCryptoServiceProvider();

            byte[] arrTokenBuffer = new byte[NEW_PW_MAX_LENGTH];
            newpw.GetBytes(arrTokenBuffer);
            return Convert.ToBase64String(arrTokenBuffer).ToLower();
        }

        /// <summary>
        /// Envia um email para o endereço, passado como argumento do método, com a informação da alteração da palavra-passe da conta.
        /// </summary>
        /// <param name="strEmail">Endereço email de destino</param>
        /// <remarks></remarks>
        private void SendFYPEmail(string strEmail)
        {
            string strNewPw = GenerateNewPassword();
            ChangePassword(strEmail, strNewPw);

            string strSubject = "[CIMOB-IPS] Alteração da palavra-passe";

            var strbBody = new StringBuilder();
            strbBody.AppendLine("Caro utilizador,<br><br>");
            strbBody.AppendFormat(@"Enviamos-lhe este email em resposta ao pedido de alteração da palavra-passe de acesso à plataforma do CIMOB-IPS.<br><br> A sua nova palavra-passe é: {0}<br><br>", strNewPw);
            strbBody.AppendLine("Caso não queira permanecer com a nova palavra-passe pode sempre alterá-la em: <a href=\"http://cimob-ips.azurewebsites.net/user/alterar_palavra_passe\"> cimob-ips.azurewebsites.net/user/alterar_palavra_passe </a> <br><br>");
            strbBody.AppendLine("Cumprimentos, <br> A Equipa do CIMOB-IPS.");

            Email.SendEmail(strEmail, strSubject, strbBody.ToString());
        }

        /// <summary>
        /// Retorna a chave primária associada à conta do utilizador autenticado no momento.
        /// </summary>
        /// <returns>Chave primária associada à conta do utilizador autenticado no momento</returns>
        /// <remarks></remarks>
        public int GetCurrentUserID()
        {
            return int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }


        /// <summary>
        /// Ação para mostrar a vista da página para atualizar a password.
        /// </summary>
        /// <returns>View Account/UpdatePassword</returns>
        /// <remarks></remarks>
        [HttpGet]
        public IActionResult UpdatePassword()
        {
            return View();
        }

        /// <summary>
        /// Atualiza a conta do utilizador com a nova password.
        /// </summary>
        /// <param name="model">Model da conta com os dados atualizados</param>
        /// <returns>View Account/UpdatePassword com uma mensagem com o resultado do pedido.</returns>
        /// <remarks></remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            string strCurrentPassword = model.CurrentPassword;
            string strConfirmation = model.Confirmation;
            string strNewPw = model.NewPassword;

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                Account account = await (from a in context.Account where a.IdAccount == GetCurrentUserID() select a).FirstOrDefaultAsync();

                if (account != null)
                {
                    string strBdpw = ToHex(account.Password, false);
                    if (!strBdpw.Equals(EncryptToMD5(strCurrentPassword)))
                    {
                        ViewData["UpdatePW-Error"] = "Password atual inválida";
                        return View("UpdatePassword");
                    }

                    ChangePassword(account.Email, strNewPw);
                    ViewData["UpdatePW-Message"] = "Password alterada com sucesso";
                }
                else
                {
                    ViewData["UpdatePW-Error"] = "Conta não existente";
                }
                //MANDAR EMAIL?
                //NOTIFICAÇÃO ?
                return View("UpdatePassword");
            }
        }

        #endregion

        /// <summary>
        /// Verifica se uma autenticação (email, password) é válida. 
        /// Caso seja retorna o estado LoginState.CONECTED_STUDENT, para o caso de um estudante, e LoginState.CONNECTED_TECH, para o case de um técnico.
        /// </summary>
        /// <param name="_strEmail">Email</param>
        /// <param name="_strPassword">Password</param>
        /// <returns>Enumerado LoginState que representa do resultado do login</returns>
        /// <remarks></remarks>
        public LoginState IsRegistered(string _strEmail, string _strPassword)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var accountWEmail = from a in context.Account where a.Email == _strEmail select a;
                var account = accountWEmail.FirstOrDefault();

                if (account != null)
                {
                    var strAccountID = account.IdAccount.ToString();

                    string strBDPW = ToHex(account.Password, false);

                    if (!strBDPW.Equals(EncryptToMD5(_strPassword)))
                        return LoginState.WRONG_PASSWORD;

                    if (AccountType(strAccountID) == EnumUserType.STUDENT)
                        return LoginState.CONNECTED_STUDENT;
                    else
                        return LoginState.CONNECTED_TECH;

                }
                else
                {
                    return LoginState.EMAIL_NOTFOUND;
                }
            }
        }

        /// <summary>
        /// Retorna a chave primária da conta que tem como registo o endereço email pasasdo como argumento.
        /// </summary>
        /// <param name="strEmail">Endereço email</param>
        /// <returns>Chave primária da conta</returns>
        /// <remarks></remarks>
        public string AccountID(string strEmail)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var accoundId = from a in context.Account where a.Email == strEmail select a;

                return accoundId.FirstOrDefault().IdAccount.ToString();
            }
        }

        /// <summary>
        /// Retorna um enumerado com o tipo de conta (Técnico ou Estudante) do registo que tem a chave primária passada como argumento
        /// </summary>
        /// <param name="_strAccountID">Chave primária da conta</param>
        /// <returns>Tipo de Conta</returns>
        /// <remarks></remarks>
        public EnumUserType AccountType(string _strAccountID)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var student = from s in context.Student where s.IdAccount == Int32.Parse(_strAccountID) select s;

                var isStudent = student.FirstOrDefault();
                if (isStudent != null)
                    return EnumUserType.STUDENT;

                return EnumUserType.TECHNICIAN;
            }
        }

        /// <summary>
        /// Retorna o nome de utilizador da conta que tem a chave primária passada como argumento
        /// </summary>
        /// <param name="_strAccountID">Chave primária da conta</param>
        /// <returns>Nome de utilizador da respetiva conta</returns>
        /// <remarks></remarks>
        public string AccountName(string _strAccountID)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var name = "";
                if (AccountType(_strAccountID) == EnumUserType.STUDENT)
                    name = (from s in context.Student where s.IdAccount == Int32.Parse(_strAccountID) select s.Name).FirstOrDefault().ToString();
                else
                    name = (from s in context.Technician where s.IdAccount == Int32.Parse(_strAccountID) select s.Name).FirstOrDefault().ToString();

                return name;
            }
        }

        /// <summary>
        /// Verifica se uma chave primária de uma conta, passada como argumento, pertence a um técnico do CIMOB correspondente a um administrador.
        /// </summary>
        /// <param name="intId">Chave primária da conta</param>
        /// <returns>Valor lógico resultante</returns>
        /// <remarks></remarks>
        public string IsAdmin(string _strAccountID)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var isAdmin = "";
                isAdmin = (from s in context.Technician where s.IdAccount == Int32.Parse(_strAccountID) select s.IsAdmin).FirstOrDefault().ToString();

                return isAdmin;
            }
        }

        /// <summary>
        /// Verifica se uma chave primária de uma conta, passada como argumento, pertence a um técnico do CIMOB.
        /// </summary>
        /// <param name="intId">Chave primária da conta</param>
        /// <returns>Valor lógico resultante</returns>
        /// <remarks></remarks>
        public bool IsTechnician(int intId)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                return context.Technician.Any(t => t.IdAccount == intId);
            }
        }

        /// <summary>
        /// Verifica se uma chave primária de uma conta, passada como argumento, pertence a um estudante.
        /// </summary>
        /// <param name="intId">Chave primária da conta</param>
        /// <returns>Valor lógico resultante</returns>
        /// <remarks></remarks>
        public bool IsStudent(int intId)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                return context.Student.Any(t => t.IdAccount == intId);
            }
        }

        /// <summary>
        /// Retorna o model do estudante que tem a chave primária passada como argumento
        /// </summary>
        /// <param name="intId">Chave primária da conta</param>
        /// <returns>Model do estudante</returns>
        /// <remarks></remarks>
        public long GetStudentId(int intId)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                return (from x in context.Student where x.IdAccount == intId select x.IdStudent).First();
            }
        }

        /// <summary>
        /// Encripta uma string para MD5
        /// </summary>
        /// <param name="strPassword">String a encriptar</param>
        /// <returns>String encriptada</returns>
        /// <remarks></remarks>
        public string EncryptToMD5(string strPassword)
        {
            byte[] result = StrToArrByte(strPassword);

            StringBuilder strBuilder = new StringBuilder();

            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Converte um vetor de bytes numa string em formato Hexadecimal
        /// </summary>
        /// <param name="bytes">N</param>
        /// <param name="upperCase">Se for <see langword="true" />, então distingue os caracteres maiúsculos ; caso contrário não distingue os caracteres maiúsculoss.</param>
        /// <returns>String resultante</returns>
        /// <remarks></remarks>
        public string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                stringBuilder.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Converte uma string num vetor de bytes
        /// </summary>
        /// <param name="str">String a converter</param>
        /// <returns>Vetor de bytes resultante</returns>
        /// <remarks></remarks>
        public byte[] StrToArrByte(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            Console.WriteLine(str);
            md5.ComputeHash(Encoding.ASCII.GetBytes(str));

            return md5.Hash;
        }
    }
}
