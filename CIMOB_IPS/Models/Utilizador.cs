using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Utilizador
    {
        public Utilizador()
        {
            Estudante = new HashSet<Estudante>();
            Notificacao = new HashSet<Notificacao>();
            Tecnico = new HashSet<Tecnico>();
        }

        public long IdUtilizador { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public bool Activo { get; set; }

        public ICollection<Estudante> Estudante { get; set; }
        public ICollection<Notificacao> Notificacao { get; set; }
        public ICollection<Tecnico> Tecnico { get; set; }
    }
}
