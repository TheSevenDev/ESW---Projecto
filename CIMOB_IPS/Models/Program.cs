using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Program
    {
        public Program()
        {
            Application = new HashSet<Application>();
            InstitutionProgram = new HashSet<InstitutionProgram>();
        }

        public long IdProgram { get; set; }
        public long IdState { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? ClosingDate { get; set; }

        public State IdStateNavigation { get; set; }
        public ICollection<Application> Application { get; set; }
        public ICollection<InstitutionProgram> InstitutionProgram { get; set; }
    }
}
