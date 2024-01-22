using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.AMS.Models.ShiftInformation
{
    public class ShiftInformationViewModel : BaseViewModel
    {
        public ShiftInformationViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.Mode = "Create";

            this.Department = new List<SelectListItem>();
            this.ShiftTypeList = new List<SelectListItem>();
            this.AMS_ShiftInformationDetail = new List<AMS_ShiftInformationViewDetail>();
            this.AMS_ShiftBreakInformationViewModel = new List<AMS_ShiftBreaInformationViewModel>();
         }

        #region Standard Property

        //public int Id { get; set; }
        [Required]
        [DisplayName("Type")]
        public int ShiftType { get; set; }
        
        [Required]
        [DisplayName("Name")]
        public string ShiftName { get; set; }
        
        [DisplayName("Department")]
        public Nullable<int> DepartmentId { get; set; }
        public string Description { get; set; }
        
        [DisplayName("Is Roster?")]
        public bool IsRoster { get; set; }
        [DisplayName("Is Bridge(Two Day) Shift?")] 
        public bool IsNightShift { get; set; }
        //public string IUser { get; set; }
        public System.DateTime IDate { get; set; }
        //public string EUser { get; set; }
        //public Nullable<System.DateTime> EDate { get; set; }

        public string DepartmentName { get; set; }
        //
        public List<AMS_ShiftInformationViewDetail> AMS_ShiftInformationDetail { get; set; }

        public List<AMS_ShiftBreaInformationViewModel> AMS_ShiftBreakInformationViewModel { get; set; }
       #endregion

        #region Others

        public string Mode { get; set; }

        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public IList<SelectListItem> Department { get; set; }
        public IList<SelectListItem> ShiftTypeList { get; set; }
      
        #endregion
    }

 

}