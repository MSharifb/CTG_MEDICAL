namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public class SalaryMonthInfo
    {
        public int Id { get; set; }
        public string SalaryMonth { get; set; }

        public string SalaryYear { get; set; }

        public int EmployeeId { get; set; }
        public bool IsConfirmed { get; set; }
        
    }
}
