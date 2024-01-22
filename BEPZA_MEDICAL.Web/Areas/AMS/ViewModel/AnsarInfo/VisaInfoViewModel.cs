using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.AMS.ViewModel.AnsarInfo
{
    public class VisaInfoViewModel
    {
        #region Ctor
        public VisaInfoViewModel()
        {
            this.FamilyMemberList = new List<SelectListItem>();
            this.VisaTypeList = new List<SelectListItem>();
            this.CountryList = new List<SelectListItem>();
            this.EmpTop = new EmpTop();
            
            this.IssueDate = DateTime.Now;
            this.ExpireDate = DateTime.Now;

            this.VisaOwner = String.Empty;
        }
        #endregion


        #region Standard Property

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }

        public string Type { get; set; }

        [Required]
        public string VisaPassportFor { get; set; }

        [DisplayName("Family Member")]
        public int? FamilyMemberId { get; set; }

        //[Required]
        public string VisaOwner { get; set; }

        [Required]
        [MaxLength(50)]
        [DisplayName("Visa No")]
        public string VisaPassportNo { get; set; }

        [MaxLength(50)]
        [DisplayName("Issuing Place")]
        public string IssuePlace { get; set; }

        [Required]
        [DisplayName("Issuing Country")]
        public int IssueCountryId { get; set; }

        [DisplayName("Type of Visa")]
        public int? VisaTypeId { get; set; }
                
        [DisplayName("Issue Date")]
        [UIHint("_Date")]
        [Required]
        public System.DateTime IssueDate { get; set; }

        [DisplayName("Expire Date")]
        [UIHint("_Date")]
        [Required]
        public System.DateTime ExpireDate { get; set; }
                
        [MaxLength(200)]
        [DisplayName("Notes")]
        //[UIHint("_MultiLineBig")]
        public string Notes { get; set; }

        #endregion

        public IList<SelectListItem> FamilyMemberList { get; set; }
        public IList<SelectListItem> VisaTypeList { get; set; }
        public IList<SelectListItem> CountryList { get; set; }
        public EmpTop EmpTop { get; set; }

        public string strMode { get; set; }
        public string Message { get; set; }
        public string errClass { get; set; }
        public string SideBarClassName { get; set; }

        public string ActionType { get; set; }
        public string ButtonText { get; set; }
        public bool DeleteEnable { get; set; }      
        public string ErrorClass { get; set; }
        public string ControlType { get; set; }
        public string SelectedClass { get; set; }
    }
}