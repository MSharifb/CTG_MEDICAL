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
    public partial class RptInvestmentOnFDR : ReportBase
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
                var FDRNo = txtFDRNo.Text;
                DateTime Date = Convert.ToDateTime(txtFDRDateTo.Text);
                decimal? InterestRateFrom = null;
                if (txtInterestFrom.Text == "")
                {
                    InterestRateFrom = null;
                }
                else
                {
                    InterestRateFrom = Convert.ToDecimal(txtInterestFrom.Text);
                }

                decimal? InterestRateTo = null;
                if (txtInterestTo.Text == "")
                {
                    InterestRateTo = null;
                }
                else
                {
                    InterestRateTo = Convert.ToDecimal(txtInterestTo.Text);
                }

                bool isFdrDate = true;

                if (RadioButton1.Checked)
                {
                    isFdrDate = true;
                }
                else
                {
                    isFdrDate = false;
                }

                int FundTypeId =Convert.ToInt32(ddlFundType.SelectedValue.ToString());

                GenerateReport(FDRNo, Date, InterestRateFrom, InterestRateTo, FundTypeId, isFdrDate);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string FDRNo, DateTime Date, decimal? InterestRateFrom, decimal? InterestRateTo, int FundTypeId, bool isFdrDate)
        {
            #region Processing Report Data

            rvFDR.Reset();
            rvFDR.ProcessingMode = ProcessingMode.Local;
            rvFDR.LocalReport.ReportPath = Server.MapPath("~/Reports/FMS/rdlc/RptInvestmentOnFDR.rdlc");
            int fundTypeId = Convert.ToInt32((Session["FDRTypeId"]));

            var data = (from s in base.fmsContext.SP_FMS_InvestmentOnFDR(FDRNo, Date, InterestRateFrom, InterestRateTo, FundTypeId, isFdrDate) select s).ToList();

            var data2 = (from s in base.fmsContext.SP_FMS_InvestmentOnFDREncashment(FDRNo, Date, InterestRateFrom, InterestRateTo, FundTypeId, isFdrDate) select s).ToList();

            #region Search parameter
            string Parameters = string.Concat("Investment On FDR As On " + Date.ToString("dd-MMM-yy"));
            string Parameter2 = Date.ToString("dd-MMM-yy");
            string fundType = fmsContext.FMS_FDRType
                   .Where(x => x.Id == fundTypeId).FirstOrDefault().Name;

            ReportParameter p1 = new ReportParameter("p1", Parameters);
            ReportParameter p2 = new ReportParameter("p2", Parameter2);
            ReportParameter p3 = new ReportParameter("FundType", fundType);

            rvFDR.LocalReport.SetParameters(new ReportParameter[] { p1 , p2, p3});
            #endregion


            ReportDataSource dataSource = new ReportDataSource("DSFDR", data);
            ReportDataSource dataSource2 = new ReportDataSource("DSFDR2", data2);

            rvFDR.LocalReport.DataSources.Add(dataSource);
            rvFDR.LocalReport.DataSources.Add(dataSource2);

            this.rvFDR.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFDR.DataBind();

            //ExportToPDF
            String newFileName = "FDRInvestmentInfo_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvFDR, newFileName, fs);

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

            ddlFundType.DataSource = fmsContext.FMS_FDRType.OrderBy(x => x.Name).ToList();
            ddlFundType.DataValueField = "Id";
            ddlFundType.DataTextField = "Name";
            ddlFundType.DataBind();
            ddlFundType.Items.FindByValue((Session["FDRTypeId"]).ToString()).Selected = true;
        }

        #endregion


        protected void rvFDR_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}