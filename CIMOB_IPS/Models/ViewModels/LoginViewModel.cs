using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "Campo email vazio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo password vazio")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Lembrar-me")]
        public bool RememberMe { get; set; }

    }
}
