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
    public partial class RptPrintBudget : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptPrintBudget()
        {

        }


        #endregion

        #region Page Event

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ShowReport();
            }
        }

        #endregion

        #region Button Event

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                checkStatus = false;

                int budgetId = 1;
                int budgetDetailId = 0;

                GenerateReport(budgetId, budgetDetailId);


                if (checkStatus == true)
                {
                    rvWorkWiseBudget.Reset();
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region Generate Report
        public void GenerateReport(int budgetId, int detailId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvWorkWiseBudget.Reset();
            rvWorkWiseBudget.ProcessingMode = ProcessingMode.Local;
            rvWorkWiseBudget.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptPrintBudgetInformation.rdlc");

            var data = pmiContext.sp_PMI_PrintBudget(budgetId, detailId).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                ReportDataSource dataSource = new ReportDataSource();
                searchParameters = "Development/Non-Development Budget";
                dataSource = new ReportDataSource("dsPrintBudget", data);
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvWorkWiseBudget.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion

                rvWorkWiseBudget.LocalReport.DataSources.Add(dataSource);
                this.rvWorkWiseBudget.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvWorkWiseBudget.Reset();
            }
            rvWorkWiseBudget.DataBind();

            #endregion
        }

        public void GenerateFinancialBudgetReport(int financialYearId, int budgetStatusId, string budgetType)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvWorkWiseBudget.Reset();
            rvWorkWiseBudget.ProcessingMode = ProcessingMode.Local;
            rvWorkWiseBudget.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptFinancialBudget.rdlc");
            var fyInfo = (from x in pmiContext.acc_Accounting_Period_Information.Where(t => t.id == financialYearId)
                          select x).FirstOrDefault();

            var data = pmiContext.sp_PMI_RptFinancialBudget(financialYearId, budgetStatusId, budgetType, numErrorCode, strErrorMsg).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = "Development/Non-Development Budget";

                ReportParameter p1 = new ReportParameter("param", searchParameters);
                searchParameters = "Financial Year : " + fyInfo.yearName;
                ReportParameter p2 = new ReportParameter("param1", searchParameters);

                rvWorkWiseBudget.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsFinancialBudget", data);
                rvWorkWiseBudget.LocalReport.DataSources.Add(dataSource);
                this.rvWorkWiseBudget.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvWorkWiseBudget.Reset();
            }
            rvWorkWiseBudget.DataBind();

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
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        protected void rvWorkWiseBudget_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        private void ShowReport()
        {
            int budgetId = 0;
            int budgetDetailId = 0;
            int budgetMasterId = 0;
            int financialYearId = 0;
            int budgetStatusId = 0;

            if (Request.QueryString["param1"] != null)
            {
                int.TryParse(Request.QueryString["param1"], out budgetId);
            }
            if (Request.QueryString["param2"] != null)
            {
                int.TryParse(Request.QueryString["param2"], out budgetDetailId);
            }
            if (Request.QueryString["param3"] != null)
            {
                int.TryParse(Request.QueryString["param3"], out financialYearId);
            }
            if (Request.QueryString["param4"] != null)
            {
                int.TryParse(Request.QueryString["param4"], out budgetStatusId);
            }


            var masterInfo = (from b in base.pmiContext.PMI_BudgetMaster
                              where b.Id == budgetId
                              select b).FirstOrDefault();

            if (masterInfo != null)
            {
                int.TryParse(masterInfo.Id.ToString(), out budgetMasterId);
            }

            if (financialYearId > 0)
            {
                GenerateFinancialBudgetReport(financialYearId, budgetStatusId, masterInfo.BudgetType);
            }
            else
            {
                GenerateReport(budgetMasterId, budgetDetailId);
            }
        }

    }
}