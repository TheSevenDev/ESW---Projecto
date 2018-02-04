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

        public double FinalEvalution { get; set; }

        public long IdTechnician { get; set; }

        public long IdInstitution { get; set; }

        public IEnumerable<SelectListItem> Technicians { get; set; }

        public IEnumerable<SelectListItem> OutgoingInstitutions { get; set; }
    }
}
