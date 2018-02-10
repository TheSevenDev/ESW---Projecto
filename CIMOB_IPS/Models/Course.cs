using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa um curso quer no IPS, quer de instiuições estrangeiras.
    /// </summary>
    public partial class Course
    {
        public Course()
        {
            Student = new HashSet<Student>();
        }

        /// <summary>
        /// Chave primária do curso.
        /// </summary>
        /// <value>Chave primária do curso.</value>
        public long IdCourse { get; set; }

        /// <summary>
        /// Chave estrangeira da <see cref="CIMOB_IPS.Models.Institution" /> a que pertence o curso.
        /// </summary>
        /// <value>Chave estrangeira da instituição a que pertence o curso.</value>
        public long IdInstitution { get; set; }

        /// <summary>
        /// Nome do curso.
        /// Ex. Engenharia Informática
        /// </summary>
        /// <value>Nome do curso.</value>
        [Display(Name = "Curso")]
        public string Name { get; set; }

        public Institution IdInstitutionNavigation { get; set; }
        public ICollection<Student> Student { get; set; }
        public ICollection<Coordenator> Coordenator { get; set; }
    }
}
