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

        public async Task<IActionResult> New()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Index", "Home");

            ViewData["app_form"] = "NewApplication";
            ViewData["submit_form"] = "NewApplicationMob";
            

            int userID = GetCurrentUserID();

            ApplicationViewModel viewModel = new ApplicationViewModel { Account = new Account(), Application = new Application { IdStudentNavigation = GetStudentById(userID) } };
            viewModel.Account.Email = GetEmail(userID);
            var program = await _context.Program.Include(p => p.IdProgramTypeNavigation).Include(p => p.IdStateNavigation).Include(p => p.InstitutionProgram).FirstOrDefaultAsync(p => p.IdProgram == 1); 

            foreach(var ip in program.InstitutionProgram)
            {
                ip.IdOutgoingInstitutionNavigation = await _context.Institution
                    .Include(i => i.IdNationalityNavigation)
                    .SingleOrDefaultAsync(i => i.IdInstitution == ip.IdOutgoingInstitution);
            }


            viewModel.Institutions = program.InstitutionProgram.ToList();
            viewModel.Nationalities = PopulateNationalities();
            viewModel.Application.IdProgramNavigation = _context.Program.Where(p => p.IdProgram == 1).FirstOrDefault(); //ERASMUS
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

      
        [HttpPost]
        public IActionResult RegisterApplication(ApplicationViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (User.IsInRole("tecnico") || User.IsInRole("tecnico_admin"))
                return RedirectToAction("Index", "Home");


            /*Application app = new Application
            {
                ApplicationDate = DateTime.Now,
                HasScholarship = model.Application.HasScholarship,
            };*/


            //_context.Application.Add(app);

            return RedirectToAction("MyApplications", "Application");
            
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
                        .Include(ai => ai.IdInstitutionNavigation).OrderBy(ai => ai.InstitutionOrder).Where(i=>i.IdApplication == app.IdApplication).ToListAsync();

                    app.IdProgramNavigation.IdProgramTypeNavigation = await context.ProgramType.Where(p => p.IdProgramType == app.IdProgramNavigation.IdProgramType).SingleOrDefaultAsync();
                }

                return View(lisApplications);
            }
        }
    }
}