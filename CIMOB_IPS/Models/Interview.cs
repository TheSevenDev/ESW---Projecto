using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    public class Interview
    {
        public long IdInterview { get; set; }

        public DateTime Date { get; set; }

        public long IdState { get; set; }

        public State IdStateNavigagion { get; set; }

        public ICollection<Application> Application { get; set; }
    }
}
