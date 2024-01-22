//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BEPZA_MEDICAL.DAL.FAR
{
    using System;
    using System.Collections.Generic;
    
    public partial class FAR_MaintenanceTypeInformation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FAR_MaintenanceTypeInformation()
        {
            this.FAR_AssetMaintenanceInformation = new HashSet<FAR_AssetMaintenanceInformation>();
        }
    
        public int Id { get; set; }
        public string MaintenanceType { get; set; }
        public bool IsPeriodicallyRepeating { get; set; }
        public Nullable<int> RedoAfter { get; set; }
        public string RedoType { get; set; }
        public string Remarks { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FAR_AssetMaintenanceInformation> FAR_AssetMaintenanceInformation { get; set; }
    }
}
