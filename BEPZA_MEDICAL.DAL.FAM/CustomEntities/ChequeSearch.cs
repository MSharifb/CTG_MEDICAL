using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEPZA_MEDICAL.DAL.FAM.CustomEntities
{
    public class ChequeSearch
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public int BankId { get; set; }
        public string BankAc { get; set; }
        public string ChequeBookNumber { get; set; }
    }
}
