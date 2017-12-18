using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    public partial class PendingAccount
    {
        public long IdPending { get; set; }
        public string Email { get; set; }
        public string Guid { get; set; }

        [Display(Name = "Administrador")]
        public bool IsAdmin { get; set; }
    }
}
