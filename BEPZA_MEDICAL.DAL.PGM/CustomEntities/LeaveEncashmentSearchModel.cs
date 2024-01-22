
namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public class LeaveEncashmentSearchModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string EmpID { get; set; }
        public string SalaryMonth { get; set; }
        public string SalaryYear { get; set; }
        public string EmployeeInitial { get; set; }
        public string FullName { get; set; }
        public string ProjectNo { get; set; }
        public decimal EncashmentDays { get; set; }
        public decimal EncashmentRate { get; set; }
        public decimal EncashmentAmount { get; set; }

    }
}
