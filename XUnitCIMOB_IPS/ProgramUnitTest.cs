using CIMOB_IPS.Controllers;
using CIMOB_IPS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xunit;

namespace XUnitCIMOB_IPS
{
    public class ProgramUnitTest : Microsoft.AspNetCore.Mvc.Controller
    {
        private CIMOB_IPS_DBContext _context;
        private ProgramController _controller;

        public ProgramUnitTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CIMOB_IPS_DBContext>();
            optionsBuilder.UseInMemoryDatabase();
            _context = new CIMOB_IPS_DBContext(optionsBuilder.Options);

            _context.Program.Add(new Program()
            {
                IdProgram = 1,
                IdState = 1,
                CreationDate = new DateTime(2017, 12, 28),
                OpenDate = new DateTime(2018, 01, 03),
                ClosingDate = new DateTime(2019, 02, 13),
                MobilityDate = new DateTime(2020, 01, 10),
                Vacancies = 2,
                IdProgramType = 1,
                IdProgramTypeNavigation = new ProgramType
                {
                    IdProgramType = 1,
                    Name = "Program",
                    Description = "Program muito fixe",
                    ImageFile = "File"
                },
                IdStateNavigation = new State
                {
                    IdState = 1,
                    Description = "Aberto"
                }
            });

            _context.SaveChanges();
            _controller = new ProgramController();
        }

        #region Aux Functions

        public async Task<Program> GetProgram()
        {
            return await _context.Program
                .Include(p => p.IdProgramTypeNavigation)
                .Include(p => p.IdStateNavigation)
                .FirstOrDefaultAsync();
        }

        //changing functions to use new context

        public async Task<IActionResult> IndexTest()
        {

            var programs = await _context.Program
                .Include(p => p.IdProgramTypeNavigation)
                .Include(p => p.IdStateNavigation).ToListAsync();

            return View(programs);

        }

        public async Task<IActionResult> Details([FromQuery] string programID)
        {
            var program = await _context.Program
                .Include(p => p.IdProgramTypeNavigation)
                .Include(p => p.IdStateNavigation)
                .Include(p => p.InstitutionProgram)
                .FirstOrDefaultAsync(p => p.IdProgram == Int32.Parse(programID));

            if (DateTime.Now > program.ClosingDate)
            {
                program.IdStateNavigation = _context.State.Where(s => s.Description == "Fechado").FirstOrDefault();
                program.IdState = _context.State.Where(s => s.Description == "Fechado").FirstOrDefault().IdState;
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

            foreach (var ip in program.InstitutionProgram)
            {
                ip.IdOutgoingInstitutionNavigation = await _context.Institution
                    .Include(i => i.IdNationalityNavigation)
                    .Include(i => i.Course)
                    .SingleOrDefaultAsync(i => i.IdInstitution == ip.IdOutgoingInstitution);
            }

            return View(program);

        }

        #endregion

        [Fact]
        public void ProgramIndexTest()
        {
            // Act
            var actionResultTask = IndexTest();
            actionResultTask.Wait();
            var viewResult = actionResultTask.Result as System.Web.Mvc.ViewResult;

            List<Program> lstPrograms = (List<Program>)viewResult.Model;

            // Assert
            Program model = lstPrograms[0];
            Assert.Equal(1, model.IdProgram);

            var viewName = viewResult.ViewName;

            Assert.True(string.IsNullOrEmpty(viewName) || viewName == "Index");
        }

        [Fact]
        public void ProgramDetailsTestExistent()
        {
            // Act
            var actionResultTask = Details("1");
            actionResultTask.Wait();
            var viewResult = actionResultTask.Result as System.Web.Mvc.ViewResult;

            Program model = (Program)viewResult.Model;

            Assert.IsType<Program>(viewResult.Model);

            // Assert
            Assert.Equal("Program", model.IdProgramTypeNavigation.Name);
        }

        [Fact]
        public void isOpenProgram_true_whenIsOpenProgram()
        {
            var program = GetProgram().Result;

            Assert.True(program.isOpenProgram());
        }

        [Fact]
        public void isOpenProgram_false_whenIsOpenProgram()
        {
            var program = GetProgram().Result;

            program.IdStateNavigation.Description = "Fechado";

            Assert.False(program.isOpenProgram());
        }

        [Fact]
        public void withVacanciesAvailable_true_withVacanciesAvailable()
        {
            var program = GetProgram().Result;

            Assert.True(program.withVacanciesAvailable());
        }

        [Fact]
        public void withVacanciesAvailable_false_withVacanciesAvailable()
        {
            var program = GetProgram().Result;

            program.Vacancies = 0;

            Assert.False(program.withVacanciesAvailable());
        }

        [Fact]
        public void withDateAvailable_true_withDateAvailable()
        {
            var program = GetProgram().Result;

            Assert.True(program.withDateAvailable());
        }

        [Fact]
        public void withDateAvailable_false_withDateAvailable()
        {
            var program = GetProgram().Result;

            program.ClosingDate = new DateTime(2017, 01, 03);

            Assert.False(program.withDateAvailable());
        }

        [Fact]
        public void withPossibleApplication_true_withPossibleApplication()
        {
            var program = GetProgram().Result;

            Assert.True(program.withPossibleApplication());
        }

        [Fact]
        public void withPossibleApplication_false_withPossibleApplication()
        {
            var program = GetProgram().Result;

            program.Vacancies = 0;

            Assert.False(program.withPossibleApplication());
        }
    }
}
