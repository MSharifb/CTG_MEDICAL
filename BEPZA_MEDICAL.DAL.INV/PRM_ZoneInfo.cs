//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BEPZA_MEDICAL.DAL.INV
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRM_ZoneInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRM_ZoneInfo()
        {
            this.INV_TransferInInfo = new HashSet<INV_TransferInInfo>();
            this.INV_TransferInInfo1 = new HashSet<INV_TransferInInfo>();
            this.INV_TransferOutInfo = new HashSet<INV_TransferOutInfo>();
            this.INV_TransferOutInfo1 = new HashSet<INV_TransferOutInfo>();
            this.INV_IssueReturnInfo = new HashSet<INV_IssueReturnInfo>();
            this.INV_AdjustmentInfo = new HashSet<INV_AdjustmentInfo>();
            this.INV_AssetQuotaInfo = new HashSet<INV_AssetQuotaInfo>();
            this.INV_IssueInfo = new HashSet<INV_IssueInfo>();
            this.INV_DelegationApprovalInfo = new HashSet<INV_DelegationApprovalInfo>();
            this.PRM_EmploymentInfo = new HashSet<PRM_EmploymentInfo>();
            this.INV_RequisitionInfo = new HashSet<INV_RequisitionInfo>();
            this.INV_ScrapInfo = new HashSet<INV_ScrapInfo>();
            this.INV_PurchaseInfo = new HashSet<INV_PurchaseInfo>();
        }
    
        public int Id { get; set; }
        public string ZoneName { get; set; }
        public string ZoneCode { get; set; }
        public int SortOrder { get; set; }
        public int OrganogramCategoryTypeId { get; set; }
        public string ZoneAddress { get; set; }
        public string Prefix { get; set; }
        public string ZoneNameInBengali { get; set; }
        public string ZoneAddressInBengali { get; set; }
        public bool IsHeadOffice { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INV_TransferInInfo> INV_TransferInInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INV_TransferInInfo> INV_TransferInInfo1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INV_TransferOutInfo> INV_TransferOutInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INV_TransferOutInfo> INV_TransferOutInfo1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INV_IssueReturnInfo> INV_IssueReturnInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INV_AdjustmentInfo> INV_AdjustmentInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INV_AssetQuotaInfo> INV_AssetQuotaInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INV_IssueInfo> INV_IssueInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INV_DelegationApprovalInfo> INV_DelegationApprovalInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRM_EmploymentInfo> PRM_EmploymentInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INV_RequisitionInfo> INV_RequisitionInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INV_ScrapInfo> INV_ScrapInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INV_PurchaseInfo> INV_PurchaseInfo { get; set; }
    }
}
