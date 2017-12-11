using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    /// <summary>Class used to provide the register credentials to the RegisterView. Contains an Account, Student, Technician and all the nationalities.</summary>
    public class RegisterViewModel
    {
        /// <summary>Property used to represent the new account created. From this property there will be an email, password and password confirmation camps to fill.</summary>
        /// <value>New generated account with the given email and password.</value>
        public Account Account { get; set; }

        /// <summary>Property used to represent the new Student created with the values provided in the form.  </summary>
        /// <value>New generated Student with the given data.</value>
        public Student Student { get; set; }

        /// <summary>Property used to represent the new Technician created with the values provided in the form.   </summary>
        /// <value>New generated Technician with the given data.</value>
        public Technician Technician { get; set; }

        /// <summary>Property used to represent the list of the nationalities selected from the database. </summary>
        /// <value>All the nationalities provided by the database. </value>
        public IEnumerable<SelectListItem> Nationalities { get; set; }

        public IEnumerable<SelectListItem> Institutions { get; set; }
    }
}
