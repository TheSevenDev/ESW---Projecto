using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models.CustomValidations
{
    /// <summary>
    /// Classe usada para validar se uma data é inferior à data atual
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CheckIfDateIsBefore : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateTime _birthJoin = Convert.ToDateTime(value);
                if (_birthJoin >= DateTime.Now)
                {
                    return new ValidationResult("A data de nascimento tem de ser anterior a hoje.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
