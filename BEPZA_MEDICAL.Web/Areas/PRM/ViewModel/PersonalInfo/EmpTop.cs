using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmpTop
    {
        public int EmployeeId { get; set; }
       
        [DisplayName("Employee ID")]
        [UIHint("_ReadOnly")]
        public string EmpID { set; get; }
       
        [UIHint("_ReadOnly")]
        [DisplayName("Initial")]
        public string EmployeeInitial { set; get; }

        [DisplayName("Employee Name")]
        [UIHint("_ReadOnly")]        
        public string Name { set; get; }
        
        [UIHint("_ReadOnly")]
        [DisplayName("Status Designation")]
        public string DesignationTop { set; get; }
        public bool IsPhotoExist { get; set; }
    }
}