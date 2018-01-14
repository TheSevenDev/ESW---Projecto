using CIMOB_IPS.Controllers;
using CIMOB_IPS.Models;
using CIMOB_IPS.Views;
using Microsoft.AspNetCore.Mvc;
using System;
using Xunit;

namespace XUnitCIMOB_IPS
{
    public class UnitTests
    {
        /*
        [Fact]
        public void TestCreateStudent()
        {
            var student = new Student();

            student.IdStudent = 1;
            student.IdAccount = 1;
            student.StudentNum = 150221066;
            student.IdCourse = 1;
            student.Name = "Estudante";
            student.Cc = 15131610;
            student.Telephone = 911111111;
            student.IdNationality = 1;
            student.Credits = 120;
            student.IdAccountNavigation = new Account
            {
                IdAccount = 1,
                Email = "email@exemplo.com"
            };

            Assert.Equal(1, student.IdAccount);
            Assert.Equal(1, student.IdStudent);
            Assert.Equal("Estudante", student.Name);
            Assert.Equal(150221066, student.StudentNum);
            Assert.Equal(1, student.IdCourse);
            Assert.Equal(911111111, student.Telephone);
            Assert.Equal(15131610, student.Cc);
            Assert.Equal(1, student.IdNationality);
            Assert.Equal(120, student.Credits);
            Assert.Equal(1, student.IdAccountNavigation.IdAccount);
            Assert.Equal("email@exemplo.com", student.IdAccountNavigation.Email);
        }

        [Fact]
        public void TestCreateTechnician()
        {
            var technician = new Technician
            {
                IdTechnician = 1,
                IdAccount = 2,
                IsAdmin = true,
                Name = "Docente",
                Telephone = 922222222,
                IdAccountNavigation = new Account
                {
                    IdAccount = 2,
                    Email = "email2@exemplo.com"
                }
            };

            Assert.Equal(2, technician.IdAccount);
            Assert.Equal(1, technician.IdTechnician);
            Assert.Equal("Docente", technician.Name);
            Assert.True(technician.IsAdmin);
            Assert.Equal(922222222, technician.Telephone);
            Assert.Equal(2, technician.IdAccountNavigation.IdAccount);
            Assert.Equal("email2@exemplo.com", technician.IdAccountNavigation.Email);
        }
        */
        [Fact]
        public void TestIndexRedirection()
        {
            var controller = new HomeController();
            var viewResult = (ViewResult)controller.Index();
            var viewName = viewResult.ViewName;

            Assert.True(string.IsNullOrEmpty(viewName) || viewName == "Index");
        }
    }
}
