using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIMOB_IPS.Models
{
    public class RegisterViewModel
    {
        public Account Account{get; set;}

        public Student Student { get; set; }

        public Technician Technician { get; set; }       

        public IEnumerable<SelectListItem> Nationalities { get; set; }
    }
}
