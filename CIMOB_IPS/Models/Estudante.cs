using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Estudante
    {
        public Estudante()
        {
            Candidatura = new HashSet<Candidatura>();
        }

        public long IdEstudante { get; set; }
        public long IdUtilizador { get; set; }
        public long IdCurso { get; set; }
        public string Nome { get; set; }
        public string Morada { get; set; }
        public long Cc { get; set; }
        public long Telefone { get; set; }
        public long IdNacionalidade { get; set; }
        public int Ects { get; set; }
        public long NumAluno { get; set; }

        public Curso IdCursoNavigation { get; set; }
        public Nacionalidade IdNacionalidadeNavigation { get; set; }
        public Utilizador IdUtilizadorNavigation { get; set; }
        public ICollection<Candidatura> Candidatura { get; set; }
    }
}
