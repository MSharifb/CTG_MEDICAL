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
    
    public partial class SP_FMS_FDRSchedule_Result
    {
        public int Id { get; set; }
        public int ZoneInfoId { get; set; }
        public string FDRNumber { get; set; }
        public Nullable<System.DateTime> FDRDate { get; set; }
        public string RenewalNo { get; set; }
        public string FDRName { get; set; }
        public string FDRType { get; set; }
        public int BankInfoId { get; set; }
        public string BankName { get; set; }
        public string BankType { get; set; }
        public int BankInfoBranchDetailId { get; set; }
        public string BranchName { get; set; }
        public Nullable<int> BankAccountId { get; set; }
        public Nullable<int> ProfitRecvId { get; set; }
        public decimal InitialDeposit { get; set; }
        public decimal FDRAmount { get; set; }
        public int FDRDuration { get; set; }
        public string FDRDurationType { get; set; }
        public decimal InterestRate { get; set; }
        public decimal InterestAmount { get; set; }
        public int InstallmentDuration { get; set; }
        public string InstallmentDurationType { get; set; }
        public decimal TAXRate { get; set; }
        public decimal TAXAmount { get; set; }
        public decimal BankCharge { get; set; }
        public decimal TotalBankCharge { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime MaturityDate { get; set; }
        public decimal TotalReceivableAmount { get; set; }
        public decimal TotalInterestAmount { get; set; }
        public decimal TotalTAXAmount { get; set; }
        public decimal TotalProfit { get; set; }
    }
}
