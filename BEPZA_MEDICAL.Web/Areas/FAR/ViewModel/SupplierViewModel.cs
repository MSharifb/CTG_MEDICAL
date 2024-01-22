using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class SupplierViewModel : BaseViewModel
    {
        [Required]
        public int ZoneInfoId { get; set; }

        [Required]
        public string Name { set; get; }

        [DisplayName("Sort Order")]
        [Required]
        public int? SortOrder { set; get; }

        [UIHint("_MultiLine")]
        public string Remarks { set; get; }
    }
}