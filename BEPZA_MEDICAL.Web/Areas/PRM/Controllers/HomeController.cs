using System.Web.Mvc;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;


namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    [NoCache]
    public class HomeController : BaseController
    {

        private readonly PRMCommonSevice _prmCommonService;

        public HomeController(PRMCommonSevice prmCommon) : base(prmCommon)
        {
            _prmCommonService = prmCommon;
        }


        [NoCache]
        public ActionResult Index()
        {

            var newView = View("NotificationTemplates/Index");
            // it is important to change area in layout.
            newView.MasterName = "~/Areas/PRM/Views/Shared/_Layout.cshtml";


            return newView;
        }
        public ActionResult SwitchZone(int id, string zoneSwitchURL)
        {
            MyAppSession.ZoneInfoId = id;
            MyAppSession.ZoneName = _prmCommonService.PRMUnit.ZoneInfoRepository.GetByID(id).ZoneName;
            return Redirect(zoneSwitchURL);
        }

        public ActionResult Unauthorized()
        {
            return View();
        }
    }
}
