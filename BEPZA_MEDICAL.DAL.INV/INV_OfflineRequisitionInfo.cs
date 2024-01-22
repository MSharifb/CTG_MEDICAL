//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP_BEPZA.DAL.INV
{
    using System;
    using System.Collections.Generic;
    
    public partial class INV_OfflineRequisitionInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public INV_OfflineRequisitionInfo()
        {
            this.INV_OfflineRequisitionItem = new HashSet<INV_OfflineRequisitionItem>();
        }
    
        public int Id { get; set; }
        public int ZoneInfoId { get; set; }
        public System.DateTime IndentDate { get; set; }
        public string IndentNo { get; set; }
        public int IssuedByEmpId { get; set; }
        public int IssuedToEmpId { get; set; }
        public string Comment { get; set; }
        public string IUser { get; set; }
        public System.DateTime IDate { get; set; }
        public string EUser { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual PRM_ZoneInfo PRM_ZoneInfo { get; set; }
        public virtual PRM_EmploymentInfo PRM_EmploymentInfo { get; set; }
        public virtual PRM_EmploymentInfo PRM_EmploymentInfo1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INV_OfflineRequisitionItem> INV_OfflineRequisitionItem { get; set; }
    }
}
