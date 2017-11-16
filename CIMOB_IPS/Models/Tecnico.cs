using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Tecnico
    {
        public Tecnico()
        {
            Mobilidade = new HashSet<Mobilidade>();
        }

        public long IdTecnico { get; set; }
        public long IdUtilizador { get; set; }
        public string Nome { get; set; }
        public long Telefone { get; set; }

        public Utilizador IdUtilizadorNavigation { get; set; }
        public ICollection<Mobilidade> Mobilidade { get; set; }
    }
}
