using BEPZA_MEDICAL.Domain.PGM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.Settlement
{
    public class SettlementViewModel
    {
        #region Ctor
        public SettlementViewModel()
        {
            this.CPFPeriodList = new List<SelectListItem>();
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.Mode = "Create";
        }
        #endregion

        #region Standard Property

        [Required]
        public int Id { get; set; }

        [Required]
        [DisplayName("Membership ID")]
        [UIHint("_ReadOnlyInteger")]
        public int MembershipId { get; set; }

        [Required]
        public int PeriodId { set; get; }

        [Required]
        [DisplayName("Date of Inactive")]
        [UIHint("_ReadOnlyDate")]
        public DateTime? InactiveDate { set; get; }

        [Required]
        [DisplayName("Date of Settlement ")]
        [UIHint("_Date")]
        public DateTime? SettlementDate { set; get; }

        [Required]
        [DisplayName("Outgoing Interest Rate")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 99999, ErrorMessage = "Outgoing Interest Rate is required.")]
        public decimal OutgoingProfitRate { set; get; }

        [Required]
        [DisplayName("Membership Length (in Year)")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 99999, ErrorMessage = "Membership Length in Year is required.")]
        public decimal MembershipLengthInYear { set; get; }

        [DisplayName("Remarks")]
        public string Remarks { set; get; }

        //[Required]
        [DisplayName("Opening Balance")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Opening Balance is required.")]
        public decimal EmpOpening { set; get; }

        [Required]
        [DisplayName("Total Contribution")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Total Contribution is required.")]
        public decimal EmpContributionInPeriod { set; get; }

        [Required]
        [DisplayName("Total Share of Interest")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Total Share of Interest is required.")]
        public decimal EmpProftInPeriod { set; get; }

        [Required]
        [DisplayName("Withdrawn ")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Withdrawn is required.")]
        public decimal EmpWithdrawnInPeriod { set; get; }

        [Required]
        [DisplayName("Balance")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Balance is required.")]
        public decimal EmpBalance { set; get; }

        //[Required]
        [DisplayName("Opening Balance")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Opening Balance is required.")]
        public decimal ComOpening { set; get; }

        [Required]
        [DisplayName("Total Contribution")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Total Contribution is required.")]
        public decimal ComContributionInPeriod { set; get; }

        [Required]
        [DisplayName("Total Share of Interest")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Total Share of Interest is required.")]
        public decimal ComProftInPeriod { set; get; }

        //[Required]
        [DisplayName("Forfeited Amount")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Forfeited Amount is required.")]
        public decimal ForfeitedAmount { set; get; }

        [Required]
        [DisplayName(" Balance")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = " Balance must is required.")]
        public decimal ComBalance { set; get; }

        [Required]
        [DisplayName("Grand Total")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Grand Total is required.")]
        public decimal GrandTotal { set; get; }

        [DisplayName("Loan ID")]
        [UIHint("_OnlyNumber")]
        public int LoanId { set; get; }


        [DisplayName("Loan Amount")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Loan mount is required.")]
        public decimal LoanAmount { set; get; }


        [DisplayName("Loan Refund")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Loan Refund is required.")]
        public decimal LoanRefund { set; get; }


        [DisplayName("Due Loan")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Due Loan is required.")]
        public decimal DueLoan { set; get; }


        [DisplayName("Other Deduction")]
        [UIHint("_OnlyNumber")]
        [Range(0, 999999999999, ErrorMessage = "Other Deduction is required.")]
        public decimal OtherDeduction { set; get; }

        [Required]
        [DisplayName("Net Payable")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Net Payable is required.")]
        public decimal NetPayable { set; get; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        public string strMessage { set; get; }
        public string errClass { set; get; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        #endregion

        #region Other
        public String Mode { get; set; }
        public bool IsMonthly { get; set; }
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }

        #endregion

        # region View Property
        [Required]
        [DisplayName("Employee ID")]
        public string EmployeeCode { get; set; }

        //[Required]
        [DisplayName("Employee Initial")]
        [UIHint("_ReadOnly")]
        public string EmployeeInitial { get; set; }

        [Required]
        [DisplayName("Employee Name")]
        [UIHint("_ReadOnly")]
        public string EmployeeName { get; set; }

        [Required]
        [UIHint("_ReadOnly")]
        [DisplayName("Membership ID")]
        public string MembershipCode { set; get; }

        [UIHint("_Date")]
        [DisplayName("Permanent Date")]
        public DateTime? PermanentDate { set; get; }

        [DisplayName("Permanent Date To")]
        [UIHint("_Date")]
        public DateTime? PermanentDateTo { set; get; }

        [DisplayName("Date of Active")]
        [UIHint("_ReadOnlyDate")]
        public DateTime? ActiveDate { set; get; }

        [DisplayName("Settlement To Date")]
        [UIHint("_Date")]
        public DateTime? SettlementToDate { set; get; }

        [Required]
        [DisplayName("CPF Period")]
        [UIHint("_ReadOnly")]
        public string CpfPeriod { set; get; }
        public IList<SelectListItem> CPFPeriodList { set; get; }

        [CustomDisplay("CustomDisplay")]
        public String CompanyPortion { get; set; }

        #endregion
    }
}