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
    public partial class RptEmpClassWiseAllZonesManpower : ReportBase
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

            ddlMonth.DataSource = BEPZA_MEDICAL.Web.Utility.Common.PopulateMonthList3();
            ddlMonth.DataValueField = "Value";
            ddlMonth.DataTextField = "Text";
            ddlMonth.DataBind();
            ddlMonth.Items.Insert(0, new ListItem("[Select One]", ""));

            ddlYear.DataSource = BEPZA_MEDICAL.Web.Utility.Common.PopulateYearList();
            ddlYear.DataValueField = "Value";
            ddlYear.DataTextField = "Text";
            ddlYear.DataBind();
            ddlYear.Items.Insert(0, new ListItem("[Select One]", ""));
        }

        #endregion

        #region Button Event

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            var month = ddlMonth.SelectedValue.ToString();
            var year = ddlYear.SelectedValue.ToString();
            GenerateReport(month, year);
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string month, string year)
        {
            rvEmployeeInfo.Reset();
            rvEmployeeInfo.ProcessingMode = ProcessingMode.Local;
            rvEmployeeInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptEmpClassWiseAllZonesManpower.rdlc");

            #region Processing Report Data

            var data = (from e in base.context.SP_PRM_EmpClassWiseAllZoneManpower(month,year) select e).ToList();

            #endregion

            if (data.Count > 0)
            {
                rvEmployeeInfo.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));
                string searchParameters = "For the Month of " + ddlMonth.SelectedItem.Text + ", " + ddlYear.SelectedItem.Text;
                ReportParameter p1 = new ReportParameter("p1", searchParameters);
                rvEmployeeInfo.LocalReport.SetParameters(new ReportParameter[] { p1 });
            }

            this.rvEmployeeInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvEmployeeInfo.DataBind();

            //ExportToPDF
            String newFileName = "EmployeeclassWiseManpowerList_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvEmployeeInfo, newFileName, fs);
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

        protected void rvEmployeeInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}