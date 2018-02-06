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
                DateTime date = Convert.ToDateTime(value);
                if (date < DateTime.Now)
                {
                    return new ValidationResult("A data não pode ser anterior a hoje");
                }
            }

            return ValidationResult.Success;
        }
    }
}
