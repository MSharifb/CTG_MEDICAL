using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class OrganogramLevelViewModel
    {
        public OrganogramLevelViewModel()
        {  
            this.OrganogramTypeList = new List<SelectListItem>();
            this.IsEditable = true;
        }

        public IList<SelectListItem> OrganogramTypeList { set; get; }

        [Required]
        [DisplayName("Organogram Type")]
        public int? OrganogramTypeId { set; get; }

        [Required]
        public int Id { get; set; }

        [Required]
        [DisplayName("Level Name")]
        public string LevelName { set; get; }

        [Required]
        [DisplayName("Parent Id")]
        public int ParentId { get; set; }

        [DisplayName("Code")]
        public string Code { set; get; }

        [DisplayName("Prefix")]
        public string Prefix { set; get; }

        [Required]
        public int Position { get; set; }

       [DisplayName("Active Status")]
        public bool IsActive { set; get; }
         
        [Required]
        public int ZoneInfoId { get; set; }
        public bool IsEditable { get; set; }

        #region Others

        public string Mode { get; set; }

        public string OrganogramTypeName { get; set; }
        /// <summary>
        /// This is use for multiple organogram popup in one view
        /// </summary>
        public string OrgIdentityName { get; set; }

        /// <summary>
        /// Added by Suman
        /// </summary>
        public string DepartmentName { get; set; }
        public string OfficeName { get; set; }
        public string SectionName { get; set; }
        public string SubSectionName { get; set; }
        #endregion
    }
}
