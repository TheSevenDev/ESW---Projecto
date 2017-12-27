using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CIMOB_IPS.Controllers
{
    public class ProgramController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChooseProgram()
        {
            return View();
        }
    }
}