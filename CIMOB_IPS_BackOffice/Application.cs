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
    
    public partial class Application
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Application()
        {
            this.Mobility = new ObservableCollection<Mobility>();
        }
    
        public long id_application { get; set; }
        public long id_student { get; set; }
        public long id_state { get; set; }
        public bool has_scholarship { get; set; }
        public short final_evaluation { get; set; }
        public string motivation_card { get; set; }
        public string emergency_contact_name { get; set; }
        public string emergency_contact_relation { get; set; }
        public long emergency_contact_telephone { get; set; }
        public Nullable<long> id_program { get; set; }
        public System.DateTime application_date { get; set; }
        public byte[] signed_app_file { get; set; }
    
        public virtual Program Program { get; set; }
        public virtual State State { get; set; }
        public virtual Student Student { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<Mobility> Mobility { get; set; }
    }
}
