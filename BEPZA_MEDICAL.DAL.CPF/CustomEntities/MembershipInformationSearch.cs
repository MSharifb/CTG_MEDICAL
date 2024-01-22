using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEPZA_MEDICAL.DAL.CPF.CustomEntities
{
    public class MembershipInformationSearch
    {
        public int Id { get; set; }
        public int EmployeeCode { get; set; }         
        public string EmployeeName { get; set; }
        public string EmployeeInitial { get; set; }
        public string MembershipID { get; set; }
        public string MembershipStatus { get; set; }
        
        public string Designation { get; set; }
        public DateTime DateofConfirmation { get; set; }
        public DateTime DateofJoining { get; set; }
        
    }
}
