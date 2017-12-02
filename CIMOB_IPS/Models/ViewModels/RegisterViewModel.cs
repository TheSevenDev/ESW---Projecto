using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    public class RegisterViewModel
    {
        public Account Account{get; set;}

        public Student Student { get; set; }

        public Technician Technician { get; set; }       
    }
}
