using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.GratuityInterestRate
{ 
    public class GratuityInterestRateModel : BaseViewModel
    {
        public GratuityInterestRateModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;

            PeriodList = new List<SelectListItem>();
        }

        #region Standard Property

        [DisplayName("Period")]
        [Required]
        public int? PeriodId { get; set; }
        public IList<SelectListItem> PeriodList { set; get; }        

        [DisplayName("Interest Rate")]
        [Required]
        public decimal? InterestRate { get; set; }

        #endregion

        #region Others

        public string Period { get; set; }

        #endregion
    }
}