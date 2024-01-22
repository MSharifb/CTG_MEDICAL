using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ERP_BEPZA.Web.Areas.AMS.Models.HolidayWorkingDay
{
    public class HolidayWorkingDaySearchModel
    {
         #region Properties

        public int? Id { get; set; }
        public string DeclarationDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        #endregion

          #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            if (Id.HasValue)
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", Id));

            if (!String.IsNullOrWhiteSpace(DeclarationDate))
                filterExpressionBuilder.Append(String.Format("DeclarationDate= {0} AND ", DeclarationDate));

            if (!String.IsNullOrWhiteSpace(FromDate))
                filterExpressionBuilder.Append(String.Format("FromDate= {0} AND ", FromDate));

            if (!String.IsNullOrWhiteSpace(ToDate))
                filterExpressionBuilder.Append(String.Format("ToDate= {0} AND ", ToDate));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
          #endregion
    }
}