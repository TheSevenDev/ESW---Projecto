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
        [Required]
        [Display(Name = "Curso:")]
        public long IdCourse { get; set; }
        [Required]
        [StringLength(60)]
        [Display(Name = "Nome:")]
        public string Name { get; set; }
        public string Address { get; set; }
        [Required]
        [Display(Name = "Cartão de Cidadão:")]
        public long Cc { get; set; }
        [Required]
        [Display(Name = "Nº Telemóvel:")]
        public long Telephone { get; set; }
        [Required]
        [Display(Name = "Nacionalidade:")]
        public long IdNationality { get; set; }
        [Required]
        [Display(Name = "ECTS:")]
        public int Credits { get; set; }
        [Required]
        [Display(Name = "Nº Estudante:")]
        public long StudentNum { get; set; }

        public Account IdAccountNavigation { get; set; }
        public Course IdCourseNavigation { get; set; }
        public Nationality IdNationalityNavigation { get; set; }
        public ICollection<Application> Application { get; set; }


    }
}
