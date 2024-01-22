﻿using System;
using System.Collections.Generic;

using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.PGM.ViewModel
{
    public class SalaryStructureViewModel : BaseViewModel
    {
        public SalaryStructureViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.StepList = new List<SelectListItem>();
        }
        
        [Required]
        [DisplayName("Grade")]
        public int GradeId { get; set; }
        public IList<SelectListItem> GradeList { get; set; }

        [Required]
        [DisplayName("Step")]
        public int StepId { get; set; }
        public IList<SelectListItem> StepList { get; set; }

        [Required]
        [DisplayName("Salary Scale")]
        public int SalaryScaleId { get; set; }
        public IList<SelectListItem> SalaryScaleList { get; set; }

        public bool IsConsolidated { get; set; }
        public decimal InitialBasic { get; set; }
        public decimal YearlyIncrement { get; set; }

        public virtual ICollection<SalaryStructureDetailsViewModel> SalaryStructureDetail { get; set; }

    }
}