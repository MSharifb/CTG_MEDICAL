using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Areas.SWM.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /SWM/Home/

        public ActionResult Index()
        {
            string url = "";
            string lmsHostServerName = ConfigurationManager.AppSettings["UMS"].ToString();

            url = "http://" + lmsHostServerName;

            return Redirect(url);
        }
        public ActionResult Ums()
        {
            string url = "";
            string lmsHostServerName = ConfigurationManager.AppSettings["UMS"].ToString();

            url = "http://" + lmsHostServerName;

            return Redirect(url);

            //string url = "";
            //string lmsHostServerName = ConfigurationManager.AppSettings["ACC"].ToString();
            //// Acount/LoginQ?userName=xxx&password=xxx&module=xxx
            //var sessionUser = MyAppSession.User;
            //int UserID = 0;
            //string password = "";
            //if (sessionUser != null)
            //{
            //    UserID = sessionUser.UserId;
            //    password = sessionUser.Password;
            //}

            //url = "http://" + lmsHostServerName + "/Account/LoginQ?userName=" + UserID.ToString() + "&password=" + password + "&module=ACC";

            //return Redirect(url);
        }
        public ActionResult ACC()
        {
            string url = "";
            string lmsHostServerName = ConfigurationManager.AppSettings["ACC"].ToString();
            // Acount/LoginQ?userName=xxx&password=xxx&module=xxx
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
                //   MyAppSession.ZoneName = zone.ZoneName;
            }

            url = "http://" + lmsHostServerName + "/Account/LoginQ?userName=" + Username + "&password=" + password + "&module=ACC&ZoneID=" + ZoneID;

            return Redirect(url);
        }
        public ActionResult Bgm()
        {
            //string url = "";
            //string lmsHostServerName = ConfigurationManager.AppSettings["BGM"].ToString();

            //url = "http://" + lmsHostServerName;

            //return Redirect(url);
            return View("NotFound");
        }

        public ActionResult Yts()
        {
            //string url = "";
            //string lmsHostServerName = ConfigurationManager.AppSettings["YTS"].ToString();

            //url = "http://" + lmsHostServerName;

            //return Redirect(url);
            return View("NotFound");
        }

        public ActionResult Unauthorized()
        {
            return View("NotFound");

        }

    }
}