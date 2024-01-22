using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class DistrictQuotaViewModel : BaseViewModel
    {
        #region Ctor
        public DistrictQuotaViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.DivisionNameList = new List<SelectListItem>();
            this.DistrictNameList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property
        [Required]
        [Display(Name="Division Name")]
        public int DivisionId { get; set; }
        [Required]
        [Display(Name = "District Name")]
        public int DistrictId { get; set; }

        public decimal? Population { get; set; }
        public decimal? Percentage { get; set; }
        #endregion

        #region Other
        public string DivisionName { get; set; }
        public IList<SelectListItem> DivisionNameList { get; set; }
        public string DistrictName { get; set; }
        public IList<SelectListItem> DistrictNameList { get; set; }
        #endregion
    }
}