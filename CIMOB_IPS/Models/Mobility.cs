using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Mobility
    {
        public long IdMobility { get; set; }
        public long IdApplication { get; set; }
        public long IdState { get; set; }
        public long IdResponsibleTechnician { get; set; }
        public long IdOutgoingInstitution { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Application IdApplicationNavigation { get; set; }
        public Institution IdOutgoingInstitutionNavigation { get; set; }
        public Technician IdResponsibleTechnicianNavigation { get; set; }
    }
}
