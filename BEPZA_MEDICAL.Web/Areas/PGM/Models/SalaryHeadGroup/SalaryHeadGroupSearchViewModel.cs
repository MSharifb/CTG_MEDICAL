using System;
using System.Text;

namespace ERP_BEPZA.Web.Areas.PGM.Models.SalaryHeadGroup
{
    public class SalaryHeadGroupSearchViewModel
    {
        #region Properties

        public int? Id { get; set; }
        public string Name { get; set; }
        public string HeadType { get; set; }

        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            if (Id.HasValue)
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", Id));
            if (!String.IsNullOrWhiteSpace(Name))
                filterExpressionBuilder.Append(String.Format("Name like {0} AND ", Name));
            if (!String.IsNullOrWhiteSpace(HeadType))
                filterExpressionBuilder.Append(String.Format("HeadType like {0} AND ", HeadType));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}