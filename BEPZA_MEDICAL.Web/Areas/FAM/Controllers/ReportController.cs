using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.Report;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class ReportController : Controller
    {
        #region Fields
        private readonly ReportViewerViewModel _model;
        #endregion

        #region Ctor
        public ReportController()
        {
            _model = new ReportViewerViewModel();
        }
        #endregion


        public ActionResult ApprovalPath()   
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/ApprovalPath.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult BalanceSheet()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/BalanceSheet.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult IncomeExpenditureSummary()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/IncomeExpenditureSummary.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult IncomeExpenditureDetailsRpt()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/IncomeExpenditureDetailsRpt.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult TrialBalanceRpt() 
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/TrialBalanceRpt.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult AdvanceStaffProject()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/AdvanceStaffProjectRpt.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult IndividualAdvance()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/IndividualAdvance.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult BankBookWithSTD()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/BankBookWithSTD.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult ServiceRevenueReceivable()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/ServiceRevenueReceivable.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult BBSummaryForSingleAccount()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/BBSummaryForSingleAccount.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult GeneralLedger()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/GeneralLedgerReport.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult ProjectWiseIncomeNExpenditure()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/ProjectWiseIncomeNExpenditure.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult ProjectMonitoringSheet()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/ProjectMonitoringSheet.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult VendorLedger()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/VendorLedger.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult IndividualVoucher()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/IndividualVoucherReport.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult DailyTransaction()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/DailyTransactionReport.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult DifferentSeriesProjectSummary()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/DifferentSeriesProjectSummary.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult DifferentSeriesProjectDetails()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/DifferentSeriesProjectDetails.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult DifferentSeriesProjectDuringThisYear()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/DifferentSeriesProjectDuringThisFinancialYear.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult InvoiceListForVoucherPreparation()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/GeneratingInvoiceVoucher.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult CashFlowReceiptPaymentStatement()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/CashFlowStatement.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult IndentAdjustmentListForVoucherPreparation()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/GeneratingIndentAdjustmentVoucher.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult DivisionWiseEarningExpenditureSummary()
        {
            _model.ReportPath = Url.Content("~/Reports/FAM/viewers/EarningVsExpenditureSummary.aspx");
            return View("ReportViewer", _model);
        }

    }
}
