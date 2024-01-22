using BEPZA_MEDICAL.Web.SecurityService;
using BEPZA_MEDICAL.Web.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace BEPZA_MEDICAL.Web.Controllers
{
    public class HomeController : Controller
    {

        private UserManagementServiceClient _userAgent;

        public ActionResult Index()
        {         

            ViewBag.Message = "Dashboard";           
            var model = new BEPZA_MEDICAL.Web.Models.Menu();
            model.ModuleName = GetModuleAuthentication();

            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        public string GetModuleAuthentication()
        {
            int UID = 0;
            string moduleName = "";
            _userAgent = new UserManagementServiceClient();
            if (MyAppSession.UserID > 0)
            {
                UID = MyAppSession.UserID;
            }
            var role = _userAgent.GetRolesList();
            var sss1 = _userAgent.GetUserList();
            UserMenu userMenu = new UserMenu();
            userMenu.UserId = UID;
            var userMenuList = _userAgent.GetAllUserMenuList(userMenu);

            var userRole = _userAgent.GetUserRole(UID);

            //var userRoleModule = from rol in role
            //                     join ur in userRole on rol.RoleId equals ur.RoleId
            //                     select rol.ModuleId;

            var userRoleModule = (from ur in userRole
                                 join rol in role on ur.RoleId equals rol.RoleId
                                 select rol.ModuleId).Concat(from um in userMenuList select um.ModuleId);

            var selectedModeles = new List<SecurityService.Module>();

            foreach (var item in userRoleModule)
            {
                var module = _userAgent.GetModuleById(item);

                 var mm = new Module
                 {
                     ModuleName = module.ModuleName,
                     ModuleTitle = module.ModuleTitle,
                     SortOrder= module.SortOrder
                 };
                 selectedModeles.Add(mm);
                //moduleName = moduleName + module.ModuleName + ",";
            }

            MyAppSession.ModuleName = selectedModeles.DistinctBy(x=>x.ModuleName).OrderBy(s=>s.SortOrder).ToList();

            return moduleName;

        }

        //public string GetModuleAuthentication()
        //{
        //    int UID = 0, groupId = 0;
        //    string moduleName = "", roleName = string.Empty;
        //    _userAgent = new UserManagementServiceClient();
        //    if (MyAppSession.User != null)
        //    {              
        //        var sessionUser = MyAppSession.User;
        //        if (sessionUser != null)
        //        {
        //            UID = sessionUser.UserId;
        //            groupId = sessionUser.GroupId;
        //        }
        //    }
        //    var role = _userAgent.GetRolesList();
        //    var sss1 = _userAgent.GetUserList();

        //    var userRole = _userAgent.GetUserRole(UID);

        //    foreach (var item in userRole)
        //    {
        //        var userRoleEntity = _userAgent.GetRoleById(item.RoleId);

        //        roleName = roleName + userRoleEntity.RoleName + ",";
        //    }
        //    var userGroup = _userAgent.GetGroupById(groupId);
        //    if (userGroup != null)
        //    {
        //        MyAppSession.UserGroupName = userGroup.GroupName;
        //    }
        //    MyAppSession.UserRoleNames = roleName;

        //    var userRoleModule = from ur in userRole
        //                         join rol in role on ur.RoleId equals rol.RoleId
        //                         select rol.ModuleId;

        //    foreach (var item in userRoleModule)
        //    {
        //        var module = _userAgent.GetModuleById(item);

        //        moduleName = moduleName + module.ModuleName + ",";
        //    }

        //    return moduleName;
        //}
        

    }
}
