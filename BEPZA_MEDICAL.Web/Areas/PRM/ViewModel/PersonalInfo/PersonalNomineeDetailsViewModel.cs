using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class PersonalNomineeDetailsViewModel
    {
        public PersonalNomineeDetailsViewModel()
        {
            this.FamilyMemberList = new List<SelectListItem>();   
        }
        public  int Id
        {
            get;
            set;
        }
        [Required]
        public int FamilyMemberId
        {
            get;
            set;
        }
        public List<SelectListItem> FamilyMemberList { get; set; }

        [Required]
        public int NomineeId
        {
            get;
            set;
        }
        [UIHint("_ReadOnly")]         
        public string Relation { get; set; }   
        public string DateOfBirth { get; set; }
         [UIHint("_ReadOnly")]
        public string Age { get; set; }

        [Required]
        public  decimal? PercentOfShare
        {
            get;
            set;
        }
         [MaxLength(100)]
        public  string Remarks
        {
            get;
            set;
        }
    }
}