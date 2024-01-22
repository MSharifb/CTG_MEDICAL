
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.AMS;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Objects;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.AMS.viewers
{
    public partial class AMSAutocomplete : System.Web.UI.Page
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

            var employees = (from E in dbContext.amsContext.AMS_AnsarEmpInfo
                             join s in dbContext.amsContext.AMS_EmpStatus on E.StatusId equals s.Id
                             where (E.AnsarId.Contains(keyword) || E.FullName.Contains(keyword)) && s.Name == "Active"
                             select new EmployeeAutocompleteModel
                             {
                                 Id = E.Id,
                                 EmpId = E.AnsarId,
                                 EmployeeName = E.FullName

                             }).ToList();

            return employees.ToArray();
        }

        [WebMethod]
        public static EmployeeAutocompleteModel[] GetSecurityPersonnelList(string keyword)
        {
            ReportBase dbContext = new ReportBase();
            List<EmployeeAutocompleteModel> objEmpList = new List<EmployeeAutocompleteModel>();

            var employees = (from E in dbContext.amsContext.PRM_EmploymentInfo
                             join S in dbContext.amsContext.SMS_SecurityInfo on E.Id equals S.EmployeeId
                             where (E.EmpID.Contains(keyword) || E.FullName.Contains(keyword))
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