using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class OfficeEquipmentFurnitureUsagesInfoViewModel: BaseViewModel
    {
        #region Ctor
        public OfficeEquipmentFurnitureUsagesInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";

            this.OfficeEquipmentFurList = new List<SelectListItem>();
            this.OfficeEquipmentUsagesDetailList= new List<OfficeEquipmentFurnitureUsagesInfoDetailViewModel>();
        }
        #endregion

        #region Standard
        public int EmployeeId { get; set; }
        #endregion

        #region Other

        [Display(Name="Employee ID")]
        public string EmpId { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }

        //Office Equipment Furniture Usages Info Details
        [Display(Name = "Name")]
        public int? OfficeEquipmentFurInfoId { get; set; }
        public IList<SelectListItem> OfficeEquipmentFurList { get; set; }
        [Display(Name = "Procurement Date")]
        [UIHint("_Date")]
        public DateTime? ProcurementDate { get; set; }
        [Display(Name = "Issue Date")]
        [UIHint("_Date")]
        public DateTime? IssueDate { get; set; }
        [Display(Name = "Issue For")]
        public string IssueFor { get; set; }
        [Display(Name = "Issue By")]
        public string IssueByName { get; set; }
        public int IssueById { get; set; }
        [Display(Name = "Property Cost")]
        public decimal? PropertyCost { get; set; }
        [Display(Name = "Is Returnable")]
        public bool IsReturn { get; set; }
        public string Remarks { get; set; }
        public string OfficeEquName { get; set; }

        public IList<OfficeEquipmentFurnitureUsagesInfoDetailViewModel> OfficeEquipmentUsagesDetailList { get; set; }
        #endregion

    }
}