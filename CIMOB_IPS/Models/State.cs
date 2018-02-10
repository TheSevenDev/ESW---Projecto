using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa o estado de uma candidatura ou programa de mobilidade.
    /// </summary>
    public partial class State
    {
        public State()
        {
            Application = new HashSet<Application>();
            Program = new HashSet<Program>();
        }

        /// <summary>
        /// Chave primária do estado.
        /// </summary>
        /// <value>Chave primária do estado.</value>
        /// <remarks></remarks>
        public long IdState { get; set; }

        /// <summary>
        /// Descrição do estado.
        /// </summary>
        /// <value>Descrição do estado.</value>
        [Display(Name = "Estado")]
        public string Description { get; set; }

        public ICollection<Application> Application { get; set; }
        public ICollection<Program> Program { get; set; }
        public ICollection<Interview> Interview { get; set; }
        public ICollection<Mobility> Mobility { get; set; }
    }
}
