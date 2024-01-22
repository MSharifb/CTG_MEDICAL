using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Domain.WFM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Approval;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.RequestedApplication;
using BEPZA_MEDICAL.Web.Areas.WFM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ApprovalController : BaseController
    {

        #region Fields
        private readonly PRMCommonSevice _prmCommonservice;
        private readonly EmployeeService _empService;
        private readonly WFMCommonService _wpfCommonService;
        private readonly INVCommonService _invCommonService;
        private readonly CPFCommonService _cpfCommonService;
        #endregion

        public ApprovalController(PRMCommonSevice prmCommonService, EmployeeService empService, WFMCommonService wfmCommonService, INVCommonService invCommonService, CPFCommonService cpfCommonService)
        {
            _prmCommonservice = prmCommonService;
            _empService = empService;
            _wpfCommonService = wfmCommonService;
            _invCommonService = invCommonService;
            _cpfCommonService = cpfCommonService;
        }
        [NoCache]
        public ActionResult Index(RequestedApplicationViewModel model)
        {
            /* Add Process Name to Generate List of Approval Process */

            var processTypeList = _prmCommonservice.PRMUnit.ApprovalProcessRepository.Get(
                    q => q.ProcessNameEnum == "WFM"
                        || q.ProcessNameEnum == "ACR"
                        || q.ProcessNameEnum == "CPFMbr"
                        || q.ProcessNameEnum == "INVReq"
                        || q.ProcessNameEnum == "InvScrap"
                        ).ToList();

            /* End of Process List*/

            model.AprovalProcessList = Common.PopulateDdlApprovalProcess(processTypeList);

            var statusList = _prmCommonservice.PRMUnit.ApprovalStatusRepository.GetAll().DefaultIfEmpty().OfType<BEPZA_MEDICAL.DAL.PRM.APV_ApprovalStatus>().ToList();
            model.ApprovalStatusList = Common.PopulateDdlApprovalStatusForRecommenderApprover(statusList);
            model.StartDate = DateTime.Now;
            model.EndDate = DateTime.Now;

            return View("Index", model);
        }
        [NoCache]
        public ActionResult SearchItem(int approvalProcessId, int approvalStatusId, DateTime? startDate, DateTime? endDate)
        {
            var applicantList = new List<RequestedApplicationViewModel>();
            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);

            string approvalProcessName = _prmCommonservice.PRMUnit.ApprovalProcessRepository.GetByID(approvalProcessId).ProcessNameEnum;
            string partialViewName = string.Empty;
            switch (approvalProcessName)
            {
                case "WFM":
                    partialViewName = "_WelfareFundApplicationList";
                    GetWelfareFundApplications(approvalProcessId, approvalStatusId, startDate, endDate, applicantList, loginUser);
                    break;

                case "ACR":
                    partialViewName = "_AcrApplicationList";
                    GetAcrApplications(approvalProcessId, approvalStatusId, startDate, endDate, applicantList, loginUser);
                    break;

                case "CPFMbr":
                    partialViewName = "_CpfApplicationList";
                    GetCpfMembershipApplications(approvalProcessId, approvalStatusId, startDate, endDate, applicantList, loginUser);
                    break;

                case "INVReq":
                    partialViewName = "_INVRequisitionApplicationList";
                    GetInventoryRequisitions(approvalProcessId, approvalStatusId, startDate, endDate, applicantList, loginUser);
                    break;

                case "InvScrap":
                    partialViewName = "_InvScrapApplicationList";
                    GetInventoryScrapItemRequisitions(approvalProcessId, approvalStatusId, startDate, endDate, applicantList, loginUser);
                    break;
            }
            return PartialView(partialViewName, applicantList);
        }
        [NoCache]
        private void GetInventoryScrapItemRequisitions(int approvalProcessId, int approvalStatusId, DateTime? startDate, DateTime? endDate, List<RequestedApplicationViewModel> applicantList, EmployeeService.EmpLoginInfo loginUser)
        {
            var invScrapList = _prmCommonservice.PRMUnit.FunctionRepository.GetInvScrapItemApplications(approvalProcessId, loginUser.EmpId, approvalStatusId, (DateTime)startDate, (DateTime)endDate).OfType<Apv_GetInvScrapItemApplication_Result>().ToList();
            if (invScrapList != null && invScrapList.Count > 0)
            {
                foreach (var item in invScrapList)
                {
                    var app = new RequestedApplicationViewModel
                    {
                        Id = item.Id,
                        ApplicationId = item.Id,
                        ApplicantId = item.EmpID,
                        ApplicantName = item.EmployeeName,
                        ApplicationDate = Convert.ToDateTime(item.ScrapDate),
                        Designation = item.DesignationName,
                        Department = item.DepartmentName,
                        Zone = item.ZoneCode,
                        ApprovalStatusName = item.ApprovalStatusName,
                        RequestedAmount = item.TotalQuantity,
                    };
                    applicantList.Add(app);
                }
            }
        }
        [NoCache]
        private void GetInventoryRequisitions(int approvalProcessId, int approvalStatusId, DateTime? startDate, DateTime? endDate, List<RequestedApplicationViewModel> applicantList, EmployeeService.EmpLoginInfo loginUser)
        {
            var INVReqList = _prmCommonservice.PRMUnit.FunctionRepository.GetINVRequisitionApplications(approvalProcessId, loginUser.EmpId, approvalStatusId, (DateTime)startDate, (DateTime)endDate).OfType<APV_GetINVRequisitionApplication_Result>().ToList();
            if (INVReqList != null && INVReqList.Count > 0)
            {
                foreach (var item in INVReqList)
                {
                    var app = new RequestedApplicationViewModel
                    {
                        Id = item.Id,
                        ApplicationId = item.Id,
                        ApplicantId = item.EmpID,
                        ApplicantName = item.FullName,
                        ApplicationDate = Convert.ToDateTime(item.IndentDate),
                        Designation = item.DesignationName,
                        Department = item.DepartmentName,
                        Zone = item.ZoneCode,
                        ApprovalStatusName = item.ApprovalStatusName,
                    };
                    applicantList.Add(app);
                }
            }
        }
        [NoCache]
        private void GetCpfMembershipApplications(int approvalProcessId, int approvalStatusId, DateTime? startDate, DateTime? endDate, List<RequestedApplicationViewModel> applicantList, EmployeeService.EmpLoginInfo loginUser)
        {
            var cpfMemberList = _prmCommonservice.PRMUnit.FunctionRepository.GetCpfMembershipApplications(approvalProcessId, loginUser.EmpId, approvalStatusId, (DateTime)startDate, (DateTime)endDate).OfType<Apv_GetCpfMembershipApplication_Result>().ToList();
            if (cpfMemberList != null && cpfMemberList.Count > 0)
            {
                foreach (var item in cpfMemberList)
                {
                    var app = new RequestedApplicationViewModel
                    {
                        Id = item.Id,
                        ApplicationId = item.Id,
                        ApplicantId = item.EmpID,
                        ApplicantName = item.FullName,
                        ApplicationDate = Convert.ToDateTime(item.ApplicationDate),
                        Designation = item.DesignationName,
                        Department = item.DepartmentName,
                        Zone = item.ZoneCode,
                        ApprovalStatusName = item.ApprovalStatusName,
                    };
                    applicantList.Add(app);
                }
            }
        }
        [NoCache]
        private void GetAcrApplications(int approvalProcessId, int approvalStatusId, DateTime? startDate, DateTime? endDate, List<RequestedApplicationViewModel> applicantList, EmployeeService.EmpLoginInfo loginUser)
        {
            var acrList = _prmCommonservice.PRMUnit.FunctionRepository.GetAcrApplications(approvalProcessId, loginUser.EmpId, approvalStatusId, (DateTime)startDate, (DateTime)endDate).OfType<Apv_GetAcrApplication_Result>().ToList();
            if (acrList != null && acrList.Count > 0)
            {
                foreach (var item in acrList)
                {
                    var app = new RequestedApplicationViewModel
                    {
                        Id = item.Id,
                        ApplicationId = item.Id,
                        IsOnlineApplication = Convert.ToBoolean(item.IsOnline),
                        ApplicantId = item.EmpID,
                        ApplicantName = item.FullName,
                        ApplicationDate = Convert.ToDateTime(item.ACRDate),
                        StartDate = Convert.ToDateTime(item.ACRPeriodFrom),
                        EndDate = Convert.ToDateTime(item.ACRPeriodTo),
                        ApprovalStatusName = item.ApprovalStatusName,
                        ActionName = item.ActionName,
                        ControllerName = item.ControllerName
                    };
                    applicantList.Add(app);
                }
            }
        }
        [NoCache]
        private void GetWelfareFundApplications(int approvalProcessId, int approvalStatusId, DateTime? startDate, DateTime? endDate, List<RequestedApplicationViewModel> applicantList, EmployeeService.EmpLoginInfo loginUser)
        {
            var list = _prmCommonservice.PRMUnit.FunctionRepository.GetWelfareFundApplications(loginUser.EmpId, approvalProcessId, startDate, endDate, approvalStatusId).DefaultIfEmpty().OfType<APV_GetWelfareFundApplication_Result>().ToList();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    var app = new RequestedApplicationViewModel
                    {
                        ApprovalProcessId = approvalProcessId,
                        ApplicationId = item.ApplicationId,
                        ApprovalStatusId = item.ApprovalStatusId,
                        ApplicationDate = item.ApplicationDate,
                        ApplicantId = item.EmploymentId,
                        ApplicantName = item.ApplicantName,
                        RequestedAmount = item.AppliedAmount,
                        ApprovalStatusName = item.ApprovalStatusName == "Submit" ? "Submited" : item.ApprovalStatusName,
                        IsSelected = false,
                        IsOnlineApplication = Convert.ToBoolean(item.IsOnline),
                        Id = Convert.ToInt32(item.Id),
                    };

                    applicantList.Add(app);
                }
            }
        }
        [NoCache]
        public JsonResult ExecuteApprovalAction(string applicationId, string actionName, string approverComment, string nextApproverId)
        {
            int applicationIdInt = 0, nextApproverIdInt = 0;
            int.TryParse(applicationId, out applicationIdInt);
            int.TryParse(nextApproverId, out nextApproverIdInt);
            string loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).EmpId;
            var result = _prmCommonservice.PRMUnit.FunctionRepository.ProceedToNextStep(applicationIdInt, actionName, approverComment, nextApproverIdInt, loginUser);
            string approvalResult = string.Empty;
            if (result != null)
            {
                approvalResult = result.ReturnMessage;
            }
            return Json(approvalResult, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult ExeWFMApprovalAction(string applicationId, string actionName, string approverComment, string nextApproverId, decimal? suggestAmount)
        {
            int applicationIdInt = 0, nextApproverIdInt = 0;
            int.TryParse(applicationId, out applicationIdInt);
            int.TryParse(nextApproverId, out nextApproverIdInt);
            string loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).EmpId;

            var result = _prmCommonservice.PRMUnit.FunctionRepository.ProceedToNextStep(applicationIdInt, actionName, approverComment, nextApproverIdInt, loginUser);
            _prmCommonservice.PRMUnit.FunctionRepository.APV_WFM_UpdateApplication(applicationIdInt, suggestAmount);
            string approvalResult = string.Empty;
            if (result != null)
            {
                approvalResult = result.ReturnMessage;
            }
            return Json(approvalResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InvRequisitionExecuteApproval(RequestedApplicationViewModel model)
        {
            var currentObj = _prmCommonservice.PRMUnit.RequestedApplicationInformationRepository.GetByID(model.Id);
            //var applicationList = _prmCommonservice.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApprovalProcessId == currentObj.ApprovalProcessId && q.ApplicationId == currentObj.ApplicationId && q.ApproverId == model.ApproverId && q.ApprovalStatusId != null).DefaultIfEmpty().OfType<APV_ApplicationInformation>().ToList();
            if (currentObj != null)
            {
                var result = _prmCommonservice.PRMUnit.FunctionRepository.ProceedToNextStep(model.Id, model.NextStepName, model.ApproverComments, model.ApproverId, HttpContext.User.Identity.Name);

                foreach (var item in model.INVRequisitionDtlList)
                {
                    _prmCommonservice.PRMUnit.FunctionRepository.APV_INV_UpdateRequisitionDtl(item.Id, item.RequisitionId, item.ItemId, item.RecommendQuantity);
                }
                //return View("Index", model);
            }
            model.ApprovalStatusId = currentObj.ApprovalStatusId;
            return RedirectToAction("Index", model);
        }

        [HttpPost]
        public ActionResult InvScrapItemExecuteApproval(RequestedApplicationViewModel model)
        {
            var currentObj = _prmCommonservice.PRMUnit.RequestedApplicationInformationRepository.GetByID(model.Id);
            //var applicationList = _prmCommonservice.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApprovalProcessId == currentObj.ApprovalProcessId && q.ApplicationId == currentObj.ApplicationId && q.ApproverId == model.ApproverId && q.ApprovalStatusId != null).DefaultIfEmpty().OfType<APV_ApplicationInformation>().ToList();
            if (currentObj != null)
            {
                var result = _prmCommonservice.PRMUnit.FunctionRepository.ProceedToNextStep(model.Id, model.NextStepName, model.ApproverComments, model.ApproverId, HttpContext.User.Identity.Name);
                foreach (var item in model.INVRequisitionDtlList)
                {
                    var obj = _invCommonService.INVUnit.ScrapItemRepository.GetByID(item.Id);
                    obj.ApproveQuantity = item.RecommendQuantity;
                    _invCommonService.INVUnit.ScrapItemRepository.Update(obj);
                }
                _invCommonService.INVUnit.ScrapItemRepository.SaveChanges();
            }
            model.ApprovalStatusId = currentObj.ApprovalStatusId;
            return RedirectToAction("Index", model);
        }
        [NoCache]
        public ActionResult ViewApplication(int approvalProcessId, int requestedApplicationId, DateTime applicationDate, int statusId)
        {
            //bool isOnlineApplication = true;
            //bool.TryParse(isOnline, out isOnlineApplication);

            var model = new RequestedApplicationViewModel();
            model.ApplicationDate = model.EndDate = applicationDate;

            model.ApprovalStatusId = statusId;
            model.ApprovalProcessId = approvalProcessId;

            string approvalProcessEnum = _prmCommonservice.PRMUnit.ApprovalProcessRepository.Get(q => q.Id == approvalProcessId).FirstOrDefault().ProcessNameEnum;

            var applicationInfo = _prmCommonservice.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApprovalProcessId == approvalProcessId && q.Id == requestedApplicationId).FirstOrDefault();
            bool isOnlineApplication = true;
            if (applicationInfo != null)
            {
                bool.TryParse(applicationInfo.IsOnlineApplication.ToString(), out isOnlineApplication);
            }

            switch (approvalProcessEnum)
            {
                case "WFM":
                    model.IsConfigurableApprovalFlow = _prmCommonservice.PRMUnit.ApprovalFlowConfigurationRepository.GetAll().Where(x => x.APV_ApprovalProcess.ProcessNameEnum.Contains("WFM")).FirstOrDefault().IsConfigurableApprovalFlow;
                    switch (isOnlineApplication)
                    {
                        case true:
                            GetOnlineApplicationInformation(model, requestedApplicationId, applicationInfo);
                            break;
                        case false:
                            GetOfflineApplicationInformation(model, requestedApplicationId, applicationInfo);
                            break;
                    }
                    break;
            }

            return PartialView("_RequestedApplicationItem", model);
        }

        [NoCache]
        public ActionResult ViewINVRequisitionApplication(int approvalProcessId, int requestedApplicationId, DateTime applicationDate, int statusId)
        {
            var model = new RequestedApplicationViewModel();
            model.ApplicationDate = model.EndDate = applicationDate;
            model.ApprovalStatusId = statusId;
            model.ApprovalProcessId = approvalProcessId;

            var applicationInfo = _prmCommonservice.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApprovalProcessId == approvalProcessId && q.Id == requestedApplicationId).FirstOrDefault();
            if (applicationInfo != null)
            {
                var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
                DateTime startDate = model.StartDate;
                DateTime endDate = model.EndDate;
                var approvalInfo = _prmCommonservice.PRMUnit.FunctionRepository.GetINVRequisitionApplications(approvalProcessId, loginUser.EmpId, statusId, startDate, endDate).DefaultIfEmpty().OfType<APV_GetINVRequisitionApplication_Result>().ToList();
                var anApproval = approvalInfo.Where(q => q.Id == requestedApplicationId).FirstOrDefault();

                model.ModuleId = applicationInfo.ModuleId;
                model.ApprovalProcessId = applicationInfo.ApprovalProcessId;
                model.ApplicationId = applicationInfo.ApplicationId;
                model.IsOnlineApplication = applicationInfo.IsOnlineApplication;
                model.ApprovalStepId = applicationInfo.ApprovalStepId;
                model.ApproverId = applicationInfo.ApproverId;
                model.ApprovalStatusId = applicationInfo.ApprovalStatusId;
                model.ApproverComments = applicationInfo.ApproverComments;

                model.Id = applicationInfo.Id;
                model.IUser = applicationInfo.IUser;
                model.IDate = applicationInfo.IDate;
                model.NextStepName = anApproval.StepName;

            }

            var applicationModel = new ViewApplicationViewModel();
            var application = _invCommonService.INVUnit.RequisitionInfoRepository.GetByID(applicationInfo.ApplicationId);

            if (application != null)
            {
                var applicantInfo = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(q => q.Id == application.IssuedToEmpId).FirstOrDefault();
                applicationModel.ApplicationDate = application.IndentDate;
                applicationModel.ApplicantEmployeeId = applicantInfo.EmpID;
                applicationModel.ApplicantName = applicantInfo.FullName;
                applicationModel.ApplicantDepartment = applicantInfo.PRM_Discipline.Name;
                applicationModel.ApplicantDesignation = applicantInfo.PRM_Designation.Name;
                applicationModel.AppliedAmount = application.TotalQuantity.Value;
                applicationModel.ApplicationNo = application.IndentNo;

                var INVReqDtlList = new List<INVRequisitionDtlViewModel>();
                var INVReqDtl = _invCommonService.INVUnit.RequisitionItemRepository.GetAll().Where(x => x.RequisitionId == applicationInfo.ApplicationId).ToList();
                foreach (var item in INVReqDtl)
                {
                    decimal approvedQuantity = 0;
                    if (System.DBNull.Value.Equals(item.ApproveQuantity))
                    {
                        approvedQuantity = item.Quantity;
                    }
                    if (!System.DBNull.Value.Equals(item.ApproveQuantity))
                    {
                        approvedQuantity = Convert.ToDecimal(item.ApproveQuantity);
                    }

                    INVReqDtlList.Add(new INVRequisitionDtlViewModel
                    {
                        Id = item.Id,
                        ItemId = item.ItemId,
                        RequisitionId = item.RequisitionId,
                        Quantity = item.Quantity,
                        ItemName = item.INV_ItemInfo.ItemName,
                        Comment = item.Comment,
                        RecommendQuantity = approvedQuantity
                    }
                   );
                }
                model.INVRequisitionDtlList = INVReqDtlList;
            }

            var approvalHistoryList = new List<ApprovalHistoryViewModel>();
            var approvalHistory = _prmCommonservice.PRMUnit.ApprovalHistoryRepository.Get(q => q.ApplicationId == model.ApplicationId && q.ApprovalProcesssId == model.ApprovalProcessId).DefaultIfEmpty().OfType<vwApvApplicationWiseApprovalStatu>().ToList();
            foreach (var item in approvalHistory)
            {
                approvalHistoryList.Add(new ApprovalHistoryViewModel
                {
                    ApprovalStepName = item.StepSequence,
                    ApproverComment = item.ApproverComments == null ? string.Empty : item.ApproverComments,
                    ApprovalStatus = item.ApprovalStatusName,
                    ApproverIdAndName = item.ApproverIdAndName,
                });
            }

            model.ApprovalHistory = approvalHistoryList;

            var nextAproverList = _prmCommonservice.PRMUnit.FunctionRepository.GetNextApprover(applicationInfo.ApprovalProcessId, application.Id, applicationInfo.ApprovalStepId);
            model.NextApproverList = Common.PopulateEmployeeDDL(nextAproverList);
            model.Application = applicationModel;
            model.IsConfigurableApprovalFlow = _prmCommonservice.PRMUnit.ApprovalFlowConfigurationRepository.GetAll().Where(x => x.APV_ApprovalProcess.ProcessNameEnum.Contains("INVReq")).FirstOrDefault().IsConfigurableApprovalFlow;

            return PartialView("_RequestedINVReqApplicationItem", model);
        }
        [NoCache]
        public ActionResult ViewInvScrapItemApplication(int approvalProcessId, int requestedApplicationId, DateTime applicationDate, int statusId)
        {
            var model = new RequestedApplicationViewModel();
            model.ApplicationDate = model.EndDate = applicationDate;
            model.ApprovalStatusId = statusId;
            model.ApprovalProcessId = approvalProcessId;

            var applicationInfo = _prmCommonservice.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApprovalProcessId == approvalProcessId && q.Id == requestedApplicationId).FirstOrDefault();
            if (applicationInfo != null)
            {
                var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
                DateTime startDate = model.StartDate;
                DateTime endDate = model.EndDate;
                var approvalInfo = _prmCommonservice.PRMUnit.FunctionRepository.GetInvScrapItemApplications(approvalProcessId, loginUser.EmpId, statusId, startDate, endDate).DefaultIfEmpty().OfType<Apv_GetInvScrapItemApplication_Result>().ToList();
                var anApproval = approvalInfo.Where(q => q.Id == requestedApplicationId).FirstOrDefault();

                model.ModuleId = applicationInfo.ModuleId;
                model.ApprovalProcessId = applicationInfo.ApprovalProcessId;
                model.ApplicationId = applicationInfo.ApplicationId;
                model.IsOnlineApplication = applicationInfo.IsOnlineApplication;
                model.ApprovalStepId = applicationInfo.ApprovalStepId;
                model.ApproverId = applicationInfo.ApproverId;
                model.ApprovalStatusId = applicationInfo.ApprovalStatusId;
                model.ApproverComments = applicationInfo.ApproverComments;

                model.Id = applicationInfo.Id;
                model.IUser = applicationInfo.IUser;
                model.IDate = applicationInfo.IDate;
                model.NextStepName = anApproval.StepName;

            }

            var applicationModel = new ViewApplicationViewModel();
            var application = _invCommonService.INVUnit.ScrapInfoRepository.GetByID(applicationInfo.ApplicationId);

            if (application != null)
            {
                var applicantInfo = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(q => q.Id == application.IssuedToEmpId).FirstOrDefault();
                applicationModel.ApplicationDate = application.ScrapDate;
                applicationModel.ApplicantEmployeeId = applicantInfo.EmpID;
                applicationModel.ApplicantName = applicantInfo.FullName;
                applicationModel.ApplicantDepartment = applicantInfo.PRM_Discipline.Name;
                applicationModel.ApplicantDesignation = applicantInfo.PRM_Designation.Name;
                applicationModel.AppliedAmount = application.TotalQuantity.Value;
                applicationModel.ApplicationNo = application.ScrapNo;

                var INVReqDtlList = new List<INVRequisitionDtlViewModel>();
                var INVReqDtl = _invCommonService.INVUnit.ScrapItemRepository.GetAll().Where(x => x.ScrapId == applicationInfo.ApplicationId).ToList();
                foreach (var item in INVReqDtl)
                {
                    decimal approvedQuantity = 0;
                    if (System.DBNull.Value.Equals(item.ApproveQuantity))
                    {
                        approvedQuantity = item.Quantity;
                    }
                    if (!System.DBNull.Value.Equals(item.ApproveQuantity))
                    {
                        approvedQuantity = Convert.ToDecimal(item.ApproveQuantity);
                    }

                    INVReqDtlList.Add(new INVRequisitionDtlViewModel
                    {
                        Id = item.Id,
                        ItemId = item.ItemId,
                        RequisitionId = item.ScrapId,
                        Quantity = item.Quantity,
                        ItemName = item.INV_ItemInfo.ItemName,
                        Comment = item.Comment,
                        RecommendQuantity = approvedQuantity,
                    }
                   );
                }
                model.INVRequisitionDtlList = INVReqDtlList;
            }

            var approvalHistoryList = new List<ApprovalHistoryViewModel>();
            var approvalHistory = _prmCommonservice.PRMUnit.ApprovalHistoryRepository.Get(q => q.ApplicationId == model.ApplicationId && q.ApprovalProcesssId == model.ApprovalProcessId).DefaultIfEmpty().OfType<vwApvApplicationWiseApprovalStatu>().ToList();
            foreach (var item in approvalHistory)
            {
                approvalHistoryList.Add(new ApprovalHistoryViewModel
                {
                    ApprovalStepName = item.StepSequence,
                    ApproverComment = item.ApproverComments == null ? string.Empty : item.ApproverComments,
                    ApprovalStatus = item.ApprovalStatusName,
                    ApproverIdAndName = item.ApproverIdAndName,
                });
            }


            model.ApprovalHistory = approvalHistoryList;
            var nextAproverList = _prmCommonservice.PRMUnit.FunctionRepository.GetNextApprover(applicationInfo.ApprovalProcessId, application.Id, applicationInfo.ApprovalStepId);
            //var nextAproverList = _prmCommonservice.PRMUnit.FunctionRepository.GetNextApprover(applicationInfo.ApprovalProcessId, application.Id);
            model.IsConfigurableApprovalFlow = _prmCommonservice.PRMUnit.ApprovalFlowConfigurationRepository.GetAll().Where(x => x.APV_ApprovalProcess.ProcessNameEnum.Contains("InvScrap")).FirstOrDefault().IsConfigurableApprovalFlow;
            model.NextApproverList = Common.PopulateEmployeeDDL(nextAproverList);
            model.Application = applicationModel;
            return PartialView("_RequestedInvScrapApplicationItem", model);
        }
        [NoCache]
        public ActionResult ViewPfMembershipApplication(int approvalProcessId, int requestedApplicationId, DateTime applicationDate, int statusId)
        {
            var model = new RequestedApplicationViewModel();
            model.ApplicationDate = model.EndDate = applicationDate;
            model.ApprovalStatusId = statusId;
            model.ApprovalProcessId = approvalProcessId;
            model.Id = requestedApplicationId;
            var applicationInfo = _prmCommonservice.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApprovalProcessId == approvalProcessId && q.Id == requestedApplicationId).FirstOrDefault();

            var application = _cpfCommonService.CPFUnit.MembershipInformationRepository.GetByID(applicationInfo.ApplicationId);
            var rpt = _cpfCommonService.CPFUnit.FunctionRepository.GetMembershipInformation(application.EmployeeId);

            GeneratePfMembershipApplication(model, rpt);


            var approvalHistoryList = new List<ApprovalHistoryViewModel>();
            var approvalHistory = _prmCommonservice.PRMUnit.ApprovalHistoryRepository.Get(q => q.ApplicationId == applicationInfo.ApplicationId && q.ApprovalProcesssId == model.ApprovalProcessId).DefaultIfEmpty().OfType<vwApvApplicationWiseApprovalStatu>().ToList();
            foreach (var item in approvalHistory)
            {
                approvalHistoryList.Add(new ApprovalHistoryViewModel
                {
                    ApprovalStepName = item.StepSequence,
                    ApproverComment = item.ApproverComments == null ? string.Empty : item.ApproverComments,
                    ApprovalStatus = item.ApprovalStatusName,
                    ApproverIdAndName = item.ApproverIdAndName,
                });
            }

            model.ApprovalHistory = approvalHistoryList;
            var nextAproverList = _prmCommonservice.PRMUnit.FunctionRepository.GetNextApprover(applicationInfo.ApprovalProcessId, application.Id, applicationInfo.ApprovalStepId);
            string stepName = nextAproverList.Count == 0 ? string.Empty : nextAproverList.DistinctBy(q => q.StepTypeName).LastOrDefault().StepTypeName;
            model.NextStepName = stepName;
            model.IsConfigurableApprovalFlow = _prmCommonservice.PRMUnit.ApprovalFlowConfigurationRepository.GetAll().Where(x => x.APV_ApprovalProcess.ProcessNameEnum.Contains("CPFMbr")).FirstOrDefault().IsConfigurableApprovalFlow;

            if (!stepName.Contains("Appr"))
            {
                model.NextApproverList = Common.PopulateEmployeeDDL(nextAproverList);
            }

            return PartialView("_CpfApplication", model);
        }
        [NoCache]
        private void GeneratePfMembershipApplication(RequestedApplicationViewModel model, CPF_RptCpfMemberInformation_Result data)
        {
            var app = new CpfApplicationViewModel();
            var companyInfo = _prmCommonservice.PRMUnit.CompanyInformation.GetAll().FirstOrDefault();
            app.CompanyName = companyInfo.CompanyName;
            app.ApplicationTo = "The Secretary";
            app.ApplicationAddress1 = @"BEPZA Employees Contributory Provident Fund, Trust";
            app.ApplicationAddress2 = @"BEPZA, Dhaka.";
            app.ApplicationSubject = @"Application for enrolment as Member, Bangladesh Export Processing Zones Authority Employee's Contributory Provident Fund Trust";
            app.ApplicationBody1 = @"I " + data.EmployeeName + @" son/daughter of " + data.FatherName + @" hereby declare that I have read and understood 
                                        the P.F Rules of BEPZA and request your to enroll me as a member of Bangladesh Export Processing Zones Authority 
                                        Employee's Contributory Provident Fund. I further declare that I shall be bound in all respect by the rules of the same being in force.";

            app.ApplicationBody2 = @"I hereby Authorized and request you to deduct from my salary/each subscription with effect from " + data.ApplicationDate.Value.ToString("dd-MMMM-yyyy") + @" as I may from time to time
                                   be liable to pay under and in accordance with rules, a copy of which has been furnished to me and to pay the same to the Trustees of the said fund.";

            app.ApplicantName = data.EmployeeName;
            app.SonDaugherOf = data.FatherName;
            app.Designation = data.Designation;
            app.IdNo = data.EmpID;
            app.EmployeeType = data.EmploymentType;
            app.Section = data.Section;
            app.Department = data.Department;
            app.JoiningDate = data.JoiningDate;
            app.PresentPayScale = data.SalaryScaleName;
            app.BasicPay = Convert.ToDecimal(data.BasicAmount);
            app.Nationality = data.Nationality;
            app.DateOfBirth = data.DateofBirth;
            app.PermanentAddress = data.PermanentAddress;
            app.Witness = data.WitnessName;
            app.ReceiptDate = Convert.ToDateTime(data.ApplicationReceiptDate);
            app.EmployeeSignatureDate = Convert.ToDateTime(data.ApplicationDate);

            model.CpfApplication = app;
        }
        [NoCache]
        private RequestedApplicationViewModel GetOnlineApplicationInformation(RequestedApplicationViewModel mainModel, int requestedApplicationId, APV_ApplicationInformation applicationInfo)
        {
            if (applicationInfo != null)
            {
                var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
                DateTime startDate = mainModel.StartDate;
                DateTime endDate = mainModel.EndDate;
                int statusId = Convert.ToInt32(mainModel.ApprovalStatusId);
                int approvalProcessId = Convert.ToInt32(mainModel.ApprovalProcessId);
                var approvalInfo = _prmCommonservice.PRMUnit.FunctionRepository.GetWelfareFundApplications(loginUser.EmpId, approvalProcessId, startDate, endDate, statusId).DefaultIfEmpty().OfType<APV_GetWelfareFundApplication_Result>().ToList();
                var anApproval = approvalInfo.Where(q => q.Id == requestedApplicationId).FirstOrDefault();

                mainModel.ModuleId = applicationInfo.ModuleId;
                mainModel.ApprovalProcessId = applicationInfo.ApprovalProcessId;
                mainModel.ApplicationId = applicationInfo.ApplicationId;
                mainModel.IsOnlineApplication = applicationInfo.IsOnlineApplication;
                mainModel.ApprovalStepId = applicationInfo.ApprovalStepId;
                mainModel.ApproverId = applicationInfo.ApproverId;
                mainModel.ApprovalStatusId = applicationInfo.ApprovalStatusId;
                mainModel.ApproverComments = applicationInfo.ApproverComments;

                mainModel.Id = applicationInfo.Id;
                mainModel.IUser = applicationInfo.IUser;
                mainModel.IDate = applicationInfo.IDate;
                mainModel.NextStepName = anApproval.StepName;

            }
            var applicationModel = new ViewApplicationViewModel();
            var application = _wpfCommonService.WFMUnit.OnlineApplicationInfoRepository.GetByID(applicationInfo.ApplicationId);

            if (application != null)
            {
                var applicantInfo = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(q => q.Id == application.EmployeeId).FirstOrDefault();
                applicationModel.ApplicationDate = application.ApplicationDate;
                applicationModel.ApplicationFor = application.WFM_WelfareFundCategory.Name;
                applicationModel.ApplicationTo = application.AppTo;
                applicationModel.ApplicationSubject = application.Subject;
                applicationModel.ApplicationBody = application.Body;
                applicationModel.ApplicantEmployeeId = applicantInfo.EmpID;
                applicationModel.ApplicantName = applicantInfo.FullName;
                applicationModel.ApplicantDepartment = applicantInfo.PRM_Discipline == null ? string.Empty : applicantInfo.PRM_Discipline.Name;
                applicationModel.ApplicantDesignation = applicantInfo.PRM_Designation.Name;
                applicationModel.AppliedAmount = application.AppliedAmount;
                mainModel.SuggestAmount = application.SuggestAmount;
            }

            var approvalHistoryList = new List<ApprovalHistoryViewModel>();
            var approvalHistory = _prmCommonservice.PRMUnit.ApprovalHistoryRepository.Get(q => q.ApplicationId == mainModel.ApplicationId && q.ApprovalProcesssId == mainModel.ApprovalProcessId).DefaultIfEmpty().OfType<vwApvApplicationWiseApprovalStatu>().ToList();
            foreach (var item in approvalHistory)
            {
                approvalHistoryList.Add(new ApprovalHistoryViewModel
                {
                    ApprovalStepName = item.StepSequence,
                    ApproverComment = item.ApproverComments == null ? string.Empty : item.ApproverComments,
                    ApprovalStatus = item.ApprovalStatusName,
                    ApproverIdAndName = item.ApproverIdAndName,
                    ApproveDate = item.ApproverDate.ToString(DateAndTime.GlobalDateFormat)
                });
            }

            mainModel.ApprovalHistory = approvalHistoryList;

            var nextAproverList = _prmCommonservice.PRMUnit.FunctionRepository.GetNextApprover(applicationInfo.ApprovalProcessId, application.Id);
            mainModel.NextApproverList = Common.PopulateEmployeeDDL(nextAproverList);

            #region Attachment
            List<WFMAttachmentViewModel> aList = (from ada in _wpfCommonService.WFMUnit.OnlineApplicationInfoDetailAttachmentRepository.GetAll()
                                                  where (ada.OnlineApplicationInfoId == applicationInfo.ApplicationId)
                                                  select new WFMAttachmentViewModel()
                                                  {
                                                      Title = ada.Title,
                                                      FileName = ada.FileName,
                                                      Attachment = ada.Attachment
                                                  }).ToList();

            var aNewlist = new List<WFMAttachmentViewModel>();
            foreach (var item in aList)
            {
                byte[] document = item.Attachment;
                if (document != null)
                {
                    string strFilename = Url.Content("~/Content/" + User.Identity.Name + item.FileName);
                    byte[] doc = document;
                    WriteToFile(Server.MapPath(strFilename), ref doc);

                    item.FilePath = strFilename;
                }
                aNewlist.Add(item);
            }
            mainModel.AttachmentFilesList = aNewlist;

            #endregion

            mainModel.Application = applicationModel;
            return mainModel;
        }
        [NoCache]
        private RequestedApplicationViewModel GetOfflineApplicationInformation(RequestedApplicationViewModel mainModel, int requestedApplicationId, APV_ApplicationInformation applicationInfo)
        {
            if (applicationInfo != null)
            {
                var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);

                DateTime startDate = mainModel.StartDate;
                DateTime endDate = mainModel.EndDate;
                int statusId = Convert.ToInt32(mainModel.ApprovalStatusId);
                int approvalProcessId = Convert.ToInt32(mainModel.ApprovalProcessId);

                var approvalInfo = _prmCommonservice.PRMUnit.FunctionRepository.GetWelfareFundApplications(loginUser.EmpId, approvalProcessId, startDate, endDate, statusId).DefaultIfEmpty().OfType<APV_GetWelfareFundApplication_Result>().ToList();
                var anApproval = approvalInfo.Where(q => q.Id == requestedApplicationId).FirstOrDefault();

                mainModel.ModuleId = applicationInfo.ModuleId;
                mainModel.ApprovalProcessId = applicationInfo.ApprovalProcessId;
                mainModel.ApplicationId = applicationInfo.ApplicationId;
                mainModel.IsOnlineApplication = applicationInfo.IsOnlineApplication;
                mainModel.ApprovalStepId = applicationInfo.ApprovalStepId;
                mainModel.ApproverId = applicationInfo.ApproverId;
                mainModel.ApprovalStatusId = applicationInfo.ApprovalStatusId;
                mainModel.ApproverComments = applicationInfo.ApproverComments;

                mainModel.Id = applicationInfo.Id;
                mainModel.IUser = applicationInfo.IUser;
                mainModel.IDate = applicationInfo.IDate;
                mainModel.NextStepName = anApproval.StepName;

            }
            var applicationModel = new ViewApplicationViewModel();
            var application = _wpfCommonService.WFMUnit.OfflineApplicationInfoRepository.GetByID(applicationInfo.ApplicationId);

            if (application != null)
            {
                var applicantInfo = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(q => q.Id == application.EmployeeId).FirstOrDefault();
                applicationModel.ApplicationDate = application.ApplicationDate;
                applicationModel.ApplicationFor = application.WFM_WelfareFundCategory.Name;
                applicationModel.ApplicationTo = application.AppTo;
                applicationModel.ApplicationSubject = application.Subject;
                applicationModel.ApplicationBody = application.Body;
                applicationModel.ApplicantEmployeeId = applicantInfo.EmpID;
                applicationModel.ApplicantName = applicantInfo.FullName;
                applicationModel.ApplicantDepartment = applicantInfo.PRM_Discipline == null ? string.Empty : applicantInfo.PRM_Discipline.Name;
                applicationModel.ApplicantDesignation = applicantInfo.PRM_Designation.Name;
                applicationModel.AppliedAmount = application.AppliedAmount;
                mainModel.SuggestAmount = application.SuggestAmount;

            }

            var approvalHistoryList = new List<ApprovalHistoryViewModel>();
            var approvalHistory = _prmCommonservice.PRMUnit.ApprovalHistoryRepository.Get(q => q.ApplicationId == mainModel.ApplicationId && q.ApprovalProcesssId == mainModel.ApprovalProcessId).DefaultIfEmpty().OfType<vwApvApplicationWiseApprovalStatu>().ToList();
            foreach (var item in approvalHistory)
            {
                approvalHistoryList.Add(new ApprovalHistoryViewModel
                {
                    ApprovalStepName = item.StepSequence,
                    ApproverComment = item.ApproverComments == null ? string.Empty : item.ApproverComments,
                    ApprovalStatus = item.ApprovalStatusName,
                    ApproverIdAndName = item.ApproverIdAndName,
                    ApproveDate = item.ApproverDate.ToString(DateAndTime.GlobalDateFormat)

                });
            }

            mainModel.ApprovalHistory = approvalHistoryList;

            var nextAproverList = _prmCommonservice.PRMUnit.FunctionRepository.GetNextApprover(applicationInfo.ApprovalProcessId, application.Id);
            mainModel.NextApproverList = Common.PopulateEmployeeDDL(nextAproverList);

            #region Attachment
            List<WFMAttachmentViewModel> aList = (from ada in _wpfCommonService.WFMUnit.OfflineApplicationInfoDetailAttachmentRepository.GetAll()
                                                  where (ada.OfflineApplicationInfoId == applicationInfo.ApplicationId)
                                                  select new WFMAttachmentViewModel()
                                                    {
                                                        Title = ada.Title,
                                                        FileName = ada.FileName,
                                                        Attachment = ada.Attachment
                                                    }).ToList();

            var aNewlist = new List<WFMAttachmentViewModel>();
            foreach (var item in aList)
            {
                byte[] document = item.Attachment;
                if (document != null)
                {
                    string strFilename = Url.Content("~/Content/" + User.Identity.Name + item.FileName);
                    byte[] doc = document;
                    WriteToFile(Server.MapPath(strFilename), ref doc);

                    item.FilePath = strFilename;
                }
                aNewlist.Add(item);
            }
            mainModel.AttachmentFilesList = aNewlist;

            #endregion

            mainModel.Application = applicationModel;
            return mainModel;
        }
        [NoCache]
        public ActionResult ViewWFMApplicantHistory(int approvalProcessId, int requestedApplicationId, DateTime applicationDate, int statusId)
        {
            var model = new RequestedApplicationViewModel();
            var applicationInfo = _prmCommonservice.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApprovalProcessId == approvalProcessId && q.Id == requestedApplicationId).FirstOrDefault();
            dynamic application;
            if (applicationInfo.IsOnlineApplication == true)
            {
                application = _wpfCommonService.WFMUnit.OnlineApplicationInfoRepository.GetByID(applicationInfo.ApplicationId);
            }
            else
            {
                application = _wpfCommonService.WFMUnit.OfflineApplicationInfoRepository.GetByID(applicationInfo.ApplicationId);
            }

            List<RequestedApplicationViewModel> resultFrm = (from OnApp in _wpfCommonService.WFMUnit.OnlineApplicationInfoRepository.GetAll()
                                                             join WlfC in _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll() on OnApp.WelfareFundCategoryId equals WlfC.Id
                                                             join AS in _prmCommonservice.PRMUnit.ApprovalStatusRepository.GetAll() on OnApp.ApplicationStatusId equals AS.Id
                                                             join apvWF in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.GetAll() on OnApp.EmployeeId equals apvWF.EmployeeId into sr
                                                             from x in sr.DefaultIfEmpty()
                                                             where (OnApp.EmployeeId == application.EmployeeId && OnApp.Id != applicationInfo.ApplicationId)
                                                             select new RequestedApplicationViewModel()
                                                                                 {
                                                                                     WelfareFundCategoryName = WlfC.Name,
                                                                                     WelfareFundReason = OnApp.Reason,
                                                                                     WelfareFundApplicationDate = OnApp.ApplicationDate.ToString(DateAndTime.GlobalDateFormat),
                                                                                     WelfareFundAppliedAmount = OnApp.AppliedAmount.ToString(),
                                                                                     WelfareFundApprovedAmount = x == null ? string.Empty : x.ApprovedAmount.ToString(),
                                                                                     ApplicationStatus = AS.StatusName
                                                                                 }).Concat(from offApp in _wpfCommonService.WFMUnit.OfflineApplicationInfoRepository.GetAll()
                                                                                           join WlfC in _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll() on offApp.WelfareFundCategoryId equals WlfC.Id
                                                                                           join AS in _prmCommonservice.PRMUnit.ApprovalStatusRepository.GetAll() on offApp.ApplicationStatusId equals AS.Id
                                                                                           join apvWF in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.GetAll() on offApp.EmployeeId equals apvWF.EmployeeId into sr
                                                                                           from x in sr.DefaultIfEmpty()
                                                                                           where (offApp.EmployeeId == application.EmployeeId && offApp.Id != applicationInfo.ApplicationId)
                                                                                           select new RequestedApplicationViewModel()
                                                                                           {
                                                                                               WelfareFundCategoryName = WlfC.Name,
                                                                                               WelfareFundReason = offApp.Reason,
                                                                                               WelfareFundApplicationDate = offApp.ApplicationDate.ToString(DateAndTime.GlobalDateFormat),
                                                                                               WelfareFundAppliedAmount = offApp.AppliedAmount.ToString(),
                                                                                               WelfareFundApprovedAmount = x == null ? string.Empty : x.ApprovedAmount.ToString(),
                                                                                               ApplicationStatus = AS.StatusName
                                                                                           }).ToList();

            model.WRMApplicantHistoryList = resultFrm;
            return PartialView("_WFMApplicantHistory", model);
        }

        [NoCache]
        public ActionResult ViewINVItemBalance(int approvalProcessId, int requestedApplicationId, DateTime applicationDate, int statusId)
        {
            var model = new RequestedApplicationViewModel();
            model.ApplicationDate = model.EndDate = applicationDate;
            model.ApprovalStatusId = statusId;
            model.ApprovalProcessId = approvalProcessId;

            var applicationInfo = _prmCommonservice.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApprovalProcessId == approvalProcessId && q.Id == requestedApplicationId).FirstOrDefault();
            if (applicationInfo != null)
            {
                var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
                DateTime startDate = model.StartDate;
                DateTime endDate = model.EndDate;
                var approvalInfo = _prmCommonservice.PRMUnit.FunctionRepository.GetINVRequisitionApplications(approvalProcessId, loginUser.EmpId, statusId, startDate, endDate).DefaultIfEmpty().OfType<APV_GetINVRequisitionApplication_Result>().ToList();
                var anApproval = approvalInfo.Where(q => q.Id == requestedApplicationId).FirstOrDefault();

                model.ModuleId = applicationInfo.ModuleId;
                model.ApprovalProcessId = applicationInfo.ApprovalProcessId;
                model.ApplicationId = applicationInfo.ApplicationId;
                model.IsOnlineApplication = applicationInfo.IsOnlineApplication;
                model.ApprovalStepId = applicationInfo.ApprovalStepId;
                model.ApproverId = applicationInfo.ApproverId;
                model.ApprovalStatusId = applicationInfo.ApprovalStatusId;
                model.ApproverComments = applicationInfo.ApproverComments;

                model.Id = applicationInfo.Id;
                model.IUser = applicationInfo.IUser;
                model.IDate = applicationInfo.IDate;
                model.NextStepName = anApproval.StepName;

            }

            var applicationModel = new ViewApplicationViewModel();
            var application = _invCommonService.INVUnit.RequisitionInfoRepository.GetByID(applicationInfo.ApplicationId);

            if (application != null)
            {
                var applicantInfo = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(q => q.Id == application.IssuedToEmpId).FirstOrDefault();
                applicationModel.ApplicationDate = application.IndentDate;
                applicationModel.ApplicantEmployeeId = applicantInfo.EmpID;
                applicationModel.ApplicantName = applicantInfo.FullName;
                applicationModel.ApplicantDepartment = applicantInfo.PRM_Discipline.Name;
                applicationModel.ApplicantDesignation = applicantInfo.PRM_Designation.Name;
                applicationModel.AppliedAmount = application.TotalQuantity.Value;
                applicationModel.ApplicationNo = application.IndentNo;

                var applicationZoneId = application.ZoneInfoId;
                var INVReqDtlList = new List<INVRequisitionDtlViewModel>();
                var INVReqDtl = _invCommonService.INVUnit.RequisitionItemRepository.GetAll().Where(x => x.RequisitionId == applicationInfo.ApplicationId).ToList();

                //new code
                var _invContext = new BEPZA_MEDICAL.DAL.INV.ERP_BEPZAINVEntities();
                
                //end

                foreach (var item in INVReqDtl)
                {
                    decimal approvedQuantity = 0;
                    if (System.DBNull.Value.Equals(item.ApproveQuantity))
                    {
                        approvedQuantity = item.Quantity;
                    }
                    if (!System.DBNull.Value.Equals(item.ApproveQuantity))
                    {
                        approvedQuantity = Convert.ToDecimal(item.ApproveQuantity);
                    }

                    var balanceInfo = _invContext.fn_INV_GetItemClosingBalance(DateTime.Now, item.ItemId, applicationZoneId.ToString()).FirstOrDefault();
                    INVReqDtlList.Add(new INVRequisitionDtlViewModel
                    {
                        Id = item.Id,
                        ItemId = item.ItemId,
                        RequisitionId = item.RequisitionId,
                        Quantity = item.Quantity,
                        ItemName = item.INV_ItemInfo.ItemName,
                        Comment = item.Comment,
                        RecommendQuantity = approvedQuantity,
                        BalanceQuantity = balanceInfo.Balance
                    }
                   );
                }
                model.INVRequisitionDtlList = INVReqDtlList;
            }

            var approvalHistoryList = new List<ApprovalHistoryViewModel>();
            var approvalHistory = _prmCommonservice.PRMUnit.ApprovalHistoryRepository.Get(q => q.ApplicationId == model.ApplicationId && q.ApprovalProcesssId == model.ApprovalProcessId).DefaultIfEmpty().OfType<vwApvApplicationWiseApprovalStatu>().ToList();
            foreach (var item in approvalHistory)
            {
                approvalHistoryList.Add(new ApprovalHistoryViewModel
                {
                    ApprovalStepName = item.StepSequence,
                    ApproverComment = item.ApproverComments == null ? string.Empty : item.ApproverComments,
                    ApprovalStatus = item.ApprovalStatusName,
                    ApproverIdAndName = item.ApproverIdAndName,
                });
            }

            model.ApprovalHistory = approvalHistoryList;

            var nextAproverList = _prmCommonservice.PRMUnit.FunctionRepository.GetNextApprover(applicationInfo.ApprovalProcessId, application.Id, applicationInfo.ApprovalStepId);
            model.NextApproverList = Common.PopulateEmployeeDDL(nextAproverList);
            model.Application = applicationModel;
            
            return PartialView("_INVItemBalanceInfo", model); 
        }
        #region Attachment
        [NoCache]
        public void DownloadDoc(OnlineWelfareFundApplicationInformationViewModel model)
        {
            byte[] document = model.Attachment;
            if (document != null)
            {
                string strFilename = Url.Content("~/Content/" + User.Identity.Name + model.FileName);
                byte[] doc = document;
                WriteToFile(Server.MapPath(strFilename), ref doc);

                model.FilePath = strFilename;
            }
        }
        [NoCache]
        private void WriteToFile(string strPath, ref byte[] Buffer)
        {
            FileStream newFile = null;

            try
            {
                newFile = new FileStream(strPath, FileMode.Create);

                newFile.Write(Buffer, 0, Buffer.Length);
                newFile.Close();
            }
            catch (Exception ex)
            {
                if (newFile != null) newFile.Close();
            }
        }

        #endregion

    }
}