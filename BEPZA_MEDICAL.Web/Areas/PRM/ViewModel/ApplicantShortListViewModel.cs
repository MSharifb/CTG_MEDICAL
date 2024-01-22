using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ApplicantShortListViewModel : BaseViewModel
    {

        #region Ctor
        public ApplicantShortListViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.JobAdvertisementInfoList = new List<SelectListItem>();
            this.ApplicantShortListDetail = new List<ApplicantShortListDetailViewModel>();
            this.ApplicantShortList = new List<ApplicantShortListViewModel>();
            this.CgpaScopeLogicList = new List<SelectListItem>();
            this.ExperienceScopeLogicList = new List<SelectListItem>();
            this.DegreeLevelList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Property

       // [Required]
        [DisplayName("Advertisement Code")]
        public int JobAdvertisementInfoId { get; set; }


       // [Required]
        [DisplayName("Short List Date")]
        [UIHint("_RequiredDate")]
        public DateTime? Date { get; set; }

      //  [Required]
        public int EmployeeId { get; set; }

        public bool IsSubmit { get; set; }

        [UIHint("_Multiline")]
        public string Comments { get; set; }
        #endregion

        #region Attachment

        [Display(Name = "Attachment")]
        public bool IsAddAttachment { set; get; }
        public byte[] Attachment { set; get; }

        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion

        #region Other

      
        public int? ApplicantShortListId { get; set; }      
        public int? ApplicantInfoId { get; set; }
        public string JobAdvertisementCode { get; set; }

        [Display(Name = "Short Listed By")]
        [UIHint("_ReadOnly")]
        public string EmployeeName { get; set; }

        public int? DesignationId { get; set; }

        [Display(Name = "Status Designation")]
        [UIHint("_ReadOnly")]
        public string DesignationName { get; set; }

        public int?  DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public string ApplicantName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }        
        public string DateOfBirth { get; set; }
        public string NID { get; set; }
        public IList<SelectListItem> JobAdvertisementInfoList { set; get; }

        public IList<ApplicantShortListDetailViewModel> ApplicantShortListDetail{ get; set; }
        public IList<ApplicantShortListViewModel> ApplicantShortList { get; set; }

        [DisplayName("GPA/CGPA")]
        public string CgpaScopeLogic { get; set; }
        public IList<SelectListItem> CgpaScopeLogicList { set; get; }
        
        [DisplayName("Total Year of Experience")]
        public string ExperienceScopeLogic { get; set; }
        public IList<SelectListItem> ExperienceScopeLogicList { set; get; }
        public bool IsChecked { get; set; }

        public bool IsCheckedFinal { get; set; }

        [Display(Name = "Degree Level")]
        public string DegreeLevel { get; set; }
        public IList<SelectListItem> DegreeLevelList { set; get; }

        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        public int? NoOfPost { get; set; }
        public string Status { get; set; }
        public decimal YearOfExperience { get; set; }
        public decimal CGPA { get; set; }
        //public bool IsRequiredQualification { get; set; }
        #endregion  

    
 
    }
}