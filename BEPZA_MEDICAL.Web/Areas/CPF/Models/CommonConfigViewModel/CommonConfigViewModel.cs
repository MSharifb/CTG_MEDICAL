using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.CommonConfigViewModel
{
    public class CommonConfigViewModel
    {
        [Required]
        public int Id { set; get; }

        [Required]
        [DisplayName("Name")]
        public string Name { set; get; }

        [DisplayName("Sort Order")]
        [Required]
        [UIHint("_OnlyInteger")]
        public int SortOrder { set; get; }

        [UIHint("_MultiLine")]
        public string Remarks { set; get; }
        public virtual ICollection<CommonConfigTypeViewModel> CommonConfigType { get; set; }
    }
}