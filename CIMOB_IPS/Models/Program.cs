using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    public partial class Program
    {
        public Program()
        {
            Application = new HashSet<Application>();
            InstitutionProgram = new HashSet<InstitutionProgram>();
        }

        public long IdProgram { get; set; }
        public long IdState { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreationDate { get; set; }

        [Required(ErrorMessage = "É necessário definir a data de abertura!")]
        [Display(Name = "Data de Abertura")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? OpenDate { get; set; }

        [Required(ErrorMessage = "É necessário definir a data de encerramento!")]
        [Display(Name = "Data de Fecho")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? ClosingDate { get; set; }


        [Required(ErrorMessage = "É necessário definir a data de mobilidade!")]
        [Display(Name = "Data prevista de Mobilidade")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime MobilityDate { get; set; }


        [Range(0, int.MaxValue, ErrorMessage = "O número de vagas tem de ser um número positivo maior que 0.")]
        [Required(ErrorMessage = "É necessário definir o número de vagas!")]
        [Display(Name = "Vagas para candidaturas")]
        public int Vacancies { get; set; }

        public long IdProgramType { get; set; }

        [Display(Name = "Programa")]
        public ProgramType IdProgramTypeNavigation { get; set; }

        [Display(Name = "Estado")]
        public State IdStateNavigation { get; set; }
        public ICollection<Application> Application { get; set; }

        [Display(Name = "Instituições disponíveis para mobilidade")]
        public ICollection<InstitutionProgram> InstitutionProgram { get; set; }

        public bool isOpenProgram()
        {
            return IdStateNavigation.Description.Equals("Aberto");
        }

        public bool withVacanciesAvailable()
        {
            return Vacancies > 0;
        }

        public bool withDateAvailable()
        {
            return DateTime.Now.Date > OpenDate && DateTime.Now.Date < ClosingDate;
        }

        public bool withPossibleApplication()
        {
            return isOpenProgram() && withVacanciesAvailable() && withDateAvailable();
        }
    }
}
