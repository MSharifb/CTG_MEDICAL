using System;
using System.Collections.Generic;
using System.Web;
using BEPZA_MEDICAL.DAL.PRM;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Notification
{
    public class NotificationViewModel : BaseViewModel
    {

        public NotificationViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;

            NotificationFlows = new List<NTF_NotificationFlow>();
        }

        public int? NotificationType { get; set; }
        public DateTime NotificationDate { get; set; }
        public String Message { get; set; }
        public String RedirectToLink { get; set; }

        public IList<NTF_NotificationFlow> NotificationFlows { get; set; }

    }
    
}