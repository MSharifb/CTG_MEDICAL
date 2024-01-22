using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Approval
{
    public class WFMAttachmentViewModel
    {
        public byte[] Attachment { set; get; }
        public string Title { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}