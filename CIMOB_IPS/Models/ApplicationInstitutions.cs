using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe usada para representar a ligação entre uma candidatura e uma instituição escolhida pelo candidato, contendo a ordem de preferência da mesma.
    /// </summary>
    /// <remarks></remarks>
    public partial class ApplicationInstitutions
    {
        /// <summary>
        /// Chave estrangeira da <see cref="CIMOB_IPS.Models.Application" />.
        /// </summary>
        /// <value>Chave estrangeira da candidatura.</value>
        public long IdApplication { get; set; }

        /// <summary>
        /// Chave estrangeira da <see cref="CIMOB_IPS.Models.Institution" />.
        /// </summary>
        /// <value>Chave estrangeira da instituição.</value>
        public long IdInstitution { get; set; }


        /// <summary>
        /// Ordem de preferência da instituição.
        /// A preferência varia de forma crescente entre 1 e 3. 
        /// </summary>
        /// <value>Ordem de preferência da instituição.</value>
        public short InstitutionOrder { get; set; }

        public Application IdApplicationNavigation { get; set; }
        public Institution IdInstitutionNavigation { get; set; }
    }
}
