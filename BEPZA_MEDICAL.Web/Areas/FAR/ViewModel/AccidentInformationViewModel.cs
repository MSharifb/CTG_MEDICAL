using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class AccidentInformationViewModel:BaseViewModel
    {
        #region Ctor
        public AccidentInformationViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
            this.LocationList = new List<SelectListItem>();
            this.AssetCodeList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Properties
        [Required]
        [DisplayName("Asset Code")]
        public int FIxedAssetId { get; set; }
        [Required]
        [UIHint("_Date")]
        public DateTime? Date { get; set; }
        [DisplayName("Where")]
        public int? LocationId { get; set; }
        public string Driver { get; set; }
        public int ZoneInfoId { get; set; }
        public string Description { get; set; }
        #endregion

        #region Other
        public IList<SelectListItem> LocationList { get; set; }
        public IList<SelectListItem> AssetCodeList { get; set; }

        public string AssetCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        #endregion
    }
}