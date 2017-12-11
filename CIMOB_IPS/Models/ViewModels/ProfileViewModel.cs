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

        /// <summary>
        /// Property used provide information of the technician from CIMOB to the view. This object is instantiated in the controller.
        /// </summary>
        /// <value>vs</value>
        public Technician Technician { get; set; }
    }


}
