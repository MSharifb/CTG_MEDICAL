using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

using BEPZA_MEDICAL.Web.Utility;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class SalaryScaleViewModel : BaseViewModel
    {
        #region Ctor
        public SalaryScaleViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            JobGradeDetails = new List<JobGradeViewModel>();
        }
        #endregion

        #region Standard

        [DisplayName("Salary Scale Name")]
        [Required]
        public string SalaryScaleName { set; get; }

        [DisplayName("Date of Circulation")]
        [UIHint("_Date")]
        [Required]
        public DateTime? DateOfCirculation { set; get; }

        [DisplayName("Date of Effective")]
        [UIHint("_Date")]
        [Required]
        public DateTime? DateOfEffective { set; get; }

        #endregion
       
        #region Others
        public IList<JobGradeViewModel> JobGradeDetails { get; set; }
        #endregion


    }
}