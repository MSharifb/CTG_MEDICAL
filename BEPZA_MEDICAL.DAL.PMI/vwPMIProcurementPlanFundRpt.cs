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
    
    public partial class vwPMIProcurementPlanFundRpt
    {
        public int ID { get; set; }
        public int ProjectMasterId { get; set; }
        public string SourceOfFund { get; set; }
        public Nullable<decimal> Cost { get; set; }
    }
}