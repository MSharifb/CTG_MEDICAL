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
    public partial class RptFDRSchedule : ReportBase
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
                int BankId = Convert.ToInt32(ddlBank.SelectedValue);
                int BranchId = Convert.ToInt32(ddlBranch.SelectedValue);
                string BankType = ddlBankType.SelectedItem.Text == "All" ? string.Empty : ddlBankType.SelectedItem.Value;

                bool isFdrDate = true;

                if (RadioButton1.Checked)
                {
                    isFdrDate = true;
                }
                else
                {
                    isFdrDate = false;
                }
                int FundTypeId = Convert.ToInt32(ddlFundType.SelectedValue.ToString());

                GenerateReport(strZoneId, FDRNo, FDRDateTo, InterestRateFrom, InterestRateTo, BankId, BranchId, BankType, isFdrDate, FundTypeId);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string strZoneList, string FDRNo, DateTime FDRDateTo, decimal? InterestRateFrom, decimal? InterestRateTo, int BankId, int BranchId, string BankType, bool isFdrDate,int FundTypeId)
        {
            #region Processing Report Data

            rvFDRSchedule.Reset();
            rvFDRSchedule.ProcessingMode = ProcessingMode.Local;
            rvFDRSchedule.LocalReport.ReportPath = Server.MapPath("~/Reports/FMS/rdlc/RptFDRSchedule.rdlc");
            int fundTypeId = Convert.ToInt32((Session["FDRTypeId"]));

            var data = (from s in base.fmsContext.SP_FMS_FDRSchedule(strZoneList, FDRNo, FDRDateTo, InterestRateFrom, InterestRateTo, BankId, BranchId, BankType, isFdrDate, FundTypeId) select s).ToList();
            var data2 = (from s in base.fmsContext.SP_FMS_FDRPercentage(FundTypeId) select s).ToList();

            #region Search parameter

            string searchParameters = "Statement of FDR as on " + FDRDateTo.ToString("dd MMM yyyy");
            string fundType = fmsContext.FMS_FDRType
                   .Where(x => x.Id == fundTypeId).FirstOrDefault().Name;

            ReportParameter p1 = new ReportParameter("ForThePeriodOf", searchParameters);
            ReportParameter p2 = new ReportParameter("FundType", fundType);
            rvFDRSchedule.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSFDRSchedule", data);
            ReportDataSource dataSource2 = new ReportDataSource("DSFDRPercentage", data2);

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
        }

        #endregion

        protected void rvFDRSchedule_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        protected void btnSchedule2_Click(object sender, EventArgs e)
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
                int BankId = Convert.ToInt32(ddlBank.SelectedValue);
                int BranchId = Convert.ToInt32(ddlBranch.SelectedValue);
                string BankType = ddlBankType.SelectedItem.Text == "All" ? string.Empty : ddlBankType.SelectedItem.Value;
                bool isFdrDate = true;

                if (RadioButton1.Checked)
                {
                    isFdrDate = true;
                }
                else
                {
                    isFdrDate = false;
                }
                int FundTypeId = Convert.ToInt32(ddlFundType.SelectedValue.ToString());

                GenerateReport2(strZoneId, FDRNo, FDRDateTo, InterestRateFrom, InterestRateTo, BankId, BranchId, BankType, isFdrDate, FundTypeId);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #region Generate Report

        public void GenerateReport2(string strZoneList, string FDRNo, DateTime FDRDateTo, decimal? InterestRateFrom, decimal? InterestRateTo, int BankId, int BranchId, string BankType,bool isFdrDate, int FundTypeId)
        {
            #region Processing Report Data

            rvFDRSchedule.Reset();
            rvFDRSchedule.ProcessingMode = ProcessingMode.Local;
            rvFDRSchedule.LocalReport.ReportPath = Server.MapPath("~/Reports/FMS/rdlc/RptFDRSchedule2.rdlc");
            int fundTypeId = Convert.ToInt32((Session["FDRTypeId"]));

            var data = (from s in base.fmsContext.SP_FMS_FDRSchedule(strZoneList, FDRNo, FDRDateTo, InterestRateFrom, InterestRateTo, BankId, BranchId, BankType, isFdrDate, FundTypeId) select s).ToList();
            var data2 = (from s in base.fmsContext.SP_FMS_FDRPercentage(FundTypeId) select s).ToList();
            
            #region Search parameter
            string searchParameters = "Statement of FDR as on " + FDRDateTo.ToString("dd MMM yyyy");
            string fundType = fmsContext.FMS_FDRType
                              .Where(x => x.Id == fundTypeId).FirstOrDefault().Name;

            ReportParameter p1 = new ReportParameter("ForThePeriodOf", searchParameters);
            ReportParameter p2 = new ReportParameter("FundType", fundType);
            rvFDRSchedule.LocalReport.SetParameters(new ReportParameter[] { p1,p2 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSFDRSchedule", data);
            ReportDataSource dataSource2 = new ReportDataSource("DSFDRPercentage", data2);

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
        #endregion

    }
}