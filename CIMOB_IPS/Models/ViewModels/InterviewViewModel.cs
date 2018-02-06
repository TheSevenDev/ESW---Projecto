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

        [CheckIfDateIsAfter]
        [Required(ErrorMessage = "É necessário seleccionar a data.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "É necessário seleccionar a hora.")]
        public int Hours { get; set; }

        [Required(ErrorMessage = "É necessário seleccionar os minutos.")]
        public int Minutes { get; set; }
    }
}
