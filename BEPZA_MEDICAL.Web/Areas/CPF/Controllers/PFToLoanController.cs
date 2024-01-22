using BEPZA_MEDICAL.Web.SecurityService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class PFToLoanController : Controller
    {
        private UserManagementServiceClient _userAgent;

        public PFToLoanController()
        {
            this._userAgent = new UserManagementServiceClient();
        }
        // GET: CPF/PFToLoan1
        public ActionResult Index()
        {
            string url = "";
            string lmsHostServerName = ConfigurationManager.AppSettings["LAServerName"].ToString();
            var user = _userAgent.GetUserByLoginId(User.Identity.Name);
            string userName = user.LoginId;
            string userPass = MyAppSession.Password;

            TempData["userid"] = userName;
            TempData["pass"] = userPass;

            //  url = "http://" + lmsHostServerName + "/LMS/Account/Logon";

            url = "http://" + lmsHostServerName + "/Account/SwitchLogOn?userName=" + userName + "&password=" + userPass + "&IsPf=1";

            return Redirect(url);
        }

        public ActionResult Unauthorized()
        {
            return View();

        }

    }
}