using BEPZA_MEDICAL.Web.SecurityService;
using BEPZA_MEDICAL.Web.Utility;
using System.Configuration;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.LAMS.Controllers
{
    public class HomeController : Controller
    {
        private UserManagementServiceClient _userAgent;

        public HomeController()
        {
            this._userAgent = new UserManagementServiceClient();
        }

        // GET: LAM/Home
        public ActionResult Index()
        {
            string url = "";
            string lmsHostServerName = ConfigurationManager.AppSettings["LAServerName"].ToString();
            //var user = _userAgent.GetUserByLoginId(User.Identity.Name);

            #region OLD LAMS

            //string userName = user.LoginId;
            //string userPass = MyAppSession.Password;
            //TempData["userid"] = userName;
            //TempData["pass"] = userPass;
            ////url = "http://" + lmsHostServerName + "/Account/SwitchLogOn?userName=" + userName + "&password=" + userPass + "&IsPf=0";

            //return Redirect(url);

            #endregion

            #region NEW LAMS

            //string password = MyAppSession.Password;
            //string Username = user.LoginId;
            //string ZoneID = MyAppSession.ZoneInfoId.ToString();

            //url = "http://" + lmsHostServerName + "/Account/LoginQ?userName=" + Username + "&password=" + password + "&module=LAMS&ZoneID=" + ZoneID + "&IsPf=0";

            //return Redirect(url);

            #endregion

            var sessionUser = MyAppSession.User;
            int UserID = 0;
            string password = "";
            string Username = "";
            string ZoneID = "";
            if (sessionUser != null)
            {
                UserID = sessionUser.UserId;
                password = sessionUser.Password;
                Username = sessionUser.LoginId;
            }
            if (MyAppSession.ZoneInfoId > 0)
            {
                ZoneID = MyAppSession.ZoneInfoId.ToString();
            }

            url = "http://" + lmsHostServerName + "/Account/LoginQ?userName=" + Username + "&password=" + password + "&module=LAMS&ZoneID=" + ZoneID + "&IsPf=0";

            return Redirect(url);
        }

        public ActionResult Unauthorized()
        {
            return View();

        }

    }
}