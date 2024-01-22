using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.PfInterestRate
{
    public class PFInterestRateModel : BaseViewModel
    {
        public PFInterestRateModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;

            YearList = new List<SelectListItem>();
            MonthList = new List<SelectListItem>();
        }

        #region Standard Property

        [DisplayName("Calculation Duration")]
        [Required]
        public string PfPeriodDuration { get; set; }

        [DisplayName("Month")]
        public string Month { get; set; }

        [DisplayName("Year")]
        public string Year { get; set; }

        public IList<SelectListItem> MonthList { get; set; }
        public IList<SelectListItem> YearList { set; get; }

        [DisplayName("Interest Rate")]
        [Required]
        public decimal? InterestRate { get; set; }

        #endregion

        #region Others

        public string Period { get; set; }

        #endregion
    }
}