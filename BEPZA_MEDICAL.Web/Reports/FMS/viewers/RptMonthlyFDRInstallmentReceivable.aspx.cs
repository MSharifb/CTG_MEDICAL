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

namespace BEPZA_MEDICAL.Web.Reports.FMS.viewers
{
    public partial class RptMonthlyFDRInstallmentReceivable : ReportBase
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
                var Year = Convert.ToInt32(ddlYear.SelectedValue);
                var Month = Convert.ToInt32(ddlMonth.SelectedValue);

                var days = DateTime.DaysInMonth(Year, Month);

                var fromDate = Year+"-"+Month+"-"+"01";
                var toDate = Year + "-" + Month + "-" +days;

                GenerateReport(strZoneId, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string strZoneList, DateTime fromDate, DateTime toDate)
        {
            #region Processing Report Data

            rvFDRInstallmentInfo.Reset();
            rvFDRInstallmentInfo.ProcessingMode = ProcessingMode.Local;
            rvFDRInstallmentInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/FMS/rdlc/RptMonthlyFDRInstallmentReceivable.rdlc");

            var data = (from s in base.fmsContext.SP_FMS_MonthlyFDRInstallmentReceivable(fromDate, toDate, strZoneList) select s).ToList();

            #region Search parameter
            string Parameters = "For the Month of " + Convert.ToString(ddlMonth.SelectedItem) + "/" + Convert.ToString(ddlYear.SelectedValue);

            ReportParameter p1 = new ReportParameter("p1", Parameters);

            rvFDRInstallmentInfo.LocalReport.SetParameters(new ReportParameter[] { p1 });
            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSFDR", data);

            rvFDRInstallmentInfo.LocalReport.DataSources.Add(dataSource);

            this.rvFDRInstallmentInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFDRInstallmentInfo.DataBind();

            //ExportToPDF
            String newFileName = "FDRInstallmentInfo_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvFDRInstallmentInfo, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = "DSCompanyInfo";
            data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                    select c).ToList();
            e.DataSources.Add(new ReportDataSource(dsName, data));
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

            int j = 0;
            foreach (var year in BEPZA_MEDICAL.Web.Utility.Common.PopulateYearList().ToList())
            {
                ddlYear.Items.Insert(j, new ListItem() { Text = year.Value.ToString(), Value = year.Value.ToString() });
                j++;
            }
            ddlYear.Items.Insert(0, new ListItem("[Select One]", ""));

            int k = 0;
            foreach (var month in BEPZA_MEDICAL.Web.Utility.Common.PopulateMonthListForReport().ToList())
            {
                ddlMonth.Items.Insert(k, new ListItem() { Text = month.Text.ToString(), Value = month.Value.ToString() });
                k++;
            }
            ddlMonth.Items.Insert(0, new ListItem("[Select One]", ""));

            ddlMonth.Items.FindByValue(DateTime.Now.Month.ToString()).Selected = true;
        }

        #endregion

        protected void rvFDRInstallmentInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}