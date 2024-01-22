using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public class BonusProcessSearchModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string EmpID { get; set; } 

        public string FullName { get; set; }

        public string EmployeeInitial { get; set; }

        public string BonusMonth { get; set; }

        public string BonusYear { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int BonusTypeId { get; set; }

        public string BonusType { get; set; }

        public bool IsAll { get; set; }

        public int ReligionId { get; set; }

        public string AmountType { get; set; }
       
        public Decimal BonusAmount { get; set; }

        public Decimal RevenueStamp { get; set; }
    }
}
