using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa um programa de mobilidade.
    /// </summary>
    public partial class Program
    {
        public Program()
        {
            Application = new HashSet<Application>();
            InstitutionProgram = new HashSet<InstitutionProgram>();
        }

        /// <summary>
        /// Chave primária do programa.
        /// </summary>
        /// <value>Chave primária do programa.</value>
        public long IdProgram { get; set; }

        /// <summary>
        /// Chave estrangeira do <see cref="CIMOB_IPS.Models.State" /> do programa.
        /// </summary>
        /// <value>Chave estrangeira do estado do programa.</value>
        public long IdState { get; set; }

        /// <summary>
        /// Data de criação do programa.
        /// Formato: dd/MM/yyy
        /// </summary>
        /// <value>Data de criação do programa.</value>
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Data de abertura do programa.
        /// Formato: dd/MM/yyy
        /// </summary>
        /// <value>Data de abertura do programa.</value>
        [Required(ErrorMessage = "É necessário definir a data de abertura!")]
        [Display(Name = "Data de Abertura")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? OpenDate { get; set; }

        /// <summary>
        /// Data de encerramento do programa.
        /// Formato: dd/MM/yyy
        /// </summary>
        /// <value>Data de encerramento do programa.</value>
        [Required(ErrorMessage = "É necessário definir a data de encerramento!")]
        [Display(Name = "Data de Fecho")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? ClosingDate { get; set; }

        /// <summary>
        /// Data prevista para o ínicio da mobilidade.
        /// Formato: dd/MM/yyy
        /// </summary>
        /// <value>Data prevista para o ínicio da mobilidade.</value>
        [Required(ErrorMessage = "É necessário definir a data de início de mobilidade!")]
        [Display(Name = "Data de início da Mobilidade")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime MobilityBeginDate { get; set; }

        /// <summary>
        /// Data prevista para o fim da mobilidade.
        /// Formato: dd/MM/yyy
        /// </summary>
        /// <value>Data prevista para o fim da mobilidade.</value>
        [Required(ErrorMessage = "É necessário definir a data de fim de mobilidade!")]
        [Display(Name = "Data de fim da Mobilidade")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime MobilityEndDate { get; set; }

        /// <summary>
        /// Número de vagas disponíveis do programa.
        /// </summary>
        /// <value>Número de vagas do programa.</value>
        [Range(0, int.MaxValue, ErrorMessage = "O número de vagas tem de ser um número positivo maior que 0.")]
        [Required(ErrorMessage = "É necessário definir o número de vagas!")]
        [Display(Name = "Vagas para candidaturas")]
        public int Vacancies { get; set; }

        /// <summary>
        /// Chave estrangeira do <see cref="CIMOB_IPS.Models.ProgramType" /> do tipo de programa.
        /// </summary>
        /// <value>Chave estrangeira do tipo de programa.</value>
        public long IdProgramType { get; set; }

        [Display(Name = "Programa")]
        public ProgramType IdProgramTypeNavigation { get; set; }

        [Display(Name = "Estado")]
        public State IdStateNavigation { get; set; }

        public ICollection<Application> Application { get; set; }

        [Display(Name = "Instituições disponíveis para mobilidade")]
        public ICollection<InstitutionProgram> InstitutionProgram { get; set; }

        /// <summary>
        /// Verifica se o programa está aberto.
        /// </summary>
        /// <returns>Valor lógico resultante</returns>
        public bool IsOpenProgram()
        {
            return IdStateNavigation.Description.Equals("Aberto");
        }

        /// <summary>
        /// Verifica se o programa tem vagas.
        /// </summary>
        /// <returns>Valor lógico resultante</returns>
        public bool WithVacanciesAvailable()
        {
            return Vacancies > 0;
        }

        /// <summary>
        /// Verifica se a data atual está entre a abertura e fecho das candidaturas.
        /// </summary>
        /// <returns>Valor lógico resultante</returns>
        public bool WithDateAvailable()
        {
            return DateTime.Now.Date > OpenDate && DateTime.Now.Date < ClosingDate;
        }

        /// <summary>
        /// Verifica se o programa está disponível para efetuar candidaturas, combinando os predicados IsOpenProgram(), WithVacanciesAvailable(), WithDateAvailable().
        /// </summary>
        /// <returns>Valor lógico resultante</returns>
        public bool WithPossibleApplication()
        {
            return IsOpenProgram() && WithVacanciesAvailable() && WithDateAvailable();
        }
    }
}
