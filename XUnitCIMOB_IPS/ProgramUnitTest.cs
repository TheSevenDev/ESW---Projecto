using CIMOB_IPS.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XUnitCIMOB_IPS
{
    public class ProgramUnitTest
    {
        [Fact]
        public void isOpenProgram_true_whenIsOpenProgram()
        {
            var program = new Program();

            program.IdProgram = 1;
            program.IdState = 1;
            program.CreationDate = new DateTime(2017, 12, 28);
            program.OpenDate = new DateTime(2018, 01, 03);
            program.ClosingDate = new DateTime(2018, 02, 13);
            program.MobilityDate = new DateTime(2018, 01, 10);
            program.Vacancies = 2;
            program.IdProgramType = 1;
            program.IdProgramTypeNavigation = new ProgramType
            {
                IdProgramType = 1,
                Name = "Program",
                Description = "Program muito fixe",
                ImageFile = "File"
            };
            program.IdStateNavigation = new State
            {
                IdState = 1,
                Description = "Aberto"
            };

            Assert.True(program.isOpenProgram());
        }

        [Fact]
        public void isOpenProgram_false_whenIsOpenProgram()
        {
            var program = new Program();

            program.IdProgram = 1;
            program.IdState = 1;
            program.CreationDate = new DateTime(2017, 12, 28);
            program.OpenDate = new DateTime(2018, 01, 03);
            program.ClosingDate = new DateTime(2018, 02, 13);
            program.MobilityDate = new DateTime(2018, 01, 10);
            program.Vacancies = 2;
            program.IdProgramType = 1;
            program.IdProgramTypeNavigation = new ProgramType
            {
                IdProgramType = 1,
                Name = "Program",
                Description = "Program muito fixe",
                ImageFile = "File"
            };
            program.IdStateNavigation = new State
            {
                IdState = 1,
                Description = "Fechado"
            };

            Assert.False(program.isOpenProgram());
        }

        [Fact]
        public void withVacanciesAvailable_true_withVacanciesAvailable()
        {
            var program = new Program();

            program.IdProgram = 1;
            program.IdState = 1;
            program.CreationDate = new DateTime(2017, 12, 28);
            program.OpenDate = new DateTime(2018, 01, 03);
            program.ClosingDate = new DateTime(2018, 02, 13);
            program.MobilityDate = new DateTime(2018, 01, 10);
            program.Vacancies = 2;
            program.IdProgramType = 1;
            program.IdProgramTypeNavigation = new ProgramType
            {
                IdProgramType = 1,
                Name = "Program",
                Description = "Program muito fixe",
                ImageFile = "File"
            };
            program.IdStateNavigation = new State
            {
                IdState = 1,
                Description = "Aberto"
            };

            Assert.True(program.withVacanciesAvailable());
        }

        [Fact]
        public void withVacanciesAvailable_false_withVacanciesAvailable()
        {
            var program = new Program();

            program.IdProgram = 1;
            program.IdState = 1;
            program.CreationDate = new DateTime(2017, 12, 28);
            program.OpenDate = new DateTime(2018, 01, 03);
            program.ClosingDate = new DateTime(2018, 02, 13);
            program.MobilityDate = new DateTime(2018, 01, 10);
            program.Vacancies = 2;
            program.IdProgramType = 1;
            program.IdProgramTypeNavigation = new ProgramType
            {
                IdProgramType = 1,
                Name = "Program",
                Description = "Program muito fixe",
                ImageFile = "File"
            };
            program.IdStateNavigation = new State
            {
                IdState = 1,
                Description = "Aberto"
            };

            Assert.False(program.withVacanciesAvailable());
        }

        [Fact]
        public void withDateAvailable_true_withDateAvailable()
        {
            var program = new Program();

            program.IdProgram = 1;
            program.IdState = 1;
            program.CreationDate = new DateTime(2017, 12, 28);
            program.OpenDate = new DateTime(2018, 01, 03);
            program.ClosingDate = new DateTime(2018, 02, 13);
            program.MobilityDate = new DateTime(2018, 01, 10);
            program.Vacancies = 2;
            program.IdProgramType = 1;
            program.IdProgramTypeNavigation = new ProgramType
            {
                IdProgramType = 1,
                Name = "Program",
                Description = "Program muito fixe",
                ImageFile = "File"
            };
            program.IdStateNavigation = new State
            {
                IdState = 1,
                Description = "Aberto"
            };

            Assert.True(program.withDateAvailable());
        }

        [Fact]
        public void withDateAvailable_false_withDateAvailable()
        {
            var program = new Program();

            program.IdProgram = 1;
            program.IdState = 1;
            program.CreationDate = new DateTime(2017, 12, 28);
            program.OpenDate = new DateTime(2018, 01, 03);
            program.ClosingDate = new DateTime(2018, 02, 13);
            program.MobilityDate = new DateTime(2018, 01, 10);
            program.Vacancies = 2;
            program.IdProgramType = 1;
            program.IdProgramTypeNavigation = new ProgramType
            {
                IdProgramType = 1,
                Name = "Program",
                Description = "Program muito fixe",
                ImageFile = "File"
            };
            program.IdStateNavigation = new State
            {
                IdState = 1,
                Description = "Aberto"
            };

            Assert.False(program.withDateAvailable());
        }

        [Fact]
        public void withPossibleApplication_true_withPossibleApplication()
        {
            var program = new Program();

            program.IdProgram = 1;
            program.IdState = 1;
            program.CreationDate = new DateTime(2017, 12, 28);
            program.OpenDate = new DateTime(2018, 01, 03);
            program.ClosingDate = new DateTime(2018, 02, 13);
            program.MobilityDate = new DateTime(2018, 01, 10);
            program.Vacancies = 2;
            program.IdProgramType = 1;
            program.IdProgramTypeNavigation = new ProgramType
            {
                IdProgramType = 1,
                Name = "Program",
                Description = "Program muito fixe",
                ImageFile = "File"
            };
            program.IdStateNavigation = new State
            {
                IdState = 1,
                Description = "Aberto"
            };

            Assert.True(program.withPossibleApplication());
        }

        [Fact]
        public void withPossibleApplication_false_withPossibleApplication()
        {
            var program = new Program();

            program.IdProgram = 1;
            program.IdState = 1;
            program.CreationDate = new DateTime(2017, 12, 28);
            program.OpenDate = new DateTime(2018, 01, 03);
            program.ClosingDate = new DateTime(2018, 02, 13);
            program.MobilityDate = new DateTime(2018, 01, 10);
            program.Vacancies = 2;
            program.IdProgramType = 1;
            program.IdProgramTypeNavigation = new ProgramType
            {
                IdProgramType = 1,
                Name = "Program",
                Description = "Program muito fixe",
                ImageFile = "File"
            };
            program.IdStateNavigation = new State
            {
                IdState = 1,
                Description = "Aberto"
            };

            Assert.False(program.withPossibleApplication());
        }
    }
}
