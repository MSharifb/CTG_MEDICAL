using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailNotificationService
{
    public class Program
    {
        static void Main()
        {
            Console.Title = "IWM E-mail Notification Service";
            
            var logFilesWritting = new LogFilesWritting();
            var obj = new EmailNotification();
            
            try
            {
                obj.NotifyEmailServices();
            }
            catch (Exception ex)
            {
                logFilesWritting.LogInfo(ex.Message);
            }
        }
    }
}
