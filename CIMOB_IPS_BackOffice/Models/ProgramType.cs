using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIMOB_IPS_BackOffice.Models
{
    public class ProgramType
    {
        public long ProgramTypeId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        override
        public string ToString()
        {
            return ("" + ProgramTypeId + " - " + Name);
        }
    }
}
