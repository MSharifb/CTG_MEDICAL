using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Report;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Domain.PMI;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Project;

namespace BEPZA_MEDICAL.Web.Areas.PMI.Controllers
{
    public class ReportController : Controller
    {
        #region Fields
        private readonly ReportViewerViewModel _model;
        private readonly PMICommonService _pmiService;
        #endregion

        #region Ctor
        public ReportController(PMICommonService pmiService)
        {
            _model = new ReportViewerViewModel();
            _pmiService = pmiService;
        }
        #endregion

        #region Actions

        public ActionResult TenderNotice()
        {
            _model.ReportPath = Url.Content("~/Reports/PMI/viewers/RptTenderNotice.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult ProcurementPlan()
        {
            _model.ReportPath = Url.Content("~/Reports/PMI/viewers/RptProcurementPlan.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult FinancialBudget()
        {
            _model.ReportPath = Url.Content("~/Reports/PMI/viewers/RptFinancialBudget.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult WorkWiseBudget()
        {
            _model.ReportPath = Url.Content("~/Reports/PMI/viewers/RptWorkWiseBudget.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult BudgetSummary()
        {
            _model.ReportPath = Url.Content("~/Reports/PMI/viewers/RptBudgetSummary.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult ProjectEstimation()
        {
            _model.ReportPath = Url.Content("~/Reports/PMI/viewers/RptProjectEstimation.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult ProgressReport()
        {
            _model.ReportPath = Url.Content("~/Reports/PMI/viewers/RptProgressReport.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult ZoneWiseBudgetSummary()
        {
            _model.ReportPath = Url.Content("~/Reports/PMI/viewers/RptZoneWiseBudget.aspx");
            return View("ReportViewer", _model);
        }


        public ActionResult BudgetInformation(int? budgetId, int? budgetDetailId, int? financialYearId, int? budgetStatusId)
        {
            var reportUrl = "~/Reports/PMI/viewers/RptPrintBudget.aspx";
            reportUrl += "?param1=" + budgetId;
            reportUrl += "&param2=" + budgetDetailId;
            reportUrl += "&param3=" + financialYearId;
            reportUrl += "&param4=" + budgetStatusId;

            _model.ReportPath = Url.Content(reportUrl);

            //var data = pmiContext.sp_PMI_PrintBudget(budgetId, detailId).ToList();

            var hasData = _pmiService.PMIUnit.FunctionRepository.GetBudgetInfo(budgetId, budgetDetailId, financialYearId, budgetStatusId, "");
            
            if (budgetId != null)
            {
                ReportBase.BudgetInformationSession = reportUrl;
            }
            if (ReportBase.BudgetInformationSession != null)
            {
                _model.ReportPath = ReportBase.BudgetInformationSession;
            }

            return Redirect(_model.ReportPath);
        }

        public ActionResult PrintTenderNotice(string projectId, string financialYearId, string formatName)
        {
            try
            {
                var reportUrl = "~/Reports/PMI/viewers/RptPrintTenderNotice.aspx";
                reportUrl += "?param1=" + projectId;
                reportUrl += "&param2=" + financialYearId;
                reportUrl += "&param3=" + formatName;

                _model.ReportPath = Url.Content(reportUrl);
                if (!String.IsNullOrEmpty(projectId))
                {
                    ReportBase.TenderNoticeSession = reportUrl;
                }

                if (ReportBase.TenderNoticeSession != null)
                {
                    _model.ReportPath = ReportBase.TenderNoticeSession;
                }

                return Redirect(_model.ReportPath);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult PrintProcurementPlan(string projectId, string financialYearId, string projectStatusId)
        {
            try
            {
                var reportUrl = "~/Reports/PMI/viewers/RptPrintProcurementPlan.aspx";
                reportUrl += "?param1=" + projectId;
                reportUrl += "&param2=" + financialYearId;
                reportUrl += "&param3=" + projectStatusId;

                _model.ReportPath = Url.Content(reportUrl);
                if (!String.IsNullOrEmpty(projectId))
                {
                    ReportBase.ProcurementPlanSession = reportUrl;
                }

                if (ReportBase.ProcurementPlanSession != null)
                {
                    _model.ReportPath = ReportBase.ProcurementPlanSession;
                }

                return Redirect(_model.ReportPath);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult PrintBudgetSummary(PrintProjectParamViewModel model)
        {

            try
            {
                var reportUrl = "~/Reports/PMI/viewers/RptBudgetSummary.aspx";
                _model.ReportPath = Url.Content(reportUrl);
                var paramList = model.ParamList;
                ReportBase.BudgetSummaryParamList = paramList;

                return Redirect(_model.ReportPath);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult PrintAnnualProcurementPlan(string appId)
        {
            try
            {
                var reportUrl = "~/Reports/PMI/viewers/RptAnnualProcurementPlan.aspx";
                reportUrl += "?param1=" + appId;
                _model.ReportPath = Url.Content(reportUrl);
                if (!String.IsNullOrEmpty(appId))
                {
                    ReportBase.AnnualProcurementPlanSession = reportUrl;
                }

                if (ReportBase.AnnualProcurementPlanSession != null)
                {
                    _model.ReportPath = ReportBase.AnnualProcurementPlanSession;
                }

                return Redirect(_model.ReportPath);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult AnnualProcurementPlan()
        {
            _model.ReportPath = Url.Content("~/Reports/PMI/viewers/RptAnnualProcurementPlan.aspx");
            return View("ReportViewer", _model);
        }

        #endregion

    }
}
