using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class DepartmentProceding : ReportBase
    {
        #region Page Event
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }
        }
        #endregion

        #region User Methods

        private void PopulateDropdownList()
        {
            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            ddlEmployee.DataSource = context.PRM_EmploymentInfo.Select(q => new { ZoneInfoId = q.ZoneInfoId, EmpID = q.EmpID, DisplayText = q.FullName + " [" + q.EmpID + " ]" }).Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.DisplayText).ToList();
            ddlEmployee.DataValueField = "EmpID";
            ddlEmployee.DataTextField = "DisplayText";
            ddlEmployee.DataBind();
            ddlEmployee.Items.Insert(0, new ListItem("All", ""));            
        }

        #endregion

        #region Button Event

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            List<int> zoneList = new List<int>();

            foreach (ListItem item in ddlZone.Items)
            {
                if (item.Selected)
                {
                    zoneList.Add(Convert.ToInt32(item.Value));
                }
            }

            string strZoneId = string.Join(",", zoneList.ToArray());
           // var divisionId = Convert.ToInt32(ddlDivision.SelectedValue);
            DateTime FromDate = Convert.ToDateTime(txtFromDate.Text);
            DateTime ToDate = Convert.ToDateTime(txtToDate.Text);
            var empID = ddlEmployee.SelectedValue;
            GenerateReport(strZoneId,FromDate, ToDate, empID);

        }

        #endregion

        #region Generate Report

        public void GenerateReport(string strZoneId, DateTime fromDate, DateTime toDate, string empID)
        {
            
            rvEmployeeInfo.Reset();
            rvEmployeeInfo.ProcessingMode = ProcessingMode.Local;
            rvEmployeeInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptDepartmentProceding.rdlc");

            #region Processing Report Data

            var data = (from e in base.context.SP_PRM_RptDepartmentProceding(strZoneId, fromDate, toDate, empID) select e).ToList();

            #endregion

            //if (data.Count > 0)
            //{
            //    rvEmployeeInfo.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));
            //    string searchParameters = "As on : " + effectiveDate.ToString("dd MMM yyyy");
            //    ReportParameter p1 = new ReportParameter("param", searchParameters);
            //    rvEmployeeInfo.LocalReport.SetParameters(new ReportParameter[] { p1 });
            //}
            rvEmployeeInfo.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));
            this.rvEmployeeInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvEmployeeInfo.DataBind();

            //ExportToPDF
            String newFileName = "DepartmentalProceeding_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvEmployeeInfo, newFileName, fs);
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = "DSCompanyInfo";
            data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                    select c).ToList();
            e.DataSources.Add(new ReportDataSource(dsName, data));

            //dynamic data = null;
            //var dsName = string.Empty;
            //var empId = 0;

            //if (e.ReportPath != "_ReportHeader")
            //{
            //    empId = Convert.ToInt32(e.Parameters["EmployeeId"].Values[0].ToString());
            //}

            //switch (e.ReportPath)
            //{

            //    case "_PromotedDesignation":
            //        data = base.context.SP_PRM_RptSeniorityPromotedDesignation(empId).ToList();
            //        dsName = "DataSet1";
            //        break;


            //    case "_ReportHeader":
            //        data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
            //                select c).ToList();
            //        dsName = "DSCompanyInfo";
            //        break;

            //    default:
            //        break;
            //}

            //e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        #endregion

        protected void rvEmployeeInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}