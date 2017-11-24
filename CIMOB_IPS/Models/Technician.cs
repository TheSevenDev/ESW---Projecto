using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    public partial class Technician : Account
    {
        public Technician()
        {
            Mobility = new HashSet<Mobility>();
        }

        public long IdTechnician { get; set; }
        public long IdAccount { get; set; }
        [Required]
        [StringLength(60)]
        public string Name { get; set; }
        [Required]
        public long Telephone { get; set; }
        public bool IsAdmin { get; set; }

        public Account IdAccountNavigation { get; set; }
        public ICollection<Mobility> Mobility { get; set; }
    }
}
