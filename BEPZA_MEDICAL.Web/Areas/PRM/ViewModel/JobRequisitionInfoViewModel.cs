using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class JobRequisitionInfoViewModel : BaseViewModel
    {
        #region Ctor
        public JobRequisitionInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.strMode = "Create";

            this.FinancialYearList = new List<SelectListItem>();
            this.DepartmentList = new List<SelectListItem>();
            this.SectionList = new List<SelectListItem>();
            this.DesignationList = new List<SelectListItem>();
            this.EmploymentTypeList = new List<SelectListItem>();
            this.SalaryScaleList = new List<SelectListItem>();

            this.JobRequisitionInfoDetail= new List<JobRequisitionInfoDetailsViewModel>();
            this.RequireDate = DateTime.UtcNow; 
        }
        #endregion

        #region Standard Property
        [Required]
        [Display(Name="Requisition No.")]
        public string RequisitionNo { get; set; }
        [Required]
        [Display(Name="Requisition Submission Date")]
        [UIHint("_Date")]
        public DateTime? RequisitionSubDate { get; set; }
        public int PreparedById { get; set; }
        [Required]
        [Display(Name="Financial Year")]
        public int FinancialYearId { get; set; }
        public string FinancialYear { get; set; }
        public IList<SelectListItem> FinancialYearList { get; set; }

        [Required]
        public int ZoneInfoId { get; set; }
      
        [Display(Name = "Department Name")]
        public int? DepartmentId { get; set; }
        public IList<SelectListItem> DepartmentList { get; set; }

        [Display(Name = "Section Name")]
        public int? SectionId { get; set; }
        public IList<SelectListItem> SectionList { get; set; }

      
        [Display(Name = "Job Post Name")]
        public int? DesignationId { get; set; }
        public IList<SelectListItem> DesignationList { get; set; }

        [Display(Name = "Salary Scale")]
        public int SalaryScaleId { get; set; }
        public IList<SelectListItem> SalaryScaleList { get; set; }

        [Display(Name = "Require Date")]
        [UIHint("_Date")]
        public DateTime RequireDate { get; set; }

        [Display(Name = "Employment Type")]
        public int? EmploymentTypeId { get; set; }
        public IList<SelectListItem> EmploymentTypeList { get; set; }

        [Display(Name = "Number of Required Post")]
        public int? NumOfRequiredPost { get; set; }

        [Display(Name = "Job Description")]
        public string JobDescription { get; set; }
        [Display(Name = "Educational Requirement")]
        public string EduRequirement { get; set; }
        [Display(Name = "Experience Requirement")]
        public string ExpRequirement { get; set; }
        [Display(Name = "Additional Job Requirement")]
        public string AdditionalRequirement { get; set; }
        public string Comments { get; set; }

        #endregion

        #region Other

        [Required]
        [Display(Name = "Requisition Prepared By")]
        public string RequisitionPreparedBy { get; set; }
        [Required]
        [Display(Name = "Status Designation")]
        public string Designation { get; set; }
        [Display(Name="Office Name")]
        public int? OfficeId { get; set; }
        public IList<SelectListItem> OfficeList { get; set; }


        public IList<JobRequisitionInfoDetailsViewModel> JobRequisitionInfoDetail { get; set; }

        [Display(Name = "Total Sanctioned Post")]
        public int? TotalSanctionedPost { get; set; }
        [Display(Name="Number Of Post for Direct Recruitment")]
        public int? DiRecSancPost { get; set; }
        [Display(Name="Number of Post for Promotion")]
        public int? PromSancPost { get; set; }

        [Display(Name = "Total Filled Up Post")]
        public int? TotalFilledUpPost { get; set; }
        [Display(Name = "Number of Filled Up Post By Direct Recruitment")]
        public int? DiRecFillPost { get; set; }
        [Display(Name = "Number of Filled Up Post By Promotion")]
        public int? PromFillPost { get; set; }

        [Display(Name="Total Vacant Post")]
        public int? TotalVacantPost { get; set; }
        [Display(Name = "Number of Vacant Post For Direct Recruitment")]
        public int? DiRecVacPost { get; set; }
        [Display(Name = "Number of  Vacant Post For Direct Promotion")]
        public int? PromVacPost { get; set; }
        public string FirstAndLastStep { get; set; }

        public bool? NewRec { get; set; }
        public bool? Promotion { get; set; }
        public decimal? Percentage { get; set; }
        #endregion

        #region Attachment
        public byte[] Attachment { set; get; }

        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        #endregion

        #region Organogram

        public int OrganogramLevelId { get; set; }
        [Display(Name = "Select Organogram Level")]
        [UIHint("_ReadOnly")]
        public string OrganogramLevelName { get; set; }
        [Display(Name = "...")]
        public string OrganogramLevelDetail { get; set; }

        #endregion
    }
}