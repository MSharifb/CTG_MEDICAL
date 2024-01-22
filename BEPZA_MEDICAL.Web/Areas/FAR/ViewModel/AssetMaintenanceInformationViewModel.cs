using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class AssetMaintenanceInformationViewModel:BaseViewModel
    {
        #region Ctor
        public AssetMaintenanceInformationViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
            this.MaintenanceTypeList = new List<SelectListItem>();
            this.AssetCodeList = new List<SelectListItem>();
            this.SparePartList = new List<SelectListItem>();
            this.AssetMaintenanceInformationDetailList = new List<AssetMaintenanceInformationDetailViewModel>();
        }
        #endregion

        #region Standard Properties
        [Required]
        [DisplayName("Asset Code")]
        public int FIxedAssetId { get; set; }

        [Required]
        [UIHint("_Date")]
        public DateTime? Date { get; set; }

        [DisplayName("Maintenance Type")]
        public int MaintenanceTypeInfoId { get; set; }

        public decimal? Amount { get; set; }
        public int ZoneInfoId { get; set; }
        public string Remarks { get; set; }
        #endregion

        #region Details
        [DisplayName("Spare Part")]
        public int? SparePartId { get; set; }
        public IList<SelectListItem> SparePartList { get; set; }

        public int? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public IList<AssetMaintenanceInformationDetailViewModel> AssetMaintenanceInformationDetailList { get; set; }
        #endregion

        #region Attachment
        public HttpPostedFileBase File { get; set; }

        [DisplayName("Attachment")]
        public byte[] Attachment { get; set; }

        public string FileName { get; set; }

        #endregion

        #region Other
        public IList<SelectListItem> MaintenanceTypeList { get; set; }
        public IList<SelectListItem> AssetCodeList { get; set; }
        public string MaintenanceType { get; set; }
        public string AssetCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        #endregion

    }
}