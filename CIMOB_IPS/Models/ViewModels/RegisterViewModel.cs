using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    public class RegisterViewModel
    {
        public Account Account{get; set;}

        [Required(ErrorMessage = "O número de Estudante não está preenchido.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "O Nº Estudante tem de ser expresso em algarismos.")]
        [Display(Name = "Número de Estudante:")]
        public long StudentNum { get; set; }


        public Student Student { get; set; }

        public Technician Technician { get; set; }       
    }
}
