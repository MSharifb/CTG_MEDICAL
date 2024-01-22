using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class PersonalEmergencyContractViewModel
    {
        public PersonalEmergencyContractViewModel()
        {          
            this.RelationList = new List<SelectListItem>();
            this.TitleList = new List<SelectListItem>();
            EmpTop = new EmpTop();
        }

        public EmpTop EmpTop { get; set; }

        #region Basic informaition

        [DisplayName("Employee ID")]
        [UIHint("_ReadOnly")]
        public string EmpID { get; set; }
        [UIHint("_ReadOnly")]
        public string Name { get; set; }
        [UIHint("_ReadOnly")]
        public string Designation { get; set; }
        [DisplayName("Initial")]
        [UIHint("_ReadOnly")]
        public string EmployeeInitial { get; set; }
        public byte[] Picture { get; set; }
        
        #endregion   

        public  int EmployeeId
        {            
            get;set;
        }       

        [Required]
        [DisplayName("Relation")] 
        public  int RelationId
        {
            get;set;           
        }
        public IList<SelectListItem> RelationList { get; set; }
                
        [DisplayName("Title")]
        public int? TitleId
        {
           get;set;
        }
        public IList<SelectListItem> TitleList { get; set; }

        [DisplayName("First Name")]
        [Required]
        public  string FirstName
        {
            get;
            set;
        }

        [DisplayName("Middle Name")]
        public  string MiddleName
        {
            get;
            set;
        }

        [DisplayName("Last Name")]
        public  string LastName
        {
            get;
            set;
        }

        [DisplayName("Full Name")]       
        [UIHint("_ReadOnly")]
        public  string FullName
        {
            get;
            set;
        }

        [Required]
        [DisplayName("Mobile")]
        public string MobileNo
        {
            get;
            set;
        }

        [DisplayName("Office Phone")]
        public string OfficePhone
        {
            get;
            set;
        }

        [DisplayName("Home Phone")]
        public string HomePhone
        {
            get;
            set;
        }
   
        [DisplayName("Email")]
        [RegularExpression(@"^\w+([-+.]*[\w-]+)*@(\w+([-.]?\w+)){1,}\.\w{2,4}$")]
        public string EmailAddress
        {
            get;
            set;
        }

        [DisplayName("Office Address")]
        [MaxLength(200)]
        public string OfficeAddress
        {
            get;
            set;
        }

        [Required]
        [DisplayName("Home Address")]
        [MaxLength(200)]
        public string HomeAddress
        {
            get;
            set;
        }  

        [DisplayName("Fax No.")]
        public  string FaxNo
        {
            get;
            set;
        }     

        [DisplayName("Add Photo?")]
        public  bool isAddPhoto
        {
            get;
            set;
        }

        public  byte[] Photo
        {
            get;
            set;
        }

        public string ImageUrl { get; set; }
        public string ImageAltText { get; set; }
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
        public bool HasDatabaserValue { get; set; }

        public string SideBarClassName { get; set; }
        public string strMode { get; set; }
        public string ActionType { get; set; }
        

    }
}