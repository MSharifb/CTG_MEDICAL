using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.VendorInfo
{
    public class VendorInformationSearchModel
    {
        #region Properties
        public int? Id { get; set; }
        public string VendorName { get; set; }
        #endregion

        #region Method
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            if (Id.HasValue)
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", Id));
            if (!String.IsNullOrWhiteSpace(VendorName))
                filterExpressionBuilder.Append(String.Format("VendorName like {0} AND ", VendorName));
                       
            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}