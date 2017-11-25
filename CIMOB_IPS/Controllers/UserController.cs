using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using CIMOB_IPS.Models;
using Microsoft.AspNetCore.Http;

namespace CIMOB_IPS.Controllers
{
    public class UserController : Controller
    {
        public string connectionString = @"Data Source=esw-cimob-db.database.windows.net;Database=CIMOB_IPS_DB;
                Integrated Security=False;User ID=adminUser; Password=f00n!l06;Connect Timeout=60;Encrypt=False;
                TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


        public IActionResult PreRegister(IFormCollection form)
        {
            String studentName = Convert.ToString(form["Student.Name"].ToString());
            long studentNumber = Convert.ToInt64(form["Student.StudentNum"].ToString());

            InsertPreRegister(studentName, studentNumber);

            return View("Index");
        }

        public IActionResult Register()
        {
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
                else { 

                    if(Account.IsAdmin(accountId) == "True")
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

        public void InsertPreRegister(String studentName, long studentNumber)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "insert into dbo.Pending_Account values (@Name,@StudentNumber)";
                command.Parameters.AddWithValue("@Name", studentName);
                command.Parameters.AddWithValue("@StudentNumber", studentNumber);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}