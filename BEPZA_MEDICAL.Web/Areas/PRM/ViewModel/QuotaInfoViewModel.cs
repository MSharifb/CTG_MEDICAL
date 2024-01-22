using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class QuotaInfoViewModel : BaseViewModel
    {
        #region Ctor
        public QuotaInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.QuotaNameList = new List<SelectListItem>();
            this.QuotaInfoList = new List<QuotaInfoViewModel>();
        }
        #endregion

        #region Standard Property
        [Required]
        [Display(Name = "Quota Name")]
        public int QuotaNameId { get; set; }

        public string QuotaName { get; set; }
        public IList<SelectListItem> QuotaNameList { get; set; }
        [Display(Name="First and Second Class Officer's Post")]
        public decimal? FirstAndSecondClsOfficer { get; set; }
        [Display(Name = "Third and Forth Class Staff's Post")]
        public decimal? ThirdAndForthClsStaff { get; set; }

        #endregion

        #region Other
        public IList<QuotaInfoViewModel> QuotaInfoList { get; set; }
        #endregion
    }
}