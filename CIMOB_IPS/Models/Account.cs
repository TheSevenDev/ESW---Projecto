using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Account
    {
        public Account()
        {
            Notification = new HashSet<Notification>();
            Student = new HashSet<Student>();
            Technician = new HashSet<Technician>();
        }

        public long IdAccount { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }

        public ICollection<Notification> Notification { get; set; }
        public ICollection<Student> Student { get; set; }
        public ICollection<Technician> Technician { get; set; }
    }
}
