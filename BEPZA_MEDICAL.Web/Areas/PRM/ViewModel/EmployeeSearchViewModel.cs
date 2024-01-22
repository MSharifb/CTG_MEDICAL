using System;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmployeeSearchViewModel
    {
        public EmployeeSearchViewModel()
        {

        }

        #region Properties
        public int? ID { get; set; }
        public string EmpId { get; set; }
        //public string EmpInitial { get; set; }
        public string EmpName { get; set; }
        public int? DesigName { get; set; }
        public int? EmpTypeId { get; set; }
        public int? DivisionName { get; set; }
        public int? JobLocName { get; set; }
        public int? GradeName { get; set; }
        public int? StaffCategoryId { get; set; }

        public int EmployeeStatus { get; set; }
        public int SelectedEmployeeStatus { get; set; }

        public string ActionName { get; set; }
        public string Message { get; set; }
        public string SearchEmpType { get; set; }

        public string UseTypeEmpId { get; set; }
        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int? SalaryWithdrawZoneId { get; set; }

        public Boolean IsBonusEligible { get; set; }
        public Boolean IsOvertimeEligible { get; set; }
        public Boolean? IsRefreshmentEligible { get; set; }

        #endregion

        #region Methods
        public string GetFilterExpression(String EmpStatus, String isBonus, String isOvertime, String isRefreshment)
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (!String.IsNullOrEmpty(EmpStatus) && EmpStatus != "0")
            {
                if (EmployeeStatus > -1)
                {
                    string str = EmployeeStatus == 0 ? "DateofInactive != null" : "DateofInactive == null";
                    filterExpressionBuilder.Append(str + " AND ");
                }
            }

            if (ID.HasValue)
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", ID));

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

            if (SalaryWithdrawZoneId.HasValue && SalaryWithdrawZoneId > 0)
                filterExpressionBuilder.Append(String.Format("SalaryWithdrawZoneId = {0} AND ", SalaryWithdrawZoneId));

            if (!string.IsNullOrEmpty(isBonus) && isBonus != "0" && (IsBonusEligible || !IsBonusEligible))
                filterExpressionBuilder.Append(String.Format("IsBonusEligible={0} AND ", String.Format("{0}", IsBonusEligible)));

            if (!string.IsNullOrEmpty(isOvertime) && isOvertime != "0" && (IsOvertimeEligible || !IsOvertimeEligible))
                filterExpressionBuilder.Append(String.Format("IsOvertimeEligible={0} AND ", String.Format("{0}", IsOvertimeEligible)));

            if (!string.IsNullOrEmpty(isRefreshment) && isRefreshment != "0" && (Convert.ToBoolean(IsRefreshmentEligible) || !Convert.ToBoolean(IsRefreshmentEligible)))
                filterExpressionBuilder.Append(String.Format("IsRefreshmentEligible={0} AND ", String.Format("{0}", Convert.ToBoolean(IsRefreshmentEligible))));

            //-----------------
            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);

            return filterExpressionBuilder.ToString();
        }
        #endregion

        #region Organogram
        [DisplayName("Organogram Level Name")]
        [UIHint("_ReadOnly")]
        public string OrganogramLevelName { get; set; }
        public int? OrganogramLevelId { get; set; }
        public int ZoneInfoId { get; set; }
        #endregion
    }
}