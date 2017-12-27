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

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? OpenDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? ClosingDate { get; set; }

        [Display(Name = "Data prevista de Mobilidade")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime MobilityDate { get; set; }

        [Display(Name = "Programa")]
        public string Name { get; set; }
        public int Vacancies { get; set; }

        public State IdStateNavigation { get; set; }
        public ICollection<Application> Application { get; set; }
        public ICollection<InstitutionProgram> InstitutionProgram { get; set; }
    }
}
