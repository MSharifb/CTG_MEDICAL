using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalFlow
{
    public class EmployeeSelectViewModel
    {
        public EmployeeSelectViewModel()
        {
            this.ZoneList = new List<SelectListItem>();
            this.EmployeeList = new List<SelectListItem>();
        }

        [DisplayName("Employee Id")]
        public int EmployeeId { get; set; }

        [DisplayName("Name")]
        public string EmployeeName { get; set; }

        [DisplayName("Designation")]
        public string Designation { get; set; }


        #region Other Prop

        [DisplayName("Zone")]
        public int ZoneId { get; set; }

        public IList<SelectListItem> ZoneList { get; set; }

        public IList<SelectListItem> EmployeeList { get; set; }

        #endregion
    }
}