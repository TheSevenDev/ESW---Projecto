using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using CIMOB_IPS.Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CIMOB_IPS.Controllers
{
    public class UserController : Controller
    {
        private const int NEW_PW_MAX_LENGTH = 8;

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult RegisterStudent(IFormCollection form)
        {
            String email = GetEmailByIdPendingAccount("1"); //Id vem do url
            String password = Convert.ToString(form["Account.Password"]);

            long idAccount = InsertAccount(email, password);


            long studentNum = 150221014; //StudentNum pelo url
            long idCourse = Convert.ToInt64(form["Student.IdCourse"]);
            String address = Convert.ToString(form["Student.Address"].ToString());
            long ccNum = Convert.ToInt64(form["Student.Cc"]);
            long telephone = Convert.ToInt64(form["Student.Telephone"]);
            long idNacionality = Convert.ToInt64(form["Student.IdNationality"]);
            int credits = Convert.ToInt32(form["Student.Credits"]);

            Student student = new Student { IdAccount = idAccount, IdCourse = idCourse, Name = "Ricardo Fernandes", Address = address, Cc = ccNum, Telephone = telephone, IdNationality = idNacionality, Credits = credits, StudentNum = studentNum };

            InsertStudent(student);

            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            //WelcomeEmail(email);
            return View("/Views/Home/Index.cshtml");
        }

        public IActionResult RegisterTechnician(IFormCollection form)
        {
            String email = GetEmailByIdPendingAccount("1"); //Id vem do url
            String password = Convert.ToString(form["Account.Password"]);

            long idAccount = InsertAccount(email, password);


            bool isAdmin = true; //buscar IsAdmin pelo Url
            String name = Convert.ToString(form["Technician.Name"].ToString());
            long telephone = Convert.ToInt64(form["Technician.Telephone"]);

            Technician technician = new Technician { IdAccount = idAccount, Name = name, Telephone = telephone, IsAdmin = isAdmin };

            InsertTechnician(technician);


            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            //WelcomeEmail(email);
            return View();
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["fyp-initial-display"] = "none";
            ViewData["initial-email"] = "";

            return View();
        }

        [ActionName("Tecnicos")]
        public IActionResult Tecnicos()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View("Technicians");
        }

        public IActionResult InviteTec(IFormCollection form)
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("tecnico"))
                return RedirectToAction("Index", "Home");

            string destination = Convert.ToString(form["email"]);
            SendEmailToTec(destination);

            return View("Invite");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PreRegister(RegisterViewModel model)
        {

            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            long studentNumber = model.Student.StudentNum;
            String studentEmail = studentNumber.ToString() + "@estudantes.ips.pt";

            //if (ModelState.IsValid) { 
            try
            {
                InsertPreRegister(studentEmail, studentNumber);
                SendEmailToStudent(studentEmail);
                ViewData["message"] = "Email enviado.";
                return View("Register");
            }
            catch (SqlException e)
            {
                ViewData["message"] = "Conexção Falhada.";
            }
            //}
            ViewData["message"] = "";

            return View("Register");
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

        public IActionResult ExecFYP(IFormCollection form)
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

        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        public String GetEmailByIdPendingAccount(string idAccount)
        {
            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "SELECT email FROM dbo.Pending_Account WHERE id_pending=@idAccount";
                command.Parameters.AddWithValue("@idAccount", idAccount);
                connection.Open();

                String email = (String)command.ExecuteScalar();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }

                return email;

            }
        }

        public long InsertAccount(String email, String password)
        {
            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("INSERT INTO dbo.Account(Email,Password) OUTPUT INSERTED.id_account VALUES (@Email,CONVERT(VARBINARY(32), HashBytes('MD5', @Password), 2))", connection))
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

        public void InsertPreRegister(String studentEmail, long studentNumber)
        {
            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "INSERT INTO dbo.Pending_Account VALUES (@Email,@StudentNumber)";
                command.Parameters.AddWithValue("@Email", studentEmail);
                command.Parameters.AddWithValue("@StudentNumber", studentNumber);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void InsertStudent(Student student)
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

        public void InsertTechnician(Technician technician)
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

            string subject = "[CIMOB-IPS] Alteração da palavra-passe.";

            string body = "Enviamos-lhe este email em resposta ao pedido de alteração da palavra-passe de acesso à plataforma do CIMOB-IPS.<br><br> A sua nova palavra-passe é:" + newPW
                + "<br<br>>Caso não queira permanecer com a nova palavra-passe pode sempre alterá-la em: <a href=\"cimob-ips.azurewebsites.net/user/alterar_palavra_passe\"> cimob-ips.azurewebsites.net/user/alterar_palavra_passe </a>"
                + "<br><br> A Equipa do CIMOB-IPS.";

            Email.SendEmail(_email, subject, body);

        }

        private void SendEmailToTec(string emailTec)
        {
            string subject = "Convite para registo no CIMOB-IPS";

            string body = "Olá, <br> Recebeu um convite para se registar na aplicação do CIMOB-IPS.<br> " +
                "Clique <a href=\"www.google.pt\">aqui</a> para confirmar";

            Email.SendEmail(emailTec, subject, body);
        }

        private void WelcomeEmail(string targetEmail)
        {
            string subject = "Bem-Vindo ao CIMOB-IPS";

            string body = "Olá, <br> Pão Pão Pão Pão Pão Pão Pão Pão Pão Pão.<br> " +
                " Pão Pão Pão Pão Pão Pão Pão Pão Pão Pão Pão Pão Pão Pão Pão Pão Pão Pão";

        }


        public int GetCurrentUserID()
        {
            return int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        public IActionResult UpdatePassword()
        {
            return View();
        }


        [HttpPost]
        public IActionResult ChangePassword([Bind("CurrentPassword, NewPassword, Confirmation")] UpdatePasswordViewModel model)
        {

            string confirmation = Convert.ToString(model.Confirmation);
            string newPW = Convert.ToString(model.NewPassword);

            if (GetCurrentUserID() != model.IdAccount)
            {
                return BadRequest();
            }

            //não está a apresentar erros na pagina
            if (!confirmation.Equals(newPW))
            {
                ViewData["message"] = "As Passwords não coincidem";
            }

                { 


                using (SqlConnection sqlConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
                {
                    //create await  

                    using (SqlCommand command = sqlConnection.CreateCommand())
                    {
                        command.CommandText = "UPDATE Account SET password = @Password WHERE id_account = @IdAccount";
                        command.Parameters.AddWithValue("@Password", model.NewPassword);
                        sqlConnection.Open();
                        command.ExecuteNonQuery();
                        sqlConnection.Close();
                    }

                    //PASSWORD ALTERADA COM SUCESSO
                    ViewData["message"] = "Password Alterada Com Sucesso";

                }
                }

            return View("Home");
            }
        



        private void SendEmailToStudent(String emailStudent)
        {
            string subject = "Registo no CIMOB-IPS";

            string body = "Olá, <br> Para se registar na aplicação do CIMOB-IPS.<br> " +
                "Clique <a href=\"www.google.pt\">aqui</a>.";

            Email.SendEmail(emailStudent, subject, body);
        }


    }
    }