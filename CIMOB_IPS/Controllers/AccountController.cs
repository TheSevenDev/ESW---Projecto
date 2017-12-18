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

namespace CIMOB_IPS.Controllers
{
    public class AccountController : Controller
    {
        private readonly CIMOB_IPS_DBContext _context;

        public AccountController(CIMOB_IPS_DBContext context)
        {
            _context = context;
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
        public IActionResult Technicians()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            return View("Technicians", PopulateTechnicians());
        }

        private TechnicianManagementViewModel PopulateTechnicians()
        {

            var lisIdAdmin = from s in _context.Technician where s.IdAccount == GetCurrentUserID() && s.IdTechnician == 1 select s;

            List<PendingAccount> lisPendingAccounts = null;

            if (lisIdAdmin != null)
            {
                lisPendingAccounts = _context.PendingAccount.ToList();
            }

            TechnicianManagementViewModel viewModel = new TechnicianManagementViewModel { PendingAccounts = lisPendingAccounts };

            viewModel.Technicians = (from s in _context.Technician
                                        select new Technician
                                        {
                                            Name = s.Name,
                                            IdAccountNavigation = new Account { Email = s.IdAccountNavigation.Email },
                                            Telephone = s.Telephone,
                                            IsAdmin = s.IsAdmin
                                        }).ToList();

            return viewModel;
        }

        [HttpPost]
        public IActionResult RegisterStudent(RegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            Console.WriteLine(ModelState);
            string strEmail = model.EmailView;
            string strPassword = model.PasswordView;

            long lngIdAccount = 0;

            lngIdAccount = InsertAccount(strEmail, Account.EncryptToMD5(strPassword));

            DeletePendingAccount(strEmail);

            long lngStudentNum = model.Student.StudentNum;
            string strName = model.Student.Name;
            long lngIdCourse = model.Student.IdCourse;
            String strAddress = model.Student.Address;
            long lngCcNum = model.Student.Cc;
            long lngTelephone = model.Student.Telephone;
            long lngIdNationality = model.Student.IdNationality;
            int intCredits = model.Student.Credits;

            Student student = new Student { IdAccount = lngIdAccount, IdCourse = lngIdCourse, Name = strName, Address = strAddress, Cc = lngCcNum, Telephone = lngTelephone, IdNationality = lngIdNationality, Credits = intCredits, StudentNum = lngStudentNum };

            InsertStudent(student);

            WelcomeEmail(strEmail);
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public IActionResult RegisterTechnician(RegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            String strEmail = model.EmailView;
            string strPassword = model.PasswordView;

            long lngIdAccount = 0;

            lngIdAccount = InsertAccount(strEmail, Account.EncryptToMD5(strPassword));

            DeletePendingAccount(strEmail);

            bool bolIsAdmin = model.Technician.IsAdmin;
            string strName = model.Technician.Name;
            long lngTelephone = model.Technician.Telephone;

            Technician technician = new Technician { IdAccount = lngIdAccount, Name = strName, Telephone = lngTelephone, IsAdmin = bolIsAdmin};

            InsertTechnician(technician);

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
                bool success = InsertPendingAccount(strStudentEmail,EnumAccountType.STUDENT, 0);
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

        public IActionResult RegisterStudent([FromQuery] string account_id)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["register-type"] = "student-register";

            List<SelectListItem> lisNationalities = new List<SelectListItem>();

            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand scmCommand = new SqlCommand("", scnConnection))
            {
                scmCommand.CommandText = "Select email from dbo.Pending_Account where guid = @guid";
                scmCommand.Parameters.AddWithValue("@guid", account_id);
                scnConnection.Open();

                SqlDataReader dtrReader = scmCommand.ExecuteReader();

                if (!dtrReader.HasRows)//Invalid GUID
                    return RedirectToAction("Index", "Home");
                else
                {
                    while (dtrReader.Read())
                    {
                        string strEmail = dtrReader[0].ToString();
                        string strStudentNumber = dtrReader[0].ToString().Substring(0, 9);

                        ViewData["student-email"] = strEmail;
                        ViewData["student-number"] = strStudentNumber;

                    }

                    dtrReader.Close();
                    scnConnection.Close();
                }
            }

            return View("Register", new RegisterViewModel { Nationalities = PopulateNationalities(), Courses = PopulateCourses() });
        }

        private IEnumerable<SelectListItem> PopulateNationalities()
        {
            List<SelectListItem> lisNationalities = new List<SelectListItem>();

            var listNationalities = _context.Nationality.OrderBy(x => x.Description).ToList();

            foreach(Nationality n in listNationalities)
            {
                lisNationalities.Add(new SelectListItem { Value = n.IdNationality.ToString(), Text = n.Description });
            }

            return lisNationalities;
        }

        private IEnumerable<SelectListItem> PopulateCourses()
        {
            List<SelectListItem> lisCourses = new List<SelectListItem>();

            var listCourses = _context.Course.OrderBy(x=>x.Name).ToList();

            foreach (Course n in listCourses)
            {
                lisCourses.Add(new SelectListItem { Value = n.IdCourse.ToString(), Text = n.Name });
            }

            return lisCourses;
        }
                             
        public IActionResult RegisterTechnician([FromQuery] string account_id)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["register-type"] = "technician";

            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand scmCommand = new SqlCommand("", scnConnection))
            {
                scmCommand.CommandText = "Select email, is_admin from dbo.Pending_Account where guid = @guid";
                scmCommand.Parameters.AddWithValue("@guid", account_id);
                scnConnection.Open();

                SqlDataReader dtrReader = scmCommand.ExecuteReader();

                if (!dtrReader.HasRows)//Invalid GUID
                    return RedirectToAction("Index", "Home");
                else
                {
                    while (dtrReader.Read())
                    {
                        string strEmail = dtrReader[0].ToString();
                        bool bolIsAdmin = Convert.ToBoolean(dtrReader[1].ToString());

                        ViewData["technician-email"] = strEmail;
                        ViewData["technician-isAdmin"] = bolIsAdmin;
                    }

                    dtrReader.Close();
                    scnConnection.Close();
                }
            }

            return View("Register", new RegisterViewModel { EmailView = ViewData["technician-email"].ToString(), Technician = new Technician { IsAdmin = (bool)ViewData["technician-isAdmin"] } });
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

                return View("Technicians", PopulateTechnicians());
            }
            catch (SqlException e)
            {
                ViewData["invite-tech-display"] = "block";
                ViewData["error-message"] = "Conexão Falhada.";
            }

            return View("Technicians", PopulateTechnicians());
        }

        public void DeletePendingAccount(string strEmail)
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand scmCommand = new SqlCommand("", scnConnection))
            {
                scmCommand.CommandText = "Delete FROM dbo.Pending_Account WHERE email=@Email";
                scmCommand.Parameters.AddWithValue("@Email", strEmail);
                scnConnection.Open();
                scmCommand.ExecuteNonQuery();
                scnConnection.Close();
            }
        }

        public long InsertAccount(string strEmail, string strPassword)
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand scmCommand = new SqlCommand("INSERT INTO dbo.Account(Email,Password) OUTPUT INSERTED.id_account VALUES (@Email, CONVERT(VARBINARY(16), @password, 2))", scnConnection))
            {
                scmCommand.Parameters.AddWithValue("@Email", strEmail);
                scmCommand.Parameters.AddWithValue("@Password", strPassword);
                scnConnection.Open();
                //command.ExecuteNonQuery();

                long lngIdAccount = (Int64)scmCommand.ExecuteScalar();

                if (scnConnection.State == System.Data.ConnectionState.Open)
                {
                    scnConnection.Close();
                }

                return lngIdAccount;
            }
        }

        public bool InsertPendingAccount(string strEmail, EnumAccountType enUserType, int intIsAdmin)
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand scmCommand = new SqlCommand("", scnConnection))
            {
                scmCommand.CommandText = "Select a.email , pa.email from dbo.Account a, dbo.Pending_Account pa where a.email = @email or pa.email = @email";
                scmCommand.Parameters.AddWithValue("@email", strEmail);
                scnConnection.Open();
                SqlDataReader dtrReader = scmCommand.ExecuteReader();
                if (dtrReader.HasRows)
                    return false;
                else
                {
                    using (SqlCommand scmCommand2 = new SqlCommand("", scnConnection))
                    {
                        scnConnection.Close();
                        scnConnection.Open();
                        scmCommand2.CommandText = "INSERT INTO dbo.Pending_Account VALUES (@email,@guid, @isAdmin)";
                        scmCommand2.Parameters.AddWithValue("@email", strEmail);
                        Guid guid;
                        guid = Guid.NewGuid();
                        scmCommand2.Parameters.AddWithValue("@guid", guid);
                        scmCommand2.Parameters.AddWithValue("@isAdmin", intIsAdmin);
                        scmCommand2.ExecuteNonQuery();
                        scnConnection.Close();

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
            }
        }

        private void InsertStudent(Student student)
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand scmCommand = new SqlCommand("", scnConnection))
            {
                scmCommand.CommandText = "INSERT INTO dbo.Student VALUES (@IdAccount,@IdCourse,@Name,@Adress,@CC,@Telephone,@IdNacionality,@Credits,@StudentNum)";
                scmCommand.Parameters.AddWithValue("@IdAccount", student.IdAccount);
                scmCommand.Parameters.AddWithValue("@IdCourse", student.IdCourse);
                scmCommand.Parameters.AddWithValue("@Name", student.Name);
                scmCommand.Parameters.AddWithValue("@Adress", student.Address);
                scmCommand.Parameters.AddWithValue("@CC", student.Cc);
                scmCommand.Parameters.AddWithValue("@Telephone", student.Telephone);
                scmCommand.Parameters.AddWithValue("@IdNacionality", student.IdNationality);
                scmCommand.Parameters.AddWithValue("@Credits", student.Credits);
                scmCommand.Parameters.AddWithValue("@StudentNum", student.StudentNum);
                scnConnection.Open();
                scmCommand.ExecuteNonQuery();
                scnConnection.Close();
            }
        }

        private void InsertTechnician(Technician technician)
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand scmCommand = new SqlCommand("", scnConnection))
            {
                scmCommand.CommandText = "INSERT INTO dbo.Technician VALUES (@IdAccount,@Name,@Telephone,@IsAdmin)";
                scmCommand.Parameters.AddWithValue("@IdAccount", technician.IdAccount);
                scmCommand.Parameters.AddWithValue("@Name", technician.Name);
                scmCommand.Parameters.AddWithValue("@Telephone", technician.Telephone);
                scmCommand.Parameters.AddWithValue("@IsAdmin", technician.IsAdmin);
                scnConnection.Open();
                scmCommand.ExecuteNonQuery();
                scnConnection.Close();
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
            strbBody.AppendLine("Clique <a href=\""+strLink+"\">aqui</a> para completar a criação da conta.<br>");
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
                LoginState state = Account.IsRegistered(strEmail, strPassword);

                if (state == LoginState.EMAIL_NOTFOUND || state == LoginState.CONNECTION_FAILED || state == LoginState.WRONG_PASSWORD)
                {
                    ViewData["Login-Message"] = state.GetMessage();
                    ViewData["fyp-initial-display"] = "none";
                    ViewData["initial-email"] = strEmail;

                    return View("Login");
                }
                else
                {
                    string strAccountId = Account.AccountID(strEmail);
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, strAccountId));
                    identity.AddClaim(new Claim(ClaimTypes.Name, Account.AccountName(strAccountId)));

                    if (state == LoginState.CONNECTED_STUDENT)
                        identity.AddClaim(new Claim(ClaimTypes.Role, "estudante"));
                    else
                    {

                        if (Account.IsAdmin(strAccountId) == "True")
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
            LoginState state = Account.IsRegistered(strEmail, "");

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

        private void ChangePassword(string strEmail, string strPassword)
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand scmCommand = new SqlCommand("", scnConnection))
            {
                scmCommand.CommandText = "update dbo.Account set password = CONVERT(VARBINARY(16),@password, 2)  where email = @email";
                scmCommand.Parameters.AddWithValue("@password", strPassword);
                scmCommand.Parameters.AddWithValue("@email", strEmail);
                scnConnection.Open();
                scmCommand.ExecuteReader();
                scnConnection.Close();
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
            ChangePassword(strEmail, Account.EncryptToMD5(strNewPw));

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
        public IActionResult UpdatePassword(UpdatePasswordViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            string strCurrentPassword = model.CurrentPassword;
            string strConfirmation = model.Confirmation;
            string strNewPw = model.NewPassword;

            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand scmCommand = new SqlCommand("", scnConnection))
            {
                scnConnection.Open();
           
                scmCommand.CommandText = "select * from Account where id_account = @idaccount";
                scmCommand.Parameters.AddWithValue("@idaccount", GetCurrentUserID());
                SqlDataReader dtrReader = scmCommand.ExecuteReader();
                if (dtrReader.HasRows)
                {
                    while (dtrReader.Read())
                    {
                        string strBdpw = Account.ToHex((byte[])dtrReader[2], false);
                        if (!strBdpw.Equals(Account.EncryptToMD5(strCurrentPassword)))
                        {
                            ViewData["UpdatePW-Error"] = "Password atual inválida";
                            return View("UpdatePassword");
                        }
                    }
                }
            }
            using (SqlConnection scnConnection2 = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            {
                //create await  
                using (SqlCommand scmCommand = scnConnection2.CreateCommand())
                {
                    scmCommand.CommandText = "update dbo.Account set password = CONVERT(VARBINARY(16),@password, 2) WHERE id_account = @idaccount";
                    scmCommand.Parameters.AddWithValue("@Password", Account.EncryptToMD5(strNewPw));
                    scmCommand.Parameters.AddWithValue("@idaccount", GetCurrentUserID());
                    scnConnection2.Open();
                    scmCommand.ExecuteNonQuery();
                    scnConnection2.Close();
                }

                //MANDAR EMAIL
                //PASSWORD ALTERADA COM SUCESSO
                ViewData["UpdatePW-Message"] = "Password alterada com sucesso";
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
    }
}