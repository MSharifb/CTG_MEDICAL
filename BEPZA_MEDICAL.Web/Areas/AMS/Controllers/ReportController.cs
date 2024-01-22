using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Areas.AMS.ViewModel.Report;

namespace BEPZA_MEDICAL.Web.Areas.AMS.Controllers
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

        public ActionResult AnsarDetails()
        {
            _model.ReportPath = Url.Content("~/Reports/AMS/viewers/RptAnsarDetails.aspx");
            return View("ReportViewer", _model);
        }
        
        public ActionResult ZoneWiseAnsarList()
        {
            _model.ReportPath = Url.Content("~/Reports/AMS/viewers/RptZoneWiseAnsarList.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult CategoryWiseAnsarList()
        {
            _model.ReportPath = Url.Content("~/Reports/AMS/viewers/RptCategoryWiseAnsarList.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult ReminderLetter()
        {
            _model.ReportPath = Url.Content("~/Reports/AMS/viewers/RptReminderLetter.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult BlacklistedAnsarList()
        {
            _model.ReportPath = Url.Content("~/Reports/AMS/viewers/RptBlacklistedAnsarList.aspx");
            return View("ReportViewer", _model);
        }
        //public ActionResult ConsolidatedAnsarList()
        //{
        //    _model.ReportPath = Url.Content("~/Reports/AMS/viewers/RptConsolidatedAnsarList.aspx");
        //    return View("ReportViewer", _model);
        //}

        #endregion

    }
}
