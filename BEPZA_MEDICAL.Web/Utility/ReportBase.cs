using System;
using BEPZA_MEDICAL.DAL.AMS;
using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.DAL.FAM;
using BEPZA_MEDICAL.DAL.FAR;
using BEPZA_MEDICAL.DAL.FMS;
using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.DAL.PGM;
using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.WFM;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Project;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class ReportBase : Page
    {
        #region Fields-------------

        internal readonly ERP_BEPZAPRMEntities context;
        internal readonly ERP_BEPZAFAMEntities famContext;
        internal readonly ERP_BEPZAPGMEntities _pgmContext;
        internal readonly ERP_BEPZACPFEntities cpfContext;
        internal readonly ERP_BEPZAPRMEntities prmContext;
        internal readonly ERP_BEPZA_AMSEntities amsContext;
        internal readonly ERP_BEPZAPMIEntities pmiContext;
        internal readonly ERP_BEPZAWFMEntities wfmContext;
        internal readonly ERP_BEPZAINVEntities invContext;
        internal readonly ERP_BEPZAFMSEntities fmsContext;
        internal readonly ERP_BEPZAFAREntities farContext;
        internal readonly Common _common;
        internal readonly PGM_ExecuteFunctions _pgmExecuteFunctions;

        #endregion

        #region Consturctor--------------

        public ReportBase()
        {
            context = new ERP_BEPZAPRMEntities();
            prmContext = new ERP_BEPZAPRMEntities();
            famContext = new ERP_BEPZAFAMEntities();
            _pgmContext = new ERP_BEPZAPGMEntities();
            cpfContext = new ERP_BEPZACPFEntities();
            prmContext = new ERP_BEPZAPRMEntities();
            amsContext = new ERP_BEPZA_AMSEntities();
            pmiContext = new ERP_BEPZAPMIEntities();
            wfmContext = new ERP_BEPZAWFMEntities();
            invContext = new ERP_BEPZAINVEntities();
            fmsContext = new ERP_BEPZAFMSEntities();
            farContext = new ERP_BEPZAFAREntities();
            _common = new Common();
            _pgmExecuteFunctions = new PGM_ExecuteFunctions(_pgmContext);

        }


        public string GetEmployeeNameByEmpID(string EmpID)
        {
            string EmployeeName = default(string);

            var EmpName = (from tr in prmContext.PRM_EmploymentInfo
                           where tr.EmpID.Trim() == EmpID.Trim()
                           select tr.FullName).FirstOrDefault();
            if (EmpName != null)
            {
                EmployeeName = EmpName;
            }
            return EmployeeName;
        }

        public int LoggedUserZoneInfoId
        {
            get { return MyAppSession.ZoneInfoId; }
        }

        public string GetProjectTitle(string projectNo)
        {
            string projectTitle = string.Empty;
            //var project = (from tr in pimContext.PIM_ProjectInfo
            //               where tr.ProjectNo.Trim() == projectNo.Trim()
            //               select tr.ProjectTitle).FirstOrDefault();
            //if (project != null)
            //{
            //    projectTitle = project;
            //}
            return projectTitle;
        }
        CustomMembershipProvider _provider = new CustomMembershipProvider();
        public IList<SelectListItem> GetZoneDDL()
        {
            var ddlList = _provider.GetZoneList(HttpContext.Current.User.Identity.Name, LoggedUserZoneInfoId);

            HashSet<int> zoneIDs = new HashSet<int>(ddlList.Select(s => s.ZoneId));
            var ddlFilterList = prmContext.PRM_ZoneInfo.Where(x => zoneIDs.Contains(x.Id)).OrderBy(s => s.SortOrder).ToList();

            var list = new List<SelectListItem>();
            foreach (var item in ddlFilterList)
            {
                if (ddlList.Count == 1)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = item.ZoneName,
                        Value = item.Id.ToString(),
                        Selected = true
                    });
                }
                else
                {
                    list.Add(new SelectListItem()
                    {
                        Text = item.ZoneName,
                        Value = item.Id.ToString()
                    });
                }

            }

            return list.DistinctBy(x => x.Value).ToList();

        }

        public string ConvertZoneArrayListToString(int[] array)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var value in array)
            {
                builder.Append(value);
                builder.Append(',');
            }
            if (builder.Length > 0) builder.Length--;
            return builder.ToString();
        }

        public dynamic GetZoneInfoForReportHeader()
        {
            return from c in _pgmExecuteFunctions.GetZoneInfoList()
                   where c.Id == LoggedUserZoneInfoId
                   select new
                   {
                       c.CompanyName,
                       c.CompanyLogo,
                       ZoneId = c.Id,
                       c.ZoneName,
                       c.ZoneAddress,
                       c.ZoneCode,
                       c.IsHeadOffice
                   };
        }

        #endregion

        #region Project Module Report Session ---------------

        public static string BudgetInformationSession = @"BudgetInformationSession";

        public static string TenderNoticeSession = @"SessionTenderNotice";

        public static string ProcurementPlanSession = @"SessionProcurementPlan";

        public static string AnnualProcurementPlanSession = @"SessionAnnualProcurementPlan";

        public static List<BudgetSummaryParamViewModel> BudgetSummaryParamList { get; set; }

        #endregion


        public IList<SelectListItem> ZoneListCached
        {
            get
            {
                return GetZoneDDL();
            }
        }
    }

    public class PGMReportBase : Page
    {
        #region Fields-------------

        internal readonly ERP_BEPZAPGMEntities _pgmContext;
        internal readonly Common _common;
        internal readonly PGM_ExecuteFunctions _pgmExecuteFunctions;

        #endregion

        #region Consturctor--------------

        public PGMReportBase()
        {
            _pgmContext = new ERP_BEPZAPGMEntities();
            _common = new Common();
            _pgmExecuteFunctions = new PGM_ExecuteFunctions(_pgmContext);
        }

        public dynamic GetZoneInfoForReportHeader()
        {
            return from c in _pgmExecuteFunctions.GetZoneInfoList()
                   where c.Id == LoggedUserZoneInfoId
                   select new
                   {
                       c.CompanyName,
                       c.CompanyLogo,
                       ZoneId = c.Id,
                       c.ZoneName,
                       c.ZoneAddress,
                       c.ZoneCode,
                       c.IsHeadOffice
                   };
        }

        public string GetEmployeeNameByEmpID(string EmpID)
        {
            string EmployeeName = default(string);

            var EmpName = _pgmExecuteFunctions.GetEmployeeByEmpId(EmpID).FullName;
            if (EmpName != null)
            {
                EmployeeName = EmpName;
            }
            return EmployeeName;
        }

        public int LoggedUserZoneInfoId
        {
            get { return MyAppSession.ZoneInfoId; }
        }

        CustomMembershipProvider _provider = new CustomMembershipProvider();
        public IList<SelectListItem> GetZoneDDL()
        {
            var ddlList = _provider.GetZoneList(HttpContext.Current.User.Identity.Name, LoggedUserZoneInfoId);

            HashSet<int> zoneIDs = new HashSet<int>(ddlList.Select(s => s.ZoneId));
            var ddlFilterList = _pgmContext.vwPGMZoneInfoes.Where(x => zoneIDs.Contains(x.Id)).OrderBy(s => s.SortOrder).ToList();

            var list = new List<SelectListItem>();
            foreach (var item in ddlFilterList)
            {
                if (ddlList.Count == 1)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = item.ZoneName,
                        Value = item.Id.ToString(),
                        Selected = true
                    });
                }
                else
                {
                    list.Add(new SelectListItem()
                    {
                        Text = item.ZoneName,
                        Value = item.Id.ToString()
                    });
                }

            }

            return list.DistinctBy(x => x.Value).ToList();

        }

        public string ConvertZoneArrayListToString(int[] array)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var value in array)
            {
                builder.Append(value);
                builder.Append(',');
            }
            if (builder.Length > 0) builder.Length--;
            return builder.ToString();
        }

        #endregion

        public IList<SelectListItem> ZoneListCached { get { return GetZoneDDL(); } }


        public class FilteredZoneList
        {
            public int Id { get; set; }
            public String ZoneName { get; set; }

            public FilteredZoneList(int id, string zoneName)
            {
                this.Id = id;
                this.ZoneName = zoneName;
            }
        }
    }
}