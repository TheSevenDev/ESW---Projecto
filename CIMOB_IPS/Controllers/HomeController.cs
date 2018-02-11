using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CIMOB_IPS.Models;
using Microsoft.EntityFrameworkCore;
using CIMOB_IPS.Models.ViewModels;
using System.Threading.Tasks;

namespace CIMOB_IPS.Controllers
{
    /// <summary>
    /// Controlador para as páginas principais da aplicação.
    /// </summary>
    /// <remarks></remarks>
    public class HomeController : Controller
    {
        /// <summary>
        /// Retorna a HomePage da aplicação.
        /// </summary>
        /// <returns>HomePage da aplicação.</returns>
        /// <remarks></remarks>
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        /// <summary>
        /// Retorna a página dos contactos.
        /// </summary>
        /// <returns>Página dos contactos.</returns>
        /// <remarks></remarks>
        public async Task<IActionResult> TechniciansContact()
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                //Coordenadores de curso
                var coordenators = await context.Coordenator
                    .Include(c => c.IdCourseNavigation)
                    .ToListAsync();

                //Técnicos
                var technicians = await context.Technician
                    .Include(t => t.IdAccountNavigation)
                    .ToListAsync();

                return View("~/Views/Profile/TechniciansContact.cshtml" , new ContactsViewModel { Coordenators = coordenators, Technicians = technicians });
            }
        }
    }
}
