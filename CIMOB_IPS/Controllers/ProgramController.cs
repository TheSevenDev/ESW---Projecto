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
                    .Include(p => p.IdStateNavigation)
                    .ToListAsync();

                return View(programs);
            }
        }
    }
}