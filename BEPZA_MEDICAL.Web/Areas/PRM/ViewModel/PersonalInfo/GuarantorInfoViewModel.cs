using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.PersonalInfo
{
    public class GuarantorInfoViewModel
    {
        #region Standard Property
        public virtual int Id
        {
            get;
            set;
        }

        public virtual string Type
        {
            get;
            set;
        }

        public virtual int EmployeeId
        {
            get;
            set;
        }
        private int _employeeId;

        public virtual Nullable<int> TitleId
        {
            get;
            set;
        }
        private Nullable<int> _titleId;

        public virtual string FirstName
        {
            get;
            set;
        }

        public virtual string MiddleName
        {
            get;
            set;
        }

        public virtual string LastName
        {
            get;
            set;
        }

        public virtual string FullName
        {
            get;
            set;
        }

        public virtual string Relation
        {
            get;
            set;
        }

        public virtual string Email
        {
            get;
            set;
        }

        public virtual string MobileNo
        {
            get;
            set;
        }

        public virtual string HomeAddress
        {
            get;
            set;
        }

        public virtual string HomePhone
        {
            get;
            set;
        }

        public virtual string HomePhoneExt
        {
            get;
            set;
        }

        public virtual string OfficeAddress
        {
            get;
            set;
        }

        public virtual string OfficePhone
        {
            get;
            set;
        }

        public virtual string OfficePhoneExt
        {
            get;
            set;
        }

        public virtual string Designation
        {
            get;
            set;
        }

        public virtual bool IsAddPhoto
        {
            get;
            set;
        }
        #endregion
    }
}