using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class SalaryScaleSearchViewModel
    {
        #region Properties
        public int? ID { get; set; }
        public string SalaryScaleName { get; set; }
        public DateTime? DateOfCirculation { get; set; }
        public DateTime? DateOfEffective { get; set; }


        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(SalaryScaleName))
                filterExpressionBuilder.Append(String.Format("SalaryScaleName LIKE {0} AND ", SalaryScaleName));

            if (DateOfCirculation != null)
                filterExpressionBuilder.Append(String.Format("DateOfCirculation = {0} AND ", DateOfCirculation));

            if (DateOfEffective != null)
                filterExpressionBuilder.Append(String.Format("DateOfEffective = {0} AND ", DateOfEffective));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);

            return filterExpressionBuilder.ToString();
        }

        #endregion
    }
}

