using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    /// <summary> Class used to provide the credentials to the UpdatePassword view. Contains an IDAccount, current password of the account and the new.</summary>
    public class UpdatePasswordViewModel
    {

        /// <summary>Property that represents an account's identification number synchronized in the database. </summary>
        /// <value>An account's ID.</value>
        public long IdAccount { get; set; }


        /// <summary> Property that represents the current password of the logged in account. This password has to match the actual password of the account.
        /// If it doesn't an error message is shown.</summary>
        /// <value>The current password of the logged in account.</value>
        [Required(ErrorMessage = "Password inserida não é a password atual")]
        [Display(Name = "Password Atual:")]
        public string CurrentPassword { get; set; }


        /// <summary> Property that representa the updated (new) password of the logged in account. </summary>
        /// <value>The new password of the logged in account.</value>
        [Display(Name = "Nova Password:")]
        public string NewPassword { get; set; }

        /// <summary> Property that represents the confirmation of the new password. In order to be valid, this property has to match the NewPassword one. </summary>
        /// <value>The confirmation of the new password for the logged in account.</value>
        [NotMapped]
        [Required]
        [Compare("NewPassword", ErrorMessage = "As Passwords não coincidem.")]
        [Display(Name = "Confirmar Password:")]
        public string Confirmation { get; set; }
    }
}
