using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class NotesAndDocumentInfoAttachmentDetailViewModel : BaseViewModel
    {
        #region Ctor
        public NotesAndDocumentInfoAttachmentDetailViewModel()
        {
            //  this.NotesAndDocumentInfoCommentsDetailList = new List<NotesAndDocumentInfoCommentsDetailViewModel>();
        }

        #endregion

        #region Standard Property

        [Required]
        public int NotesAndDocumentInfoId { get; set; }

        [Display(Name = "Ref. No.")]
        public string RefNo { get; set; }
        [UIHint("_Date")]
        public DateTime? Date { get; set; }

        public string Subject { get; set; }

        [UIHint("_Multiline")]
        [MaxLength(500)]
        public string Details { get; set; }

        #region Attachment

        [Display(Name = "Attachment")]
        public bool IsAddAttachment { set; get; }
        public byte[] Attachment { set; get; }

        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public int? FileSize { get; set; }
        #endregion

        #endregion

        #region Other
        public IList<NotesAndDocumentInfoCommentsDetailViewModel> NotesAndDocumentInfoCommentsDetailList { get; set; }
        #endregion
    }
}