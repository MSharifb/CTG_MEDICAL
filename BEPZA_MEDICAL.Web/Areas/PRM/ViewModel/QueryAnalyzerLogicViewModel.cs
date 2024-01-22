using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class QueryAnalyzerLogicViewModel
    {
        public string TableName { get; set; }
        public bool IsSelected { get; set; }
        public string Item { get; set; }
        public string ItemName { get; set; }
        public string ScodeStart { get; set; }
        public string ScopeStartName { get; set; }
        public string ScopeLogic { get; set; }
        public string ScopeEnd { get; set; }
        public string ScopeEndName { get; set; }
        public string SortType { get; set; }
    }
}