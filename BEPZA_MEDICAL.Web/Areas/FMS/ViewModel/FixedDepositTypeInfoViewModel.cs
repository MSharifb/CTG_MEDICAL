using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.ViewModel
{
    public class FixedDepositTypeInfoViewModel:BaseViewModel
    {
        #region Ctor
        public FixedDepositTypeInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
            this.DurationTypeList = new List<SelectListItem>();
            this.InstallmentInList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Properties

        [Required]
        [DisplayName("Fixed Deposit Type")]

        public string FixedDepositType { get; set; }

        [Required]
        [DisplayName("Duration")]
        public int Duration { get; set; }

        public string DurationType { get; set; }
        [Required]
        [DisplayName("Interest Rate")]
        public decimal InterestRate { get; set; }
        [Required]
        [DisplayName("Interest In")]
        public int InstallmentIn { get; set; }

        public string InstallmentType { get; set; }

        [DisplayName("Tax %(On Interest)")]
        public decimal? Tax { get; set; }
        [DisplayName("OR")]
        public decimal? BankChargeFix { get; set; }
        [DisplayName("Excise Duty Others")]
        public decimal? BankChargePerc { get; set; }
        public int ZoneInfoId { get; set; }

        public string Remarks { get; set; }
        #endregion

        #region Others

        public IList<SelectListItem> DurationTypeList { get; set; }
        public IList<SelectListItem> InstallmentInList { get; set; }
        #endregion

    }
}