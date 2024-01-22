using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel
{
    public class PeriodicAssetDurationViewModel : BaseViewModel
    {
        public PeriodicAssetDurationViewModel()
        {
            this.ItemInfoList = new List<SelectListItem>();
            this.DesignationList = new List<SelectListItem>();
            this.DurationScaleList = new List<SelectListItem>();
        }

        [DisplayName("Asset Name")]
        [Required]
        public int ItemInfoId { get; set; }
        [DisplayName("Asset_Name")]
        public string ItemInfoName { get; set; }
        [DisplayName("Employee Type")]
        [Required]
        public int DesignationId { get; set; }
        [DisplayName("Employee Type Name")]
        public string DesignationName { get; set; }
        [DisplayName("Re-issue After")]
        public int ReIssueAfter { get; set; }
        public string DurationScale { get; set; }
        public string Remarks { get; set; }
        public IList<SelectListItem> ItemInfoList { get; set; }
        public IList<SelectListItem> DesignationList { get; set; }
        public IList<SelectListItem> DurationScaleList { get; set; }
    }
}