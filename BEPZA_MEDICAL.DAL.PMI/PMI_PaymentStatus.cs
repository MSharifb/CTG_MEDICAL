//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BEPZA_MEDICAL.DAL.PMI
{
    using System;
    using System.Collections.Generic;
    
    public partial class PMI_PaymentStatus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PMI_PaymentStatus()
        {
            this.PMI_ProjectMaster = new HashSet<PMI_ProjectMaster>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public string Remarks { get; set; }
        public string IUser { get; set; }
        public System.DateTime IDate { get; set; }
        public string EUser { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PMI_ProjectMaster> PMI_ProjectMaster { get; set; }
    }
}
