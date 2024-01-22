using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class AssetConditionViewModel : BaseViewModel
    {
        #region Ctor

        public AssetConditionViewModel()
        {
            this.AssetStatusList = new List<SelectListItem>();
        }
        #endregion
       
        [Required]
        [DisplayName("Asset Condition")]
        [StringLength(100, ErrorMessage = "Asset condition must be within 100 characters.")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Asset Status")]
        public int AssetStatusId { get; set; }

        [Required]
        [DisplayName("Sort Order")]
        public int? SortOrder { get; set; }

        [DisplayName("Remarks")]
        [StringLength(100, ErrorMessage = "Remarks must be within 100 characters.")]
        public string Remarks { get; set; }


        #region Other Properties
        public IList<SelectListItem> AssetStatusList { get; set; }
        public string AssetStatusName { get; set; }

        #endregion
    }
}