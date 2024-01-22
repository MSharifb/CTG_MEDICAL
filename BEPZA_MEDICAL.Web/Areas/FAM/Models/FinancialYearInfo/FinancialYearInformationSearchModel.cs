using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.FinancialYearInfo
{
    public class FinancialYearInformationSearchModel
    {
        #region Property
        public int? Id { get; set; }
        public string FinancialYearName { get; set; }
        public DateTime FinancialYearStartDate { get; set; }
        #endregion


        #region Method
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (Id.HasValue)
                filterExpressionBuilder.Append(String.Format("Id={0} AND", Id));
            if(!String.IsNullOrWhiteSpace(FinancialYearName))
                filterExpressionBuilder.Append(String.Format("FinancialYearName like {0} AND", FinancialYearName));
            //if (FinancialYearStartDate != null && !FinancialYearStartDate.Date.ToString().Contains("01/01/0001"))
            //{
            //    string ActualRateFromP = "EffectiveDate >=" + String.Format("{0:dd/MM/yyyy}", FinancialYearStartDate) + " AND ";
            //    filterExpressionBuilder.Append(ActualRateFromP);
            //}

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 4, 4);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}