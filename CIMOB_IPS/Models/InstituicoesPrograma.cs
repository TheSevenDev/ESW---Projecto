using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class InstituicoesPrograma
    {
        public long IdPrograma { get; set; }
        public long IdInstituicaoOutgoing { get; set; }

        public Instituicao IdInstituicaoOutgoingNavigation { get; set; }
        public Programa IdProgramaNavigation { get; set; }
    }
}
