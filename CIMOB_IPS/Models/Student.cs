using CIMOB_IPS.Models.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    public partial class Student
    {
        public Student()
        {
            Application = new HashSet<Application>();
        }

        public long IdStudent { get; set; }
        public long IdAccount { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "O Número de Estudante não está preenchido.")]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "O Nº Estudante tem de ser expresso em 9 algarismos.")]
        [Display(Name = "Número de Estudante:")]
        public long StudentNum { get; set; }

        [Required(ErrorMessage = "O Curso não está preenchido.")]
        [Display(Name = "Curso:")]
        public long IdCourse { get; set; }

        [Required(ErrorMessage = "O Nome não está preenchido.")]
        [StringLength(60 ,ErrorMessage = "O Nome deve conter no máximo 60 caracteres.")]
        [Display(Name = "Nome Completo:")]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "O Cartão de Cidadão não está preenchido.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "O Cartão de Cidadão tem de ser expresso em algarismos.")]
        [Display(Name = "Cartão de Cidadão:")]
        public long Cc { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "O Nº Telemóvel não está preenchido.")]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "O Nº Telemovel tem de ser expresso em 9 algarismos.")]
        [Display(Name = "Nº Telemóvel:")]
        public long Telephone { get; set; }

        [Required(ErrorMessage = "Seleccione uma nacionalidade.")]
        [Display(Name = "Nacionalidade:")]
        public long IdNationality { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "OS ECTS não estão preenchidos.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Os ECTS tem de ser expresso em algarismos.")]
        [Display(Name = "ECTS:")]
        public int Credits { get; set; }

        //required?
        [Display(Name = "Género:")]
        public string Gender { get; set; }

        [CheckDateRange]
        [Required(ErrorMessage = "Insira a sua data de nascimento.")]
        [Display(Name = "Data de Nascimento:")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Código Postal:")]
        public string PostalCode { get; set; }

        [Display(Name = "Morada:")]
        public string Address { get; set; }

        [Display(Name = "N.º da Porta:")]
        public int DoorNumber { get; set; }

        [Display(Name = "Andar:")]
        public string Floor { get; set; }

        public Account IdAccountNavigation { get; set; }
        [Display(Name = "Curso:")]
        public Course IdCourseNavigation { get; set; }

        [Display(Name = "Nacionalidade:")]
        public Nationality IdNationalityNavigation { get; set; }

        public ICollection<Application> Application { get; set; }
    }
}
