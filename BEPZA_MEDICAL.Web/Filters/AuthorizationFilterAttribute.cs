using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.SecurityService;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Filters
{
    public class AuthorizationFilterAttribute : AuthorizeAttribute
    {
        #region Fields
        private string controller;
        private string action;
        private string requestType;
        // private string area;

        #endregion

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var request = httpContext.Request;
            requestType = request.HttpMethod.ToString();
            controller = Convert.ToString(request.RequestContext.RouteData.Values["controller"] ?? request["controller"]);
            action = Convert.ToString(request.RequestContext.RouteData.Values["action"] ?? request["action"]);
            // area = Convert.ToString(request.RequestContext.RouteData.Values["area"] ?? request["area"]);
            int employeeID = 0;

            try
            {
                employeeID = Convert.ToInt32(request.RequestContext.RouteData.Values["Id"] ?? request["Id"]);
            }
            catch { }

            ERP_BEPZAPRMEntities ec = new ERP_BEPZAPRMEntities();

            PRM_EmploymentInfo emp = null;
            string empId = "";
            if (employeeID > 0 && controller.Equals("Employee"))
            {
                emp = ec.PRM_EmploymentInfo.Where(e => e.Id == employeeID).SingleOrDefault();
                empId = emp != null ? emp.EmpID : string.Empty;
            }
            if (employeeID > 0 && controller.Equals("PersonalInfo"))
            {
                emp = ec.PRM_EmploymentInfo.Where(e => e.Id == employeeID).SingleOrDefault();
                empId = emp != null ? emp.EmpID : string.Empty;
            }

            if (controller.Equals("Account"))
            {
                return true;
            }
            var isAuthorized = base.AuthorizeCore(httpContext);

            if (isAuthorized)
            {
                var currentUser = httpContext.User.Identity.Name;

                return ValidatePageUrl(controller, action, currentUser, empId);
            }
            return isAuthorized;
        }

        private bool ValidatePageUrl(string controller, string action, string currentUser, string empId)
        {
            if (controller.Contains("Home") && action.Contains("Index")) return true;
            if (controller.Contains("Home") && action.Contains("Ums")) return true;
            if (controller.Contains("Home") && action.Contains("Bgm")) return true;
            if (controller.Contains("Home") && action.Contains("Yts")) return true;
            if (controller.Contains("Home") && action.Contains("ACC")) return true;
            if (controller.Contains("Home") && action.Contains("SecurityManagement")) return true;
            if (controller.Contains("Home") && action.Contains("FDRType")) return true;
            if (controller.Contains("Home") && action.Contains("ProjectSection")) return true;
            if (controller.Contains("Approval") && action.Contains("Index")) return true;
            if (controller.Contains("ELMAHError") && action.Contains("Index")) return true;
            if (controller.Contains("PersonalInfo") && action.Contains("PersonalNomineeInformationIndex")) return true;
            if (controller.Contains("Employee") && action.Contains("Index")) return true;
            if (controller.Contains("PersonalInfo") && action.Contains("Index")) return true;

            bool flag = true;

            User user = (User)HttpContext.Current.Session["User"];
            //var s = HttpContext.Current.Items[0];
            if (user != null)
            {
                if (currentUser.Equals(user.LoginId) && controller.Equals("Employee") && action.Equals("EmploymentInfoIndex") && (empId == user.EmpId))
                {
                    return true;
                }
                if (currentUser.Equals(user.LoginId) && controller.Equals("PersonalInfo") && action.Equals("PersonaInfoIndex") && (empId == user.EmpId))
                {
                    return true;
                }
            }
            var currentMenus = MyAppSession.CurrentMenus;

            //session out then user compel to signout
            if (currentMenus == null)
            {
                try
                {
                    System.Web.Security.FormsAuthentication.SignOut();
                }
                catch (Exception) { }
                return false;
            }

            var visitingMenu = currentMenus.Find(x => x.PageUrl.ToLower().Contains(controller.ToLower()) && x.PageUrl.ToLower().Contains(action.ToLower()));
            //var visitingMenu = currentMenus.Find(x => x.PageUrl.ToLower().Equals(controller.ToLower()) && x.PageUrl.ToLower().Contains(action.ToLower()));

            if (visitingMenu == null && !action.Contains("Index"))
                visitingMenu = MyAppSession.CurrentMenu;

            if (user != null)
            {
                if (currentUser.Equals(user.LoginId) && controller.Equals("PersonalInfo") && action.Equals("PersonaInfoIndex") && (empId == user.EmpId))
                {
                    return true;
                }
            }

            if (currentMenus != null && action.Contains("Index")) //check view permission
            {
                flag = visitingMenu != null ? visitingMenu.IsAssignedMenu : false;//currentMenus.Exists(x => x.PageUrl.ToLower().Contains(controller.ToLower()) && x.IsAssignedMenu);
            }
            //else if (currentMenus != null && action.Contains("Create") && requestType.Contains("GET")) // check view permission
            //{
            //    flag = visitingMenu != null ? (visitingMenu.IsAssignedMenu | visitingMenu.IsAddAssigned) : false;//currentMenus.Exists(x => x.PageUrl.ToLower().Contains(controller.ToLower()) && x.IsAddAssigned);
            //}
            else if (currentMenus != null && action.Contains("Create") && requestType.Contains("POST")) // check add permission
            {
                flag = visitingMenu != null ? visitingMenu.IsAddAssigned : false;//currentMenus.Exists(x => x.PageUrl.ToLower().Contains(controller.ToLower()) && x.IsAddAssigned);
            }
            else if (currentMenus != null && action.Contains("Edit") && requestType.Contains("POST")) // check update permission
            {
                flag = visitingMenu != null ? visitingMenu.IsEditAssigned : currentMenus.Exists(x => x.PageUrl.ToLower().Contains(controller.ToLower()) && x.IsEditAssigned);
            }
            else if (currentMenus != null && action.Contains("Delete")) // check delete permission
            {
                flag = visitingMenu != null ? visitingMenu.IsDeleteAssigned : currentMenus.Exists(x => x.PageUrl.ToLower().Contains(controller.ToLower()) && x.IsDeleteAssigned);
            }

            if (currentMenus != null &&
                currentMenus.Exists(
                    x =>
                        x.PageUrl.ToLower().Contains(controller.ToLower()) &&
                        x.PageUrl.ToLower().Contains(action.ToLower())))
            {
                MyAppSession.CurrentMenu = visitingMenu;
                //currentMenus.Find(x => x.PageUrl.ToLower().Contains(controller.ToLower()) && x.PageUrl.ToLower().Contains(action.ToLower()));
            }

            return flag;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                filterContext.Result = new RedirectResult("~/Account/LogOn");
            else
            {
                UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);
                filterContext.Result = new RedirectResult(urlHelper.Action("Unauthorized", "Home"));
            }
        }
    }
}