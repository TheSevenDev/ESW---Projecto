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
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        #region Register
        /// <summary>
        ///  
        /// </summary>
        /// <returns>View Register/Index</returns>
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["register-type"] = "student-preregister";
            return View();
        }

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


        public ContentResult Address([FromQuery] string code)
        {
            if (!String.IsNullOrEmpty(code))
            {
                var url = "http://www.ctt.pt/pdcp/xml_pdcp?incodpos=" + code;
                var xmlDoc = new XmlDocument();       
                xmlDoc.Load(url);
                return Content(XmlToString(xmlDoc), "application/xml", Encoding.UTF8);             
            }
            return Content("" , "");
        }

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

        private async Task<PaginatedList<PendingAccount>> GetPendingAccountsPaginated(int intPendingPageNumber, int intPendingPageSize)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var pendingAccounts = from s in context.PendingAccount
                                      select s;

                return await PaginatedList<PendingAccount>.CreateAsync(pendingAccounts.AsNoTracking(), intPendingPageNumber, intPendingPageSize);
            }
        }

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

        [HttpPost]
        public IActionResult RegisterStudent(RegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            //Console.WriteLine(ModelState);

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

            Technician technician = new Technician { IdAccount = lngIdAccount, Name = strName, Telephone = lngTelephone, IsAdmin = bolIsAdmin };

            InsertTechnician(technician, strEmail);

            WelcomeEmail(strEmail);
            return RedirectToAction("Login", "Account");
        }

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
                    ViewData["message"] = "Número registado.";
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

        public IActionResult RegisterStudent([FromQuery] string account_id, bool? birthDateError)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["register-type"] = "student-register";

            if(birthDateError != null)
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

        private IEnumerable<SelectListItem> PopulateCourses()
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                List<SelectListItem> lisCourses = new List<SelectListItem>();

                var listCourses = context.Course
                    .Where(x => (from i in context.Institution where i.IdNationality == 
                                 (from n in context.Nationality where n.Description == "PORTUGAL" select n.IdNationality).FirstOrDefault() select i.IdInstitution).Contains(x.IdInstitution))
                    .OrderBy(x => x.Name).ToList();

                foreach (Course n in listCourses)
                {
                    lisCourses.Add(new SelectListItem { Value = n.IdCourse.ToString(), Text = n.Name });
                }

                return lisCourses;
            }
        }

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

        public IActionResult InviteTec()
        {
            return View("InviteTechnician");
        }

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

                return View("Technicians", PopulateTechnicians(1, 1, 3));
            }
            catch (SqlException e)
            {
                ViewData["invite-tech-display"] = "block";
                ViewData["error-message"] = "Conexão Falhada.";
            }

            return View("Technicians", PopulateTechnicians(1, 1, 3));
        }

        public void DeletePendingAccount(string strEmail)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                PendingAccount pendingAccount = context.PendingAccount.SingleOrDefault(p => p.Email == strEmail);

                context.PendingAccount.Remove(pendingAccount);
                context.SaveChanges();
            }
        }

        public long InsertAccountGetId(string strEmail, string strPassword)
        {
            Account account = new Account { Email = strEmail, Password = StrToArrByte(strPassword) };

            InsertAccount(account);

            return account.IdAccount;
        }

        public void InsertAddress(Address address)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                context.Add(address);
                context.SaveChanges();
            }
        }

        public async void InsertAccount(Account account)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                context.Add(account);
                await context.SaveChangesAsync();
            }
        }

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
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["fyp-initial-display"] = "none";
            ViewData["initial-email"] = "";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            string strEmail = model.Email;
            string strPassword = model.Password;

            if (ModelState.IsValid)
            {
                LoginState state = IsRegistered(strEmail, strPassword);

                if (state == LoginState.EMAIL_NOTFOUND || state == LoginState.CONNECTION_FAILED || state == LoginState.WRONG_PASSWORD)
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

                    if (state == LoginState.CONNECTED_STUDENT)
                        identity.AddClaim(new Claim(ClaimTypes.Role, "estudante"));
                    else
                    {

                        if (IsAdmin(strAccountId) == "True")
                            identity.AddClaim(new Claim(ClaimTypes.Role, "tecnico_admin"));
                        else
                            identity.AddClaim(new Claim(ClaimTypes.Role, "tecnico"));
                    }

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = model.RememberMe });
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewData["initial-email"] = strEmail;
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region FYP
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

        private string GenerateNewPassword()
        {
            RNGCryptoServiceProvider newpw = new RNGCryptoServiceProvider();

            byte[] arrTokenBuffer = new byte[NEW_PW_MAX_LENGTH];
            newpw.GetBytes(arrTokenBuffer);
            return Convert.ToBase64String(arrTokenBuffer).ToLower();
        }

        private void SendFYPEmail(string strEmail)
        {
            string strNewPw = GenerateNewPassword();
            ChangePassword(strEmail, strNewPw);

            //SEND EMAIL WITH PASSWORD
            string strSubject = "[CIMOB-IPS] Alteração da palavra-passe";

            var strbBody = new StringBuilder();
            strbBody.AppendLine("Caro utilizador,<br><br>");
            strbBody.AppendFormat(@"Enviamos-lhe este email em resposta ao pedido de alteração da palavra-passe de acesso à plataforma do CIMOB-IPS.<br><br> A sua nova palavra-passe é: {0}<br><br>", strNewPw);
            strbBody.AppendLine("Caso não queira permanecer com a nova palavra-passe pode sempre alterá-la em: <a href=\"http://cimob-ips.azurewebsites.net/user/alterar_palavra_passe\"> cimob-ips.azurewebsites.net/user/alterar_palavra_passe </a> <br><br>");
            strbBody.AppendLine("Cumprimentos, <br> A Equipa do CIMOB-IPS.");

            Email.SendEmail(strEmail, strSubject, strbBody.ToString());
        }

        public int GetCurrentUserID()
        {
            return int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        [HttpGet]
        public IActionResult UpdatePassword()
        {
            return View();
        }

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

                //MANDAR EMAIL
                //PASSWORD ALTERADA COM SUCESSO
                return View("UpdatePassword");
            }
        }

        private void SendEmailToStudent(string strEmailStudent)
        {
            string strSubject = "[CIMOB-IPS] Pedido de mudança de palavra-passe";

            var strbBody = new StringBuilder();
            strbBody.AppendLine("Caro utilizador,<br><br>");
            strbBody.AppendFormat(@"O seu pedido de mudança de palavra-passe foi efetuado com sucesso.<br><br>");
            strbBody.AppendLine("Caso não tenha efetuado nenhum pedido de aletaração da palavra-passe, por favor, contacte a equipa do CIMOB-IPS");


            strbBody.AppendLine("Cumprimentos, <br> A Equipa do CIMOB-IPS.");
            Email.SendEmail(strEmailStudent, strSubject, strbBody.ToString());
        }

        #endregion

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


        public string AccountID(string strEmail)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var accoundId = from a in context.Account where a.Email == strEmail select a;

                return accoundId.FirstOrDefault().IdAccount.ToString();
            }
        }

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

        public string IsAdmin(string _strAccountID)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var isAdmin = "";
                isAdmin = (from s in context.Technician where s.IdAccount == Int32.Parse(_strAccountID) select s.IsAdmin).FirstOrDefault().ToString();

                return isAdmin;
            }
        }

        public bool IsTechnician(int intId)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                return context.Technician.Any(t => t.IdAccount == intId);
            }
        }

        public bool IsStudent(int intId)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                return context.Student.Any(t => t.IdAccount == intId);
            }
        }

        public long GetStudentId(int intId)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                return (from x in context.Student where x.IdAccount == intId select x.IdStudent).First();
            }
        }

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

        public string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                stringBuilder.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return stringBuilder.ToString();
        }

        public byte[] StrToArrByte(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            Console.WriteLine(str);
            md5.ComputeHash(Encoding.ASCII.GetBytes(str));

            return md5.Hash;
        }
    }
}
