using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Programa
    {
        public Programa()
        {
            InstituicoesPrograma = new HashSet<InstituicoesPrograma>();
            Mobilidade = new HashSet<Mobilidade>();
        }

        public long IdPrograma { get; set; }
        public long IdEstado { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAbertura { get; set; }
        public DateTime? DataEncerramento { get; set; }

        public Estado IdEstadoNavigation { get; set; }
        public ICollection<InstituicoesPrograma> InstituicoesPrograma { get; set; }
        public ICollection<Mobilidade> Mobilidade { get; set; }
    }
}
