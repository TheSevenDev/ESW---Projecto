using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace CIMOB_IPS.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {

            return View();
        }

        public IActionResult Login()
        {
            
            return View();
           
        }

        [ActionName("ConvidarTecnico")]
        public IActionResult Invite()
        {
            return View("Invite");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

    
    }
}