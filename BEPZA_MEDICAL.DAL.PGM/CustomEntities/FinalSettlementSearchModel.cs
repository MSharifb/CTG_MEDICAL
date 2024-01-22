using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public class FinalSettlementSearchModel
    {

        public int EmployeeId { get; set; }
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public string EmployeeInitial { get; set; }       
        public string Division { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime DateofSettlement { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal GrossSalary { get; set; }

        public int ShortageDays { get; set; }
        public int EarnLeaveBalance { get; set; }
        public int AdjustLeave { get; set; }
        public int UnAdjsutedLeave { get; set; }
        public int LastMonthWorkedDays { get; set; }

        public decimal SalaryPayable { get; set; }
        public decimal LeaveEncasement { get; set; }
        public decimal GratuityPayable { get; set; }
        public decimal OtherAdjustment { get; set; }
        public decimal ShortageofNoticePeriod { get; set; }
        public decimal OtherDeduction { get; set; }
        public decimal AdvanceDeduction { get; set; }
        public decimal NetPayable { get; set; }
        public decimal NetPFBalance { get; set; }

    }
}
