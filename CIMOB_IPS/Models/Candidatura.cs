using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Candidatura
    {
        public Candidatura()
        {
            Mobilidade = new HashSet<Mobilidade>();
        }

        public long IdCandidatura { get; set; }
        public long IdEstudante { get; set; }
        public long IdEstado { get; set; }
        public bool Bolsa { get; set; }
        public short AvaliacaoFinal { get; set; }
        public string CartaMotivacao { get; set; }
        public string ContactoEmergenciaNome { get; set; }
        public string ContactoEmergenciaRelacao { get; set; }
        public long ContactoEmergenciaTelefone { get; set; }

        public Estado IdEstadoNavigation { get; set; }
        public Estudante IdEstudanteNavigation { get; set; }
        public ICollection<Mobilidade> Mobilidade { get; set; }
    }
}
