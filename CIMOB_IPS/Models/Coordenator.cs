using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    public partial class Coordenator
    {
        public long IdCoordenator { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Contacto")]
        public long? Telephone { get; set; }

        public long IdCourse { get; set; }

        [Display(Name = "Curso")]
        public Course IdCourseNavigation { get; set; }
    }
}
