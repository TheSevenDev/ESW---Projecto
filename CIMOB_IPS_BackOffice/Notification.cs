//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CIMOB_IPS_BackOffice
{
    using System;
    using System.Collections.ObjectModel;
    
    public partial class Notification
    {
        public long id_notification { get; set; }
        public long id_account { get; set; }
        public string description { get; set; }
        public bool read_notification { get; set; }
        public string controller_name { get; set; }
        public string action_name { get; set; }
        public System.DateTime notification_date { get; set; }
    
        public virtual Account Account { get; set; }
    }
}
