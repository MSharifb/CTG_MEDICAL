using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public  class VehicleDeductionBillList
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        
        public string EmpID { get; set; }

        public string SalaryMonth { get; set; }

        public string SalaryYear { get; set; }

        public string FullName { get; set; }

        public string EmployeeInitial { get; set; }
       
        public string Designation { get; set; }

        public Decimal PersonalAmount { get; set; }

        public Decimal OfficalAmount { get; set; }

        public string Remarks { get; set; }
    }
}
