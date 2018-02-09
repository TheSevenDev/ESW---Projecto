using CIMOB_IPS.Models.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models.ViewModels
{
    public class InterviewViewModel
    {
        public long IdInterview { get; set; }

        public long IdApplication { get; set; }

        [Display(Name ="Dia")]
        [CheckIfDateIsAfter]       
        [Required(ErrorMessage = "É necessário seleccionar o dia.")]
        public DateTime Date { get; set; }

        [Display(Name ="Hora")]
        [DataType(DataType.Time)]
        [Required(ErrorMessage = "É necessário seleccionar a hora.")]
        public DateTime Hours { get; set; }

    }
}
