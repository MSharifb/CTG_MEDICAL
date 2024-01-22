using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmployeeWealthStatementViewModel : BaseViewModel
    {
        #region Ctor
        public EmployeeWealthStatementViewModel()
        {
            this.AssetTypeList = new List<SelectListItem>();

            this.AssetGainDate = DateTime.Now;
            this.EmpTop = new EmpTop();

        }
        #endregion

        #region Standard Property
        [Required]
        public int Id { get; set; }

        public int EmployeeId { set; get; }

        [DisplayName("Asset Type")]
        [Required]
        public int AssetTypeId { set; get; }

        [Required]
        [DisplayName("Name of the Asset")]
        public string AssetName { get; set; }

        [DisplayName("Description of the Asset")]
        [UIHint("_MultiLine")]
        public string AssetDescription { get; set; }

        [DisplayName("Asset Quantity")]
        public int? AssetQuantity { get; set; }

        [DisplayName("Date of Asset Gain")]
        [Required]
        [UIHint("_Date")]
        public DateTime? AssetGainDate { set; get; }

        [Required]
        [DisplayName("Who Gain this Asset")]
        public string AssetGainer { get; set; }

        [DisplayName("Type and Location of the Asset")]
        [UIHint("_MultiLine")]
        public string AssetLocation { get; set; }

        [DisplayName("How to achieve and value of the Asset during achievement date")]
        [UIHint("_MultiLine")]
        public string AssentValue { get; set; }

        [DisplayName("If purchase, Income source")]
        [UIHint("_MultiLine")]
        public string AssetPurchase { get; set; }

        [UIHint("_MultiLine")]
        public string Remarks { set; get; }

        #endregion


        #region Others
        public IList<SelectListItem> AssetTypeList { set; get; }
        public EmpTop EmpTop { get; set; }

        #endregion


    }
}