using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel
{ 
    public class HeadViewModel: BaseViewModel
    {
        public HeadViewModel()
        {
            this.ParentHeadList = new List<SelectListItem>();
        }
       [DisplayName("Parent Head")]
       public int? ParentHeadId { get; set; }
       public string HeadName { get; set; }
       public bool IsParentHead { get; set; }
       public bool IsSubledger { get; set; }
       public int? SortingOrder { get; set; }
       public IList<SelectListItem> ParentHeadList { get; set; }
    }
}