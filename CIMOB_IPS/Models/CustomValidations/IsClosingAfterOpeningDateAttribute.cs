using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class IsClosingAfterOpeningDateAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (ViewModels.ProgramViewModel)validationContext.ObjectInstance;
            DateTime StartDate = Convert.ToDateTime(model.OpenDate);
            DateTime EndDate = Convert.ToDateTime(value);

            if (StartDate > EndDate)
            {
                return new ValidationResult("A data de encerramento tem de ser superior à data de abertura.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
