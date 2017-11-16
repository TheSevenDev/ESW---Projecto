using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Notificacao
    {
        public long IdNotificacao { get; set; }
        public long IdUtilizador { get; set; }
        public string Descricao { get; set; }

        public Utilizador IdUtilizadorNavigation { get; set; }
    }
}
