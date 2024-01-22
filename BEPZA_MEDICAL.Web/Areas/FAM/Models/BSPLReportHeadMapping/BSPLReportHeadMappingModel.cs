using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.BSPLReportHeadMapping
{
    public class BSPLReportHeadMappingModel
    {
        #region Ctor
        public BSPLReportHeadMappingModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.ReportNameList = new List<SelectListItem>();
            this.BSPLReportHeadList = new List<SelectListItem>();
            this.BSPLReportAccountHeadList = new List<BSPLReportAccountHead>();

            this.Mode = "Create";
        }
        #endregion

        #region Standard Property
        public int Id { get; set; }

        [DisplayName("Report Name")]
        [Required]
        public int ReportId { get; set; }
        public string ReportName { get; set; }

        [DisplayName("BSPL/Report Head")]
        [Required]
        public int BSPLReportHeadId { get; set; }
        public string BSPLReportHeadName { get; set; }

        [DisplayName("Report Head Serial")]
        public int ReportHeadSerial { set; get; }

        [DisplayName("Remarks")]
        [UIHint("_MultiLine")]
        [StringLength(500)]
        //[MaxLength(500, ErrorMessage = "The field Remarks must be string with a maximum length of 500")] 
        //[MaxLength(500)]
        public string Remarks { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        public IList<SelectListItem> ReportNameList { get; set; }
        public IList<SelectListItem> BSPLReportHeadList { get; set; }
        #endregion

        #region Other
        public string Mode { get; set; }
        public ICollection<BSPLReportAccountHead> BSPLReportAccountHeadList { get; set; }
        #endregion
    }

    public class BSPLReportAccountHead
    {
        #region Ctor
        public BSPLReportAccountHead()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
        }
        #endregion

        #region Standard Property
        public int Id { get; set; }

        public int BSPLReportHeadMappingId { get; set; }

        public int AccountHeadId { get; set; }
        [UIHint("_ReadOnly")]
        public string AccountHeadName { get; set; }
        public string AccountHeadType { get; set; }
        public bool IsSelected { get; set; }
        public bool IsUsed { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        #endregion

    }
}