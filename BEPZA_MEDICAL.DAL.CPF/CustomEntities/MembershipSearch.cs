using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEPZA_MEDICAL.DAL.CPF.CustomEntities
{
   public class MembershipSearch
    {
        public Int32 ID { get; set; }
        public string EmpId { get; set; }
        public string EmpInitial { get; set; }
        public string EmpName { get; set; }
        public int? DesignationId { get; set; }
        public string DesigName { get; set; }
        public int EmpTypeId { get; set; }
        public string EmpTypeName { get; set; }
        public int? DivisionId { get; set; }
        public string DivisionName { get; set; }
        public int? JobLocationId { get; set; }
        public string JobLocName { get; set; }
        public int? GradeId { get; set; }
        public string GradeName { get; set; }
        public int StaffCategoryId { get; set; }
        public string StaffCatName { get; set; }
        public int? ResourceLevelId { get; set; }
        public string ResLevelName { get; set; }
        public DateTime DateOfJoining { get; set; }
        public DateTime? DateOfConfirmation { get; set; }
        public DateTime? DateOfInactive { get; set; }
        public bool IsContractual { get; set; }
    }
}
