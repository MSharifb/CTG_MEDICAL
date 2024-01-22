using System.Collections.Generic;
using System.Web;
using BEPZA_MEDICAL.DAL.PRM;

namespace BEPZA_MEDICAL.Web.Utility
{
    public static class MyAppSession
    {

        private const string _user = "User";
        private const string _userID = "UserID";
        private const string _userGroupName = "UserGroupName";
        private const string _userRoleNames = "UserRoleNames";
        private const string _password = "Password";
        private const string _empId = "EmpId";
        private const string _empName = "EmpName";
        private const string _empDesignation = "EmpDesignation";


        private const string _zoneInfoId = "ZoneInfoId";
        private const string _zoneName = "ZoneName";
        private const string _isMultiZone = "IsMultiZone";
        private const string _loginInfo = "LoginInfo";

        private const string _loginRank = "LoginRank";
        private const string _currentMenu = "CurrentMenu";
        private const string _currentMenus = "CurrentMenus";
        private const string _selectedZoneList = "SelectedZoneList";
        private const string _displayName = "DisplayName";

        private const string _moduleName = "ModuleName";

        private const string _userHistoryID = "UserHistoryID";

        private const string _authenticationCode = "AuthenticationCode";


        public static BEPZA_MEDICAL.Web.SecurityService.User User
        {
            get { return (BEPZA_MEDICAL.Web.SecurityService.User)HttpContext.Current.Session[_user]; }
            set { HttpContext.Current.Session[_user] = value; }
        }

        public static string UserGroupName
        {
            get { return Common.GetString(HttpContext.Current.Session[_userGroupName]); }
            set { HttpContext.Current.Session[_userGroupName] = value; }
        }

        public static string UserRoleNames
        {
            get { return Common.GetString(HttpContext.Current.Session[_userRoleNames]); }
            set { HttpContext.Current.Session[_userRoleNames] = value; }
        }

        public static int UserID
        {
            get { return Common.GetInteger(HttpContext.Current.Session[_userID]); }
            set { HttpContext.Current.Session[_userID] = value; }
        }

        public static string Password
        {
            get { return Common.GetString(HttpContext.Current.Session[_password]); }
            set { HttpContext.Current.Session[_password] = value; }
        }

        public static string EmpId
        {
            get { return Common.GetString(HttpContext.Current.Session[_empId]); }
            set { HttpContext.Current.Session[_empId] = value; }
        }

        public static string EmpName
        {
            get { return Common.GetString(HttpContext.Current.Session[_empName]); }
            set { HttpContext.Current.Session[_empName] = value; }
        }

        public static string EmpDesignation
        {
            get { return Common.GetString(HttpContext.Current.Session[_empDesignation]); }
            set { HttpContext.Current.Session[_empDesignation] = value; }
        }

        public static int ZoneInfoId
        {
            get { return Common.GetInteger(HttpContext.Current.Session[_zoneInfoId]); }
            set { HttpContext.Current.Session[_zoneInfoId] = value; }
        }

        public static string ZoneName
        {
            get { return Common.GetString(HttpContext.Current.Session[_zoneName]); }
            set { HttpContext.Current.Session[_zoneName] = value; }
        }

        public static bool IsMultiZone
        {
            get { return Common.GetBoolean(HttpContext.Current.Session[_isMultiZone]); }
            set { HttpContext.Current.Session[_isMultiZone] = value; }
        }

        public static LoginInfo LoginInfo
        {
            get { return (LoginInfo)HttpContext.Current.Session[_loginInfo]; }
            set { HttpContext.Current.Session[_loginInfo] = value; }
        }

        public static string LoginRank
        {
            get { return Common.GetString(HttpContext.Current.Session[_loginRank]); }
            set { HttpContext.Current.Session[_loginRank] = value; }
        }

        public static SecurityService.Menu CurrentMenu
        {
            get { return (SecurityService.Menu)HttpContext.Current.Session[_currentMenu]; }
            set { HttpContext.Current.Session[_currentMenu] = value; }
        }

        public static List<SecurityService.Menu> CurrentMenus
        {
            get { return (List<SecurityService.Menu>)HttpContext.Current.Session[_currentMenus]; }
            set { HttpContext.Current.Session[_currentMenus] = value; }
        }

        public static List<PRM_ZoneInfo> SelectedZoneList
        {
            get { return (List<PRM_ZoneInfo>)HttpContext.Current.Session[_selectedZoneList]; }
            set { HttpContext.Current.Session[_selectedZoneList] = value; }
        }

        public static string DisplayName
        {
            get { return Common.GetString(HttpContext.Current.Session[_displayName]); }
            set { HttpContext.Current.Session[_displayName] = value; }
        }
        public static List<SecurityService.Module> ModuleName
        {
            get { return (List<SecurityService.Module>)HttpContext.Current.Session[_moduleName]; }
            set { HttpContext.Current.Session[_moduleName] = value; }
        }
        public static int UserHistoryID
        {
            get { return Common.GetInteger(HttpContext.Current.Session[_userHistoryID]); }
            set { HttpContext.Current.Session[_userHistoryID] = value; }
        }

        public static string AuthenticationCode
        {
            get { return Common.GetString(HttpContext.Current.Session[_authenticationCode]); }
            set { HttpContext.Current.Session[_authenticationCode] = value; }
        }

    }
}