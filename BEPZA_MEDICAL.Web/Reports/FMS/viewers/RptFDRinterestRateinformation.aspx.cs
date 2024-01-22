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
    public partial class RptFDRinterestRateinformation : ReportBase
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

                decimal? duration = null;
                if (txtDuration.Text == "")
                {
                    duration = null;
                }
                else
                {
                    duration = Convert.ToDecimal(txtDuration.Text);
                }


                int BankId = Convert.ToInt32(ddlBank.SelectedValue);
                int BranchId = Convert.ToInt32(ddlBranch.SelectedValue);
                string BankType = ddlBankType.SelectedItem.Text == "All" ? string.Empty : ddlBankType.SelectedItem.Value;

                string type = string.Empty;

                if (RadioButton1.Checked)
                {
                    type = "Year";
                }
                else
                {
                    type = "Month";
                }

                GenerateReport(InterestRateFrom, InterestRateTo, BankId, BranchId, BankType, duration, type);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(decimal? InterestRateFrom, decimal? InterestRateTo, int BankId, int BranchId, string BankType, decimal? duration, string type)
        {
            #region Processing Report Data

            rvFDRSchedule.Reset();
            rvFDRSchedule.ProcessingMode = ProcessingMode.Local;
            rvFDRSchedule.LocalReport.ReportPath = Server.MapPath("~/Reports/FMS/rdlc/RptFDRinterestRateinformation.rdlc");
            int fundTypeId = Convert.ToInt32((Session["FDRTypeId"]));

            var data = (from s in base.fmsContext.SP_FMS_FDRinterestRateinformation(InterestRateFrom, InterestRateTo, BankId, BranchId, BankType, duration, type, fundTypeId) select s).ToList();
           
            #region Search parameter
            string fundType = fmsContext.FMS_FDRType
                               .Where(x => x.Id == fundTypeId).FirstOrDefault().Name;

            ReportParameter p1 = new ReportParameter("FundType", fundType);
            rvFDRSchedule.LocalReport.SetParameters(new ReportParameter[] { p1 });
            #endregion

            ReportDataSource dataSource = new ReportDataSource("DataSet1", data);

            rvFDRSchedule.LocalReport.DataSources.Add(dataSource);

            this.rvFDRSchedule.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFDRSchedule.DataBind();

            //ExportToPDF
            String newFileName = "FDRinterestRateinformation_" + Guid.NewGuid() + ".pdf";
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
            ddlBank.DataSource = fmsContext.FMS_BankInfo.OrderBy(x => x.BankName).ToList();
            ddlBank.DataValueField = "Id";
            ddlBank.DataTextField = "BankName";
            ddlBank.DataBind();
            ddlBank.Items.Insert(0, new ListItem("All", "0"));

            ddlBranch.DataSource = fmsContext.FMS_BankInfoBranchDetail.OrderBy(x => x.BranchName).ToList();
            ddlBranch.DataValueField = "Id";
            ddlBranch.DataTextField = "BranchName";
            ddlBranch.DataBind();
            ddlBranch.Items.Insert(0, new ListItem("All", "0"));

            ddlBankType.DataSource = BEPZA_MEDICAL.Web.Utility.Common.PopulateBankType();
            ddlBankType.DataValueField = "Value";
            ddlBankType.DataTextField = "Text";
            ddlBankType.DataBind();
            ddlBankType.Items.Insert(0, new ListItem("All", ""));
        }

        #endregion

        protected void rvFDRSchedule_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}