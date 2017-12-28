using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    public partial class ProgramType
    {
        public ProgramType()
        {
            Program = new HashSet<Program>();
        }

        public long IdProgramType { get; set; }

        [Display(Name = "Programa")]
        public string Name { get; set; }

        [Display(Name = "Descrição")]
        public string Description { get; set; }

        public ICollection<Program> Program { get; set; }
    }
}
