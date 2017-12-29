using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    public partial class Application
    {
        public Application()
        {
            ApplicationInstitutions = new HashSet<ApplicationInstitutions>();
            Mobility = new HashSet<Mobility>();
            HasScholarship = false;
        }

        public long IdApplication { get; set; }
        public long IdStudent { get; set; }
        public long IdState { get; set; }
        public long? IdProgram { get; set; }

        [Display(Name = "Candidato a bolsa")]
        public bool HasScholarship { get; set; }

        [Display(Name = "Avaliação Final")]
        public short FinalEvaluation { get; set; }

        [Display(Name = "Carta de Motivação")]
        [DataType(DataType.MultilineText)]
        public string MotivationCard { get; set; }

        [Display(Name = "Nome Completo")]
        [Required(ErrorMessage = "O nome não está preenchido.")]
        public string EmergencyContactName { get; set; }

        [Display(Name = "Relação com o candidato")]
        [Required(ErrorMessage = "A relação não está preenchida.")]
        public string EmergencyContactRelation { get; set; }

        [Display(Name = "Nº Telemóvel")]
        [Required(ErrorMessage = "O nº telemóvel não está preenchido.")]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "O Nº Telemovel tem de ser expresso em 9 algarismos.")]
        public long EmergencyContactTelephone { get; set; }

        [Display(Name = "Estado")]
        public State IdStateNavigation { get; set; }

        [Display(Name = "Submetida a")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime ApplicationDate { get; set; }

        public Student IdStudentNavigation { get; set; }

        [Display(Name = "Programa")]
        public Program IdProgramNavigation { get; set; }

        [Display(Name = "Instituições")]
        public ICollection<ApplicationInstitutions> ApplicationInstitutions { get; set; }
        public ICollection<Mobility> Mobility { get; set; }

    }
}
