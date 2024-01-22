using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Models;
//using BEPZA_MEDICAL.Web.SecurityService;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Controllers
{
    public class MenuController : Controller
    {
        #region Fields
        private readonly BEPZA_MEDICAL.Web.SecurityService.UserManagementServiceClient _userAgent;
        #endregion

        #region Ctor
        public MenuController()
        {
            _userAgent = new BEPZA_MEDICAL.Web.SecurityService.UserManagementServiceClient();
        }
        #endregion

        public ActionResult Index(string ModuleName)
        {
            return View();
        }

        public PartialViewResult GetManueList(string ModuleName)
        {
            MenuModels model = new MenuModels();

            //model.MenuList = PopulatePRMMenu();

            PrepareMenu(model.MenuList, ModuleName);

            //model.MenuList = (from r in model.MenuList
            //                  where r.ModuleName == ModuleName
            //                  select r).ToList<Menu>();

            //sorting
            /*var report = model.MenuList[0];
            var setup = model.MenuList[1];
            var task = model.MenuList[2];
            model.MenuList[0] = setup;
            model.MenuList[1] = task;
            model.MenuList[2] = report;
            */
            return PartialView("Menu", model);
        }

        public PartialViewResult GetSiteMapList(string ModuleName)
        {
            MenuModels model = new MenuModels();
            PrepareMenu(model.MenuList, ModuleName);
            return PartialView("SiteMap", model);
        }

        [NonAction]
        private void PrepareMenu(List<Menu> MenuList, string moduleName)
        {
            var allMenus = new List<SecurityService.Menu>();
            var selectedMenus = new List<SecurityService.Menu>();

            //if (_menus != null && _menus.Count == 0)
            //{
            allMenus = _userAgent.GetMenus(User.Identity.Name, AppConstant.ApplicationName, moduleName);

            selectedMenus = allMenus.Where(x => x.IsAssignedMenu).OrderBy(q => q.SerialNo).ToList();

            var tempItem = new List<SecurityService.Menu>();

            foreach (var item in selectedMenus)
            {
                var parentItem = allMenus.Find(x => x.MenuId == item.ParentMenuId);
                if (parentItem != null)
                {
                    tempItem.Add(parentItem);
                    if (parentItem.ParentMenuId != -1)
                    {
                        parentItem = allMenus.Find(x => x.MenuId == parentItem.ParentMenuId);
                        if (parentItem != null)
                            tempItem.Add(parentItem);
                    }
                }
            }
            selectedMenus.AddRange(tempItem.Distinct());

            //MyAppSession.CurrentMenus = selectedMenus;

            _menus = selectedMenus;

            //foreach (var item in selectedMenus)
            //{
            //    var name = item.MenuName;
            //}

            //}
            //else
            //    selectedMenus = _menus;

            foreach (var item in selectedMenus.Where(x => !x.MenuName.ToUpper().Contains("LEFT")).Distinct().ToList())
            {
                string strActionName = "";
                var url = item.PageUrl.Split('/');
                if (url.Count() >= 4)
                {

                    for (int i = 3; i <= url.Count() - 1; i++)
                    {
                        strActionName += url[i] + "/";
                    }
                    var menu = new Menu()
                    {
                        ActionName = strActionName,
                        ControllerName = url[2],
                        MenuName = item.MenuCaption,
                        // ModuleName = AppConstant.PRMModuleName,
                        ModuleName = moduleName,
                        ParentMenuName = item.ParentMenuName,
                        MenuId = item.MenuId,
                        ParentMenuId = item.ParentMenuId,
                        IsAddAssigned = item.IsAddAssigned,
                        IsEditAssigned = item.IsEditAssigned,
                        IsDeleteAssigned = item.IsDeleteAssigned,
                        IsCancelAssigned = item.IsCancelAssigned,
                        IsPrintAssigned = item.IsPrintAssigned
                    };
                    MenuList.Add(menu);
                }
                else
                {
                    var menu = new Menu()
                    {
                        //ActionName = url[2],
                        //ControllerName = url[1],
                        MenuName = item.MenuCaption,
                        ModuleName = AppConstant.PRMModuleName,
                        ParentMenuName = item.ParentMenuName,
                        PageUrl = item.PageUrl,
                        ParentMenuId = item.ParentMenuId,
                        MenuId = item.MenuId,

                        IsAddAssigned = item.IsAddAssigned,
                        IsEditAssigned = item.IsEditAssigned,
                        IsDeleteAssigned = item.IsDeleteAssigned,
                        IsCancelAssigned = item.IsCancelAssigned,
                        IsPrintAssigned = item.IsPrintAssigned
                    };
                    MenuList.Add(menu);
                }
            }
        }

        #region Properties
        private List<BEPZA_MEDICAL.Web.SecurityService.Menu> _menus
        {
            get
            {
                if (MyAppSession.CurrentMenus != null)
                    return MyAppSession.CurrentMenus;
                else
                    return new List<BEPZA_MEDICAL.Web.SecurityService.Menu>();
            }
            set
            {
                MyAppSession.CurrentMenus = value;
            }
        }
        #endregion
    }
}
