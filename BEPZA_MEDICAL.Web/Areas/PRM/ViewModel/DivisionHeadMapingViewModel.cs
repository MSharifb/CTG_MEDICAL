using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class DivisionHeadMapingViewModel : BaseViewModel
    {
        #region Ctor
        public DivisionHeadMapingViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.DesignationList = new List<SelectListItem>();            
        }

        #endregion

        #region Standard Properties
        [Required]
        [DisplayName("Organogram Level")]
        public int OrganogramLevelId { get; set; }

        [Required]
        [DisplayName("Designation")]
        public int DesignationId { get; set; }

        public int? EmployeeId { get; set; }

        [Required]
        public int ZoneInfoId { get; set; }

        #endregion

        #region Other Properties

        [DisplayName("Organogram Level")]
        public string OrganogramLevelName { get; set; }
       
        [DisplayName("Employee ID")]
        public string EmpId { set; get; }


        [DisplayName("Designation")]
        [ReadOnly(true)]
        public virtual string Designation { set; get; }

        public IList<SelectListItem> DesignationList { get; set; }

        [DisplayName("Employee Name")]
        [ReadOnly(true)]
        public string EmployeeName { set; get; }      

        #endregion

    }
}