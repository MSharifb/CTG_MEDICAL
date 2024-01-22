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
    public partial class RptFDRDailyreport : ReportBase
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
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        zoneList.Add(Convert.ToInt32(item.Value));
                    }
                }

                string strZoneId = string.Join(",", zoneList.ToArray());
                var FDRNo = txtFDRNo.Text;
                DateTime? FDRDate = null;
                if (txtFDRDateTo.Text == string.Empty)
                {
                    FDRDate = null;
                }
                else
                {
                    FDRDate =  Convert.ToDateTime(txtFDRDateTo.Text);
                }

                decimal? duration = null;
                if (txtDuration.Text == string.Empty)
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

                string year = ddlYear.SelectedValue.ToString();
                string month = ddlMonth.SelectedValue.ToString();

                string type = string.Empty;

                if (RadioButton1.Checked)
                {
                    type = "Year";
                }
                else if(RadioButton2.Checked)
                {
                    type = "Month";
                }
                else if (RadioButton5.Checked)
                {
                    type = string.Empty;
                }
                int FundTypeId = Convert.ToInt32(ddlFundType.SelectedValue.ToString());
                bool isFdrDate = true;

                if (RadioButton3.Checked)
                {
                    isFdrDate = true;
                }
                else
                {
                    isFdrDate = false;
                }

                GenerateReport(strZoneId, FDRNo, year, month, FDRDate, duration, type, BankId, BranchId, BankType, isFdrDate, FundTypeId);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string strZoneList, string FDRNo, string year, string month, DateTime? FDRDate, decimal? duration, string type, int BankId, int BranchId, string BankType, bool isFdrDate, int FundTypeId)
        {
            #region Processing Report Data

            rvFDRSchedule.Reset();
            rvFDRSchedule.ProcessingMode = ProcessingMode.Local;
            rvFDRSchedule.LocalReport.ReportPath = Server.MapPath("~/Reports/FMS/rdlc/RptFDRDailyreport.rdlc");

            var data = (from s in base.fmsContext.SP_FMS_FDRDailyreport(strZoneList, FDRNo, year, month, FDRDate, duration, type, BankId, BranchId, BankType, FundTypeId, isFdrDate) select s).ToList();
            var data2 = (from s in base.fmsContext.SP_FMS_FDRPercentage(FundTypeId) select s).ToList();
            #region Search parameter
            string searchParameters = string.Empty;
            if (txtFDRDateTo.Text == string.Empty)
            {
                if(!month.Contains("0"))
                 searchParameters = "Renewal Date of FDR for the month of " + ddlMonth.SelectedItem.Text + ", " + ddlYear.SelectedItem.Text;
                else
                    searchParameters = "Renewal Date of FDR for the year of " + ddlYear.SelectedItem.Text;
            }
            else
            {
                searchParameters = "Renewal Date of FDR up to " + Convert.ToDateTime(FDRDate).ToString("dd.MM.yyyy");
            }
            var footer = "0";
            if (isFooter.Checked)
            {
                footer = "1";
            }
            string fundType = fmsContext.FMS_FDRType
                   .Where(x => x.Id == FundTypeId).FirstOrDefault().Name;


            ReportParameter p1 = new ReportParameter("p1", searchParameters);
            ReportParameter p2 = new ReportParameter("p2", footer);
            ReportParameter p3 = new ReportParameter("FundType", fundType);

            rvFDRSchedule.LocalReport.SetParameters(new ReportParameter[] { p1,p2,p3 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DataSet1", data);
            ReportDataSource dataSource2 = new ReportDataSource("DataSet2", data2);

            rvFDRSchedule.LocalReport.DataSources.Add(dataSource);
            rvFDRSchedule.LocalReport.DataSources.Add(dataSource2);

            this.rvFDRSchedule.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFDRSchedule.DataBind();

            //ExportToPDF
            String newFileName = "FDRSchedule_" + Guid.NewGuid() + ".pdf";
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

            ddlFundType.DataSource = fmsContext.FMS_FDRType.OrderBy(x => x.Name).ToList();
            ddlFundType.DataValueField = "Id";
            ddlFundType.DataTextField = "Name";
            ddlFundType.DataBind();
            ddlFundType.Items.FindByValue((Session["FDRTypeId"]).ToString()).Selected = true;

            ddlYear.DataSource = BEPZA_MEDICAL.Web.Utility.Common.PopulateYearListFMS();
            ddlYear.DataValueField = "Value";
            ddlYear.DataTextField = "Text";
            ddlYear.DataBind();
            ddlYear.Items.Insert(0, new ListItem("All", ""));

            ddlMonth.DataSource = BEPZA_MEDICAL.Web.Utility.Common.PopulateMonthList3();
            ddlMonth.DataValueField = "Value";
            ddlMonth.DataTextField = "Text";
            ddlMonth.DataBind();
            ddlMonth.Items.Insert(0, new ListItem("All", ""));

        }

        #endregion

        protected void rvFDRSchedule_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}