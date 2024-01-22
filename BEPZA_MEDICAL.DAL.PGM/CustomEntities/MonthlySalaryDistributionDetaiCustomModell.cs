using System;
using System.ComponentModel;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public class MonthlySalaryDistributionDetaiCustomModell
    {
        public Int64 Id { get; set; }
        public Int64 SalaryId { get; set; }
        public int ProjectId { get; set; }

       [DisplayName("Year")]
        public string SalaryYear { get; set; }

        [DisplayName("Month")]
        public string SalaryMonth { get; set; }
       
       public int EmployeeId { get; set; }
        public string EmployeeInitial { get; set; }
        public string EmpID { get; set; }
        public string FullName { get; set; }

        public string ProjectNo { get; set; }
        public int? DivisionId { get; set; }

        public decimal RatePerHour { get; set; }
        public decimal TotalHours { get; set; }
        public decimal GrossSalary { get; set; } 
        public decimal ProjectHour { get; set; }
        public decimal ProjectAmount { get; set; }
        
    }
}
