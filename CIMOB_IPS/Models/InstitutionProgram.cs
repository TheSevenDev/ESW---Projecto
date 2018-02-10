using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa a relação entre uma instuição e um programa de mobilidade.
    /// </summary>
    public partial class InstitutionProgram
    {
        /// <summary>
        /// Chave estrangeira do <see cref="CIMOB_IPS.Models.Program" />
        /// </summary>
        /// <value>Chave estrangeira do programa.</value>
        public long IdProgram { get; set; }

        /// <summary>
        /// Chave estrangeira da <see cref="CIMOB_IPS.Models.Institution" />
        /// </summary>
        /// <value>Chave estrangeira da instituição.</value>
        public long IdOutgoingInstitution { get; set; }

        public Institution IdOutgoingInstitutionNavigation { get; set; }
        public Program IdProgramNavigation { get; set; }
    }
}
