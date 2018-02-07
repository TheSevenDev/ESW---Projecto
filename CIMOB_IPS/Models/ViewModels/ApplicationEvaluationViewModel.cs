using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CIMOB_IPS.Models.ViewModels
{
    public class ApplicationEvaluationViewModel
    {
        public long IdApplication { get; set; }

        public long IdTechnician { get; set; }

        public long IdInstitution { get; set; }


        public int TotalCourseCredits { get; set; }
        public int MotivationCardPoints { get; set; }
        public int InterviewPoints { get; set; }
        public int AverageGrade { get; set; }
        public double FinalEvalution { get; set; }

        public IEnumerable<SelectListItem> Technicians { get; set; }

        public IEnumerable<SelectListItem> OutgoingInstitutions { get; set; }
    }
}
