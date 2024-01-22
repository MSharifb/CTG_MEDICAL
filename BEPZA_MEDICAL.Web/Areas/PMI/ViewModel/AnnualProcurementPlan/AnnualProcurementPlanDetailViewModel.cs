using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.AnnualProcurementPlan
{
    public class AnnualProcurementPlanDetailViewModel : BaseViewModel
    {
        public AnnualProcurementPlanDetailViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;

            BudgetHeadList = new List<SelectListItem>();
            ConstructionTypeList = new List<SelectListItem>();
            ProcurementTypeList = new List<SelectListItem>();
            SourceOfFundList = new List<SelectListItem>();
            ApprovingAuthorityList = new List<SelectListItem>();
            DescritionOfAPPList = new List<SelectListItem>();
        }

        #region Standard
        public int AnnualProcurementPlanMasterId { get; set; }
        public string LotNo { get; set; }
        public string PackageNo { get; set; }
        public string Unit { get; set; }
        public string Quantity { get; set; }
        public int? ProcurementTypeId { get; set; }
        public int? SourceOfFundId { get; set; }
        public int? ApprovingAuthorityId { get; set; }
        public decimal? EstdCost { get; set; }
        public decimal? ContractValue { get; set; }

        [UIHint("_Date")]
        public DateTime? InvitationforTender { get; set; }
        public int InvitationforTenderDays { get; set; }
        [UIHint("_Date")]
        public DateTime? InvitationforTenderActualDates { get; set; }

        [UIHint("_Date")]
        public DateTime? TenderOpening { get; set; }
        public int TenderOpeningDays { get; set; }
        [UIHint("_Date")]
        public DateTime? TenderOpeningActualDates { get; set; }

        [UIHint("_Date")]
        public DateTime? TenderElavuation { get; set; }
        public int TenderElavuationDays { get; set; }
        [UIHint("_Date")]
        public DateTime? TenderElavuationActualDates { get; set; }

        [UIHint("_Date")]
        public DateTime? ApprovalToAward { get; set; }
        public int ApprovalToAwardDays { get; set; }
        [UIHint("_Date")]
        public DateTime? ApprovalToAwardActualDates { get; set; }

        [UIHint("_Date")]
        public DateTime? NotificationOfAward { get; set; }
        public int NotificationOfAwardDays { get; set; }
        [UIHint("_Date")]
        public DateTime? NotificationOfAwardActualDates { get; set; }


        [UIHint("_Date")]
        public DateTime? SigningofContract { get; set; }
        public int SigningofContractDays { get; set; }
        [UIHint("_Date")]
        public DateTime? SigningofContractActualDates { get; set; }

        [UIHint("_Date")]
        public DateTime? TimeToContract { get; set; }
        public int TimeToContractDays { get; set; }
        [UIHint("_Date")]
        public DateTime? TimeToContractActualDates { get; set; }


        [UIHint("_Date")]
        public DateTime? CompletionofContract { get; set; }
        public int CompletionofContractDays { get; set; }
        [UIHint("_Date")]
        public DateTime? CompletionofContractActualDates { get; set; }


        [Required]
        public int APPHeadId { get; set; }
        [Required]
        [DisplayName("Construction Type")]
        public int ConstructionTypeId { get; set; }
        public string DescritionOfAPP { get; set; }
        [DisplayName("Remarks")]
        public string Remarks { get; set; }
        public int? BudgetDetailsId { get; set; }
        #endregion

        #region List

        public IList<SelectListItem> BudgetHeadList { get; set; }

        public IList<SelectListItem> ConstructionTypeList { get; set; }

        public IList<SelectListItem> ProcurementTypeList { get; set; }

        public IList<SelectListItem> SourceOfFundList { get; set; }

        public IList<SelectListItem> ApprovingAuthorityList { get; set; }

        public IList<SelectListItem> DescritionOfAPPList { get; set; }
        #endregion

        #region Others
        public int TempHeadId { get; set; }
        public string BudgetHeadName { get; set; }
        public string BudgetSubHeadName { get; set; }
        public string ConstructionTypeName { get; set; }

        #endregion
    }
}