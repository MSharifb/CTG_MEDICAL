using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BOM_MPA.Web.Areas.PRM.ViewModel.PersonalInfo
{
    public class SpouseInfoViewModel
    {
        #region Ctor
        public SpouseInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            //this.Mode = "Create";
            this.EmpTop = new EmpTop();
            ChildrenList = new List<ChildrenInfoViewModel>(); 

        }
        #endregion

        #region Standard Property
        public int Id { get; set; }

       
        public int EmployeeId { get; set; }

        [Required]       
        public string Name { get; set; }

        [DisplayName("Marriage Date")]
        [UIHint("_Date")]
        public DateTime? DateofMarriage { get; set; }

        [DisplayName("Date of Birth")]
        [UIHint("_Date")]
        public DateTime? DateofBirth { get; set; }

        [DisplayName("Profession")]
        public int ProfessionId { get; set; }

        public IList<SelectListItem> ProfessionList { get; set; }


        [DisplayName("Academic Qualification")]
        public int DegreeLevelId { get; set; }

        public IList<SelectListItem> DegreeLeveList { get; set; }

        [DisplayName("Blood Group")]
        public int BloodGroupId { get; set; }

        public IList<SelectListItem> BloodGroupList { get; set; }


        [DisplayName("Home District")]
        public int DistrictId { get; set; }

        public IList<SelectListItem> DistrictList { get; set; }
        public int NationalID { get; set; }
       
        [DisplayName("Nationality")]
        public int NationalityId { get; set; }

        public IList<SelectListItem> NationalityList { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        #endregion


        #region Other

        public virtual IList<ChildrenInfoViewModel> ChildrenList { get; set; }
        public EmpTop EmpTop { get; set; } 
        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }

        #endregion
    }
}