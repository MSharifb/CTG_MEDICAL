//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BOM_MPA.DAL.PRM
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRM_EmpContractPersonInfo
    {
        public int EmployeeId { get; set; }
        public Nullable<int> TitleId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public int RelationId { get; set; }
        public string MobileNo { get; set; }
        public string OfficePhone { get; set; }
        public string HomePhone { get; set; }
        public string Faxno { get; set; }
        public string EmailAddress { get; set; }
        public string OfficeAddress { get; set; }
        public string HomeAddress { get; set; }
        public bool isAddPhoto { get; set; }
        public byte[] Photo { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual PRM_NameTitle PRM_NameTitle { get; set; }
        public virtual PRM_Relation PRM_Relation { get; set; }
        public virtual PRM_EmploymentInfo PRM_EmploymentInfo { get; set; }
    }
}