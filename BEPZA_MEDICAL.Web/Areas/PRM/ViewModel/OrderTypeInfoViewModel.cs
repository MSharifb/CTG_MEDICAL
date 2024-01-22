using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class OrderTypeInfoViewModel : BaseViewModel
    {
        #region Ctor
        public OrderTypeInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }
        #endregion

        #region Standard Property

        [Required]
        [DisplayName("Order Type Name")]
        //[Remote("doesOrderTypeExist", "OrderTypeInfo", HttpMethod = "POST", ErrorMessage = "Order Type Name already exists. Please enter a different Order Type Name.")]
        public string Name { get; set; }

        [DisplayName("Is Final Order")]
        public bool IsFinalOrder { get; set; }

        [DisplayName("Sort Order")]
        public int SortOrder { get; set; }

        [UIHint("_MultiLine")]
        public string Remarks { get; set; }
        
        #endregion 
    }
}