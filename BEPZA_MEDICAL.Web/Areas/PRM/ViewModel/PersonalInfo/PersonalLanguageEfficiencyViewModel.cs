using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class PersonalLanguageEfficiencyViewModel
    {
        public PersonalLanguageEfficiencyViewModel()
        {
            this.LanguageList = new List<SelectListItem>();
            this.ListeningEfficiencyList = new List<SelectListItem>();
            this.WrittingEfficiencyList = new List<SelectListItem>();
            this.ReadingEfficiencyList = new List<SelectListItem>();
            this.SpeakingEfficiencyList = new List<SelectListItem>();
            EmpTop = new EmpTop();
        }
        public EmpTop EmpTop { get; set; }

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



        public int? Id { get; set; }
        public int EmployeeId { get; set; }

        [Required]
        [DisplayName("Language")]
        public int LanguageId { get; set; }
        public IList<SelectListItem> LanguageList { get; set; }

        [Required]
        [DisplayName("Reading")]        
        public int ReadingEfficiencyId { get; set; }
        public IList<SelectListItem> ReadingEfficiencyList { get; set; }

        [Required]
        [DisplayName("Speaking")]
        public int SpeakingEfficiencyId { get; set; }
        public IList<SelectListItem> SpeakingEfficiencyList { get; set; }

        [Required]
        [DisplayName("Listening")]
        public int ListeningEfficiencyId { get; set; }
        public IList<SelectListItem> ListeningEfficiencyList { get; set; }

        [Required]
        [DisplayName("Writing")]
        public int WrittingEfficiencyId { get; set; }
        public IList<SelectListItem> WrittingEfficiencyList { get; set; }

        [DisplayName("Is Native Language?")]
        public bool IsNative { get; set; }

        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public string errClass { get; set; }
        public bool DeleteEnable { get; set; }
        public string ButtonText { get; set; }
        public string SideBarClassName { get; set; }
        public string strMode { get; set; }

        public string ActionType { get; set; }   
        
    }
}