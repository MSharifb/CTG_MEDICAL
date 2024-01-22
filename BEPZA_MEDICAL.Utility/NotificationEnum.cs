using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_BEPZA.Utility
{
    public  static class NotificationEnum
    {
        public enum NotificationType
        {
            Employee_Transfer = 1,
            Employee_Promotion,
            Employee_Demotion,
            Salary_Increment,
            PF_Statement_Generation,
            Employee_Suspend,
            Employee_Retirement,
            General_Purpose,
            From_Approval_Flow,
            Employee_Separation,
            Employee_Confirmation
        }

        
        public enum  NotificationReadStatus
        {
            Read = 1,
            Unread,
            All
        }
        
        public enum NotificationModule
        {
            Payroll_Management_System = 6,
            Human_Resource_Management_System = 3,
            Inventory_Management_System = 15,
            Provident_Fund_Management_System = 7,
            Fund_Management_System = 19,
            Welfare_Fund_Management_System = 18,
            Project_Management_System = 16,
            Fixed_Asset_Management_System = 17,
            Loan_Management_System = 25,
            Leave_Management_System = 8,
            Security_And_Ansar_Management_System = 20
        }
    }
}
