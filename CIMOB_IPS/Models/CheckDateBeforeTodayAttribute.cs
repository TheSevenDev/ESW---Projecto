using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed class CheckDateRangeAttribute : ValidationAttribute
    {
        public CheckDateRangeAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            DateTime dt = (DateTime)value;
            if (dt >= DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

    }
}
