using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ERP_BEPZA.Web.Areas.AMS.Models.OverTimeRule
{
    public class OverTimeRuleSearchModel
    {
        #region Properties

        public int? Id { get; set; }
        public int? RuleName { get; set; }

        #endregion

          #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            if (Id.HasValue)
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", Id));
            if (RuleName.HasValue && RuleName > 0)
                filterExpressionBuilder.Append(String.Format("Id= {0} AND ", RuleName));
            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
          #endregion
    }
}