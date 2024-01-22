using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Approval
{
    public class ViewApplicationViewModel : BaseViewModel
    {
        public ViewApplicationViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        public DateTime ApplicationDate { get; set; }

        public string ApplicationTo { get; set; }

        public string ApplicationSubject { get; set; }

        public string ApplicationBody { get; set; }

        public string ApplicantEmployeeId { get; set; }

        public string ApplicantName { get; set; }

        public string ApplicantDepartment { get; set; }

        public string ApplicantDesignation { get; set; }

        public string ApplicationFor { get; set; }

        public string ApplicationReason { get; set; }

        public decimal AppliedAmount { get; set; }

        public string ApplicationNo { get; set; }

    }
}