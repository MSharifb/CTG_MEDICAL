
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.SecurityService;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Configuration;
using System.Web.Mvc;


namespace BEPZA_MEDICAL.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class HomeController : Controller
    {
        private UserManagementServiceClient _userAgent;
        private readonly PRMCommonSevice _prmCommonService;

        #region Ctor
        public HomeController(PRMCommonSevice prmCommonService)
        {
            this._userAgent = new UserManagementServiceClient();
            this._prmCommonService = prmCommonService;
        }
        #endregion
        
        [NoCache]

        public ActionResult Index()
        {
            //return View();

            //----------------------------
            // run From IIS
            //----------------------------
            string url = "";
            string lmsHostServerName = ConfigurationManager.AppSettings["PGMServerName"].ToString();
            var sessionUser = MyAppSession.User;
            string userName = "";
            string userPass = "";
            Int32 ZoneId =0;
            if (sessionUser != null)
            {
                //UserID = sessionUser.UserId;
                userPass = sessionUser.Password;
                userName = sessionUser.LoginId;
            }
            if (MyAppSession.ZoneInfoId > 0)
            {
                ZoneId = MyAppSession.ZoneInfoId;
            }

            TempData["userid"] = userName;
            TempData["pass"] = userPass;

            url = "http://" + lmsHostServerName + "/Account/LogOn?uid=" + userName + "&pwd=" + userPass + "&ZoneId=" + ZoneId + "&isInjectingLogon=1";

            return Redirect(url);
        }

        public ActionResult SwitchZone(int id,string zoneSwitchURL)
        {
            MyAppSession.ZoneInfoId = id;
            MyAppSession.ZoneName = _prmCommonService.PRMUnit.ZoneInfoRepository.GetByID(id).ZoneName;
            return Redirect(zoneSwitchURL);
        }

        public ActionResult Unauthorized()
        {
            return View();
        }


    } // End of class Home
}
