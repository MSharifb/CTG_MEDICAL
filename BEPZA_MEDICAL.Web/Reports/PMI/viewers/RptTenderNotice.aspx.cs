using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity.Core.Objects;
using System.IO;

namespace BEPZA_MEDICAL.Web.Reports.PMI.viewers
{
    public partial class RptTenderNotice : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptTenderNotice()
        {
            //
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

        int financialYearId = 0;
        int nameOfWorksId = 0;
        string strZoneId = string.Empty;
        DateTime? fromDate = null;
        DateTime? toDate = null;


        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                checkStatus = false;

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

                strZoneId = ConvertZoneArrayListToString(arrZoneList);
                financialYearId = Convert.ToInt32(ddlFinancialYear.SelectedValue);

                if (dtFromDate.Text != "")
                {
                    fromDate = Convert.ToDateTime(dtFromDate.Text);
                }
                if (dtToDate.Text != "")
                {
                    toDate = Convert.ToDateTime(dtToDate.Text);
                }

                if (ddlNameOfWorks.SelectedIndex > 0)
                {
                    nameOfWorksId = Convert.ToInt32(ddlNameOfWorks.SelectedValue);
                }

                GenerateReport(financialYearId, strZoneId, fromDate, toDate, nameOfWorksId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvTenderNotice.Reset();
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report
        public void GenerateReport(int financialYearId, string strZoneId, DateTime? fromDate, DateTime? toDate, int nameOfWorksId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvTenderNotice.Reset();
            rvTenderNotice.ProcessingMode = ProcessingMode.Local;

            switch (ddlFormat.SelectedValue)
            {
                case "Standard Application Form for Enlistment (SAFE) for Goods (SAFE – A)":
                    rvTenderNotice.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptTenderNoticeStandard.rdlc");
                    break;
                case "PW3 - Standard Tender Document (National) For Procurement of Works [Open Tendering Method] (December 2016)":
                    rvTenderNotice.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptTenderNoticeCPTUPW3.rdlc");
                    break;
                case "e-PG3-Procurement of Goods through e-GP System":
                    rvTenderNotice.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptTenderNoticeEPG3.rdlc");
                    break;
                case "PW2a - Standard Tender Document (National) For Procurement of Works [Open Tendering Method] (January-2017)":
                    rvTenderNotice.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptTenderNoticePW2a.rdlc");
                    break;
                case "PW2b - Preliminary Working draft: Standard Tender Document (National) For Procurement of Works [Limited Tendering Method] [December-2012]":
                    rvTenderNotice.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptTenderNoticePW2b.rdlc");
                    break;
                case "PG3 - Preliminary Working draft: Standard Tender Document (National)For Procurement of Goods [Open Tendering Method] (February 2015)":
                    rvTenderNotice.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptTenderNoticeSTDPG3.rdlc");
                    break;

            }

            var data = pmiContext.sp_PMI_RptTenderNotice(financialYearId, fromDate, toDate, nameOfWorksId, strZoneId, numErrorCode, strErrorMsg).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = "Tender Notice";
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvTenderNotice.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsTenderNotice", data);
                rvTenderNotice.LocalReport.DataSources.Add(dataSource);
                this.rvTenderNotice.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvTenderNotice.Reset();
            }
            rvTenderNotice.DataBind();

            //ExportToPDF
            String newFileName = "TenderNoticeReport_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvTenderNotice, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            dynamic data = null;
            var ProjectMasterId = 0;
            var dsName = string.Empty;
            if (e.ReportPath != "_ReportHeader")
            {
                ProjectMasterId = Convert.ToInt32(e.Parameters["ProjectMasterId"].Values[0] == null ? 0 : Convert.ToInt32(e.Parameters["ProjectMasterId"].Values[0]));
            }
            switch (e.ReportPath)
            {
                case "SubRptTenderNoticeDetailsCommon":
                    var tenderDetails = pmiContext.sp_PMI_RptTenderNoticeDetails(financialYearId, fromDate, toDate, nameOfWorksId, strZoneId, numErrorCode, strErrorMsg).ToList();
                    data = tenderDetails.Where(x => x.ProjectMasterId == ProjectMasterId).ToList();
                    dsName = "dsTenderNoticeDetails";
                    break;

                case "SubRptTenderNoticeDetailsEPG3":
                    var tenderDetailsEPG3 = pmiContext.sp_PMI_RptTenderNoticeDetails(financialYearId, fromDate, toDate, nameOfWorksId, strZoneId, numErrorCode, strErrorMsg).ToList();
                    data = tenderDetailsEPG3.Where(x => x.ProjectMasterId == ProjectMasterId).ToList();
                    dsName = "dsTenderNoticeDetails";
                    break;

                case "_ReportHeader":
                    data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                            select c).ToList();
                    dsName = "DSCompanyInfo";
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
            var projectMaster = pmiContext.PMI_ProjectMaster.ToList();

            ddlFinancialYear.DataSource = pmiContext.acc_Accounting_Period_Information.OrderByDescending(x => x.yearName).ToList();
            ddlFinancialYear.DataValueField = "Id";
            ddlFinancialYear.DataTextField = "yearName";
            ddlFinancialYear.DataBind();
            ddlFinancialYear.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlNameOfWorks.DataSource = projectMaster.OrderBy(x => x.NameOfWorks).ToList();
            ddlNameOfWorks.DataValueField = "Id";
            ddlNameOfWorks.DataTextField = "NameOfWorks";
            ddlNameOfWorks.DataBind();
            ddlNameOfWorks.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            var formatList = new List<string> { "Standard Application Form for Enlistment (SAFE) for Goods (SAFE – A)", "PW3 - Standard Tender Document (National) For Procurement of Works [Open Tendering Method] (December 2016)", "e-PG3-Procurement of Goods through e-GP System", "PW2a - Standard Tender Document (National) For Procurement of Works [Open Tendering Method] (January-2017)", "PW2b - Preliminary Working draft: Standard Tender Document (National) For Procurement of Works [Limited Tendering Method] [December-2012]", "PG3 - Preliminary Working draft: Standard Tender Document (National)For Procurement of Goods [Open Tendering Method] (February 2015)" };
            ddlFormat.DataSource = formatList;
            ddlFormat.DataBind();
        }
        #endregion

        protected void rvTenderNotice_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}