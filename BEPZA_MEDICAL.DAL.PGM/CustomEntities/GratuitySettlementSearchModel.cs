using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public class GratuitySettlementSearchModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmpID { get; set; }

        public string SalaryMonth { get; set; }
        public string SalaryYear { get; set; }
        public string EmployeeInitial { get; set; }
        public string FullName { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime DateofPayment { get; set; } 
        public DateTime DateofJoining { get; set; }
        public DateTime DateofConfirmation { get; set; } 
        public DateTime DateofInactive { get; set; }

        public string PaymentStatus { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal GratuityAmount { get; set; }
        public decimal ServiceLength { get; set; } 
    }
}
