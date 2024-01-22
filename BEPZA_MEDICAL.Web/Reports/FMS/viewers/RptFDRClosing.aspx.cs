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
    public partial class RptFDRClosing : ReportBase
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
                var FDRNo = txtFDRNo.Text;
                DateTime FDRDateFrom = Convert.ToDateTime(txtFRDDateFrom.Text);
                DateTime FDRDateTo = Convert.ToDateTime(txtFDRDateTo.Text);
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
                DateTime? MaturityDateFrom = null;
                if (txtMaturityDateFrom.Text == "")
                {
                    MaturityDateFrom = null;
                }
                else
                {
                    MaturityDateFrom = Convert.ToDateTime(txtMaturityDateFrom.Text);
                }
                DateTime? MaturityDateTo = null;
                if (txtMaturityDateTo.Text == "")
                {
                    MaturityDateTo = null;
                }
                else
                {
                    MaturityDateTo = Convert.ToDateTime(txtMaturityDateTo.Text);
                }

                DateTime? ClosingDateFrom = null;
                if (txtClosingDateFrom.Text == "")
                {
                    ClosingDateFrom = null;
                }
                else
                {
                   ClosingDateFrom = Convert.ToDateTime(txtClosingDateFrom.Text);
                }

                DateTime? ClosingDateTo = null;
                if (txtClosingDateTo.Text == "")
                {
                    ClosingDateTo = null;
                }
                else
                {
                    ClosingDateTo = Convert.ToDateTime(txtClosingDateTo.Text);
                }              

                int BankId = Convert.ToInt32(ddlBank.SelectedValue);
                int BranchId = Convert.ToInt32(ddlBranch.SelectedValue);
                string BankType = ddlBankType.SelectedItem.Text == "All" ? string.Empty : ddlBankType.SelectedItem.Value;

                GenerateReport(strZoneId, FDRNo, FDRDateFrom, FDRDateTo, InterestRateFrom, InterestRateTo, MaturityDateFrom, MaturityDateTo, ClosingDateFrom, ClosingDateTo, BankId, BranchId, BankType);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string strZoneList, string FDRNo, DateTime FDRDateFrom, DateTime FDRDateTo, decimal? InterestRateFrom, decimal? InterestRateTo, DateTime? MaturityDateFrom, DateTime? MaturityDateTo, DateTime? ClosingDateFrom, DateTime? ClosingDateTo, int BankId, int BranchId, string BankType)
        {
            #region Processing Report Data

            rvFDRClosing.Reset();
            rvFDRClosing.ProcessingMode = ProcessingMode.Local;
            rvFDRClosing.LocalReport.ReportPath = Server.MapPath("~/Reports/FMS/rdlc/RptFDRClosing.rdlc");

            var data = (from s in base.fmsContext.SP_FMS_FDRClosingInfo(strZoneList, FDRNo, FDRDateFrom, FDRDateTo, InterestRateFrom, InterestRateTo, MaturityDateFrom, MaturityDateTo, ClosingDateFrom, ClosingDateTo, BankId, BranchId, BankType) select s).ToList();

            #region Search parameter
            string searchParameters = "For the period of : " + FDRDateFrom.ToString("dd MMM yyyy") + " To " + FDRDateTo.ToString("dd MMM yyyy");
            ReportParameter p1 = new ReportParameter("ForThePeriodOf", searchParameters);
            rvFDRClosing.LocalReport.SetParameters(new ReportParameter[] { p1 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSFDRClosing", data);
            rvFDRClosing.LocalReport.DataSources.Add(dataSource);
            this.rvFDRClosing.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFDRClosing.DataBind();

            //ExportToPDF
            String newFileName = "FDRClosingReport_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvFDRClosing, newFileName, fs);

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

        protected void rvFDRClosing_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}