using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class SendSMSViewModel
    {

        #region Ctor
        public SendSMSViewModel()
        {
            this.ZoneListByUser = new List<SelectListItem>();
            this.DepartmentList = new List<SelectListItem>();
            this.AllEmployeeList = new List<SelectListItem>();
            this.IsExternal = false;
        }
        #endregion

        #region standard properties
        [DisplayName("Zone")]
        public int? ZoneInfoIdByUser { get; set; }
        public IList<SelectListItem> ZoneListByUser { set; get; }

        [DisplayName("Department")]
        public int? DepartmentId { get; set; }
        public IList<SelectListItem> DepartmentList { set; get; }

        [DisplayName("Employee")]
        public string[] SelectedEmployee{ get; set; }
        public IList<SelectListItem> AllEmployeeList { set; get; }

        public int? EmployeeId { get; set; }

        [Required]
        [StringLength(450, ErrorMessage = "maximum 450 characters.")]
        public string Message { get; set; }
        public DateTime MessageDate { get; set; }

        public string MessageOption { get; set; }       
        public bool IsExternal { get; set; }
        public string ExternalName { get; set; }
        #endregion

    }
}