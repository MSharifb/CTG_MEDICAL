using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public class IncentiveBonusDetailProcessSearchModel
    {
        public Int64 Id { get; set; }
        public int IncentiveBonusId { get; set; }
        public int EmployeeId { get; set; }
        public Decimal BasicSalary { get; set; }
        public Decimal IncentiveBonusAmount { get; set; }
        public Decimal RevenueStamp { get; set; }
        public Decimal NetPayable { get; set; }

        public string EmpID { get; set; }
        public string FullName { get; set; }
    }
}
