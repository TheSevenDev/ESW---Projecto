using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    public class UpdatePasswordViewModel
    {
        
        public long IdAccount { get; set; }

        [Required(ErrorMessage ="Password inserida não é a password atual")]
        [Display(Name="Password Atual:")]
        public string CurrentPassword { get; set; }

        [Display(Name ="Nova Password:")]
        public string NewPassword { get; set; }

        [NotMapped]
        [Required]
        [Compare("NewPassword", ErrorMessage = "As Passwords não coincidem.")]
        [Display(Name ="Confirmar Password:")]
        public string Confirmation { get; set; }
    }
}
