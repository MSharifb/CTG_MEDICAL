using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using BEPZA_MEDICAL.Web.SecurityService;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class CustomMembershipProvider : MembershipProvider
    {
        #region Fields
        private readonly UserManagementServiceClient _userAgent;
        #endregion

        #region Ctor
        public CustomMembershipProvider()
        {
            _userAgent = new UserManagementServiceClient();
        }
        #endregion

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            //User user = UserMgtAgent.GetUserByLoginId(username);


            //if (Common.verifyMd5Hash(oldPassword, user.Password) == false)
            //{
            //    return false;
            //}
            //user.ChangePasswordAtFirstLogin = false;
            //user.Password = Common.getMd5Hash(newPassword);
            //if (UserMgtAgent.UpdateUserData(user) > 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            //User user = UserMgtAgent.GetUserByLoginId(username);

            User user = _userAgent.GetUserByLoginId(username);

            if (Common.verifyMd5Hash(oldPassword, user.Password) == false)
            {
                return false;
            }
            user.ChangePasswordAtFirstLogin = false;
            user.Password = Common.getMd5Hash(newPassword);
            if (_userAgent.UpdateUserData(user) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        //public User GetUser(string username)
        //{
        //    return UserMgtAgent.GetUserByLoginId(username);
        //}

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 3; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            try
            {
                User user = new User();
                user.LoginId = username;
                user.Password = Common.getMd5Hash(password);

                if (_userAgent.GetUserListByCraiteria(user).Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {   
                throw new Exception("Error occured at Security Service. Please check connections.", ex.InnerException);
            }




            //if (username.Equals("admin",StringComparison.OrdinalIgnoreCase) && password=="admin")
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        public bool ValidateUser(string username, string password, out User user)
        {
            user = new User();
            user.LoginId = username;
            user.Password = Common.getMd5Hash(password);

            var userList = _userAgent.GetUserListByCraiteria(user);
            if (userList.Count > 0)
            {
                user = userList.FirstOrDefault();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ValidateZone(string empId, int zoneId )
        {
            var ZoneList = _userAgent.LoadZoneListByEmpID(empId);

            var empZonelist = ZoneList.Where(x => x.EmpId == empId).ToList();

            if (empZonelist.Count > 0)
            {
                if (empZonelist.Count == 1)
                {
                    var sameList = empZonelist.Where(x => x.ZoneId == zoneId).ToList();
                    if (sameList.Count == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Zone> GetZoneList(string empId, int zoneId)
        {
            var ZoneList = _userAgent.LoadZoneListByEmpID(empId);


            

            var empZonelist = ZoneList.Where(x => x.EmpId == empId || x.ZoneId == zoneId).ToList();

            return empZonelist;
        }

        public bool GetZoneNameList(int zoneId, out Zone zone)
        {
            zone = new Zone();
            var ZoneList = _userAgent.GetZoneList();
            zone = ZoneList.Where(x => x.ZoneId == zoneId).FirstOrDefault();
            return true;
        }

        public bool IsVerificationEnable()
        {
            bool isVerificationEnable = false;
            isVerificationEnable = _userAgent.GetAllApplications().Select(x => x.IsVerificationEnable).FirstOrDefault();
            return isVerificationEnable;
        }

        public bool GetValidUserByEncryptedPassword(string username, string password, out User user)
        {
            user = new User();
            user.LoginId = username;
            user.Password = password;
            var userList = _userAgent.GetUserListByCraiteria(user);
            if (userList.Count > 0)
            {
                user = userList.FirstOrDefault();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
