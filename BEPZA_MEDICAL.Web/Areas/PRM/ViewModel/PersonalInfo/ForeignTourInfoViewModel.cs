using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.PersonalInfo
{
    public class ForeignTourInfoViewModel
    {
        #region Ctor
        public ForeignTourInfoViewModel()
        {
            this.CountryList = new List<SelectListItem>();
            this.EmpTop = new EmpTop();
            this.VisitDateFrom = DateTime.Now;
            this.VisitDateTo = DateTime.Now;
        }
        #endregion

        #region Standard Property

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }

        public string Type { get; set; }

        [Required]
        [DisplayName("Title of the Tour")]
        public string TitleOfTheTour { get; set; }

        [DisplayName("Organized By")]
        public string OrganizedBy { get; set; }

        [DisplayName("Financed By")]
        public string Financed { get; set; }

        [Required]
        [DisplayName("Visiting Country")]
        public int CountryId { get; set; }

        [DisplayName("Visit Date To")]
        [UIHint("_Date")]
        public DateTime? VisitDateTo { get; set; }

        [DisplayName("Visit Date From")]
        [UIHint("_Date")]
        public DateTime? VisitDateFrom { get; set; }

        [MaxLength(200)]
        [DisplayName("Remarks/Purpuse")]
        public string Remarks { get; set; }

        #endregion

        #region Other

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

        #endregion

        #region Attachment

        public byte[] Attachment { set; get; }
        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion

    }
}