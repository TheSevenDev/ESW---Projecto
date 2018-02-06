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
    public class MobilityController : Controller
    {
        public int GetCurrentUserID()
        {
            return int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

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

                if(confirmedApplication != null)
                {
                    mobility = context.Mobility.Where(m => m.IdApplication == confirmedApplication.IdApplication)
                        .Include(m => m.IdOutgoingInstitutionNavigation)
                        .Include(m => m.IdResponsibleTechnicianNavigation)
                        .Include(m => m.IdStateNavigation)
                        .SingleOrDefault();
                }
                
                if(mobility != null)
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
    }
}