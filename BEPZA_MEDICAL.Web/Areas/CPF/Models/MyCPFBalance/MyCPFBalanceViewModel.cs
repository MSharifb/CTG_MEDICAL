using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using BEPZA_MEDICAL.Domain.PGM;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.MyCPFBalance
{
    public class MyCPFBalanceViewModel
    {

        #region Standard Porperties

        public int Id { get; set; }

        [DisplayName("Employee ID")]
        public string EmpID { get; set; }

        public int EmployeeId { get; set; }

        [DisplayName("Membership ID")]
        public string MembershipID { get; set; }

        [DisplayName("Employee Initial")]
        public string EmployeeInitial { get; set; }

        [DisplayName("Employee Name")]
        public string FullName { get; set; }

        [DisplayName("Balance On")]
        public string BalanceOn { get; set; }

        [DisplayName("Membership Length")]
        public decimal? MembershipLength { get; set; }

        [DisplayName("Own Contribution Amount")]
        public decimal? OwnContributionAmount { get; set; }

        [DisplayName("Own Interest Amount")]
        public decimal? OwnShareofProfit { get; set; }

        [DisplayName("Withdraw ( - )")]
        public decimal OwnWithdraw { get; set; }

        [DisplayName("Own Total")]
        public decimal OwnTotal { get; set; }

        [CustomDisplay("Company Contribution Amount")]
        public decimal ComContributionAmount { get; set; }

        [CustomDisplay("Company Interest Amount")]
        public decimal ComShareofProfit { get; set; }

        [DisplayName("To Be Forfeited ( - ) ")]
        public decimal ForfeitedAmount { get; set; }

        // No display text
        public decimal ComTotal { get; set; }

        [CustomDisplay("Company Total")]
        public decimal ComTotalWithoutForfietedAmount { get; set; }

        [CustomDisplay("Total (Own + Company)")]
        public decimal PFTotal { get; set; }

        [DisplayName("LoanAmount")]
        public decimal LoanAmount { get; set; }

        [DisplayName("Both Contribution")]
        public decimal BothContribution { get; set; }

        [DisplayName("Paid (-)")]
        public decimal PaidAmount { get; set; }

        [DisplayName("Dues")]
        public decimal Dues { get; set; }

        [DisplayName("Loan Dues")]
        public decimal LoanDues { get; set; }

        [DisplayName("Net Balance")]
        public decimal NetBalance { get; set; }      

        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }

        [CustomDisplay("Employer Part (B)")]
        public String EmployerPart { get; set; }

        #endregion
    }
}