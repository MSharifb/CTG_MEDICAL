using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel
{
    public class ItemInfoViewModel : BaseViewModel
    {
        public ItemInfoViewModel()
        {
            CategoryList = new List<SelectListItem>();
            ColorList = new List<SelectListItem>();
            ManufacturerList = new List<SelectListItem>();
            ItemTypeList = new List<SelectListItem>();
            ModelList = new List<SelectListItem>();
            UnitList = new List<SelectListItem>();
            AssetGroupList = new List<SelectListItem>();

            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        [Required]
        public int Id { get; set; }

        [DisplayName("Item")]
        [MaxLength(100)]
        [Required]
        public string ItemName { get; set; }

        [DisplayName("Code")]
        [MaxLength(50)]
        public string Code { get; set; }

        [DisplayName("Description")]
        [MaxLength(100)]
        public string Description { get; set; }

        [DisplayName("Category")]
        [Required]
        public int? CategoryId { set; get; }

        [DisplayName("Color")]
        public int? ColorId { set; get; }

        [DisplayName("Manufacturer")]
        //[Required]
        public int? ManufacturerId { set; get; }

        [DisplayName("Minimum Quantity")]
        public int? MinimumQuantity { set; get; }

        [DisplayName("Maximum Quantity")]
        public int? MaximumQuantity { set; get; }

        [DisplayName("ReOrder Quantity")]
        public int? ReOrderQuantity { set; get; }

        [DisplayName("Periodic")]
        public bool IsPeriodicAsset { set; get; }

        [DisplayName("Condemnable")]
        public bool IsCondemnableAsset { set; get; }

        [DisplayName("Has Quota")]
        public bool HasQuota { set; get; }

        [DisplayName("Type")]
        public int? TypeId { set; get; }

        [DisplayName("Model")]
        //[Required]
        public int? ModelId { set; get; }

        [DisplayName("Unit")]
        public int? UnitId { set; get; }

        [DisplayName("Size")]
        [MaxLength(50)]
        public string Size { get; set; }

        [DisplayName("Remarks")]
        [MaxLength(200)]
        public string Remarks { get; set; }

        [Required]
        public int AssetGroupId { set; get; }

        // Others

        public IList<SelectListItem> CategoryList { set; get; }
        public IList<SelectListItem> ColorList { set; get; }
        public IList<SelectListItem> ManufacturerList { set; get; }
        public IList<SelectListItem> ItemTypeList { set; get; }
        public IList<SelectListItem> ModelList { set; get; }
        public IList<SelectListItem> UnitList { set; get; }
        public IList<SelectListItem> AssetGroupList { set; get; }

        //
    }
}