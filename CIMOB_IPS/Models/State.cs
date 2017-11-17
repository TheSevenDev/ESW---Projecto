using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class State
    {
        public State()
        {
            Application = new HashSet<Application>();
            Program = new HashSet<Program>();
        }

        public long IdState { get; set; }
        public string Description { get; set; }

        public ICollection<Application> Application { get; set; }
        public ICollection<Program> Program { get; set; }
    }
}
