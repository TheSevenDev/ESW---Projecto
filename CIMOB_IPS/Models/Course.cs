using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Display(Name="Curso")]
        public string Name { get; set; }

        public Institution IdInstitutionNavigation { get; set; }
        public ICollection<Student> Student { get; set; }
        public ICollection<Coordenator> Coordenator { get; set; }
    }
}
