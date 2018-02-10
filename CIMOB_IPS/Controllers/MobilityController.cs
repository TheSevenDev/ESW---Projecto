using CIMOB_IPS.Controllers;
using CIMOB_IPS.Models;
using CIMOB_IPS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xunit;


namespace CIMOB_IPS.Controllers
{
    /// <summary>
    /// Controlador para as acções que envolvem uma mobilidade.
    /// Contém métodos para a confirmação de uma mobilidade ou cancelamento da mesma.
    /// Mostra em vistas as mobilidades ao cargo de um técnico ou a mobilidade do estudante autenticado.
    /// </summary>
    public class MobilityController : Microsoft.AspNetCore.Mvc.Controller
    {
        /// <summary>
        /// Retorna a chave primária associada à conta do utilizador autenticado no momento.
        /// </summary>
        /// <returns>Chave primária associada à conta do utilizador autenticado no momento</returns>
        /// <remarks></remarks>
        public int GetCurrentUserID()
        {
            return int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        /// <summary>
        /// Retorna a vista com as mobilidades às quais o técnico autenticado está encarregue. Esta lista pode ser filtrada por um critério, nomeadamente nome ou número do estudante.
        /// </summary>
        /// <param name="search_by">Critério de filtragem</param>
        /// <returns>View Mobility/MobilitiesInCharge</returns>
        /// <remarks></remarks>
        public IActionResult MobilitiesInCharge(string search_by)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!(User.IsInRole("tecnico") || User.IsInRole("tecnico_admin")))
                return RedirectToAction("Index", "Home");

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                long lngTechId = context.Technician.Where(t => t.IdAccount == GetCurrentUserID()).Select(t => t.IdTechnician).SingleOrDefault();

                var mobilities = context.Mobility.Where(m => m.IdResponsibleTechnician == lngTechId)
                    .Include(m => m.IdApplicationNavigation)
                    .Include(m => m.IdOutgoingInstitutionNavigation)
                    .Include(m => m.IdStateNavigation)
                    .ToList();

                foreach (Mobility mobility in mobilities)
                {
                    mobility.IdApplicationNavigation.IdProgramNavigation = context.Program.Where(p => p.IdProgram == mobility.IdApplicationNavigation.IdProgram)
                                                                            .Include(p => p.IdProgramTypeNavigation).SingleOrDefault();

                    mobility.IdApplicationNavigation.IdStudentNavigation = context.Student.Where(s => s.IdStudent == mobility.IdApplicationNavigation.IdStudent)
                                                                            .Include(s => s.IdAccountNavigation).SingleOrDefault();
                }

                if (String.IsNullOrEmpty(search_by))
                {
                    ViewData["search-by"] = "";
                }
                else
                {
                    mobilities = mobilities
                        .Where(m => m.IdApplicationNavigation.IdStudentNavigation.Name.Contains(search_by) ||
                        m.IdApplicationNavigation.IdStudentNavigation.StudentNum.ToString().Contains(search_by)).ToList();

                    ViewData["search-by"] = search_by.ToString();
                }

                return View(mobilities);
            }
        }

        /// <summary>
        /// Retorna uma vista com a mobilidade atual do estudante autenticado.
        /// </summary>
        /// <returns>Vista com a mobilidade atual do estudante autenticado.</returns>
        public IActionResult MyMobility()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if ((User.IsInRole("tecnico") || User.IsInRole("tecnico_admin")))
                return RedirectToAction("Index", "Home");

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                long lngStudentId = context.Student.Where(s => s.IdAccount == GetCurrentUserID()).Select(s => s.IdStudent).SingleOrDefault();

                long lngConfirmedState = context.State.Where(s => s.Description == "Confirmada").Select(s => s.IdState).SingleOrDefault();

                Application confirmedApplication = context.Application.Where(a => a.IdStudent == lngStudentId && a.IdState == lngConfirmedState)
                    .SingleOrDefault();

                Mobility mobility = null;

                if (confirmedApplication != null)
                {
                    mobility = context.Mobility.Where(m => m.IdApplication == confirmedApplication.IdApplication)
                        .Include(m => m.IdOutgoingInstitutionNavigation)
                        .Include(m => m.IdResponsibleTechnicianNavigation)
                        .Include(m => m.IdStateNavigation)
                        .SingleOrDefault();
                }

                if (mobility != null)
                {
                    confirmedApplication.IdProgramNavigation = context.Program.Where(p => p.IdProgram == mobility.IdApplicationNavigation.IdProgram)
                                                                            .Include(p => p.IdProgramTypeNavigation).SingleOrDefault();

                    confirmedApplication.IdStudentNavigation = context.Student.Where(s => s.IdStudent == mobility.IdApplicationNavigation.IdStudent)
                                                                            .Include(s => s.IdAccountNavigation).SingleOrDefault();

                    mobility.IdApplicationNavigation = confirmedApplication;

                    long lngTechnicianAccountId = context.Technician.Where(t => t.IdTechnician == mobility.IdResponsibleTechnician).Select(t => t.IdAccount).SingleOrDefault();

                    mobility.IdResponsibleTechnicianNavigation.IdAccountNavigation = context.Account.Where(a => a.IdAccount == lngTechnicianAccountId).SingleOrDefault();
                }

                return View(mobility);
            }
        }

        /// <summary>
        /// Retorna uma partial view com um resumo do perfil de um técnico.
        /// </summary>
        /// <param name="IdTechnician">Chave primária de um técnico</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IActionResult TechnicianPreview(int? IdTechnician)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if ((User.IsInRole("tecnico") || User.IsInRole("tecnico_admin")))
                return RedirectToAction("Index", "Home");

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                Technician tech = context.Technician.Where(t => t.IdTechnician == IdTechnician).Include(t => t.IdAccountNavigation).SingleOrDefault();


                if (tech == null)
                {
                    return RedirectToAction("MyMobility", "Mobility");
                }

                return PartialView("~/Views/Profile/_ViewTechProfilePreview.cshtml", tech);
            }
        }
    }
}
