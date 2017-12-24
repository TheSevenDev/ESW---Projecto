using PagedList;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models.ViewModels
{
    public class TechnicianManagementViewModel
    {
        public PaginatedList<PendingAccount> PendingAccounts { get; set; }

        public PaginatedList<Technician> Technicians { get; set; }

        public long IdAccount { get; set; }
        [Required(ErrorMessage = "O email não está preenchido")]
        [EmailAddress(ErrorMessage = "O email deverá ter a seguinte estrutura: exemplo@dominio.com")]
        [Display(Name = "E-mail:")]
        public string EmailView { get; set; }

        [Display(Name = "Administrador")]
        public bool IsAdmin { get; set; }
    }
}
