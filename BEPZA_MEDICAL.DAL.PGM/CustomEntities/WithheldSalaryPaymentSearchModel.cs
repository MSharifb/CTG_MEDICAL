using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public class WithheldSalaryPaymentSearchModel
    {
        public Int64 Id { get; set; }
        public int EmployeeId { get; set; }
        public Int64 SalaryId { get; set; }

        public string EmpID { get; set; }
        public string SalaryMonth { get; set; }
        public string SalaryYear { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string View { get; set; }
        public decimal HeadAmount { get; set; }
        public string PaymentStatus { get; set; }

        public bool IsPaid { get; set; }

        public string FullName { get; set; }
        public string EmployeeInitial { get; set; }
        public string AccountNo { get; set; }
        public int BankId { get; set; }
        public int BranchId { get; set; }
    }
}
