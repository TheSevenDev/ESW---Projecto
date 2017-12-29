using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models.ViewModels
{
    public class ApplicationViewModel
    {
        public Application Application {get; set;}
        public Student Student { get; set; }
        public Account Account { get; set; }

        public IEnumerable<SelectListItem> Nationalities { get; set; }

        public IEnumerable<SelectListItem> Courses { get; set; }

        public IEnumerable<InstitutionProgram> Institutions { get; set; }
    }
}
