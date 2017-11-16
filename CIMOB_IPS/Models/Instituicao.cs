using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Instituicao
    {
        public Instituicao()
        {
            Curso = new HashSet<Curso>();
            InstituicoesPrograma = new HashSet<InstituicoesPrograma>();
            Mobilidade = new HashSet<Mobilidade>();
        }

        public long IdInstituicao { get; set; }
        public string Nome { get; set; }
        public long IdNacionalidade { get; set; }

        public Nacionalidade IdNacionalidadeNavigation { get; set; }
        public ICollection<Curso> Curso { get; set; }
        public ICollection<InstituicoesPrograma> InstituicoesPrograma { get; set; }
        public ICollection<Mobilidade> Mobilidade { get; set; }
    }
}
