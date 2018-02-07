using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    public class ApplicationEvaluation
    {
        public long IdApplicationEvaluation { get; set; }
        public long IdApplication { get; set; }

        public float CreditsRatio { get; set; }
        public int MotivationCardPoints { get; set; }
        public int InterviewPoints { get; set; }
        public float AverageGrade { get; set; }

        public Application IdApplicationNavigation { get; set; }
    }
}
