using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ACRCriteriaInformationViewModel: BaseViewModel
    {
        #region Ctor
        public ACRCriteriaInformationViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";

            this.StaffCategoryList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property

        [Required]
        [Display(Name = "ACR Criteria For")]
        public int StaffCategoryId { get; set; }
        public string StaffCategory { get; set; }
        public IList<SelectListItem> StaffCategoryList { get; set; }

        [Required]
        [Display(Name = "ACR Criteria Name")]
        public string ACRCriteriaName { get; set; }
        [Display(Name="Full Mark")]
        public decimal? FullMark { get; set; }
        [Display(Name="Sort Order")]
        public int? SortOrder { get; set; }
        #endregion

        #region Other
        #endregion
    }
}