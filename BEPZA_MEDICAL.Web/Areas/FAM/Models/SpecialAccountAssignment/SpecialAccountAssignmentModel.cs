using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.SpecialAccountAssignment
{
    public class SpecialAccountAssignmentModel
    {
        #region Ctor
        public SpecialAccountAssignmentModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.AccountHeadList = new List<SelectListItem>();
            this.PurposeList = new List<SelectListItem>();

            this.Mode = "Create";
        }
        #endregion

        #region Standard Property
        public int Id { get; set; }

        [DisplayName("Purpose")]
        [Required]
        public int PurposeId { get; set; }
        public string PurposeName { get; set; }

        [DisplayName("Account Head")]
        [Required]
        public int AccountHeadId { get; set; }
        public string AccountHeadName { get; set; }

        [DisplayName("Remarks")]
        [UIHint("_MultiLine")]
        [MaxLength(500, ErrorMessage = "The field Remarks must be string with a maximum length of 500")]
        public string Remarks { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        public IList<SelectListItem> AccountHeadList { get; set; }
        public IList<SelectListItem> PurposeList { get; set; }
        #endregion

        #region Other
        public string Mode { get; set; }
        #endregion
    }
}