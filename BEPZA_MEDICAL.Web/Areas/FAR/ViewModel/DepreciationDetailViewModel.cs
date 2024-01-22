using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class DepreciationDetailViewModel : BaseViewModel
    {
        public DepreciationDetailViewModel()
        {
            this.strMode = "Create";
        }

        public int DepreciationId { get; set; }
        public int FixedAssetId { get; set; }

        public int AssetStatusId { get; set; }
        public int AssetConditionId { get; set; }
        public int CategoryId { get; set; }

        public Decimal DepreciationRate { get; set; }
        public Decimal AssetCost { get; set; }

        public Decimal OBBookValue { get; set; }
        public Decimal OBDepreciation { get; set; }
        public Decimal Depreciation { get; set; }

        public Decimal CBDepreciation { get; set; }
        public Decimal CBBookValue { get; set; }


        [DisplayName("financial Year")]
        //[Required]
        public int FinancialYearId { get; set; }

        [DisplayName("financial Year")]
        public string YearName { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        [DisplayName("Asset Status")]
        public string AssetStatusName { get; set; }

        [DisplayName("Asset Condition")]
        public string AssetCondition { get; set; }

        [DisplayName("Category Code")]
        public string AssetCategoryCode { get; set; }

        [DisplayName("Category Name")]
        public string AssetCategory { get; set; }

        [DisplayName("Asset Name")]
        public string AssetName { get; set; }

        [DisplayName("Asset Code")]
        public string AssetCode { get; set; }

        #region Other properties

        #endregion
    }
}