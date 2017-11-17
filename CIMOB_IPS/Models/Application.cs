using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Application
    {
        public Application()
        {
            Mobility = new HashSet<Mobility>();
        }

        public long IdApplication { get; set; }
        public long IdStudent { get; set; }
        public long IdState { get; set; }
        public bool HasScholarship { get; set; }
        public short FinalEvaluation { get; set; }
        public string MotivationCard { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactRelation { get; set; }
        public long EmergencyContactTelephone { get; set; }

        public State IdStateNavigation { get; set; }
        public Student IdStudentNavigation { get; set; }
        public ICollection<Mobility> Mobility { get; set; }
    }
}
