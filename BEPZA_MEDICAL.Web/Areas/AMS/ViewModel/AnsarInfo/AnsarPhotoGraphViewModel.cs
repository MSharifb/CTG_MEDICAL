using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel
{
    public class AnsarPhotoGraphViewModel
    {

        [DisplayName("Ansar ID")]
        [UIHint("_ReadOnly")]
        public virtual string EmpCode { get; set; }

        [Required]
        [MaxLength(200)]
        [UIHint("_ReadOnly")]
        [DisplayName("Name")]
        public virtual string FullName { get; set; }
        public virtual Nullable<System.DateTime> InactiveDate { get; set; }

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public bool IsPhoto { get; set; }   
        public byte[] PhotoSignature { get; set; }     
        public string ImageUrl { get; set; }
        public string ImageAltText { get; set; }
        public string SelectedClass { get; set; }
        public string ActionType { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }
}