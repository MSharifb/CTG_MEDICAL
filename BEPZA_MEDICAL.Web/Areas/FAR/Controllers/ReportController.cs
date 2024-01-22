using BEPZA_MEDICAL.Web.Areas.FAR.ViewModel.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.Controllers
{
    public class ReportController : Controller
    {
        #region Fields

        private readonly ReportViewerViewModel model;

        #endregion
        #region Ctor

        public ReportController()
        {
            model = new ReportViewerViewModel();
        }

        #endregion

        public ActionResult LocationWiseAsset()
        {
            model.ReportPath = Url.Content("~/Reports/FAR/viewers/RptLocationWiseAsset.aspx");
            return View("ReportViewer", model);
        }
        public ActionResult FixedAssetRegistration()
        {
            model.ReportPath = Url.Content("~/Reports/FAR/viewers/RptFixedAssetRegistration.aspx");
            return View("ReportViewer", model);
        }

        public ActionResult FixedAssetSchedule()
        {
            model.ReportPath = Url.Content("~/Reports/FAR/viewers/RptFixedAssetSchedule.aspx");
            return View("ReportViewer", model);
        }

        public ActionResult AssetTranferGatePass()
        {
            model.ReportPath = Url.Content("~/Reports/FAR/viewers/RptGatePass.aspx");
            return View("ReportViewer", model);
        }

        public ActionResult AssetTransferHistory()
        {
            model.ReportPath = Url.Content("~/Reports/FAR/viewers/RptAssetTransferHistory.aspx");
            return View("ReportViewer", model);
        }

        public ActionResult AssetRegisterWithDepreciationCal()
        {
            model.ReportPath = Url.Content("~/Reports/FAR/viewers/RptAssetRegisterWithDepreciationCal.aspx");
            return View("ReportViewer", model);
        }
    }
}