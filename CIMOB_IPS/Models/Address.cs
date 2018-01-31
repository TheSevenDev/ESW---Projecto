using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    public class Address
    {
        public long IdAddress { get; set; }

        [Display(Name = "Código Postal:")]
        public string PostalCode { get; set; }

        [Display(Name = "Morada:")]
        [Required(ErrorMessage = "Insira a sua morada.")]
        public string AddressDesc { get; set; }

        [Display(Name = "N.º da Porta:")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "O nº da porta tem de ser expresso em algarismos.")]
        public short DoorNumber { get; set; }

        [Display(Name = "Andar:")]
        public string Floor { get; set; }

        public ICollection<Student> Student { get; set; }
    }
}
