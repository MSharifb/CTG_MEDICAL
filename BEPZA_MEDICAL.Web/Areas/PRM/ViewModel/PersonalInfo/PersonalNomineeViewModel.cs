using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class PersonalNomineeViewModel
    {       

        public PersonalNomineeViewModel()
        {
            this.NomineeForList = new List<SelectListItem>();
            EmpTop = new EmpTop();
        }
        public EmpTop EmpTop { get; set; }

        #region Basic informaition

        [DisplayName("Employee ID")]
        [UIHint("_ReadOnly")]
        public string EmpID { get; set; }
        [UIHint("_ReadOnly")]
        public string Name { get; set; }
        [UIHint("_ReadOnly")]
        public string Designation { get; set; }
        [DisplayName("Initial")]
        [UIHint("_ReadOnly")]
        public string EmployeeInitial { get; set; }
        public byte[] Picture { get; set; }

        #endregion   

        public int Id
        {
            get;
            set;
        }
        public int EmployeeId
        {
            get;
            set;
        }
        public int NomineeForId { get; set; }
        public List<SelectListItem> NomineeForList { get; set; }         
        public virtual ICollection<PersonalNomineeDetailsViewModel> PersonalNomineeDetail { get; set; }
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
        public string TotalShare { get; set; }
        public string ActionMode { get; set; }
        public string SideBarClassName { get; set; }
        public string strMode { get; set; }

    }
}