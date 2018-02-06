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
        public long IdProgram { get; set; }
        public long IdInterview { get; set; }

        [Display(Name = "Candidato a bolsa")]
        public bool HasScholarship { get; set; }

        [Display(Name = "Avaliação Final")]
        public short FinalEvaluation { get; set; }

        [Required(ErrorMessage = "Preencha a carta de motivação.")]
        [MinLength(40, ErrorMessage = "Insira, no mínimo, 40 caracteres.")]
        [MaxLength(3000, ErrorMessage = "Insira, no máximo, 3000 caracteres.")]
        [Display(Name = "Carta de Motivações")]
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
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ApplicationDate { get; set; }

        public Student IdStudentNavigation { get; set; }

        [Display(Name = "Programa")]
        public Program IdProgramNavigation { get; set; }

        [Display(Name = "Entrevista")]
        public Interview IdInterviewNavigation { get; set; }

        [Display(Name = "Comprovativo de candidatura")]
        public byte[] SignedAppFile { get; set; }

        [Display(Name = "Instituições")]
        public ICollection<ApplicationInstitutions> ApplicationInstitutions { get; set; }
        public ICollection<Mobility> Mobility { get; set; }


        public bool IsAvailable()
        {
            return IdStateNavigation.Description.Equals("Aberto");
        }
    }
}
