using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PGM.Models.SalaryStructure
{
    public class SalaryStructureDetailsModel : BaseViewModel
    {
        public SalaryStructureDetailsModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            HeadAmountTypeList = new List<SelectListItem>();
            BasedOnList = new List<SelectListItem>();
        }

        public int SalaryStructureId { get; set; }

        [Required]
        public int HeadId { get; set; }

        public int EmployeeId { get; set; }

        public string DisplayHeadName { get; set; }

        [Required]
        public string HeadType { get; set; }

        //[Required]
        //public string HeadAmountType { get; set; }

        [Required]
        public string AmountType { get; set; }

        [DisplayName("Amount")]
        [UIHint("_OnlyCurrency")]
        [Required]
        public decimal Amount { set; get; }

        [DisplayName("Taxable")]
        public bool IsTaxable { set; get; }

        [DisplayName("GrossPay")]
        public bool IsGrossPayHead { set; get; }

        public string cssSalaryHeadClass { get; set; }

        public IList<SelectListItem> HeadAmountTypeList { get; set; }

        [DisplayName("Based On")]
        public string BasedOn { get; set; }
        public IList<SelectListItem> BasedOnList { get; set; }

        public decimal? TotalAmount { get; set; }
    }
}