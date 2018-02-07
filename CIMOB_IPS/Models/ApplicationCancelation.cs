using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    public class ApplicationCancelation
    {
        public long IdApplicationCancelation { get; set; }
        public long IdApplication { get; set; }
        public string Reason { get; set; }

        public Application IdApplicationNavigation { get; set; }
    }
}
