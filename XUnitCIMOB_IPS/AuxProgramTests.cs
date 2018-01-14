using System;
using System.Collections.Generic;
using System.Text;
using CIMOB_IPS.Controllers;
using CIMOB_IPS.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace XUnitCIMOB_IPS
{
    public class AuxProgramTests : Controller
    {
        private CIMOB_IPS_DBContext _context;
        private ProgramController _controller;

        public AuxProgramTests()
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
                ClosingDate = new DateTime(2018, 02, 13),
                MobilityDate = new DateTime(2018, 01, 10),
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

        //changing functions to use new context

        public async Task<IActionResult> IndexTest()
        {

            var programs = await _context.Program
                .Include(p => p.IdProgramTypeNavigation)
                .Include(p => p.IdStateNavigation).ToListAsync();

                return View(programs);
            
        }

        [Fact]
        public void ProgramIndexTest()
        {
            // Act
            var actionResultTask = IndexTest();
            actionResultTask.Wait();
            var viewResult = actionResultTask.Result as ViewResult;

            List<Program> lstPrograms = (List<Program>)viewResult.Model;

            // Assert
            Program model = lstPrograms[0];
            Assert.Equal(1, model.IdProgram);
        }


    }
}