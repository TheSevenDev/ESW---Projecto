using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{

    /// <summary>
    /// Classe usada para validar se a data de fecho da mobilidade é superior à data de ínicio.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class IsEndMobilityAfterBeginDate : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (ViewModels.ProgramViewModel)validationContext.ObjectInstance;
            DateTime StartDate = Convert.ToDateTime(model.MobilityBeginDate);
            DateTime EndDate = Convert.ToDateTime(value);

            if (StartDate > EndDate)
            {
                return new ValidationResult("A data de término de mobilidade tem de ser superior à data de inicio.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
