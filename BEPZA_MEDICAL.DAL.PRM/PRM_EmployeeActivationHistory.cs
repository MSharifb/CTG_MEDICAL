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
    
    public partial class PRM_EmployeeActivationHistory
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public System.DateTime ActivationDate { get; set; }
        public string Reason { get; set; }
    
        public virtual PRM_EmploymentInfo PRM_EmploymentInfo { get; set; }
    }
}
