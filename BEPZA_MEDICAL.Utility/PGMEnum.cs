using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEPZA_MEDICAL.Utility
{
    public static class PGMEnum
    {

        public enum HouseRentDeductionType
        {
            Fixed = 1,
            HRA
        };

        public enum IncentiveBonusFormulaSelect
        {
            F1 = 1,
            F2,
            F3
        }

        public enum TaxAssesseeType
        {
            Male = 1,
            Female,
            People_With_Disability,
            Injured_Freedom_Fighter,
            Aged_65_or_Over
        }


        public enum AmountType
        {
            Fixed = 1,
            Percent
        }

        public enum CalculationBasedOn
        {
            Basic = 1,
            Gross
        }

        public enum TaxEntityType
        {
            OtherInvestment = 1,
            AdvanceTaxPaid
        }

        public enum DocumentType
        {
            TaxInvestment = 1,
            TaxAdvancePaid
        }

        public enum SalaryHeadType
        {
            Addition = 1,
            Deduction,
            Provision
        }

        public enum GetBasicCalculationMonthForBonus
        {
            Current_Month = 1,
            Previous_Month
        }

        public enum SalaryHeadMapper
        {
            House_Rent = 1,
            Medical,
            Conveyance,
            PF_Own_Contribution,
            PF_Company_Contribution,
            Leave_Without_Pay,
            Incometax_Addition,
            Incometax_Deduction,
            Pension_Provision,
            GPF,
            Arrear,
            Gratuity
        }

        public enum ImportXlFileType
        {
            Overtime = 1,
            Refreshment,
            Attendance
        }
    }
}
