using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    /// <summary>
    ///  Class used to provide the User's profile information to the View. 
    ///  It contains an enum to represent the user type and both a Student and a Technician in order to show different values deppending on the type.
    /// </summary>
    public class ProfileViewModel
    {

        /// <summary>
        /// Property used to represent the different type of user of the profile.
        /// </summary>
        /// <value>User type</value>
        public EnumAccountType AccountType { get; set; }

        /// <summary>
        /// Property used provide information of the student to the view. This object is instantiated in the controller.
        /// </summary>
        /// <value>Studend</value>
        public Student Student { get; set; }

        [Required(ErrorMessage = "É necessário preencher o código postal")]
        [RegularExpression("^[0-9]{4}$",ErrorMessage =("O código postal deve ter 4 algarismos"))]
        public string PostalCode1 { get; set; }

        [Required(ErrorMessage = "É necessário preencher o código postal")]
        [RegularExpression("^[0-9]{3}$", ErrorMessage = ("O código postal deve ter 3 algarismos"))]
        public string PostalCode2 { get; set; }

        /// <summary>
        /// Property used provide information of the technician from CIMOB to the view. This object is instantiated in the controller.
        /// </summary>
        /// <value>vs</value>
        public Technician Technician { get; set; }

        /// <summary>Property used to represent the list of the courses selected from the database. </summary>
        /// <value>All the courses provided by the database. </value>
        public IEnumerable<SelectListItem> Courses { get; set; }
    }


}
