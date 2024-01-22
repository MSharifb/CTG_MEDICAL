using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public  class MonthlySalaryDetail
    {
        public Int64 Id { get; set; }
        public string SalaryYear { get; set; }
        public string SalaryMonth { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeInitial { get; set; }
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public string AccountNo { get; set; }
        public int? DivisionId { get; set; }
        public int? EmploymentTypeId { get; set; }
        public decimal GrossSal { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal NetPay { get; set; }

        public Boolean IsWithheld { get; set; }
        public Boolean IsPaid { get; set; }

        public Boolean IsConfirmed { get; set; }

    }
}
