using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ApprovalFlow
{
    public class ApprovalFlowViewModel : BaseViewModel
    {
        public ApprovalFlowViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
            EmployeeList = new List<SelectListItem>();
            DesignationList = new List<SelectListItem>();
        }

        #region Standard Property

        public int BudgetProjectAPPId { get; set; }
        public int ApprovalSectionId { get; set; }
        public DateTime? CreateDate { get; set; }

        #endregion

        #region Others
        public int? EmployeeId { get; set; }

        public IList<SelectListItem> EmployeeList { get; set; }

        public int? DesignationId { get; set; }

        public IList<SelectListItem> DesignationList { get; set; }


        public List<ApprovalFlowViewModel> ApproverList { get; set; }

        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }

        public string ApprovalSection { get; set; }
        public string EmployeeName { get; set; }
        public string Remarks { get; set; }
        public string EmpId { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        #endregion

    }
}