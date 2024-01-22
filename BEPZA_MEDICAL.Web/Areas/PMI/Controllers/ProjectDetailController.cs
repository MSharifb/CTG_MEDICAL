using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PMI;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ApprovalFlow;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Project;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BEPZA_MEDICAL.Web.Areas.PMI.Controllers
{
    public class ProjectDetailController : BaseController
    {

        #region DECLARAION

        private readonly PMICommonService _pmiCommonService;
        private readonly PRMCommonSevice _prmCommonService;

        #endregion

        #region CONSTRUCTOR

        public ProjectDetailController(PMICommonService pmiCommonServices, PRMCommonSevice prmCommonService)
        {
            _pmiCommonService = pmiCommonServices;
            _prmCommonService = prmCommonService;
        }

        #endregion

        #region METHODS

        private void BindProjectZoneInfo(ProjectViewModel model, List<PMI_AppZones> selectedZones)
        {
            var query = (from zone in _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll()
                         join budgetZone in selectedZones on zone.Id equals budgetZone.ZoneOrProjectId
                         select zone).ToList();
            string zoneList = string.Join(",", query.Select(t => t.ZoneName));
            model.ProjectZones = zoneList;
        }

        public void PopulateDateField(ProjectViewModel model)
        {
            model.AdministrativeApprovalDate = DateTime.Now;
            model.TenderEvalDate = DateTime.Now;
            model.TenderIssueDate = DateTime.Now;
            model.TenderPubDate = DateTime.Now;
            model.TenderLastSellingDate = DateTime.Now;
            model.TenderClosingDate = DateTime.Now;
            model.TenderOpeningDate = DateTime.Now;
            model.CompletionDate = DateTime.Now;
            model.CreatedDate = DateTime.Now;
            model.LastPaymentDate = DateTime.Now;
            model.FinalPaymentDate = DateTime.Now;
            model.BudgetApprovalDate = DateTime.Now;
            model.DrawingApprovalDate = DateTime.Now;
        }

        public void PopulateDropdown(ProjectViewModel model)
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

            var approvalStatusList = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll().Where(x => x.Name.Contains("Submit") || x.Name.Contains("Draft")).ToList();
            model.ApprovalStatusList = Common.PopulateDllList(approvalStatusList);
        }

        private void PopulateDropdownForProjectDetails(ProjectDetailsViewModel model)
        {
            var procurementMethodList = _pmiCommonService.PMIUnit.ProcurementMethodRepository.GetAll().DefaultIfEmpty().OfType<PMI_ProcurementMethod>().ToList();
            model.ProcurementMethodList = Common.PopulateDllList(procurementMethodList);

            var procurementTypeList = _pmiCommonService.PMIUnit.ProcurementTypeRepository.GetAll().DefaultIfEmpty().OfType<PMI_ProcurementType>().ToList();
            model.ProcurementTypeList = Common.PopulateDllList(procurementTypeList);

            var sourceOfFundList = _pmiCommonService.PMIUnit.SourceOfFundRepository.GetAll().DefaultIfEmpty().OfType<PMI_SourceOfFund>().ToList();
            model.SourceOfFundList = Common.PopulateDllList(sourceOfFundList);

            var approvalAuthorityList = _pmiCommonService.PMIUnit.ApprovalAuthorityRepository.GetAll().DefaultIfEmpty().OfType<PMI_ApprovalAuthority>().ToList();
            model.ApprovalAuthorityList = Common.PopulateDllList(approvalAuthorityList);
        }

        #endregion

        #region Approval Flow

        public void SaveApprovalFlow(int BudgetProjectAPPId, int? approverStatusId, List<ApprovalFlowViewModel> approverList)
        {
            PMI_ApprovalFlowMaster ApprovalFlowMaster = new PMI_ApprovalFlowMaster();
            var approverStatus = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetByID(approverStatusId);
            var tempPeriod = _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Get(x => x.BudgetProjectAPPId == BudgetProjectAPPId && x.PMI_ApprovalSection.Enum.Contains("PJT")).FirstOrDefault();
            if (tempPeriod != null && approverList.Count > 0)
            {
                List<Type> allTypes = new List<Type> { typeof(PMI_ApprovalFlowDetails) };
                _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Delete(tempPeriod.Id, allTypes);
                _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.SaveChanges();
            }
            if (approverList.Count > 0)
            {
                foreach (var item in approverList)
                {
                    PMI_ApprovalFlowDetails ApprovalFlowDetails = new PMI_ApprovalFlowDetails();

                    ApprovalFlowDetails.ApprovalFlowMasterId = ApprovalFlowDetails.Id;
                    ApprovalFlowDetails.EmployeeId = (int)item.EmployeeId;
                    ApprovalFlowDetails.StatusId = approverStatusId.Value;
                    ApprovalFlowDetails.Remarks = string.Empty;
                    ApprovalFlowDetails.IUser = HttpContext.User.Identity.Name;
                    ApprovalFlowDetails.IDate = DateTime.Now;

                    _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.Add(ApprovalFlowDetails);
                }

                ApprovalFlowMaster.BudgetProjectAPPId = BudgetProjectAPPId;
                ApprovalFlowMaster.ApprovalSectionId = _pmiCommonService.PMIUnit.ApprovalSectionRepository.GetAll().Where(x => x.Enum == "PJT").Select(s => s.Id).FirstOrDefault();
                ApprovalFlowMaster.CreateDate = DateTime.Now;
                ApprovalFlowMaster.IUser = HttpContext.User.Identity.Name;
                ApprovalFlowMaster.IDate = DateTime.Now;

                _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Add(ApprovalFlowMaster);
                _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.SaveChanges();
            }
        }

        #endregion

        #region ACTION

        //
        // GET: /PMI/ProjectDetail/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddTenderDetails(TenderDetailsViewModel model)
        {
            model.CompletionDate = DateTime.Now;
            return PartialView("_TenderDetails", model);
        }

        [HttpPost]
        public ActionResult AddProjectDetails(ProjectDetailsViewModel model, string budgetId)
        {
            PopulateDropdownForProjectDetails(model);
            int id = 0;
            int.TryParse(budgetId, out id);
            //var budgetMaster = _pmiCommonService.PMIUnit.BudgetMasterRepository.GetByID(id);
            //model.ProcurementTypeId = budgetMaster.ProcurementTypeId;
            //model.SourceOfFundId = budgetMaster.SourceOfFundId;
            //model.ApprovalAuthorityId = budgetMaster.ApprovalAuthorityId;
            return PartialView("_ProjectDetails", model);
        }

        private PMI_ProjectMaster ToAttachFile(PMI_ProjectMaster projectMaster, HttpFileCollectionBase files)
        {
            foreach (string fileTagName in files)
            {
                HttpPostedFileBase file = Request.Files[fileTagName];
                if (file.ContentLength > 0)
                {
                    // Due to the limit of the max for a int type, the largest file can be
                    // uploaded is 2147483647, which is very large anyway.

                    int size = file.ContentLength;
                    string name = file.FileName;
                    int position = name.LastIndexOf("\\");
                    name = name.Substring(position + 1);
                    string contentType = file.ContentType;
                    byte[] fileData = new byte[size];
                    file.InputStream.Read(fileData, 0, size);
                    projectMaster.DrawingAttachment = fileData;
                    projectMaster.FileName = file.FileName;
                }
            }

            return projectMaster;
        }

        public ActionResult SaveProjectDetails([Bind(Exclude = "DrawingAttachment")]ProjectViewModel model)
        {
            try
            {
                ModelState.Clear();
                if (ModelState.IsValid)
                {
                    var projectMasterObj = model.ToEntity();
                    var tenderDetailsList = new List<PMI_TenderDetails>();
                    var projectDetailsList = new List<PMI_ProjectDetails>();
                    var projectZoneList = new List<PMI_ProjectZone>();

                    HttpFileCollectionBase files = Request.Files;
                    projectMasterObj = ToAttachFile(projectMasterObj, files);

                    projectMasterObj.TenderOpeningDateTime = Convert.ToDateTime(model.TenderOpeningDate + model.TenderOpeningTime);
                    projectMasterObj.TenderClosingDateTime = Convert.ToDateTime(model.TenderClosingDate + model.TenderClosingTime);
                    foreach (var item in model.TenderDetailsViewModelList)
                    {
                        item.ProjectMasterId = projectMasterObj.Id;
                        tenderDetailsList.Add(item.ToEntity());
                    }
                    _pmiCommonService.PMIUnit.ProjectMasterRepository.Add(projectMasterObj);

                    foreach (var details in model.ProjectDetailList)
                    {
                        var obj = details.ToEntity();
                        obj.ProjectMasterId = projectMasterObj.Id;
                        _pmiCommonService.PMIUnit.ProjectDetailsRepository.Add(obj);
                    }
                    foreach (var tenderObj in tenderDetailsList)
                    {
                        _pmiCommonService.PMIUnit.TenderDetailsRepository.Add(tenderObj);
                    }

                    //foreach (var zoneInfo in model.ZoneViewModelList.Where(t => t.IsSelected).ToList())
                    //{
                    //    var obj = new PMI_ProjectZone();
                    //    obj.ZoneId = zoneInfo.ZoneId;
                    //    obj.ProjectMasterId = projectMasterObj.Id;
                    //    obj.IUser = projectMasterObj.IUser;
                    //    obj.IDate = projectMasterObj.IDate;
                    //    projectZoneList.Add(obj);
                    //}


                    _pmiCommonService.PMIUnit.ProjectMasterRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.ProjectDetailsRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.TenderDetailsRepository.SaveChanges();

                    SaveApprovalFlow(projectMasterObj.Id, model.ApprovalStatusId, model.ApproverList);

                    model.IsError = 0;
                    model.ErrMsg = Resources.ErrorMessages.InsertSuccessful;

                    PopulateDropdown(model);
                    foreach (var item in model.ProjectDetailList)
                    {
                        PopulateDropdownForProjectDetails(item);
                    }

                    PopulateDateField(model);
                    //BindProjectZoneInfo(model, projectZoneList);
                    model.ActionType = "Update";

                }
                else
                {
                    model.IsError = 1;
                    model.ErrMsg = Resources.ErrorMessages.ExceptionMessage;
                }
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

            return View("CreateOrEdit", model);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ProjectViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var loggedZoneId = LoggedUserZoneInfoId;
            List<ProjectViewModel> list = (
                                            from prj in _pmiCommonService.PMIUnit.ProjectViewRepository.Get(q => q.ZoneId == loggedZoneId).DefaultIfEmpty().OfType<vwPMIProjectSummary>().ToList()
                                            select new ProjectViewModel
                                            {
                                                APPDetailsId = prj.AnnualProcurementPlanDetailsId,
                                                Id = prj.ProjectMasterId,
                                                NameOfWorks = prj.DescritionOfAPP,
                                                ProjectCode = prj.ProjectCode,
                                                FinancialYearId = prj.FinancialYearId,
                                                FinancialYearName = prj.FinancialYearName,
                                                TenderPubDate = prj.TenderPubDate,
                                                TotalCostOfProject = prj.TotalCost,
                                                MinistryName = prj.MinistryName,
                                                ProjectStatusName = prj.ProjectStatusName,
                                                ZoneId = prj.ZoneId,
                                                ZoneCode = prj.ZoneCode
                                            }).OrderBy(x => x.NameOfWorks).ToList();



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
                if (!string.IsNullOrEmpty(model.NameOfWorks))
                {
                    if (!model.NameOfWorks.Equals("0"))
                        list = list.Where(x => x.NameOfWorks.Trim().ToLower().Contains(model.NameOfWorks.Trim().ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(model.ProjectStatusName))
                {
                    if (!model.ProjectStatusName.Equals("0"))
                        list = list.Where(x => x.ProjectStatusName != null && x.ProjectStatusName.Trim().ToLower().Contains(model.ProjectStatusName.Trim().ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(model.FinancialYearName))
                {
                    if (!model.FinancialYearName.Equals("0"))
                        list = list.Where(x => x.FinancialYearName.Trim().ToLower().Equals(model.FinancialYearName.Trim().ToLower())).ToList();
                }
            }
            if (!request.Searching)
            {
                list = (from prj in list
                        join fy in _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll() on prj.FinancialYearId equals fy.id into lFy
                        from fy in lFy.DefaultIfEmpty()
                        //where fy.isActive == true
                        select new ProjectViewModel
                        {
                            APPDetailsId = prj.APPDetailsId,
                            Id = prj.Id,
                            NameOfWorks = prj.NameOfWorks,
                            ProjectCode = prj.ProjectCode,
                            FinancialYearId = prj.FinancialYearId,
                            FinancialYearName = prj.FinancialYearName,
                            TenderPubDate = prj.TenderPubDate,
                            TotalCostOfProject = prj.TotalCostOfProject,
                            MinistryName = prj.MinistryName,
                            ProjectStatusName = prj.ProjectStatusName,
                            ZoneId = prj.ZoneId,
                            ZoneCode = prj.ZoneCode
                        }).ToList();
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

                response.Records.Add(new JqGridRecord(d.Id == 0 ? "0" : Convert.ToString(d.Id), new List<object>()
                {
                   
                    d.APPDetailsId,
                    d.Id!= 0 ? d.Id:0,
                     "PD",
                    d.NameOfWorks,
                    d.ProjectCode !=null ? d.ProjectCode:null,
                    d.FinancialYearName != null ? d.FinancialYearName : null,
                    d.TenderPubDate != null ? d.TenderPubDate.Value.ToString("dd/MM/yyyy"):null,
                    d.TotalCostOfProject != 0 ? d.TotalCostOfProject.ToString("N2") : "0",
                    d.MinistryName != null ? d.MinistryName : null,
                    d.ProjectStatusName != null ? d.ProjectStatusName : null,
                    //d.ProjectImplementationAuthority,
                    //d.SourceOfFundName,
                    //d.NoOfPackage,
                    //d.PackageName,
                    
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Edit(int APPDetailsId, int id)
        {
            var model = new ProjectViewModel();
            try
            {
                //var budgetDetails = _pmiCommonService.PMIUnit.BudgetDetailsRepository.GetByID(BudgetDetailsId);
                //var budgetMaster = _pmiCommonService.PMIUnit.BudgetMasterRepository.GetByID(budgetDetails.BudgetMasterId);

                var appDetails = _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.GetByID(APPDetailsId);
                var appMaster = _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.GetByID(appDetails.AnnualProcurementPlanMasterId);

                var master = _pmiCommonService.PMIUnit.ProjectMasterRepository.GetByID(id);

                var tenderDetails = new List<PMI_TenderDetails>();
                var projectDetails = new List<PMI_ProjectDetails>();
                var projectZoneList = new List<PMI_AppZones>();
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
                else
                {

                    PopulateDateField(model);
                    model.ProjectCode = appMaster.ProjectCode;
                    model.ProcuringEntryName = appMaster.ProcuringEntityName;
                    model.ProcuringEntryCode = appMaster.ProcuringEntityCode;
                }

                projectZoneList = _pmiCommonService.PMIUnit.AppZonesRepository.Get(t => t.APPMasterId == appDetails.AnnualProcurementPlanMasterId).DefaultIfEmpty().OfType<PMI_AppZones>().ToList();


                model.MinistryId = appMaster.MinistryOrDivisionId;
                model.MinistryName = appMaster.PMI_Ministry.Name;
                model.APPMasterId = appMaster.Id;
                model.APPDetailsId = appDetails.Id;
                model.ActionType = "Update";
                model.NameOfWorks = appDetails.DescritionOfAPP;
                model.ConstructionTypeId = appDetails.ConstructionTypeId;
                model.ConstructionTypeName = appDetails.PMI_ConstructionType.Name;

                model.CompletionDate = appDetails.CompletionofContract;
                model.ContractSignDate = appDetails.SigningofContract;

                if (tenderDetails.Any())
                {
                    foreach (var item in tenderDetails)
                    {
                        var tenderDetailModel = new TenderDetailsViewModel();
                        tenderDetailModel = item.ToModel();

                        model.TenderDetailsViewModelList.Add(tenderDetailModel);
                    }
                }
                else
                {
                    var tenderDetailModel = new TenderDetailsViewModel();
                    tenderDetailModel.CompletionDate = DateTime.Now;
                    tenderDetailModel.LotNumber = appDetails.LotNo;

                    model.TenderDetailsViewModelList.Add(tenderDetailModel);
                }

                if (projectDetails.Any())
                {
                    foreach (var item in projectDetails)
                    {
                        var details = new ProjectDetailsViewModel();
                        details = item.ToModel();

                        PopulateDropdownForProjectDetails(details);
                        model.ProjectDetailList.Add(details);
                    }
                }
                else
                {
                    var details = new ProjectDetailsViewModel();

                   // details.ProcurementMethodId = appDetails.ProcurementTypeId.Value;
                    details.SourceOfFundId = appDetails.SourceOfFundId.Value;
                    //details.ApprovalAuthorityId = appDetails.ApprovingAuthorityId.Value;
                    details.PackageNo = appDetails.PackageNo;
                    details.Unit = appDetails.Unit;
                    //details.Quantity = appDetails.Quantity;
                    details.EstimatedCost = appDetails.EstdCost.Value;

                    PopulateDropdownForProjectDetails(details);

                    model.ProjectDetailList.Add(details);
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
                else
                {
                    var mdl = new ProjectTimeExtensionViewModel();
                    mdl.ExtendedDays = 0;
                    model.ProjectTimeExtensionList.Add(mdl);
                }


                BindProjectZoneInfo(model, projectZoneList);
                if (master != null)
                {
                    model.DrawingApprovalDate = master.DrawingApprovalDate == DateTime.MinValue || master.DrawingApprovalDate == null ? DateTime.Now : (DateTime)master.DrawingApprovalDate;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            PopulateDropdown(model);

            #region Approval Flow
            
            var approverStatus = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetByID(model.ApprovalStatusId==null?0 : model.ApprovalStatusId);
            if (id == 0)
            {
                var apvModel = new ApprovalFlowViewModel();
                var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();
                apvModel.DesignationList = Common.PopulateDllList(designationList);

                model.ApproverList.Add(apvModel); 
            }
            else
            {
                var approverList = (from x in _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.GetAll()
                                    join y in _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.GetAll() on x.Id equals y.ApprovalFlowMasterId
                                    join appStatus in _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll() on y.StatusId equals appStatus.Id
                                    join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on y.EmployeeId equals emp.Id
                                    where (x.BudgetProjectAPPId == id)
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
                                        Status = appStatus.Name.Contains("Submit") ? "Pending" : appStatus.Name

                                    }).ToList();
                if (approverStatus.Name.Contains("Draft"))
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
                    model.ApproverList = approverList;
                }
                model.ApprovalStatus = approverStatus == null ? string.Empty : approverStatus.Name;
            }
            #endregion

            return View("CreateOrEdit", model);

        }

        public ActionResult ShowProjectPrintPopUp()
        {
            return PartialView("PrintProject");
        }

        public ActionResult Update(ProjectViewModel model)
        {
            var projectZoneList = new List<PMI_AppZones>();
            PMI_ProjectMaster projectMasterObj = new PMI_ProjectMaster();
            try
            {

                if (ModelState.IsValid)
                {
                    projectMasterObj = model.ToEntity();
                    var tenderDetailsList = new List<PMI_TenderDetails>();
                    var projectDetailList = new List<PMI_ProjectDetails>();
                    var projectTimeExtensionList = new List<PMI_ProjectTimeExtension>();

                    HttpFileCollectionBase files = Request.Files;
                    projectMasterObj = ToAttachFile(projectMasterObj, files);

                    if (files == null)
                    {
                        var existingObj = _pmiCommonService.PMIUnit.ProjectMasterRepository.GetByID(projectMasterObj.Id);
                        projectMasterObj.DrawingAttachment = existingObj.DrawingAttachment;
                    }

                    foreach (var item in model.ProjectDetailList)
                    {
                        var obj = item.ToEntity();
                        obj.ProjectMasterId = projectMasterObj.Id;
                        projectDetailList.Add(obj);
                    }

                    foreach (var item in model.TenderDetailsViewModelList)
                    {
                        item.ProjectMasterId = projectMasterObj.Id;
                        tenderDetailsList.Add(item.ToEntity());
                    }

                    foreach (var item in model.ProjectTimeExtensionList)
                    {
                        item.ProjectId = projectMasterObj.Id;
                        projectTimeExtensionList.Add(item.ToEntity());
                    }

                    DateTime openingDateTime = Convert.ToDateTime(model.TenderOpeningDate.ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(model.TenderOpeningTime).ToString("HH:mm:ss"));
                    DateTime closingDateTime = Convert.ToDateTime(model.TenderClosingDate.ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(model.TenderClosingTime).ToString("HH:mm:ss"));

                    projectMasterObj.TenderOpeningDateTime = openingDateTime;
                    projectMasterObj.TenderClosingDateTime = closingDateTime;

                    //projectMasterObj.TenderOpeningDateTime = model.TenderOpeningDate + Convert.ToDateTime(model.TenderOpeningTime);
                    //projectMasterObj.TenderClosingDateTime = model.TenderClosingDate + Convert.ToDateTime(model.TenderClosingTime);

                    if (projectMasterObj.Id != 0)
                    {
                        _pmiCommonService.PMIUnit.ProjectMasterRepository.Update(projectMasterObj);
                    }
                    else
                    {
                        _pmiCommonService.PMIUnit.ProjectMasterRepository.Add(projectMasterObj);
                    }



                    var existingProjectDetails = _pmiCommonService.PMIUnit.ProjectDetailsRepository.Get(t => t.ProjectMasterId == projectMasterObj.Id).ToList();
                    var deletdProjectDetails = (from existing in existingProjectDetails
                                                where !(projectDetailList.Any(dt => dt.Id == existing.Id))
                                                select existing).DefaultIfEmpty().OfType<PMI_ProjectDetails>().ToList();

                    var existingTenders = _pmiCommonService.PMIUnit.TenderDetailsRepository.Get(t => t.ProjectMasterId == projectMasterObj.Id).ToList();
                    var deletedTenders = (from e in existingTenders
                                          where !(tenderDetailsList.Any(t => t.Id == e.Id))
                                          select e).DefaultIfEmpty().OfType<PMI_TenderDetails>().ToList();

                    var existingTimeExtension = _pmiCommonService.PMIUnit.ProjectTimeExtensionRepository.Get(q => q.ProjectId == projectMasterObj.Id);
                    var deletedTime = (from e in existingTimeExtension
                                       where !(projectTimeExtensionList.Any(q => q.Id == e.Id))
                                       select e).DefaultIfEmpty().OfType<PMI_ProjectTimeExtension>().ToList();

                    foreach (var item in deletdProjectDetails)
                    {
                        _pmiCommonService.PMIUnit.ProjectDetailsRepository.Delete(item.Id);
                    }

                    foreach (var item in deletedTenders)
                    {
                        _pmiCommonService.PMIUnit.TenderDetailsRepository.Delete(item.Id);
                    }

                    foreach (var item in deletedTime)
                    {
                        _pmiCommonService.PMIUnit.ProjectTimeExtensionRepository.Delete(item.Id);
                    }

                    foreach (var detailObj in projectDetailList)
                    {
                        if (detailObj.Id != 0)
                        {
                            _pmiCommonService.PMIUnit.ProjectDetailsRepository.Update(detailObj);
                        }
                        else
                        {
                            _pmiCommonService.PMIUnit.ProjectDetailsRepository.Add(detailObj);
                        }
                    }
                    foreach (var tenderObj in tenderDetailsList)
                    {
                        if (tenderObj.Id != 0)
                        {
                            _pmiCommonService.PMIUnit.TenderDetailsRepository.Update(tenderObj);
                        }
                        else
                        {
                            _pmiCommonService.PMIUnit.TenderDetailsRepository.Add(tenderObj);
                        }
                    }

                    foreach (var item in projectTimeExtensionList)
                    {
                        if (item.Id != 0)
                        {
                            _pmiCommonService.PMIUnit.ProjectTimeExtensionRepository.Update(item);
                        }
                        else
                        {
                            if (item.ExtendedDays > 0)
                            {
                                _pmiCommonService.PMIUnit.ProjectTimeExtensionRepository.Add(item);
                            }
                        }
                    }

                    _pmiCommonService.PMIUnit.ProjectMasterRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.TenderDetailsRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.ProjectDetailsRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.ProjectTimeExtensionRepository.SaveChanges();

                    SaveApprovalFlow(projectMasterObj.Id, model.ApprovalStatusId, model.ApproverList);

                    model.IsError = 0;
                    model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;

                    projectZoneList = _pmiCommonService.PMIUnit.AppZonesRepository.Get(t => t.APPMasterId == projectMasterObj.Id).DefaultIfEmpty().OfType<PMI_AppZones>().ToList();
                }
                else
                {
                    model.IsError = 1;
                    model.ErrMsg = Resources.ErrorMessages.ExceptionMessage;
                }
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
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            //return Json(new
            //{
            //    Message = model.ErrMsg,
            //    IsError = model.IsError
            //}, JsonRequestBehavior.AllowGet);

            PopulateDropdown(model);
            foreach (var item in model.ProjectDetailList)
            {
                PopulateDropdownForProjectDetails(item);
            }



            PopulateDateField(model);
            BindProjectZoneInfo(model, projectZoneList);
            model.ActionType = "Update";
            return RedirectToAction("Edit", new { APPDetailsId = model.APPDetailsId, id = projectMasterObj.Id, type = "success" });
            //return View("Index", model);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Delete(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _pmiCommonService.PMIUnit.ProjectMasterRepository.Delete(id);
                var tenderDetailsList = _pmiCommonService.PMIUnit.TenderDetailsRepository.Get(t => t.ProjectMasterId == id).DefaultIfEmpty().OfType<PMI_TenderDetails>().ToList();
                var projectDetailList = _pmiCommonService.PMIUnit.ProjectDetailsRepository.Get(t => t.ProjectMasterId == id).DefaultIfEmpty().OfType<PMI_ProjectDetails>().ToList();
                var zoneList = _pmiCommonService.PMIUnit.ProjectZoneRepository.Get(t => t.ProjectMasterId == id).DefaultIfEmpty().OfType<PMI_ProjectZone>().ToList();
                if (projectDetailList.Any())
                {
                    foreach (var item in projectDetailList)
                    {
                        _pmiCommonService.PMIUnit.ProjectDetailsRepository.Delete(item.Id);
                    }
                }
                if (tenderDetailsList.Any())
                {
                    foreach (var tenderDetail in tenderDetailsList)
                    {
                        _pmiCommonService.PMIUnit.TenderDetailsRepository.Delete(tenderDetail.Id);
                    }
                }
                if (zoneList.Any())
                {
                    foreach (var item in zoneList)
                    {
                        _pmiCommonService.PMIUnit.ProjectZoneRepository.Delete(item.Id);
                    }
                }

                _pmiCommonService.PMIUnit.TenderDetailsRepository.SaveChanges();
                _pmiCommonService.PMIUnit.ProjectDetailsRepository.SaveChanges();
                _pmiCommonService.PMIUnit.ProjectZoneRepository.SaveChanges();
                _pmiCommonService.PMIUnit.ProjectMasterRepository.SaveChanges();


                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteDetail(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                var tenderDetail = _pmiCommonService.PMIUnit.TenderDetailsRepository.GetByID(id);

                if (tenderDetail != null)
                {
                    _pmiCommonService.PMIUnit.TenderDetailsRepository.Delete(tenderDetail.Id);
                    _pmiCommonService.PMIUnit.TenderDetailsRepository.SaveChanges();
                    result = false;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);

                }
            }
            catch (Exception ex)
            {
                result = true;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }

            return Json(new
            {
                IsError = result,
                Message = errMsg
            });
        }

        public ActionResult DownloadFile(int projectId)
        {
            try
            {
                var projectInfo = _pmiCommonService.PMIUnit.ProjectMasterRepository.GetByID(projectId);
                string fileName = string.Empty;
                byte[] fileData = null;
                if (projectInfo != null)
                {
                    if (!string.IsNullOrEmpty(projectInfo.FileName))
                    {
                        fileName = projectInfo.FileName;
                        fileData = projectInfo.DrawingAttachment;
                        string filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
                        string contentType = MimeMapping.GetMimeMapping(filePath);
                        var cd = new System.Net.Mime.ContentDisposition
                        {
                            FileName = fileName,
                            Inline = true,
                        };

                        Response.AppendHeader("Content-Disposition", cd.ToString());

                        return File(fileData, contentType);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            //string filename = "File.pdf";
            //string filepath = AppDomain.CurrentDomain.BaseDirectory + "/Path/To/File/" + filename;
            //byte[] filedata = System.IO.File.ReadAllBytes(filepath);
            //string contentType = MimeMapping.GetMimeMapping(filepath);

            //var cd = new System.Net.Mime.ContentDisposition
            //{
            //    FileName = filename,
            //    Inline = true,
            //};

            //Response.AppendHeader("Content-Disposition", cd.ToString());

            //return File(filedata, contentType);
        }

        [HttpPost]
        public ActionResult Upload(ProjectViewModel assignment, HttpPostedFileBase file)
        {
            if (Request.Files != null && Request.Files.Count == 1)
            {
                //var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var content = new byte[file.ContentLength];
                    file.InputStream.Read(content, 0, file.ContentLength);
                    assignment.DrawingAttachment = content;

                    // the rest of your db code here
                }
            }
            return RedirectToAction("Create");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetZoneList()
        {
            var zoneList = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll();
            var list = new List<string>();
            foreach (var zone in zoneList)
            {
                list.Add(zone.ZoneName);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNameOfWorks()
        {
            var masterList = _pmiCommonService.PMIUnit.BudgetDetailsRepository.GetAll().DistinctBy(q => q.NameOfWorks).ToList();
            Dictionary<string, string> dicNameOfWorks = new Dictionary<string, string>();

            foreach (var item in masterList)
            {
                dicNameOfWorks.Add(item.NameOfWorks, item.NameOfWorks);
            }

            return PartialView("_Select", dicNameOfWorks);
        }

        public ActionResult GetFinancialYear()
        {
            var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll().OrderByDescending(q => q.yearName).DefaultIfEmpty().ToList();
            Dictionary<string, string> dicFinancialYear = new Dictionary<string, string>();

            foreach (var item in financialYearList)
            {
                dicFinancialYear.Add(item.yearName, item.yearName);
            }

            return PartialView("_Select", dicFinancialYear);
        }

        public ActionResult GetProjectStatus()
        {
            var projectStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll();
            Dictionary<string, string> dicProjectStaus = new Dictionary<string, string>();

            foreach (var item in projectStatusList)
            {
                dicProjectStaus.Add(item.Name, item.Name);
            }

            return PartialView("_Select", dicProjectStaus);
        }

        [HttpPost]
        public ActionResult AddProjectTimeExtension(ProjectTimeExtensionViewModel model)
        {
            return PartialView("_ProjectTimeExtension", model);
        }

        #endregion

        #region Report Param

        private PrintProjectParamViewModel PopulateDropdown(PrintProjectParamViewModel model)
        {
            var financialYearList = (from fy in _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll().OrderByDescending(t => t.yearName).ToList()
                                     join bc in _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.GetAll() on fy.id equals bc.FinancialYearId
                                     select fy).DefaultIfEmpty().OfType<acc_Accounting_Period_Information>().ToList();

            var fyList = financialYearList.DistinctBy(q => q.yearName).DefaultIfEmpty();
            model.FinancialYearList = Common.PopulateAccountingPeriodDdl(fyList);
            var activeFy = fyList.FirstOrDefault(t => t.isActive == true);
            model.FinancialYearId = activeFy != null ? activeFy.id : fyList.Select(q => q.id).FirstOrDefault();

            var projectList = _pmiCommonService.PMIUnit.ProjectMasterRepository.GetAll().DistinctBy(q => q.APPDetailsId).ToList();
            model.ProjectList = Common.PopulateDdlProjectList(projectList);

            var zoneList = _pmiCommonService.PMIUnit.ProjectZoneRepository.Get(t => t.ProjectMasterId == model.ProjectId).ToList();
            foreach (var item in zoneList)
            {
                var zoneModel = item.ToModel();
                model.ZoneList.Add(zoneModel);
            }
            var approvalTypeList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll();
            model.ApprovalTypeList = Common.PopulateDllList(approvalTypeList);

            return model;
        }

        private List<string> GetTenderNoticeFormatList()
        {
            var list = new List<string> { "Standard Application Form for Enlistment (SAFE) for Goods (SAFE – A)", "PW3 - Standard Tender Document (National) For Procurement of Works [Open Tendering Method] (December 2016)", "e-PG3-Procurement of Goods through e-GP System", "PW2a - Standard Tender Document (National) For Procurement of Works [Open Tendering Method] (January-2017)", "PW2b - Preliminary Working draft: Standard Tender Document (National) For Procurement of Works [Limited Tendering Method] [December-2012]", "PG3 - Preliminary Working draft: Standard Tender Document (National)For Procurement of Goods [Open Tendering Method] (February 2015)" };
            return list;
        }

        public ActionResult ShowProcurementPlanParam(int projectId)
        {
            try
            {
                var projectInfo = _pmiCommonService.PMIUnit.ProjectMasterRepository.GetByID(projectId);
                var model = new PrintProjectParamViewModel();
                model.ProjectId = projectId;
                PopulateDropdown(model);
                model.ReportName = @"Annual Procurement Plan";
                //model.FinancialYearId = projectInfo.FinancialYearId;
                //model.FinancialYearName = _famCommonService.FAMUnit.FinancialYearInformationRepository.GetByID(projectInfo.FinancialYearId).FinancialYearName;
                model.NameOfWork = projectInfo.NameOfWorks;
                model.ProjectDate = (DateTime)projectInfo.TenderPubDate;
                model.ProjectDateStr = projectInfo.TenderPubDate.Value.ToString("dd-MM-yyyy");
                model.ProjectId = projectInfo.Id;
                var projectStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll();
                model.ProjectStatusList = Common.PopulateDllList(projectStatusList);
                return PartialView("_ReportParam", model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult ShowTenderNoticeParam(int projectId)
        {
            try
            {
                var projectInfo = _pmiCommonService.PMIUnit.ProjectMasterRepository.GetByID(projectId);
                var model = new PrintProjectParamViewModel();
                model.ProjectId = projectId;
                model.ReportName = "Tender Notice";
                //model.FinancialYearId = projectInfo.FinancialYearId;
                //model.FinancialYearName = _famCommonService.FAMUnit.FinancialYearInformationRepository.GetByID(projectInfo.FinancialYearId).FinancialYearName;
                model.NameOfWork = projectInfo.NameOfWorks;
                model.ProjectDate = (DateTime)projectInfo.TenderPubDate;
                model.ProjectDateStr = projectInfo.TenderPubDate.Value.ToString("dd-MM-yyyy");
                model.ProjectId = projectInfo.Id;
                model.TenderNoticeFormatList = GetTenderNoticeFormatList();
                PopulateDropdown(model);
                return PartialView("_ReportParam", model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult ShowBudgetSummaryParam(int projectId)
        {
            var projectInfo = _pmiCommonService.PMIUnit.ProjectMasterRepository.GetByID(projectId);
            var model = new PrintProjectParamViewModel();
            model.ProjectId = projectId;
            model.ReportName = "Budget Summary";
            model.NameOfWork = projectInfo.NameOfWorks;
            model.ProjectDate = (DateTime)projectInfo.TenderPubDate;
            model.ProjectDateStr = projectInfo.TenderPubDate.Value.ToString("dd-MM-yyyy");
            model.ProjectId = projectInfo.Id;
            PopulateDropdown(model);
            var paramModel = new BudgetSummaryParamViewModel();

            paramModel.FinancialYearList = model.FinancialYearList;
            paramModel.ApprovalTypeList = model.ApprovalTypeList;
            model.ParamList.Add(paramModel);
            model.ActionType = @"PrintBudgetSummary";
            return PartialView("_ReportParam", model);
        }

        [HttpPost]
        public ActionResult GetCurrentFinancialYear()
        {
            var currentFy = _pmiCommonService.PMIUnit.AccountingPeriodRepository.Get(t => t.isActive == true).FirstOrDefault();
            return Json(currentFy.yearName, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddParamBudgetSummaryDetails(PrintProjectParamViewModel model)
        {
            PopulateDropdown(model);
            var paramModel = new BudgetSummaryParamViewModel();
            paramModel.FinancialYearList = model.FinancialYearList;
            paramModel.ApprovalTypeList = model.ApprovalTypeList;
            model.ParamList.Add(paramModel);
            return PartialView("_ReportParamBudgetSummary", model.ParamList);
        }



        #endregion

    }
}