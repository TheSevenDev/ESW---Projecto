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
                }
            });

            _context.Application.Add(new Application() 
            {
                IdApplication = 1,
                IdStudent = 1,
                IdState = 1,
                HasScholarship = false,
                FinalEvaluation = 15,
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
                    IdState = 1,
                    HasScholarship = false,
                    FinalEvaluation = 15,
                    MotivationCard = "",
                    EmergencyContactName = "Mae Estudante",
                    EmergencyContactRelation = "Mae",
                    EmergencyContactTelephone = 922222222,

                    ApplicationDate = DateTime.Now.Date
            });

            _context.SaveChanges();
            _controller = new ApplicationController(_context);
        }

        #region Aux Functions
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

        #endregion

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
    }
}
