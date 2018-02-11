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
    /// <summary>
    /// Controlador usado paras ações de um programa de mobilidade.
    /// Contém essencialmente acções para a visualização dos detalhes de um programa e criação e gestão do mesmo.
    /// </summary>
    /// <remarks></remarks>
    public class ProgramController : Controller
    {
        /// <summary>
        /// Mostra, numa vista apropriada, os programas de mobilidade suportados pela aplicação.
        /// </summary>
        /// <returns>Vista com os programas de mobilidade suportados pela aplicação.</returns>
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

        /// <summary>
        /// Mostra os detalhes de um programa cuja chave primária é passada por argumento.
        /// </summary>
        /// <param name="programID">Chave primária do programa a mostrar.</param>
        /// <returns>Vitsa com os detalhes de um programa.</returns>
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
                    program.IdStateNavigation = context.State.Where(s => s.Description == "Em Seriação").FirstOrDefault();
                    program.IdState = context.State.Where(s => s.Description == "Em Seriação").FirstOrDefault().IdState;
                }

                switch (program.IdStateNavigation.Description)
                {
                    case "Aberto":
                        ViewData["programstate-color"] = "Green";
                        break;
                    case "Em seriação":
                        ViewData["programstate-color"] = "Red";
                        break;
                    case "A Decorrer":
                        ViewData["programstate-color"] = "Orange";
                        break;

                }


                //INSTITUIÇÕES
                foreach (var ip in program.InstitutionProgram)
                {
                    ip.IdOutgoingInstitutionNavigation = await context.Institution
                        .Include(i => i.IdNationalityNavigation)
                        .Include(i => i.Course)
                        .SingleOrDefaultAsync(i => i.IdInstitution == ip.IdOutgoingInstitution);
                }

                CheckProgramStateChange(program);


                return PartialView("_ProgramDetails", program);
            }
        }

        /// <summary>
        /// Verifica se, consoante a data de um programa, ele se encontra aberto ou fechado e muda o seu estado
        /// </summary>
        /// <param name="program">Programa a modificar</param>
        /// <remarks></remarks>
        public void CheckProgramStateChange(Models.Program program)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                long lngOpenProgramState = context.State.Where(s => s.Description == "Aberto").Select(s => s.IdState).SingleOrDefault();
                long lngInSeriationState = context.State.Where(s => s.Description == "Em Seriação").Select(s => s.IdState).SingleOrDefault();

                if(DateTime.Compare((DateTime)program.ClosingDate, DateTime.Now) <= 0 && program.IdState == lngOpenProgramState)
                {
                    program.IdState = lngInSeriationState;
                    context.Update(program);
                    context.SaveChanges();
                }
                else if(DateTime.Compare((DateTime)program.ClosingDate, DateTime.Now) <= 0 
                    && DateTime.Compare((DateTime)program.ClosingDate, DateTime.Now) > 0
                    && program.IdState != lngOpenProgramState)
                {
                    program.IdState = lngOpenProgramState;
                    context.Update(program);
                    context.SaveChanges();
                }
            }
        }

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
        /// Retorna a página com o formulário de criação de um programa.
        /// </summary>
        /// <returns>Página com o formulário de criação de um programa.</returns>
        /// <remarks></remarks>
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!new AccountController().IsTechnician(GetCurrentUserID()))
                return RedirectToAction("Index", "Home");

            return View(new ProgramViewModel { ProgramTypes = PopulateProgramTypes(), Institutions = PopulateInstitutions() });
        }

        /// <summary>
        /// Retorna a página com o formulário de edição de um programa.
        /// Podem ser alteradas as datas para as candidaturas e a data prevista para o ínicio da mobilidade.
        /// </summary>
        /// <param name="programID">Chave primária do programa a editar</param>
        /// <returns>Página com o formulário de edição de um programa.</returns>
        /// <remarks></remarks>
        public IActionResult Edit(string programID)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!new AccountController().IsTechnician(GetCurrentUserID()))
                return RedirectToAction("Index", "Home");

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                var auxProgram = context.Program.Where(p => p.IdProgram == Int32.Parse(programID)).FirstOrDefault();

                if (auxProgram == null)
                    return RedirectToAction("Index", "Home");
                else
                {
                    return View(auxProgram);
                }
            }
        }

        /// <summary>
        /// Edição de um programa.
        /// Podem ser alteradas as datas para as candidaturas e a data prevista para o ínicio da mobilidade.
        /// </summary>
        /// <param name="model">Modeo do programa</param>
        /// <returns>Vista com os detalhes do programa editado</returns>
        /// <remarks></remarks>
        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public async Task<IActionResult> Edit(Models.Program model)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!new AccountController().IsTechnician(GetCurrentUserID()))
                return RedirectToAction("Index", "Home");

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {

                context.Update(model);
                await context.SaveChangesAsync();

                return RedirectToAction("Details", "Program", new { programID = model.IdProgram });
            }
        }

        /// <summary>
        /// Cria uma lista de seleção com os vários tipos de programa suportados pela aplicação.
        /// </summary>
        /// <returns>Lista de seleção criada</returns>
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
        /// <summary>
        /// Cria uma lista de seleção com as várias instituições passíveis de destino de mobilidade. 
        /// Estas instituições não podem ser de nacionalide portuguesa, visto que a aplicação não suporta mobilidades dentro do mesmo país.
        /// </summary>
        /// <returns>Lista de seleção criada</returns>
        private List<CheckBoxListItem> PopulateInstitutions()
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                List<CheckBoxListItem> lisInstitutions = new List<CheckBoxListItem>();

                var listInstitutions = context.Institution.Where(i => i.IdNationality !=
                    (from n in context.Nationality where n.Description == "PORTUGAL" select n.IdNationality).SingleOrDefault()).OrderBy(i => i.Name).ToList();

                foreach (Institution i in listInstitutions)
                {
                    lisInstitutions.Add(new CheckBoxListItem { ID = i.IdInstitution, Display = i.Name, IsChecked = false });
                }

                return lisInstitutions;
            }
        }

        /// <summary>
        /// Cria um novo programa na aplicação.
        /// Contém informações do tipo de programa, da data de abertura e fecho das candidaturas, instituições de destino e data prevista para mobilidade.
        /// </summary>
        /// <param name="model">Modelo do programa criado</param>
        /// <returns>A vista com os detalhes do programa criado</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProgramViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Institutions.Count(i => i.IsChecked == true) > 0)
                {
                    using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
                    {
                        if (!context.Program.Where(a => a.IdProgramType == model.IdProgramType && a.IdStateNavigation.Description != "Aberto").Any())
                        {
                            var program = new Models.Program
                            {
                                //Programa inicialmente aberto para candidaturas
                                IdState = await (context.State.Where(s => s.Description == "Aberto").Select(s => s.IdState).SingleOrDefaultAsync()),
                                CreationDate = DateTime.Now,
                                OpenDate = model.OpenDate,
                                ClosingDate = model.ClosingDate,
                                MobilityBeginDate = (DateTime)model.MobilityBeginDate,
                                MobilityEndDate = (DateTime)model.MobilityEndDate,
                                Vacancies = model.Vacancies,
                                IdProgramType = model.IdProgramType
                            };

                            context.Add(program);
                            await context.SaveChangesAsync();

                            foreach (var i in model.Institutions.Where(i => i.IsChecked == true))
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