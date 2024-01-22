using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public class OtherAdjustDeductSearchModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string SalaryMonth { get; set; }
        public string SalaryYear { get; set; }

        public string Type { get; set; }
        public int SalaryHeadId { get; set; }
        public String SalaryHead { get; set; }

        public string EmpID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeDesignation { get; set; }
        public Decimal Amount { get; set; }
        public bool IsOverrideStructureAmount { get; set; }
    }
}
