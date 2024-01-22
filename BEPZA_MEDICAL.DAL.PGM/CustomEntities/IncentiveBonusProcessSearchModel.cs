using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public class IncentiveBonusProcessSearchModel
    {
        public int Id { get; set; }
        public string FinancialYear { get; set; }
        public DateTime IncentiveBonusDate { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public int StaffCategoryId { get; set; }
        public string StaffCategoryName { get; set; }

        public int EmploymentTypeId { get; set; }
        public string EmploymentTypeName { get; set; }

        public int EmployeeId { get; set; }
        public string EmpID { get; set; }
        public string FullName { get; set; }
    }
}
