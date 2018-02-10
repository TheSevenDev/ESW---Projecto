using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// CheckBoxListItem, representa um controlo checkbox numa lista.
    /// </summary>
    /// <remarks></remarks>
    public class CheckBoxListItem
    {
        public long ID { get; set; }
        public string Display { get; set; }
        public bool IsChecked { get; set; }
    }
}
