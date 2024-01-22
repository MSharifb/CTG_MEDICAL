using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.VoucherSearchModel
{
    public class VoucherSearchViewModel
    {
        #region Standard properties-----------------

        public int Id { get; set; }

        public string VoucherNumber { get; set; }

        public string VoucherDate { get; set; }

        public string ReferenceNumber { get; set; }

        public int ProjectId { get; set; }

        public decimal Debit { get; set; }

        public string ProjectNo { get; set; }   

        public string ProjectTitle { get; set; }


        #endregion

        #region Others

        public int SelectedStatus { get; set; }

        public string ActionName { get; set; }

        #endregion

        #region For PL/PS-----------

        #endregion 
    }
}