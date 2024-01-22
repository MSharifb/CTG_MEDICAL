using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel
{
    public class JobExperienceInfoViewModel
    {
        public JobExperienceInfoViewModel()
        {
            this.OrganizationTypeList = new List<SelectListItem>();

            this.FromDate = DateTime.Now;
            this.EndDate = DateTime.Now;
            this.EmpTop = new EmpTop();
        }

        #region Standard Property

        public virtual int Id{ get; set; }

        public virtual int EmployeeId{ get; set; }
        [Required]
        [DisplayName("Name of the Employer")]
        public virtual string Organization1{ get; set; }

        [Required]
        [DisplayName("Type of Service")]
        public virtual int OrganizationTypeId{ get; set; }

        [DisplayName("Period From")]
        [UIHint("_Date")]
        [Required]
        public virtual System.DateTime FromDate{ get; set; }

        [DisplayName("Period To")]
        [UIHint("_Date")]
        [Required]
        public virtual System.DateTime EndDate{ get; set; }

        [MaxLength(200)]
        //[UIHint("_MultiLineBig")]
        public virtual string Responsibility{ get; set; }

        [UIHint("_OnlyNumber")]
        public decimal Duration { get; set; }

        [DisplayName("Address")]
        [MaxLength(200)]
        public virtual string Address { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        #endregion
        public IList<SelectListItem> OrganizationTypeList { get; set; }
        public EmpTop EmpTop { get; set; }
        public virtual Nullable<System.DateTime> InactiveDate { get; set; }

        public string strMode { get; set; }
        public string Message { get; set; }
        public string errClass { get; set; }
        public bool DeleteEnable { get; set; }
        public string ButtonText { get; set; }
        public string SideBarClassName { get; set; }

        public string StrDuration { get; set; }
    }
}