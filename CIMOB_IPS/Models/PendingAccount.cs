using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa uma conta pendente, isto é, conta onde tenha sido efetuado o pré-registo mas sem registo efetivo.
    /// </summary>

    public partial class PendingAccount
    {
        /// <summary>
        /// Chave primária da conta pendente.
        /// </summary>
        /// <value>Chave primária da conta pendente.</value>
        public long IdPending { get; set; }


        /// <summary>
        /// Endereço email da conta pendente.
        /// </summary>
        /// <value>Endereço email da conta pendente.</value>
        [EmailAddress(ErrorMessage = "O email deverá conter a seguinte estrutura: exemplo@dominio.com")]
        [Display(Name ="E-mail")]
        public string Email { get; set; }


        /// <summary>
        /// Código único usado no envio do email de confirmação de para efetuar o registo completo na aplicação.
        /// </summary>
        /// <value>Código único usado para identificar a conta pendente.</value>
        public string Guid { get; set; }

        /// <summary>
        /// Valor que, no caso de se tratar de uma conta associada a um técnico do CIMOB, verifica se se trata de um administrador ou não.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> Conta pendente de um técnico do CIMOB administrador ; Conta pendente de um técnico do CIMOB não administrador, <see langword="false" />.</value>
        /// <remarks></remarks>
        [Display(Name = "Administrador")]
        public bool IsAdmin { get; set; }
    }
}
