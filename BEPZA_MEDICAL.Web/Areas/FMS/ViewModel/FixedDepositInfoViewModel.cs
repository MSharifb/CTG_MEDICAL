using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.ViewModel
{
    public class FixedDepositInfoViewModel : BaseViewModel
    {
        #region Ctor
        public FixedDepositInfoViewModel()
        {
            this.FixedDepositTypeInfoList = new List<SelectListItem>();
            this.BankInfoList = new List<SelectListItem>();
            this.BankInfoBranchDetailList = new List<SelectListItem>();
            this.BankAccountList = new List<SelectListItem>();
            this.ProfitRecvList = new List<SelectListItem>();
            this.SourceofFundList = new List<SelectListItem>();

            this.AccBankList = new List<SelectListItem>();
            this.AccBranchList = new List<SelectListItem>();
            this.ChequeList = new List<SelectListItem>();
            this.FDRDurationTypeList = new List<SelectListItem>();
            this.InstallmentDurationTypeList = new List<SelectListItem>();

            this.InstallmentSchedulList = new List<FixedDepositInfoInstallmentScheduleViewModel>();
        }
        #endregion

        #region Fixed Deposit Info
        [Required]
        public int ZoneInfoId { get; set; }

        [Required]
        [DisplayName("FDR Number")]

        public string FDRNumber { get; set; }

        [Required]
        [DisplayName("FDR Date")]
        [UIHint("_RequiredDate")]
        public DateTime? FDRDate { get; set; }

        //[Required]
        [DisplayName("Renewal No.")]
        [UIHint("_ReadOnly")]
        public string RenewalNo { get; set; }

        [Required]
        [DisplayName("FDR Type")]
        public int FixedDepositTypeInfoId { get; set; }
        public IList<SelectListItem> FixedDepositTypeInfoList { get; set; }

        [Required]
        [DisplayName("Bank")]
        public int BankInfoId { get; set; }
        public IList<SelectListItem> BankInfoList { get; set; }

        [Required]
        [DisplayName("Branch")]
        public int BankInfoBranchDetailId { get; set; }
        public IList<SelectListItem> BankInfoBranchDetailList { get; set; }

        [Required]
        [DisplayName("FDR Name")]
        public string FDRName { get; set; }


        [DisplayName("FDR Description")]
        [MaxLength(250, ErrorMessage = "Maximum length 250.")]
        public string FDRDescription { get; set; }

        [DisplayName("Bank Account No.")]
        public int? BankAccountId { get; set; }
        public IList<SelectListItem> BankAccountList { get; set; }

        [DisplayName("Ledger head (Profit/Income) of Accounts")]
        public int? ProfitRecvId { get; set; }
        public IList<SelectListItem> ProfitRecvList { get; set; }

        [Required]
        [DisplayName("FDR Type")]
        public int FDRTypeId { get; set; }

        [DisplayName("Source of Fund")]
        public int? SourceofFundId { get; set; }
        public IList<SelectListItem> SourceofFundList { get; set; }
        #endregion

        #region cheque info

        [DisplayName("Bank")]
        public int? AccBankId { get; set; }
        public IList<SelectListItem> AccBankList { get; set; }

        [DisplayName("Branch")]
        public int? AccBranchId { get; set; }
        public IList<SelectListItem> AccBranchList { get; set; }


        #endregion

        #region Fixed Deposit Details

        [Required]
        [DisplayName("FDR Amount")]
        public decimal? InitialDeposit { get; set; }

        [Required]
        [DisplayName("FDR Amount(Present Value)")]
        public decimal? FDRAmount { get; set; }

        [DisplayName("Cheque No.")]
        public int? ChequeId { get; set; }
        public IList<SelectListItem> ChequeList { get; set; }

        [Required]
        [DisplayName("FDR Duration")]
        public int? FDRDuration { get; set; }

        [Required]
        [DisplayName("FDR Duration")]
        public string FDRDurationType { get; set; }
        public IList<SelectListItem> FDRDurationTypeList { get; set; }

        public int FDRDurationInMonth { get; set; }

        [Required]
        [DisplayName("Interest Rate")]
        public decimal? InterestRate { get; set; }


        [Required]
        [DisplayName("Interest Amount (Per Int.)")]
        [UIHint("_ReadOnlyAmount")]
        public decimal? InterestAmount { get; set; }

        [Required]
        [DisplayName("Interest Receive Duration")]
        public int? InstallmentDuration { get; set; }

        public string InstallmentDurationType { get; set; }
        public IList<SelectListItem> InstallmentDurationTypeList { get; set; }

        [DisplayName("TAX Rate (On Interest)")]
        public decimal? TAXRate { get; set; }

        [DisplayName("TAX On Interest")]
        [UIHint("_ReadOnlyAmount")]
        public decimal? TAXAmount { get; set; }

        [DisplayName("Excise Duty Others (Per Int.)")]
        public decimal? BankCharge { get; set; }

        [DisplayName("Total Excise Duty Others")]
        //[UIHint("_ReadOnlyAmount")]
        public decimal? TotalBankCharge { get; set; }

        [Required]
        [DisplayName("First Interest Rec. Date")]
        [UIHint("_RequiredDate")]
        public DateTime? StartDate { get; set; }

        [Required]
        [DisplayName("Maturity Date")]
        [UIHint("_RequiredDate")]
        public DateTime? MaturityDate { get; set; }

        public bool IsClose { get; set; }
        #endregion

        #region TotalCalculation
        [Required]
        [DisplayName("Total Receivable")]
        [UIHint("_ReadOnlyAmount")]
        public decimal? TotalReceivableAmount { get; set; }
      
        [Required]
        public decimal? TotalInterestAmount { get; set; }

        [DisplayName("Total TAX On Interest")]
        [UIHint("_ReadOnlyAmount")]
        public decimal? TotalTAXAmount { get; set; }

        [DisplayName("Total Excise Duty Others")]
        [UIHint("_ReadOnlyAmount")]
        public decimal? tempTotalBankCharge { get; set; }

        [Required]
        [DisplayName("Total Profit")]
        [UIHint("_ReadOnlyAmount")]
        public decimal? TotalProfit { get; set; }

        [Required]
        [DisplayName("Net Receivable")]
        [UIHint("_ReadOnlyAmount")]
        public decimal? NetReceivableAmount { get; set; }

        public int? FDRCloseingId { get; set; }
        #endregion

        #region Other
        public List<FixedDepositInfoInstallmentScheduleViewModel> InstallmentSchedulList { get; set; }

        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string ShowRecord { get; set; }

        public DateTime? FDRDateFrom { get; set; }
        public DateTime? FDRDateTo { get; set; }
        public DateTime? MaturityDateFrom { get; set; }
        public DateTime? MaturityDateTo { get; set; }

        public decimal? InterestRateFrom { get; set; }
        public decimal? InterestRateTo { get; set; }

        [UIHint("_ReadOnly")]
        [DisplayName("Fund Type")]
        public string FDRTypeName { get; set; }


        [UIHint("_ReadOnly")]
        [DisplayName("Bank Name")]
        public string AccBankName { get; set; }

        [UIHint("_ReadOnly")]
        [DisplayName("Branch Name")]
        public string AccBranchName { get; set; }

        [UIHint("_ReadOnly")]
        [DisplayName("Address")]
        public string AccBankBranchAddress { get; set; }

        #endregion
    }
}