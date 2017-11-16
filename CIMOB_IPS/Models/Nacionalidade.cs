using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Nacionalidade
    {
        public Nacionalidade()
        {
            Estudante = new HashSet<Estudante>();
            Instituicao = new HashSet<Instituicao>();
        }

        public long IdNacionalidade { get; set; }
        public string Descricao { get; set; }

        public ICollection<Estudante> Estudante { get; set; }
        public ICollection<Instituicao> Instituicao { get; set; }
    }
}
