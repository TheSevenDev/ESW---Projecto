using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa uma instituição nacional ou internacional.
    /// </summary>
    public partial class Institution
    {
        public Institution()
        {
            ApplicationInstitutions = new HashSet<ApplicationInstitutions>();
            Course = new HashSet<Course>();
            InstitutionProgram = new HashSet<InstitutionProgram>();
            Mobility = new HashSet<Mobility>();
        }

        /// <summary>
        /// Chave primária da instituição.
        /// </summary>
        /// <value>Chave primária da instituição.</value>
        public long IdInstitution { get; set; }

        /// <summary>
        /// Nome da instituição.
        /// </summary>
        /// <value>Nome da instituição.</value>
        [Display(Name = "Instituição")]
        public string Name { get; set; }

        /// <summary>
        /// Chave estrangeira da <see cref="CIMOB_IPS.Models.Nationality" /> que representa o país onde a instiuição está sediada.
        /// </summary>
        /// <value>Chave estrangeira do país da insitutição.</value>
        public long IdNationality { get; set; }


        /// <summary>
        /// Hiperligação para o site da instituição.
        /// </summary>
        /// <value>Hiperligação para o site da instituição.</value>
        public string Hyperlink { get; set; }

        public Nationality IdNationalityNavigation { get; set; }
        public ICollection<ApplicationInstitutions> ApplicationInstitutions { get; set; }
        public ICollection<Course> Course { get; set; }
        public ICollection<InstitutionProgram> InstitutionProgram { get; set; }
        public ICollection<Mobility> Mobility { get; set; }
    }
}
