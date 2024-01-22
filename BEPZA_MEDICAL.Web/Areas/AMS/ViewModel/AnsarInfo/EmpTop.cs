using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel
{
    public class EmpTop
    {
        public int EmployeeId { get; set; }

        [DisplayName("Ansar ID")]
        [UIHint("_ReadOnly")]
        public string AnsarId { set; get; }

        [DisplayName("Name")]
        [UIHint("_ReadOnly")]        
        public string Name { set; get; }
        
        [UIHint("_ReadOnly")]
        [DisplayName("Designation")]
        public string DesignationTop { set; get; }
        public bool IsPhotoExist { get; set; }
    }
}