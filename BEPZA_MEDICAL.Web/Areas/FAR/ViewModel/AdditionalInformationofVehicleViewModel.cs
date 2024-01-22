using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class AdditionalInformationofVehicleViewModel : BaseViewModel
    {
        #region Ctor
        public AdditionalInformationofVehicleViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
            this.SourceOfFundList = new List<SelectListItem>();
            this.AssetCodeList = new List<SelectListItem>();
            this.SparePartList = new List<SelectListItem>();
            this.AdditionalInfoVehicleDetailList = new List<AdditionalInformationofVehicleDetailViewModel>();
        }
        #endregion

        #region Standard Properties
        [Required]
        [DisplayName("Asset Code")]
        public int FIxedAssetId { get; set; }
        [UIHint("_Date")]
        public DateTime? Date { get; set; }

        [DisplayName("Source of Fund")]
        public int? SourceOfFundId { get; set; }
        public string Registration { get; set; }
        public string Chassis { get; set; }
        public string Engine { get; set; }
        public string CC { get; set; }
        public int? Seats { get; set; }
        public int ZoneInfoId { get; set; }
        public string Remarks { get; set; }
        #endregion

        #region Details
        [DisplayName("Spare Part")]
        public int? SparePartId { get; set; }
        public IList<SelectListItem> SparePartList { get; set; }
        public int? Quantity { get; set; }
        public IList<AdditionalInformationofVehicleDetailViewModel> AdditionalInfoVehicleDetailList { get; set; }
        #endregion

        #region Other
        public IList<SelectListItem> SourceOfFundList { get; set; }
        public IList<SelectListItem> AssetCodeList { get; set; }
        public string MaintenanceType { get; set; }
        public string AssetCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        #endregion

    }
}