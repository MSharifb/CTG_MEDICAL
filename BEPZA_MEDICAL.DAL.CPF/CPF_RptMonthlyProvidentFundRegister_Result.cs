//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BEPZA_MEDICAL.DAL.CPF
{
    using System;
    
    public partial class CPF_RptMonthlyProvidentFundRegister_Result
    {
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> PfMonthId { get; set; }
        public string PfMonthName { get; set; }
        public string PeriodName { get; set; }
        public Nullable<decimal> OwnContrib { get; set; }
        public decimal EmpTotalContrib { get; set; }
        public decimal ComTotalContirb { get; set; }
        public decimal Profit { get; set; }
        public decimal TotalAmount { get; set; }
        public Nullable<int> SequenceNo { get; set; }
    }
}
