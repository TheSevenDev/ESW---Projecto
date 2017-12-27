using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CIMOB_IPS.Models;
using CIMOB_IPS.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CIMOB_IPS.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly CIMOB_IPS_DBContext _context;

        public ApplicationController(CIMOB_IPS_DBContext context)
        {
            _context = context;
        }

        public int GetCurrentUserID()
        {
            return int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        public IActionResult New(ApplicationViewModel model)
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Login", "Account");

            ViewData["app_form"] = "NewApplication";
            ViewData["submit_form"] = "NewApplication";
            ApplicationViewModel viewModel = new ApplicationViewModel { Student = new Student(), Account = new Account(), Application = new Application() };
            if (model.Student == null)
            {
                viewModel.Student.Name = "ASD";
                viewModel.Student.Telephone = 912345;
                viewModel.Account.Email = "ASD";
            }
            else
                viewModel = model;


            viewModel.Nationalities = PopulateNationalities();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult NewApplication(ApplicationViewModel model)
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Login", "Account");

            ViewData["app_form"] = "NewApplication_Mob";
            ViewData["submit_form"] = "NewApplicationMotiv";

            return View("New", model);
        }

        public IActionResult NewApplicationMotiv(ApplicationViewModel model)
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Login", "Account");

            ViewData["app_form"] = "NewApplication_Motiv";
            ViewData["submit_form"] = "RegisterApplication";

            return View("New", model);
        }

        public IActionResult RegisterApplication(ApplicationViewModel model)
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Login", "Account");

           
            return View("New", model);
        }

        private IEnumerable<SelectListItem> PopulateNationalities()
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                List<SelectListItem> lisNationalities = new List<SelectListItem>();

                var listNationalities = context.Nationality.OrderBy(x => x.Description).ToList();

                foreach (Nationality n in listNationalities)
                {
                    lisNationalities.Add(new SelectListItem { Value = n.IdNationality.ToString(), Text = n.Description });
                }

                return lisNationalities;
            }
        }

        public async Task<IActionResult> MyApplications()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            AccountController ac = new AccountController();
            ProfileController pc = new ProfileController();

            int lngCurrentUserId = GetCurrentUserID();

            if (!ac.IsStudent(lngCurrentUserId))
                return RedirectToAction("Index", "Home");

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var lisApplications = await context.Application.Where(a => a.IdStudent == lngCurrentUserId)
                    .Include(a => a.IdStateNavigation)
                    .Include(a => a.IdProgramNavigation)
                    .Include(a => a.ApplicationInstitutions)
                    .ToListAsync();

                return View(lisApplications);
            }
        }
    }
}