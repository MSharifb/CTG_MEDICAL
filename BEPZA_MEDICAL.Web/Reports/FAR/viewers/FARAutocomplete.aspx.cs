using BEPZA_MEDICAL.Web.Reports.Model;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.FAR.viewers
{
    public partial class FARAutocomplete : ReportBase
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        #region Autocomplete for Employee

        [WebMethod]
        public static EmployeeAutocompleteModel[] GetEmployeeList(string keyword)
        {
            ReportBase dbContext = new ReportBase();
            int LoggedUserZoneInfoId = MyAppSession.ZoneInfoId;
            List<EmployeeAutocompleteModel> objEmpList = new List<EmployeeAutocompleteModel>();

            var employeeInfo = (from tr in dbContext.context.PRM_EmploymentInfo
                                where (tr.ZoneInfoId == LoggedUserZoneInfoId && tr.EmpID.StartsWith(keyword))
                                select new { Id = tr.Id, EmpID = tr.EmpID, employeeName = tr.FullName }).ToList();
            foreach (var item in employeeInfo)
            {
                EmployeeAutocompleteModel obj = new EmployeeAutocompleteModel();
                obj.Id = item.Id;
                obj.EmpID = item.EmpID;
                obj.EmployeeName = item.employeeName;
                objEmpList.Add(obj);
            }

            return objEmpList.ToArray();
        }

        [WebMethod]
        public static string GetEmployeeNameByEmpId(string EmpId)
        {
            ReportBase dbContext = new ReportBase();
            string employeeName = string.Empty;
            var EmpName = (from tr in dbContext.context.PRM_EmploymentInfo
                           where tr.EmpID.Trim() == EmpId.Trim()
                           select tr.FullName).FirstOrDefault();
            if (EmpName != null)
            {
                employeeName = EmpName;
            }
            return employeeName;
        }

        #endregion

        #region Autocomplete for Asset

        [WebMethod]
        public static AssetAutocompleteModel[] GetAssetList(string keyword)
        {
            ReportBase dbContext = new ReportBase();
            int LoggedUserZoneInfoId = MyAppSession.ZoneInfoId;
            List<AssetAutocompleteModel> objList = new List<AssetAutocompleteModel>();


            var assetInfo = keyword != "0" ?
              (from tr in dbContext.farContext.FAR_FixedAsset
               where (tr.ZoneInfoId == LoggedUserZoneInfoId && tr.AssetCode.Contains(keyword))
               select new { Id = tr.Id, assetCode = tr.AssetCode }).ToList() :
              (from tr in dbContext.farContext.FAR_FixedAsset
               where tr.AssetCode.StartsWith(keyword)
               select new { Id = tr.Id, assetCode = tr.AssetCode }).ToList();

            foreach (var item in assetInfo)
            {
                AssetAutocompleteModel obj = new AssetAutocompleteModel();
                obj.Id = item.Id;
                obj.AssetCode = item.assetCode;
                objList.Add(obj);
            }

            return objList.ToArray();
        }

        #endregion
    }
}