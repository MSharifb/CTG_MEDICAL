using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class CPFAutocomplete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Autocomplete for Employee

        [WebMethod]
        public static EmployeeAutocompleteModel[] GetEmployeeList(string keyword)
        {
            ReportBase dbContext = new ReportBase();
            List<EmployeeAutocompleteModel> objEmpList = new List<EmployeeAutocompleteModel>();

            var employees = (from E in dbContext.context.PRM_EmploymentInfo
                             where E.EmpID.Contains(keyword) || E.FullName.Contains(keyword)
                             select new EmployeeAutocompleteModel
                             {
                                 Id = E.Id,
                                 EmpId = E.EmpID,
                                 EmployeeName = E.FullName

                             }).ToList();

            return employees.ToArray();
        }

        [WebMethod]
        public static string GetEmployeeNameByEmpId(string empId)
        {
            ReportBase dbContext = new ReportBase();
            string employeeName = string.Empty;
            var EmpName = (from tr in dbContext.context.PRM_EmploymentInfo
                           where tr.EmpID.Trim() == empId.Trim()
                           select tr.FullName).FirstOrDefault();
            if (EmpName != null)
            {
                employeeName = EmpName;
            }
            return employeeName;
        }
        #endregion
    }

    public class EmployeeAutocompleteModel
    {
        public int Id { get; set; }
        public string EmpId { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
    }
}