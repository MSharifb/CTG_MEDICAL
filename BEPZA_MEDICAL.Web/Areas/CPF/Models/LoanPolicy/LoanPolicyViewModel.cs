using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.LoanPolicy
{
    public class LoanPolicyViewModel : BaseViewModel
    {
        public LoanPolicyViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.LoanPolicyForList = new List<SelectListItem>();
            this.NRfApplicableForList = new List<SelectListItem>();
            this.RfApplicableForList = new List<SelectListItem>();
            this.RfBalanceDeductionConfirmationList = new List<SelectListItem>();
            this.RfInstallmentBalanceAdditionConformationList = new List<SelectListItem>();
        }

        #region Sandard Prop

        [DisplayName("Loan Policy For")]
        [Required]
        public string LoanPolicyFor { get; set; }

        [DisplayName("Effective From")]
        [Required]
        [UIHint("_Date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("To")]
        [Required]
        [UIHint("_Date")]
        public DateTime? EndDate { get; set; }

        [DisplayName("% Allowed")]
        public decimal? NRfLoanPercentage { get; set; }

        [DisplayName("On")]
        public string NRfApplicableFor { get; set; }

        [DisplayName("Age >=")]
        public int? NRfMinimumAge { get; set; }

        [DisplayName("% Allowed")]
        [Required]
        public decimal? RfLoanPercentage { get; set; }

        [DisplayName("On")]
        [Required]
        public string RfApplicableFor { get; set; }

        [DisplayName("Will be deduct from balance")]
        [Required]
        public bool IsRfDeductFromBalance { get; set; }

        [DisplayName("Installment will be added balance")]
        [Required]
        public bool IsRfInstallmentAddedWithBalance { get; set; }

        [DisplayName("Maximum No. of Concurrent Loan")]
        [Required]
        public int? RfMaxConcurrentLoan { get; set; }

        [DisplayName("Service Duration Required (Minimum)")]
        [Required]
        public int? RfMinServiceYear { get; set; }

        #endregion

        #region Other Prop

        public IList<SelectListItem> LoanPolicyForList { get; set; }

        public IList<SelectListItem> NRfApplicableForList { get; set; }

        public IList<SelectListItem> RfApplicableForList { get; set; }

        public IList<SelectListItem> RfBalanceDeductionConfirmationList { get; set; }

        public IList<SelectListItem> RfInstallmentBalanceAdditionConformationList { get; set; }

        #endregion
    }
}