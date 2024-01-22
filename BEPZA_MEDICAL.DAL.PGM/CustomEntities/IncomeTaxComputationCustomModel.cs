using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public  class IncomeTaxComputationCustomModel
    {
        public Int64 Id { get; set; }
        public int TaxRuleId { get; set; }

        public int EmployeeId { get; set; }
        public string EmpID { get; set; }
        public string EmployeeInitial { get; set; }
        public string FullName { get; set; }

        public int? DivisionId { get; set; }
        public int? DesignationId { get; set; }
        public int? EmploymentTypeId { get; set; }

        public string Gender { get; set; }
        public int? AgeInYear { get; set; }
        public int? ActualMonth { get; set; }
        public int? RemainingMonth { get; set; }

        public Decimal TaxPayable { get; set; }
        public Decimal TaxDeducted { get; set; }
        public Decimal TaxDue { get; set; }
        public Decimal TaxPerMonth { get; set; }
        public Decimal InvestmentRebate { get; set; }
        public Decimal SpecialRebate { get; set; }
        public Decimal TaxableIncome { get; set; }
        public Decimal PFCompany { get; set; }
        public Decimal TaxLiability { get; set; }

        public string IncomeYear { get; set; }

        public string AssessmentYear { get; set; }
       
        public string SalaryMonth { get; set; }
       
        public string SalaryYear { get; set; }
    }
}
