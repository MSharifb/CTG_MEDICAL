using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.WFM.ViewModel
{
    public class PaymentInfoViewModel : BaseViewModel
    {
        #region Ctor
        public PaymentInfoViewModel()
        {
            this.ApprovalWelfareFundInfoList = new List<SelectListItem>();
            this.EmployeeList = new List<PaymentInfoEmployeeDetailsViewDetail>();
        }

        #endregion

        #region Standard Properties

        [Required]
        public int ZoneInfoId { get; set; }

        [Required]
        [DisplayName("Order No.")]
        public string PayOrderNo { get; set; }

        [Required]
        [DisplayName("Order Date")]
        [UIHint("_RequiredDate")]
        public DateTime? PayOrderDate { get; set; }

        [MaxLength(500, ErrorMessage = "Maximum length is 500.")]
        public string Comments { get; set; }

        [Required]
        [DisplayName("Meeting Ref. No.")]
        public int ApprovalWelfareFundInfoId { get; set; }

        #endregion

        #region Others

        public string ApprovalWelfareFundInfoRefNo { get; set; }
        public IList<SelectListItem> ApprovalWelfareFundInfoList { get; set; }

        [DisplayName("Committee Name")]
        [UIHint("_ReadOnly")]
        public string CommitteeName { get; set; }

        [DisplayName("Meeting Date")]
        [UIHint("_ReadOnly")]
        public string MeetDate { get; set; }

        [DisplayName("Meeting Time")]
        [UIHint("_ReadOnly")]
        public string MeetTime { get; set; }

        [DisplayName("Meeting Place")]
        [UIHint("_ReadOnly")]
        public string MeetPlace { get; set; }

        [DisplayName("Meeting Agenda")]
        [UIHint("_ReadOnly")]
        public string MeetAgenda { get; set; }

        [DisplayName("Meeting Cycle")]
        [UIHint("_ReadOnly")]
        public string MeetCycle { get; set; }

        [UIHint("_ReadOnly")]
        public string MeetCycleMonth { get; set; }
        
        [UIHint("_ReadOnly")]
        public string MeetCycleYear { get; set; }
        public IList<PaymentInfoEmployeeDetailsViewDetail> EmployeeList { get; set; }

        public string ShowRecord { get; set; }

        public string EmpId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string ApproveAmount { get; set; }
        public string CategoryName { get; set; }
        #endregion
    }
}