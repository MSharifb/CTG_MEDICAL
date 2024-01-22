using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalFlow
{
    public class AssignApprovalFlowViewModel : BaseViewModel
    {
        public AssignApprovalFlowViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;

            //ApprovalFlowInitialization = new ApprovalFlowDrawViewModel();
            ApprovalFlowInitializationList = new List<ApprovalFlowDrawViewModel>();
        }

        #region Std Prop

        [DisplayName("Zone")]
        public int ZoneId { get; set; }

        [DisplayName("Approval Process")]
        public int ApprovalProcessId { get; set; }

        [DisplayName("Flow Name")]
        public int ApprovalMasterId { get; set; }

        public bool IsApplicableForGroup { get; set; }

        [DisplayName("Employee Id")]
        public int? EmployeeId { get; set; }

        [DisplayName("Designation")]

        public int? DesignationId { get; set; }

        [DisplayName("Employee Category")]
        public int? EmployeeCategory { get; set; }

        [DisplayName("Gender")]
        public string Gender { get; set; }

        [DisplayName("Organogram Level")]
        public int OrganogramLevelId { get; set; }

        public int? InitialStepId { get; set; }

        [DisplayName("Staff Category")]
        public int StaffCategoryId { get; set; }

        #endregion

        #region Other Prop

        [DisplayName("Flow Name")]
        public string FlowName { get; set; }

        [DisplayName("Process Name")]
        public string ProcessName { get; set; }

        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }

        public int? DivisionId { get; set; }

        [DisplayName("Division/Unit")]
        public string DivisionOrUnit { get; set; }

        public int? DepartmentId { get; set; }

        [DisplayName("Department")]
        public string Department { get; set; }

        public int? SectionId { get; set; }

        [DisplayName("Section")]
        public string Section { get; set; }

        [DisplayName("Designation")]
        public string Designation { get; set; }

        [DisplayName("Employee ID")]
        public string EmpId { get; set; }

        public IList<SelectListItem> ZoneList { get; set; }

        public IList<SelectListItem> ApprovalProcessList { get; set; }

        public IList<SelectListItem> ApprovalFlowList { get; set; }

        public IList<SelectListItem> DesignationList { get; set; }

        public IList<SelectListItem> EmployeeCategoryList { get; set; }

        public IList<SelectListItem> GenderList { get; set; }

        [DisplayName("Employee Id & Name")]
        public string EmployeeIdAndName { get; set; }

        [DisplayName("Designation Id")]
        public int? SelectedDesignationId { get; set; }

        public List<ApprovalFlowDrawViewModel> ApprovalFlowInitializationList { get; set; }

        public IList<SelectListItem> StaffCategoryList { get; set; }

        public string OrganogramLevelName { get; set; }

        public string GroupOrIndividual { get; set; }

        public string ZoneName { get; set; }

        [DisplayName("Level Name")]
        public string LevelDetail { get; set; }

        #endregion
    }
}