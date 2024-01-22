using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.JobAdvertisementInfo
{
    public class JobAdvertisementInfoAttachmentViewModel : BaseViewModel
    {
        public JobAdvertisementInfoAttachmentViewModel()
        {
        
        }
        #region Attachment
        public int JobAdvertisementId { set; get; }
        public byte[] Attachment { set; get; }
        public string Title { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion
    }
}