using System.Collections.Generic;

namespace CIMOB_IPS.Models.ViewModels
{
    public class TechnicianManagementViewModel
    {
        public List<PendingAccount> PendingAccounts { get; set; }

        public List<Technician> Technicians { get; set; }
    }
}
