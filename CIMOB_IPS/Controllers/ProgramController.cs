using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CIMOB_IPS.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CIMOB_IPS.Controllers
{
    public class ProgramController : Controller
    {
        public async Task<IActionResult> Index()
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var programs = await context.Program
                    .Include(p => p.IdProgramTypeNavigation)
                    .Include(p => p.IdStateNavigation)
                    .ToListAsync();

                return View(programs);
            }
        }

        public async Task<IActionResult> Details(string programID)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var program = await context.Program
                    .Include(p => p.IdProgramTypeNavigation)
                    .Include(p => p.IdStateNavigation)
                    .Include(p => p.InstitutionProgram)
                    .FirstOrDefaultAsync(p => p.IdProgram == Int32.Parse(programID));

                switch (program.IdStateNavigation.Description)
                {
                    case "Aberto":
                        ViewData["programstate-color"] = "Green";
                        break;
                    case "Fechado":
                        ViewData["programstate-color"] = "Red";
                        break;
                    case "A Decorrer":
                        ViewData["programstate-color"] = "Orange";
                        break;

                }
          

                foreach(var ip in program.InstitutionProgram)
                {
                    ip.IdOutgoingInstitutionNavigation = await context.Institution
                        .Include(i => i.IdNationalityNavigation)
                        .Include(i => i.Course)
                        .SingleOrDefaultAsync(i => i.IdInstitution == ip.IdOutgoingInstitution);
                }

                return View(program);
            }
        }

        public int GetCurrentUserID()
        {
            return int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if(!new AccountController().IsTechnician(GetCurrentUserID()))
                return RedirectToAction("Index", "Home");

            return View();
        }
    }
}