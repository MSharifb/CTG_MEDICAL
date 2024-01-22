using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.SecurityService;
using System.Configuration;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class HomeController : Controller
    {
        private UserManagementServiceClient _userAgent;

        #region Actions
        public HomeController()
        {
            this._userAgent = new UserManagementServiceClient();
        }

        public ActionResult Index()
        {
            string url = "";
            string fmHostServerName = ConfigurationManager.AppSettings["AccountsHostServerName"].ToString();
            var user = _userAgent.GetUserByLoginId(User.Identity.Name);
            string userName = user.LoginId;
            string userPass = MyAppSession.Password;

            TempData["userid"] = userName;
            TempData["pass"] = userPass;

            //  url = "localhost:8088/login.jsp?user=admin&pass=admin ";

            url = "http://" + fmHostServerName + "/login.jsp?user=" + userName + "&pass=" + userPass;

            return Redirect(url);
        }

        public ActionResult Unauthorized()
        {
            return View();
            
        }
        
        #endregion

        //public JsonResult GetModuleAuthentication()
        //{ 
        //    this._userAgent = new UserManagementServiceClient();
        //    var uaa = User.Identity.Name;
        //    var role = _userAgent.GetRolesList();
        //    var sss1 = _userAgent.GetUserList();
            
        //    var userRole = _userAgent.GetUserRole(9);

        //    var userRoleModule = from rol in role
        //                         join ur in userRole on rol.RoleId equals ur.RoleId
        //                         select rol.ModuleId;

        //    var allow = 0;
        //    foreach (var item in userRoleModule)
        //    {
        //        var module = _userAgent.GetModuleById(item);
        //        if (module.ModuleName == "FAM")
        //        {
        //            allow = 1;
        //        }
        //    }

        //   // return Json(allow, JsonRequestBehavior.AllowGet);
        //    return Json(new { allow});

        //    //var userRoleList = role.Where(x=>x.RoleId==userRole.RoleId).SingleOrDefault().ModuleId;

        //}

    }
}
