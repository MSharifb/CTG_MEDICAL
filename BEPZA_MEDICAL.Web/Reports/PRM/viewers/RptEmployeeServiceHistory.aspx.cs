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
    public partial class RptEmployeeServiceHistory : ReportBase
    {
        #region Fields
        #endregion

        #region Ctor
        #endregion

        #region Page Event

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }

        }
        #endregion

        #region Button Event
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> zoneList = new List<int>();
                int[] arrZoneList = new int[] { };
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        zoneList.Add(Convert.ToInt32(item.Value));
                    }
                }
                arrZoneList = zoneList.ToArray();

                string strZoneId = ConvertZoneArrayListToString(arrZoneList);
                int designationId = Convert.ToInt32(ddlDesignation.SelectedValue);
                int employeeId = Convert.ToInt32(ddlEmployee.SelectedValue);
                string reportType = rbReportType.SelectedValue;
                GenerateReport(employeeId, strZoneId, designationId, reportType);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report
        public void GenerateReport(int employeeId, string ZoneId, int Designation, string reportType)
        {
            #region Processing Report Data

            rvEmployeeInfo.Reset();
            rvEmployeeInfo.ProcessingMode = ProcessingMode.Local;
            if (reportType == "F1")
            {
                rvEmployeeInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptEmployeeServiceHistory.rdlc");
                var data = (from s in base.prmContext.SP_PRM_RptServiceVistory(employeeId, Designation, ZoneId) select s).ToList();
                ReportDataSource dataSource = new ReportDataSource("DataSet1", data);
                rvEmployeeInfo.LocalReport.DataSources.Add(dataSource);

            }
            else
            {
                rvEmployeeInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptEmployeeServiceHistory_M1.rdlc");
                var data = (from s in base.prmContext.SP_PRM_RptServiceVistory_M1(employeeId, Designation, ZoneId) select s).ToList();
                ReportDataSource dataSource = new ReportDataSource("DataSet1", data);
                rvEmployeeInfo.LocalReport.DataSources.Add(dataSource);
            }

            this.rvEmployeeInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvEmployeeInfo.DataBind();

            //ExportToPDF
            String newFileName = "ServiceHistory_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvEmployeeInfo, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = string.Empty;
            switch (e.ReportPath)
            {
                case "_ReportHeader":
                    data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                            select c).ToList();
                    dsName = "DSCompanyInfo";
                    break;

                default:
                    break;
            }
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }
        #endregion

        #region User Methods
        private void PopulateDropdownList()
        {
            ddlDesignation.DataSource = context.PRM_Designation.OrderBy(x => x.Name).ToList();
            ddlDesignation.DataValueField = "Id";
            ddlDesignation.DataTextField = "Name";
            ddlDesignation.DataBind();
            ddlDesignation.Items.Insert(0, new ListItem("All", "0"));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            ddlEmployee.DataSource = context
            .PRM_EmploymentInfo
            .Select(q => new
            {
                ZoneInfoId = q.ZoneInfoId,
                EmpID = q.Id,
                DisplayText = q.FullName + " [" + q.EmpID + "]"
            }).ToList()
            .Where(x => x.ZoneInfoId == LoggedUserZoneInfoId)
            .OrderBy(x => x.DisplayText);
            ddlEmployee.DataValueField = "EmpID";
            ddlEmployee.DataTextField = "DisplayText";
            ddlEmployee.DataBind();
            ddlEmployee.Items.Insert(0, new ListItem("All", "0"));

        }
        #endregion

        protected void rvEmployeeInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}