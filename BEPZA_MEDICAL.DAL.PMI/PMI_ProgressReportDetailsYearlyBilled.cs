//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BEPZA_MEDICAL.DAL.PMI
{
    using System;
    using System.Collections.Generic;
    
    public partial class PMI_ProgressReportDetailsYearlyBilled
    {
        public int Id { get; set; }
        public int ProgressReportDetailsId { get; set; }
        public Nullable<int> FinancialYearId { get; set; }
        public Nullable<decimal> BilledAmount { get; set; }
    
        public virtual PMI_ProgressReportDetails PMI_ProgressReportDetails { get; set; }
    }
}
