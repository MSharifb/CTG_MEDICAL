using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PMI;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ApprovalFlow;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Budget;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.Controllers
{
    public class BudgetController : BaseController
    {
        #region Declaration

        private readonly PMICommonService _pmiCommonService;
        private readonly PRMCommonSevice _prmCommonService;

        #endregion

        #region Ctrl
        public BudgetController(PMICommonService pmiCommonServices, PRMCommonSevice prmCommonService)
        {
            _pmiCommonService = pmiCommonServices;
            _prmCommonService = prmCommonService;
        }

        #endregion

        #region Index and List
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NonDevlopmentBudget()
        {
            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, BudgetMasterViewModel model, string budgetType)
        {
            string filterExpression = String.Empty;

            int totalRecords = 0;
            int loggedUserZoneId = LoggedUserZoneInfoId;
            int ProjectForId = Convert.ToInt32(Session["ProjectSectionId"]);

            //List<BudgetMasterViewModel> prelist = (from x in _pmiCommonService.PMIUnit.BudgetViewRepository.Get(q => q.ZoneId == loggedUserZoneId && q.ProjectForId == ProjectForId).DefaultIfEmpty().OfType<vwPMIBudgetInfo>().ToList()
            //                                       where x.BudgetType == budgetType
            //                                       select new BudgetMasterViewModel
            //                                       {
            //                                           Id = x.BudgetId,
            //                                           BudgetDetailId = x.BudgetDetailsId,
            //                                           FinancialYearId = x.FinancialYearId,
            //                                           FinancialYearName = x.FinancialYearName,
            //                                           NameOfWorks = x.NameOfWorks,
            //                                           ProcurementTypeId = x.ProcurementTypeId,
            //                                           projectTypeName = x.ProcurementType,
            //                                           BudgetAmount = x.BudgetAmount,
            //                                           CreationDate = x.CreationDate,
            //                                           ApprovedAmount = x.AprovedAmount,
            //                                           ApprovalDate = x.ApprovalDate,
            //                                           BudgetStatusName = x.BudgetStatusName,
            //                                       }).DefaultIfEmpty().OfType<BudgetMasterViewModel>().ToList();

            List<BudgetMasterViewModel> list = (from x in _pmiCommonService.PMIUnit.NewBudgetInfoRepository.Get(q => q.ZoneId == loggedUserZoneId && q.ProjectForId == ProjectForId).DefaultIfEmpty().ToList()
                                                where x.BudgetType == budgetType
                                                select new BudgetMasterViewModel
                                                {
                                                    Id = x.Id,
                                                    FinancialYearName = x.yearName,
                                                    BudgetStatusName = x.Status,
                                                }).DefaultIfEmpty().OfType<BudgetMasterViewModel>().ToList();

            if (list != null && list.Count > 0)
            {
                list = list.OrderByDescending(q => q.Id)
                                                .ThenByDescending(q => q.BudgetDetailId)
                                                .DefaultIfEmpty().OfType<BudgetMasterViewModel>()
                                                .ToList();
            }

            #region Sorting

            //if (request.SortingName == "BudgetSubHeadName")
            //{
            //    if (request.SortingOrder.ToString().ToLower() == "asc")
            //    {
            //        list = list.OrderBy(x => x.BudgetHeadName).ToList();
            //    }
            //    else
            //    {
            //        list = list.OrderByDescending(x => x.BudgetHeadName).ToList();
            //    }
            //}

            #endregion

            #region Search

            if (request.Searching)
            {
                string financialYearName = model.FinancialYearName.Trim();
                switch (financialYearName)
                {
                    case "0":
                        list = list.ToList();
                        break;
                    default:
                        list = list.Where(x => x.FinancialYearName.Trim().ToLower().Contains(model.FinancialYearName.Trim().ToLower())).ToList();
                        break;
                }

                string budgetStatus = model.BudgetStatusName.Trim();
                switch (budgetStatus)
                {
                    case "0":
                        list = list.ToList();
                        break;
                    default:
                        list = list.Where(x => x.BudgetStatusName.Trim().ToLower().Contains(model.BudgetStatusName.Trim().ToLower())).ToList();
                        break;
                }

                if (model.Id != 0)
                {
                    list = list.Where(q => q.Id == model.Id).DefaultIfEmpty().ToList();
                }
            }

            #endregion

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();


            foreach (var d in list)
            {

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.FinancialYearName,
                    d.BudgetStatusName,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        #endregion

        #region Populate Dropdown

        private void PopulateDropdown(BudgetMasterViewModel model)
        {
            var ministryList = _pmiCommonService.PMIUnit.MinistryRepository.GetAll().DefaultIfEmpty().OfType<PMI_Ministry>().ToList();
            model.DivisionOrMinistryList = Common.PopulateDllList(ministryList);

            var sourceOfFundList = _pmiCommonService.PMIUnit.SourceOfFundRepository.GetAll().DefaultIfEmpty().OfType<PMI_SourceOfFund>().ToList();
            model.SourceOfFundList = Common.PopulateDllList(sourceOfFundList);

            var approvalAuthorityList = _pmiCommonService.PMIUnit.ApprovalAuthorityRepository.GetAll().DefaultIfEmpty().OfType<PMI_ApprovalAuthority>().ToList();
            model.ApprovalAuthorityList = Common.PopulateDllList(approvalAuthorityList);

            var procurementTypeList = _pmiCommonService.PMIUnit.ProcurementTypeRepository.GetAll().DefaultIfEmpty().OfType<PMI_ProcurementType>().ToList();
            model.ProcurementTypeList = Common.PopulateDllList(procurementTypeList);

            var projectStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll().DefaultIfEmpty().OfType<PMI_ProjectStatus>().ToList();
            model.ProjectStatusList = Common.PopulateDllList(projectStatusList);

            var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().ToList();
            model.DesignationList = Common.PopulateDllList(designationList);

            var approvalStatusList = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll().Where(x=>x.Name.Contains("Submit") || x.Name.Contains("Draft")).ToList();
            model.ApprovalStatusList = Common.PopulateDllList(approvalStatusList);
        }

        public ActionResult GetFinancialYear()
        {
            var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll().DefaultIfEmpty().OfType<acc_Accounting_Period_Information>().ToList();
            financialYearList = financialYearList.OrderByDescending(q => q.yearName).ToList();
            Dictionary<string, string> dicFinancialYear = new Dictionary<string, string>();
            foreach (var item in financialYearList)
            {
                dicFinancialYear.Add(item.yearName, item.yearName);
            }
            return PartialView("_Select", dicFinancialYear);
        }

        public ActionResult GetNameOfWorks()
        {
            var listItem = _pmiCommonService.PMIUnit.BudgetDetailsRepository.GetAll().Distinct().Select(t => t.NameOfWorks).ToList();
            Dictionary<string, string> dicNameOfWorks = new Dictionary<string, string>();
            listItem = listItem.Distinct().ToList();
            foreach (var item in listItem)
            {
                dicNameOfWorks.Add(item, item);
            }

            return PartialView("_Select", dicNameOfWorks);
        }

        public ActionResult GetProcurementType()
        {
            var listItem = _pmiCommonService.PMIUnit.ProcurementTypeRepository.GetAll();
            Dictionary<string, string> dicNameOfWorks = new Dictionary<string, string>();

            foreach (var item in listItem)
            {
                dicNameOfWorks.Add(item.Name, item.Name);
            }

            return PartialView("_Select", dicNameOfWorks);
        }

        public ActionResult GetBudgetStatus()
        {
            var listItem = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll();
            Dictionary<string, string> dicNameOfWorks = new Dictionary<string, string>();

            foreach (var item in listItem)
            {
                dicNameOfWorks.Add(item.Name, item.Name);
            }

            return PartialView("_Select", dicNameOfWorks);
        }

        #endregion

        #region Others

        [HttpPost]
        public ActionResult GetCurrentFinancialYear()
        {
            var currentFy = _pmiCommonService.PMIUnit.AccountingPeriodRepository.Get(t => t.isActive == true).FirstOrDefault();
            return Json(currentFy.yearName, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Approval Flow

        public void SaveApprovalFlow(int BudgetProjectAPPId, int? approverStatusId, List<ApprovalFlowViewModel> approverList)
        {
            PMI_ApprovalFlowMaster ApprovalFlowMaster = new PMI_ApprovalFlowMaster();
            var approverStatus = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetByID(approverStatusId);
            var tempPeriod = _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Get(x => x.BudgetProjectAPPId == BudgetProjectAPPId && x.PMI_ApprovalSection.Enum.Contains("BGT")).FirstOrDefault();
                if (tempPeriod != null && approverList.Count> 0)
                {
                    List<Type> allTypes = new List<Type> { typeof(PMI_ApprovalFlowDetails) };
                    _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Delete(tempPeriod.Id,allTypes);
                   _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.SaveChanges();
                }

          if(approverList.Count > 0)
            {
            foreach (var item in approverList)
            {
                    if (item.EmployeeId != null && item.EmployeeId != 0)
                    {
                        PMI_ApprovalFlowDetails ApprovalFlowDetails = new PMI_ApprovalFlowDetails();

                        ApprovalFlowDetails.ApprovalFlowMasterId = ApprovalFlowDetails.Id;
                        ApprovalFlowDetails.EmployeeId = (int)item.EmployeeId;
                        ApprovalFlowDetails.StatusId = Convert.ToInt32(approverStatusId);
                        ApprovalFlowDetails.Remarks = string.Empty;
                        ApprovalFlowDetails.IUser = HttpContext.User.Identity.Name;
                        ApprovalFlowDetails.IDate = DateTime.Now;
                        _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.Add(ApprovalFlowDetails);
                    }
            }

            ApprovalFlowMaster.BudgetProjectAPPId = BudgetProjectAPPId;
            ApprovalFlowMaster.ApprovalSectionId = _pmiCommonService.PMIUnit.ApprovalSectionRepository.GetAll().Where(x => x.Enum == "BGT").Select(s=>s.Id).FirstOrDefault();
            ApprovalFlowMaster.CreateDate = DateTime.Now;
            ApprovalFlowMaster.IUser = HttpContext.User.Identity.Name;
            ApprovalFlowMaster.IDate = DateTime.Now;

            _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Add(ApprovalFlowMaster);
            _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.SaveChanges();
            }
        }

        #endregion

        public ActionResult Create(string type)
        {
            var model = new BudgetMasterViewModel();
            model.ActionType = "SaveBudget";
            PopulateDropdown(model);
            GenerateBudgetFirstRow(model, null, 0, 0, string.Empty);

            model.ApprovalDate = DateTime.Now;
            model.CreationDate = DateTime.Now;
            model.LastUpdateDate = DateTime.Now;
            model.BudgetType = type;
            model.ZoneInfoId = LoggedUserZoneInfoId;
            model.ProjectForId = Convert.ToInt32(Session["ProjectSectionId"]);
            model.IsConfirm = false;

            var approvalStatusId = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll().Where(x => x.Name.Contains("Draft")).Select(s=>s.Id).FirstOrDefault();
            model.ApprovalStatusId = approvalStatusId;

            var yearWiseBudgetStatusList = GetYearWiseBudgetStatusList();
            model.YearWiseBudgetStatusList.Add(yearWiseBudgetStatusList);

            var yearWiseBilledList = GetYearWiseBilledList();
            model.YearWiseBilledList.Add(yearWiseBilledList);



            var apvModel = new ApprovalFlowViewModel();
            var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x=>x.Name).ToList();
            apvModel.DesignationList = Common.PopulateDllList(designationList);

            var empList = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x=>x.EmpID == HttpContext.User.Identity.Name)
                         .Select(x => new { Id = x.Id, Name = string.Concat(x.FullName, " [", x.EmpID, "]", " [", x.PRM_Division == null ? string.Empty : x.PRM_Division.Name, "]") }).ToList();
            apvModel.EmployeeList = Common.PopulateDllList(empList);


            model.ApproverList.Add(apvModel); 

            return View("CreateOrEdit", model);
        }

        private YearWiseBudgetStatusViewModel GetYearWiseBudgetStatusList()
        {
            var model = new YearWiseBudgetStatusViewModel();
            var projectStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll().DefaultIfEmpty().OfType<PMI_ProjectStatus>().ToList();
            model.BudgetStatusList = Common.PopulateDllList(projectStatusList);

            var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();
            model.FinancialYearList = Common.PopulateAccountingPeriodDdl(financialYearList);

            return model;
        }

        private YearWiseBilledViewModel GetYearWiseBilledList()
        {
            var model = new YearWiseBilledViewModel();
            var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();
            model.FinancialYearList = Common.PopulateAccountingPeriodDdl(financialYearList);
            return model;
        }

        private BudgetMasterViewModel GenerateBudgetFirstRow(BudgetMasterViewModel model, int? budgetHeadId, int? noOfFinancialYear, int? noOfFinancialYearBilled, string id)
        {
            BudgetDetailViewModel anDetailItem = new BudgetDetailViewModel();
            var budgetHeads = _pmiCommonService.PMIUnit.BudgetHeadViewRepository.GetAll().DefaultIfEmpty().OfType<vwPMIBudgetHead>().ToList();
            var budgetHeadList = budgetHeads.Where(q => q.ParentId == null).DefaultIfEmpty().OfType<vwPMIBudgetHead>().ToList();
            if (budgetHeadList.Count > 0)
            {
                anDetailItem.BudgetHeadList = Common.PopulateBudgetHeadDDL(budgetHeadList);
            }


            if (budgetHeadId != null)
            {
                anDetailItem.BudgetHeadId = Convert.ToInt32(budgetHeadId);
                var subHeadList = budgetHeads.Where(t => t.ParentId == budgetHeadId).OfType<vwPMIBudgetHead>().ToList();
                anDetailItem.BudgetSubHeadList = Common.PopulateBudgetSubDllList(subHeadList);
            }
            else
            {
                anDetailItem.BudgetSubHeadList = new List<SelectListItem>();
            }

            var constructionTypeList = _pmiCommonService.PMIUnit.ConstructionTypeRepository.GetAll().ToList();
            anDetailItem.ConstructionTypeList = Common.PopulateDllList(constructionTypeList);

            var workStatusList = _pmiCommonService.PMIUnit.WorkStatusRepository.GetAll().DefaultIfEmpty().OfType<PMI_WorkStatus>().ToList();
            anDetailItem.WorkStatusList = Common.PopulateDllList(workStatusList);

            #region Sub Ledger
            var subLedgerList = _pmiCommonService.PMIUnit.HeadRepository.GetAll().Where(x=>x.IsSubledger == true).ToList();
            var Slist = new List<SelectListItem>();
            foreach (var item in subLedgerList)
            {
                Slist.Add(new SelectListItem()
                {
                    Text = item.HeadName,
                    Value = item.Id.ToString()
                });
            }
            anDetailItem.SubLedgerList = Slist;
            #endregion

            #region name Of Work List
            var nameOfWorkList = _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.GetAll().ToList();

            var list = new List<SelectListItem>();
            foreach (var item in nameOfWorkList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.DescritionOfAPP,
                    Value = item.DescritionOfAPP
                });
            }
            anDetailItem.NameOfWorksList = list;
            #endregion


            GenerateYearlyCost(anDetailItem, noOfFinancialYear, id);
            GenerateYearlyBilled(anDetailItem, noOfFinancialYearBilled, id);

            model.BudgetDetailList.Add(anDetailItem);

            var detailHead = new BudgetDetailsHeadViewModel();
            var headList = budgetHeadList.Where(q => q.ParentId == null).DefaultIfEmpty().OfType<vwPMIBudgetHead>().ToList();
            detailHead.BudgetHeadList = Common.PopulateBudgetHeadDDL(headList);
            model.BudgetDetailHeadList.Add(detailHead);

            return model;
        }

        private BudgetDetailViewModel GenerateYearlyCost(BudgetDetailViewModel model, int? noOfFinancialYear, string id)
        {
            int noOfFy = 0;
            int.TryParse(noOfFinancialYear.ToString(), out noOfFy);
            if (noOfFy == 0) { noOfFy = 1; }
            var yearlyCostList = new List<BudgetDetailYearlyCostViewModel>();
            for (int i = 0; i < noOfFy; i++)
            {
                var anYearlyCost = new BudgetDetailYearlyCostViewModel();
                anYearlyCost.PreviousFieldId = id;
                yearlyCostList.Add(anYearlyCost);
            }
            model.YearlyCostList.AddRange(yearlyCostList);
            return model;
        }

        private BudgetDetailViewModel GenerateYearlyBilled(BudgetDetailViewModel model, int? noOfFinancialYear, string id)
        {
            int noOfFy = 0;
            int.TryParse(noOfFinancialYear.ToString(), out noOfFy);
            if (noOfFy == 0) { noOfFy = 1; }
            var yearlyBilledList = new List<BudgetDetailYearlyBilledViewModel>();
            for (int i = 0; i < noOfFy; i++)
            {
                var anYearlyBilled = new BudgetDetailYearlyBilledViewModel();
                anYearlyBilled.PreviousFieldId = id;
                yearlyBilledList.Add(anYearlyBilled);
            }
            model.YearlyBilledList.AddRange(yearlyBilledList);
            return model;
        }


        [HttpPost]
        public ActionResult SaveBudget(BudgetMasterViewModel model)
        {
            try
            {
                    var masterObj = model.ToEntity();
                    masterObj.IUser = HttpContext.User.Identity.Name;
                    masterObj.IDate = DateTime.Now;

                    var budgetAllHeadList = _pmiCommonService.PMIUnit.BudgetHeadViewRepository.GetAll();

                    foreach (var item in model.BudgetDetailList)
                    {
                        var detailObj = item.ToEntity();
                        decimal estimatedBudget = 0;
                        decimal.TryParse(item.BudgetAmount.ToString(), out estimatedBudget);
                        detailObj.BudgetAmount = estimatedBudget;

                        int budgetHeadId = 0;
                        var headInfo = budgetAllHeadList.Where(q => q.Id == item.BudgetSubHeadId).FirstOrDefault();
                        if (headInfo != null)
                        {
                            int.TryParse(headInfo.ParentId.ToString(), out budgetHeadId);
                        }
                        if (budgetHeadId != 0)
                        {
                            detailObj.BudgetHeadId = budgetHeadId;
                        }
                        //if (item.BudgetSubHeadId == 0)
                        //    detailObj.BudgetSubHeadId = null;

                        foreach (var cost in item.YearlyCostList)
                        {
                            var costObj = cost.ToEntity();

                            decimal estimatedCost = 0;
                            decimal.TryParse(cost.EstematedCost.ToString(), out estimatedCost);
                            costObj.EstematedCost = estimatedCost;

                            costObj.BudgetDetailsId = detailObj.Id;
                            costObj.PMI_BudgetDetails = detailObj;
                            _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.Add(costObj);
                        }

                        foreach (var billed in item.YearlyBilledList)
                        {
                            var billedObj = billed.ToEntity();

                            decimal BilledAmount = 0;
                            decimal.TryParse(billed.BilledAmount.ToString(), out BilledAmount);
                            billedObj.BilledAmount = BilledAmount;

                            billedObj.BudgetDetailsId = detailObj.Id;
                            billedObj.PMI_BudgetDetails = detailObj;
                            _pmiCommonService.PMIUnit.BudgetDetailsYearlyBilledRepository.Add(billedObj);
                        }

                    detailObj.PMI_BudgetMaster = masterObj;
                        _pmiCommonService.PMIUnit.BudgetDetailsRepository.Add(detailObj);
                    }

                    _pmiCommonService.PMIUnit.BudgetMasterRepository.Add(masterObj);
                    _pmiCommonService.PMIUnit.BudgetMasterRepository.SaveChanges();

                    SaveApprovalFlow(masterObj.Id, model.ApprovalStatusId, model.ApproverList);

                    model = masterObj.ToModel();
                   
            }
            catch (Exception ex)
            {
                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }

            //if (model.BudgetType.Equals("Development"))
            //{
            //    model.ActionType = "Index";
            //    return View("Index", model);
            //}
            //else
            //{
            //model.ActionType = "NonDevlopmentBudget";
            //return View("NonDevlopmentBudget", model);
            return RedirectToAction("Edit", new { id = model.Id, type = "success" });
            //}


        }

        public ActionResult Edit(int id)
        {
            var model = new BudgetMasterViewModel();
            try
            {
                var budgetStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll();
                var approveStatusId = budgetStatusList.Where(q => q.Name.Contains("Appr")).FirstOrDefault().Id;

                var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();

                var master = _pmiCommonService.PMIUnit.BudgetMasterRepository.GetByID(id);
                model = master.ToModel();

                var details = _pmiCommonService.PMIUnit.BudgetDetailsRepository.Get(q => q.BudgetMasterId == id)
                             .OrderBy(s => s.HiddenSlNo == null?string.Empty : s.HiddenSlNo.Length.ToString()).ThenBy(s=>s.HiddenSlNo == null ? string.Empty : s.HiddenSlNo)
                             .DefaultIfEmpty().OfType<PMI_BudgetDetails>().ToList();

                var budgetHeadList = _pmiCommonService.PMIUnit.BudgetHeadViewRepository.GetAll().ToList();
                var constructionTypeList = _pmiCommonService.PMIUnit.ConstructionTypeRepository.GetAll().ToList();
                var workStatusList = _pmiCommonService.PMIUnit.WorkStatusRepository.GetAll().ToList();


                var yearWiseBudgetStatusList = new List<YearWiseBudgetStatusViewModel>();
                var yearWiseBilledList = new List<YearWiseBilledViewModel>();

                var distinctHeads = details.DistinctBy(q => q.BudgetHeadId).DefaultIfEmpty();
                foreach (var item in distinctHeads)
                {
                    var detailHead = new BudgetDetailsHeadViewModel();
                    var headList = budgetHeadList.Where(q => q.ParentId == null).DefaultIfEmpty().ToList();
                    detailHead.BudgetHeadList = Common.PopulateBudgetHeadDDL(headList);
                    detailHead.BudgetHeadId = item.BudgetHeadId;
                    model.BudgetDetailHeadList.Add(detailHead);
                }

                if (details != null && details.Count > 0)
                {
                    foreach (var item in details)
                    {

                        var detailModel = item.ToModel();
                        detailModel.ConstructionTypeList = Common.PopulateDllList(constructionTypeList);
                        var subHeadList = budgetHeadList.Where(q => q.ParentId == item.BudgetHeadId).ToList();
                        detailModel.BudgetSubHeadList = Common.PopulateBudgetSubDllList(subHeadList);
                        var budgetHeads = budgetHeadList.Where(q => q.ParentId == null).DefaultIfEmpty().ToList();
                        detailModel.BudgetHeadList = Common.PopulateBudgetHeadDDL(budgetHeads);
                        detailModel.WorkStatusList = Common.PopulateDllList(workStatusList);
                        #region Sub Ledger
                        var subLedgerList = _pmiCommonService.PMIUnit.HeadRepository.GetAll().Where(x => x.IsSubledger == true).ToList();
                        var Slist = new List<SelectListItem>();
                        foreach (var sitem in subLedgerList)
                        {
                            Slist.Add(new SelectListItem()
                            {
                                Text = sitem.HeadName,
                                Value = sitem.Id.ToString()
                            });
                        }
                        detailModel.SubLedgerList = Slist;
                        #endregion
                    
                        var costList = _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.Get(q => q.BudgetDetailsId == item.Id).DefaultIfEmpty().OfType<PMI_BudgetDetailsYearlyCost>().ToList();
                        if (costList != null && costList.Count > 0)
                        {
                            foreach (var cost in costList)
                            {
                                var costModel = cost.ToModel();
                                costModel.BudgetSubHeadId = item.BudgetSubHeadId;
                                costModel.ActionType = "Update";
                                if (costModel.BudgetStatusId == approveStatusId)
                                {
                                    costModel.StatusName = "Approved";
                                }
                                detailModel.YearlyCostList.Add(costModel);

                                var addedList = yearWiseBudgetStatusList.Where(q => q.BudgetStatusId == cost.BudgetStatusId && q.FinancialYearId == cost.FinancialYearId).DefaultIfEmpty().OfType<YearWiseBudgetStatusViewModel>().ToList();
                                if (addedList == null || addedList.Count == 0)
                                {
                                    var statusModel = new YearWiseBudgetStatusViewModel();
                                    statusModel.FinancialYearId = cost.FinancialYearId;
                                    statusModel.BudgetStatusId = cost.BudgetStatusId;
                                    if (cost.BudgetStatusId == approveStatusId)
                                    {
                                        var approvedFyList = financialYearList.Where(q => q.id == cost.FinancialYearId).DefaultIfEmpty().ToList();
                                        var approvedSatusList = budgetStatusList.Where(q => q.Id == cost.BudgetStatusId).DefaultIfEmpty().ToList();
                                        statusModel.BudgetStatusList = Common.PopulateDllList(approvedSatusList);
                                        statusModel.FinancialYearList = Common.PopulateAccountingPeriodDdl(approvedFyList);
                                    }
                                    else
                                    {
                                        statusModel.BudgetStatusList = Common.PopulateDllList(budgetStatusList);
                                        statusModel.FinancialYearList = Common.PopulateAccountingPeriodDdl(financialYearList);
                                    }
                                    yearWiseBudgetStatusList.Add(statusModel);
                                }

                            }
                        }

                        #region Bill List
                        //var billedList = _pmiCommonService.PMIUnit.BudgetDetailsYearlyBilledRepository.Get(q => q.BudgetDetailsId == item.Id).DefaultIfEmpty().OfType<PMI_BudgetDetailsYearlyBilled>().ToList();

                        //if (billedList != null && billedList.Count > 0)
                        //{
                        //    foreach (var billed in billedList)
                        //    {
                        //        var billedModel = billed.ToModel();
                        //        billedModel.BudgetSubHeadId = item.BudgetSubHeadId;
                        //        billedModel.ActionType = "Update";
                        //        detailModel.YearlyBilledList.Add(billedModel);

                        //        var addedList = yearWiseBilledList.Where(q => q.FinancialYearId == billed.FinancialYearId).DefaultIfEmpty().OfType<YearWiseBilledViewModel>().ToList();
                        //        if (addedList == null || addedList.Count == 0)
                        //        {
                        //            var statusModel = new YearWiseBilledViewModel();
                        //            statusModel.FinancialYearId = billed.FinancialYearId;
                        //            statusModel.FinancialYearList = Common.PopulateAccountingPeriodDdl(financialYearList);
                        //            yearWiseBilledList.Add(statusModel);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    GenerateYearlyBilled(detailModel, 0, string.Empty);
                        //}
                        #endregion

                        #region name Of Work List
                        var nameOfWorkList = _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.GetAll().ToList();

                        var list = new List<SelectListItem>();
                        foreach (var item2 in nameOfWorkList)
                        {
                            list.Add(new SelectListItem()
                            {
                                Text = item2.DescritionOfAPP,
                                Value = item2.DescritionOfAPP
                            });
                        }
                        detailModel.NameOfWorksList = list;
                        #endregion

                        model.BudgetDetailList.Add(detailModel);
                        model.YearWiseBudgetStatusList = yearWiseBudgetStatusList;
                        model.YearWiseBilledList = yearWiseBilledList;
                    }

                    if(model.YearWiseBilledList.Count == 0)
                    {
                            var yearWiseBilledListOne = GetYearWiseBilledList();
                            model.YearWiseBilledList.Add(yearWiseBilledListOne);
                         
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            PopulateDropdown(model);

            #region Approval Flow
            var approverStatus = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetByID(model.ApprovalStatusId);
            var approverList = (from x in _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.GetAll()
                                join y in _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.GetAll() on x.Id equals y.ApprovalFlowMasterId
                                join appStatus in _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll() on y.StatusId equals appStatus.Id
                                join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on y.EmployeeId equals emp.Id
                                where(x.BudgetProjectAPPId == id)
                                select new ApprovalFlowViewModel
                                {
                                    Id = x.Id,
                                    EmployeeName = emp.FullName,
                                    EmpId = emp.EmpID,
                                    EmployeeId = emp.Id,
                                    DesignationId = emp.DesignationId,
                                    Remarks = y.Remarks,
                                    DesignationName = emp.PRM_Designation.Name,
                                    DepartmentName = emp.PRM_Division == null ? string.Empty : emp.PRM_Division.Name,
                                    Status = appStatus.Name.Contains("Submit") ? "Pending": appStatus.Name

                                }).ToList();
            if (approverStatus.Name.Contains("Draft"))
            {
                if (approverList.Count > 0)
                {
                    foreach (var item in approverList)
                    {
                        ApprovalFlowViewModel apModel = new ApprovalFlowViewModel();
                        apModel.Id = item.Id;
                        apModel.EmployeeId = item.EmployeeId;
                        apModel.DesignationId = item.DesignationId;
                        apModel.EmployeeList = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.Id == item.EmployeeId)
                            .Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = string.Concat(x.FullName, " [", x.EmpID, "]", " [", x.PRM_Division == null ? string.Empty : x.PRM_Division.Name, "]") }).ToList();

                        apModel.DesignationList = Common.PopulateDllList(_prmCommonService.PRMUnit.DesignationRepository.Get(x => x.Id == item.DesignationId).ToList());


                        model.ApproverList.Add(apModel);
                    }
                }
                else
                {
                    var apvModel = new ApprovalFlowViewModel();
                    var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();
                    apvModel.DesignationList = Common.PopulateDllList(designationList);
                    var empList = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == HttpContext.User.Identity.Name)
                                 .Select(x => new { Id = x.Id, Name = string.Concat(x.FullName, " [", x.EmpID, "]", " [", x.PRM_Division == null ? string.Empty : x.PRM_Division.Name, "]") }).ToList();
                    apvModel.EmployeeList = Common.PopulateDllList(empList);

                    model.ApproverList.Add(apvModel);
                }
            }
            else
            {
                model.ApproverList = approverList;
            }



            #endregion

            model.ActionType = "UpdateBudget";
            model.ApprovalStatus = approverStatus ==null ?string.Empty : approverStatus.Name;
            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult ShowBudgetPrintPopUp(int budgetDetailId, PrintBudgetViewModel model)
        {
            var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll().OrderByDescending(t => t.periodEndDate).ToList();

            model.FinancialYearList = Common.PopulateAccountingPeriodDdl(financialYearList);

            var budgetDetail = _pmiCommonService.PMIUnit.BudgetDetailsRepository.Get(t => t.BudgetMasterId == budgetDetailId).FirstOrDefault();
            if (budgetDetail != null)
            {
                var projectList = _pmiCommonService.PMIUnit.BudgetDetailsRepository.Get(t => t.BudgetMasterId == budgetDetail.BudgetMasterId).ToList();
                model.ProjectList = Common.PopulateDdlProjectList(projectList);
            }

            var budgetStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll();
            model.BudgetStatusList = Common.PopulateDllList(budgetStatusList);

            return PartialView("PrintBudget", model);
        }

        [HttpPost]
        public ActionResult UpdateBudget(BudgetMasterViewModel model)
        {
            try
            {
                    var masterObj = model.ToEntity();
                    var budgetAllHeadList = _pmiCommonService.PMIUnit.BudgetHeadViewRepository.GetAll();

                    var existingProjects = _pmiCommonService.PMIUnit.BudgetDetailsRepository.Get(q => q.BudgetMasterId == masterObj.Id).DefaultIfEmpty().OfType<PMI_BudgetDetails>().ToList();
                    var existingProjectInModel = model.BudgetDetailList.Where(q => q.Id > 0).DefaultIfEmpty().ToList();
                    var deletedProjects = (from existing in existingProjects
                                           where !(existingProjectInModel.Any(dt => dt.Id == existing.Id))
                                           select existing).DefaultIfEmpty().OfType<PMI_BudgetDetails>().ToList();
                    foreach (var item in deletedProjects)
                    {
                        _pmiCommonService.PMIUnit.BudgetDetailsRepository.Delete(item);
                        var costs = _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.Get(q => q.BudgetDetailsId == item.Id).DefaultIfEmpty().OfType<PMI_BudgetDetailsYearlyCost>().ToList();
                        foreach (var c in costs)
                        {
                            _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.Delete(c);
                        }
                    }

                    foreach (var item in model.BudgetDetailList)
                    {
                        var detailObj = item.ToEntity();
                        decimal estimatedBudget = 0;
                        decimal.TryParse(item.BudgetAmount.ToString(), out estimatedBudget);
                        detailObj.BudgetAmount = estimatedBudget;
                        detailObj.BudgetMasterId = masterObj.Id;
                        int budgetHeadId = 0;
                        var headInfo = budgetAllHeadList.Where(q => q.Id == item.BudgetSubHeadId).FirstOrDefault();
                        if (headInfo != null)
                        {
                            int.TryParse(headInfo.ParentId.ToString(), out budgetHeadId);
                        }
                        if (budgetHeadId != 0)
                        {
                            detailObj.BudgetHeadId = budgetHeadId;
                        }
                        //if (item.BudgetSubHeadId == 0)
                        //    detailObj.BudgetSubHeadId = null;

                    if (detailObj.Id > 0)
                        {
                            _pmiCommonService.PMIUnit.BudgetDetailsRepository.Update(detailObj);
                        }
                        else
                        {
                            _pmiCommonService.PMIUnit.BudgetDetailsRepository.Add(detailObj);
                            _pmiCommonService.PMIUnit.BudgetDetailsRepository.SaveChanges();
                        }

                        // Yearly Cost Amount Update
                        var existingCost = _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.Get(q => q.BudgetDetailsId == detailObj.Id).DefaultIfEmpty().OfType<PMI_BudgetDetailsYearlyCost>().ToList();
                        var existingCostModel = item.YearlyCostList.Where(q => q.Id > 0).DefaultIfEmpty().OfType<BudgetDetailYearlyCostViewModel>().ToList();
                        var deletedCost = (from existing in existingCost
                                           where !(existingCostModel.Any(dt => dt.Id == existing.Id))
                                           select existing).DefaultIfEmpty().OfType<PMI_BudgetDetailsYearlyCost>().ToList();

                        foreach (var deletedItem in deletedCost)
                        {
                            _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.Delete(deletedItem);
                        }

                        foreach (var cost in item.YearlyCostList)
                        {
                            var costObj = cost.ToEntity();

                            decimal estimatedCost = 0;
                            decimal.TryParse(cost.EstematedCost.ToString(), out estimatedCost);
                            costObj.EstematedCost = estimatedCost;

                            if (costObj.Id > 0)
                            {
                                _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.Update(costObj);
                            }
                            else
                            {
                                costObj.BudgetDetailsId = detailObj.Id;
                                _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.Add(costObj);
                            }
                        }

                    // Yearly Billed Amount Update
                    var existingBilled = _pmiCommonService.PMIUnit.BudgetDetailsYearlyBilledRepository.Get(q => q.BudgetDetailsId == detailObj.Id).DefaultIfEmpty().OfType<PMI_BudgetDetailsYearlyBilled>().ToList();
                    var existingBilledModel = item.YearlyBilledList.Where(q => q.Id > 0).DefaultIfEmpty().OfType<BudgetDetailYearlyBilledViewModel>().ToList();
                    var deletedBilled = (from existing in existingBilled
                                         where !(existingBilledModel.Any(dt => dt.Id == existing.Id))
                                         select existing).DefaultIfEmpty().OfType<PMI_BudgetDetailsYearlyBilled>().ToList();

                    foreach (var deletedItem in deletedBilled)
                    {
                        _pmiCommonService.PMIUnit.BudgetDetailsYearlyBilledRepository.Delete(deletedItem);
                    }

                    foreach (var billed in item.YearlyBilledList)
                    {
                        var billedObj = billed.ToEntity();

                        decimal billedAmount = 0;
                        decimal.TryParse(billed.BilledAmount.ToString(), out billedAmount);
                        billedObj.BilledAmount = billedAmount;

                        if (billedObj.Id > 0)
                        {
                            _pmiCommonService.PMIUnit.BudgetDetailsYearlyBilledRepository.Update(billedObj);
                        }
                        else
                        {
                            billedObj.BudgetDetailsId = detailObj.Id;
                            _pmiCommonService.PMIUnit.BudgetDetailsYearlyBilledRepository.Add(billedObj);
                        }
                    }
                }
                    _pmiCommonService.PMIUnit.BudgetMasterRepository.Update(masterObj);
                    _pmiCommonService.PMIUnit.BudgetMasterRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.BudgetDetailsYearlyBilledRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.BudgetDetailsRepository.SaveChanges();

                    _pmiCommonService.PMIUnit.BudgetZoneRepository.SaveChanges();

                    SaveApprovalFlow(masterObj.Id, model.ApprovalStatusId, model.ApproverList);

            }
            catch (Exception ex)
            {
                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            return RedirectToAction("Edit", new { id = model.Id, type = "success" });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                var isConfirm = _pmiCommonService.PMIUnit.BudgetMasterRepository.GetByID(id).IsConfirm;
                if (isConfirm)
                {
                    result = false;
                    errMsg = "Sorry! Budget is confirmed.";
                }
                else
                {
                    var budgetDetail = _pmiCommonService.PMIUnit.BudgetDetailsRepository.Get(x => x.BudgetMasterId == id).ToList();
                    foreach (var dtl in budgetDetail)
                    {
                        var costList = _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.Get(t => t.BudgetDetailsId == dtl.Id).ToList();
                        foreach (var item in costList)
                        {
                            _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.Delete(item.Id);
                        }

                        var billedList = _pmiCommonService.PMIUnit.BudgetDetailsYearlyBilledRepository.Get(t => t.BudgetDetailsId == dtl.Id).ToList();
                        foreach (var item in billedList)
                        {
                            _pmiCommonService.PMIUnit.BudgetDetailsYearlyBilledRepository.Delete(item.Id);
                        }

                        _pmiCommonService.PMIUnit.BudgetDetailsRepository.Delete(dtl.Id);
                    }
                    var budgetZoneList = _pmiCommonService.PMIUnit.BudgetZoneRepository.Get(t => t.BudgetMasterId == id);
                    foreach (var item in budgetZoneList)
                    {
                        _pmiCommonService.PMIUnit.BudgetZoneRepository.Delete(item.Id);

                    }
                    _pmiCommonService.PMIUnit.BudgetMasterRepository.Delete(id);

                    _pmiCommonService.PMIUnit.BudgetZoneRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.BudgetDetailsYearlyBilledRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.BudgetDetailsRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.BudgetMasterRepository.SaveChanges();

                    result = true;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                }
            }
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        [HttpPost]
        public ActionResult AddNewFinancialYearCost(BudgetMasterViewModel model, int? noOfFinancialYear, string id)
        {
            //GenerateBudgetFirstRow(model, 0, noOfFinancialYear, 1, id);

            var detailModel = new BudgetDetailViewModel();
            GenerateYearlyCost(detailModel, noOfFinancialYear,id);
            //var yearlyCostList = model.BudgetDetailList.SelectMany(q => q.YearlyCostList).ToList();
            var yearlyCostList = detailModel.YearlyCostList.ToList();
            return PartialView("_BudgetDetailYearlyCost", yearlyCostList);
        }

        [HttpPost]
        public ActionResult AddNewFinancialYearBilled(BudgetMasterViewModel model, int? noOfFinancialYear, string id)
        {
            GenerateBudgetFirstRow(model, 0, noOfFinancialYear, 1, id);
            var yearlyZBilledList = model.BudgetDetailList.SelectMany(q => q.YearlyBilledList).ToList();
            return PartialView("_BudgetDetailYearlyBilled", yearlyZBilledList);
        }

        [HttpPost]
        public ActionResult AddNewProject(BudgetMasterViewModel model, int? budgetHeadId, int? noOfFinancialYear, int? noOfFinancialYearBilled,  string id)
        {
            GenerateBudgetFirstRow(model, budgetHeadId, noOfFinancialYear, noOfFinancialYearBilled, id);
            return PartialView("_BudgetDetail", model.BudgetDetailList);
        }

        [HttpPost]
        public ActionResult AddEmployeeList(ApprovalFlowViewModel model)
        {
            var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();
            model.DesignationList = Common.PopulateDllList(designationList);
            var empList = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == HttpContext.User.Identity.Name)
                         .Select(x => new { Id = x.Id, Name = string.Concat(x.FullName, " [", x.EmpID, "]", " [", x.PRM_Division == null ? string.Empty : x.PRM_Division.Name, "]") }).ToList();
            model.EmployeeList = Common.PopulateDllList(empList);
            return PartialView("_PartialEmployeeList", model);
        }

        public JsonResult GetEmployeeList(int designationId)
        {
            var result = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.DesignationId == designationId)
                         .Select(x => new { Id = x.Id, Name = string.Concat(x.FullName," [",x.EmpID,"]"," [", x.PRM_Division == null ? string.Empty : x.PRM_Division.Name ,"]")});
                         
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetBudgetSubHeadByHeadId(int budgetHeadId, int budgetDetailsId)
        {
            try
            {
                int budgetDetailId = 0;
                int.TryParse(budgetDetailsId.ToString(), out budgetDetailId);
                var budgetHeadList = _pmiCommonService.PMIUnit.BudgetHeadViewRepository.Get(t => t.ParentId == budgetHeadId).DefaultIfEmpty().OfType<vwPMIBudgetHead>().ToList();

                var list = new List<KeyValuePair<int, string>>();
                foreach (var item in budgetHeadList)
                {
                    list.Add(new KeyValuePair<int, string>(item.Id, item.BudgetSubHead));
                }

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddNewBudgetHead(BudgetMasterViewModel model, int? noOfFinancialYear, int? noOfFinancialYearBilled)
        {
            GenerateBudgetFirstRow(model, null, noOfFinancialYear, noOfFinancialYearBilled, string.Empty);
            return PartialView("_BudgetDetailHeads", model);
        }

        [HttpPost]
        public ActionResult GetFinancialYearList(FinancialYearViewModel model, int[] selectedYears)
        {
            var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll().DefaultIfEmpty().OfType<acc_Accounting_Period_Information>().ToList();
            var list = (from x in financialYearList
                        select new FinancialYearViewModel
                        {
                            Id = x.id,
                            FinancialYearName = x.yearName,
                            FinancialYearStartDate = x.periodStartDate,
                            FinancialYearEndDate = x.periodEndDate,
                            IsSelected = selectedYears.Contains(x.id) ? true : false
                        }).DefaultIfEmpty().OrderByDescending(t => t.FinancialYearEndDate).ToList();
            model.FinancialYearList = list;
            return PartialView("_FinancialYearList", model);
        }

        [HttpPost]
        public ActionResult AddNewFinancialYearHead(BudgetMasterViewModel model)
        {
            var headStatus = GetYearWiseBudgetStatusList();
            model.YearWiseBudgetStatusList.Add(headStatus);
            return PartialView("_PartialHead", model.YearWiseBudgetStatusList);
        }
        [HttpPost]
        public ActionResult AddNewFinancialYearHeadBilled(BudgetMasterViewModel model)
        {
            var headStatus = GetYearWiseBilledList();
            model.YearWiseBilledList.Add(headStatus);
            return PartialView("_PartialHeadBilled", model.YearWiseBilledList);
        }

        public JsonResult IsSubheadAlreadyAdded(int budgetDetailsId, int budgetSubHeadId)
        {
            int isAdded = 0;
            int? previousSubHeadId = 0;
            int budgetMasterId = 0;
            var detailObj = _pmiCommonService.PMIUnit.BudgetDetailsRepository.Get(q => q.Id == budgetDetailsId).FirstOrDefault();

            if (detailObj != null)
            {
                budgetMasterId = detailObj.BudgetMasterId;
            }
            var detailsList = _pmiCommonService.PMIUnit.BudgetDetailsRepository.Get(q => q.BudgetMasterId == budgetMasterId).DefaultIfEmpty().ToList();
            var item = detailsList.Where(q => q.BudgetMasterId == budgetMasterId && q.BudgetSubHeadId == budgetSubHeadId && q.Id != budgetDetailsId).DefaultIfEmpty().OfType<PMI_BudgetDetails>().ToList();
            if (item != null && item.Count > 0)
            {
                isAdded = 1;
                previousSubHeadId = detailObj.BudgetSubHeadId;
            }
            else
            {
                isAdded = 0;
                previousSubHeadId = budgetSubHeadId;
            }
            var result = new { IsAdded = isAdded, PreviousSubHeadId = previousSubHeadId };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Save as
        public ActionResult Saveas(int id)
        {
            var model = new BudgetMasterViewModel();
            try
            {
                var budgetStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll();
                var proposedStatusId = budgetStatusList.Where(q => q.Name.Contains("Prop")).FirstOrDefault().Id;

                var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();

                var master = _pmiCommonService.PMIUnit.BudgetMasterRepository.GetByID(id);
                model = master.ToModel();
                model.Id = 0;
                var details = _pmiCommonService.PMIUnit.BudgetDetailsRepository.Get(q => q.BudgetMasterId == id)
                             .OrderBy(s => s.HiddenSlNo == null ? string.Empty : s.HiddenSlNo.Length.ToString()).ThenBy(s => s.HiddenSlNo == null ? string.Empty : s.HiddenSlNo)
                             .DefaultIfEmpty().OfType<PMI_BudgetDetails>().ToList();

                var budgetHeadList = _pmiCommonService.PMIUnit.BudgetHeadViewRepository.GetAll().ToList();
                var constructionTypeList = _pmiCommonService.PMIUnit.ConstructionTypeRepository.GetAll().ToList();
                var workStatusList = _pmiCommonService.PMIUnit.WorkStatusRepository.GetAll().ToList();


                var yearWiseBudgetStatusList = new List<YearWiseBudgetStatusViewModel>();
                var yearWiseBilledList = new List<YearWiseBilledViewModel>();

                var distinctHeads = details.DistinctBy(q => q.BudgetHeadId).DefaultIfEmpty();
                foreach (var item in distinctHeads)
                {
                    var detailHead = new BudgetDetailsHeadViewModel();
                    var headList = budgetHeadList.Where(q => q.ParentId == null).DefaultIfEmpty().ToList();
                    detailHead.BudgetHeadList = Common.PopulateBudgetHeadDDL(headList);
                    detailHead.BudgetHeadId = item.BudgetHeadId;
                    model.BudgetDetailHeadList.Add(detailHead);
                }

                if (details != null && details.Count > 0)
                {
                    foreach (var item in details)
                    {

                        var detailModel = item.ToModel();
                        detailModel.Id = 0;
                        detailModel.BudgetMasterId = 0;
                        detailModel.ConstructionTypeList = Common.PopulateDllList(constructionTypeList);
                        var subHeadList = budgetHeadList.Where(q => q.ParentId == item.BudgetHeadId).ToList();
                        detailModel.BudgetSubHeadList = Common.PopulateBudgetSubDllList(subHeadList);
                        var budgetHeads = budgetHeadList.Where(q => q.ParentId == null).DefaultIfEmpty().ToList();
                        detailModel.BudgetHeadList = Common.PopulateBudgetHeadDDL(budgetHeads);
                        detailModel.WorkStatusList = Common.PopulateDllList(workStatusList);
                        #region Sub Ledger
                        var subLedgerList = _pmiCommonService.PMIUnit.HeadRepository.GetAll().Where(x => x.IsSubledger == true).ToList();
                        var Slist = new List<SelectListItem>();
                        foreach (var sitem in subLedgerList)
                        {
                            Slist.Add(new SelectListItem()
                            {
                                Text = sitem.HeadName,
                                Value = sitem.Id.ToString()
                            });
                        }
                        detailModel.SubLedgerList = Slist;
                        #endregion

                        var costList = _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.Get(q => q.BudgetDetailsId == item.Id && q.BudgetStatusId == proposedStatusId).DefaultIfEmpty().OfType<PMI_BudgetDetailsYearlyCost>().ToList();
                        if (costList != null && costList.Count > 0)
                        {
                            foreach (var cost in costList)
                            {
                                var costModel = cost.ToModel();
                                costModel.BudgetDetailsId = 0;
                                costModel.Id = 0;
                                costModel.BudgetSubHeadId = item.BudgetSubHeadId;
                                detailModel.YearlyCostList.Add(costModel);

                                var addedList = yearWiseBudgetStatusList.Where(q => q.BudgetStatusId == cost.BudgetStatusId && q.FinancialYearId == cost.FinancialYearId).DefaultIfEmpty().OfType<YearWiseBudgetStatusViewModel>().ToList();
                                if (addedList == null || addedList.Count == 0)
                                {
                                    var statusModel = new YearWiseBudgetStatusViewModel();
                                    statusModel.FinancialYearId = cost.FinancialYearId;
                                    statusModel.BudgetStatusId = cost.BudgetStatusId;
                                    statusModel.BudgetStatusList = Common.PopulateDllList(budgetStatusList);
                                    statusModel.FinancialYearList = Common.PopulateAccountingPeriodDdl(financialYearList);
                                    yearWiseBudgetStatusList.Add(statusModel);
                                }

                            }
                        }

                        #region Bill
                        //var billedList = _pmiCommonService.PMIUnit.BudgetDetailsYearlyBilledRepository.Get(q => q.BudgetDetailsId == item.Id).DefaultIfEmpty().OfType<PMI_BudgetDetailsYearlyBilled>().ToList();

                        //if (billedList != null && billedList.Count > 0)
                        //{
                        //    foreach (var billed in billedList)
                        //    {
                        //        var billedModel = billed.ToModel();
                        //        billedModel.BudgetSubHeadId = item.BudgetSubHeadId;
                        //        detailModel.YearlyBilledList.Add(billedModel);

                        //        var addedList = yearWiseBilledList.Where(q => q.FinancialYearId == billed.FinancialYearId).DefaultIfEmpty().OfType<YearWiseBilledViewModel>().ToList();
                        //        if (addedList == null || addedList.Count == 0)
                        //        {
                        //            var statusModel = new YearWiseBilledViewModel();
                        //            statusModel.FinancialYearId = billed.FinancialYearId;
                        //            statusModel.FinancialYearList = Common.PopulateAccountingPeriodDdl(financialYearList);
                        //            yearWiseBilledList.Add(statusModel);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    GenerateYearlyBilled(detailModel, 0, string.Empty);
                        //}
                        #endregion

                        #region name Of Work List
                        var nameOfWorkList = _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.GetAll().ToList();

                        var list = new List<SelectListItem>();
                        foreach (var item2 in nameOfWorkList)
                        {
                            list.Add(new SelectListItem()
                            {
                                Text = item2.DescritionOfAPP,
                                Value = item2.DescritionOfAPP
                            });
                        }
                        detailModel.NameOfWorksList = list;
                        #endregion

                        model.BudgetDetailList.Add(detailModel);
                        model.YearWiseBudgetStatusList = yearWiseBudgetStatusList;
                        model.YearWiseBilledList = yearWiseBilledList;
                    }

                    //if (model.YearWiseBilledList.Count == 0)
                    //{
                    //    var yearWiseBilledListOne = GetYearWiseBilledList();
                    //    model.YearWiseBilledList.Add(yearWiseBilledListOne);
                    //}
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            PopulateDropdown(model);

            #region Approval Flow
            var approverStatus = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetByID(model.ApprovalStatusId);
            var apvModel = new ApprovalFlowViewModel();
            var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();
            apvModel.DesignationList = Common.PopulateDllList(designationList);
            model.ApproverList.Add(apvModel);
            #endregion

            model.ActionType = "SaveBudget";
            //model.ActionType = "UpdateBudget";
            model.ApprovalStatus = approverStatus == null ? string.Empty : approverStatus.Name;
            return View("CreateOrEdit", model);
        }
        #endregion

        public ActionResult Confirm(int id)
        {
            var masterData = _pmiCommonService.PMIUnit.BudgetMasterRepository.GetByID(id);
            masterData.IsConfirm = true;
            _pmiCommonService.PMIUnit.BudgetMasterRepository.Update(masterData);
            _pmiCommonService.PMIUnit.BudgetMasterRepository.SaveChanges();
            return RedirectToAction("Edit", new { id = id, type = "success" });
        }
    }
}