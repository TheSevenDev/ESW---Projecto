using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CIMOB_IPS.Controllers
{
    public class MobilityController : Controller
    {
        public IActionResult MobilitiesInCharge()
        {
            return View();
        }

        public IActionResult MyMobility()
        {
            return View();
        }
    }
}