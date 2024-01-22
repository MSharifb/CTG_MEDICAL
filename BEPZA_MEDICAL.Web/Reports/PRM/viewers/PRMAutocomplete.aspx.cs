using BEPZA_MEDICAL.Web.Reports.Model;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class PRMAutocomplete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static EmployeeAutocompleteModel[] GetEmployeeList(string keyword)
        {
            ReportBase dbContext = new ReportBase();
            List<EmployeeAutocompleteModel> objEmpList = new List<EmployeeAutocompleteModel>();

            var employeeInfo = (from tr in dbContext.context.PRM_EmploymentInfo
                                where tr.ZoneInfoId == MyAppSession.ZoneInfoId && tr.EmpID.StartsWith(keyword)
                                select new { Id = tr.Id, EmpID = tr.EmpID, employeeName = tr.FullName }).ToList();
            foreach (var item in employeeInfo)
            {
                EmployeeAutocompleteModel obj = new EmployeeAutocompleteModel();
                obj.Id = item.Id;
                obj.EmpId = item.EmpID;
                obj.EmployeeName = item.employeeName;
                objEmpList.Add(obj);
            }

            return objEmpList.ToArray();
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
                employeeName = EmpName.ToString();
            }           
            return employeeName;
        }


    }
}