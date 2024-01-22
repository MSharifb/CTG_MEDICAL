using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel
{
    public class AssetQuotaInfoViewModel : BaseViewModel
    {
        public AssetQuotaInfoViewModel()
        {
            this.ItemInfoList = new List<SelectListItem>();
            this.ZoneList = new List<SelectListItem>();
            this.UnitList = new List<SelectListItem>();
        }
        
        [DisplayName("Asset Name")]
        [Required]
        public int ItemId { get; set; }
        public string ItemInfoName { get; set; }
        [DisplayName("Zone")]
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public int Quota { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string Remarks { get; set; }
        public IList<SelectListItem> ItemInfoList { get; set; }
        public IList<SelectListItem> ZoneList { get; set; }
        public IList<SelectListItem> UnitList { get; set; }
    }
}