using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmployeeActivationSearchViewModel
    {
        #region Properties        
        public string EmpId { get; set; }
        public int? DivisionId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeInitial { get; set; }           

        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            if (DivisionId.HasValue && DivisionId > 0)
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.DivisionId = {0} AND ", DivisionId));

            if (!String.IsNullOrWhiteSpace(EmpId))
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.EmpID like {0} AND ", EmpId));

            if (!String.IsNullOrWhiteSpace(EmployeeName))
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.FullName like {0} AND ", EmployeeName));

            if (!String.IsNullOrWhiteSpace(EmployeeInitial))
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.EmployeeInitial like {0} AND ", EmployeeInitial));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}

