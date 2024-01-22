using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.ViewModel
{
    public class CPFInterestReceivableViewModel : BaseViewModel
    {
        #region Ctor
        public CPFInterestReceivableViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.YearList = new List<SelectListItem>();
            this.FDRNoList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Properties
        [Required]
        [DisplayName("FDR No.")]
        public int FixedDepositInfoId { get; set; }
        public IList<SelectListItem> FDRNoList { get; set; }

        [DisplayName("Up to Year")]
        public int FinancialYearId { get; set; }
        public IList<SelectListItem> YearList { get; set; }

        [DisplayName("Interest Already Distributed")]
        public decimal? DistributedAmount { get; set; }

        [DisplayName("Applicable Month")]
        public decimal ApplicableMonth { get; set; }

        [DisplayName("Other Excise Duty")]
        public decimal OtherExciseDuty { get; set; }

        [DisplayName("Approx Interest During the Year")]
        public decimal ApproxInterestDuring { get; set; }
        #endregion

        #region Others
        public string FDRNo { get; set; }
        public DateTime FDRDate { get; set; }
        public DateTime MaturityDate { get; set; }
        #endregion

    }
}