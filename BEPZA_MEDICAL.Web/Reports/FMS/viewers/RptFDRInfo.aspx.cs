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
    public partial class RptFDRInfo : ReportBase
    {
        #region Fields
        //  private readonly PRM_GenericRepository<PRM_EmploymentInfo> _empRepository;
        #endregion

        #region Ctor
        public RptFDRInfo()
        {
            //  _empRepository = new PRM_GenericRepository<PRM_EmploymentInfo>(new ERP_BEPZAPRMEntities());
        }
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
                int BankId = Convert.ToInt32(ddlBank.SelectedValue);
                int BranchId = Convert.ToInt32(ddlBranch.SelectedValue);
                string BankType = ddlBankType.SelectedItem.Text == "All" ? string.Empty : ddlBankType.SelectedItem.Value;

                GenerateReport(strZoneId, FDRNo, FDRDateFrom, FDRDateTo, InterestRateFrom, InterestRateTo, BankId, BranchId, BankType);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string strZoneList, string FDRNo, DateTime FDRDateFrom, DateTime FDRDateTo, decimal? InterestRateFrom, decimal? InterestRateTo, int BankId, int BranchId, string BankType)
        {
            #region Processing Report Data

            rvFDRInfo.Reset();
            rvFDRInfo.ProcessingMode = ProcessingMode.Local;
            rvFDRInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/FMS/rdlc/RptFDRInfo.rdlc");

            var data = (from s in base.fmsContext.SP_FMS_FDRInfo(strZoneList,FDRNo, FDRDateFrom, FDRDateTo, InterestRateFrom, InterestRateTo, BankId, BranchId, BankType) select s).ToList();

            #region Search parameter
            string searchParameters = "For the period of : " + FDRDateFrom.ToString("dd MMM yyyy") + " to " + FDRDateTo.ToString("dd MMM yyyy");
            ReportParameter p1 = new ReportParameter("ForThePeriodOf", searchParameters);
            rvFDRInfo.LocalReport.SetParameters(new ReportParameter[] { p1 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSFDRInfo", data);
            rvFDRInfo.LocalReport.DataSources.Add(dataSource);
            this.rvFDRInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFDRInfo.DataBind();

            //ExportToPDF
            String newFileName = "FDRInfo_" + Guid.NewGuid() + ".pdf";
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
                    //data = (from c in base.context.vwCompanyInformations
                    //        select c).ToList();
                    //dsName = "DSCompanyInfo";
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

        protected void rvFDRInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}