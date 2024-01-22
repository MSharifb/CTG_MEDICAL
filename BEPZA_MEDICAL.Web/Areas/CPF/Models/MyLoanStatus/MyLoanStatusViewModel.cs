using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.MyLoanStatus
{
    public class MyLoanStatusViewModel
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

        [DisplayName("Loan No.")]
        public string LoanNo { get; set; }

        [DisplayName("Loan Amount")]
        public decimal? LoanAmount { get; set; }

        [DisplayName("Loan Date")]
        public string LoanDate { get; set; }

        [DisplayName("Interest Rate")]
        public decimal InterestRate { get; set; }

        [DisplayName("Interest Amount")]
        public decimal InterestAmount { get; set; }

        [DisplayName("No. Of Installment Principal")]
        public int NoOfInstallmentPrincipal { get; set; }

        [DisplayName("No. Of Installment Interest")]
        public int NoOfInstallmentInterest { get; set; }

        [DisplayName("Total No. Of Installment")]
        public int TotalNoOfInstallment { get; set; }

        [DisplayName("Total Repayment")]
        public decimal TotalRepayment { get; set; }

        [DisplayName("Repayment Date")]
        public string RepaymentDate { get; set; }

        [DisplayName("Repayment End Date")]
        public string RepaymentEndDate { get; set; }

        [DisplayName("Total Installment")]
        public decimal PrincipalInstallment { get; set; }

        [DisplayName("Interest Installment")]
        public decimal InterestInstallment { get; set; }

        [DisplayName("No. Of Paid Installment")]
        public int NoOfPaidInstallment { get; set; }

        [DisplayName("Paid Amount")]
        public decimal PaidInstallmentAmount { get; set; }

        [DisplayName("No. Of Due Installment")]
        public int NoOfUnPaidInstallment { get; set; }

        [DisplayName("Due Amount")]
        public decimal  UnpaidInstallmentAmount { get; set; }

        public int ID { get; set; }

        public int LoanId { get; set; }

        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; } 

        #endregion
    }
}