using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe usada para representar uma entrevista a um candidato a um programa de mobilidade.
    /// </summary>
    public class Interview
    {
        /// <summary>
        /// Chave primária da entrevista.
        /// </summary>
        /// <value>Chave primária da entrevista.</value>
        public long IdInterview { get; set; }

        /// <summary>
        /// Data da realização da entrevista.
        /// </summary>
        /// <value></value>
        /// <remarks>Data da realização da entrevista.</remarks>
        public DateTime Date { get; set; }

        /// <summary>
        /// Chave estrangeira do <see cref="CIMOB_IPS.Models.State" /> da entrevista.
        /// </summary>
        /// <value>Chave estrangeira do estado da entrevista.</value>
        public long IdState { get; set; }

        public State IdStateNavigation { get; set; }

        public ICollection<Application> Application { get; set; }

        /// <summary>
        /// Verifica se a entrevista já foi realizada, isto é, a data de hoje é posterior à data da entrevista.
        /// </summary>
        /// <returns>Valor lógico resultante</returns>
        /// <remarks></remarks>
        public bool IsInterviewDone()
        {
            return Date < DateTime.Now;
        }
    }
}
