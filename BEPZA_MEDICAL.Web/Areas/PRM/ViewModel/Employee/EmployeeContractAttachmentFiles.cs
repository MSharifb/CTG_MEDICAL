using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Employee
{
    public class EmployeeContractAttachmentFiles
    {
        public EmployeeContractAttachmentFiles()
        {
        
        }

        public int Id { get; set; }

        public int EmpContactInfoId { get; set; }

        public string UserFileName { get; set; }

        public string OriginalName { get; set; }

        public string Attachment { get; set; } 

        public string ContentType { get; set; }

        public Int64 Size { get; set; }

        public byte[] Data { get; set; }

    }
}