using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class QueryAnalyzerItemsViewModel
    {
        public QueryAnalyzerItemsViewModel()
        {

        }
        public bool IsSelected { get; set; }
        public string CategoryTable { get; set; }
        public string CategoryTableName { get; set; }
        public string CategoryColumn { get; set; }
        public string CategoryColumnName { get; set; }
    }
}