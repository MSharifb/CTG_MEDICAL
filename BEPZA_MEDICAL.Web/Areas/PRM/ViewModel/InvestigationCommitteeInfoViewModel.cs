using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class InvestigationCommitteeInfoViewModel : BaseViewModel
    {
        #region Ctor
        public InvestigationCommitteeInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.ComplaintNoteSheetList = new List<SelectListItem>();
            this.InvestigationCommitteeInfoMemberInfoList = new List<InvestigationCommitteeInfoMemberInfoViewModel>();
            this.InvestigationCommitteeInfoFormedForList = new List<InvestigationCommitteeInfoFormedForViewModel>();
        }

        #endregion

        #region Standard Property
        [Required]
        public int ZoneInfoId { get; set; }
        [Required]
        [DisplayName("Ref. No.")]
        public string RefNo { get; set; }

        [DisplayName("Is Continuous ")]
        public bool IsContinuous { get; set; }

        [Required]
        [DisplayName("Effective From Date")]
        [UIHint("_RequiredDate")]
        public DateTime? EffectiveFromDate { get; set; }
        
       
        [DisplayName("Effective To Date")]
        [UIHint("_Date")]
        public DateTime? EffectiveToDate { get; set; }

        [Required]
        public int CommitteeFormedEmployeeId { get; set; }


        #region Attachment

        [Display(Name = "Attachment")]
        public bool IsAddAttachment { set; get; }
        public byte[] Attachment { set; get; }

        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion

        #endregion

        #region Others  Property

        #region Member Information

        [DisplayName("Is External")]
        public bool IsExternal { get; set; }

     
        public int? MemberEmployeeId { get; set; }

        [DisplayName("Member ID")]
        public string MemberEmpId { get; set; }

        [DisplayName("Member Name")]
        public string MemberEmployeeName { get; set; }

        [DisplayName("Member Designation")]
        public string MemberEmployeeDesignationName { get; set; }

        [DisplayName("Member Role")]
        public string MemberRole { get; set; }

        [DisplayName("Active Status")]
        public bool ActiveStatus { get; set; }
       
        [DisplayName("Remarks")]
        [UIHint("_MultiLine")]
        public string MemberRemarks { get; set; }
        public string EffectDateView { get; set; }
        #endregion

        #region Committee Formed For
        [DisplayName("Departmental Proceedings No.")]
        public int? ComplaintNoteSheetId { get; set; }
        public string ComplaintNoteSheetName { get; set; }

        [DisplayName("Remarks")]
        [UIHint("_MultiLine")]
        public string Remarks { get; set; }
        public IList<SelectListItem> ComplaintNoteSheetList { set; get; }
        #endregion

        #region Committee Formed By

        [Required]
        [Display(Name = "Employee ID")]
        [UIHint("_ReadOnly")]
        public string CommitteeFormedByEmpId { get; set; }

        [Required]
        [Display(Name = "Explanation Received By")]
        [UIHint("_ReadOnly")]
        public string CommitteeFormedByEmployeeName { get; set; }

        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string CommitteeFormedByDesignationName { get; set; }

        #endregion

        public IList<InvestigationCommitteeInfoMemberInfoViewModel> InvestigationCommitteeInfoMemberInfoList { set; get; }
        public IList<InvestigationCommitteeInfoFormedForViewModel> InvestigationCommitteeInfoFormedForList { set; get; }
        #endregion
    }
}