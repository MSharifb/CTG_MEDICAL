using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Notification;
using BEPZA_MEDICAL.Web.Models;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.SecurityService;
using BEPZA_MEDICAL.Web.Utility;
using MyNotificationLib.Operation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using System.Net.NetworkInformation;

namespace BEPZA_MEDICAL.Web.Controllers
{
    public class AccountController : Controller
    {
        #region Fields

        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Ctor
        public AccountController(PRMCommonSevice prmCommonservice)
            : this(null, null)
        {
            this._prmCommonservice = prmCommonservice;
        }

        // This constructor is used by the MVC framework to instantiate the controller using
        // the default forms authentication and membership providers.
        //
        // This constructor is not used by the MVC framework but is instead provided for ease
        // of unit testing this type. See the comments at the end of this file for more
        // information.
        public AccountController(IFormsAuthentication formsAuth, IMembershipService service)
        {

            FormsAuth = formsAuth ?? new FormsAuthenticationService();
            MembershipService = service ?? new AccountMembershipService();
        }
        #endregion

        #region Properties
        public IFormsAuthentication FormsAuth
        {
            get;
            private set;
        }

        public IMembershipService MembershipService
        {
            get;
            private set;
        }
        #endregion

        #region Action Results

        #region Notification Action Results
        public JsonResult GetNotifications()
        {
            int loggedinEmployeeId = _prmCommonservice.PRMUnit
                .EmploymentInfoRepository
                .Get(e => e.EmpID == MyAppSession.EmpId)
                .FirstOrDefault().Id;

            var notificationAlreadyRead = _prmCommonservice.PRMUnit
                .NotificationReadByRepository
                .Get(n => n.EmployeeId == loggedinEmployeeId).Select(s=>s.NotificationId).ToList();

            var notificationFlow = _prmCommonservice.PRMUnit.NotificationFlowRepository.Get(nf => nf.EmployeeId == loggedinEmployeeId).ToList();

            //var notification = (from n in _prmCommonservice.PRMUnit.NotificationRepository.Fetch()
            //                    join nf in notificationFlow on n.Id equals nf.NotificationId
            //                    where !(from nr in notificationAlreadyRead select nr.NotificationId).Contains(n.Id)
            //                    select new
            //                    {
            //                        n.Id,
            //                        n.NotificationDate,
            //                        n.NotificationType,
            //                        n.Message,
            //                        n.RedirectToLink,
            //                        n.IUser,
            //                        nf.Module
            //                    }).ToList();

            var notification = (from n in _prmCommonservice.PRMUnit.NotificationRepository.Fetch()
                                join nf in _prmCommonservice.PRMUnit.NotificationFlowRepository.Fetch() on n.Id equals nf.NotificationId
                                where (nf.EmployeeId == loggedinEmployeeId) && !notificationAlreadyRead.Contains(n.Id)
                                select new
                                {
                                    n.Id,
                                    n.NotificationDate,
                                    n.NotificationType,
                                    n.Message,
                                    n.RedirectToLink,
                                    n.IUser,
                                    nf.Module
                                }).ToList();

            //notification = notification.Where(x => !notificationAlreadyRead.Contains(x.Id)).ToList();

            var emp = (from e in _prmCommonservice.PRMUnit.EmploymentInfoRepository.Fetch()
                       select new
                       {
                           e.FullName,
                           e.EmpID
                       }).ToList();

            var myNotifications = (from n in notification
                                   join e in emp on n.IUser equals e.EmpID
                                   select new NotificationSearchViewModel
                                   {
                                       Id = n.Id,
                                       Module = ((MyNotificationLibEnum.NotificationModule)n.Module).ToString().Replace("_", " "),
                                       NotificationType = ((MyNotificationLibEnum.NotificationType)n.NotificationType).ToString().Replace("_", " "),
                                       NotificationDate = Common.GetDate(n.NotificationDate),
                                       NotifyBy = e.FullName + " (" + e.EmpID + ")",
                                       Message = n.Message,
                                       RedirectLink = n.RedirectToLink
                                   }).ToList();


            return Json(
                new
                {
                    notifications = myNotifications
                }, JsonRequestBehavior.AllowGet
            );

        }

        [HttpPost]
        [NoCache]
        public ActionResult MarkMyNotificationAsRead(int id)
        {
            var model = new NotificationReadByViewModel();

            try
            {
                int loggedinEmployeeId = _prmCommonservice.PRMUnit
                    .EmploymentInfoRepository
                    .Get(e => e.EmpID == MyAppSession.EmpId)
                    .Select(s => s.Id)
                    .FirstOrDefault();

                model.NotificationId = id;
                model.EmployeeId = loggedinEmployeeId;
                var entity = model.ToEntity();

                _prmCommonservice.PRMUnit.NotificationReadByRepository.Add(entity);
                _prmCommonservice.PRMUnit.NotificationReadByRepository.SaveChanges();

                return Json(new
                {
                    Success = 1,
                    Message = ErrorMessages.DeleteSuccessful
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    Success = 0,
                    Message = ErrorMessages.DeleteFailed
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [NoCache]
        public ActionResult RedirectToUrlFromNotification(String redirectUrl)
        {
            string hostServerName = ConfigurationManager.AppSettings["url"].ToString();

            var url = hostServerName + redirectUrl;

            return Json(
                new
                {
                    url = url
                }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Two Factor Authentication

        public void SendCodeViaSMS(string mobile, string code)
        {
            //String sid = "BEPZA"; String user = "BEPZA"; String pass = "25q<0Y73";
            //String URI = "http://sms.sslwireless.com/pushapi/dynamic/server.php";
            //String SMSSenderAndText = string.Empty;



            //if (!string.IsNullOrEmpty(mobile) && !mobile.StartsWith("88"))
            //{
            //    int i = 0;
            //    mobile = "88" + mobile;
            //    // insert
            //    SMSSenderAndText += string.Format("&sms[{0}][0]={1}&sms[{0}][1]={2}&sms[{0}][2]=BEPZA{3}",i, mobile, System.Web.HttpUtility.UrlEncode("Authentication Code :" + code), DateTime.Now.Ticks.ToString());
            //}

            //String myParameters = "user=" + user + "&pass=" + pass + SMSSenderAndText + "&sid=" + sid;
            //using (WebClient wc = new WebClient())
            //{
            //    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            //    string HtmlResult = wc.UploadString(URI, myParameters);
            //}

        }

        public string GenerateUniqueNumber()
        {
            string numbers = "1234567890";

            string characters = numbers;
            int length = 6;
            string otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }
            return otp;
        }

        #endregion

        #region IP & MAC

        protected string GetUser_IP()
        {
            string VisitorsIPAddr = string.Empty;
            //if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            //{
            //    VisitorsIPAddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            //}
            //else if (Request.UserHostAddress.Length != 0)
            //{
            //    VisitorsIPAddr = Request.UserHostAddress;
            //}

            VisitorsIPAddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (VisitorsIPAddr == "" || VisitorsIPAddr == null)
                VisitorsIPAddr = Request.ServerVariables["REMOTE_ADDR"];

            return VisitorsIPAddr;
        }

        //public string GetMACAddress()
        //{
        //    NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        //    String sMacAddress = string.Empty;
        //    foreach (NetworkInterface adapter in nics)
        //    {
        //        if (sMacAddress == String.Empty)// only return MAC Address from first card  
        //        {
        //            IPInterfaceProperties properties = adapter.GetIPProperties();
        //            sMacAddress = adapter.GetPhysicalAddress().ToString();
        //        }
        //    }
        //    return sMacAddress;
        //}
        private string GetMAC()
        {
            string macAddresses = "";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return macAddresses;
        }
        public string DetermineCompName(string IP)
        {
            var computerName = string.Empty;
            IPAddress myIP = IPAddress.Parse(IP);
            IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
            List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
            computerName = compName.Count > 0 ? compName.First() : string.Empty;
            return computerName;


        }
        #endregion

        #region Save User History
        public void SaveUserHistory(int id, int userId, int empId, DateTime date)
        {
            try
            {
                PRM_UserLoginHistory userHistory = new PRM_UserLoginHistory();
                string userName = string.Empty;// DetermineCompName(Request.UserHostName); 
                string myIP = GetUser_IP();
                string sMacAddress = string.Empty;//GetMAC();
                if (id > 0)
                {
                    userHistory.Id = id;
                    userHistory.UserId = userId;
                    userHistory.EmployeeId = empId;
                    userHistory.LoginTime = _prmCommonservice.PRMUnit.UserLoginHistoryRepository.GetByID(id).LoginTime;
                    userHistory.LogoutTime = date;
                    userHistory.UserPCName = userName;
                    userHistory.UserIP = myIP;
                    userHistory.UserMAC = sMacAddress;
                    _prmCommonservice.PRMUnit.UserLoginHistoryRepository.Update(userHistory);
                    _prmCommonservice.PRMUnit.UserLoginHistoryRepository.SaveChanges();
                }
                else
                {
                    userHistory.UserId = userId;
                    userHistory.EmployeeId = empId;
                    userHistory.LoginTime = date;
                    userHistory.UserPCName = userName;
                    userHistory.UserIP = myIP;
                    userHistory.UserMAC = sMacAddress;
                    _prmCommonservice.PRMUnit.UserLoginHistoryRepository.Add(userHistory);
                    _prmCommonservice.PRMUnit.UserLoginHistoryRepository.SaveChanges();
                    MyAppSession.UserHistoryID = userHistory.Id;
                }
            }
            catch
            {

            }
        }
        #endregion

        //
        // GET: /Account/LogOn
        public ActionResult LogOn()
      {
            LogOnModel model = new LogOnModel();
            model.ISMultiZone = false;
            return View(model);
        }

        //
        // POST: /Account/LogOn
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

                var Type = Request.Browser.Type;
                var Name = Request.Browser.Browser;
                var Version = Request.Browser.Version;


                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    var user = new User();
                    var zone = new Zone();

                    MembershipService.ValidateUser(model.UserName, model.Password, out user);

                    if (user.Status)
                    {

                        if (MembershipService.IsVerificationEnable())
                        {
                            if (Request.Cookies["Username"] != null)
                            {
                                var userName = Request.Cookies["Username"].Value;
                                var password = Request.Cookies["Password"].Value;
                                if (userName == model.UserName && password == model.Password)
                                {
                                    model.IsVerified = true;
                                }
                            }

                            if (!model.IsVerified)
                            {
                                MyAppSession.AuthenticationCode = GenerateUniqueNumber();
                                string MobileNo = _prmCommonservice.PRMUnit.EmploymentInfoRepository
                                    .Get(x => x.EmpID == user.EmpId)
                                    .Select(s => s.MobileNo)
                                    .FirstOrDefault();

                                SendCodeViaSMS(MobileNo, MyAppSession.AuthenticationCode);
                                return View("VerifyAccount", model);
                            }
                            else if (model.IsVerified && !string.IsNullOrEmpty(model.TwoFactorCode))
                            {
                                if (MyAppSession.AuthenticationCode == model.TwoFactorCode)
                                {

                                }
                                else
                                {
                                    ModelState.AddModelError("", "Incorrect Authentication Code.");
                                    return View("VerifyAccount", model);
                                }
                            }

                            if (model.RememberMeforVerification)
                            {
                                Response.Cookies["Username"].Value = model.UserName;
                                Response.Cookies["Password"].Value = model.Password;
                            }

                        }

                        #region ZoneInfo

                        var IsMultiZone = MembershipService.ValidateZone(user.EmpId, user.ZoneId);

                        #endregion

                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        MyAppSession.User = user;

                        MyAppSession.UserID = user.UserId;
                        MyAppSession.Password = model.Password;
                        MyAppSession.EmpId = user.EmpId;
                        MyAppSession.EmpName = user.LastName;
                        MyAppSession.EmpDesignation = _prmCommonservice.PRMUnit.EmploymentInfoRepository
                            .Get(x => x.EmpID == user.EmpId)
                            .Select(s => s.PRM_Designation.Name)
                            .FirstOrDefault();
                        MyAppSession.IsMultiZone = IsMultiZone;

                        #region getUser ZoneList

                        var List = MembershipService.GetZoneList(user.EmpId, user.ZoneId);
                        HashSet<int> zoneIDs = new HashSet<int>(List.Select(s => s.ZoneId));

                        var ddlFilterList = (from z in _prmCommonservice.PRMUnit.ZoneInfoRepository.Fetch()
                                             where zoneIDs.Contains(z.Id)
                                             orderby z.SortOrder
                                             select new
                                             {
                                                 z.Id,
                                                 z.ZoneName
                                             }).ToList();


                        //var ddlFilterList = _prmCommonservice.PRMUnit.ZoneInfoRepository.Get(x => zoneIDs.Contains(x.Id)).OrderBy(s => s.SortOrder).ToList();

                        var ZoneList = new List<PRM_ZoneInfo>();
                        foreach (var item in ddlFilterList)
                        {
                            var obj = new PRM_ZoneInfo
                            {
                                Id = item.Id,
                                ZoneName = item.ZoneName,
                            };
                            ZoneList.Add(obj);
                        }
                        model.ZoneList = ZoneList;
                        MyAppSession.SelectedZoneList = ZoneList;

                        #endregion

                        var EmploueeId = _prmCommonservice.PRMUnit.EmploymentInfoRepository
                            .Get(x => x.EmpID == user.EmpId).Select(s => s.Id).FirstOrDefault();

                        SaveUserHistory(0, user.UserId, EmploueeId, DateTime.Now);

                        if (!IsMultiZone)
                        {
                            #region Get Zone Name
                            MembershipService.GetZoneNameList(user.ZoneId, out zone);
                            #endregion

                            MyAppSession.ZoneInfoId = user.ZoneId;
                            MyAppSession.ZoneName = zone.ZoneName;

                            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            model.ISMultiZone = true;
                            return View("ZoneLogOn", model);
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", "The user is inactive.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ZoneLogOn
        public bool ZoneLogOn(int ZoneId)
        {
            #region Get Zone Name
            var zone = new Zone();
            MembershipService.GetZoneNameList(ZoneId, out zone);
            #endregion

            MyAppSession.ZoneInfoId = ZoneId;
            MyAppSession.ZoneName = zone.ZoneName;

            return true;
        }

        //
        // GET: /Account/SwitchEPZ
        public ActionResult SwitchEPZ(LogOnModel model)
        {
            try
            {
                model.ISMultiZone = true;
                model.ZoneInfoId = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == MyAppSession.EmpId).FirstOrDefault().ZoneInfoId;
                var List = MembershipService.GetZoneList(MyAppSession.EmpId, model.ZoneInfoId);
                HashSet<int> zoneIDs = new HashSet<int>(List.Select(s => s.ZoneId));

                var ddlFilterList = (from z in _prmCommonservice.PRMUnit.ZoneInfoRepository.Fetch()
                             where zoneIDs.Contains(z.Id)
                             orderby z.SortOrder
                             select new
                             {
                                 z.Id,
                                 z.ZoneName
                             }).ToList();

                //var ddlFilterList = _prmCommonservice.PRMUnit.ZoneInfoRepository.Get(x => zoneIDs.Contains(x.Id)).OrderBy(s => s.SortOrder).ToList();

                var ZoneList = new List<PRM_ZoneInfo>();
                foreach (var item in ddlFilterList)
                {
                    var obj = new PRM_ZoneInfo
                    {
                        Id = item.Id,
                        ZoneName = item.ZoneName,
                    };
                    ZoneList.Add(obj);
                }
                model.ZoneList = ZoneList;
            }
            catch
            {
                FormsAuthentication.SignOut();

                Session.Abandon();

                return RedirectToAction("LogOn", "Account");

            }
            return View("ZoneLogOn", model);
        }

        //
        // GET: /Account/LogOff
        [NoCache]
        public ActionResult LogOnDashboard(string uid, string pwd)
        {

            LogOnModel model = new LogOnModel();
            model.UserName = uid;
            model.Password = pwd;

            if (ModelState.IsValid)
            {        
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    var user = new User();
                    MembershipService.GetValidUserByEncryptedPassword(model.UserName, model.Password, out user);
                if (user.UserId>0)
                {
                    MyAppSession.User = user;
                    MyAppSession.UserID = user.UserId;
                    MyAppSession.Password = model.Password;

                    if (MyAppSession.ZoneInfoId == 0)
                    {
                        MyAppSession.ZoneInfoId = user.ZoneId;
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
          }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            var employeeId = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == MyAppSession.EmpId).Select(s => s.Id).FirstOrDefault();
            SaveUserHistory(MyAppSession.UserHistoryID, MyAppSession.UserID, employeeId, DateTime.Now);
            Session.Abandon();
            return RedirectToAction("LogOn", "Account");
        }

        //
        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword
        [Authorize]
        public ActionResult ChangePassword()
        {
            ChangePasswordModel model = new ChangePasswordModel();
            //MyAppSession.AuthenticationCode = GenerateUniqueNumber();
            //string mobileNo = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == MyAppSession.EmpId).Select(s => s.MobileNo).FirstOrDefault();
            //if (MembershipService.IsVerificationEnable())
            //{
            //    SendCodeViaSMS(mobileNo, MyAppSession.AuthenticationCode);
            //}

            model.IsVerificationEnable = MembershipService.IsVerificationEnable();
            return View(model);
        }

        //
        // POST: /Account/ChangePassword
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                if (MembershipService.IsVerificationEnable())
                {
                    if (MyAppSession.AuthenticationCode == model.AuthenticationCode)
                    {
                        try
                        {
                            changePasswordSucceeded = MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                        }
                        catch (Exception)
                        {
                            changePasswordSucceeded = false;
                        }

                        if (changePasswordSucceeded)
                        {
                            return RedirectToAction("ChangePasswordSuccess");
                        }
                        else
                        {
                            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect Authentication Code.");
                    }

                }
                else
                {
                    try
                    {
                        changePasswordSucceeded = MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("ChangePasswordSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [Authorize]
        public ActionResult UserManual()
        {
            return View();
        }

        public ActionResult GetUserManual(String fileName)
        {
            String filePath = Server.MapPath("~/Content/UserManual/" + fileName);
            if (System.IO.File.Exists(filePath))
            {
                Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName);
                FileStream fsSource = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return new FileStreamResult(fsSource, "application/pdf");
            }

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        // The FormsAuthentication type is sealed and contains static members, so it is difficult to
        // unit test code that calls its members. The interface and helper class below demonstrate
        // how to create an abstract wrapper around such a type in order to make the AccountController
        // code unit testable.
        public interface IFormsAuthentication
        {
            void SignIn(string userName, bool createPersistentCookie);
            void SignOut();
        }

        public class FormsAuthenticationService : IFormsAuthentication
        {
            public void SignIn(string userName, bool createPersistentCookie)
            {
                FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
            }
            public void SignOut()
            {
                FormsAuthentication.SignOut();
            }
        }

        public interface IMembershipService
        {
            int MinPasswordLength { get; }

            bool ValidateUser(string userName, string password);
            bool ValidateUser(string userName, string password, out User user);
            bool ValidateZone(string empID, int zoneId);
            List<Zone> GetZoneList(string empID, int zoneId);
            MembershipCreateStatus CreateUser(string userName, string password, string email);
            bool ChangePassword(string userName, string oldPassword, string newPassword);
            bool GetZoneNameList(int zoneId, out Zone zone);
            bool IsVerificationEnable();
            bool GetValidUserByEncryptedPassword(string userName, string password, out User user);
        }

        public class AccountMembershipService : IMembershipService
        {
            private CustomMembershipProvider _provider;

            public AccountMembershipService()
                : this(null)
            {
            }

            public AccountMembershipService(CustomMembershipProvider provider)
            {
                _provider = provider ?? new CustomMembershipProvider();
            }

            public int MinPasswordLength
            {
                get
                {
                    return _provider.MinRequiredPasswordLength;
                }
            }

            public bool ValidateUser(string userName, string password)
            {
                return _provider.ValidateUser(userName, password);
            }

            public MembershipCreateStatus CreateUser(string userName, string password, string email)
            {
                MembershipCreateStatus status;
                _provider.CreateUser(userName, password, email, null, null, true, null, out status);
                return status;
            }

            public bool ChangePassword(string userName, string oldPassword, string newPassword)
            {
                return new CustomMembershipProvider().ChangePassword(userName, oldPassword, newPassword);
            }

            public bool ValidateUser(string userName, string password, out User user)
            {
                return _provider.ValidateUser(userName, password, out user);
            }

            public bool ValidateZone(string empID, int zoneId)
            {
                return _provider.ValidateZone(empID, zoneId);
            }

            public List<Zone> GetZoneList(string empID, int zoneId)
            {
                return _provider.GetZoneList(empID, zoneId);
            }

            public bool GetZoneNameList(int zoneId, out Zone zone)
            {
                return _provider.GetZoneNameList(zoneId, out zone);
            }

            public bool IsVerificationEnable()
            {
                return _provider.IsVerificationEnable();
            }
            public bool GetValidUserByEncryptedPassword(string userName, string password, out User user)
            {
                return _provider.GetValidUserByEncryptedPassword(userName, password, out user);
            }
        }
    }
}
