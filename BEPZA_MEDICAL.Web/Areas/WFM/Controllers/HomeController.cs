using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.WFM.Controllers
{
    [NoCache]
    public class HomeController : BaseController
    {
        private readonly PRMCommonSevice _prmCommonService;

        public HomeController(PRMCommonSevice prmCommon)
            : base(prmCommon)
        {
            _prmCommonService = prmCommon;
        }

        // GET: WFM/Home
        [NoCache]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SwitchZone(int id, string zoneSwitchURL)
        {
            MyAppSession.ZoneInfoId = id;
            MyAppSession.ZoneName = _prmCommonService.PRMUnit.ZoneInfoRepository.GetByID(id).ZoneName;
            return Redirect(zoneSwitchURL);
        }

        [NoCache]
        public ActionResult Unauthorized()
        {
            return View();
        }
    }
}