using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data.Objects.DataClasses;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class JobSkillInfoViewModel
    {
        public JobSkillInfoViewModel()
        {
            this.JobSkillList = new List<SelectListItem>();
            this.JobSkillLeveList = new List<SelectListItem>();

            this.EmpTop = new EmpTop();
        }

        #region Standard Property

        public int Id { get; set; }

        public int EmployeeId { get; set; }

        [Required]
        [DisplayName("Skill Name")]
        public int JobSkillId { get; set; }

        [Required]
        [DisplayName("Skill Level")]
        public int JobSkillLevelId { get; set; }

        [DisplayName("Year of Experience (on relevant area)")]
        public decimal YearofExperience { get; set; }   
   
        [MaxLength(200)]
        //[UIHint("_MultiLine")]
        public string Remarks { get; set; }

        [MaxLength(200)]
        [DisplayName("Skill Description")]
        public string SkillDescription { get; set; }

        #endregion

        public IList<SelectListItem> JobSkillList { get; set; }
        public IList<SelectListItem> JobSkillLeveList { get; set; }
        public EmpTop EmpTop { get; set; }

        public string strMode { get; set; }
        public string Message { get; set; }
        public string errClass { get; set; }
        public bool DeleteEnable { get; set; }
        public string ButtonText { get; set; }
        public string SideBarClassName { get; set; }
    }
}