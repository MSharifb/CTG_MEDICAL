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
    
    public partial class PRM_SalaryHead
    {
        public PRM_SalaryHead()
        {
            this.PRM_EmpSalaryDetail = new HashSet<PRM_EmpSalaryDetail>();
            this.PRM_SalaryStructureDetail = new HashSet<PRM_SalaryStructureDetail>();
            this.PRM_EmpStatusChangeDetail = new HashSet<PRM_EmpStatusChangeDetail>();
        }
    
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string HeadName { get; set; }
        public string HeadType { get; set; }
        public string AmountType { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsGrossPayHead { get; set; }
        public int SortOrder { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual ICollection<PRM_EmpSalaryDetail> PRM_EmpSalaryDetail { get; set; }
        public virtual PRM_SalaryHeadGroup PRM_SalaryHeadGroup { get; set; }
        public virtual ICollection<PRM_SalaryStructureDetail> PRM_SalaryStructureDetail { get; set; }
        public virtual ICollection<PRM_EmpStatusChangeDetail> PRM_EmpStatusChangeDetail { get; set; }
    }
}