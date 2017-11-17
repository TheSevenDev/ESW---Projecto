using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Nationality
    {
        public Nationality()
        {
            Institution = new HashSet<Institution>();
            Student = new HashSet<Student>();
        }

        public long IdNationality { get; set; }
        public string Description { get; set; }

        public ICollection<Institution> Institution { get; set; }
        public ICollection<Student> Student { get; set; }
    }
}
