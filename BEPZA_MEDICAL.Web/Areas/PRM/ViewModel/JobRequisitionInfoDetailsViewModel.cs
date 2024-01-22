using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class JobRequisitionInfoDetailsViewModel : BaseViewModel
    {
        #region Ctor
        public JobRequisitionInfoDetailsViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
        }
        #endregion

        #region Standard Property

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public int SalaryScaleId { get; set; }
        public string SalaryScaleName { get; set; }
        public DateTime? RequireDate { get; set; }
        public int EmploymentTypeId { get; set; }
        public int NumOfRequiredPost { get; set; }
        public string JobDescription { get; set; }
        public string EduRequirement { get; set; }
        public string ExpRequirement { get; set; }
        public string AdditionalRequirement { get; set; }

        #endregion
    }
}