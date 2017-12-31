using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIMOB_IPS.Models
{
    /// <summary>Class used to provide the register credentials to the RegisterView. Contains an Account, Student, Technician and all the nationalities.</summary>
    public class RegisterViewModel
    {
        /// <summary>Property used to represent the new account created. From this property there will be an email, password and password confirmation camps to fill.</summary>
        /// <value>New generated account with the given email and password.</value>
        public Account Account { get; set; }

        public long IdAccount { get; set; }
        [Required(ErrorMessage = "O email não preenchido")]
        [EmailAddress(ErrorMessage = "O email deverá conter a seguinte estrutura: exemplo@dominio.com")]
        [Display(Name = "E-mail:")]
        public string EmailView { get; set; }


        [Required(ErrorMessage = "A password não está preenchida")]
        [Display(Name = "Password:")]
        [DataType(DataType.Password)]
        public string PasswordView { get; set; }


        [Required(ErrorMessage = "A confirmação da password não está preenchida")]
        [Compare("PasswordView", ErrorMessage = "As passwords não coincidem.")]
        [Display(Name = "Confirme a Password:")]
        [DataType(DataType.Password)]
        public string ConfirmPasswordView { get; set; }

        [Required(ErrorMessage = "É necessário preencher o código postal")]
        [RegularExpression("^[0-9]{4}$",ErrorMessage =("O código postal deve ter 4 algarismos"))]
        public string PostalCode1 { get; set; }

        [Required(ErrorMessage = "É necessário preencher o código postal")]
        [RegularExpression("^[0-9]{3}$", ErrorMessage = ("O código postal deve ter 3 algarismos"))]
        public string PostalCode2 { get; set; }


        /// <summary>Property used to represent the new Student created with the values provided in the form.  </summary>
        /// <value>New generated Student with the given data.</value>
        public Student Student { get; set; }

        /// <summary>Property used to represent the new Technician created with the values provided in the form.   </summary>
        /// <value>New generated Technician with the given data.</value>
        public Technician Technician { get; set; }

        /// <summary>Property used to represent the list of the nationalities selected from the database. </summary>
        /// <value>All the nationalities provided by the database. </value>
        public IEnumerable<SelectListItem> Nationalities { get; set; }

        public IEnumerable<SelectListItem> Courses { get; set; }

    }
}
