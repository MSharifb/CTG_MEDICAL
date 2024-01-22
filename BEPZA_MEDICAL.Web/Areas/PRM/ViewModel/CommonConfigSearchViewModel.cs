using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class CommonConfigSearchViewModel
    {
        #region Properties

        public string ID { get; set; }

        public string Name { get; set; }

        public int SortOrder { get; set; }

        #endregion

        #region Methods

        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(Name))
                filterExpressionBuilder.Append(String.Format("Name like {0} AND ", Name));
            if (!String.IsNullOrWhiteSpace(ID))
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", ID));
            if (SortOrder > 0)
                filterExpressionBuilder.Append(String.Format("Rank = {0} AND ", SortOrder));


            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }

        #endregion
    }
}