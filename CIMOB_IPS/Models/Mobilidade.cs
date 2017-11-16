using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Mobilidade
    {
        public long IdMobilidade { get; set; }
        public long IdCandidatura { get; set; }
        public long IdPrograma { get; set; }
        public long IdEstado { get; set; }
        public long IdTecnicoResponsavel { get; set; }
        public long IdInstituicaoOutgoing { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataTermino { get; set; }

        public Candidatura IdCandidaturaNavigation { get; set; }
        public Instituicao IdInstituicaoOutgoingNavigation { get; set; }
        public Programa IdProgramaNavigation { get; set; }
        public Tecnico IdTecnicoResponsavelNavigation { get; set; }
    }
}
