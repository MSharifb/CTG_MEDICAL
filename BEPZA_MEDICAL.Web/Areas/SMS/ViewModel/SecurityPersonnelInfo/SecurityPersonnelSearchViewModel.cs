using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BEPZA_MEDICAL.Web.Areas.SMS.ViewModel
{
    public class SecurityPersonnelSearchViewModel
    {
        public SecurityPersonnelSearchViewModel()
        {

        }

        #region Properties
        public int? ID { get; set; }
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public int? DesignationId { get; set; }
       
        public int SelectedEmployeeStatus { get; set; }
        public string ActionName { get; set; }
        public string Message { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ZoneInfoId { get; set; }

        public int? OrganizationId { get; set; }

        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (ID.HasValue)
                filterExpressionBuilder.Append(String.Format("Id = {0} AND ", ID));

            if (!String.IsNullOrWhiteSpace(EmpID))
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.EmpID like {0} AND ", EmpID));

            if (!String.IsNullOrWhiteSpace(FullName))
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.FullName like {0} AND ", FullName));

            if (DesignationId.HasValue && DesignationId > 0)
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.DesignationId = {0} AND ", DesignationId));

            
            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion




    }
}