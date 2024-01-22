using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Areas.SMS.ViewModel.Report;

namespace BEPZA_MEDICAL.Web.Areas.SMS.Controllers
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

        public ActionResult SecurityPersonnelDetails()
        {
            _model.ReportPath = Url.Content("~/Reports/AMS/viewers/RptSecurityPersonnelDetails.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult ZoneWiseSecurityPersonnelList()
        {
            _model.ReportPath = Url.Content("~/Reports/AMS/viewers/RptZoneWiseSecurityPersonnelList.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult AwardedOrPunishedSecurityPersonnelList()
        {
            _model.ReportPath = Url.Content("~/Reports/AMS/viewers/RptAwardedOrPunishedSecurityPersonnelList.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult OrganizationWiseSecurityPersonnelList()
        {
            _model.ReportPath = Url.Content("~/Reports/AMS/viewers/RptOrganizationWiseSecurityPersonnelList.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult SecurityPersonnelListWorkingInIntelligence()
        {
            _model.ReportPath = Url.Content("~/Reports/AMS/viewers/RptSecurityPersonnelListWorkingInIntelligence.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult BroadSheet()
        {
            _model.ReportPath = Url.Content("~/Reports/AMS/viewers/RptZoneWiseBroadSheet.aspx");
            return View("ReportViewer", _model);
        }
        

        #endregion

    }
}
