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

namespace BEPZA_MEDICAL.Web.Reports.PMI.viewers
{
    public partial class RptPrintTenderNotice : ReportBase
    {
        #region Fields

        bool checkStatus;

        int financialYearId = 0;
        int nameOfWorksId = 0;
        string strZoneId = string.Empty;
        DateTime? fromDate = null;
        DateTime? toDate = null;


        #endregion

        #region Ctor
        public RptPrintTenderNotice()
        {

        }
        #endregion

        #region Page Event

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (TenderNoticeSession != null)
                {
                    int projectId = 0;
                    int financialYearId = 0;
                    string reportFormat = string.Empty;
                    if (Request.QueryString["param1"] != null)
                    {
                        int.TryParse(Request.QueryString["param1"], out projectId);
                    }
                    if (Request.QueryString["param2"] != null)
                    {
                        int.TryParse(Request.QueryString["param2"], out financialYearId);
                    }
                    if (Request.QueryString["param3"] != null)
                    {
                        reportFormat = Request.QueryString["param3"];
                    }
                    ShowReport(projectId, financialYearId, reportFormat);
                }

            }
        }

        private void ShowReport(int projectId, int financialYearId, string reportFormat)
        {


            var projectInfo = (from x in pmiContext.PMI_ProjectMaster
                               where x.Id == projectId
                               select x).SingleOrDefault();

            if (projectInfo == null)
            {
                return;
            }
            //financialYearId = projectInfo.FinancialYearId;
            nameOfWorksId = projectId;
            var zoneIdList = (from x in context.PRM_ZoneInfo
                              select x.Id).ToArray();

            strZoneId = string.Join(",", zoneIdList);
            fromDate = toDate = projectInfo.TenderPubDate;
            GenerateReport(financialYearId, strZoneId, fromDate, toDate, nameOfWorksId, reportFormat);
        }

        #endregion

        #region Generate Report
        public void GenerateReport(int financialYearId, string strZoneId, DateTime? fromDate, DateTime? toDate, int nameOfWorksId, string reportFormat)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data
            
            rvTenderNotice.Reset();
            rvTenderNotice.ProcessingMode = ProcessingMode.Local;
            switch (reportFormat)
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

            fromDate = toDate = null;

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

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            int financialYearId = 0;
            if (Request.QueryString["param2"] != null)
            {
                int.TryParse(Request.QueryString["param2"], out financialYearId);
            }

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

        protected void rvTenderNotice_ReportRefresh(object sender, CancelEventArgs e)
        {
            //btnViewReport_Click(null, null);
        }

    }
}