using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public class BankAdviceLetterDetailCustomModel
    {

        public Int64 Id { get; set; }
        public Int64 BankLetterId { get; set; }
        public int EmployeeId { get; set; }
        public string EmpID { get; set; }
        public string AccountNo { get; set; }
        public decimal NetPayable { get; set; }

        public bool Ischecked { get; set; }

        public string FullName { get; set; }

        public string EmployeeInitial { get; set; }

        public string BankName { get; set; }
    }
}
