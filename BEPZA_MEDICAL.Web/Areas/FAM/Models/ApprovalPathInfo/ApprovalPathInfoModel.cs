using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.ApprovalPathInfo
{
    public class ApprovalPathInfoModel
    {
        #region Ctor

        public ApprovalPathInfoModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            
            this.Mode = "Create";
            this.ApprovalPathSearch = new List<ApprovalPathSearch>();
            this.ApprovalPathDetails = new List<ApprovalPathDetails>();

            this.DesignationList = new List<SelectListItem>();
            this.EmpList = new List<SelectListItem>();

        }

        #endregion

        #region Standard Property

        public int PathId { get; set; }

        [DisplayName("Path Name")]
        [Required]
        public string PathName { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        #endregion

        #region Others

        public IList<ApprovalPathSearch> ApprovalPathSearch { get; set; }
        public IList<ApprovalPathDetails> ApprovalPathDetails { get; set; }

        [DisplayName("Node Type")]
        public string ApprovalTypeId { get; set; }
        public IList<SelectListItem> ApprovalTypeList
        {
            get
            {
                var approvalType = new List<SelectListItem>();
                approvalType.Add(new SelectListItem() { Text = "Approver", Value = "Approver" });
                approvalType.Add(new SelectListItem() { Text = "Recommender", Value = "Recommender" });
                return approvalType;
            }
        }

        [DisplayName("Role/Designation")]
        public int? DesignationId { get; set; }
        public IList<SelectListItem> DesignationList { get; set; }

        [DisplayName("Node")]
        public int? EmpId { get; set; }
        public IList<SelectListItem> EmpList { get; set; }

        [DisplayName("Node Order")]
        public int? NodeOrder { get; set; }

        public string Mode {get;set;}
        #endregion
    }

    public class ApprovalPathDetails
    {
        #region Ctor

        public ApprovalPathDetails()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.NodeOrder = 1;
            this.ParentNodeEmpId = null;
        }

        #endregion

        #region Standard Property

        public int NodeId { get; set; }
        public int PathId { get; set; }

        public string ApprovalType { get; set; }
        public int RoleDesignationId { get; set; }
        public int NodeEmpId { get; set; }
        public int NodeOrder { get; set; }
        
        public int? ParentNodeEmpId { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        #endregion

        #region Others

        public string EmpName { get; set; }        

        #endregion
    }

    public class ApprovalPathSearch
    {
        public int Id { get; set; }
        public string PathName { get; set; }
        public string Description { get; set; }
    }
}