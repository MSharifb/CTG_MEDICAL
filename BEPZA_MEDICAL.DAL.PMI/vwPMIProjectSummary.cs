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
    
    public partial class vwPMIProjectSummary
    {
        public int ProjectMasterId { get; set; }
        public int AnnualProcurementPlanMasterId { get; set; }
        public int AnnualProcurementPlanDetailsId { get; set; }
        public string DescritionOfAPP { get; set; }
        public string ProjectCode { get; set; }
        public Nullable<System.DateTime> TenderPubDate { get; set; }
        public Nullable<int> MinistryId { get; set; }
        public string MinistryName { get; set; }
        public int FinancialYearId { get; set; }
        public string FinancialYearName { get; set; }
        public decimal TotalCost { get; set; }
        public Nullable<int> ProjectStatusId { get; set; }
        public string ProjectStatusName { get; set; }
        public Nullable<int> ZoneId { get; set; }
        public string ZoneCode { get; set; }
    }
}