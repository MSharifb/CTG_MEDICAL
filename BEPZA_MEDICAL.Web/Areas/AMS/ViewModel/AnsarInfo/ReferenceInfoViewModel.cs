using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using ERP_BEPZA.Web.Utility;

namespace ERP_BEPZA.Web.Areas.AMS.ViewModel.AnsarInfo
{
    public class ReferenceInfoViewModel
    {
        #region Ctor
        public ReferenceInfoViewModel()
        {
            this.TitleList = new List<SelectListItem>();
            this.EmpTop = new EmpTop();
        }
        #endregion

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

        [DisplayName("Title")]
        public virtual Nullable<int> TitleId
        {
            get;
            set;
        }

        [DisplayName("First Name")]
        [Required]
        public virtual string FirstName
        {
            get;
            set;
        }

        [DisplayName("Middle Name")]
        public virtual string MiddleName
        {
            get;
            set;
        }

        [DisplayName("Last Name")]
        public virtual string LastName
        {
            get;
            set;
        }

        [Required]
        [DisplayName("Full Name")]
        //[StringLength(3)]
        [UIHint("_ReadOnly")]
        public virtual string FullName
        {
            get;
            set;
        }

        [Required]
        [DisplayName("Relation")]
        public virtual string Relation
        {
            get;
            set;
        }

        [DisplayName("Email Address")]
        //[Email]
        [RegularExpression(@"^\w+([-+.]*[\w-]+)*@(\w+([-.]?\w+)){1,}\.\w{2,4}$")]
        public virtual string Email
        {
            get;
            set;
        }

        [DisplayName("Contact No.")]
        [Required]
        public virtual string MobileNo
        {
            get;
            set;
        }

        [Required]
        [DisplayName("Home Address")]
        //[UIHint("_MultiLine")]
        [MaxLength(200)]
        public virtual string HomeAddress
        {
            get;
            set;
        }

        [DisplayName("Home Phone")]
        [UIHint("_Phone")]
        public virtual string HomePhone
        {
            get;
            set;
        }

        [DisplayName("Ext.")]
        [UIHint("_PhoneExt")]
        public virtual string HomePhoneExt
        {
            get;
            set;
        }

        [DisplayName("Office Address")]
        //[UIHint("_MultiLine")]
        [MaxLength(200)]
        public virtual string OfficeAddress
        {
            get;
            set;
        }

        [DisplayName("Office Phone")]
        [UIHint("_Phone")]
        public virtual string OfficePhone
        {
            get;
            set;
        }

        [DisplayName("Ext.")]
        [UIHint("_PhoneExt")]
        public virtual string OfficePhoneExt
        {
            get;
            set;
        }

        public string Organization { get; set; }

        [DisplayName("Designation")]
        public virtual string DesignationRG
        {
            get;
            set;
        }

        [DisplayName("Add Picture")]
        public virtual bool IsAddPhoto
        {
            get;
            set;
        }

        public virtual byte[] Photo
        {
            get;
            set;
        }

        #endregion

        #region Others
        public string ImageUrl { get; set; }
        public string ImageAltText { get; set; }
        public IList<SelectListItem> TitleList
        {
            get;
            set;
        }
        public EmpTop EmpTop { get; set; }
        public string Message { get; set; }
        public string ActionType { get; set; }
        public string ButtonText { get; set; }
        public bool DeleteEnable { get; set; }
        public string SideBarClassName { get; set; }
        public string ErrorClass { get; set; }
        public string SelectedClass { get; set; }
        #endregion
    }
}