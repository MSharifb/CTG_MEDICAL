using System;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Notification
{
    public class NotificationReadByViewModel : BaseViewModel
    {
        public NotificationReadByViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        public int? NotificationId { get; set; }
        public int? EmployeeId { get; set; }
    }
}