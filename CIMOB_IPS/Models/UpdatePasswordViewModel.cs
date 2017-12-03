using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    public class UpdatePasswordViewModel
    {
        public Account Account { get; set; }
        public string NewPassword { get; set; }
        public string Confirmation { get; set; }
    }
}
