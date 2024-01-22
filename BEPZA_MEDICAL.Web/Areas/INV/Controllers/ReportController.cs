using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel.Report;

namespace BEPZA_MEDICAL.Web.Areas.INV.Controllers
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

        public ActionResult PurchaseInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/INV/viewers/RptPurchaseInfo.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult IssueInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/INV/viewers/RptIssueInfo.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult IssueReturnInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/INV/viewers/RptIssueReturnInfo.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult TransferInInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/INV/viewers/RptTransferInInfo.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult TransferOutInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/INV/viewers/RptTransferOutInfo.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult ItemLedger()
        {
            _model.ReportPath = Url.Content("~/Reports/INV/viewers/RptItemLedger.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmployeeWiseItemLedger()
        {
            _model.ReportPath = Url.Content("~/Reports/INV/viewers/RptEmployeeWiseItemLedger.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult StockBalance()
        {
            _model.ReportPath = Url.Content("~/Reports/INV/viewers/RptStockBalance.aspx");
            return View("ReportViewer", _model);
        }

        #endregion

    }
}
