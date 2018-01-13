using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    /// <summary>Class used to represent the Technician from CIMOB. This technician will contain an ID for both himself and for the account that is associated to him.
    /// Al</summary>
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
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Nº Telemóvel não está preenchido")]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "O Nº Telemovel tem de ser expresso em 9 algarismos.")]
        [Display(Name = "Nº Telemóvel")]
        public long Telephone { get; set; }

        [Display(Name = "Administrador")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Activo")]
        public bool Active { get; set; }

        public Account IdAccountNavigation { get; set; }
        public ICollection<Mobility> Mobility { get; set; }
    }
}
