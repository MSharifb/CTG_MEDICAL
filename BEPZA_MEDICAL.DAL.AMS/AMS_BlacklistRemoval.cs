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
    
    public partial class AMS_BlacklistRemoval
    {
        public int Id { get; set; }
        public int BlacklistId { get; set; }
        public System.DateTime DateofRemoval { get; set; }
        public string Remarks { get; set; }
        public string IUser { get; set; }
        public System.DateTime IDate { get; set; }
        public string EUser { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        public int RemovedByEmpId { get; set; }
        public int ApprovedByEmpId { get; set; }
    
        public virtual PRM_EmploymentInfo PRM_EmploymentInfo { get; set; }
        public virtual PRM_EmploymentInfo PRM_EmploymentInfo1 { get; set; }
        public virtual AMS_Blacklist AMS_Blacklist { get; set; }
    }
}