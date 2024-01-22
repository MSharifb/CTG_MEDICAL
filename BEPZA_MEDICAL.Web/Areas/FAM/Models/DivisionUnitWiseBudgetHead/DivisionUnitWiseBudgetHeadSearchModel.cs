using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.DivisionUnitWiseBudgetHead
{
    public class DivisionUnitWiseBudgetHeadSearchModel
    {
        #region Property
        public int? Id { get; set; }
        public int? DivisionUnitId { get; set; }
        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            if (Id.HasValue)
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", Id));

            if (DivisionUnitId.HasValue && DivisionUnitId > 0)
                filterExpressionBuilder.Append(String.Format("DivisionUnitId = {0} AND ", DivisionUnitId));


            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}