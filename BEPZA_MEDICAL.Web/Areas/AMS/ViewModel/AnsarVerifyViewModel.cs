using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel
{
    public class AnsarVerifyViewModel : BaseViewModel
    {
        #region Ctor
        public AnsarVerifyViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
            this.VerifyDate = DateTime.UtcNow;
            this.FromDate = DateTime.UtcNow;
            this.EndDate = DateTime.UtcNow;

            this.ZoneList = new List<SelectListItem>();
            this.DesignationList = new List<SelectListItem>();
            this.AnsarList = new List<AnsarVerifyViewModel>();
        }
        #endregion

        #region Standard Property
        [DisplayName("Executive Office/Zone")]
        public int AnsarZoneInfoId { get; set; }

        [DisplayName("Verify Date")]
        [UIHint("_Date")]
        public DateTime? VerifyDate { get; set; }

        public int VerifiedById { get; set; }

        public string Comments { get; set; }

        public int ZoneInfoId { get; set; }

        public string Status { get; set; }

        [DisplayName("Date of Joining From")]
        [UIHint("_Date")]
        [Required]
        public virtual System.DateTime FromDate { get; set; }

        [DisplayName("To")]
        [UIHint("_Date")]
        [Required]
        public virtual System.DateTime EndDate { get; set; }
        #endregion

        #region Other

        [DisplayName("Designation")]
        public int? DesignationId { get; set; }
        
        [DisplayName("Ansar Name")]
        public string AnsarName { get; set; }

        [DisplayName("BEPZA ID")]
        public string BEPZAID { get; set; }

        [DisplayName("Ansar ID")]
        public string AnsarId { get; set; }



        public IList<SelectListItem> ZoneList { get; set; }
        public IList<SelectListItem> DesignationList { get; set; }

        public List<AnsarVerifyViewModel> AnsarList { get; set; }

        [DisplayName("Name")]
        public string VerifiedByName { get; set; }
        [DisplayName("Designation")]
        public string VerifiedByDesignation { get; set; }
        [DisplayName("Department")]
        public string VerifiedByDepartment { get; set; }

        public string Subject { get; set; }
        #endregion

        #region Detail

        public string EmpId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsCheckedFinal { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string ApplieedAmount { get; set; }
        public string AppliedDate { get; set; }
        public DateTime DBAppliedDate { get; set; }

        [DisplayName("Date of Joining in BEPZA")]
        public DateTime? DateofJoining { get; set; }

        [DisplayName("Date of Joining in Ansar")]
        public DateTime? AnsarJoiningDate { get; set; }

        #endregion

    }
}