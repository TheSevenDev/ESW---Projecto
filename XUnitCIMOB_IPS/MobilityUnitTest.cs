using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CIMOB_IPS.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CIMOB_IPS.Controllers;
using Xunit;

namespace XUnitCIMOB_IPS
{
    public class MobilityUnitTest : Controller
    {
        private CIMOB_IPS_DBContext _context;
        private MobilityController _controller;

        public MobilityUnitTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CIMOB_IPS_DBContext>();
            optionsBuilder.UseInMemoryDatabase();
            _context = new CIMOB_IPS_DBContext(optionsBuilder.Options);

            AddStates();

            _context.Program.Add(new Program()
            {
                IdProgram = 1,
                IdState = 1,
                CreationDate = new DateTime(2017, 12, 28),
                OpenDate = new DateTime(2018, 01, 03),
                ClosingDate = new DateTime(2019, 02, 13),
                MobilityBeginDate = new DateTime(2020, 01, 10),
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
                },
            });

            _context.Institution.Add(new Institution()
            {
                IdInstitution = 1,
                Name = "Instituição",
                Hyperlink = "www.teste.com",
                IdNationality = 1
            });

            _context.InstitutionProgram.Add(new InstitutionProgram()
            {
                IdOutgoingInstitution = 1,
                IdProgram = 1
            });

            _context.Student.Add(new Student()
            {
                IdStudent = 1,
                IdAccount = 1,
                StudentNum = 150221066,
                IdCourse = 1,
                Name = "Estudante",
                Cc = 15131610,
                Telephone = 911111111,
                IdNationality = 1,
                Credits = 120,
                IdAccountNavigation = new Account
                {
                    IdAccount = 1,
                    Email = "email@exemplo.com"
                },
                IdAddressNavigation = new Address
                {
                    AddressDesc = "Desc",
                    DoorNumber = 1,
                    Floor = "1",
                    PostalCode = "2000-123"
                }
            });

            _context.Technician.Add(new Technician()
            {
                IdTechnician = 1,
                IdAccount = 2,
                Name = "Técnico",
                Telephone = 911111111,
                IdAccountNavigation = new Account
                {
                    IdAccount = 2,
                    Email = "email2@exemplo.com"
                },
                IsAdmin = true,
                Active = true
            });

            _context.Application.Add(new Application()
            {
                IdApplication = 1,
                IdStudent = 1,
                IdState = 1,
                HasScholarship = false,
                FinalEvaluation = 0,
                MotivationCard = "",
                EmergencyContactName = "Mae Estudante",
                EmergencyContactRelation = "Mae",
                EmergencyContactTelephone = 922222222,
                ApplicationDate = DateTime.Now.Date
            });

            _context.Application.Add(new Application()
            {
                IdApplication = 2,
                IdStudent = 1,
                IdState = 10,
                HasScholarship = false,
                FinalEvaluation = 15,
                MotivationCard = "",
                EmergencyContactName = "Mae Estudante",
                EmergencyContactRelation = "Mae",
                EmergencyContactTelephone = 922222222,
                ApplicationDate = DateTime.Now.Date
            });

            _context.ApplicationInstitutions.Add(new ApplicationInstitutions
            {
                IdApplication = 2,
                IdInstitution = 1
            });

            _context.Mobility.Add(new Mobility
            {
                IdApplication = 2,
                IdOutgoingInstitution = 1,
                IdResponsibleTechnician = 1,
                IdState = 8
            });

            _context.ApplicationEvaluation.Add(new ApplicationEvaluation
            {
                IdApplication = 2,
                AverageGrade = 12,
                CreditsRatio = 1,
                InterviewPoints = 75,
                MotivationCardPoints = 77
            });

            _context.Interview.Add(new Interview
            {
                Date = new DateTime(2017, 12, 1),
                IdState = 12
            });

            _context.Interview.Add(new Interview
            {
                Date = new DateTime(2018, 12, 1),
                IdState = 12
            });

            _context.SaveChanges();

            _controller = new MobilityController();
        }

        #region aux Functions
        public void AddStates()
        {
            _context.State.Add(new State
            {
                Description = "Em Avaliação"
            });

            _context.State.Add(new State
            {
                Description = "Aceite"
            });

            _context.State.Add(new State
            {
                Description = "Negada"
            });

            _context.State.Add(new State
            {
                Description = "Aberto"
            });

            _context.State.Add(new State
            {
                Description = "Em Seriação"
            });

            _context.State.Add(new State
            {
                Description = "A Decorrer"
            });

            _context.State.Add(new State
            {
                Description = "Concluido"
            });

            _context.State.Add(new State
            {
                Description = "Em preparação"
            });

            _context.State.Add(new State
            {
                Description = "Terminada"
            });

            _context.State.Add(new State
            {
                Description = "Confirmada"
            });

            _context.State.Add(new State
            {
                Description = "Sem Marcação"
            });

            _context.State.Add(new State
            {
                Description = "Marcada"
            });

            _context.State.Add(new State
            {
                Description = "Realizada"
            });

            _context.State.Add(new State
            {
                Description = "Descartada"
            });

            _context.SaveChanges();
        }

        public async Task<IActionResult> MobilitiesInCharge(string search_by)
        {
            long lngTechId = _context.Technician.Where(t => t.IdAccount == 2).Select(t => t.IdTechnician).SingleOrDefault();

            var mobilities = await _context.Mobility.Where(m => m.IdResponsibleTechnician == lngTechId)
                .Include(m => m.IdApplicationNavigation)
                .Include(m => m.IdOutgoingInstitutionNavigation)
                .Include(m => m.IdStateNavigation)
                .ToListAsync();

            foreach (Mobility mobility in mobilities)
            {
                mobility.IdApplicationNavigation.IdProgramNavigation = _context.Program.Where(p => p.IdProgram == mobility.IdApplicationNavigation.IdProgram)
                                                                        .Include(p => p.IdProgramTypeNavigation).SingleOrDefault();

                mobility.IdApplicationNavigation.IdStudentNavigation = _context.Student.Where(s => s.IdStudent == mobility.IdApplicationNavigation.IdStudent)
                                                                        .Include(s => s.IdAccountNavigation).SingleOrDefault();
            }

            if (String.IsNullOrEmpty(search_by))
            {
                    
            }
            else
            {
                mobilities = mobilities
                    .Where(m => m.IdApplicationNavigation.IdStudentNavigation.Name.Contains(search_by) ||
                    m.IdApplicationNavigation.IdStudentNavigation.StudentNum.ToString().Contains(search_by)).ToList();
            }

            return View(mobilities);
        }

        public async Task<IActionResult> MyMobility()
        {
            long lngStudentId = _context.Student.Where(s => s.IdAccount == 1).Select(s => s.IdStudent).SingleOrDefault();

            long lngConfirmedState = _context.State.Where(s => s.Description == "Confirmada").Select(s => s.IdState).SingleOrDefault();

            Application confirmedApplication = _context.Application.Where(a => a.IdStudent == lngStudentId && a.IdState == lngConfirmedState)
                .SingleOrDefault();

            Mobility mobility = null;

            if (confirmedApplication != null)
            {
                mobility = await _context.Mobility.Where(m => m.IdApplication == confirmedApplication.IdApplication)
                    .Include(m => m.IdOutgoingInstitutionNavigation)
                    .Include(m => m.IdResponsibleTechnicianNavigation)
                    .Include(m => m.IdStateNavigation)
                    .SingleOrDefaultAsync();
            }

            if (mobility != null)
            {
                confirmedApplication.IdProgramNavigation = _context.Program.Where(p => p.IdProgram == mobility.IdApplicationNavigation.IdProgram)
                                                                        .Include(p => p.IdProgramTypeNavigation).SingleOrDefault();

                confirmedApplication.IdStudentNavigation = _context.Student.Where(s => s.IdStudent == mobility.IdApplicationNavigation.IdStudent)
                                                                        .Include(s => s.IdAccountNavigation).SingleOrDefault();

                mobility.IdApplicationNavigation = confirmedApplication;

                long lngTechnicianAccountId = _context.Technician.Where(t => t.IdTechnician == mobility.IdResponsibleTechnician).Select(t => t.IdAccount).SingleOrDefault();

                mobility.IdResponsibleTechnicianNavigation.IdAccountNavigation = _context.Account.Where(a => a.IdAccount == lngTechnicianAccountId).SingleOrDefault();
            }

            return View(mobility);
            
        }
        #endregion

        [Fact]
        public void MobilityMobilitiesInChargeTest()
        {
            // Act
            var actionResultTask = MobilitiesInCharge("");
            actionResultTask.Wait();
            var viewResult = actionResultTask.Result as ViewResult;

            List<Mobility> lstMobilities = (List<Mobility>)viewResult.Model;

            // Assert
            Assert.Single(lstMobilities);
        }

        [Fact]
        public void MobilityMyMobilityTest()
        {
            // Act
            var actionResultTask = MyMobility();
            actionResultTask.Wait();
            var viewResult = actionResultTask.Result as ViewResult;

            Mobility mobility = (Mobility)viewResult.Model;

            // Assert
            Assert.NotNull(mobility);
        }
    }
}
