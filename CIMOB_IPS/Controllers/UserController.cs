using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
<<<<<<< HEAD
using CIMOB_IPS.Models;
=======
>>>>>>> 0f3cf9c2935a87209e9057cd4c593c9dad49685f
using Microsoft.AspNetCore.Http;

namespace CIMOB_IPS.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
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

    
    }
}