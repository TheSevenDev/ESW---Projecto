using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa um cancelamento da candidatura.
    /// Inclui o motivo do cancelamento.
    /// </summary>
    /// <remarks></remarks>
    public class ApplicationCancelation
    {
        /// <summary>
        /// Chave primária da classe.
        /// </summary>
        /// <value>Chave primária da classe.</value>
        public long IdApplicationCancelation { get; set; }

        /// <summary>
        /// Chave estrangeira da <see cref="CIMOB_IPS.Models.Application" />.
        /// </summary>
        /// <value>Chave estrangeira da candidatura.</value>
        public long IdApplication { get; set; }

        /// <summary>
        /// Texto com o motivo para o cancelamento.
        /// </summary>
        /// <value>Texto com o motivo para o cancelamento.</value>
        public string Reason { get; set; }

        public Application IdApplicationNavigation { get; set; }
    }
}
