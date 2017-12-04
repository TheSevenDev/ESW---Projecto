﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using CIMOB_IPS.Models;
using System.Security.Cryptography;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CIMOB_IPS.Controllers
{
    public class AccountController : Controller
    {
        private const int NEW_PW_MAX_LENGTH = 8;

        public IActionResult Register()
        {
            ViewData["pre-register-display"] = "block";
            ViewData["register-display"] = "none";
            return View();
        }


        [HttpPost]
        public IActionResult RegisterStudent(IFormCollection form)
        {
            String email = getEmailByIdPendingAccount("1"); //Id vem do url
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
            String email = getEmailByIdPendingAccount("1"); //Id vem do url
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

            ViewData["pre-register-display"] = "block";
            ViewData["register-display"] = "none";

            long studentNumber = model.Student.StudentNum;
            String studentEmail = studentNumber.ToString() + "@estudantes.ips.pt";

            try
            {
                bool success = InsertPreRegister(studentEmail);
                if(success)
                {
                    ViewData["message"] = "Número registado.";
                    ViewData["error-message"] = "";
                }
                else{
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

            ViewData["pre-register-display"] = "none";
            ViewData["register-display"] = "block";

            List<SelectListItem> nationalities = new List<SelectListItem>();

            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "Select email from dbo.Pending_Account where guid = @guid";
                command.Parameters.AddWithValue("@guid", account_id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if(!reader.HasRows)//Invalid GUID
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
                    using (SqlCommand command2 = new SqlCommand("", connection))
                    {
                        connection.Open();
                        command2.CommandText = "Select * from dbo.Nationality";
                        SqlDataReader reader2 = command2.ExecuteReader();
                        while (reader2.Read())
                        {
                            nationalities.Add(new SelectListItem { Value = reader2[0].ToString(), Text = (string)reader2[1] });
                        }

                    }
                    connection.Close();
                 }
                }
      
            ViewBag.Nationalities = nationalities;
            return View("Register" , new RegisterViewModel { Nationalities = nationalities });
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

        public String getEmailByIdPendingAccount(string idAccount)
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

        public bool InsertPreRegister(String studentEmail)
        {
            using (SqlConnection connection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "Select a.email , pa.email from dbo.Account a, dbo.Pending_Account pa where a.email = @email or pa.email = @email";
                command.Parameters.AddWithValue("@email", studentEmail);
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
                        command2.Parameters.AddWithValue("@email", studentEmail);
                        Guid guid;
                        guid = Guid.NewGuid();
                        command2.Parameters.AddWithValue("@guid", guid);
                        command2.ExecuteNonQuery();
                        connection.Close();
                        SendEmailToStudent(studentEmail, guid.ToString());
                        return true;
                    }
                }
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

            Email.SendEmail(targetEmail, subject, body);
        }

        private void SendEmailToStudent(String emailStudent, String guid)
        {
            string subject = "Registo no CIMOB-IPS";
            string link = "cimob-ips.azurewebsites.net/RegisterStudent?account_id=" + guid;

            string body = "Olá, <br> Clique <a href =\"" + link + "\">aqui</a> para se registar na aplicação do CIMOB-IPS.<br> ";
               
            Email.SendEmail(emailStudent, subject, body);
        }


    }
}