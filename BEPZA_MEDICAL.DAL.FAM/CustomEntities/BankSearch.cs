using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEPZA_MEDICAL.DAL.FAM.CustomEntities
{
    public class BankSearch
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string BankAc { get; set; }
        public string AccountHead { get; set; }
        public string SWIFTCode { get; set; }
        public string BankAddr { get; set; }
    }
}
