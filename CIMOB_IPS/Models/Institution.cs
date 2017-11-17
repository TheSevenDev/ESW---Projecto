using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Institution
    {
        public Institution()
        {
            Course = new HashSet<Course>();
            InstitutionProgram = new HashSet<InstitutionProgram>();
            Mobility = new HashSet<Mobility>();
        }

        public long IdInstitution { get; set; }
        public string Name { get; set; }
        public long IdNationality { get; set; }

        public Nationality IdNationalityNavigation { get; set; }
        public ICollection<Course> Course { get; set; }
        public ICollection<InstitutionProgram> InstitutionProgram { get; set; }
        public ICollection<Mobility> Mobility { get; set; }
    }
}
