using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Domain.WFM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Approval;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.RequestedApplication;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class AlternateApprovalController : Controller
    {

        #region Fields
        private readonly PRMCommonSevice _prmCommonservice;
        private readonly EmployeeService _empService;
        private readonly WFMCommonService _wpfCommonService;

        #endregion

        public AlternateApprovalController(PRMCommonSevice prmCommonService, EmployeeService empService, WFMCommonService wfmCommonService)
        {
            _prmCommonservice = prmCommonService;
            _empService = empService;
            _wpfCommonService = wfmCommonService;
        }
        public ActionResult Index(RequestedApplicationViewModel model)
        {
            var processTypeList = _prmCommonservice.PRMUnit.ApprovalProcessRepository.GetAll();
            model.AprovalProcessList = Common.PopulateDdlApprovalProcess(processTypeList);

            var statusList = _prmCommonservice.PRMUnit.ApprovalStatusRepository.GetAll().DefaultIfEmpty().OfType<APV_ApprovalStatus>().ToList();
            model.ApprovalStatusList = Common.PopulateDdlApprovalStatusForRecommenderApprover(statusList);

            model.StartDate = DateTime.Now;
            model.EndDate = DateTime.Now;

            return View("Index", model);
        }

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
                    var list = _prmCommonservice.PRMUnit.FunctionRepository.GetWelfareFundApplicationsForAlternateProcess(loginUser.EmpId, approvalProcessId, startDate, endDate, approvalStatusId).DefaultIfEmpty().OfType<APV_GetWelfareFundApplicationAlternateProcess_Result>().ToList();
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
                    break;
            }
            return PartialView(partialViewName, applicantList);
        }

        public JsonResult ExecuteApprovalAction(string applicationId, string actionName, string approverComment, string nextStepId, string nextApproverId)
        {
            int applicationIdInt = 0, nextApproverIdInt = 0, nextStepIdInt = 0;
            int.TryParse(applicationId, out applicationIdInt);
            int.TryParse(nextApproverId, out nextApproverIdInt);
            int.TryParse(nextStepId, out nextStepIdInt);
            string loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).EmpId;

            var result = _prmCommonservice.PRMUnit.FunctionRepository.ProceedToNextStepAlternateProcess(applicationIdInt, actionName, approverComment, nextStepIdInt, nextApproverIdInt, loginUser);
            string approvalResult = string.Empty;
            if (result != null)
            {
                approvalResult = result.ReturnMessage;
            }
            return Json(approvalResult, JsonRequestBehavior.AllowGet);
        }

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

        private RequestedApplicationViewModel GetOnlineApplicationInformation(RequestedApplicationViewModel mainModel, int requestedApplicationId, APV_ApplicationInformation applicationInfo)
        {
            if (applicationInfo != null)
            {
                var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
                DateTime startDate = mainModel.StartDate;
                DateTime endDate = mainModel.EndDate;
                int statusId = Convert.ToInt32(mainModel.ApprovalStatusId);
                int approvalProcessId = Convert.ToInt32(mainModel.ApprovalProcessId);
                //var approvalInfo = _prmCommonservice.PRMUnit.FunctionRepository.GetWelfareFundApplicationsForAlternateProcess(loginUser.EmpId, approvalProcessId, startDate, endDate, statusId).DefaultIfEmpty().OfType<APV_GetWelfareFundApplicationAlternateProcess_Result>().ToList();
                //var anApproval = approvalInfo.Where(q => q.Id == requestedApplicationId).FirstOrDefault();

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

                var anAppStp = _prmCommonservice.PRMUnit.ApprovalFlowDetailRepository.GetByID(mainModel.ApprovalStepId);

                var approvalSteps = (from steps in _prmCommonservice.PRMUnit.ApprovalFlowDetailRepository.GetAll()
                                     where steps.Id > mainModel.ApprovalStepId
                                     && steps.ApprovalFlowMasterId == anAppStp.ApprovalFlowMasterId
                                     select steps).DefaultIfEmpty().OfType<APV_ApprovalFlowDetail>().ToList();

                mainModel.ApprovalStepList = Common.PopulateDdlApprovalStep(approvalSteps);
                //mainModel.NextStepName = anApproval != null ? anApproval.StepName : string.Empty;

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
                applicationModel.ApplicantDepartment = applicantInfo.PRM_Discipline.Name;
                applicationModel.ApplicantDesignation = applicantInfo.PRM_Designation.Name;
                applicationModel.AppliedAmount = application.AppliedAmount;
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
                });
            }

            mainModel.ApprovalHistory = approvalHistoryList;

            var nextAproverList = _prmCommonservice.PRMUnit.FunctionRepository.GetNextApprover(applicationInfo.ApprovalProcessId, application.Id);
            mainModel.NextApproverList = Common.PopulateEmployeeDDL(nextAproverList);

            mainModel.Application = applicationModel;
            return mainModel;
        }

        public JsonResult GetNextApprover(int approvalStepId, int applicationId)
        {
            int approvalProcessId = _prmCommonservice.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "WFM").FirstOrDefault().Id;
            var nextAproverList = _prmCommonservice.PRMUnit.FunctionRepository.GetNextApprover(approvalProcessId, applicationId);
            return Json(nextAproverList, JsonRequestBehavior.AllowGet);
        }

        private RequestedApplicationViewModel GetOfflineApplicationInformation(RequestedApplicationViewModel mainModel, int requestedApplicationId, APV_ApplicationInformation applicationInfo)
        {
            if (applicationInfo != null)
            {
                var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);

                DateTime startDate = mainModel.StartDate;
                DateTime endDate = mainModel.EndDate;
                int statusId = Convert.ToInt32(mainModel.ApprovalStatusId);
                int approvalProcessId = Convert.ToInt32(mainModel.ApprovalProcessId);

                //var approvalInfo = _prmCommonservice.PRMUnit.FunctionRepository.GetWelfareFundApplicationsForAlternateProcess(loginUser.EmpId, approvalProcessId, startDate, endDate, statusId).DefaultIfEmpty().OfType<APV_GetWelfareFundApplication_Result>().ToList();
                //var anApproval = approvalInfo.Where(q => q.Id == requestedApplicationId).FirstOrDefault();

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
                //mainModel.NextStepName = anApproval.StepName;

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
                applicationModel.ApplicantDepartment = applicantInfo.PRM_Discipline.Name;
                applicationModel.ApplicantDesignation = applicantInfo.PRM_Designation.Name;
                applicationModel.AppliedAmount = application.AppliedAmount;
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
                });
            }

            mainModel.ApprovalHistory = approvalHistoryList;

            var nextAproverList = _prmCommonservice.PRMUnit.FunctionRepository.GetNextApprover(applicationInfo.ApprovalProcessId, application.Id);
            mainModel.NextApproverList = Common.PopulateEmployeeDDL(nextAproverList);

            mainModel.Application = applicationModel;
            return mainModel;
        }
    }
}