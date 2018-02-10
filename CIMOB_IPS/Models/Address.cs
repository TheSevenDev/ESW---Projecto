using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa uma morada associada a um utilizador da aplicação.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Representa a chave primária da morada.
        /// </summary>
        /// <value>Chave primária da morada</value>
        public long IdAddress { get; set; }

        /// <summary>
        /// Representa o código postal associado a uma morada.
        /// </summary>
        /// <value>Código postal associado a uma morada.</value>
        [Display(Name = "Código Postal:")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Representa a descrição da morada completa:Ex:Avenida dos cravos...."
        /// </summary>
        /// <value>Representa a descrição da morada completa</value>
        /// <remarks></remarks>
        [Display(Name = "Morada:")]
        [Required(ErrorMessage = "Insira a sua morada.")]
        public string AddressDesc { get; set; }

        /// <summary>
        /// Representa o número da porta da morada.
        /// </summary>
        /// <value>Número da porta da morada.</value>
        [Display(Name = "N.º da Porta:")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "O nº da porta tem de ser expresso em algarismos.")]
        public short DoorNumber { get; set; }

        /// <summary>
        /// Representa o andar associado à morada.
        /// </summary>
        /// <value>Andar associado à morada.</value>
        [Display(Name = "Andar:")]
        public string Floor { get; set; }

        public ICollection<Student> Student { get; set; }
    }
}
