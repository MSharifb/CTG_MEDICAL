using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.OpeningBalance
{
    public class MembershipSearchViewModel
    {

        #region Properties
        public int? ID { get; set; }
        public string EmpId { get; set; }

        public string EmpInitial { get; set; }
        public string EmpName { get; set; }
        public int? DesigName { get; set; }
        public int? EmpTypeId { get; set; }

        public int? DivisionName { get; set; }
        public int? JobLocName { get; set; }
        public int? GradeName { get; set; }
        public int? StaffCategoryId { get; set; }
        public int? ResourceLevelId { get; set; }

        public int EmployeeStatus { get; set; }
        public int SelectedEmployeeStatus { get; set; }

        public string ActionName { get; set; }
        public string Message { get; set; }
        public string SearchEmpType { get; set; }

        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (EmployeeStatus > -1)
            {
                string str = EmployeeStatus == 0 ? "DateofInactive != null" : "DateofInactive == null";
                filterExpressionBuilder.Append(str + " AND");
            }

            if (ID.HasValue)
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", ID));

            if (!String.IsNullOrWhiteSpace(EmpInitial))
                filterExpressionBuilder.Append(String.Format("EmployeeInitial  =  \"{0}\" AND ", EmpInitial));

            if (!String.IsNullOrWhiteSpace(EmpName))
                filterExpressionBuilder.Append(String.Format("FullName like {0} AND ", EmpName));

            if (DesigName.HasValue && DesigName > 0)
                filterExpressionBuilder.Append(String.Format("DesignationId = {0} AND ", DesigName));

            if (EmpTypeId.HasValue && EmpTypeId > 0)
                filterExpressionBuilder.Append(String.Format("EmploymentTypeId = {0} AND ", EmpTypeId));

            if (DivisionName.HasValue && DivisionName > 0)
                filterExpressionBuilder.Append(String.Format("DivisionId = {0} AND ", DivisionName));

            if (JobLocName.HasValue && JobLocName > 0)
                filterExpressionBuilder.Append(String.Format("JobLocationId = {0} AND ", JobLocName));

            if (GradeName.HasValue && GradeName > 0)
                filterExpressionBuilder.Append(String.Format("GradeId = {0} AND ", GradeName));

            if (StaffCategoryId.HasValue && StaffCategoryId > 0)
                filterExpressionBuilder.Append(String.Format("StaffCategoryId = {0} AND ", StaffCategoryId));

            if (ResourceLevelId.HasValue && ResourceLevelId > 0)
                filterExpressionBuilder.Append(String.Format("ResourceLevelId = {0} AND ", ResourceLevelId));


            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}