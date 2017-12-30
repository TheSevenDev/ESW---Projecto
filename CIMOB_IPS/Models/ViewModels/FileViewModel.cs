using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models.ViewModels
{
    public class FileViewModel
    {
        [Display(Name = "Ficheiro")]
        public IFormFile File { get; set; }
    }
}
