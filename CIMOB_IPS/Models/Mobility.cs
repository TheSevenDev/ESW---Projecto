using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CIMOB_IPS.Models
{
    public partial class Mobility
    {
        public long IdMobility { get; set; }
        public long IdApplication { get; set; }
        public long IdState { get; set; }
        public long IdResponsibleTechnician { get; set; }
        public long IdOutgoingInstitution { get; set; }

        public Application IdApplicationNavigation { get; set; }

        [DisplayName("Instituição de destino")]
        public Institution IdOutgoingInstitutionNavigation { get; set; }

        [DisplayName("Técnico Responsável")]
        public Technician IdResponsibleTechnicianNavigation { get; set; }

        public State IdStateNavigation { get; set; }
    }
}
