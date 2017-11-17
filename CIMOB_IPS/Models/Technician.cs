using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Technician
    {
        public Technician()
        {
            Mobility = new HashSet<Mobility>();
        }

        public long IdTechnician { get; set; }
        public long IdAccount { get; set; }
        public string Name { get; set; }
        public long Telephone { get; set; }

        public Account IdAccountNavigation { get; set; }
        public ICollection<Mobility> Mobility { get; set; }
    }
}
