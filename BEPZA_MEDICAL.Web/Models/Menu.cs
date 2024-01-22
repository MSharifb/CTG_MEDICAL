using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Models
{
    public class Menu
    {
        public Menu() { }

        public Menu(string ModuleName, string MenuName, string ParentMenuName, int MenuId, int ParentMenuId)
        {
            this.ModuleName = ModuleName;
            this.MenuName = MenuName;
            this.ParentMenuName = ParentMenuName;
            this.MenuId = MenuId;
            this.ParentMenuId = ParentMenuId;

        }

        public int ApplicationId { get; set; }

        public string ApplicationName { get; set; }

        public bool IsAddAssigned { get; set; }

        public bool IsAssignedMenu { get; set; }

        public bool IsCancelAssigned { get; set; }

        public bool IsDeleteAssigned { get; set; }

        public bool IsEditAssigned { get; set; }

        public bool IsPrintAssigned { get; set; }

        public string LoginId { get; set; }

        public string MenuCaption { get; set; }

        public string MenuCaptionBng { get; set; }

        public int MenuId { get; set; }

        public string MenuName { get; set; }

        public int ModuleId { get; set; }

        public string ModuleName { get; set; }

        public string PageUrl { get; set; }

        public int ParentMenuId { get; set; }

        public string ParentMenuName { get; set; }

        public int RoleId { get; set; }

        public int SerialNo { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }
    }
}