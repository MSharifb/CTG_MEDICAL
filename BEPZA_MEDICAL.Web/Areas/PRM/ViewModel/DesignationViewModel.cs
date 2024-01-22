using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class DesignationViewModel
    {
        public DesignationViewModel()
        {            
            this.JobGradeList = new List<SelectListItem>();
            this.EmployeeClassList = new List<SelectListItem>();
        }

        [Required]
        public int Id { get; set; }

        [Required]
        [DisplayName("Grade")]
        public int GradeId { get; set; }
        
        [Required]
        [DisplayName("Designation")]
        public string Name { get; set; }      
       

        [UIHint("_MultiLine")]
        [DisplayName("Job Description")]
        public string JobDescription { get; set; }
        [UIHint("_MultiLine")]

        [DisplayName("Employee Class")]
        public int EmployeeClassId { get; set; }
        public string Remarks { get; set; }
        
        public string ActionType { get; set; }
        
        public int IsError { set; get; }
        
        public string ErrMsg { set; get; }

        public IList<SelectListItem> JobGradeList { get; set; }
        public IList<SelectListItem> EmployeeClassList { get; set; }

        [Required]
        [DisplayName("Designation in Bangla")]
        public string NameB { get; set; }

        [DisplayName("Sorting Order")]
        [UIHint("_OnlyCurrency")]
        public int SortingOrder { get; set; }

        [DisplayName("Short Name")]
        public string ShortName { get; set; }

        [DisplayName("Pay Scale Details")]
        public string PayScaleDetails { get; set; }  

        public string GradeName { get; set; }
    }
}