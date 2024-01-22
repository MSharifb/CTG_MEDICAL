//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BEPZA_MEDICAL.DAL.FMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class FMS_BankInfoBranchDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FMS_BankInfoBranchDetail()
        {
            this.FMS_FixedDepositInfo = new HashSet<FMS_FixedDepositInfo>();
            this.FMS_BankWiseOfferRate = new HashSet<FMS_BankWiseOfferRate>();
        }
    
        public int Id { get; set; }
        public int BankInfoId { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string SWIFTCode { get; set; }
        public string BranchContactNo { get; set; }
        public string BranchEmail { get; set; }
        public int CountryId { get; set; }
        public string IUser { get; set; }
        public System.DateTime IDate { get; set; }
        public string EUser { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FMS_FixedDepositInfo> FMS_FixedDepositInfo { get; set; }
        public virtual FMS_BankInfo FMS_BankInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FMS_BankWiseOfferRate> FMS_BankWiseOfferRate { get; set; }
    }
}
