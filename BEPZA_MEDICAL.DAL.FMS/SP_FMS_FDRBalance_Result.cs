//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BEPZA_MEDICAL.DAL.FMS
{
    using System;
    
    public partial class SP_FMS_FDRBalance_Result
    {
        public string FDRNumber { get; set; }
        public System.DateTime FDRDate { get; set; }
        public decimal FDRAmount { get; set; }
        public System.DateTime MaturityDate { get; set; }
        public decimal InterestAmount { get; set; }
        public Nullable<decimal> TAXAmount { get; set; }
        public Nullable<decimal> BankCharge { get; set; }
        public Nullable<decimal> Profit { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public string BankType { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
    }
}
