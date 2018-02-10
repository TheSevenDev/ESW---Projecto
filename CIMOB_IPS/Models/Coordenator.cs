using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa o coordenador de um cursos.
    /// </summary>
    public partial class Coordenator
    {
        /// <summary>
        /// Chave primária do coordenador de curso.
        /// </summary>
        /// <value>Chave primária do coordenador de curso.</value>
        public long IdCoordenator { get; set; }

        /// <summary>
        /// Nome do coordenador de curso.
        /// </summary>
        /// <value>Nome do coordenador de curso.</value>
        [Display(Name = "Nome")]
        public string Name { get; set; }

        /// <summary>
        /// Endereço email do coordenador de curso.
        /// </summary>
        /// <value>Endereço email do coordenador de curso.</value>
        [EmailAddress(ErrorMessage = "O email deverá conter a seguinte estrutura: exemplo@dominio.com")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        /// <summary>
        /// Contacto do coordenador de curso.
        /// </summary>
        /// <value>Contacto do coordenador de curso.</value>
        [Display(Name = "Contacto")]
        public long? Telephone { get; set; }

        /// <summary>
        /// Chave estrangeira do <see cref="CIMOB_IPS.Models.Course" /> do coordenador.
        /// </summary>
        /// <value>Chave estrangeira do curso do coordenador.</value>
        public long IdCourse { get; set; }

        [Display(Name = "Curso")]
        public Course IdCourseNavigation { get; set; }
    }
}
