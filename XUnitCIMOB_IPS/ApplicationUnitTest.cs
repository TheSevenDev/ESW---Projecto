using CIMOB_IPS.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XUnitCIMOB_IPS
{
    public class ApplicationUnitTest
    {
        [Fact]
        public void hasEnoughCredits_true_hasEnoughCredits()
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
            Assert.True(student.hasEnoughCredits());
        }

        [Fact]
        public void hasEnoughCredits_false_hasEnoughCredits()
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
            student.Credits = 35;
            student.IdAccountNavigation = new Account
            {
                IdAccount = 1,
                Email = "email@exemplo.com"
            };
            Assert.False(student.hasEnoughCredits());
        }

        [Fact]
        public void hasNotMaxApplicationNumber_true_hasNotMaxApplicationNumber()
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
            student.Credits = 35;
            student.IdAccountNavigation = new Account
            {
                IdAccount = 1,
                Email = "email@exemplo.com"
            };
            student.Application = new List<Application>
            {
                new Application {
                    IdApplication = 1,
                    IdStudent = 1,
                    IdState = 1,
                    HasScholarship = false,
                    FinalEvaluation = 15,
                    MotivationCard = "",
                    EmergencyContactName = "Mae Estudante",
                    EmergencyContactRelation = "Mae",
                    EmergencyContactTelephone = 922222222,
                    IdStateNavigation = new State
                    {
                        IdState = 1,
                        Description = "Aberto"
                    },
                    ApplicationDate = DateTime.Now.Date,
                    IdStudentNavigation = student,
                    IdProgramNavigation = new Program
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
                        }
                    }
                },
                new Application {
                    IdApplication = 2,
                    IdStudent = 1,
                    IdState = 1,
                    HasScholarship = false,
                    FinalEvaluation = 15,
                    MotivationCard = "",
                    EmergencyContactName = "Mae Estudante",
                    EmergencyContactRelation = "Mae",
                    EmergencyContactTelephone = 922222222,
                    IdStateNavigation = new State
                    {
                        IdState = 1,
                        Description = "Fechado"
                    },
                    ApplicationDate = DateTime.Now.Date,
                    IdStudentNavigation = student,
                    IdProgramNavigation = new Program
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
                        }
                    }
                }
            };
            Assert.True(student.hasNotMaxApplicationNumber());
        }

        [Fact]
        public void hasNotMaxApplicationNumber_false_hasNotMaxApplicationNumber()
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
            student.Credits = 35;
            student.IdAccountNavigation = new Account
            {
                IdAccount = 1,
                Email = "email@exemplo.com"
            };
            student.Application = new List<Application>
            {
                new Application {
                    IdApplication = 1,
                    IdStudent = 1,
                    IdState = 1,
                    HasScholarship = false,
                    FinalEvaluation = 15,
                    MotivationCard = "",
                    EmergencyContactName = "Mae Estudante",
                    EmergencyContactRelation = "Mae",
                    EmergencyContactTelephone = 922222222,
                    IdStateNavigation = new State
                    {
                        IdState = 1,
                        Description = "Aberto"
                    },
                    ApplicationDate = DateTime.Now.Date,
                    IdStudentNavigation = student,
                    IdProgramNavigation = new Program
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
                        }
                    }
                },
                new Application {
                    IdApplication = 2,
                    IdStudent = 1,
                    IdState = 1,
                    HasScholarship = false,
                    FinalEvaluation = 15,
                    MotivationCard = "",
                    EmergencyContactName = "Mae Estudante",
                    EmergencyContactRelation = "Mae",
                    EmergencyContactTelephone = 922222222,
                    IdStateNavigation = new State
                    {
                        IdState = 1,
                        Description = "Fechado"
                    },
                    ApplicationDate = DateTime.Now.Date,
                    IdStudentNavigation = student,
                    IdProgramNavigation = new Program
                    {
                        IdProgram = 1,
                        IdState = 1,
                        CreationDate = new DateTime(2016, 11, 28),
                        OpenDate = new DateTime(2016, 11, 30),
                        ClosingDate = new DateTime(2016, 12, 30),
                        MobilityDate = new DateTime(2016, 12, 03),
                        Vacancies = 2,
                        IdProgramType = 1,
                        IdProgramTypeNavigation = new ProgramType
                        {
                            IdProgramType = 1,
                            Name = "Program",
                            Description = "Program muito fixe",
                            ImageFile = "File"
                        }
                    }
                },
                new Application {
                    IdApplication = 3,
                    IdStudent = 1,
                    IdState = 1,
                    HasScholarship = false,
                    FinalEvaluation = 15,
                    MotivationCard = "",
                    EmergencyContactName = "Mae Estudante",
                    EmergencyContactRelation = "Mae",
                    EmergencyContactTelephone = 922222222,
                    IdStateNavigation = new State
                    {
                        IdState = 1,
                        Description = "Fechado"
                    },
                    ApplicationDate = DateTime.Now.Date,
                    IdStudentNavigation = student,
                    IdProgramNavigation = new Program
                    {
                        IdProgram = 1,
                        IdState = 1,
                        CreationDate = new DateTime(2017, 11, 28),
                        OpenDate = new DateTime(2017, 11, 30),
                        ClosingDate = new DateTime(2017, 12, 30),
                        MobilityDate = new DateTime(2017, 12, 03),
                        Vacancies = 2,
                        IdProgramType = 1,
                        IdProgramTypeNavigation = new ProgramType
                        {
                            IdProgramType = 1,
                            Name = "Program",
                            Description = "Program muito fixe",
                            ImageFile = "File"
                        }
                    }
                }
            };
            Assert.False(student.hasNotMaxApplicationNumber());
        }
    }
}
