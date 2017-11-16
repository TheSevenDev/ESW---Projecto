using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Estado
    {
        public Estado()
        {
            Candidatura = new HashSet<Candidatura>();
            Programa = new HashSet<Programa>();
        }

        public long IdEstado { get; set; }
        public string Descricao { get; set; }

        public ICollection<Candidatura> Candidatura { get; set; }
        public ICollection<Programa> Programa { get; set; }
    }
}
