using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEPZA_MEDICAL.DAL.FAM.CustomEntities
{
    public class VoucherSearch
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime VoucherDate { get; set; }
        public int? PayeeClientId { get; set; }
        public string ClientName { get; set; }
        public int? PayeeStaffId { get; set; }
        public string StaffName { get; set; }
        public int? PayeeVendorId { get; set; }
        public string VendorName { get; set; }

        public string ReceivedBy { get; set; }
        public int? ReceiveFromClientId { get; set; }
        public int? ReceiveFromStaffId { get; set; }
        public int? ReceiveFromVendorId { get; set; }
        public int? DivisionId { get; set; }
        public string DivisionName { get; set; }

        public string VoucherStatus { get; set; }
        public int VoucherTypeId { get; set; }
        public string VoucherTypeName { get; set; }
        public string CurrentLocation { get; set; }

        public DateTime VoucherDateTo { get; set; }
        public DateTime VoucherDateFrom { get; set; }
        public string Payee { get; set; }
        public int ClientId { get; set; }
        public string PendingApproval { get; set; }
        
    }
}
