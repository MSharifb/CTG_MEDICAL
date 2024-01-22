using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class PersonalPublicationViewModel
    {
        public PersonalPublicationViewModel() 
        {
            this.CountryList = new List<SelectListItem>();
            this.PublicationAreaList = new List<SelectListItem>();
            EmpTop = new EmpTop();
        }

        public EmpTop EmpTop { get; set; }

        //[DisplayName("Employee ID")]
        //[UIHint("_ReadOnly")]
        //public string EmpID { get; set; }
        //[UIHint("_ReadOnly")]
        //public string Name { get; set; }
        //[UIHint("_ReadOnly")]
        //public string Designation { get; set; }
        //[DisplayName("Initial")]
        //[UIHint("_ReadOnly")]
        //public string EmployeeInitial { get; set; }
        //public byte[] Picture { get; set; }


        public  int Id
        {
            get;
            set;
        }

        public int EmployeeId
        {
            get;
            set;
        }

        [Required]
        [DisplayName("Serial No.")]
        public  string SerialNo
        {
            get;
            set;
        }

        [Required]
        [MaxLength(1000)]
        [DisplayName("Name of Publication")]
        public  string PublicationName
        {
            get;
            set;
        }

        [Required]
        [DisplayName("Publication Area")]
        public int PublicationAreaId
        {
            get;
            set;
        }
        public IList<SelectListItem> PublicationAreaList { get; set; }

        [DisplayName("Date of Publication")]
        [UIHint("_Date")]
        public  System.DateTime? PublicationDate
        {
            get;
            set;
        }

        [DisplayName("Name of Journal")]
        [MaxLength(1000)]
        public  string JournalName
        {
            get;
            set;
        }

        [Required]
        [DisplayName("Country")]
        public int CountryId
        {
            get;
            set;
        }
        public IList<SelectListItem> CountryList { get; set; }
        [MaxLength(1000)]
        public  string Remarks
        {
            get;
            set;
        }

        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public string strMode { get; set; }
        public string SideBarClassName { get; set; }

    }
}