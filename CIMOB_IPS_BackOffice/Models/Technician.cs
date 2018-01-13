using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIMOB_IPS_BackOffice.Models
{
    public class Technician
    {
        public long IdTechnician { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public long Telephone { get; set; }
        public bool IsAdmin { get; set; }
        public bool Active { get; set; }
    }
}
