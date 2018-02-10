using CIMOB_IPS.Models.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa um estudante registado na aplicação.
    /// </summary>
    /// <remarks></remarks>
    public partial class Student
    {
        public Student()
        {
            Application = new HashSet<Application>();
        }

        /// <summary>
        /// Chave primária do estudante registado.
        /// </summary>
        /// <value>Chave primária do estudante registado.</value>
        public long IdStudent { get; set; }

        /// <summary>
        /// Chave estrangeira da <see cref="CIMOB_IPS.Models.Account" /> associada ao estudante.
        /// </summary>
        /// <value>Chave estrangeira da conta associada ao estudante.</value>
        public long IdAccount { get; set; }

        /// <summary>
        /// Número do IPS do estudante.
        /// Tem de ser expresso em 9 algarismos.
        /// </summary>
        /// <value>Número do IPS do estudante.</value>
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "O Número de Estudante não está preenchido.")]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "O Nº Estudante tem de ser expresso em 9 algarismos.")]
        [Display(Name = "Número de Estudante")]
        public long StudentNum { get; set; }

        /// <summary>
        /// Chave estrangeira do <see cref="CIMOB_IPS.Models.Course" /> frequentado pelo estudante.
        /// </summary>
        /// <value>Chave estrangeira da curso frequentado pelo estudante.</value>
        [Required(ErrorMessage = "O Curso não está preenchido.")]
        [Display(Name = "Curso:")]
        public long IdCourse { get; set; }

        /// <summary>
        /// Nome do estudante.
        /// Deverá conter, no máximo, 60 caracteres.
        /// </summary>
        /// <value>Nome do estudante.</value>
        [Required(ErrorMessage = "O Nome não está preenchido.")]
        [StringLength(60, ErrorMessage = "O Nome deve conter no máximo 60 caracteres.")]
        [Display(Name = "Nome Completo:")]
        public string Name { get; set; }

        /// <summary>
        /// Número do cartão de cidadão do estudante.
        /// </summary>
        /// <value>Número do cartão de cidadão do estudante.</value>
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "O Cartão de Cidadão não está preenchido.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "O Cartão de Cidadão tem de ser expresso em algarismos.")]
        [Display(Name = "Cartão de Cidadão:")]
        public long Cc { get; set; }


        /// <summary>
        /// Contacto de telefone do estudante.
        /// Tem de ser expresso em 9 algarismos.
        /// </summary>
        /// <value>Contacto de telefone do estudante.</value>
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "O Nº Telemóvel não está preenchido.")]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "O Nº Telemovel tem de ser expresso em 9 algarismos.")]
        [Display(Name = "Nº Telemóvel:")]
        public long Telephone { get; set; }

        /// <summary>
        /// Chave estrangeira da <see cref="CIMOB_IPS.Models.Nationality" /> do estudante.
        /// </summary>
        /// <value>Chave estrangeira da nacionalidade do estudante.</value>
        [Required(ErrorMessage = "Seleccione uma nacionalidade.")]
        [Display(Name = "Nacionalidade:")]
        public long IdNationality { get; set; }

        /// <summary>
        /// Créditos(ECTS) obtidos até ao momento pelo estudante.
        /// </summary>
        /// <value>Créditos(ECTS) obtidos até ao momento pelo estudante.</value>
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "OS ECTS não estão preenchidos.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Os ECTS tem de ser expresso em algarismos.")]
        [Display(Name = "ECTS:")]
        public int Credits { get; set; }

        /// <summary>
        /// Género do estudante.
        /// </summary>
        /// <value>Género do estudante.</value>
        [Display(Name = " Género:")]
        public string Gender { get; set; }

        /// <summary>
        /// Data de nascimento do estudante.
        /// </summary>
        /// <value>Data de nascimento do estudante.</value>
        [CheckIfDateIsBefore]
        [Required(ErrorMessage = "Insira a sua data de nascimento.")]
        [Display(Name = "Data de Nascimento:")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public Address IdAddressNavigation { get; set; }

        /// <summary>
        /// Chave estrangeira da <see cref="CIMOB_IPS.Models.Address" /> do estudante.
        /// </summary>
        /// <value>Chave estrangeira da morada do estudante.</value>
        [Required(ErrorMessage = "A morada não está preenchida.")]
        [Display(Name = "Morada:")]
        public long IdAddress { get; set; }

        public Account IdAccountNavigation { get; set; }


        [Display(Name = "Curso:")]
        public Course IdCourseNavigation { get; set; }

        [Display(Name = "Nacionalidade:")]
        public Nationality IdNationalityNavigation { get; set; }

        public ICollection<Application> Application { get; set; }

        public bool HasEnoughCredits()
        {
            return Credits > 60;
        }

        public bool HasNotMaximumApplications()
        {
            return Application.Count < 3;
        }

    }
}
