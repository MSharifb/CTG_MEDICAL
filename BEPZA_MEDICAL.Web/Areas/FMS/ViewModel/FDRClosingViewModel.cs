using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.ViewModel
{
    public class FDRClosingViewModel : BaseViewModel
    {
        #region Ctor
        public FDRClosingViewModel()
        {
            //this.IUser = HttpContext.Current.User.Identity.Name;
            //this.IDate = DateTime.Now;
            //this.strMode = "Create";

            this.ProfitRecvList = new List<SelectListItem>();
            this.ChequeList = new List<SelectListItem>();
            this.FDRNoList = new List<SelectListItem>();
            this.BankInfoList = new List<SelectListItem>();
            this.BankInfoBranchDetailList = new List<SelectListItem>();
            this.FDRClosingHistoryList = new List<FDRClosingViewModel>();
            this.FDRInstallmentList = new List<FDRClosingViewModel>();
            this.ApproverList = new List<SelectListItem>();
            this.SubLedgerList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Properties

        [DisplayName("Closing/Renew Date")]
        [UIHint("_RequiredDate")]
        public DateTime? ClosingDate { get; set; }

        [DisplayName("FDR No.")]
        public int FixedDepositInfoId { get; set; }
        public IList<SelectListItem> FDRNoList { get; set; }

        [DisplayName("FDR Amount")]
        public decimal InitialDeposit { get; set; }

        [DisplayName("FDR Amount(Present Value)")]
        public decimal FDRAmount { get; set; }

        [DisplayName("Interest Rate")]
        public decimal InterestRate { get; set; }

        [DisplayName("Tax Rate")]
        public decimal TaxRate { get; set; }

        [DisplayName("Total Excise Duty Others")]
        public decimal? BankCharge { get; set; }

        public decimal Duration { get; set; }

        [DisplayName("Ledger head (Profit/Income) of Accounts")]
        public int? ProfitRecvId { get; set; }
        public IList<SelectListItem> ProfitRecvList { get; set; }

        [DisplayName("Total Interest")]
        public decimal? InterestAmount { get; set; }

        [DisplayName("TAX Amount")]
        public decimal? TaxAmount { get; set; }

        [DisplayName("Net Profit")]
        public decimal ProfitAmount { get; set; }

        public decimal TotalReceivableAmount { get; set; }

        [Required]
        [DisplayName("Closing/Renew Amount")]
        public decimal? ClosingAmount { get; set; }

        [DisplayName("Accrued Amount (Profit)")]
        public decimal? WithdrawalAmount { get; set; }

        [DisplayName("Net Payable")]
        public decimal? LossAmount { get; set; }

        [DisplayName("Cheque No.")]
        public int? ChequeId { get; set; }
        public IList<SelectListItem> ChequeList { get; set; }

        public int ZoneInfoId { get; set; }
        public string Description { get; set; }
        public bool IsRenew { get; set; }
        public bool IsFDRSaved { get; set; }
        #endregion

        #region Others

        public string FDRNo { get; set; }
        public IList<FDRClosingViewModel> FDRClosingHistoryList { get; set; }
        public bool IsCheckedFinal { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string FundType { get; set; }

        public DateTime? FDRDateFrom { get; set; }
        public DateTime? FDRDateTo { get; set; }
        public DateTime? ClosingDateFrom { get; set; }
        public DateTime? ClosingDateTo { get; set; }

        public decimal? InterestRateFrom { get; set; }
        public decimal? InterestRateTo { get; set; }

        [DisplayName("Recommender/Approver")]
        public int? ApproverId { get; set; }
        public IList<SelectListItem> ApproverList { get; set; }

        [UIHint("_RequiredDate")]
        public DateTime? VoucherDate { get; set; }

        public string Narration { get; set; }
        [DisplayName("Sub Ledger")]
        public int SubLedgerId { get; set; }
        public IList<SelectListItem> SubLedgerList { get; set; }
        #endregion

        #region Closing History
        public IList<FDRClosingViewModel> FDRInstallmentList { get; set; }

        [DisplayName("Bank")]
        public int? BankInfoId { get; set; }
        public IList<SelectListItem> BankInfoList { get; set; }

        [DisplayName("Branch")]
        public int? BankInfoBranchDetailId { get; set; }
        public IList<SelectListItem> BankInfoBranchDetailList { get; set; }


        [DisplayName("FDR Date")]
        [UIHint("_Date")]
        public DateTime? FDRDate { get; set; }

        public DateTime? MaturityDate { get; set; }
        public string Renew { get; set; }

        public string InsDate { get; set; }
        public decimal? InsAmount { get; set; }

        public string Bank { get; set; }
        public string Branch { get; set; }
        #endregion

    }
}