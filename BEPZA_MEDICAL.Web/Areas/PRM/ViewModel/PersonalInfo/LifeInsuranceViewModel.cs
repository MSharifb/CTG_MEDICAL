using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.PersonalInfo
{
    public class LifeInsuranceViewModel
    {
        #region Ctor
        public LifeInsuranceViewModel()
        {
            this.EmpTop = new EmpTop();
            this.DateOfDeath = DateTime.Now;
        }
        #endregion

        #region Standard Property

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }

        public string Type { get; set; }
        [DisplayName("Date of Birth")]
        [UIHint("_Date")]
        public string DateOfBirth { get; set; }
        [Required]
        [DisplayName("Date of Death")]
        [UIHint("_Date")]
        public DateTime DateOfDeath { get; set; }
        [DisplayName("Cause of Death")]
        public string CauseOfDeath { get; set; }

        #endregion

        #region Other

        public EmpTop EmpTop { get; set; }

        public string strMode { get; set; }
        public string Message { get; set; }
        public string errClass { get; set; }
        public string SideBarClassName { get; set; }

        public string ActionType { get; set; }
        public string ButtonText { get; set; }
        public bool DeleteEnable { get; set; }
        public string ErrorClass { get; set; }
        public string ControlType { get; set; }
        public string SelectedClass { get; set; }

        #endregion

        #region Attachment

        public byte[] Attachment { set; get; }
        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion

    }
}