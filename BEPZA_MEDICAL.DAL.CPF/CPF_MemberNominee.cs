//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BEPZA_MEDICAL.DAL.CPF
{
    using System;
    using System.Collections.Generic;
    
    public partial class CPF_MemberNominee
    {
        public int Id { get; set; }
        public int MembershipId { get; set; }
        public string NomineeName { get; set; }
        public string NomineeAddress { get; set; }
        public string Relationship { get; set; }
        public System.DateTime DateOfBirth { get; set; }
        public decimal PercentOfShare { get; set; }
        public string GuardianName { get; set; }
        public string GuardianAddress { get; set; }
        public string FileName { get; set; }
        public byte[] Photograph { get; set; }
        public string FileExtension { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual CPF_MembershipInfo CPF_MembershipInfo { get; set; }
    }
}
