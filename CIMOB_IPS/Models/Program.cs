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

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Data de Abertura")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? OpenDate { get; set; }

        [Display(Name = "Data de Fecho")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? ClosingDate { get; set; }

        [Display(Name = "Data prevista de Mobilidade")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime MobilityDate { get; set; }

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
    }
}
