using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforOfficer
{
    public class GraphAndRecommendation
    {
        #region Ctor
        public GraphAndRecommendation()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }

        #endregion

        #region Standard 

        public int OfficerInfoId { get; set; }
        public int EmployeeId { get; set; }
        [Display(Name = "A. Special Qualification")]
        public string SpecialQualification { get; set; }
        [Display(Name = "B. Honesty & Goodwill")]
        public string HonestyAndGoodwill { get; set; }
        [Display(Name = "C. Necessity for Training in Particular Section")]
        public string NecessityforTraining { get; set; }

        public string QualificationForPromotion { get; set; }
        [Display(Name = "3. Others Recommendation (If Any)")]
        public string OthersRecommendation { get; set; }
        public int ReportingOfficerId { get; set; }
        [Display(Name = "Report Initiating Officer (RIO)")]
        public string NameOfReportingOfficer { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        [Display(Name="Date")]
        [UIHint("_Date")]
        public DateTime? ReportingDate { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

        public int ApproverId { get; set; }

        [DisplayName("Forward To")]
        [Required]
        public int NextApproverId { get; set; }

        public int ApprovalStatusId { get; set; }

        [DisplayName("Comments")]
        public string ApproverComments { get; set; }

        public IList<SelectListItem> ApproverList { get; set; }

        #endregion

        #region Other

        public string Message { get; set; }
        public string ActionType { get; set; }
        public string ButtonText { get; set; }
        public string SelectedClass { get; set; }
        public bool DeleteEnable { get; set; }
        public string ErrorClass { get; set; }
        public int IsError { get; set; }
        public bool isExist { get; set; }

        public bool A { get; set; }
        public bool B { get; set; }
        public bool C { get; set; }
        public bool D { get; set; }
        #endregion

        #region Officer's Info

        [Display(Name = "Employee ID")]
        public string EmpId { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        [Display(Name = "Designation")]
        public string EmployeeDesignation { get; set; }
        [Display(Name = "Department")]
        public string EmployeeDepartment { get; set; }
        [Display(Name = "Annual Confidential Report Date")]
        public string ACRDate { get; set; }
        [Display(Name = "ACR Period From")]
        public string ACRPeriodFrom { get; set; }
        [Display(Name = "ACR Period To")]
        public string ACRPeriodTo { get; set; }
        [Display(Name = "Seniority Serial Number")]
        public string SeniorityNumber { get; set; }

        #endregion

        #region Attachment

        public byte[] Attachment { set; get; }
        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion

    }
}