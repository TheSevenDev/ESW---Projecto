using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace CIMOB_IPS.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            HomeController home = new HomeController();
            return home.Index();
        }

        public async Task<IActionResult> LoginAsTec()
        {
            if (ModelState.IsValid)
            {
                var isValid = true; // TODO Validate the username and the password with your own logic

                if (!isValid)
                {
                    ModelState.AddModelError("", "username or password is invalid");
                    return Index();
                }

                // Create the identity from the user info
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.Name, "username"));
                identity.AddClaim(new Claim(ClaimTypes.Role, "tecnico"));

                // Authenticate using the identity
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });

                return RedirectToAction("Index", "Home");
            }

            return Index();
        }

        public async Task<IActionResult> LoginAsStud()
        {
            if (ModelState.IsValid)
            {
                var isValid = true; // TODO Validate the username and the password with your own logic

                if (!isValid)
                {
                    ModelState.AddModelError("", "username or password is invalid");
                    return Index();
                }

                // Create the identity from the user info
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.Name, "username"));
                identity.AddClaim(new Claim(ClaimTypes.Role, "estudante"));

                // Authenticate using the identity
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });

                return RedirectToAction("Index", "Home");
            }

            return Index();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}