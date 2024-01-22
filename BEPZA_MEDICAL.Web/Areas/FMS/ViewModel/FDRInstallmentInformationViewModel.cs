using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.ViewModel
{
    public class FDRInstallmentInformationViewModel:BaseViewModel
    {
        #region Ctor
        public FDRInstallmentInformationViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
            this.ProfitRecvList = new List<SelectListItem>();
            this.ChequeList = new List<SelectListItem>();
            this.FDRNoList = new List<SelectListItem>();
            this.MonthList = new List<SelectListItem>();
            this.YearList = new List<SelectListItem>();
            this.BankInfoList = new List<SelectListItem>();
            this.BankInfoBranchDetailList = new List<SelectListItem>();
            this.InstallmentList = new List<FDRInstallmentInformationViewModel>();
        }

        #endregion

        #region Standard Properties

        [Required]
        [DisplayName("Bank")]
        public int BankInfoId { get; set; }
        public IList<SelectListItem> BankInfoList { get; set; }

        [Required]
        [DisplayName("Branch")]
        public int BankInfoBranchDetailId { get; set; }
        public IList<SelectListItem> BankInfoBranchDetailList { get; set; }


        [DisplayName("Date")]
        [UIHint("_Date")]
        public DateTime Date { get; set; }

        [DisplayName("FDR No.")]
        public int FixedDepositInfoId { get; set; }
        public IList<SelectListItem> FDRNoList { get; set; }

        [DisplayName("FDR Amount")]
        public decimal FDRAmount { get; set; }

        [DisplayName("Interest Rate")]
        public decimal InterestRate { get; set; }

        [DisplayName("Ledger head (Profit/Income) of Accounts")]
        public int? ProfitRecvId { get; set; }
        public IList<SelectListItem> ProfitRecvList { get; set; }

        [DisplayName("Interest Amount")]
        public decimal InstallmentAmount { get; set; }

        [DisplayName("TAX Amount")]
        public decimal TAXAmount { get; set; }


        [DisplayName("Bank Charge")]
        public decimal BankCharge { get; set; }

        [DisplayName("Net Profit")]
        public decimal Profit { get; set; }

        [DisplayName("Cheque No.")]
        public int? ChequeId { get; set; }
        public IList<SelectListItem> ChequeList { get; set; }

        public int ZoneInfoId { get; set; }

        public string Description { get; set; }

        [DisplayName("Interest Receive Amount")]
        public decimal InterestReceiveAmount { get; set; }

        #endregion

        #region Others
        [DisplayName("Month")]
        public string Month { get; set; }
        public IList<SelectListItem> MonthList { get; set; }

        [DisplayName("Year")]
        public string Year { get; set; }
        public IList<SelectListItem> YearList { get; set; }

        [DisplayName("Installment Date Between")]
        [UIHint("_Date")]
        public DateTime? DateFrom { get; set; }

        [DisplayName("To")]
        [UIHint("_Date")]
        public DateTime? DateTo { get; set; }

        [DisplayName("Ratio")]
        public int? Ratio { get; set;}
        public string FDRNo { get; set; }
        public List<FDRInstallmentInformationViewModel> InstallmentList { get; set; }
        public bool IsCheckedFinal { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        #endregion

        #region Attachment
        public byte[] Attachment { set; get; }

        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        #endregion
    }
}