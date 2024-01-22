using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.PersonalInfo
{
    public class EmpAttachmentViewModel
    {
        #region Ctor
        public EmpAttachmentViewModel()
        {
            this.AttachmentTypeList = new List<SelectListItem>();
            this.EmpTop = new EmpTop();
        }
        #endregion

        #region Standard Property

        public virtual int Id
        {
            get;
            set;
        }

        public virtual int EmployeeId
        {
            get;
            set;
        }

        [Required()]
        [DisplayName("Attachment Type")]
        public virtual int AttachmentTypeId
        {
            get;
            set;
        }

        [Required()]
        [DisplayName("File Name")]
        public virtual string FileName
        {
            get;
            set;
        }

        public string  FileExtention { get; set; }

        public string  OriginalFileName { get; set; }

        [DisplayName("Description")]
        [MaxLength(200)]
        //[UIHint("_MultiLine")]
        public virtual string Description
        {
            get;
            set;
        }
        
        public virtual byte[] Attachment
        {
            get;
            set;
        }

        #endregion

        #region Others

        public string FileSize { get; set; }
        public string DownloadLink { get; set; }
        public IList<SelectListItem> AttachmentTypeList
        {
            get;
            set;
        }
        public EmpTop EmpTop { get; set; }
        public string Message { get; set; }
        public string ActionType { get; set; }
        public string ButtonText { get; set; }
        public bool DeleteEnable { get; set; }
        public string SideBarClassName { get; set; }
        public string ErrorClass { get; set; }
        public string SelectedClass { get; set; }

        #endregion
    }
}