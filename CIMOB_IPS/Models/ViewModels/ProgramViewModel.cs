using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models.ViewModels
{
    public class ProgramViewModel
    {
        public ProgramViewModel()
        {
            Institutions = new List<CheckBoxListItem>();
        }

        [Required(ErrorMessage = "É necessário definir a data de abertura!")]
        [Display(Name = "Data de Abertura")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? OpenDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O número de vagas tem de ser um número positivo maior que 0.")]
        [Required(ErrorMessage = "É necessário definir o número de vagas!")]
        [Display(Name = "Vagas para candidaturas")]
        public int Vacancies { get; set; }

        [IsClosingAfterOpeningDate]
        [Required(ErrorMessage = "É necessário definir a data de encerramento!")]
        [Display(Name = "Data de Fecho")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? ClosingDate { get; set; }

        [Required(ErrorMessage = "É necessário definir a data de mobilidade!")]
        [Display(Name = "Data prevista de Mobilidade")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? MobilityDate { get; set; }

        [Required(ErrorMessage = "É necessário definir o tipo de programa!")]
        [Display(Name = "Tipo de Programa")]
        public long IdProgramType { get; set; }

        public IEnumerable<SelectListItem> ProgramTypes { get; set; }

        [Display(Name = "Instituições associadas")]
        public List<CheckBoxListItem> Institutions { get; set; }
    }
}
