using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe usada para representar uma nacionalidade de um estudante ou país de uma instituição.
    /// </summary>
    public partial class Nationality
    {
        public Nationality()
        {
            Institution = new HashSet<Institution>();
            Student = new HashSet<Student>();
        }

        /// <summary>
        /// Chave primária da nacionalidade.
        /// </summary>
        /// <value>Chave primária da nacionalidade.</value>
        public long IdNationality { get; set; }

        /// <summary>
        /// Descrição da nacionalidade.
        /// </summary>
        /// <value>Descrição da nacionalidade.</value>
        public string Description { get; set; }

        public ICollection<Institution> Institution { get; set; }
        public ICollection<Student> Student { get; set; }
    }
}
