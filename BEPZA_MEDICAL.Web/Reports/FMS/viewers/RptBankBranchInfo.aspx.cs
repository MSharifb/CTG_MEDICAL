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
    public partial class RptBankBranchInfo : ReportBase
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
                int BankId = Convert.ToInt32(ddlBank.SelectedValue);
                int BranchId = Convert.ToInt32(ddlBranch.SelectedValue);
                GenerateReport(BankId, BranchId);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(int BankId, int BranchId)
        {
            #region Processing Report Data

            rvFDRInfo.Reset();
            rvFDRInfo.ProcessingMode = ProcessingMode.Local;
            rvFDRInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/FMS/rdlc/BankBranchInfo.rdlc");

            var data = (from s in base.fmsContext.SP_FMS_BankBranchInfo(BankId, BranchId) select s).ToList();

            #region Search parameter
            //string searchParameters = "For the period of : " + FDRDateFrom.ToString("dd MMM yyyy") + " to " + FDRDateTo.ToString("dd MMM yyyy");
            //ReportParameter p1 = new ReportParameter("ForThePeriodOf", searchParameters);
            //rvFDRInfo.LocalReport.SetParameters(new ReportParameter[] { p1 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DataSet1", data);
            rvFDRInfo.LocalReport.DataSources.Add(dataSource);
            this.rvFDRInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFDRInfo.DataBind();

            //ExportToPDF
            String newFileName = "BankBranch_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvFDRInfo, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = string.Empty;
            var fdrId = 0;

            if (e.ReportPath != "_ReportHeader")
            {
                fdrId = Convert.ToInt32(e.Parameters["FDRID"].Values[0].ToString());
            }

            switch (e.ReportPath)
            {
                case "_FDRInfoInstallmentSchedule":
                    data = (from InstSch in base.fmsContext.SP_FMS_FDRInfoInstallmentSchedule(fdrId) select InstSch).ToList();
                    dsName = "DSInstallmentSchedule";
                    break;
                case "_ReportHeader":
                    data = null;
                    dsName = "DSCompanyInfo";
                    data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                            select c).ToList();
                    e.DataSources.Add(new ReportDataSource(dsName, data));
                    break;

                default:
                    break;
            }


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
        }

        #endregion

        protected void rvFDRInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}