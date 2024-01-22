using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.JobAdvertisementInfo
{
    public class JobAdvertisementInfoViewModel : BaseViewModel
    {
        #region Ctor
        public JobAdvertisementInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
            this.AdvertisementDate = DateTime.UtcNow;
            this.AppStartDate = DateTime.UtcNow;

            this.AdvertisementMediaList= new List<SelectListItem>();
            this.ReferenceNoList = new List<SelectListItem>();

            this.JobAdvertisementInfoMedia= new List<JobAdvertisementInfoMediaViewModel>();
            this.JobRequisitionInfoList = new List<JobAdvertisementInfoViewModel>();
            this.JobRequisitionInfoDetailList = new List<JobAdvertisementInfoViewModel>();

            this.DepartmentList = new List<SelectListItem>();
            this.SectionList = new List<SelectListItem>();
            this.DesignationList = new List<SelectListItem>();
            this.EmploymentTypeList = new List<SelectListItem>();
            this.JobAdvertisementPostDetail = new List<JobAdvertisementPostDetailViewModel>();
            this.JobAdvertisementInfoDistricts = new List<JobAdvertisementInfoDistrictsViewModel>();
            //this.JobAdvertisementInfoAttachment = new List<JobAdvertisementInfoAttachmentViewModel>();
        }
        #endregion

        #region Standard Property
        [DisplayName("Advertisement Code")]
        public string AdCode { get; set; }

        [DisplayName("Date")]
        [UIHint("_Date")]
        public DateTime? AdDate { get; set; }

        [DisplayName("Description")]
        public string AdDescription { get; set; }

        [DisplayName("Application Start Date")]
        [UIHint("_RequiredDate")]
        public DateTime AppStartDate { get; set; }

        [DisplayName("Application Submission Last Date")]
        [UIHint("_RequiredDate")]
        public DateTime? AppEndDate { get; set; }

        [DisplayName("Age Calculation on")]
        [UIHint("_RequiredDate")]
        public DateTime? AgeCalDate { get; set; }

        public int ZoneInfoId { get; set; }

        public string Comments { get; set; }
        #endregion

        #region Other

        [Display(Name = "Reference No.")]
        public int? JobRequisitionInfoSummaryId { get; set; }
        public IList<SelectListItem> ReferenceNoList { get; set; }
        public IList<JobAdvertisementInfoMediaViewModel> JobAdvertisementInfoMedia { get; set; }
        public List<JobAdvertisementInfoViewModel> JobRequisitionInfoList { get; set; }
        public List<JobAdvertisementInfoViewModel> JobRequisitionInfoDetailList { get; set; }

        //public IList<JobAdvertisementInfoAttachmentViewModel> JobAdvertisementInfoAttachment { get; set; }

        public IList<JobAdvertisementInfoViewModel> AttachmentFilesList { get; set; }

        public IList<JobAdvertisementPostDetailViewModel> JobAdvertisementPostDetail { get; set; }

        public IList<JobAdvertisementInfoDistrictsViewModel> JobAdvertisementInfoDistricts { get; set; }

        #endregion

        #region Advertisement Info Media

        [DisplayName("Advertisement Media")]
        public int? AdvertisementMediaId { get; set; }
        public IList<SelectListItem> AdvertisementMediaList { get;set;}
        [DisplayName("Advertisement Date")]
        [UIHint("_RequiredDate")]
        public DateTime AdvertisementDate { get; set; }
        public string Notes { get; set; }
        [DisplayName("Advertisement Expire Date")]
        [UIHint("_Date")]
        public DateTime? AdvertisementExpDate { get; set; }

        #endregion

        #region Requisition info

        public bool IsChecked { get; set; }
        public int RequisionId { get; set; }
        public int RequisitionSummaryId { get; set; }
        public int RequisitionInfoClearanceId { get; set; }
        public string RequisitionNo { get; set; }
        public string ReqPreparedBy { get; set; }
        public string Designation { get; set; }
        public string SubmissionDate { get; set; }

        #endregion

        #region Requisition Info Detail

        public int JobRequisitionInfoApprovalDetailId { get; set; }
        
        [Display(Name = "Name of Post")]
        public int? DesignationId { get; set; }
        public bool IsCheckedFinal { get; set; }
        public int NumberOfRequiredPost { get; set; }
        public string PayScale { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        public int? RecommendPost { get; set; }
        public int? ApprovedPost { get; set; }
        public string RequireDate { get; set; }
        public string Category { get; set; }
        public string ReferenceNo { get; set; }

        #endregion

        #region Attachment

        public byte[] Attachment { set; get; }
        public string Title { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion

        #region Post Detail
        public string OrganogramLevelName { get; set; }
        public Int32 OrgLevelId { get; set; }


        //[Display(Name = "Department Name")]
        //public int? DepartmentId { get; set; }
        public IList<SelectListItem> DepartmentList { get; set; }

        //[Display(Name = "Section Name")]
        //public int? SectionId { get; set; }
        public IList<SelectListItem> SectionList { get; set; }


        //[Display(Name = "Job Post Name")]
        //public int? DesignationId { get; set; }
        public IList<SelectListItem> DesignationList { get; set; }
         [Display(Name = "Number of Post")]
        public Int32 NumOfRequiredPost { get; set; }
        [Display(Name = "Employment Type")]
        public int? EmploymentTypeId { get; set; }
        public IList<SelectListItem> EmploymentTypeList { get; set; }

         [DisplayName("Application Fee")]
        public decimal ApplicationFee { get; set; }
       
        [DisplayName("First & Last Step")]
        public string FirstAndLastStep { get; set; }
        
        [Display(Name = "Roll No. Generation Date")]
        [UIHint("_Date")]
        public DateTime RollGenerationDate { get; set; }
        #endregion 

        #region District Info
        [Display(Name = "District")]
        public Int32? DistrictId { get; set; }
        public IList<SelectListItem> DistrictList { get; set; }

        #endregion 
    }

    public class JobAdvertisementPostDetailViewModel : BaseViewModel
    {
        public Int32 JobAdvertisementInfoId { get; set; }
        [Required]
        [DisplayName("No. Of Position")]
        public Int32 NumberOfPosition { get; set; }
        public Int32 OrgLevelId { get; set; }
        public Int32? DepartmentId { get; set; }
        public Int32? SectionId { get; set; }
        [Required]
        [DisplayName("Designation")]
        public Int32 DesignationId { get; set; }
        public decimal SalaryScale { get; set; }
        [Required]
        [DisplayName("Employee Type")]
        public Int32 EmployeeTypeId { get; set; }
        [Required]
        [DisplayName("Application Fee")]
        public decimal ApplicationFee { get; set; }
        public object DesignationName { get; set; }
    }

    public class JobAdvertisementInfoDistrictsViewModel : BaseViewModel
    {
        public Int32 JobAdvertisementInfoId { get; set; }
        [Required]
        public Int32 DistrictId { get; set; }
        [Required]
        [DisplayName("Designation")]
        public Int32 DesignationId { get; set; }

        public string DistrictName { get; set; }

        public string DesignationName { get; set; }
    }
}