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
    
    public partial class vwEmployeeFutureJobConfirmation
    {
        public int Id { get; set; }
        public string EmployeeID { get; set; }
        public string Initial { get; set; }
        public string Name { get; set; }
        public string Designaton { get; set; }
        public System.DateTime JoiningDate { get; set; }
        public int ProvisionMonth { get; set; }
        public Nullable<System.DateTime> ConfirmationDate { get; set; }
    }
}
