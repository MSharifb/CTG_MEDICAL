using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class NotesAndDocumentInfoCommentsDetailViewModel : BaseViewModel
    {
        public NotesAndDocumentInfoCommentsDetailViewModel()
        {
            this.NotesAndDocumentInfoCommentsDetailList = new List<NotesAndDocumentInfoCommentsDetailViewModel>();
        }

        #region standard properties

        [Required]
        public int NotesAndDocumentInfoAttachmentDetailId { get; set; }
        
      
        public string Comments { get; set; }
      
        [DisplayName("Employee ID")]
        public int CommentByEmployeeId { get; set; }

        #endregion


        #region other properties
        public string CommentByEmpId { get; set; }

        [DisplayName("Comment By")]
        public string Employee { get; set; }

        
        public string Designation { get; set; }


        public IList<NotesAndDocumentInfoCommentsDetailViewModel> NotesAndDocumentInfoCommentsDetailList { get; set; }
        #endregion
    }
}