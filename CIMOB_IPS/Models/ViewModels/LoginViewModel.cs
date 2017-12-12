using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    /// <summary>
    ///  Class used to provide login credentials to the View. It contains the email, password and the remember me option.
    /// </summary>
    public class LoginViewModel
    {

        /// <summary>
        /// Property that represents the email field of the form.
        /// </summary>
        /// <value>Email credential</value>
        [Required(ErrorMessage = "Campo email vazio")]
        [EmailAddress(ErrorMessage = "O campo tem de ser um email")]
        public string Email { get; set; }

        /// <summary>
        /// Property that represents the password field of the form.
        /// </summary>
        /// <value>Password credential</value>
        [Required(ErrorMessage = "Campo password vazio")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// </summary>
        /// <value> RememberMe credential which maintains the browser login session on, until the user logouts.</value>
        [Display(Name = "Manter sessão")]
        public bool RememberMe { get; set; }

    }
}
