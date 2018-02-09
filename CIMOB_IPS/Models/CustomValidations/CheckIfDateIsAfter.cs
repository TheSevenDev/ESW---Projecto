using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models.CustomValidations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CheckIfDateIsAfter : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var model = (ViewModels.InterviewViewModel)validationContext.ObjectInstance;
                DateTime hours = Convert.ToDateTime(model.Hours);

                DateTime date = Convert.ToDateTime(value);

                DateTime actualDate = new DateTime(date.Year, date.Month, date.Day, hours.Hour, hours.Minute, 0);

                if (actualDate < DateTime.Now)
                {
                    return new ValidationResult("A data não pode ser anterior a hoje");
                }
            }

            return ValidationResult.Success;
        }
    }
}
