using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ResourceCategorySearchViewModel
    {
        #region Properties
        public int? ID { get; set; }
        public string ResourceCategory { get; set; }
        public decimal? ActualRateFrom { get; set; }
        public decimal? ActualRateTo { get; set; }
        public decimal? BudgetRateFrom { get; set; }
        public decimal? BudgetRateTo { get; set; }
        public int? ResourceTypeId { get; set; }
        public int? UOMId { get; set; }


        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            if (ID.HasValue)
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", ID));
            if (!String.IsNullOrWhiteSpace(ResourceCategory))
                filterExpressionBuilder.Append(String.Format("ResourceCategory like {0} AND ", ResourceCategory));
            if (ResourceTypeId.HasValue && ResourceTypeId > 0)
                filterExpressionBuilder.Append(String.Format("ResourceTypeId = {0} AND ", ResourceTypeId));
            if (ActualRateFrom.HasValue && ActualRateFrom > 0)
            {
                string ActualRateFromP = "ActualRate >=" + String.Format("{0:0.00}", ActualRateFrom) + " AND ";
                filterExpressionBuilder.Append(ActualRateFromP);
            }
            if (ActualRateTo.HasValue && ActualRateTo > 0)
            {
                string ActualRateToP = "ActualRate <=" + String.Format("{0:0.00}", ActualRateTo) + " AND ";
                filterExpressionBuilder.Append(ActualRateToP);
            }
            if (BudgetRateFrom.HasValue && BudgetRateFrom > 0)
            {
                string BudgetRateFromP = "BudgetRate >=" + String.Format("{0:0.00}", BudgetRateFrom) + " AND ";
                filterExpressionBuilder.Append(BudgetRateFromP);
            }
            if (BudgetRateTo.HasValue && BudgetRateTo > 0)
            {
                string BudgetRateToP = "BudgetRate <=" + String.Format("{0:0.00}", BudgetRateTo) + " AND ";
                filterExpressionBuilder.Append(BudgetRateToP);
            }
            if (UOMId.HasValue && UOMId > 0)
                filterExpressionBuilder.Append(String.Format("UOMId = {0} AND ", UOMId));
            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}

