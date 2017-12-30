using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    public partial class TestFile
    {
        public long IdFile { get; set; }

        [Display(Name = "Ficheiro")]
        public byte[] FileTest { get; set; }
    }
}
