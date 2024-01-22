using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.JobAdvertisementInfo
{
    public class JobAdvertisementInfoRequisitionViewModel : BaseViewModel
    {
        public int RequisitionInfoSummaryDetailId { get; set; }
        public string Designation { get; set; }
        public int DesignationId { get; set; }
        public bool IsCheckedFinal { get; set; }
        public string RequisitionNo { get; set; }
        public int NumberOfRequiredPost { get; set; }
        public string PayScale { get; set; }
        public string DepartmentName { get; set; }
        public string SectionName { get; set; }
        public int? RecommendPost { get; set; }
        public int? ApprovedPost { get; set; }
        public int RequisionId { get; set; }
        public int RequisitionInfoClearanceId { get; set; }
    }
}