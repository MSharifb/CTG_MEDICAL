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
    
    public partial class vwTransferredEmpRpt
    {
        public int Id { get; set; }
        public string EmployeeInitial { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public System.DateTime DateofJoining { get; set; }
        public Nullable<System.DateTime> TransferDate { get; set; }
        public string FromDiv { get; set; }
        public string ToDiv { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public Nullable<int> DesigId { get; set; }
        public Nullable<int> FromDivId { get; set; }
        public Nullable<int> ToDivId { get; set; }
        public Nullable<int> FormLocId { get; set; }
        public Nullable<int> ToLocId { get; set; }
        public string EmpID { get; set; }
    }
}
