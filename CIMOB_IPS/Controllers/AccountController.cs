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
            ViewData["register-type"] = "student-preregister";
            return View();
        }

        [ActionName("Tecnicos")]
        public IActionResult Tecnicos()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View("Technicians", PopulateTechnicians());
        }

        private TechnicianManagementViewModel PopulateTechnicians()
        {
            var isAdmin = from s in _context.Technician where s.IdAccount == GetCurrentUserID() && s.IdTechnician == 1 select s;

            List<PendingAccount> pendingAccounts = null;
            if (isAdmin != null)
            {
                pendingAccounts = _context.PendingAccount.ToList();
            }

            TechnicianManagementViewModel viewModel = new TechnicianManagementViewModel { PendingAccounts = pendingAccounts };


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
            String email = model.EmailView;
            string password = model.PasswordView;

            long idAccount = 0;

            idAccount = InsertAccount(email, Account.EncryptToMD5(password));

            DeletePendingAccount(email);

            long studentNum = model.Student.StudentNum;
            string name = model.Student.Name;
            long idCourse = model.Student.IdCourse;
            String address = model.Student.Address;
            long ccNum = model.Student.Cc;
            long telephone = model.Student.Telephone;
            long idNationality = model.Student.IdNationality;
            int credits = model.Student.Credits;

            Student student = new Student { IdAccount = idAccount, IdCourse = idCourse, Name = name, Address = address, Cc = ccNum, Telephone = telephone, IdNationality = idNationality, Credits = credits, StudentNum = studentNum };

            InsertStudent(student);

            WelcomeEmail(email);
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public IActionResult RegisterTechnician(RegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            String email = model.EmailView;
            string password = model.PasswordView;

            long idAccount = 0;

            idAccount = InsertAccount(email, Account.EncryptToMD5(password));

            DeletePendingAccount(email);

            bool isAdmin = model.Technician.IsAdmin;
            string name = model.Technician.Name;
            long telephone = model.Technician.Telephone;


            Technician technician = new Technician { IdAccount = idAccount, Name = name, Telephone = telephone, IsAdmin = isAdmin};

            InsertTechnician(technician);


            WelcomeEmail(email);
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PreRegister(RegisterViewModel model)
        {

            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["register-type"] = "student-preregister";

            long studentNumber = model.Student.StudentNum;
            String studentEmail = studentNumber.ToString() + "@estudantes.ips.pt";

            try
            {
                bool success = InsertPendingAccount(studentEmail,EnumAccountType.STUDENT);
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

            List<SelectListItem> nationalities = new List<SelectListItem>();

            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "Select email from dbo.Pending_Account where guid = @guid";
                command.Parameters.AddWithValue("@guid", account_id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (!reader.HasRows)//Invalid GUID
                    return RedirectToAction("Index", "Home");
                else
                {
                    while (reader.Read())
                    {
                        string email = reader[0].ToString();
                        string studentNumber = reader[0].ToString().Substring(0, 9);

                        ViewData["student-email"] = email;
                        ViewData["student-number"] = studentNumber;

                    }
                    reader.Close();
                    connection.Close();
                }

            }
            return View("Register", new RegisterViewModel { Nationalities = PopulateNationalities(), Courses = PopulateCourses() });
        }

        private IEnumerable<SelectListItem> PopulateNationalities()
        {
            List<SelectListItem> nationalities = new List<SelectListItem>();
            var nationalitiesList = _context.Nationality.OrderBy(x => x.Description).ToList();
            foreach(Nationality n in nationalitiesList)
            {
                nationalities.Add(new SelectListItem { Value = n.IdNationality.ToString(), Text = n.Description });
            }
            return nationalities;
        }

        private IEnumerable<SelectListItem> PopulateCourses()
        {
            List<SelectListItem> courses = new List<SelectListItem>();
            var coursesList = _context.Course.OrderBy(x=>x.Name).ToList();
            foreach (Course n in coursesList)
            {
                courses.Add(new SelectListItem { Value = n.IdCourse.ToString(), Text = n.Name });
            }

            return courses;
        }

                             
        public IActionResult RegisterTechnician([FromQuery] string account_id)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["register-type"] = "technician";

            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "Select email, is_admin from dbo.Pending_Account where guid = @guid";
                command.Parameters.AddWithValue("@guid", account_id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (!reader.HasRows)//Invalid GUID
                    return RedirectToAction("Index", "Home");
                else
                {
                    while (reader.Read())
                    {
                        string email = reader[0].ToString();

                        ViewData["technician-email"] = email;
                        ViewData["technician-isAdmin"] = email;
                    }
                    reader.Close();
                    connection.Close();
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

            string destination = model.EmailView;
            bool isAdmin = model.IsAdmin;
            Guid guid;
            guid = Guid.NewGuid();

            try
            {
                bool success = InsertPendingAccount(destination, EnumAccountType.TECHNICIAN);
                if (success)
                {
                    ViewData["message"] = "Email enviado.";
                    ViewData["invite-tech-display"] = "block";
                    ViewData["error-message"] = "";
                }
                else
                {
                    ViewData["error-message"] = "Email já registado.";
                    ViewData["invite-tech-display"] = "block";
                    ViewData["invite-tech-email"] = destination;
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

        public void DeletePendingAccount(string email)
        {
            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "Delete FROM dbo.Pending_Account WHERE email=@Email";
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

            }
        }

        public long InsertAccount(string email, String password)
        {
            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("INSERT INTO dbo.Account(Email,Password) OUTPUT INSERTED.id_account VALUES (@Email, CONVERT(VARBINARY(16), @password, 2))", connection))
            {

                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);
                connection.Open();
                //command.ExecuteNonQuery();
                long idAccount = (Int64)command.ExecuteScalar();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }

                return idAccount;

            }

        }

        public bool InsertPendingAccount(String email, EnumAccountType userType)
        {
            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "Select a.email , pa.email from dbo.Account a, dbo.Pending_Account pa where a.email = @email or pa.email = @email";
                command.Parameters.AddWithValue("@email", email);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                    return false;
                else
                {
                    using (SqlCommand command2 = new SqlCommand("", connection))
                    {
                        connection.Close();
                        connection.Open();
                        command2.CommandText = "INSERT INTO dbo.Pending_Account VALUES (@email,@guid)";
                        command2.Parameters.AddWithValue("@email", email);
                        Guid guid;
                        guid = Guid.NewGuid();
                        command2.Parameters.AddWithValue("@guid", guid);
                        command2.ExecuteNonQuery();
                        connection.Close();
                        if (userType == EnumAccountType.STUDENT)
                        {
                            SendEmailToStudent(email, guid.ToString());
                        }
                        else
                        {
                            SendEmailToTec(email, guid.ToString());
                        }                        
                        return true;
                    }
                }
            }
        }

        private void InsertStudent(Student student)
        {
            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "INSERT INTO dbo.Student VALUES (@IdAccount,@IdCourse,@Name,@Adress,@CC,@Telephone,@IdNacionality,@Credits,@StudentNum)";
                command.Parameters.AddWithValue("@IdAccount", student.IdAccount);
                command.Parameters.AddWithValue("@IdCourse", student.IdCourse);
                command.Parameters.AddWithValue("@Name", student.Name);
                command.Parameters.AddWithValue("@Adress", student.Address);
                command.Parameters.AddWithValue("@CC", student.Cc);
                command.Parameters.AddWithValue("@Telephone", student.Telephone);
                command.Parameters.AddWithValue("@IdNacionality", student.IdNationality);
                command.Parameters.AddWithValue("@Credits", student.Credits);
                command.Parameters.AddWithValue("@StudentNum", student.StudentNum);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private void InsertTechnician(Technician technician)
        {
            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "INSERT INTO dbo.Technician VALUES (@IdAccount,@Name,@Telephone,@IsAdmin)";
                command.Parameters.AddWithValue("@IdAccount", technician.IdAccount);
                command.Parameters.AddWithValue("@Name", technician.Name);
                command.Parameters.AddWithValue("@Telephone", technician.Telephone);
                command.Parameters.AddWithValue("@IsAdmin", technician.IsAdmin);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private void SendEmailToTec(string emailTec, string guid)
        {
            string subject = "[CIMOB-IPS] Registo no CIMOB-IPS";
            string link = "http://cimob-ips.azurewebsites.net/RegisterTechnician?account_id=" + guid;

            var body = new StringBuilder();
            body.AppendLine("Caro utilizador,<br><br>");
            body.AppendFormat(@"O seu pedido de registo na plataforma do CIMOB-IPS foi aprovado.<br><br>");
            body.AppendLine("Clique <a href=\"" + link + "\">aqui</a> para completar a criação da conta.<br>");
            body.AppendLine("Caso não tenha efetuado nenhum pedido de ciração de conta, por favor, contacte a equipa do CIMOB-IPS");


            body.AppendLine("Cumprimentos, <br> A Equipa do CIMOB-IPS.");
            Email.SendEmail(emailTec, subject, body.ToString());
        }

        private void WelcomeEmail(string targetEmail)
        {
            string subject = "[CIMOB-IPS] Bem-Vindo ao CIMOB-IPS";

            var body = new StringBuilder();
            body.AppendLine("Caro utilizador,<br><br>");
            body.AppendFormat(@"O seu registo na plataforma do CIMOB-IPS foi efetuado com sucesso.<br><br>");
            body.AppendLine("Pode entrar na aplicação em <a href=\"http://cimob-ips.azurewebsites.net/Login </a>." );

            body.AppendLine("Cumprimentos, <br> A Equipa do CIMOB-IPS.");
            Email.SendEmail(targetEmail, subject, body.ToString());

        }


        private void SendEmailToStudent(string emailStudent, string guid)
        {
            string subject = "[CIMOB-IPS] Registo no CIMOB-IPS";
            string link = "http://cimob-ips.azurewebsites.net/RegisterStudent?account_id=" + guid;

            var body = new StringBuilder();
            body.AppendLine("Caro utilizador,<br><br>");
            body.AppendFormat(@"O seu pedido de registo na plataforma do CIMOB-IPS foi aprovado.<br><br>");
            body.AppendLine("Clique <a href=\""+link+"\">aqui</a> para completar a criação da conta.<br>");
            body.AppendLine("Caso não tenha efetuado nenhum pedido de criação de conta, por favor, contacte a equipa do CIMOB-IPS");


            body.AppendLine("Cumprimentos, <br> A Equipa do CIMOB-IPS.");
            Email.SendEmail(emailStudent, subject, body.ToString());
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
            string email = model.Email;
            string password = model.Password;

            if (ModelState.IsValid)
            {
                LoginState state = Account.IsRegistered(email, password);


                if (state == LoginState.EMAIL_NOTFOUND || state == LoginState.CONNECTION_FAILED || state == LoginState.WRONG_PASSWORD)
                {
                    ViewData["Login-Message"] = state.GetMessage();
                    ViewData["fyp-initial-display"] = "none";
                    ViewData["initial-email"] = email;

                    return View("Login");

                }
                else
                {
                    string accountId = Account.AccountID(email);
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, accountId));
                    identity.AddClaim(new Claim(ClaimTypes.Name, Account.AccountName(accountId)));
                    if (state == LoginState.CONNECTED_STUDENT)
                        identity.AddClaim(new Claim(ClaimTypes.Role, "estudante"));
                    else
                    {

                        if (Account.IsAdmin(accountId) == "True")
                            identity.AddClaim(new Claim(ClaimTypes.Role, "tecnico_admin"));
                        else
                            identity.AddClaim(new Claim(ClaimTypes.Role, "tecnico"));
                    }

                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = model.RememberMe });
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewData["initial-email"] = email;
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
            string email = Convert.ToString(form["email"]);
            LoginState state = Account.IsRegistered(email, "");


            if (state == LoginState.EMAIL_NOTFOUND || state == LoginState.CONNECTION_FAILED)
            {
                ViewData["FYP-Message-Error"] = state.GetMessage();
                ViewData["FYP-Message"] = "";
                ViewData["fyp-initial-display"] = "block";
                ViewData["initial-email-fyp"] = email;
                return View("Login");
            }
            else
            {
                SendFYPEmail(email);
                ViewData["FYP-Message"] = "Password renovada. <br>Verifique a sua caixa de correio.";
                ViewData["FYP-Message-Error"] = "";
                ViewData["fyp-initial-display"] = "block";
                return View("Login");
            }
        }

        private void ChangePassword(string _email, String _newpassword) {

            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "update dbo.Account set password = CONVERT(VARBINARY(16),@password, 2)  where email = @email";
                command.Parameters.AddWithValue("@password", _newpassword);
                command.Parameters.AddWithValue("@email", _email);
                connection.Open();
                command.ExecuteReader();
                connection.Close();
            }
        }

        private const int NEW_PW_MAX_LENGTH = 8;

        private string GenerateNewPassword()
        {
            RNGCryptoServiceProvider newpw = new RNGCryptoServiceProvider();

            byte[] tokenBuffer = new byte[NEW_PW_MAX_LENGTH];
            newpw.GetBytes(tokenBuffer);
            return Convert.ToBase64String(tokenBuffer);

        }

        private void SendFYPEmail(string _email)
        {
            string newPW = GenerateNewPassword();
            ChangePassword(_email, Account.EncryptToMD5(newPW));

            //SEND EMAIL WITH PASSWORD
            string subject = "[CIMOB-IPS] Alteração da palavra-passe";

            var body = new StringBuilder();
            body.AppendLine("Caro utilizador,<br><br>");
            body.AppendFormat(@"Enviamos-lhe este email em resposta ao pedido de alteração da palavra-passe de acesso à plataforma do CIMOB-IPS.<br><br> A sua nova palavra-passe é: {0}<br><br>", newPW);
            body.AppendLine("Caso não queira permanecer com a nova palavra-passe pode sempre alterá-la em: <a href=\"http://cimob-ips.azurewebsites.net/user/alterar_palavra_passe\"> cimob-ips.azurewebsites.net/user/alterar_palavra_passe </a> <br><br>");
            body.AppendLine("Cumprimentos, <br> A Equipa do CIMOB-IPS.");

            Email.SendEmail(_email, subject, body.ToString());

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
            string currentPassword = model.CurrentPassword;
            string confirmation = model.Confirmation;
            string newPW = model.NewPassword;

            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                connection.Open();
           
                command.CommandText = "select * from Account where id_account = @idaccount";
                command.Parameters.AddWithValue("@idaccount", GetCurrentUserID());
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string bdpw = Account.ToHex((byte[])reader[2], false);
                        if (!bdpw.Equals(Account.EncryptToMD5(currentPassword)))
                        {
                            ViewData["UpdatePW-Error"] = "Password atual inválida";
                            return View("UpdatePassword");
                        }

                    }
                }
            }
            using (SqlConnection sqlConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            {
                //create await  

                using (SqlCommand command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "update dbo.Account set password = CONVERT(VARBINARY(16),@password, 2) WHERE id_account = @idaccount";
                    command.Parameters.AddWithValue("@Password", Account.EncryptToMD5(newPW));
                    command.Parameters.AddWithValue("@idaccount", GetCurrentUserID());
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                    sqlConnection.Close();
                }

                //MANDAR EMAIL
                //PASSWORD ALTERADA COM SUCESSO
                ViewData["UpdatePW-Message"] = "Password alterada com sucesso";
                return View("UpdatePassword");
            }
        }


        private void SendEmailToStudent(string emailStudent)
        {
            string subject = "[CIMOB-IPS] Pedido de mudança de palavra-passe";

            var body = new StringBuilder();
            body.AppendLine("Caro utilizador,<br><br>");
            body.AppendFormat(@"O seu pedido de mudança de palavra-passe foi efetuado com sucesso.<br><br>");
            body.AppendLine("Caso não tenha efetuado nenhum pedido de aletaração da palavra-passe, por favor, contacte a equipa do CIMOB-IPS");


            body.AppendLine("Cumprimentos, <br> A Equipa do CIMOB-IPS.");
            Email.SendEmail(emailStudent, subject, body.ToString());
        }

        #endregion
    }
}