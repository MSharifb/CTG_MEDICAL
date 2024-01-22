using BEPZA_MEDICAL.Web.Areas.WFM.ViewModel.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.WFM.Controllers
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

        public ActionResult ZonewiseWelfareFund()
        {
            _model.ReportPath = Url.Content("~/Reports/WFM/viewers/RptZonewiseWelfareFund.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult SectionwiseWelfareFund()
        {
            _model.ReportPath = Url.Content("~/Reports/WFM/viewers/RptSectionwiseWelfareFund.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult WelfareFundConsolidateReport()
        {
            _model.ReportPath = Url.Content("~/Reports/WFM/viewers/RptWelfareFundConsolidateReport.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult ApprovedApplicantList()
        {
            _model.ReportPath = Url.Content("~/Reports/WFM/viewers/RptApprovedApplicantList.aspx");
            return View("ReportViewer", _model);
        } 
 
        #endregion
    }
}