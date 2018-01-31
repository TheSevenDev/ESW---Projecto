using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CIMOB_IPS.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CIMOB_IPS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CIMOB_IPS.Controllers
{
    public class ProgramController : Controller
    {
        public async Task<IActionResult> Index()
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var programs = await context.Program
                    .OrderBy(p => p.ClosingDate)
                    .Include(p => p.IdProgramTypeNavigation)
                    .Include(p => p.IdStateNavigation)
                    .ToListAsync();

                return View(programs);
            }
        }

        public async Task<IActionResult> Details([FromQuery] string programID)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var program = await context.Program
                    .Include(p => p.IdProgramTypeNavigation)
                    .Include(p => p.IdStateNavigation)
                    .Include(p => p.InstitutionProgram)
                    .FirstOrDefaultAsync(p => p.IdProgram == Int32.Parse(programID));

                if (program == null)
                {
                    return RedirectToAction("Index", "Program");
                }

                if (DateTime.Now > program.ClosingDate)
                {
                    program.IdStateNavigation = context.State.Where(s => s.Description == "Fechado").FirstOrDefault();
                    program.IdState = context.State.Where(s => s.Description == "Fechado").FirstOrDefault().IdState;
                }

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

            return View(new ProgramViewModel { ProgramTypes = PopulateProgramTypes(), Institutions = PopulateInstitutions() } );
        }

        public IActionResult Edit(string programID)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if(!new AccountController().IsTechnician(GetCurrentUserID()))
                return RedirectToAction("Index", "Home");

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var auxProgram = context.Program.Where(p => p.IdProgram == Int32.Parse(programID)).FirstOrDefault();

                if(auxProgram == null)
                    return RedirectToAction("Index", "Home");
                else
                {
                    return View(auxProgram);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public async Task<IActionResult> Edit(CIMOB_IPS.Models.Program model)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if(!new AccountController().IsTechnician(GetCurrentUserID()))
                return RedirectToAction("Index", "Home");

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            { 
                
                context.Update(model);
                await context.SaveChangesAsync();

                return RedirectToAction("Details", "Program", new {programID = model.IdProgram });
            }
        }

        private IEnumerable<SelectListItem> PopulateProgramTypes()
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                List<SelectListItem> lisProgramTypes = new List<SelectListItem>();

                var listProgramTypes = context.ProgramType.OrderBy(x => x.Name).ToList();

                foreach (ProgramType n in listProgramTypes)
                {
                    lisProgramTypes.Add(new SelectListItem { Value = n.IdProgramType.ToString(), Text = n.Name });
                }

                return lisProgramTypes;
            }
        }

        private List<CheckBoxListItem> PopulateInstitutions()
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                List<CheckBoxListItem> lisInstitutions = new List<CheckBoxListItem>();

                var listInstitutions = context.Institution.Where(i => i.IdNationality !=
                    (from n in context.Nationality where n.Description == "PORTUGAL" select n.IdNationality).SingleOrDefault()).OrderBy(i => i.Name).ToList();

                foreach(Institution i in listInstitutions)
                {
                    lisInstitutions.Add(new CheckBoxListItem { ID = i.IdInstitution, Display = i.Name, IsChecked = false });
                }

                return lisInstitutions;
            }
        }

        
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProgramViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(model.Institutions.Count(i => i.IsChecked == true) > 0)
                {
                    using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
                    {
                        if(!context.Program.Where(a => a.IdProgramType == model.IdProgramType).Any())
                        { 
                            var program = new Models.Program
                            {
                                IdState = await (context.State.Where(s => s.Description == "Aberto").Select(s => s.IdState).SingleOrDefaultAsync()),
                                CreationDate = DateTime.Now,
                                OpenDate = model.OpenDate,
                                ClosingDate = model.ClosingDate,
                                MobilityDate = (DateTime)model.MobilityDate,
                                Vacancies = model.Vacancies,
                                IdProgramType = model.IdProgramType
                            };

                            context.Add(program);
                            await context.SaveChangesAsync();

                            foreach(var i in model.Institutions.Where(i => i.IsChecked == true))
                            {
                                var instution = new InstitutionProgram
                                {
                                    IdProgram = program.IdProgram,
                                    IdOutgoingInstitution = i.ID
                                };

                                context.Add(instution);
                                await context.SaveChangesAsync();
                            }

                            return RedirectToAction("Index", "Program");
                        }
                        else
                        {
                            ViewData["Program-type-error"] = "Já existe um programa activo do mesmo tipo";
                        }
                    }
                }
                else
                {
                    ViewData["Institutions-Error"] = "É necessário associar instituições ao programa.";
                }
            }

            return View(new ProgramViewModel { ProgramTypes = PopulateProgramTypes(), Institutions = PopulateInstitutions() });
        }
        

    }
}