using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    public class Signature
    {
        [UIHint("SignaturePad")]
        public byte[] MySignature { get; set; }
    }
}
