using BEPZA_MEDICAL.Web.Areas.FMS.ViewModel.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.Controllers
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

        #region Actions

        public ActionResult FDRInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptFDRInfo.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult FDRInstallment()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptFDRInstallment.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult FDRBalance()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptFDRBalance.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult RptFDRClosing()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptFDRClosing.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult RptFDRClosingHistory()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptFDRClosingHistory.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult FDRInterestReceivable()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RtpFDRInterestReceivable.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult FDRSchedule()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptFDRSchedule.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult FDRInterestReceivableForCPF()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptFDRInterestReceivableforCPF.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult StatementofFDRReceivableMonthly()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptFDRInterestRecforCPFMonthly.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult MonthlyFDRInstallmentReceivable()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptMonthlyFDRInstallmentReceivable.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult FDRRegister()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptFDRRegister.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult NewFDRforCurrentYear()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptNewFDRforCurrentYear.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EncashedFDRinCurrentYear()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptEncashFDRinCurrentYear.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult InvestmentOnFDR()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptInvestmentOnFDR.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult FDRDailyreport()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptFDRDailyreport.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult FDRinterestRateinformation()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptFDRinterestRateinformation.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult FDRSummary()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptFDRSummary.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult AllInvestmentonFDR()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptAllInvestmentonFDR.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult BankBranchInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/FMS/viewers/RptBankBranchInfo.aspx");
            return View("ReportViewer", _model);
        }
        #endregion
    }
}