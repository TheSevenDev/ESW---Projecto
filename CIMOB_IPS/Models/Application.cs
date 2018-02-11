using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa uma candidatura de um estudante a um programa de mobilidade.
    /// </summary>
    public partial class Application
    {
        /// <summary>
        /// Inicializa uma nova <see cref="CIMOB_IPS.Models.Application" /> com uma lista vazia de instituições e o valor lógico da requisitação da bolsa, por defeito, a falso. 
        /// </summary>
        public Application()
        {
            ApplicationInstitutions = new HashSet<ApplicationInstitutions>();
            Mobility = new HashSet<Mobility>();
            HasScholarship = false;
        }


        /// <summary>
        /// Chave primária da candidatura.
        /// </summary>
        /// <value>Chave primária da candidatura.</value>
        public long IdApplication { get; set; }

        /// <summary>
        /// Chave estrangeira do <see cref="CIMOB_IPS.Models.Student" /> que efetuou a candidatura.
        /// </summary>
        /// <value>Chave estrangeira do estudante que efetuou a candidatura.</value>
        public long IdStudent { get; set; }

        /// <summary>
        /// Chave estrangeira do <see cref="CIMOB_IPS.Models.State" /> da candidatura.
        /// </summary>
        /// <value>Chave estrangeira do estado da candidatura.</value>
        public long IdState { get; set; }

        /// <summary>
        /// Chave estrangeira do <see cref="CIMOB_IPS.Models.Program" /> associado à candidatura.
        /// </summary>
        /// <value>Chave estrangeira do programa associado à candidatura.</value>
        public long IdProgram { get; set; }


        /// <summary>
        /// Chave estrangeira da <see cref="CIMOB_IPS.Models.Interview" /> associada à candidatura.
        /// </summary>
        /// <value>Chave estrangeira do entrevista associada à candidatura.</value>
        public long IdInterview { get; set; }

        /// <summary>
        /// Valor lógico que permite aferir se o estudante é candidato, ou não, à bolsa.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> estudante candidato à bolsa ; estudante não candidato à bolsa, <see langword="false" />.</value>
        [Display(Name = "Candidato a bolsa")]
        public bool HasScholarship { get; set; }



        /// <summary>
        /// Avaliação final da candidatura.
        /// </summary>
        /// <value>Avaliação final da candidatura.</value>
        [Display(Name = "Avaliação Final")]
        public short FinalEvaluation { get; set; }

        /// <summary>
        /// Carta de motivações do estudante para efetuar uma candidatura ao programa de mobilidade.
        /// Tem de ter no mínimo 40 caracteres e no máximo 3000.
        /// </summary>
        /// <value>Carta de motivações do estudante.</value>
        [Required(ErrorMessage = "Preencha a carta de motivação.")]
        [MinLength(40, ErrorMessage = "Insira, no mínimo, 40 caracteres.")]
        [MaxLength(3000, ErrorMessage = "Insira, no máximo, 3000 caracteres.")]
        [Display(Name = "Carta de Motivação")]
        [DataType(DataType.MultilineText)]
        public string MotivationCard { get; set; }


        /// <summary>
        /// Nome do contacto a estabelecer em caso de emergência.
        /// </summary>
        /// <value> Nome do contacto de emergência</value>
        [Display(Name = "Nome Completo")]
        [Required(ErrorMessage = "O nome não está preenchido.")]
        public string EmergencyContactName { get; set; }

        /// <summary>
        /// Relação do candidato com o contacto de emergência.
        /// </summary>
        /// <value>Relação do candidato com o contacto de emergência.</value>
        [Display(Name = "Relação com o candidato")]
        [Required(ErrorMessage = "A relação não está preenchida.")]
        public string EmergencyContactRelation { get; set; }

        /// <summary>
        /// Número de telefone do contacto a estabelecer em caso de emergência.
        /// </summary>
        /// <value>Número de telefone do contacto de emergência</value>
        [Display(Name = "Nº Telemóvel")]
        [Required(ErrorMessage = "O nº telemóvel não está preenchido.")]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "O Nº Telemovel tem de ser expresso em 9 algarismos.")]
        public long EmergencyContactTelephone { get; set; }

        /// <summary>
        /// Estado da candidatura.
        /// </summary>
        /// <value>Estado da candidatura.</value>
        [Display(Name = "Estado")]
        public State IdStateNavigation { get; set; }

        /// <summary>
        /// Data em que foi submetida a candidatura.
        /// Formato: dd/MM/yyyy
        /// </summary>
        /// <value>Data em que foi submetida a candidatura.</value>
        [Display(Name = "Submetida a")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ApplicationDate { get; set; }

        /// <summary>
        /// Estudante candidato.
        /// </summary>
        /// <value>Estudante candidato.</value>
        public Student IdStudentNavigation { get; set; }

        /// <summary>
        /// Programa de mobilidade ao qual a candidatura diz respeito.
        /// </summary>
        /// <value>Programa de mobilidade</value>
        [Display(Name = "Programa")]
        public Program IdProgramNavigation { get; set; }

        /// <summary>
        /// Entrevista associada à candidatura.
        /// </summary>
        /// <value>Entrevista associada à candidatura.</value>
        [Display(Name = "Entrevista")]
        public Interview IdInterviewNavigation { get; set; }

        /// <summary>
        /// Ficheiro comprovativo da candidatura, gerado aquando submissão do formulário da candidatura.
        /// </summary>
        /// <value>Ficheiro comprovativo da candidatura</value>
        [Display(Name = "Comprovativo de candidatura")]
        public byte[] SignedAppFile { get; set; }

        [Display(Name = "Instituições")]
        public ICollection<ApplicationInstitutions> ApplicationInstitutions { get; set; }
        public ICollection<Mobility> Mobility { get; set; }
        public ICollection<ApplicationCancelation> ApplicationCancelation { get; set; }
        public ICollection<ApplicationEvaluation> ApplicationEvaluation { get; set; }


        public bool IsAvailable()
        {
            return IdStateNavigation.Description.Equals("Aberto");
        }
    }
}
