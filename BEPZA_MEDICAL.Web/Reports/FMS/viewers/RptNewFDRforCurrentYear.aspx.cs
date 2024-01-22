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
    public partial class RptNewFDRforCurrentYear : ReportBase
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
                DateTime FDRDateTo = DateTime.Now;
                var financialYearId =Convert.ToInt32(ddlYear.SelectedValue.ToString());

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

                GenerateReport(strZoneId, FDRNo, financialYearId, InterestRateFrom, InterestRateTo, BankId, BranchId, BankType);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string strZoneList, string FDRNo, int financialYearId, decimal? InterestRateFrom, decimal? InterestRateTo, int BankId, int BranchId, string BankType)
        {
            #region Processing Report Data

            rvFDRSchedule.Reset();
            rvFDRSchedule.ProcessingMode = ProcessingMode.Local;
            rvFDRSchedule.LocalReport.ReportPath = Server.MapPath("~/Reports/FMS/rdlc/RptNewFDRforCurrentYear.rdlc");
            int fundTypeId = Convert.ToInt32((Session["FDRTypeId"]));

            var data = (from s in base.fmsContext.SP_FMS_NewFDRforCurrentYear(strZoneList, FDRNo, financialYearId, InterestRateFrom, InterestRateTo, BankId, BranchId, BankType, fundTypeId) select s).ToList();
           
            #region Search parameter

            string searchParameters = "Statement of FDR for the Period of "+ ddlYear.SelectedItem.Text;
            string fundType = fmsContext.FMS_FDRType
                   .Where(x => x.Id == fundTypeId).FirstOrDefault().Name;

            ReportParameter p1 = new ReportParameter("ForThePeriodOf", searchParameters);
            ReportParameter p2 = new ReportParameter("FundType", fundType);
            rvFDRSchedule.LocalReport.SetParameters(new ReportParameter[] { p1,p2 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSFDRSchedule", data);

            rvFDRSchedule.LocalReport.DataSources.Add(dataSource);

            this.rvFDRSchedule.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFDRSchedule.DataBind();

            //ExportToPDF
            String newFileName = "NewFDRforCurrentYear_" + Guid.NewGuid() + ".pdf";
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

            ddlYear.DataSource = _pgmContext.acc_Accounting_Period_Information.OrderBy(x => x.yearName).ToList();
            ddlYear.DataValueField = "id";
            ddlYear.DataTextField = "yearName";
            ddlYear.DataBind();
            ddlYear.Items.Insert(0, new ListItem("All", "0"));


        }

        #endregion

        protected void rvFDRSchedule_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}