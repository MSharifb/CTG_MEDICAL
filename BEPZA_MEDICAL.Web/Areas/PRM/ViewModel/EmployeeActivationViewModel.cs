using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BEPZA_MEDICAL.Web.Utility;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmployeeActivationViewModel
    {
        public EmployeeActivationViewModel()
        {
            this.ActivationDate = DateTime.Now;
        }

        public int Id { set; get; }

        [Required] 
        public int EmployeeId { set; get; }

        [DisplayName("Division/Unit")]
        public string Division { set; get; }
        
        [DisplayName("Employee ID")]
        [Required] 
        public string EmpId { set; get; }        

        [DisplayName("Employee Initial")]
        [ReadOnly(true)]      
        public virtual string EmployeeInitial{ set; get; }
        
        [DisplayName("Designation")]
        [ReadOnly(true)]
        public virtual string Designation { set; get; }        

        [DisplayName("Employee Name")]
        [ReadOnly(true)]
        public string EmployeeName { set; get; }

        [DisplayName("Activation Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime? ActivationDate { set; get; }

        [DisplayName("Joining Date")]
        [UIHint("_ReadOnlyDate")]
        [ReadOnly(true)] 
        public DateTime? JoiningDate { set; get; }

        [UIHint("_MultiLine")]
        [Required]
        [StringLength(200)]
        public string Reason { set; get; }

        public int IsError { set; get; }
        public string ErrMsg { set; get; }
    }
  
}