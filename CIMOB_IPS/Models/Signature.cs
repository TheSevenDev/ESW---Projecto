using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa o desenho no canvas da assinatura, usado no formulária de submussão de uma candidatura.
    /// </summary>
    /// <remarks></remarks>
    public class Signature
    {
        /// <summary>
        /// Bytes representativos dos traços no desenho da assinatura.
        /// </summary>
        /// <value>Bytes representativos dos traços no desenho da assinatura.</value>
        [UIHint("SignaturePad")]
        public byte[] MySignature { get; set; }
    }
}
