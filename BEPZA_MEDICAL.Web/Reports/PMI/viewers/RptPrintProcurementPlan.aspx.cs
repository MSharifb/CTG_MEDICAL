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
    public partial class RptPrintProcurementPlan : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptPrintProcurementPlan()
        {
            //
        }
        #endregion

        #region Page Event

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (ProcurementPlanSession != null)
                {
                    int projectId = 0;
                    int financialYearId = 0;
                    int projectStatusId = 0;
                    string reportFormat = string.Empty;
                    if (Request.QueryString["param1"] != null)
                    {
                        int.TryParse(Request.QueryString["param1"], out projectId);
                        int.TryParse(Request.QueryString["param2"], out financialYearId);
                        int.TryParse(Request.QueryString["param3"], out projectStatusId);
                    }

                    ShowReport(projectId, financialYearId, projectStatusId);
                }
            }

        }

        #endregion

        #region Button Event

        int financialYearId = 0;
        int nameOfWorksId = 0;
        string strZoneId = string.Empty;
        int projectStatusId = 0;
        DateTime? fromDate = null;
        DateTime? toDate = null;

        private void ShowReport(int projectId, int financialYearId, int projectStatusId)
        {


            var projectInfo = (from x in pmiContext.PMI_ProjectMaster
                               where x.Id == projectId
                               select x).SingleOrDefault();

            if (projectInfo == null)
            {
                return;
            }
            else
            {
                nameOfWorksId = projectInfo.Id;
            }

            //financialYearId = projectInfo.FinancialYearId;

            var zoneIdList = (from x in context.PRM_ZoneInfo
                              select x.Id).ToArray();

            strZoneId = string.Join(",", zoneIdList);
            //fromDate = toDate = projectInfo.TenderPubDate;
            GenerateReport(financialYearId, strZoneId, null, null, nameOfWorksId, projectStatusId);
        }



        #endregion

        #region Generate Report
        public void GenerateReport(int financialYearId, string strZoneId, DateTime? fromDate, DateTime? toDate, int nameOfWorksId, int projectStatusId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvProcurementPlan.Reset();
            rvProcurementPlan.ProcessingMode = ProcessingMode.Local;
            rvProcurementPlan.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptProcurementPlan.rdlc");
            
            var data = pmiContext.sp_PMI_RptProcurementPlan(financialYearId, fromDate, toDate, nameOfWorksId, strZoneId, projectStatusId, numErrorCode, strErrorMsg).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = "Procurement Plan";
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvProcurementPlan.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsProcurementPlan", data);
                rvProcurementPlan.LocalReport.DataSources.Add(dataSource);
                this.rvProcurementPlan.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvProcurementPlan.Reset();
            }
            rvProcurementPlan.DataBind();

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
                var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

                int.TryParse(Request.QueryString["param2"], out financialYearId);
                int.TryParse(Request.QueryString["param3"], out projectStatusId);

                dynamic data = null;
                var ProjectMasterId = 0;
                var dsName = string.Empty;
                if (e.ReportPath != "_ReportHeader")
                {
                    ProjectMasterId = Convert.ToInt32(e.Parameters["ProjectMasterId"].Values[0] == null ? 0 : Convert.ToInt32(e.Parameters["ProjectMasterId"].Values[0]));
                }

                switch (e.ReportPath)
                {
                    case "SubRptProcurementPlanDetails":
                        var procurementDetails = pmiContext.sp_PMI_RptProcurementPlanDetails(financialYearId, fromDate, toDate, nameOfWorksId, strZoneId, projectStatusId, numErrorCode, strErrorMsg).ToList();
                        data = procurementDetails.Where(x => x.ProjectMasterId == ProjectMasterId).ToList();
                        dsName = "dsProcurementPlanDetails";
                        break;

                    case "SubRptProcurementPlanHead":
                        data = pmiContext.sp_PMI_RptProcurementPlan(financialYearId, fromDate, toDate, nameOfWorksId, strZoneId, 0, numErrorCode, strErrorMsg).Where(x => x.Id == ProjectMasterId).ToList();
                        dsName = "dsProcurementPlan";
                        break;

                    case "SubRptProcurementPlanFund":
                        var procurementFund = pmiContext.vwPMIProcurementPlanFundRpt.ToList();
                        data = procurementFund.Where(x => x.ProjectMasterId == ProjectMasterId).ToList();
                        dsName = "dsProcurementPlanFund";
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
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        protected void rvProcurementPlan_ReportRefresh(object sender, CancelEventArgs e)
        {
            if (ProcurementPlanSession != null)
            {
                int projectId = 0;
                int financialYearId = 0;
                int projectStatusId = 0;
                string reportFormat = string.Empty;
                if (Request.QueryString["param1"] != null)
                {
                    int.TryParse(Request.QueryString["param1"], out projectId);
                    int.TryParse(Request.QueryString["param2"], out financialYearId);
                    int.TryParse(Request.QueryString["param3"], out projectStatusId);
                }

                ShowReport(projectId, financialYearId, projectStatusId);
            }
        }

    }
}