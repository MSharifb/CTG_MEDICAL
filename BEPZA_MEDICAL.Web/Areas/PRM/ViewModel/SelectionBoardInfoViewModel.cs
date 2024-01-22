using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class SelectionBoardInfoViewModel : BaseViewModel
    {
        #region Ctor
        public SelectionBoardInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.JobAdvertisementInfoList = new List<SelectListItem>();
            this.SelectionBoardInfoCommittee = new List<SelectionBoardInfoCommitteeViewModel>();
            this.SelectionBoardInfo = new List<SelectionBoardInfoViewModel>();
        }

        #endregion

        #region Standard Property

        [Required]
        [DisplayName("Committee Name")]
        public string CommitteeName { get; set; }

        [DisplayName("Is Continuous ")]
        public bool IsContinuous { get; set; }

        [Required]
        [DisplayName("Effective From Date")]
        [UIHint("_RequiredDate")]
        public DateTime? EffectiveFromDate { get; set; }


        [DisplayName("Effective To Date")]
        [UIHint("_Date")]
        public DateTime? EffectiveToDate { get; set; }


        [UIHint("_Multiline")]
        public string Comments { get; set; }

        [Required]
        [DisplayName("Advertisement Code")]
        public int JobAdvertisementInfoId { get; set; }

        [DisplayName("Selection Option of Job Position")]
        public bool IsIndividual { get; set; }

        [Required]
        public int ZoneInfoId { get; set; }

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

        public IList<SelectListItem> JobAdvertisementInfoList { set; get; }

        public string JobAdvertisementCode { get; set; }
        public IList<SelectionBoardInfoCommitteeViewModel> SelectionBoardInfoCommittee { get; set; }
        public IList<SelectionBoardInfoViewModel> SelectionBoardInfo { get; set; }


        [DisplayName("Is External")]
        public bool IsExternal { get; set; }

        public int? MemberEmployeeId { get; set; }

        [DisplayName("Employee ID")]
        public string MemberEmpId { get; set; }

        [DisplayName("Member Name")]
        public string MemberName { get; set; }

        [DisplayName("Member Designation")]
        public string MemberDesignation { get; set; }

        [DisplayName("Member Role")]
        public string MemberRole { get; set; }

        [DisplayName("Active Status")]
        public bool ActiveStatus { get; set; }

        [DisplayName("Sort Order")]
        public int? SortOrder { get; set; }


        public bool IsChecked { get; set; }
        public int? DesignationId { get; set; }
        public string DesignationName { get; set; }

        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public int? SectionId { get; set; }
        public string SectionName { get; set; }

        public int? CommitteeId { get; set; }
        public string EffectDateView { get; set; }
        #endregion
    }
}