using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Coordenador
    {
        public long IdCoordenador { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public long? Telefone { get; set; }
    }
}
