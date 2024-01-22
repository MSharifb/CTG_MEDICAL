using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEPZA_MEDICAL.Utility
{
    public static class PRMEnum
    {
        public enum EmploymentType { Permanent = 1, Contractual, Temporary, Probationary };
        public enum EmploymentStatus { Active = 1, Inactive, OnLeave };
        public enum EmployeeStatusChange { Confirmation = 1, Increment, Promotion, Demotion, SelectionGrade, Transfer };

        /// <summary>
        /// DivisionId = Department; DisciplineId = Office
        /// </summary>
        public enum EmployeeOrganogramDynamicLabel { ExecutiveOfficeOrZoneId = 1, DivisionId, SectionId, OfficeId, SubSectionId };

        public enum EmployeeOrganogram { ExecutiveOfficeOrZone = 1, Department, Section, Office, SubSection };

        public enum PunishmentRestrictionName { Promotion = 1, Increment, Transfer, Bonus, SalaryDeduction };

        public enum DepartmentalProceedingOrderType { ShowcauseNotice = 1, FIR, ChargeSheet, AppealReview, FinalOrder, };

        public enum FixedPercent { Fixed = 1, Percent }

        public enum EmpStatus { Active = 1, Inactive , OnLeave , Suspended }


    }
}
