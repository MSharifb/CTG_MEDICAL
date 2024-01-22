using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ACRAttributesInformationViewModel : BaseViewModel
    {
        #region Ctor
        public ACRAttributesInformationViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";

            this.StaffCategoryList = new List<SelectListItem>();
            this.ACRCriteriaInfoList = new List<SelectListItem>();

            this.ACRAttributesInfoDetailList = new List<ACRAttributesInformationDetailViewModel>();
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
        public int ACRCriteriaInfoId { get; set; }
        public string ACRCriteriaName { get; set; }
        public IList<SelectListItem> ACRCriteriaInfoList { get; set; }

        #endregion

        #region Other

        [Display(Name = "Attribute Name")]
        public string AttributeName { get; set; }
        [Display(Name = "Full Mark")]
        public decimal? FullMark { get; set; }
        [Display(Name = "Serial Number")]
        public decimal? SerialNumber { get; set; }

        public IList<ACRAttributesInformationDetailViewModel> ACRAttributesInfoDetailList { get; set; }

        #endregion


    }
}