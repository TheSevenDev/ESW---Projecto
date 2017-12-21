using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CIMOB_IPS.Models;
using CIMOB_IPS.Models.ViewModels;

namespace CIMOB_IPS.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly CIMOB_IPS_DBContext _context;

        public ApplicationController(CIMOB_IPS_DBContext context)
        {
            _context = context;
        }

        public IActionResult New()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            return View(new ApplicationViewModel { Nationalities = PopulateNationalities()});
        }

        private IEnumerable<SelectListItem> PopulateNationalities()
        {
            List<SelectListItem> lisNationalities = new List<SelectListItem>();

            var listNationalities = _context.Nationality.OrderBy(x => x.Description).ToList();

            foreach (Nationality n in listNationalities)
            {
                lisNationalities.Add(new SelectListItem { Value = n.IdNationality.ToString(), Text = n.Description });
            }

            return lisNationalities;
        }
    }
}