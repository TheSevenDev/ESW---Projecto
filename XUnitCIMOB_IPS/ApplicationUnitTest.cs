using CIMOB_IPS.Controllers;
using CIMOB_IPS.Models;
using CIMOB_IPS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitCIMOB_IPS
{
    public class ApplicationUnitTest : Controller
    {
        private CIMOB_IPS_DBContext _context;
        private ApplicationController _controller;

        public ApplicationUnitTest()
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

            _context.Student.Add(new Student()
            {
                IdStudent = 2,
                IdAccount = 2,
                StudentNum = 150221067,
                IdCourse = 1,
                Name = "Estudant2",
                Cc = 15131611,
                Telephone = 912111111,
                IdNationality = 1,
                Credits = 20,
                IdAccountNavigation = new Account
                {
                    IdAccount = 2,
                    Email = "email2@exemplo.com"
                },
                IdAddressNavigation = new Address
                {
                    AddressDesc = "Desc2",
                    DoorNumber = 2,
                    Floor = "2",
                    PostalCode = "2010-123"
                }
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
            
            _controller = new ApplicationController(_context, null);
        }
        
        #region Aux Functions

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

        [HttpPost]
        public IActionResult DeleteApplication(int applicationId)
        {
            try
            {
                var application = _context.Application.Where(app => app.IdApplication == applicationId).FirstOrDefault();
                if (application != null)
                    _context.Application.Remove(application);

                _context.SaveChanges();
            }
            catch
            {

            }

            return RedirectToAction("MyApplications", "Application");
        }

        [HttpPost]
        public IActionResult SubmitApplication()
        {
            try
            {
                var application = new Application()
                {
                    IdApplication = 3,
                    IdStudent = 1,
                    IdState = 1,
                    HasScholarship = false,
                    FinalEvaluation = 10,
                    MotivationCard = "",
                    EmergencyContactName = "Pai Estudante",
                    EmergencyContactRelation = "Pai",
                    EmergencyContactTelephone = 922222223,

                    ApplicationDate = DateTime.Now.Date
                };
                if(application != null)
                {
                    _context.Application.Add(application);
                    _context.SaveChanges();
                }

            }
            catch
            {

            }

            return RedirectToAction("MyApplications", "Application");
        }

        private async Task<Student> GetStudent()
        {
            return await _context.Student
                .Include(s => s.IdAccountNavigation)
                .Include(s => s.Application)
                .FirstOrDefaultAsync();
        }

        public bool HasConfirmedApp(int intIdStudent)
        {
            return _context.Application
                .Where(s => s.IdStudent == intIdStudent)
                .Include(a => a.IdStateNavigation)
                .Count(m => m.IdStateNavigation.Description == "Confirmada") > 0;
        }

        public void AddApplicationNotification()
        {
            Notification not = new Notification
            {
                ReadNotification = false,
                Description = "Candidatura a mobilidade submetida a " + DateTime.Now.ToString("dd/MM/yyyy"),
                ControllerName = "Application",
                ActionName = "MyApplications",
                NotificationDate = DateTime.Now,
                IdAccountNavigation = _context.Account.Where(a => a.IdAccount == 1).FirstOrDefault()
            };
            _context.Notification.Add(not);
            _context.SaveChanges();
        }

        #endregion

        //MODULO 1

        [Fact]
        public void ApplicationEnoughCreditsTest()
        {
            var student = GetStudent().Result;

            Assert.True(student.hasEnoughCredits());
        }

        [Fact]
        public void ApplicationNotEnoughCreditsTest()
        {
            var student = GetStudent().Result;
            student.Credits = 30;

            Assert.False(student.hasEnoughCredits());
        }

        [Fact]
        public void ApplicationHasNotMaxApplicationsTest()
        {
            var student = GetStudent().Result;

            Assert.True(student.hasNotMaxApplicationNumber());
        }

        [Fact]
        public void ApplicationHasMaxApplicationsTest()
        {
            var student = GetStudent().Result;

            student.Application.Add(new Application());
            
            Assert.False(student.hasNotMaxApplicationNumber());
        }

        [Fact]
        public void ApplicationGetStudentByIdExistentTest()
        {
            var student = _controller.GetStudentById(1);

            Assert.Equal("Estudante", student.Name);
        }

        [Fact]
        public void ApplicationGetStudentByIdNonExistentTest()
        {
            var student = _controller.GetStudentById(30);

            Assert.Null(student);
        }

        [Fact]
        public void ApplicationGetEmailExistentTest()
        {
            var email = _controller.GetEmail(1);

            Assert.Equal("email@exemplo.com", email);
        }

        [Fact]
        public void ApplicationGetEmailNonExistentTest()
        {
            var email = _controller.GetEmail(30);

            Assert.Equal("", email);
        }

        [Fact]
        public void ApplicationDeleteApplicationTest()
        {
            int countBefore = GetStudent().Result.Application.Count;

            var actionResultTask = DeleteApplication(1);
            var viewResult = actionResultTask as System.Web.Mvc.ViewResult;

            int countAfter = GetStudent().Result.Application.Count;

            Assert.NotEqual(countBefore, countAfter);
        }
        
        [Fact]
        public void ApplicationSubmitApplicationTest()
        {
            int countBefore = _context.Application.Count();

            var actionResultTask = SubmitApplication();
            var viewResult = actionResultTask as System.Web.Mvc.ViewResult;

            int countAfter = _context.Application.Count();

            Assert.NotEqual(countBefore,countAfter);
        }


        //MODULO 2
        [Fact]
        public void ApplicationHasConfirmedAppTest()
        {
            bool bolHasConfirmedApp = HasConfirmedApp(1);

            Assert.True(bolHasConfirmedApp);
        }

        [Fact]
        public void ApplicationHasNoConfirmedAppTest()
        {
            bool bolHasConfirmedApp = HasConfirmedApp(2);

            Assert.False(bolHasConfirmedApp);
        }

        [Fact]
        public void ApplicationGetIdByInstitutionTest()
        {
            long idInstitution = _controller.GetIdByInstitution("Instituição");

            Assert.Equal(1, idInstitution);
        }

        [Fact]
        public void ApplicationGetIdByInstitutionInvalidTest()
        {
            long idInstitution = _controller.GetIdByInstitution("Escola");

            Assert.Equal(0, idInstitution);
        }

        [Fact]
        public void ApplicationGetNewApplicationIDTest()
        {
            long lngNewAppId = _controller.GetNewApplicationID();

            Assert.Equal(3, lngNewAppId);
        }

        [Fact]
        public void ApplicationAddNotificationTest()
        {
            AddApplicationNotification();

            int intNotificationCount = _context.Notification.Count();

            Assert.Equal(1, intNotificationCount);
        }

        [Fact]
        public void ApplicationCalculateEvaluationTest()
        {
            ApplicationEvaluation applicationEvaluation = _context.ApplicationEvaluation.Where(a => a.IdApplicationEvaluation == 1).SingleOrDefault();

            Assert.NotNull(applicationEvaluation);

            double dblEvaluation = applicationEvaluation.CalculateEvaluation();

            Assert.Equal(79.6, dblEvaluation);
        }

        [Fact]
        public void ApplicationIsInterviewDoneTest()
        {
            Interview interview = _context.Interview.Where(i => i.IdInterview == 1).SingleOrDefault();

            Assert.NotNull(interview);

            Assert.True(interview.IsInterviewDone());
        }

        [Fact]
        public void ApplicationIsInterviewNotDoneTest()
        {
            Interview interview = _context.Interview.Where(i => i.IdInterview == 2).SingleOrDefault();

            Assert.NotNull(interview);

            Assert.False(interview.IsInterviewDone());
        }
    }
}
