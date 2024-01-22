using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BEPZA_MEDICAL.Web.Models;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class AppConstant
    {
        #region Fields
        private static SecurityService.Menu currentMenu;
        #endregion

        #region Ctor
        public AppConstant()
        {
            if (MyAppSession.CurrentMenu != null)
                currentMenu = MyAppSession.CurrentMenu;
            else
                currentMenu = new SecurityService.Menu();
            
        }
        #endregion

        public static string ProjectName
        {
            get
            {
                string projectName = string.Empty;
                if (System.Configuration.ConfigurationManager.AppSettings["ProjectName"] != null)
                {
                    return System.Configuration.ConfigurationManager.AppSettings["ProjectName"].ToString();
                }
                else
                {
                    projectName = "ERP_BEPZA";
                }
                return projectName;
            }
        }
        public static Int32 PageSize
        {

            get { return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["pageSize"].ToString()); }
        }
        
        public static string ClientName
        {
            get { return "Bangladesh Export Processing Zones Authority"; }
        }

        public static string PRMModuleName
        {
            get { return "PRM"; }
        }

        public static string PIMModuleName
        {
            get { return "PIM"; }
        }

        public static string ApplicationName
        {
            get { return "ERP_BEPZA"; }
        }

        public bool IsAddAssigned
        {
            get
            {
                return MyAppSession.CurrentMenu != null && MyAppSession.CurrentMenu.IsAddAssigned;
            }
        }
       
        public bool IsEditAssigned
        {
            get
            {
                return MyAppSession.CurrentMenu != null && MyAppSession.CurrentMenu.IsEditAssigned;
            }
        }
        public bool IsDeleteAssigned
        {
            get
            {
                return MyAppSession.CurrentMenu != null && MyAppSession.CurrentMenu.IsDeleteAssigned;
            }
        }
        public bool IsCancelAssigned
        {
            get
            {
                return MyAppSession.CurrentMenu != null && MyAppSession.CurrentMenu.IsCancelAssigned;
            }
        }
        public bool IsPrintAssigned
        {
            get
            {
                return MyAppSession.CurrentMenu != null && MyAppSession.CurrentMenu.IsPrintAssigned;
            }
        }

        public bool IsViewAssigned
        {
            get
            {
                return MyAppSession.CurrentMenu != null && MyAppSession.CurrentMenu.IsViewAssigned;
            }
        }

        public static string mailServer
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["smtp"].ToString(); }
        }

        #region constants and class varibles
        private const string _SQLSERVER = "system.data.sqlclient";
        private const string _ORACLE = "system.data.oracleclient";
        private const string _OTHER = "Other";
        #endregion

        public static string ConnectionString
        {
            get { return System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString; }
        }
        public static string Provider
        {
            get
            {
                string strProviderName = "";
                string strProvider = "";

                strProviderName = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ProviderName;

                switch (strProviderName.ToLower())
                {

                    case _SQLSERVER:
                        strProvider = "sqlserver";
                        break;

                    case _ORACLE:
                        strProvider = "oracle";
                        break;

                    case _OTHER:
                        strProvider = "oledb";
                        break;
                }

                return strProvider;

            }
        }
    }
}