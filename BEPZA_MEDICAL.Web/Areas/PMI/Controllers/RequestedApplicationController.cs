using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PMI;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.AnnualProcurementPlan;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ApprovalFlow;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Budget;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Estimation;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Project;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.Controllers
{
    public class RequestedApplicationController : Controller
    {
        #region Declaration

        private readonly PMICommonService _pmiCommonService;
        private readonly PRMCommonSevice _prmCommonService;

        #endregion

        #region Ctrl
        public RequestedApplicationController(PMICommonService pmiCommonServices, PRMCommonSevice prmCommonService)
        {
            _pmiCommonService = pmiCommonServices;
            _prmCommonService = prmCommonService;
        }

        #endregion

        #region Action

        // GET: PMI/RequestedApplication
        public ActionResult Index()
        {
            return View();
        }


        #region Grid List
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ApprovalFlowViewModel model, string budgetType)
        {
            string filterExpression = String.Empty;
            int employeeId = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == MyAppSession.EmpId).Select(s => s.Id).FirstOrDefault();
            int totalRecords = 0;
            List<ApprovalFlowViewModel> list = (from x in _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.GetAll()
                                                join y in _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.GetAll() on x.Id equals y.ApprovalFlowMasterId
                                                join appStatus in _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll() on y.StatusId equals appStatus.Id
                                                where y.EmployeeId == employeeId && appStatus.Name != "Draft"
                                                select new ApprovalFlowViewModel
                                                {
                                                    Id = x.Id,
                                                    BudgetProjectAPPId = x.BudgetProjectAPPId,
                                                    ApprovalSectionId = x.ApprovalSectionId,
                                                    ApprovalSection = x.PMI_ApprovalSection.ApprovalSectionName,
                                                    CreateDate = x.CreateDate,
                                                    StatusId = y.StatusId,
                                                    Status = appStatus.Name.Contains("Submit") ? "Pending" : appStatus.Name
                                                }).ToList();


            #region Search

            if (request.Searching)
            {
                switch (model.StatusId)
                {
                    case 0:
                        list = list.ToList();
                        break;
                    default:
                        list = list.Where(x => x.StatusId == model.StatusId).ToList();
                        break;
                }

                switch (model.ApprovalSectionId.ToString())
                {
                    case "0":
                        list = list.ToList();
                        break;
                    default:
                        list = list.Where(x => x.ApprovalSectionId == model.ApprovalSectionId).ToList();
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
                    d.BudgetProjectAPPId,
                    d.ApprovalSectionId,
                    d.ApprovalSection,
                    (Convert.ToDateTime(d.CreateDate)).ToString(DateAndTime.GlobalDateFormat),
                    d.StatusId,
                    d.Status,
                    "View"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult GetStatus()
        {
            //var list = new List<SelectListItem>()   {
            //           new SelectListItem() {Text = "Pending",Value = "Pending"},
            //           new SelectListItem() {Text = "Recommended",Value = "Recommended"}
            //};

            var list = Common.PopulateDllList(_pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll().ToList());
            return PartialView("Select", list);
        }

        public ActionResult GetApplication()
        {
            var list = _pmiCommonService.PMIUnit.ApprovalSectionRepository.GetAll().ToList()
                        .Select(y =>
                        new SelectListItem()
                        {
                            Text = y.ApprovalSectionName,
                            Value = y.Id.ToString()
                        }).ToList();

            return PartialView("Select", list);

        }
        #endregion

        public ActionResult Edit(int id, string type)
        {
            if (type.Contains("Budget"))
            {
                #region Budget
                var model = new BudgetMasterViewModel();
                try
                {
                    var budgetStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll();
                    var approveStatusId = budgetStatusList.Where(q => q.Name.Contains("Appr")).FirstOrDefault().Id;

                    var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();

                    var master = _pmiCommonService.PMIUnit.BudgetMasterRepository.GetByID(id);
                    model = master.ToModel();
                    model.strCreateDate = model.CreationDate.ToString(DateAndTime.GlobalDateFormat);
                    model.strLastUpdateDate = model.LastUpdateDate.ToString(DateAndTime.GlobalDateFormat);

                    var details = _pmiCommonService.PMIUnit.BudgetDetailsRepository.Get(q => q.BudgetMasterId == id).DefaultIfEmpty().OfType<PMI_BudgetDetails>().ToList();

                    var budgetDetailList = new List<BudgetDetailViewModel>();
                    var yearlyCostList = new List<YearWiseBudgetStatusViewModel>();


                    foreach (var item in details)
                    {
                        var budgetDetailViewModel = new BudgetDetailViewModel();

                        budgetDetailViewModel.Id = item.Id;
                        budgetDetailViewModel.NameOfWorks = item.NameOfWorks;
                        budgetDetailViewModel.BudgetHeadName = _pmiCommonService.PMIUnit.BudgetHeadViewRepository.Get(x => x.Id == item.BudgetHeadId).Select(s => s.BudgetHead).FirstOrDefault();
                        budgetDetailViewModel.BudgetSubHeadName = _pmiCommonService.PMIUnit.BudgetHeadViewRepository.Get(x => x.Id == item.BudgetSubHeadId).Select(s => s.BudgetSubHead).FirstOrDefault();
                        budgetDetailViewModel.BudgetAmount = item.BudgetAmount;
                        budgetDetailViewModel.ConstructionTypeName = _pmiCommonService.PMIUnit.ConstructionTypeRepository.Get(x => x.Id == item.ConstructionTypeId).Select(s => s.Name).FirstOrDefault();
                        budgetDetailList.Add(budgetDetailViewModel);


                        var detailYearlyCost = _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.Get(q => q.BudgetDetailsId == item.Id).DefaultIfEmpty().OfType<PMI_BudgetDetailsYearlyCost>().ToList();

                        foreach(var item2 in detailYearlyCost){
                            var budgetDetailYearlyCostViewModel = new YearWiseBudgetStatusViewModel();

                            budgetDetailYearlyCostViewModel.FinancialYearName = _pmiCommonService.PMIUnit.AccountingPeriodRepository.Get(x => x.id == item2.FinancialYearId).Select(s => s.yearName).FirstOrDefault();
                            budgetDetailYearlyCostViewModel.StatusName = item2.PMI_ProjectStatus.Name;
                            budgetDetailYearlyCostViewModel.EstematedCost = item2.EstematedCost;
                            yearlyCostList.Add(budgetDetailYearlyCostViewModel);
                        }
                    }

                    model.BudgetDetailList = budgetDetailList;
                    model.YearWiseBudgetStatusList = yearlyCostList;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                PopulateDropdown(model);
                var zoneList = _pmiCommonService.PMIUnit.BudgetZoneRepository.Get(t => t.BudgetMasterId == model.Id).ToList();
                var zoneArray = string.Join(",", zoneList.Select(t => t.ZoneOrProjectId).ToArray());
                model.BudgetZonesString = zoneArray;
                model.ActionType = "Update";

                var apvModel = new ApprovalFlowViewModel();
                var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x=>x.Name).ToList();
                apvModel.DesignationList = Common.PopulateDllList(designationList);

                model.ApproverList.Add(apvModel);

                int employeeId = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID.Contains(MyAppSession.EmpId)).Select(s => s.Id).FirstOrDefault();
                var approvalStatus = (from x in _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Get(x => x.BudgetProjectAPPId == id && x.PMI_ApprovalSection.Enum.Contains("BGT"))
                                      join y in _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.GetAll() on x.Id equals y.ApprovalFlowMasterId
                                      where y.EmployeeId == employeeId
                                      select y).FirstOrDefault();

                model.ApprovalStatus = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.Get(x => x.Id == approvalStatus.StatusId).FirstOrDefault().Name;
                model.ApprovalStatusId = approvalStatus.StatusId;
                model.Remarks = approvalStatus.Remarks;
                model.ApprovalSelectionId = approvalStatus.PMI_ApprovalFlowMaster.ApprovalSectionId;
                return View("BudgetView", model);
                #endregion
            }
            else if (type.Contains("Project"))
            {
                #region Project
                var model = new ProjectViewModel();
                try
                {
                    var master = _pmiCommonService.PMIUnit.ProjectMasterRepository.GetByID(id);

                    var budgetDetails = _pmiCommonService.PMIUnit.BudgetDetailsRepository.GetByID(master.APPDetailsId);
                    var budgetMaster = _pmiCommonService.PMIUnit.BudgetMasterRepository.GetByID(budgetDetails.BudgetMasterId);


                    var tenderDetails = new List<PMI_TenderDetails>();
                    var projectDetails = new List<PMI_ProjectDetails>();
                    var projectZoneList = new List<PMI_BudgetZoneOrProject>();
                    var projectTimeExtensionList = new List<PMI_ProjectTimeExtension>();

                    if (master != null)
                    {
                        model = master.ToModel();
                        model.TenderOpeningDate = master.TenderOpeningDateTime.Value.Date;
                        model.TenderOpeningTime = Convert.ToDateTime(master.TenderOpeningDateTime).ToString("hh:mm:ss tt");
                        model.TenderClosingDate = master.TenderClosingDateTime.Value.Date;
                        model.TenderClosingTime = Convert.ToDateTime(master.TenderClosingDateTime).ToString("hh:mm:ss tt");

                        tenderDetails = _pmiCommonService.PMIUnit.TenderDetailsRepository.Get(t => t.ProjectMasterId == id).DefaultIfEmpty().OfType<PMI_TenderDetails>().ToList();
                        projectDetails = _pmiCommonService.PMIUnit.ProjectDetailsRepository.Get(t => t.ProjectMasterId == master.Id).DefaultIfEmpty().OfType<PMI_ProjectDetails>().ToList();
                        projectTimeExtensionList = _pmiCommonService.PMIUnit.ProjectTimeExtensionRepository.Get(q => q.ProjectId == master.Id).DefaultIfEmpty().OfType<PMI_ProjectTimeExtension>().ToList();
                    }

                    projectZoneList = _pmiCommonService.PMIUnit.BudgetZoneRepository.Get(t => t.BudgetMasterId == budgetDetails.BudgetMasterId).DefaultIfEmpty().OfType<PMI_BudgetZoneOrProject>().ToList();


                    model.MinistryId = budgetMaster.MinistryOrDivisionId;
                    model.MinistryName = budgetMaster.PMI_Ministry.Name;
                    model.APPMasterId = budgetMaster.Id;
                    model.APPDetailsId = budgetDetails.Id;
                    model.ActionType = "Update";
                    model.NameOfWorks = budgetDetails.NameOfWorks;
                    model.ConstructionTypeId = budgetDetails.ConstructionTypeId;
                    model.ConstructionTypeName = budgetDetails.PMI_ConstructionType.Name;

                    if (tenderDetails.Any())
                    {
                        foreach (var item in tenderDetails)
                        {
                            var tenderDetailModel = new TenderDetailsViewModel();
                            tenderDetailModel = item.ToModel();

                            model.TenderDetailsViewModelList.Add(tenderDetailModel);
                        }
                    }
                    if (projectDetails.Any())
                    {
                        foreach (var item in projectDetails)
                        {
                            var details = new ProjectDetailsViewModel();
                            details = item.ToModel();
                            details.ProcurementType = item.PMI_ProcurementType.Name;
                            details.ProcurementMethod = item.PMI_ProcurementMethod.Name;
                            details.SourceOfFund = item.PMI_SourceOfFund.Name;
                            details.ApprovalAuthority = item.PMI_ApprovalAuthority.Name;
                            //PopulateDropdownForProjectDetails(details);
                            model.ProjectDetailList.Add(details);
                        }
                    }

                    if (projectTimeExtensionList.Any())
                    {
                        foreach (var item in projectTimeExtensionList)
                        {
                            var mdl = new ProjectTimeExtensionViewModel();
                            mdl = item.ToModel();
                            model.ProjectTimeExtensionList.Add(mdl);
                        }
                    }

                    //BindProjectZoneInfo(model, projectZoneList);

                    if (master != null)
                    {
                        model.DrawingApprovalDate = master.DrawingApprovalDate == DateTime.MinValue || master.DrawingApprovalDate == null ? DateTime.Now : (DateTime)master.DrawingApprovalDate;
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
                var apvModel = new ApprovalFlowViewModel();
                var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();
                apvModel.DesignationList = Common.PopulateDllList(designationList);

                PopulateDropdownforPoject(model);

                model.ApproverList.Add(apvModel);

                int employeeId = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID.Contains(MyAppSession.EmpId)).Select(s => s.Id).FirstOrDefault();
                var approvalStatus = (from x in _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Get(x => x.BudgetProjectAPPId == id && x.PMI_ApprovalSection.Enum.Contains("PJT"))
                                      join y in _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.GetAll() on x.Id equals y.ApprovalFlowMasterId
                                      where y.EmployeeId == employeeId
                                      select y).FirstOrDefault();

                model.ApprovalStatus = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.Get(x => x.Id == approvalStatus.StatusId).FirstOrDefault().Name;
                model.ApprovalStatusId = approvalStatus.StatusId;
                model.Remarks = approvalStatus.Remarks;
                model.ApprovalSelectionId = approvalStatus.PMI_ApprovalFlowMaster.ApprovalSectionId;

                return View("ProjectView", model);
                #endregion
            }
            else if (type.Contains("APP"))
            {
                #region App
                var model = new AnnualProcurementPlanMasterViewModel();
                try
                {
                    var budgetStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll();
                    var approveStatusId = budgetStatusList.Where(q => q.Name.Contains("Appr")).FirstOrDefault().Id;

                    var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();

                    var master = _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.GetByID(id);
                    model = master.ToModel();

                    var details = _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.Get(q => q.AnnualProcurementPlanMasterId == id).ToList();

                    var budgetDetailList = new List<AnnualProcurementPlanDetailViewModel>();

                    foreach (var item in details)
                    {
                        var budgetDetailViewModel = new AnnualProcurementPlanDetailViewModel();

                        budgetDetailViewModel.Id = item.Id;
                        budgetDetailViewModel.LotNo = item.LotNo;
                        budgetDetailViewModel.PackageNo = item.PackageNo;
                        budgetDetailViewModel.Unit = item.Unit;
                        budgetDetailViewModel.Quantity = item.Quantity;
                        budgetDetailViewModel.DescritionOfAPP = item.DescritionOfAPP;
                        budgetDetailViewModel.BudgetHeadName = _pmiCommonService.PMIUnit.BudgetHeadViewRepository.Get(x => x.Id == item.APPHeadId).Select(s => s.BudgetHead).FirstOrDefault();
                        budgetDetailViewModel.EstdCost = item.EstdCost;
                        budgetDetailViewModel.ConstructionTypeName = _pmiCommonService.PMIUnit.ConstructionTypeRepository.Get(x => x.Id == item.ConstructionTypeId).Select(s => s.Name).FirstOrDefault();
                        budgetDetailViewModel.ContractValue = item.ContractValue;
                        budgetDetailViewModel.InvitationforTender = item.InvitationforTender;
                        budgetDetailViewModel.SigningofContract = item.SigningofContract;
                        budgetDetailViewModel.CompletionofContract = item.CompletionofContract;
                        budgetDetailViewModel.Remarks = item.Remarks;
                        budgetDetailList.Add(budgetDetailViewModel);


                        var detailYearlyCost = _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.Get(q => q.BudgetDetailsId == item.Id).DefaultIfEmpty().OfType<PMI_BudgetDetailsYearlyCost>().ToList();
                    }

                    model.AnnualProcurementPlanDetailList = budgetDetailList;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                PopulateDropdownForApp(model);
                var zoneList = _pmiCommonService.PMIUnit.AppZonesRepository.Get(t => t.APPMasterId == model.Id).ToList();
                var zoneArray = string.Join(",", zoneList.Select(t => t.ZoneOrProjectId).ToArray());
                model.BudgetZonesString = zoneArray;
                model.ActionType = "Update";

                var apvModel = new ApprovalFlowViewModel();
                var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();
                apvModel.DesignationList = Common.PopulateDllList(designationList);

                model.ApproverList.Add(apvModel);

                int employeeId = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID.Contains(MyAppSession.EmpId)).Select(s => s.Id).FirstOrDefault();
                var approvalStatus = (from x in _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Get(x => x.BudgetProjectAPPId == id && x.PMI_ApprovalSection.Enum.Contains("APP"))
                                      join y in _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.GetAll() on x.Id equals y.ApprovalFlowMasterId
                                      where y.EmployeeId == employeeId
                                      select y).FirstOrDefault();

                model.ApprovalStatus = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.Get(x => x.Id == approvalStatus.StatusId).FirstOrDefault().Name;
                model.ApprovalStatusId = approvalStatus.StatusId;
                model.Remarks = approvalStatus.Remarks;
                model.ApprovalSelectionId = approvalStatus.PMI_ApprovalFlowMaster.ApprovalSectionId;
                return View("AppView", model);
                #endregion
            }
            else if (type.Contains("Estimation"))
            {
                #region Estimation
                var master = _pmiCommonService.PMIUnit.ProjectEstimationRepository.Get(t => t.Id == id).FirstOrDefault();
                var model = master.ToModel();
                GetEstimatedItemInformation(model);

                var apvModel = new ApprovalFlowViewModel();
                var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();
                apvModel.DesignationList = Common.PopulateDllList(designationList);
                model.ApproverList.Add(apvModel);

                int employeeId = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID.Contains(MyAppSession.EmpId)).Select(s => s.Id).FirstOrDefault();
                var approvalStatus = (from x in _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Get(x => x.BudgetProjectAPPId == id && x.PMI_ApprovalSection.Enum.Contains("ETN"))
                                      join y in _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.GetAll() on x.Id equals y.ApprovalFlowMasterId
                                      where y.EmployeeId == employeeId
                                      select y).FirstOrDefault();

                model.ApprovalStatus = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.Get(x => x.Id == approvalStatus.StatusId).FirstOrDefault().Name;
                model.ApprovalStatusId = approvalStatus.StatusId;
                model.Remarks = approvalStatus.Remarks;
                model.ApprovalSelectionId = approvalStatus.PMI_ApprovalFlowMaster.ApprovalSectionId;

                return View("EstimationView", model);
                #endregion
            }

            return View("BudgetView");
        }

        #region Save and Update Approval Flow data
        [HttpPost]
        public ActionResult SaveApplication(BudgetMasterViewModel model)
        {
            try
            {
            var EmployeeList = model.ApproverList;
            var employeeId = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == MyAppSession.EmpId).Select(s => s.Id).FirstOrDefault();

            int approvalFlowMasterId = _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Get(x => x.BudgetProjectAPPId == model.Id && x.ApprovalSectionId == model.ApprovalSelectionId).Select(s => s.Id).FirstOrDefault();

            var perApprovarList = _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.GetAll().Where(x => x.ApprovalFlowMasterId == approvalFlowMasterId && x.EmployeeId == employeeId).ToList();

            foreach (var item in perApprovarList)
            {
                PMI_ApprovalFlowDetails ApprovalFlowDetails = new PMI_ApprovalFlowDetails();
                ApprovalFlowDetails.Id = item.Id;
                ApprovalFlowDetails.ApprovalFlowMasterId = approvalFlowMasterId;
                ApprovalFlowDetails.EmployeeId = item.EmployeeId;
                ApprovalFlowDetails.StatusId = model.ApprovalStatusId.Value;
                ApprovalFlowDetails.Remarks = model.Remarks;
                ApprovalFlowDetails.EUser = HttpContext.User.Identity.Name;
                ApprovalFlowDetails.EDate = DateTime.Now;

                _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.Update(ApprovalFlowDetails);
            }
            _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.SaveChanges();

            foreach (var item in EmployeeList)
            {
                PMI_ApprovalFlowDetails ApprovalFlowDetails = new PMI_ApprovalFlowDetails();
                if (item.EmployeeId != null)
                {
                    ApprovalFlowDetails.ApprovalFlowMasterId = approvalFlowMasterId;
                    ApprovalFlowDetails.EmployeeId = (int)item.EmployeeId;
                    ApprovalFlowDetails.StatusId = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.Get(x => x.Name.Contains("Submit")).Select(s => s.Id).FirstOrDefault();
                    ApprovalFlowDetails.Remarks = string.Empty;
                    ApprovalFlowDetails.IUser = HttpContext.User.Identity.Name;
                    ApprovalFlowDetails.IDate = DateTime.Now;

                    _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.Add(ApprovalFlowDetails);
                }
            }
            _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.SaveChanges();
            }
            catch
            {

            }
            return View("Index");
        }

        [HttpPost]
        public ActionResult SaveProject(ProjectViewModel model)
        {

            var EmployeeList = model.ApproverList;
            var employeeId = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == MyAppSession.EmpId).Select(s => s.Id).FirstOrDefault();

            int approvalFlowMasterId = _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Get(x => x.BudgetProjectAPPId == model.Id && x.PMI_ApprovalSection.Enum.Contains("PJT")).Select(s => s.Id).FirstOrDefault();

            var perApprovarList = _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.GetAll().Where(x => x.ApprovalFlowMasterId == approvalFlowMasterId && x.EmployeeId == employeeId).ToList();

            foreach (var item in perApprovarList)
            {
                PMI_ApprovalFlowDetails ApprovalFlowDetails = new PMI_ApprovalFlowDetails();
                ApprovalFlowDetails.Id = item.Id;
                ApprovalFlowDetails.ApprovalFlowMasterId = approvalFlowMasterId;
                ApprovalFlowDetails.EmployeeId = item.EmployeeId;
                ApprovalFlowDetails.StatusId = model.ApprovalStatusId.Value;
                ApprovalFlowDetails.Remarks = model.Remarks;
                ApprovalFlowDetails.EUser = HttpContext.User.Identity.Name;
                ApprovalFlowDetails.EDate = DateTime.Now;

                _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.Update(ApprovalFlowDetails);
            }
            _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.SaveChanges();

            foreach (var item in EmployeeList)
            {
                PMI_ApprovalFlowDetails ApprovalFlowDetails = new PMI_ApprovalFlowDetails();
                if (item.EmployeeId != null)
                {
                    ApprovalFlowDetails.ApprovalFlowMasterId = approvalFlowMasterId;
                    ApprovalFlowDetails.EmployeeId = (int)item.EmployeeId;
                    ApprovalFlowDetails.StatusId = model.ApprovalStatusId.Value;
                    ApprovalFlowDetails.Remarks = string.Empty;
                    ApprovalFlowDetails.IUser = HttpContext.User.Identity.Name;
                    ApprovalFlowDetails.IDate = DateTime.Now;

                    _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.Add(ApprovalFlowDetails);
                }
            }
            _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.SaveChanges();
            return View("Index");
        }

        [HttpPost]
        public ActionResult SaveEstimation(ProjectEstimationViewModel model)
        {
            var EmployeeList = model.ApproverList;
            var employeeId = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == MyAppSession.EmpId).Select(s => s.Id).FirstOrDefault();

            int approvalFlowMasterId = _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Get(x => x.BudgetProjectAPPId == model.Id && x.PMI_ApprovalSection.Enum.Contains("ETN")).Select(s => s.Id).FirstOrDefault();

            var perApprovarList = _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.GetAll().Where(x => x.ApprovalFlowMasterId == approvalFlowMasterId && x.EmployeeId == employeeId).ToList();

            foreach (var item in perApprovarList)
            {
                PMI_ApprovalFlowDetails ApprovalFlowDetails = new PMI_ApprovalFlowDetails();
                ApprovalFlowDetails.Id = item.Id;
                ApprovalFlowDetails.ApprovalFlowMasterId = approvalFlowMasterId;
                ApprovalFlowDetails.EmployeeId = item.EmployeeId;
                ApprovalFlowDetails.StatusId = model.ApprovalStatusId.Value;
                ApprovalFlowDetails.Remarks = model.Remarks;
                ApprovalFlowDetails.EUser = HttpContext.User.Identity.Name;
                ApprovalFlowDetails.EDate = DateTime.Now;

                _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.Update(ApprovalFlowDetails);
            }
            _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.SaveChanges();

            foreach (var item in EmployeeList)
            {
                PMI_ApprovalFlowDetails ApprovalFlowDetails = new PMI_ApprovalFlowDetails();
                if (item.EmployeeId != null)
                {
                    ApprovalFlowDetails.ApprovalFlowMasterId = approvalFlowMasterId;
                    ApprovalFlowDetails.EmployeeId = (int)item.EmployeeId;
                    ApprovalFlowDetails.StatusId = model.ApprovalStatusId.Value;
                    ApprovalFlowDetails.Remarks = string.Empty;
                    ApprovalFlowDetails.IUser = HttpContext.User.Identity.Name;
                    ApprovalFlowDetails.IDate = DateTime.Now;

                    _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.Add(ApprovalFlowDetails);
                }
            }
            _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.SaveChanges();
            return View("Index");
        }
        #endregion

        #endregion

        #region Other Methods

        private void GetEstimatedItemInformation(ProjectEstimationViewModel model)
        {
            var detailList = _pmiCommonService.PMIUnit.ProjectEstimationDetailsRepository.Get(t => t.MasterId == model.Id).DefaultIfEmpty().OfType<PMI_EstimationDetails>().ToList();
            var estimationItemList = _pmiCommonService.PMIUnit.EstimationSetupRepository.GetAll();
            var unitList = _pmiCommonService.PMIUnit.EstimationUnitRepository.GetAll();
            var headList = _pmiCommonService.PMIUnit.EstimationHeadRepository.GetAll();

            var projectWiseDescription = _pmiCommonService.PMIUnit.EstimationHeadDescriptionRepository.Get(q => q.MasterId == model.Id).DefaultIfEmpty().OfType<PMI_EstimationHeadDescription>().ToList();

            PopulateList(model);

            var estimationHeadIds = detailList.DistinctBy(q => q.EstimationHeadId).Select(q => q.EstimationHeadId);
            if (estimationHeadIds != null && estimationHeadIds.Count() > 0)
            {
                foreach (var heads in estimationHeadIds)
                {
                    var anEstimationDetail = new ProjectEstimationDetailsViewModel();

                    anEstimationDetail.EstimationHeadId = heads;

                    foreach (var item in detailList.Where(q => q.EstimationHeadId == heads).ToList())
                    {
                        var detailModel = item.ToModel();
                        detailModel.EstimationItemName = _pmiCommonService.PMIUnit.EstimationSetupRepository.Get(x => x.Id == item.EstimationItemId).Select(s => s.ItemName).FirstOrDefault();
                        detailModel.UnitName = _pmiCommonService.PMIUnit.EstimationUnitRepository.Get(x => x.Id == item.UnitId).Select(s => s.Name).FirstOrDefault();
                        model.EstimationItemList.Add(detailModel);
                    }

                    model.ProjectEstimationDetails.Add(anEstimationDetail);

                    var headDescriptionViewModel = new EstimationHeadDesciptionViewModel();
                    headDescriptionViewModel.HeadName = _pmiCommonService.PMIUnit.EstimationHeadRepository.Get(x => x.Id == heads).Select(s => s.HeadName).FirstOrDefault();
                    headDescriptionViewModel.EstimationHeadId = heads;
                    var description = string.Empty;
                    var projectDescObj = projectWiseDescription.Where(q => q.EstimationHeadId == heads).FirstOrDefault();
                    if (projectDescObj != null)
                    {
                        description = projectDescObj.HeadDescription;
                    }
                    else
                    {
                        description = headList.Where(q => q.Id == heads).FirstOrDefault().Description;
                    }
                    headDescriptionViewModel.HeadDescription = description;
                    model.EstimationHeadDescriptions.Add(headDescriptionViewModel);
                }
            }

            decimal budgetAmount = 0;
            var budgetInfo = (from cost in _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.GetAll()
                              join bd in _pmiCommonService.PMIUnit.BudgetDetailsRepository.GetAll() on cost.BudgetDetailsId equals bd.Id
                              join prj in _pmiCommonService.PMIUnit.ProjectMasterRepository.GetAll() on bd.Id equals prj.APPDetailsId
                              join sts in _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll() on cost.BudgetStatusId equals sts.Id
                              where sts.Name.Contains("Approv") && prj.Id == model.ProjectId
                              select cost
                                  ).FirstOrDefault();
            if (budgetInfo != null)
            {
                decimal.TryParse(budgetInfo.EstematedCost.ToString(), out budgetAmount);
            }
            budgetAmount = budgetAmount * 100000;
            model.BudgetAmount = budgetAmount;

            decimal totalAmount = 0;
            if (detailList != null)
            {
                totalAmount = detailList != null ? detailList.Sum(t => t.TotalAmount) : 0;
            }
            model.TotalAmount = totalAmount;
            model.StatusName = _pmiCommonService.PMIUnit.ProjectStatusRepository.Get(q => q.Id == model.EstimationStatusId).FirstOrDefault().Name;
            model.strMode = "Update";
            model.ActionType = "Update";
        }

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

            var projectOrZoneList = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().DefaultIfEmpty().OfType<PRM_ZoneInfo>().ToList();
            model.ProjectOrZoneList = Common.PopulateDdlZoneList(projectOrZoneList);

            var projectStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll().DefaultIfEmpty().OfType<PMI_ProjectStatus>().ToList();
            model.ProjectStatusList = Common.PopulateDllList(projectStatusList);

            var approvalStatusList = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll().Where(x =>! x.Name.Contains("Draft") && !x.Name.Contains("Submit")).ToList();
            model.ApprovalStatusList = Common.PopulateDllList(approvalStatusList);
        }

        public void PopulateDropdownforPoject(ProjectViewModel model)
        {
            //var ministryList = _pmiCommonService.PMIUnit.MinistryRepository.GetAll().DefaultIfEmpty().OfType<PMI_Ministry>().ToList();
            //model.MinistryList = Common.PopulateDllList(_pmiCommonService.PMIUnit.MinistryRepository.GetAll().DefaultIfEmpty().OfType<PMI_Ministry>().ToList());

            var agencyList = _pmiCommonService.PMIUnit.AgencyRepository.GetAll().DefaultIfEmpty().OfType<PMI_Agency>().ToList();
            model.AgencyList = Common.PopulateDllList(agencyList);

            var construmctionTypeList = _pmiCommonService.PMIUnit.ConstructionTypeRepository.GetAll().DefaultIfEmpty().OfType<PMI_ConstructionType>().ToList();
            model.ConstructionTypeList = Common.PopulateDllList(construmctionTypeList);

            var constructionCategoryList = _pmiCommonService.PMIUnit.ConstructionCategoryRepository.GetAll().DefaultIfEmpty().OfType<PMI_ConstructionCategory>().ToList();
            model.ConstructionCategoryList = Common.PopulateDllList(constructionCategoryList);

            var projectStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll().DefaultIfEmpty().OfType<PMI_ProjectStatus>().ToList();
            model.ProjectStatusList = Common.PopulateDllList(projectStatusList);

            var paymentStatusList = _pmiCommonService.PMIUnit.PaymentStatusRepository.GetAll().DefaultIfEmpty().OfType<PMI_PaymentStatus>().ToList();
            model.PaymentStatusList = Common.PopulateDllList(paymentStatusList);

            var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().DefaultIfEmpty().OfType<PRM_Designation>().ToList();
            model.DesignationList = Common.PopulateDllList(designationList);

            //var zoneList = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().DefaultIfEmpty().OfType<BEPZA_MEDICAL.DAL.PRM.PRM_ZoneInfo>().ToList();
            //model.ZoneList = Common.PopulateDdlZoneList(zoneList);

            var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll().OrderByDescending(x => x.yearName).ToList();
            model.FinancialYearList = Common.PopulateAccountingPeriodDdl(financialYearList);

            var approvalStatusList = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll().Where(x => !x.Name.Contains("Draft") && !x.Name.Contains("Submit")).ToList();
            model.ApprovalStatusList = Common.PopulateDllList(approvalStatusList);
        }

        private void PopulateList(ProjectEstimationViewModel model)
        {
            var estimationHeadList = _pmiCommonService.PMIUnit.EstimationHeadRepository.GetAll();
            model.EstimationHeadList = Common.PopulateEstimationHead(estimationHeadList);
            decimal totalAmount = 0;
            //var a = GetProjectInformation(model.ProjectId);
            //decimal.TryParse(a.Data.ToString(), out totalAmount);
            model.TotalAmount = totalAmount;
            var fyList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();
            int currentFyId = 0;
            if (fyList != null)
            {
                var currentFy = fyList.Where(q => q.isActive == true).LastOrDefault();
                if (currentFy != null)
                {
                    int.TryParse(currentFy.id.ToString(), out currentFyId);
                }
                else
                {
                    currentFyId = fyList.Max(q => q.id);
                }

            }
            //var projectList = _pmiCommonService.PMIUnit.ProjectViewRepository.Get(q => q.ZoneId == LoggedUserZoneInfoId && q.FinancialYearId == currentFyId).DefaultIfEmpty().OfType<vwPMIProjectSummary>().ToList();
            var projectList = _pmiCommonService.PMIUnit.ProjectViewRepository.Get(q => q.ZoneId == MyAppSession.ZoneInfoId).DefaultIfEmpty().OfType<vwPMIProjectSummary>().ToList();
            projectList = projectList.Where(q => q.ProjectMasterId != 0).DefaultIfEmpty().OfType<vwPMIProjectSummary>().ToList();
            model.ProjectList = projectList
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.DescritionOfAPP,
                    Value = y.ProjectMasterId.ToString()
                }).ToList();

            var statusList = _pmiCommonService.PMIUnit.StatusInformationRepository.Get(t => t.ApplicableFor == "Estimation").ToList();
            model.EstimationStatusList = Common.PopulateDllList(statusList);

            var approvalStatusList = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll().Where(x => !x.Name.Contains("Draft") && !x.Name.Contains("Submit")).ToList();
            model.ApprovalStatusList = Common.PopulateDllList(approvalStatusList);
        }
        private void PopulateDropdownForApp(AnnualProcurementPlanMasterViewModel model)
        {
            var ministryList = _pmiCommonService.PMIUnit.MinistryRepository.GetAll().ToList();
            model.DivisionOrMinistryList = Common.PopulateDllList(ministryList);

            var approvalAuthorityList = _pmiCommonService.PMIUnit.ApprovalAuthorityRepository.GetAll().DefaultIfEmpty().OfType<PMI_ApprovalAuthority>().ToList();
            model.ApprovalAuthorityList = Common.PopulateDllList(approvalAuthorityList);

            var projectOrZoneList = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().DefaultIfEmpty().ToList();
            model.ProjectOrZoneList = Common.PopulateDdlZoneList(projectOrZoneList);

            var projectStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll().DefaultIfEmpty().OfType<PMI_ProjectStatus>().ToList();
            model.ProjectStatusList = Common.PopulateDllList(projectStatusList);

            var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().ToList();
            model.DesignationList = Common.PopulateDllList(designationList);

            var approvalStatusList = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll().Where(x => !x.Name.Contains("Draft") && !x.Name.Contains("Submit")).ToList();
            model.ApprovalStatusList = Common.PopulateDllList(approvalStatusList);

            var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();
            model.FinancialYearList = Common.PopulateAccountingPeriodDdl(financialYearList);

            var statusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll();
            model.APPStatusList = Common.PopulateDllList(statusList);
        }
        #endregion
    }
}