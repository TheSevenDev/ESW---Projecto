using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    public class ProfileViewModel
    {

        public EnumAccountType AccountType { get; set; }

        public Student Student { get; set; }

        public Technician Technician { get; set; }

        public ProfileViewModel()
        {
            Student = new Student();
            Technician = new Technician();
        }
    }


}
