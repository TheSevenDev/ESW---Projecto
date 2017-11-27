using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    public partial class Technician
    {
        public Technician()
        {
            Mobility = new HashSet<Mobility>();
        }

        public long IdTechnician { get; set; }
        public long IdAccount { get; set; }
        [Required(ErrorMessage = "O Nome não está preenchido.")]
        [StringLength(60, ErrorMessage = "O Nome deve conter no máximo 60 caracteres.")]
        [Display(Name = "Nome:")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Nº Telemóvel não está preenchido")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Nº Telemovel tem de ser expresso em algarismos.")]
        [MinLength(9,ErrorMessage = "O Nº Telemóvel tem de conter no minimo 9 algarismos.")]
        [Display(Name = "Nº Telemóvel:")]
        public long Telephone { get; set; }
        public bool IsAdmin { get; set; }

        public Account IdAccountNavigation { get; set; }
        public ICollection<Mobility> Mobility { get; set; }
    }
}
