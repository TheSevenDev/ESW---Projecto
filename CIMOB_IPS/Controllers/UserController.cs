﻿using System;
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

            return View();
        }

        public  IActionResult Login(IFormCollection form)
        {
            
            string email = Convert.ToString(form["email"]);
            string password = Convert.ToString(form["password"]);
            if (Account.IsRegistered(email, password)){
                LoginController lc = new LoginController();
                lc.LoginAsStud();
            }
            return View("Login");
        }

        [ActionName("ConvidarTecnico")]
        public IActionResult Invite()
        {
            return View("Invite");
        }

        public IActionResult InviteTec(IFormCollection form)
        {
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


        public async Task<IActionResult> Logout()
        {
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