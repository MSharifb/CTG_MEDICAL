using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class LoginInfo
    {
        private bool bolShowMenus = true;
        public int UserId
        {
            get;
            set;
        }
        public string LoginName
        {
            get;
            set;
        }
        public string EmployeeName
        {
            get;
            set;
        }
      
        public bool IsSuperUser
        {
            get;
            set;
        }

        public string strCompanyID
        {
            get;
            set;
        }
        public float fltOfficeTime
        {
            get;
            set;
        }
        public int intLeaveYearID
        {
            get;
            set;
        }
        
       
        public string strDepartmentID
        {
            get;
            set;
        }
        public string strDesignationID
        {
            get;
            set;
        }
        
        public string strEmpID
        {
            get;
            set;
        }

        public List<string> Archive
        {
            get;
            set;
        }

        public string strSupervisorId
        {
            set;
            get;
        }

        public string strAllowHourlyLeave
        {
            set;
            get;
        }

        public DateTime StartOfficeTime { set; get; }

        public DateTime EndOfficeTime { set; get; }

        public bool ShowMenus
        {
            set { this.bolShowMenus = value; }
            get { return this.bolShowMenus; }
        }

        public string EmailAddress { set; get; }

        public int intCountLeaveYear { set; get; }

        public static LoginInfo Current
        {

            get
            {
                
                if (MyAppSession.LoginInfo == null)
                {
                   // Entity.User user = UserMgtAgent.GetUserByLoginId(HttpContext.Current.User.Identity.Name);

                 //   LMS.BLL.LoginDetailsBLL loginDetailsBll = new BLL.LoginDetailsBLL();
                 
                   // LMSEntity.LoginDetails loginDetails = loginDetailsBll.GetLoginDetailsByEmpAndCompany(user.EmpId, strCompanyId);

                   // if (loginDetails != null || HttpContext.Current.User.Identity.Name.ContainsCaseInsensitive(AppConstant.SysInitializer))
                    {
                        LoginInfo Current = new LoginInfo();

                        try
                        {
                            //Current.strCompanyID = loginDetails.strCompanyID;
                            //Current.LoginName = user.LoginId;
                            //Current.strEmpID = loginDetails.strEmpID;
                            //Current.EmployeeName = loginDetails.strEmpName;
                            //Current.Archive = new System.Collections.Generic.List<string>();
                            //Current.UserId = user.UserId;
                            //Current.strDesignationID = loginDetails.strDesignationID;

                            //Current.strDepartmentID = loginDetails.strDepartmentID;
                            //Current.fltOfficeTime = (float)loginDetails.fltDuration;
                            //Current.intLeaveYearID = loginDetails.intLeaveYearID;
                            //Current.EmailAddress = loginDetails.strEmail;
                            //Current.strSupervisorId = loginDetails.strSupervisorID;

                            //Current.intDestNodeID = loginDetails.intNodeID;
                            //Current.strAllowHourlyLeave = loginDetails.strAllowHourlyLeave;
                            //Current.StartOfficeTime = loginDetails.StartOfficeTime;
                            //Current.EndOfficeTime = loginDetails.EndOfficeTime;  
                            Current.LoginName = "loginId1";

                        }

                        catch (Exception)
                        {

                        }
                        MyAppSession.LoginInfo = Current;
                    }
                }

                return MyAppSession.LoginInfo;
            }
        }

    }
}
