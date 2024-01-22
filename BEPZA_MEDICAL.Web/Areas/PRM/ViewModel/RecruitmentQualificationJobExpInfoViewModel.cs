using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class RecruitmentQualificationJobExpInfoViewModel : BaseViewModel
    {
        #region Ctor
        public RecruitmentQualificationJobExpInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
        }
        #endregion

        #region Standard Property

        public int? RecruitmentQualificationDetailsId { get; set; }

        public decimal? YearOfExp { get; set; }
        public string OnBy { get; set; }
        public int? JobGradeId { get; set; }
        public int? PostId { get; set; }
        public int? ProfessionalCertificateId { get; set; }
        public decimal? TotalYearOfExp { get; set; }
        public string Remarks { get; set; }

        #endregion

        #region Other
        public string OnType { get; set; }
        #endregion

    }
}