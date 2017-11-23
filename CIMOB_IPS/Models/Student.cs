using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Student : Account
    {
        public Student()
        {
            Application = new HashSet<Application>();
        }

        public long IdStudent { get; set; }
        public long IdAccount { get; set; }
        public long IdCourse { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public long Cc { get; set; }
        public long Telephone { get; set; }
        public long IdNationality { get; set; }
        public int Credits { get; set; }
        public long StudentNum { get; set; }

        public Account IdAccountNavigation { get; set; }
        public Course IdCourseNavigation { get; set; }
        public Nationality IdNationalityNavigation { get; set; }
        public ICollection<Application> Application { get; set; }
    }
}
