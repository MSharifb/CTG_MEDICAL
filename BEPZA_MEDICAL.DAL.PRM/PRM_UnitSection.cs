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
    
    public partial class PRM_UnitSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        public string Remarks { get; set; }
    }
}