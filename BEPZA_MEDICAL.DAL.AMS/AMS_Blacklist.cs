//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BEPZA_MEDICAL.DAL.AMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class AMS_Blacklist
    {
        public AMS_Blacklist()
        {
            this.AMS_BlacklistRemoval = new HashSet<AMS_BlacklistRemoval>();
        }
    
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Reason { get; set; }
        public System.DateTime Date { get; set; }
        public int BlacklistedByEmpId { get; set; }
        public Nullable<int> ApprovedByEmpId { get; set; }
        public bool IsRevoked { get; set; }
        public string IUser { get; set; }
        public System.DateTime IDate { get; set; }
        public string EUser { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual AMS_AnsarEmpInfo AMS_AnsarEmpInfo { get; set; }
        public virtual PRM_EmploymentInfo PRM_EmploymentInfo { get; set; }
        public virtual ICollection<AMS_BlacklistRemoval> AMS_BlacklistRemoval { get; set; }
    }
}