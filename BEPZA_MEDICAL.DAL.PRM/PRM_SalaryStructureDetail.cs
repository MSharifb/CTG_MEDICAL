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
    
    public partial class PRM_SalaryStructureDetail
    {
        public int Id { get; set; }
        public int SalaryStructureId { get; set; }
        public int HeadId { get; set; }
        public string HeadType { get; set; }
        public string AmountType { get; set; }
        public decimal Amount { get; set; }
        public bool IsTaxable { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual PRM_SalaryHead PRM_SalaryHead { get; set; }
        public virtual PRM_SalaryStructure PRM_SalaryStructure { get; set; }
    }
}
