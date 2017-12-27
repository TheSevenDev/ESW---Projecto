using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class ApplicationInstitutions
    {
        public long IdApplication { get; set; }
        public long IdInstitution { get; set; }
        public short InstitutionOrder { get; set; }

        public Application IdApplicationNavigation { get; set; }
        public Institution IdInstitutionNavigation { get; set; }
    }
}
