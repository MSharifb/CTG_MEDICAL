using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public  class BankAdviceLetterSearchModel
    {
        #region Standard Properties

        public Int64 Id { get; set; }

        public int EmployeeId { get; set; }

        public string LetterType { get; set; }
        public string SelectedLetterType { get; set; }
        public string LetterTypeClickChange { get; set; }

        public DateTime DateofLetter { get; set; }

        public decimal LimitAmount { get; set; }

        public string SalaryMonth { get; set; }

        public string SalaryYear { get; set; }

        public string SignatoryName { get; set; }

        public string SignatoryDesignation { get; set; }

        public string ReferenceNo { get; set; }

        public string AccountNo { get; set; }
        public string BankName { get; set; }
        public int BankId { get; set; }

        public int BranchId { get; set; }
        public string BankAddress { get; set; }

        public Decimal TotalAmount{ get; set; }
        //public Decimal BonusAmount { get; set; }

        public int ZoneInfoId { get; set; }

        #endregion 
    }

}
