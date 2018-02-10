using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa um técnico do CIMOB, registado na aplicação.
    /// </summary>
    public partial class Technician
    {
        public Technician()
        {
            Mobility = new HashSet<Mobility>();
        }

        /// <summary>
        /// Chave primária do técnico do CIMOB.
        /// </summary>
        /// <value>Chave primária do técnico do CIMOB.</value>
        public long IdTechnician { get; set; }

        /// <summary>
        /// Chave estrangeira da <see cref="CIMOB_IPS.Models.Account" /> associada ao técnico.
        /// </summary>
        /// <value>Chave estrangeira da conta associada ao técnico.</value>
        public long IdAccount { get; set; }


        /// <summary>
        /// Nome do técnico do CIMOB.
        /// Deverá conte, no máximo, 60 caracteres.
        /// </summary>
        /// <value>Nome do técnico do CIMOB.</value>
        [Required(ErrorMessage = "O Nome não está preenchido.")]
        [StringLength(60, ErrorMessage = "O Nome deve conter no máximo 60 caracteres.")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        /// <summary>
        /// Contacto do técnico do CIMOB.
        /// Tem de ser expresso em 9 algarismos.
        /// </summary>
        /// <value>Contacto do técnico do CIMOB.</value>
        [Required(ErrorMessage = "Nº Telemóvel não está preenchido")]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "O Nº Telemovel tem de ser expresso em 9 algarismos.")]
        [Display(Name = "Nº Telemóvel")]
        public long Telephone { get; set; }

        /// <summary>
        /// Valor lógico usado para aferir se um técnico é administrador ou não.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> técnico autenticado como administrador ; técnico autenticado como não administrador, <see langword="false" />.</value>
        /// <remarks></remarks>
        [Display(Name = "Administrador")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Activo")]
        public bool Active { get; set; }

        public Account IdAccountNavigation { get; set; }
        public ICollection<Mobility> Mobility { get; set; }
    }
}
