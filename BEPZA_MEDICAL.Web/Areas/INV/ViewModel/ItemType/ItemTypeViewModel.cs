using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel.ItemType
{
    public class ItemTypeViewModel : BaseViewModel
    {
        public ItemTypeViewModel()
        {
            ItemTypeGroupList = new List<SelectListItem>();
            BudgetHeadList = new List<SelectListItem>();
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }        

        [DisplayName("Is Group")]
        [Required]
        public bool IsGroup { set; get; }

        [DisplayName("Code"),StringLength(50)]        
        public string Code { get; set; }
       
        [DisplayName("Code Count")]
        public int? CodeCount { get; set; }

        [ DisplayName("Level")]
        public int? Level { get; set; }

        [DisplayName("Item Type")]
        [MaxLength(100)]
        [Required]
        public string ItemTypeName { get; set; }

        [DisplayName("Item Type Group")]
        [Required]
        public int? ParentId { set; get; }

        [DisplayName("Budget Head")]
        public int? BudgetHeadId { set; get; }

        [DisplayName("Remarks")]
        [MaxLength(200)]
        public string Remarks { get; set; }

        // Others

        public string Mode { get; set; }
        public string ItemTypeIdentityName { get; set; }
        public IList<SelectListItem> ItemTypeGroupList { set; get; }
        public IList<SelectListItem> BudgetHeadList { set; get; }
        //
    }
}