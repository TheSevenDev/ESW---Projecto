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

namespace CIMOB_IPS.Controllers
{
    public class UserController : Controller
    {

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult PreRegister(IFormCollection form)
        {
            long studentNumber = Convert.ToInt64(form["Student.StudentNum"]);
            String studentEmail = studentNumber + "@estudantes.ips.pt";

            InsertPreRegister(studentEmail, studentNumber);
            SendEmailToStudent(studentEmail);

            return View("Index");
        }

        public IActionResult RegisterStudent(IFormCollection form)
        {
            String email = getEmailByIdPendingAccount("1"); //Id vem do url
            String password = Convert.ToString(form["Account.Password"]);

            long idAccount = InsertAccount(email, password);

            //buscar Name e StudentNum pelo url
            long idCourse = Convert.ToInt64(form["Student.IdCourse"]);
            String address = Convert.ToString(form["Student.Address"].ToString());
            long ccNum = Convert.ToInt64(form["Student.Cc"]);
            long telephone = Convert.ToInt64(form["Student.Telephone"]);
            long idNacionality = Convert.ToInt64(form["Student.IdNacionality"]);
            int credits = Convert.ToInt32(form["Student.Credits"]);

            Student student = new Student {IdAccount = idAccount, IdCourse = idCourse, Name = "Ricardo Fernandes", Address = address, Cc = ccNum, Telephone = telephone, IdNationality = idNacionality, Credits = credits, StudentNum = 150221014 };

            InsertStudent(student);

            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            WelcomeEmail(email);
            return View("/Views/Home/Index.cshtml");
        }

        public IActionResult RegisterTechnician(IFormCollection form)
        {
            String email = getEmailByIdPendingAccount("1"); //Id vem do url
            String password = Convert.ToString(form["Account.Password"]);

            long idAccount = InsertAccount(email, password);
            //buscar IsAdmin pelo Url

            String name = Convert.ToString(form["Technician.Name"].ToString());
            long telephone = Convert.ToInt64(form["Technician.Telephone"]);

            Technician technician = new Technician {IdAccount=idAccount, Name=name,Telephone=telephone,IsAdmin=true};

            InsertTechnician(technician);


            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");


            return View();
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

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

            Email.SendEmail(targetEmail, subject, body);
        }

        private void SendEmailToStudent(String emailStudent)
        {
            string subject = "Registo no CIMOB-IPS";

            string body = "Olá, <br> Para se registar na aplicação do CIMOB-IPS.<br> " +
                "Clique <a href=\"www.google.pt\">aqui</a>.";

            Email.SendEmail(emailStudent, subject, body);
        }

        public async Task<IActionResult> ExecLoginAsync(IFormCollection form)
        {
            string email = Convert.ToString(form["email"]);
            string password = Convert.ToString(form["password"]);
            LoginState state = Account.IsRegistered(email, password);


            if (state == LoginState.EMAIL_NOTFOUND || state == LoginState.CONNECTION_FAILED || state == LoginState.WRONG_PASSWORD)
            {
                ViewData["Login-Message"] = state.GetMessage();
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
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        public String getEmailByIdPendingAccount(string idAccount)
        {
            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "Select email From dbo.Pending_Account Where id_pending=@idAccount";
                command.Parameters.AddWithValue("@idAccount", idAccount);
                connection.Open();
                
                String email = (String) command.ExecuteScalar();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }

                return email;

            }
        }

        public long InsertAccount(String email, String password)
        {
            password = Account.EncryptToMD5(password);
            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "insert into dbo.Account values (@Email,@Password); Select SCOPE_IDENTITY()";
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.Add("@ID", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                connection.Open();

                command.ExecuteNonQuery();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }

                return Convert.ToInt64(command.Parameters["@ID"].Value);
            }
        }

        public void InsertPreRegister(String studentEmail, long studentNumber)
        {
            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "insert into dbo.Pending_Account values (@Email,@StudentNumber)";
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
                command.CommandText = "insert into dbo.Student values (@IdAccount,@IdCourse,@Name,@Adress,@CC,@Telephone,@IdNacionality,@Credits,@StudentNum)";
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
                command.CommandText = "insert into dbo.Technician values (@IdAccount,@Name,@Telephone,@IsAdmin)";
                command.Parameters.AddWithValue("@IdAccount", technician.IdAccount);
                command.Parameters.AddWithValue("@Name", technician.Name);
                command.Parameters.AddWithValue("@Telephone", technician.Telephone);
                command.Parameters.AddWithValue("@IsAdmin", technician.IsAdmin);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}