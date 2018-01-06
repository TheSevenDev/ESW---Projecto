using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models.ViewModels
{
    public class ChooseInstitutionViewModel
    {
        public long IdInstitution { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
