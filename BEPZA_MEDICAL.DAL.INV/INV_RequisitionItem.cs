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
    
    public partial class INV_RequisitionItem
    {
        public int Id { get; set; }
        public int RequisitionId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string Comment { get; set; }
        public Nullable<decimal> ApproveQuantity { get; set; }
    
        public virtual INV_ItemInfo INV_ItemInfo { get; set; }
        public virtual INV_RequisitionInfo INV_RequisitionInfo { get; set; }
    }
}
