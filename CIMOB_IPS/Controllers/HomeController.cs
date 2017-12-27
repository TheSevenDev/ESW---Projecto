using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CIMOB_IPS.Models;
using Microsoft.EntityFrameworkCore;
using CIMOB_IPS.Models.ViewModels;
using System.Threading.Tasks;

namespace CIMOB_IPS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Contact()
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var coordenators = await context.Coordenator
                    .Include(c => c.IdCourseNavigation)
                    .ToListAsync();

                var technicians = await context.Technician
                    .Include(t => t.IdAccountNavigation)
                    .ToListAsync();

                return View(new ContactsViewModel { Coordenators = coordenators, Technicians = technicians });
            }
        }
    }
}
