using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data.Objects.DataClasses;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class JobExperienceInfoViewModel
    {
        public JobExperienceInfoViewModel()
        {
            this.EmployeeTypeList = new List<SelectListItem>();
            this.OrganizationTypeList = new List<SelectListItem>();
            this.JobGradeList = new List<SelectListItem>();           
            this.EmpTop = new EmpTop();

            this.SecurityOrganizationList = new List<SelectListItem>();
        }

        #region Standard Property

        public virtual int Id{ get; set; }

        public virtual int EmployeeId{ get; set; }
        [Required]
        [MaxLength(100)]
        [DisplayName("Organization/Company")]
        public virtual string Organization1{ get; set; }

        [Required]
        [MaxLength(100)]
        [DisplayName("Position")]
        public virtual string Designation{ get; set; }

        [Required]
        [DisplayName("Employment Type")]
        public virtual int EmploymentTypeId{ get; set; }

        [Required]
        [DisplayName("Type of Service")]
        public virtual int OrganizationTypeId{ get; set; }

        [DisplayName("Period From")]
        [UIHint("_RequiredDate")]
        [Required]
        public virtual DateTime? FromDate{ get; set; }

        [DisplayName("Period To")]
        [UIHint("_RequiredDate")]
        [Required]
        public virtual DateTime? EndDate{ get; set; }

        [MaxLength(200)]
        //[UIHint("_MultiLineBig")]
        public virtual string Responsibility{ get; set; }

        [MaxLength(200)]
        //[UIHint("_MultiLineBig")]
        [DisplayName("Expertise Area")]
        public virtual string Achievement{ get; set; }

        [DisplayName("Is Internal Experience?")]
        public virtual bool isInternalExperience{ get; set; }

        [UIHint("_OnlyNumber")]
        public decimal Duration { get; set; }

        [DisplayName("Is Proper Channel Documentation?")]
        public virtual bool IsProperChannelDocumentation{ get; set; }

        [DisplayName("Is Counting Service Period In Present Organization?")]
        public virtual bool IsCountingServicePeriodInPresentOrganization { get; set; }

        [DisplayName("Grade")]
        public int? JobGradeId { get; set; }
       
        [UIHint("_OnlyNumber")]
        public decimal? Salary { get; set; }

        [DisplayName("Address")]
        [MaxLength(200)]
        public virtual string Address { get; set; }

        public int? SecurityOrganizationId { get; set; }

        #endregion

        public IList<SelectListItem> EmployeeTypeList { get; set; }
        public IList<SelectListItem> OrganizationTypeList { get; set; }
        public IList<SelectListItem> JobGradeList { get; set; }
        public EmpTop EmpTop { get; set; }

        public string strMode { get; set; }
        public string Message { get; set; }
        public string errClass { get; set; }
        public bool DeleteEnable { get; set; }
        public string ButtonText { get; set; }
        public string SideBarClassName { get; set; }

        public string StrDuration { get; set; }
        public IList<SelectListItem> SecurityOrganizationList { get; set; }
    }
}