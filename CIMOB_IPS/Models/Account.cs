using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
//COMMIT

namespace CIMOB_IPS.Models
{
    public partial class Account
    {
        public Account()
        {
            Notification = new HashSet<Notification>();
            Student = new HashSet<Student>();
            Technician = new HashSet<Technician>();
        }

        public long IdAccount { get; set; }

        [Required(ErrorMessage = "O email não preenchido")]
        [EmailAddress(ErrorMessage = "O email deverá conter a seguinte estrutura: exemplo@dominio.com")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A password não está preenchida")]
        [Display(Name = "Password:")]
        [DataType(DataType.Password)]
        public byte[] Password { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "A confirmação da password não está preenchida")]
        [Compare("Password", ErrorMessage = "As passwords não coincidem.")]
        [Display(Name = "Confirme a Password:")]
        [DataType(DataType.Password)]
        public byte[] ConfirmPassword { get; set; }

        public ICollection<Notification> Notification { get; set; }
        public ICollection<Student> Student { get; set; }
        public ICollection<Technician> Technician { get; set; }
    }
}
