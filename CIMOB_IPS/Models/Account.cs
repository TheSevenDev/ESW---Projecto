using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa uma conta de um utilizador da aplicação.
    /// </summary>
    public partial class Account
    {
        /// <summary>
        /// Inicializa uma nova instância de uma <see cref="CIMOB_IPS.Models.Account" />. 
        /// </summary>
        public Account()
        {
            Notification = new HashSet<Notification>();
            Student = new HashSet<Student>();
            Technician = new HashSet<Technician>();
        }

        /// <summary>
        /// Representa a chave primária da conta.
        /// </summary>
        /// <value>Chave primária da conta</value>
        public long IdAccount { get; set; }

        /// <summary>
        /// Representa o email associado à conta de utilizador.
        /// Deverá conter a seguinte estrutura: exemplo@dominio.com
        /// </summary>
        /// <value>Representa o email associado à conta de utilizador.</value>
        [Required(ErrorMessage = "O email não preenchido")]
        [EmailAddress(ErrorMessage = "O email deverá conter a seguinte estrutura: exemplo@dominio.com")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        /// <summary>
        /// Representa a palavra-passe usada na autenticação.
        /// </summary>
        /// <value>Palavra-passe usada na autenticação</value>
        [Required(ErrorMessage = "A password não está preenchida")]
        [Display(Name = "Password:")]
        [DataType(DataType.Password)]
        public byte[] Password { get; set; }

        /// <summary>
        /// Representa a confirmação da palavra-passe usada na autenticação.
        /// Esta propriedade não está mapeada para a base de dados e é exclusivamente usada durante a criação da conta, para a confirmação da palavra-passe.
        /// </summary>
        /// <value>Confirmação da palavra-passe usada na autenticação</value>
        [NotMapped]
        [Required(ErrorMessage = "A confirmação da password não está preenchida")]
        [Compare("Password", ErrorMessage = "As passwords não coincidem.")]
        [Display(Name = "Confirme a Password:")]
        [DataType(DataType.Password)]
        public byte[] ConfirmPassword { get; set; }

        /// <summary>
        /// Representa o caminho para a imagem usada como avatar do utilizador.
        /// </summary>
        /// <value>Caminho para a imagem usada como avatar do utilizador.></value>
        public string AvatarUrl { get; set; }


        public ICollection<Notification> Notification { get; set; }
        public ICollection<Student> Student { get; set; }
        public ICollection<Technician> Technician { get; set; }
    }
}
