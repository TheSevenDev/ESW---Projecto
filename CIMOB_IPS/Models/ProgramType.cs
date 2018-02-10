using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa um tipo de programa de mobilidade. Ex: Erasmus+, Santander, Ibero-Americana, etc...
    /// </summary>
    /// <remarks></remarks>
    public partial class ProgramType
    {
        public ProgramType()
        {
            Program = new HashSet<Program>();
        }

        /// <summary>
        /// Chave primária do tipo de programa.
        /// </summary>
        /// <value>Chave primária do tipo de programa.</value>
        public long IdProgramType { get; set; }

        /// <summary>
        /// Nome do tipo de programa.
        /// Ex: Erasmus+, Santander, Ibero-Americana
        /// </summary>
        /// <value>Nome do tipo de programa.</value>
        [Display(Name = "Programa")]
        public string Name { get; set; }

        /// <summary>
        /// Descrição do tipo de programa.
        /// </summary>
        /// <value>Descrição do tipo de programa.</value>
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        /// <summary>
        /// Caminho para a imagem usada ao dispor os programas.
        /// </summary>
        /// <value>Caminho para a imagem usada ao dispor os programas.</value>
        /// <remarks></remarks>
        public string ImageFile { get; set; }

        public ICollection<Program> Program { get; set; }
    }
}
