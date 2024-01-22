using System;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Notification
{
    public class NotificationFlowViewModel : BaseViewModel
    {
        public NotificationFlowViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        public int? NotificationId { get; set; }
        public int? Module { get; set; }
        public int? EmployeeId { get; set; }
    }
}