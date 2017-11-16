using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Curso
    {
        public Curso()
        {
            Estudante = new HashSet<Estudante>();
        }

        public long IdCurso { get; set; }
        public long IdInstituicao { get; set; }
        public string Nome { get; set; }

        public Instituicao IdInstituicaoNavigation { get; set; }
        public ICollection<Estudante> Estudante { get; set; }
    }
}
