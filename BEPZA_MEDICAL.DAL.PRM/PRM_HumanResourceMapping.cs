//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BOM_MPA.DAL.PRM
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRM_HumanResourceMapping
    {
        public string Type { get; set; }
        public int ResourceTypeId { get; set; }
        public int ResourceCategoryId { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual PRM_ResourceCategory PRM_ResourceCategory { get; set; }
        public virtual PRM_ResourceType PRM_ResourceType { get; set; }
    }
}