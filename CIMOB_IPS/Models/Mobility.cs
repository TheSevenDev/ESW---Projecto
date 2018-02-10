using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe usada para representar uma mobilidade em curso.
    /// Contém a candidatura a instituição escolhida e um técnico do CIMOB responsável.
    /// </summary>
    /// <remarks></remarks>
    public partial class Mobility
    {
        /// <summary>
        /// Chave primária da mobilidade.
        /// </summary>
        /// <value>Chave primária da mobilidade.</value>
        public long IdMobility { get; set; }

        /// <summary>
        /// Chave estrangeira da <see cref="CIMOB_IPS.Models.Application" />.
        /// </summary>
        /// <value>Chave estrangeira da candidatura associada à mobilidade.</value>
        public long IdApplication { get; set; }

        /// <summary>
        /// Chave estrangeira do <see cref="CIMOB_IPS.Models.State" /> da mobilidade.
        /// </summary>
        /// <value>Chave estrangeira do estado da mobilidade.</value>
        public long IdState { get; set; }

        /// <summary>
        /// Chave estrangeira do <see cref="CIMOB_IPS.Models.Technician" /> responsável.
        /// </summary>
        /// <value>Chave estrangeira do técnico responsável pela mobilidade.</value>
        public long IdResponsibleTechnician { get; set; }

        /// <summary>
        /// Chave estrangeira da <see cref="CIMOB_IPS.Models.Institution" />.
        /// </summary>
        /// <value>Chave estrangeira da instituição destino da mobilidade.</value>
        public long IdOutgoingInstitution { get; set; }

        public Application IdApplicationNavigation { get; set; }

        [DisplayName("Instituição de destino")]
        public Institution IdOutgoingInstitutionNavigation { get; set; }

        [DisplayName("Técnico Responsável")]
        public Technician IdResponsibleTechnicianNavigation { get; set; }

        public State IdStateNavigation { get; set; }
    }
}
