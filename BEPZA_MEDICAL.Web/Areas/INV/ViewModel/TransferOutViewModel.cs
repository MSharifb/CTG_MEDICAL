using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
//using System.Web.WebPages.Html;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel
{
    public class TransferOutViewModel : BaseViewModel
    {

        #region Ctor
        public TransferOutViewModel()
        {
            //this.IUser = HttpContext.Current.User.Identity.Name;
            //this.IDate = DateTime.Now;

            TransferredToZoneList = new List<SelectListItem>();
            TransferredByEmpList = new List<SelectListItem>();
            this.TransferOutDetail = new List<TransferOutDetailViewModel>();
            CategoryList = new List<SelectListItem>();
            ColorList = new List<SelectListItem>();
            ItemTypeList = new List<SelectListItem>();
            ModelList = new List<SelectListItem>();
            UnitList = new List<SelectListItem>();
            ItemList = new List<SelectListItem>();
            MRRList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property
        public int ZoneInfoId { get; set; }
        //[Required]
        public int Id { get; set; }

        [Display(Name = "Challan #")]
        [MaxLength(50)]
        public string ChallanNo { get; set; }
        [Display(Name = "Transfer #")]
        [MaxLength(50)]
        [UIHint("_ReadOnly")]
        public string TransferNo { get; set; }

        [Display(Name = "Transfer Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime TransferDate { get; set; }

        [Display(Name = "Challan Date")]
        [UIHint("_Date")]
        public DateTime? ChallanDate { get; set; }

        [Display(Name = "Transferred By")]
        [Required]
        public int TransferredByEmpId { get; set; }

        [DisplayName("Transferred By")]
        //[Required]
        [UIHint("_ReadOnly")]
        public string TransferredByEmp { get; set; }
        [Required]
        public string EmpId { get; set; }
        public string FullName { get; set; }

        [Display(Name = "Comment")]
        [MaxLength(100)]
        public string Comment { get; set; }
        [Display(Name = "Transferred To")]
        [Required]
        public int TransferredToZoneId { get; set; }
        [Display(Name = "Transferred To")]
        //[Required]
        public string TransferredToZone { get; set; }

        #endregion

        #region Attachment
        public HttpPostedFileBase File { get; set; }

        [DisplayName("Attachment")]
        public byte[] Attachment { get; set; }

        public string FileName { get; set; }

        #endregion

        #region Other

        public IList<SelectListItem> TransferredToZoneList { get; set; }
        public IList<SelectListItem> TransferredByEmpList { get; set; }
        public IList<TransferOutDetailViewModel> TransferOutDetail { get; set; }
        //public IList<ItemPurchaseViewModel> TempItemDetail { get; set; }


        [DisplayName("Item")]
        public int? ItemId { get; set; }

        [DisplayName("Category")]
        public int? CategoryId { set; get; }

        [DisplayName("Color")]
        public int? ColorId { set; get; }

        [DisplayName("Type")]
        public int? TypeId { set; get; }

        [DisplayName("Model")]
        public int? ModelId { set; get; }

        [DisplayName("Unit")]
        public int? UnitId { set; get; }

        //[Required]
        [DisplayName("Quantity")]
        [Range(0, 99999999)]
        public decimal? Quantity { get; set; }
        [DisplayName("MRR #")]
        public int? PurchaseId { get; set; }

        [DisplayName("Comment")]
        [MaxLength(100)]
        public string CommentDetail { get; set; }
        //public string ChallanNo { get; set; }
        //public string TransferNo { get; set; }
        public DateTime? TransferDateFrom { get; set; }
        public DateTime? TransferDateTo { get; set; }
        public DateTime? ChallanDateFrom { get; set; }
        public DateTime? ChallanDateTo { get; set; }


        public IList<SelectListItem> ItemList { set; get; }
        public IList<SelectListItem> CategoryList { set; get; }
        public IList<SelectListItem> ColorList { set; get; }
        public IList<SelectListItem> ItemTypeList { set; get; }
        public IList<SelectListItem> ModelList { set; get; }
        public IList<SelectListItem> UnitList { set; get; }
        public IList<SelectListItem> MRRList { set; get; }

        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(TransferredByEmp))
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.EmpID like {0} AND ", TransferredByEmp));

            if (!String.IsNullOrWhiteSpace(TransferredToZone))
                filterExpressionBuilder.Append(String.Format("MRR like {0} AND ", TransferredToZone));

            if (!String.IsNullOrWhiteSpace(TransferNo))
                filterExpressionBuilder.Append(String.Format("PO like {0} AND ", TransferNo));

            if (!String.IsNullOrWhiteSpace(ChallanNo))
                filterExpressionBuilder.Append(String.Format("Challan like {0} AND ", ChallanNo));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion

    }


    //public class TransferOutSearchViewModel
    //{
    //    #region Properties
    //    public string EmpID { get; set; }
    //    public string ZoneID { get; set; }
        

    //    #endregion

       
    //}

}
