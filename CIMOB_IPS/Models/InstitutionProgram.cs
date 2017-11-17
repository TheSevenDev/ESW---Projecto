using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class InstitutionProgram
    {
        public long IdProgram { get; set; }
        public long IdOutgoingInstitution { get; set; }

        public Institution IdOutgoingInstitutionNavigation { get; set; }
        public Program IdProgramNavigation { get; set; }
    }
}
