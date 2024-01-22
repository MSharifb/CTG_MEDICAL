using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Employee
{
    public class EmploymentContractPeriodViewModel
    {
        public EmploymentContractPeriodViewModel()
        {
            ContractStartDate = DateTime.Now;
            ContractEndDate = DateTime.Now;
        }

        #region Standard Property

        public virtual int Id { get; set; }
        
        [DisplayName("Employee ID")]
        [UIHint("_ReadOnly")]
        public virtual string EmpCode { get; set; }

        //[Required]
        [DisplayName("Employee Initial")]
        [StringLength(3)]
        [UIHint("_ReadOnly")]
        public virtual string EmployeeInitial { get; set; }

        [Required]
        [MaxLength(200)]
        [UIHint("_ReadOnly")]
        [DisplayName("Full Name")]
        public virtual string FullName { get; set; }

        public virtual Nullable<System.DateTime> DateofInactive { get; set; }

        public int EmpoyeeId { get; set; }
        public virtual bool IsContractual { get; set; }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Contract Start Date")]
        public virtual System.DateTime ContractStartDate { get; set; }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Contract End Date")]
        public virtual Nullable<System.DateTime> ContractEndDate { get; set; }

        public virtual string Remarks { get; set; }

        [DisplayName("Is Extension of Previous Contract?")]
        public virtual bool isExtension { get; set; }

        public virtual byte[] Attachment { get; set; }

        public string OriginalFileName { get; set; }

        [DisplayName("Order No.")]
        public string OrderNo{ get; set; }

        [UIHint("_Date")]
        [DisplayName("Order Date")]
        public DateTime? OrderDate { get; set; }

        #endregion

        public string Message { get; set; }
        public string ActionType { get; set; }
        public string ButtonText { get; set; }
        public bool DeleteEnable { get; set; }
        public string SelectedClass { get; set; }
        public string ErrorClass { get; set; }
        public int IsError{get;set;}
        public virtual ICollection<EmployeeContractAttachmentFiles> AttachmentFilesList { get; set; }
    }
}