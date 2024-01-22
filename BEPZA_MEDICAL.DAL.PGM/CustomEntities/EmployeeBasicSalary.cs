using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public class EmployeeBasicSalary
    {
       public int Id { get; set; }
       public string EmpID { get; set; }
        public int EmployeeId { get; set; }
        public Decimal Amount { get; set; }
        public Decimal Basic { get; set; }
    }
}
