using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Utility;
using System.Web.Helpers;
using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.Domain.PMI;
using System.Collections;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel;
using BEPZA_MEDICAL.Domain.FAM;
using BEPZA_MEDICAL.Domain.PRM;

namespace BEPZA_MEDICAL.Web.Areas.PMI.Controllers
{
    [NoCache]
    public class HomeController : BaseController
    {
        private readonly PMICommonService _pmiCommonService;
        private readonly FAMCommonService _famCommonService;
        private readonly PRMCommonSevice _prmCommonService;

        public HomeController(PMICommonService pmiCommonServices, FAMCommonService famCommonService, PRMCommonSevice prmCommonService)
        {
            _pmiCommonService = pmiCommonServices;
            _famCommonService = famCommonService;
            _prmCommonService = prmCommonService;
        }

        [NoCache]
        public ActionResult Index()
        {
            return View();
        }

        #region Project Section
        public ActionResult ProjectSection(ProjectSectionViewModel model)
        {
            model.ProjectSectionList = PopulateDllList(_pmiCommonService.PMIUnit.ProjectForRepository.GetAll().ToList());
            return View(model);
        }

        public bool SetProjectSection(int Id)
        {
            System.Web.HttpContext.Current.Session["ProjectSectionId"] = Id;
            if(Id == 1)
                System.Web.HttpContext.Current.Session["ProjectSectionName"] = "Civil";
            else if(Id == 2)
                System.Web.HttpContext.Current.Session["ProjectSectionName"] = "Maintenance";

            return true;
        }

        public static IList<SelectListItem> PopulateDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }
        #endregion

        public ActionResult Unauthorized()
        {
            return View();
        }

        public JsonResult BudgetBarChart(int? fyId, int? atId)
        {
            var financialYearId = 0;
            var approvalTypeId = 0;

            if (fyId != null)
            {
                financialYearId = Convert.ToInt32(fyId);
            }
            else
            {
                financialYearId = _famCommonService.FAMUnit.FinancialYearInformationRepository.Get(x => x.IsActive == true).FirstOrDefault().Id;
            }

            if (atId != null)
            {
                approvalTypeId = Convert.ToInt32(atId);
            }
            else
            {
                approvalTypeId = _pmiCommonService.PMIUnit.StatusInformationRepository.Get(x => x.Name == "Approved").FirstOrDefault().Id;
            }

            var list = new List<sp_PMI_ChartBudgetVariance_Result>();

            using (var pmiContext = new ERP_BEPZAPMIEntities())
            {
                list = pmiContext.sp_PMI_ChartBudgetVariance(financialYearId, approvalTypeId).ToList();
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BudgetPieChart(int? fyId, int? atId)
        {
            var financialYearId = 0;
            var approvalTypeId = 0;

            if (fyId != null)
            {
                financialYearId = Convert.ToInt32(fyId);
            }
            else
            {
                financialYearId = _famCommonService.FAMUnit.FinancialYearInformationRepository.Get(x => x.IsActive == true).FirstOrDefault().Id;
            }

            if (atId != null)
            {
                approvalTypeId = Convert.ToInt32(atId);
            }
            else
            {
                approvalTypeId = _pmiCommonService.PMIUnit.StatusInformationRepository.Get(x => x.Name == "Approved").FirstOrDefault().Id;
            }

            var listPieData = new DashboardViewModel();
            var list = new List<sp_PMI_ChartBudgetVariance_Result>();
            var listColor = new string[] { "#00c0ef", "#d2d6de", "#3c8dbc", "#16a085", "#00a65a", "#2ecc71", "#9b59b6", "#f39c12", "#f56954" };

            using (var pmiContext = new ERP_BEPZAPMIEntities())
            {
                list = pmiContext.sp_PMI_ChartBudgetVariance(financialYearId, approvalTypeId).ToList();
            }

            for (int i = 0; i < list.Count(); i++)
            {
                var pieData = new PieData();

                pieData.label = list[i].ZoneName;
                pieData.value = list[i].BudgetAmount;
                pieData.color = listColor[i];
                pieData.highlight = listColor[i];

                listPieData.PieData.Add(pieData);
            }
            return Json(listPieData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProjectStatus(int? fyId)
        {
            var financialYearId = 0;

            if (fyId != null)
            {
                financialYearId = Convert.ToInt32(fyId);
            }
            else
            {
                financialYearId = _famCommonService.FAMUnit.FinancialYearInformationRepository.Get(x => x.IsActive == true).FirstOrDefault().Id;
            }

            var listProjectStatus = new List<sp_PMI_ProjectStatus_Result>();

            using (var pmiContext = new ERP_BEPZAPMIEntities())
            {
                listProjectStatus = pmiContext.sp_PMI_ProjectStatus(financialYearId, GetZoneList()).ToList();
            }
            return Json(listProjectStatus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BudgetStatus(int? fyId, int? atId)
        {
            var financialYearId = 0;
            var approvalTypeId = 0;

            if (fyId != null)
            {
                financialYearId = Convert.ToInt32(fyId);
            }
            else
            {
                financialYearId = _famCommonService.FAMUnit.FinancialYearInformationRepository.Get(x => x.IsActive == true).FirstOrDefault().Id;
            }

            if (atId != null)
            {
                approvalTypeId = Convert.ToInt32(atId);
            }
            else
            {
                approvalTypeId = _pmiCommonService.PMIUnit.StatusInformationRepository.Get(x => x.Name == "Approved").FirstOrDefault().Id;
            }

            var listBudgetStatus = new List<sp_PMI_BudgetStatus_Result>();

            using (var pmiContext = new ERP_BEPZAPMIEntities())
            {
                listBudgetStatus = pmiContext.sp_PMI_BudgetStatus(financialYearId, approvalTypeId, GetZoneList()).ToList();
            }
            return Json(listBudgetStatus, JsonRequestBehavior.AllowGet);
        }

        private string GetZoneList()
        {
            var strZoneList = string.Empty;

            CustomMembershipProvider _provider = new CustomMembershipProvider();
            var zoneList = _provider.GetZoneList(System.Web.HttpContext.Current.User.Identity.Name, LoggedUserZoneInfoId).Select(x => x.ZoneId);

            strZoneList = string.Join(",", zoneList);

            return strZoneList;
        }

        [HttpGet]
        public virtual JsonResult LoadApprovalTypeDDL()
        {
            var listApprovalType = _pmiCommonService.PMIUnit.StatusInformationRepository.GetAll().Where(x => x.ApplicableFor == "Budget").ToList();
            return Json(listApprovalType, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual JsonResult LoadFinancialYearDDL()
        {
            var listFinancialYear = _famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll().OrderByDescending(t => t.EDate).Select(x => new { x.Id, x.FinancialYearName, x.IsActive }).ToList();
            return Json(listFinancialYear, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SwitchZone(int id, string zoneSwitchURL)
        {
            MyAppSession.ZoneInfoId = id;
            MyAppSession.ZoneName = _prmCommonService.PRMUnit.ZoneInfoRepository.GetByID(id).ZoneName;
            return Redirect(zoneSwitchURL);
        }

    }
}
