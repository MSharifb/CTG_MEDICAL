using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Notification
{
    public class NotificationFlowSetupViewModel : BaseViewModel
    {
        public NotificationFlowSetupViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
            
        }

        #region Std Prop

        [DisplayName("Notification Type")]
        [Required]
        public int NotificationTypeId { get; set; }

        [DisplayName("Module")]
        public int ModuleId { get; set; }

        public int? EmployeeId { get; set; }

        public bool IsApplicableForGroup { get; set; }

        [DisplayName("Organogram Level")]
        public int? OrganogramLevelId { get; set; }

        public bool OnlyLevelHead { get; set; }

        [DisplayName("Designation")]
        public int? DesignationId { get; set; }
        
        public bool HasReminder { get; set; }
        
        public int ReminderBeforeDays { get; set; }

        #endregion

        #region Other Prop
        
        [Required(ErrorMessage = "Atleast one module selection is required.")]
        public int[] SelectedModuleIds { get; set; }

        [DisplayName("Employee ID")]
        public string EmpId { get; set; }

        [DisplayName("Zone (Filter)")]
        public int? ZoneId { get; set; }

        public string NotificationTypeName { get; set; }
        
        public string ModuleName { get; set; }
        
        public string EmployeeName { get; set; }
        
        public string Designation { get; set; }

        public IList<SelectListItem> ZoneList { get; set; }

        public IList<SelectListItem> NotificationTypeList { get; set; }

        public IList<SelectListItem> ModuleList { get; set; }

        public IList<SelectListItem> DesignationList { get; set; }
        
        public int? SelectedDesignationId { get; set; }

        public int? NotificationTypeIdWhenEdit { get; set; }
        public int? ModuleIdWhenEdit { get; set; }

        public string OrganogramLevelName { get; set; }

        public string GroupOrIndividual { get; set; }

        public string ZoneName { get; set; }

        [DisplayName("Selected Level Name")]
        public string LevelDetail { get; set; }

        #endregion
    }
}