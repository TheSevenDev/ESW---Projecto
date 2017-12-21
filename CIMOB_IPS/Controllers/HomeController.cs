using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CIMOB_IPS.Models;

namespace CIMOB_IPS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
