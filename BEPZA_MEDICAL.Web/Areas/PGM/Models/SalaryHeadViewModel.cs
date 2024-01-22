using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PGM.Models.SalaryHead
{
    public class SalaryHeadViewModel : BaseViewModel
    {
        public SalaryHeadViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.HeadGroupList = new List<SelectListItem>();
            this.HeadMappingList = new List<SelectListItem>();
            this.AccountHeadList = new List<SelectListItem>();
        }

        [DisplayName("Head Group")]
        [Required]
        public int GroupId { set; get; }
        public IList<SelectListItem> HeadGroupList { set; get; }

        [DisplayName("Head Name")]
        [MaxLength(50)]
        [Required]
        public String HeadName { set; get; }
        
        [DisplayName("Head Mapping")]
        public int? HeadMappingId { set; get; }
        public IList<SelectListItem> HeadMappingList { set; get; }

        [DisplayName("Short Name")]
        [Required]
        [MaxLength(20)]
        public string ShortName { get; set; }
        
        [DisplayName("Head Type")]
        [Required]
        public string HeadType { set; get; }
        public IList<SelectListItem> HeadTypeList { get; set; }

        [DisplayName("Amount Type")]
        [Required]
        public string AmountType { set; get; }

        [DisplayName("Entity Name")]
        public int? EntityNameId { get; set; }
        public IList<SelectListItem> AccEntityList { get; set; }

        [DisplayName("Account Head")]
        [Required]
        public int? AccountHeadId { get; set; }
        public IList<SelectListItem> AccountHeadList { set; get; }

        [DisplayName("Basic Head")]
        public bool IsBasicHead { get; set; }
        
        [DisplayName("Taxable Head")]
        public bool IsTaxable { set; get; }

        [DisplayName("Gross Pay Head")]
        public bool IsGrossPayHead { set; get; }
        
        [DisplayName("Sort Order")]
        [Required]
        public int SortOrder { set; get; }

        [DisplayName("Default Amount")]
        [Required]
        public decimal? DefaultAmount { get; set; }
        
        [DisplayName("Is Other Addition")]
        public bool IsOtherAddition { get; set; }

        [DisplayName("Is Other Deduction")]
        public bool IsOtherDeduction { get; set; }

        #region Salary Head Mapping

        public bool IsIncomeTaxAdditionHead { get; set; }
        public bool IsIncomeTaxDeductionHead { get; set; }
        public bool IsPfOwnContributionHead { get; set; }
        public bool IsPfCompanyContributionHead { get; set; }
        public bool IsHouseRentHead { get; set; }
        public bool IsMedicalHead { get; set; }
        public bool IsConveyanceHead { get; set; }
        public bool IsLeaveWithoutPayHead { get; set; }
        public bool IsPensionHead { get; set; }
        public bool IsGPFHead { get; set; }
        public bool IsArrearHead { get; set; }
        public bool IsGratuityHead { get; set; }

        #endregion

    }
}