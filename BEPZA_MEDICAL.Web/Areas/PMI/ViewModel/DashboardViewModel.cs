using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel
{
    public class DashboardViewModel : BaseViewModel
    {
        #region Ctor
        public DashboardViewModel()
        {
            PieData = new List<PieData>();
        }

        #endregion

        public List<PieData> PieData {get; set;}
        
    }

    public class PieData
    { 
        public decimal? value {get; set;}
        public string color {get; set;}
        public string highlight {get; set;}
        public string label {get; set;}
    }
}