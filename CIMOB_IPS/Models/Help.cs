using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Help
    {
        public long IdHelp { get; set; }
        public string Title { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Description { get; set; }
    }
}
