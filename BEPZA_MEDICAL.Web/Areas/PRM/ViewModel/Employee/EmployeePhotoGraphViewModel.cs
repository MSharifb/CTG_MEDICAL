using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BEPZA_MEDICAL.DAL.PRM;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmployeePhotoGraphViewModel
    {

         [DisplayName("Employee ID")]
        [UIHint("_ReadOnly")]
        public virtual string EmpCode
        {
            get;
            set;
        }

        //[Required]
        [DisplayName("Employee Initial")]
        [StringLength(3)]
        [UIHint("_ReadOnly")]
        public virtual string EmployeeInitial
        {
            get;
            set;
        }

        [Required]
        [MaxLength(200)]
        [UIHint("_ReadOnly")]
        [DisplayName("Full Name")]
        public virtual string FullName
        {
            get;
            set;
        }
        public virtual Nullable<System.DateTime> DateofInactive
        {
            get;
            set;
        }

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public bool IsPhoto { get; set; }   
        public byte[] PhotoSignature { get; set; }     
        public string ImageUrl { get; set; }
        public string ImageAltText { get; set; }
        //public int IsError { set; get; }
        //public string ErrMsg { set; get; }
        public string SelectedClass { get; set; }
        public string ActionType { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }
}