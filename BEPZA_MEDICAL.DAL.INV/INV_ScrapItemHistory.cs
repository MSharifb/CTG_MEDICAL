//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BEPZA_MEDICAL.DAL.INV
{
    using System;
    using System.Collections.Generic;
    
    public partial class INV_ScrapItemHistory
    {
        public int Id { get; set; }
        public int ScrapId { get; set; }
        public int ScrapItemId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string Comment { get; set; }
        public int ItemStatusId { get; set; }
        public string IUser { get; set; }
        public System.DateTime IDate { get; set; }
        public string EUser { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    }
}
