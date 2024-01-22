using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class DegreeLevelMappingViewModel:BaseViewModel
    {
        #region Ctor
        public DegreeLevelMappingViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.DegreeTypeList = new List<SelectListItem>();
            this.AvailableDegreeLevelList = new List<SelectListItem>();
            this.DegreeLevelList = new List<SelectListItem>();
            this.DegreeLevelMappingDetail = new List<DegreeLevelMappingDetailViewModel>();
        }
        #endregion

        #region Standard Property

        [Required]
        [Display(Name="Degree Type")]
        public int DegreeTypeId { get; set; }
        [Required]
        public int ZoneInfoId { get; set; }

        #endregion

        #region Other
        public int? AvailableDegreeLevelId { get; set; }
        public int? DegreeLevelId { get; set; }

        public IList<SelectListItem> DegreeTypeList { get; set; }

        public IList<SelectListItem> AvailableDegreeLevelList { get; set; }

        public IList<SelectListItem> DegreeLevelList { get; set; }

        public IList<DegreeLevelMappingDetailViewModel> DegreeLevelMappingDetail { get; set; }

        #endregion

    }
}