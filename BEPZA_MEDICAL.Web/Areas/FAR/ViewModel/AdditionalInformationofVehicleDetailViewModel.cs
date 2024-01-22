using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class AdditionalInformationofVehicleDetailViewModel : BaseViewModel
    {
        public int? SparePartId { get; set; }
        public IList<SelectListItem> SparePartList { get; set; }
        public string SparePartName { get; set; }
        public int? Quantity { get; set; }
    }
}