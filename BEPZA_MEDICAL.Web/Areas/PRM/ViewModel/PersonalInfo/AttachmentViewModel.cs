using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MFS_IWM.Web.Areas.PRM.ViewModel.PersonalInfo
{
    public class AttachmentViewModel
    {
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
        private int _employeeId;

        public virtual int AttachmentTypeId
        {
            get;
            set;
        }
        private int _attachmentTypeId;

        public virtual string FileName
        {
            get;
            set;
        }

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
    }
}