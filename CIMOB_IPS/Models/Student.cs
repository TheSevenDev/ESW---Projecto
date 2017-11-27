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
        [Required(ErrorMessage = "O Curso não está preenchido.")]
        [Display(Name = "Curso:")]
        public long IdCourse { get; set; }
        [Required(ErrorMessage = "O Nome não está preenchido.")]
        [StringLength(60 ,ErrorMessage = "O Nome deve conter no máximo 60 caracteres.")]
        [Display(Name = "Nome:")]
        public string Name { get; set; }
        public string Address { get; set; }
        [Required(ErrorMessage = "O Cartão de Cidadão não está preenchido.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "O Cartão de Cidadão tem de ser expresso em algarismos.")]
        [Display(Name = "Cartão de Cidadão:")]
        public long Cc { get; set; }
        [Required(ErrorMessage = "O Nº Telemóvel não está preenchido.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "O Nº Telemovel tem de ser expresso em algarismos.")]
        [MinLength(9, ErrorMessage = "O Nº Telemóvel tem de conter no minimo 9 algarismos.")]
        [Display(Name = "Nº Telemóvel:")]
        public long Telephone { get; set; }
        [Required(ErrorMessage = "A Nacionalidade não está preenchida.")]
        [Display(Name = "Nacionalidade:")]
        public long IdNationality { get; set; }
        [Required(ErrorMessage = "Os ECTS não está preenchido.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Os ECTS tem de ser expresso em algarismos.")]
        [Display(Name = "ECTS:")]
        public int Credits { get; set; }
        [Required (ErrorMessage = "O Nº Estudante não está preenchido.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "O Nº Estudante tem de ser expresso em algarismos.")]
        [Display(Name = "Nº Estudante:")]
        public long StudentNum { get; set; }

        public Account IdAccountNavigation { get; set; }
        public Course IdCourseNavigation { get; set; }
        public Nationality IdNationalityNavigation { get; set; }
        public ICollection<Application> Application { get; set; }


    }
}
