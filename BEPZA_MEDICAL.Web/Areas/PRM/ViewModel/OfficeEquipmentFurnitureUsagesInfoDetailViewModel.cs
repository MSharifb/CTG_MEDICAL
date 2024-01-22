using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class OfficeEquipmentFurnitureUsagesInfoDetailViewModel:BaseViewModel
    {
        #region Ctor
        public OfficeEquipmentFurnitureUsagesInfoDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";

        }
        #endregion

        #region Standard

        public int OfficeEquipmentFurInfoId { get; set; }
        public DateTime? ProcurementDate { get; set; }
        public DateTime? IssueDate { get; set; }
        public string IssueFor { get; set; }
        public int IssueById { get; set; }
        public decimal? PropertyCost { get; set; }
        public bool IsReturn { get; set; }
        public string Remarks { get; set; }

        #endregion

        #region Other
        public IList<SelectListItem> OfficeEquipmentFurList { get; set; }
        public string IssueByName { get; set; }
        public string OfficeEquName { get; set; }
        #endregion
    }
}