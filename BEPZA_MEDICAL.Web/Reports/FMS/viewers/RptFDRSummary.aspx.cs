using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.FMS.viewers
{
    public partial class RptFDRSummary : ReportBase
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
                string amountType = ddlAmountType.SelectedValue.ToString();
                int fundTypeId =Convert.ToInt32(ddlFundType.SelectedValue);
                DateTime date = Convert.ToDateTime(txtFDRDateTo.Text);
                bool isFDRDate = true;
                if (RadioButton1.Checked)
                {
                    isFDRDate = true;
                }
                else
                {
                    isFDRDate = false;
                }
                GenerateReport(amountType, fundTypeId, date, isFDRDate);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string type, int fundTypeId, DateTime date, bool isFDRDate)
        {
            #region Processing Report Data

            rvFDRSchedule.Reset();
            rvFDRSchedule.ProcessingMode = ProcessingMode.Local;
            rvFDRSchedule.LocalReport.ReportPath = Server.MapPath("~/Reports/FMS/rdlc/RptFDRSummary.rdlc");

            var data = (from s in base.fmsContext.SP_FMS_FDRSummary(type, fundTypeId, date, isFDRDate) select s).ToList();
            #region Search parameter
            string searchParameters = "Statement of FDR as on " + date.ToString("dd.MM.yyyy");
            string fundType = fmsContext.FMS_FDRType
                   .Where(x => x.Id == fundTypeId).FirstOrDefault().Name;

            ReportParameter p1 = new ReportParameter("p1", searchParameters);
            ReportParameter p2 = new ReportParameter("p2", ddlAmountType.SelectedValue.ToString());
            ReportParameter p3 = new ReportParameter("FundType", fundType);

            rvFDRSchedule.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DataSet1", data);

            rvFDRSchedule.LocalReport.DataSources.Add(dataSource);

            this.rvFDRSchedule.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFDRSchedule.DataBind();

            //ExportToPDF
            String newFileName = "FDRSummary_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvFDRSchedule, newFileName, fs);

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

            ddlAmountType.DataSource = PopulateBankType();
            ddlAmountType.DataValueField = "Value";
            ddlAmountType.DataTextField = "Text";
            ddlAmountType.DataBind();
            //ddlAmountType.Items.Insert(0, new ListItem("All", ""));

            ddlFundType.DataSource = fmsContext.FMS_FDRType.OrderBy(x => x.Name).ToList();
            ddlFundType.DataValueField = "Id";
            ddlFundType.DataTextField = "Name";
            ddlFundType.DataBind();
            ddlFundType.Items.FindByValue((Session["FDRTypeId"]).ToString()).Selected = true;
        }
        public static IList<SelectListItem> PopulateBankType()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem(){ Text="FDR Amount", Value="FDR"},
                new SelectListItem(){ Text="FDR Amount with Present Value", Value="PV" },
            };
        }
        #endregion

        protected void rvFDRSchedule_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}