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
    public partial class RptFDRInterestReceivableforCPF : ReportBase
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
                int periodId = Convert.ToInt32(ddlPeriod.SelectedValue);

                GenerateReport(strZoneId, periodId);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string strZoneList, int periodId)
        {
            #region Processing Report Data

            rvFDRInstallmentInfo.Reset();
            rvFDRInstallmentInfo.ProcessingMode = ProcessingMode.Local;
            rvFDRInstallmentInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/FMS/rdlc/RptFDRInterestReceivableforCPF.rdlc");
            int fundTypeId = Convert.ToInt32((Session["FDRTypeId"]));

            var data = (from s in base.fmsContext.SP_FMS_CPFInserestReceivable(strZoneList, periodId, fundTypeId) select s).ToList();
            var data2 = (from s in base.fmsContext.SP_FMS_StatementOfFRDInserestEncashmentYearly(strZoneList, periodId, fundTypeId) select s).ToList();

            #region Search parameter
            var Date = _pgmContext.acc_Accounting_Period_Information.Where(x => x.id == periodId).FirstOrDefault();
            var lastDate = Date.periodEndDate.AddYears(-1);
            string searchParameters = "Up To : " + Date.periodEndDate.ToString("dd MMM yyyy");
            string lastFinancialYearDate = lastDate.ToString("dd MMM yyyy");
            string period = ddlPeriod.SelectedItem.Text;

            ReportParameter p1 = new ReportParameter("BalanceAsOn", searchParameters);
            ReportParameter p2 = new ReportParameter("LastFinancialYearDate", lastFinancialYearDate);
            ReportParameter p3 = new ReportParameter("Period", period);

            rvFDRInstallmentInfo.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3 });
            #endregion


            ReportDataSource dataSource = new ReportDataSource("DSFDRInterestReceivable", data);
            ReportDataSource dataSource2 = new ReportDataSource("DSFDRInterestReceivable2", data2);

            rvFDRInstallmentInfo.LocalReport.DataSources.Add(dataSource);
            rvFDRInstallmentInfo.LocalReport.DataSources.Add(dataSource2);

            //this.rvFDRInstallmentInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFDRInstallmentInfo.DataBind();

            //ExportToPDF
            String newFileName = "FDRInstallmentInfo_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvFDRInstallmentInfo, newFileName, fs);

            #endregion
        }

        //void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        //{
        //    dynamic data = null;
        //    var dsName = "DSCompanyInfo";
        //    data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
        //            select c).ToList();
        //    e.DataSources.Add(new ReportDataSource(dsName, data));
        //}

        #endregion

        #region User Methods

        private void PopulateDropdownList()
        {

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            ddlPeriod.DataSource = _pgmContext.acc_Accounting_Period_Information.OrderBy(x => x.yearName).ToList();
            ddlPeriod.DataValueField = "id";
            ddlPeriod.DataTextField = "yearName";
            ddlPeriod.DataBind();
            ddlPeriod.Items.Insert(0, new ListItem("[Select One]", "0"));
        }

        #endregion

        protected void rvFDRInstallmentInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}