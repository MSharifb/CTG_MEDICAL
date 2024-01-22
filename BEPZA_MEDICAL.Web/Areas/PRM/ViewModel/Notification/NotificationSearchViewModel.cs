using System;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Notification
{
    public class NotificationSearchViewModel : BaseViewModel
    {   
        public String Module { get; set; }
        public String NotificationType { get; set; }

        public int SlNo { get; set; }

        public DateTime NotificationDate { get; set; }

        public String NotifyBy { get; set; }

        public String Message { get; set; }

        public String RedirectLink { get; set; }

        public String ModuleName { get; set; }
        public String NotificationTypeName { get; set; }
    }
}