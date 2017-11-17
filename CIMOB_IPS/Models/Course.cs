using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Course
    {
        public Course()
        {
            Student = new HashSet<Student>();
        }

        public long IdCourse { get; set; }
        public long IdInstitution { get; set; }
        public string Name { get; set; }

        public Institution IdInstitutionNavigation { get; set; }
        public ICollection<Student> Student { get; set; }
    }
}
