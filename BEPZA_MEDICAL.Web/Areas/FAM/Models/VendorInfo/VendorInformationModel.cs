using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.VendorInfo
{
    public class VendorInformationModel
    {
        #region Ctor
        public VendorInformationModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.Mode = "Create";
            this.IsActive = true;
        }
        #endregion

        #region Standard Property

        public int Id { get; set; }

        [DisplayName("Vendor Name")]
        [Required]
        //[MaxLength(100)]
        [StringLength(100)]
        public string VendorName { get; set; }

        [DisplayName("Contact Person")]
        //[MaxLength(50)]
        [StringLength(50)]
        public string ContactPerson { get; set; }

        [DisplayName("Contact Number")]
        //[MaxLength(50)]
        [StringLength(50)]
        public string ContactNumber { get; set; }

        [DisplayName("Address")]
        //[MaxLength(250)]
        [StringLength(250)]
        public string Address { get; set; }

        [DisplayName("E-Mail")]
        //[MaxLength(100)]
        [StringLength(100)]
      //  [Email]
        public string EMail { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }
        #endregion

        #region Other
        public string Mode { get; set; }
        #endregion
    }
}