using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public  class BonusDetailsSearchModel
    {
        public Int64 Id { get; set; }

        public int BonusId { get; set; }

        public int? DivisionId { get; set; }
        public string Division { get; set; }

        public int? DesignationId { get; set; }

        public int? GradeId { get; set; }

        public int? BankId { get; set; }

        public int? BranchId { get; set; }

        public int? BonusTypeId { get; set; }

        public int? EmpProjectId { get; set; } 

        public string ProjectNo { get; set; }

        public string AccountNo { get; set; }

        public Decimal EmpBasicSalary { get; set; }

        public Decimal EmpBonusAmount { get; set; }

        public Decimal EmpRevenueStamp { get; set; }

        public Decimal EmpNetPayable { get; set; }

        public int EmployeeId { get; set; }

        public string EmpID { get; set; } 

        public string FullName { get; set; }

        public string EmployeeInitial { get; set; }
    }
}
