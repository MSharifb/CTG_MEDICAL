using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BEPZA_MEDICAL.Domain.FAM;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.Configuration
{
    public class ConfigurationModel
    {
        #region Ctor
        public ConfigurationModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.CommonConfigTypes = new List<CommonConfigType>();
            //this.Remarks = string.Empty;
        } 
        #endregion

        #region Standard Property
        public int Id { set; get; }

        [Required]
        public string Name { set; get; }

        [DisplayName("Sort Order")]
        [Required]
        public int? SortOrder { set; get; }

        [UIHint("_MultiLine")]
        public string Remarks { set; get; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime? EDate { get; set; } 
        #endregion

        #region Common Property
        public string ActiveName { get; set; }
        #endregion

        public IList<CommonConfigType> CommonConfigTypes { get; set; }
    }

    public class CommonConfigType
    {
        public string DisplayName { get; set; }
        public int? SortOrder { get; set; }
    }
}