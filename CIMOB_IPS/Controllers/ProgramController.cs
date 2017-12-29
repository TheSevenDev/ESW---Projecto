using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CIMOB_IPS.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Details(int programId)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var program = await context.Program
                    .Include(p => p.IdProgramTypeNavigation)
                    .Include(p => p.IdStateNavigation)
                    .Include(p => p.InstitutionProgram)
                    .FirstOrDefaultAsync(p => p.IdProgram == programId);

                foreach(var ip in program.InstitutionProgram)
                {
                    ip.IdOutgoingInstitutionNavigation = await context.Institution
                        .Include(i => i.IdNationalityNavigation)
                        .SingleOrDefaultAsync(i => i.IdInstitution == ip.IdOutgoingInstitution);
                }

                return View(program);
            }
        }
    }
}