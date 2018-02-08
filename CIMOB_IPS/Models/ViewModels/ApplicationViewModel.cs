using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models.ViewModels
{
    public class ApplicationViewModel
    {
        public Application Application {get; set;}
        public Account Account { get; set; }
        public Program Program { get; set; }
        public long IdProgram { get; set; }

        public string PostalCode1 { get; set; }
        public string PostalCode2 { get; set; }

        public IEnumerable<SelectListItem> Nationalities { get; set; }

        public IEnumerable<SelectListItem> Courses { get; set; }

        public IEnumerable<InstitutionProgram> Institutions { get; set; }

        [UIHint("SignaturePad")]
        [Required(ErrorMessage = "É necessário a sua assinatura digital.")]
        public byte[] Signature { get; set; }
    }
}
