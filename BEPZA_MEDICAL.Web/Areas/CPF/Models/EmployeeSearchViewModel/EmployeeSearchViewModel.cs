using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.EmployeeSearchViewModel
{
    public class EmployeeSearchViewModel
    {
        #region Properties
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmpID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeInitial { get; set; }
        public string MembershipID { get; set; }
    
        public string MembershipStatus { get; set; }

        public string Designation { get; set; }
        public DateTime PermanentDate { get; set; }
        public DateTime JoiningDate { get; set; }
        public int DesignationId { get; set; } 
        public int? DivisionId { get; set; }

        public string ActionName { get; set; }
        public int EmployeeStatus { get; set; }
        public int SelectedEmployeeStatus { get; set; }
        public int? SortOrder { get; set; }
        #endregion
    }
}