using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    public partial class Application
    {
        public Application()
        {
            Mobility = new HashSet<Mobility>();
        }

        public long IdApplication { get; set; }
        public long IdStudent { get; set; }
        public long IdState { get; set; }
        public bool HasScholarship { get; set; }
        public short FinalEvaluation { get; set; }

        [Display(Name = "Carta de Motivação")]
        public string MotivationCard { get; set; }

        [Display(Name = "Nome Completo")]
        public string EmergencyContactName { get; set; }

        [Display(Name = "Relação com o candidato")]
        public string EmergencyContactRelation { get; set; }

        [Display(Name = "Nº Telemóvel")]
        public long EmergencyContactTelephone { get; set; }

        public State IdStateNavigation { get; set; }
        public Student IdStudentNavigation { get; set; }
        public ICollection<Mobility> Mobility { get; set; }
    }
}
