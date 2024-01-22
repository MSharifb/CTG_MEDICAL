using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.PGM.Models.SalaryHeadGroup
{
    public class SalaryHeadGroupViewModel: BaseViewModel
    {
        public SalaryHeadGroupViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            HeadTypeList = new List<SelectListItem>();
        }

        [DisplayName("Head Group")]
        [MaxLength(50)]
        [Required(AllowEmptyStrings = false)]
        public string Name { set; get; }

        [DisplayName("Head Type")]
        [Required]
        public string HeadType { set; get; }
        public IList<SelectListItem> HeadTypeList { get; set; }

        [MaxLength(100)]
        public string Remarks { set; get; }

        [DisplayName("Sort Order")]
        [Required]
        public int SortOrder { set; get; }
        
    }
}