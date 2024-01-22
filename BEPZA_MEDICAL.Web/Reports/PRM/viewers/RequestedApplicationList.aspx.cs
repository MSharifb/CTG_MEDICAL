using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class RequestedApplicationList : ReportBase
    {
        #region Page Event

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ShowReport();
            }
        }

        #endregion

        #region Generate Report
        public void GenerateReport(int? approvalProcessId, int? approvalStatusId, DateTime? startDate, DateTime? endDate)
        {
            //var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            //var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var loginUser = string.Empty;

            #region Processing Report Data

            rvApplicationList.Reset();
            rvApplicationList.ProcessingMode = ProcessingMode.Local;
            rvApplicationList.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RequestedApplicationList.rdlc");

            string approvalProcessName = prmContext.APV_ApprovalProcess.Where(s => s.Id == approvalProcessId).Select(x => x.ProcessNameEnum).FirstOrDefault();
            var data = prmContext.APV_GetWelfareFundApplication(MyAppSession.EmpId, approvalProcessId, startDate, endDate, approvalStatusId).ToList();
            //dynamic data = 0;
            //switch (approvalProcessName)
            //{
            //    case "WFM":
            //        data = prmContext.APV_GetWelfareFundApplicationAlternateProcess("0002", approvalProcessId, startDate, endDate, approvalStatusId);
            //        break;

            //}

            //if (data.Count() > 0)
            //{
                #region Search parameter

                //string searchParameters = string.Empty;
                ReportDataSource dataSource = new ReportDataSource();
                //searchParameters = "Development/Non-Development Budget";
                dataSource = new ReportDataSource("dsApplicationList", data);
                //ReportParameter p1 = new ReportParameter("param", searchParameters);
                //rvApplicationList.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion

                rvApplicationList.LocalReport.DataSources.Add(dataSource);
                this.rvApplicationList.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            //}
            //else
            //{
            //    rvApplicationList.Reset();
            //}
            rvApplicationList.DataBind();

            #endregion
        }
        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                dynamic data = null;
                var dsName = string.Empty;
                switch (e.ReportPath)
                {
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
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion


        private void ShowReport()
        {
            int approvalProcessId = 0;
            int approvalStatusId = 0;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;

            if (Request.QueryString["param1"] != null)
            {
                int.TryParse(Request.QueryString["param1"], out approvalProcessId);
            }
            if (Request.QueryString["param2"] != null)
            {
                int.TryParse(Request.QueryString["param2"], out approvalStatusId);
            }
            if (Request.QueryString["param3"] != null)
            {
                DateTime.TryParse(Request.QueryString["param3"], out startDate);
            }
            if (Request.QueryString["param4"] != null)
            {
                DateTime.TryParse(Request.QueryString["param4"], out endDate);
            }

            GenerateReport(approvalProcessId, approvalStatusId, startDate, endDate);
        }

        protected void rvApplicationList_ReportRefresh(object sender, CancelEventArgs e)
        {
        }
    }
}