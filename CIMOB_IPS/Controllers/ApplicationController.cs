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
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Index", "Home");

            ViewData["app_form"] = "NewApplication";
            ViewData["submit_form"] = "NewApplicationMob";
            ApplicationViewModel viewModel = new ApplicationViewModel { Student = new Student(), Account = new Account(), Application = new Application() };

            int userID = GetCurrentUserID();
            Student student = GetStudentById(userID);


            if (model.Student == null)
            {
                viewModel.Student.Name = student.Name;
                viewModel.Student.Telephone = student.Telephone;
                viewModel.Account.Email = GetEmail(userID);
                viewModel.Student.IdNationality = student.IdNationality;
                viewModel.Student.Address = student.Address;
            }
            else
                viewModel = model;


            viewModel.Nationalities = PopulateNationalities();
            return View(viewModel);
        }

        public Student GetStudentById(int intId)
        {
            return _context.Student.Where(s => s.IdAccount == intId).FirstOrDefault();
        }

        public string GetEmail(int intId)
        {
            return _context.Account.Where(s => s.IdAccount == intId).FirstOrDefault().Email;
        }

        
        public IActionResult NewApplicationMob(ApplicationViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Index", "Home");

            if (model.Student == null)
                return RedirectToAction("New", "Application");

            ViewData["app_form"] = "NewApplication_Mob";
            ViewData["submit_form"] = "NewApplicationMotiv";

            var institutions = _context.Institution;

            model.Institutions = institutions.ToList();


            return View("New", model);
        }

        public IActionResult NewApplicationMotiv(ApplicationViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Index", "Home");

            if (model.Student == null)
                return RedirectToAction("New", "Application");

            ViewData["app_form"] = "NewApplication_Motiv";
            ViewData["submit_form"] = "RegisterApplication";

            return View("New", model);
        }

        public IActionResult RegisterApplication(ApplicationViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Index", "Home");

           
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

            if (User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Index", "Home");

            AccountController ac = new AccountController();
            ProfileController pc = new ProfileController();

            int lngCurrentUserId = GetCurrentUserID();

            if (!ac.IsStudent(lngCurrentUserId))
                return RedirectToAction("Index", "Home");

            long studentId = ac.GetStudentId(lngCurrentUserId);

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var lisApplications = await context.Application.Where(a => a.IdStudent == studentId)
                    .Include(a => a.IdStateNavigation)
                    .Include(a => a.IdProgramNavigation)
                    .Include(a => a.ApplicationInstitutions)
                    .ToListAsync();

                foreach(Application app in lisApplications)
                {
                    app.ApplicationInstitutions = await context.ApplicationInstitutions
                        .Include(ai => ai.IdInstitutionNavigation).OrderBy(ai => ai.InstitutionOrder).ToListAsync();

                    app.IdProgramNavigation.IdProgramTypeNavigation = await context.ProgramType.Where(p => p.IdProgramType == app.IdProgramNavigation.IdProgramType).SingleOrDefaultAsync();
                }

                return View(lisApplications);
            }
        }
    }
}