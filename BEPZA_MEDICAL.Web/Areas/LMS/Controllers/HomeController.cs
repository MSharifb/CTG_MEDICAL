using BEPZA_MEDICAL.Web.SecurityService;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.LMS.Controllers
{
    public class HomeController : Controller
    {
        private UserManagementServiceClient _userAgent;
        
        #region Ctor
        public HomeController()
        {
            this._userAgent = new UserManagementServiceClient();
        }
        #endregion

        #region Actions

        public ActionResult Index()
        {
            string url = string.Empty;
            string lmsHostServerName = ConfigurationManager.AppSettings["HostServerName"].ToString();
            var user = _userAgent.GetUserByLoginId(User.Identity.Name);
            var sessionUser = MyAppSession.User;

            string userName = "";
            string userPass = "";
            Int32 ZoneId = 0;
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

            url = "http://" + lmsHostServerName + "/Account/Logon?uid=" + userName + "&pwd=" + Uri.EscapeDataString(userPass) + "&ZoneId=" + ZoneId;

            return Redirect(url);
        }

        public ActionResult Unauthorized()
        {
            return View();

        }

        #endregion

    }
}
