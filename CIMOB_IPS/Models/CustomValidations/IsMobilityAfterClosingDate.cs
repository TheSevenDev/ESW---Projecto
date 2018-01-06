using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models.CustomValidations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class IsMobilityAfterClosingDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (ViewModels.ProgramViewModel)validationContext.ObjectInstance;
            DateTime StartDate = Convert.ToDateTime(model.ClosingDate);
            DateTime EndDate = Convert.ToDateTime(value);

            if (StartDate > EndDate)
            {
                return new ValidationResult("A data de mobilidade tem de ser superior à data de encerramento.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
