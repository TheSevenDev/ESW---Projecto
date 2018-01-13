using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIMOB_IPS_BackOffice.Models
{
    public class PendingInvite
    {
        public long IdPendingInvite { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public string Guid { get; set; }
    }
}
