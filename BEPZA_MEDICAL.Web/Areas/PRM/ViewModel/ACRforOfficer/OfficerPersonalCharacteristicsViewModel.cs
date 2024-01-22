using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforOfficer
{
    public class OfficerPersonalCharacteristicsViewModel
    {
        #region Ctor
        public OfficerPersonalCharacteristicsViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.AttributeDetailList= new List<OfficerPersonalCharacteristicsDetailViewModel>();
        }

        #endregion

        #region Standerd Property
        public int OfficerInfoId { get; set; }
        public int EmployeeId { get; set; }
        public int ACRAttributeId { get; set; }
        public string ACRAttributeName { get; set; }
        public int ACRMark { get; set; }

        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public decimal? SerialNumber { get; set; }

        public bool ChkFour { get; set; }
        public bool ChkThree { get; set; }
        public bool CkhTwo { get; set; }
        public bool ChkOne { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

        

        #endregion

        #region Other

        public string Message { get; set; }
        public string ActionType { get; set; }
        public string ButtonText { get; set; }
        public string SelectedClass { get; set; }
        public bool DeleteEnable { get; set; }
        public string ErrorClass { get; set; }
        public int IsError { get; set; }
        public bool isExist { get; set; }
        public bool IsPhotoExist { get; set; }

        public IList<OfficerPersonalCharacteristicsDetailViewModel> AttributeDetailList { get; set; }

        
        #endregion

        #region Officer's Info

        [Display(Name = "Employee ID")]
        public string EmpId { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        [Display(Name = "Designation")]
        public string EmployeeDesignation { get; set; }
        [Display(Name = "Department")]
        public string EmployeeDepartment { get; set; }
        [Display(Name = "Annual Confidential Report Date")]
        public string ACRDate { get; set; }
        [Display(Name = "ACR Period From")]
        public string ACRPeriodFrom { get; set; }
        [Display(Name = "ACR Period To")]
        public string ACRPeriodTo { get; set; }
        [Display(Name = "Seniority Serial Number")]
        public string SeniorityNumber { get; set; }

        #endregion
    }
}