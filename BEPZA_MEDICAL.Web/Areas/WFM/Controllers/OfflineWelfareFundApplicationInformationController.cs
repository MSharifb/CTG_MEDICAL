using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.WFM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Domain.WFM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Approval;
using BEPZA_MEDICAL.Web.Areas.WFM.ViewModel;
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

namespace BEPZA_MEDICAL.Web.Areas.WFM.Controllers
{
    public class OfflineWelfareFundApplicationInformationController : BaseController
    {
        // GET: WFM/OfflineWelfareFundApplicationInformation
        #region Fields
        private readonly EmployeeService _empService;
        private readonly WFMCommonService _wfmCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public OfflineWelfareFundApplicationInformationController(EmployeeService empService, WFMCommonService wfmCommonService, PRMCommonSevice prmCommonService)
        {
            this._empService = empService;
            this._wfmCommonService = wfmCommonService;
            _prmCommonService = prmCommonService;
        }
        #endregion

        #region Action
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, OfflineWelfareFundApplicationInformationViewModel viewModel)
        {
            string filterExpression = String.Empty;
            var applicationSatusList = _prmCommonService.PRMUnit.ApprovalStatusRepository.GetAll();
            int totalRecords = 0;
            var list = (from onlApp in _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.GetAll()
                        join wfc in _wfmCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll() on onlApp.WelfareFundCategoryId equals wfc.Id
                        where (viewModel.WelfareFundCategoryId == onlApp.WelfareFundCategoryId || viewModel.WelfareFundCategoryId == 0)
                        select new OfflineWelfareFundApplicationInformationViewModel()
                        {
                            Id = onlApp.Id,
                            ApplicationDate = onlApp.ApplicationDate,
                            WelfareFundCategoryId = onlApp.WelfareFundCategoryId,
                            Reason = onlApp.Reason,
                            AppliedAmount = onlApp.AppliedAmount,
                            WelfareFundCategoryName = wfc.Name,
                            ApplicationFromDate = onlApp.ApplicationDate,
                            ApplicationToDate = onlApp.ApplicationDate,
                            ApplicationStatusName = applicationSatusList.Where(t => t.Id == onlApp.ApplicationStatusId).FirstOrDefault().StatusName

                        }).OrderBy(x => x.Name).ToList();

            if (request.Searching)
            {
                if ((viewModel.ApplicationFromDate != null && viewModel.ApplicationFromDate != DateTime.MinValue) && (viewModel.ApplicationToDate != null && viewModel.ApplicationToDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.ApplicationDate >= viewModel.ApplicationFromDate && d.ApplicationDate <= viewModel.ApplicationToDate).ToList();
                }
                else if ((viewModel.ApplicationFromDate != null && viewModel.ApplicationFromDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.ApplicationDate >= viewModel.ApplicationFromDate).ToList();
                }
                else if ((viewModel.ApplicationToDate != null && viewModel.ApplicationToDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.ApplicationDate <= viewModel.ApplicationToDate).ToList();
                }

            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "ApplicationDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ApplicationDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ApplicationDate).ToList();
                }
            }

            if (request.SortingName == "WelfareFundCategoryName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.WelfareFundCategoryName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.WelfareFundCategoryName).ToList();
                }
            }
            if (request.SortingName == "Reason")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Reason).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Reason).ToList();
                }
            }
            if (request.SortingName == "AppliedAmount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AppliedAmount).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AppliedAmount).ToList();
                }
            }

            #endregion

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
                    (Convert.ToDateTime(d.ApplicationDate)).ToString(DateAndTime.GlobalDateFormat),
                    d.WelfareFundCategoryId,
                    d.WelfareFundCategoryName,
                    d.Reason,
                    d.AppliedAmount,
                    d.ApplicationStatusName=="Submit"?"Submitted":d.ApplicationStatusName,
                    d.ApplicationFromDate,
                    d.ApplicationToDate
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }
        [NoCache]
        public ActionResult WelfareFundforView()
        {
            var itemList = Common.PopulateWelfareFundCategoryDDL(_wfmCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll().Where(q => q.IsActive == true).OrderBy(x => x.Name).ToList());
            return PartialView("Select", itemList);
        }

        public ActionResult Create()
        {
            OfflineWelfareFundApplicationInformationViewModel model = new OfflineWelfareFundApplicationInformationViewModel();
            model.strMode = "Create";
            model.ApplicantStatus = "Self";
            model.AppTo = "Secretary";
            model.Subject = "For financial support from BEPZA Welfare Fund.";
            model.Body = "With the honor to state that, @@ApplicantName @@ApplicantID @@Designation is the employee of EPZ apply for financial support from BEPZA Welfare Fund. He/She provide all Attachment of his/her @@Reason. He/She spend  @@AppliedAmount TK for @@WelfareFundCategory.";

            #region Application No

            var preFix = "OF";
            var Number = 1;
            var appNo = string.Empty;

            var applicationNo = _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.GetAll().LastOrDefault(); ;
            if (applicationNo != null)
            {
                Number = Convert.ToInt32(applicationNo.ApplicationNo.Substring(2, 6)) + 1;
            }
            appNo = Number.ToString();
            if (appNo.Length < 7)
            {
                appNo = appNo.PadLeft(6, '0');
            }

            model.ApplicationNo = string.Concat(preFix, appNo);
            #endregion

            #region Approval Flow Info
            var apvInfo = _prmCommonService.PRMUnit.ApprovalFlowConfigurationRepository.GetAll().Where(x => x.APV_ApprovalProcess.ProcessNameEnum.Contains("WFM")).FirstOrDefault();
            if (apvInfo != null)
            {
                model.IsConfigurableApprovalFlow = apvInfo.IsConfigurableApprovalFlow;
            }
            #endregion

            BindApprovalHistory(model);

            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        [NoCache]
        public ActionResult Create([Bind(Exclude = "Attachment")] OfflineWelfareFundApplicationInformationViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                model.IsError = 1;
                var attachment = Request.Files["attachment"];
                errorList = BusinessLogicValidation(model, model.Id);

                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    model.Body = model.Body.Replace("@@ApplicantName", model.Name);
                    model.Body = model.Body.Replace("@@ApplicantID", model.EmpID);
                    model.Body = model.Body.Replace("@@Designation", model.Designation);
                    model.Body = model.Body.Replace("@@Reason", model.Reason);
                    model.Body = model.Body.Replace("@@AppliedAmount", model.AppliedAmount.ToString());
                    model.Body = model.Body.Replace("@@WelfareFundCategory", model.WelfareFundCategoryName);

                    SetApplicationCyclePeriod(model);
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, true);
                    if (entity.Id <= 0)
                    {
                        _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.Add(entity);
                        _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    else
                    {
                        if (errorList.Length == 0)
                        {
                            entity.EUser = User.Identity.Name;
                            entity.EDate = DateTime.Now;

                            _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.Update(entity);
                            _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.SaveChanges();
                            return RedirectToAction("Edit", new { id = model.Id, type = "success" });
                        }
                    }

                }
                else
                {
                    populateDropdown(model);
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = errorList;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                populateDropdown(model);
                model.IsError = 1;
                model.errClass = "failed";
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                return View(model);
            }
            return View(model);
        }

        public ActionResult Edit(int id, string type)
        {
            var AdInfoEntity = _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.GetByID(id);
            var parentModel = AdInfoEntity.ToModel();
            parentModel.strMode = "Edit";
            parentModel.IsInEditMode = true;

            #region Applicant Info
            var Emp = _empService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == parentModel.EmployeeId).FirstOrDefault();
            var loginUser = _empService.GetEmpLoginInfo(Emp.EmpID);
            parentModel.EmployeeId = loginUser.ID;
            parentModel.EmpID = loginUser.EmpId;
            parentModel.Name = loginUser.EmpName;
            parentModel.Department = loginUser.Department;
            parentModel.Designation = loginUser.DesignationName;
            parentModel.JoiningDate = loginUser.JoiningDate.ToString("yyyy-MM-dd");
            parentModel.ConfirmationDate = (Convert.ToDateTime(loginUser.ConfirmationDate)).ToString("yyyy-MM-dd");
            parentModel.ServiceDuration = CalculateServiceDuration(loginUser.JoiningDate, DateTime.Now);

            parentModel.SignatureAttachment = _prmCommonService.PRMUnit.EmployeePhotoGraphRepository.Get(x => x.IsPhoto == false && x.EmployeeId == parentModel.EmployeeId).Select(s => s.PhotoSignature).FirstOrDefault();
            #endregion

            #region Signatory
            var obj = _empService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == parentModel.SignatoryId).FirstOrDefault();

            parentModel.SignatoryEmpId = obj.EmpID;
            parentModel.SignatoryId = obj.Id;
            parentModel.SignatoryEmpName = obj.FullName;
            parentModel.SignatoryEmpDesignation = obj.PRM_Designation.Name;
            parentModel.SignatoryEmpPhone = obj.TelephoneOffice;
            parentModel.SignatoryEmpEmail = obj.EmialAddress;

            #endregion

            #region Attachment
            List<OfflineWelfareFundApplicationInformationViewModel> aList = (from ada in _wfmCommonService.WFMUnit.OfflineApplicationInfoDetailAttachmentRepository.GetAll()
                                                                             where (ada.OfflineApplicationInfoId == id)
                                                                             select new OfflineWelfareFundApplicationInformationViewModel()
                                                                             {
                                                                                 Id = ada.Id,
                                                                                 Title = ada.Title,
                                                                                 FileName = ada.FileName,
                                                                                 Attachment = ada.Attachment
                                                                             }).ToList();

            var aNewlist = new List<OfflineWelfareFundApplicationInformationViewModel>();
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
            parentModel.AttachmentFilesList = aNewlist;

            #endregion

            BindApprovalHistory(parentModel);

            populateDropdown(parentModel);
            var applicationStatus = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.Id == parentModel.ApplicationStatusId).FirstOrDefault();
            parentModel.ApplicationStatusName = applicationStatus != null ? applicationStatus.StatusName : "Draft";

            if (type == "success")
            {
                parentModel.IsError = 1;
                parentModel.errClass = "success";
                parentModel.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            return View("Create", parentModel);
        }

        #region Delete

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.GetByID(id);

            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(WFM_OfflineApplicationInfoDetailAttachment) };

                    _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.Delete(tempPeriod.Id, allTypes);
                    _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.SaveChanges();
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
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteDetail(int Id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _wfmCommonService.WFMUnit.OfflineApplicationInfoDetailAttachmentRepository.Delete(Id);
                _wfmCommonService.WFMUnit.OfflineApplicationInfoDetailAttachmentRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (UpdateException ex)
            {
                try
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        // if (ex.InnerException.Message.Contains("REFERENCE constraint"))
                        // "The user has related information and cannot be deleted."
                        ModelState.AddModelError("Error", errMsg);
                    }
                    else
                    {
                        ModelState.AddModelError("Error", errMsg);
                    }
                    result = false;
                }
                catch (Exception)
                {
                    result = false;
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
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });

        }
        #endregion

        [NoCache]
        public string BusinessLogicValidation(OfflineWelfareFundApplicationInformationViewModel model, int id)
        {
            var ApplicationNo = false;
            string errorMessage = string.Empty;
            if (id < 1)
            {
                ApplicationNo = _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.Get(q => q.ApplicationNo == model.ApplicationNo).Any();
                if (ApplicationNo)
                {
                    return errorMessage = "Duplicate Application No.";
                }
            }
            else
            {
                ApplicationNo = _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.Get(q => q.ApplicationNo == model.ApplicationNo && id != q.Id).Any();
                if (ApplicationNo)
                {
                    return errorMessage = "Duplicate Application No.";
                }
            }
            return errorMessage;
        }

        private WFM_OfflineApplicationInfo CreateEntity([Bind(Exclude = "Attachment")] OfflineWelfareFundApplicationInformationViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            if (model.strMode == "Edit")
            {
                pAddEdit = false;
            }

            #region Attachment
            if (model.AttachmentFilesList != null)
            {
                foreach (var c in model.AttachmentFilesList)
                {
                    var wfm_OfflineApplicationInfoDetailAttachment = new WFM_OfflineApplicationInfoDetailAttachment();

                    wfm_OfflineApplicationInfoDetailAttachment.FileName = c.FileName;
                    wfm_OfflineApplicationInfoDetailAttachment.Attachment = c.Attachment;
                    wfm_OfflineApplicationInfoDetailAttachment.Id = c.Id;
                    wfm_OfflineApplicationInfoDetailAttachment.Title = c.Title;
                    wfm_OfflineApplicationInfoDetailAttachment.IUser = User.Identity.Name;
                    wfm_OfflineApplicationInfoDetailAttachment.IDate = DateTime.Now;

                    if (pAddEdit)
                    {
                        wfm_OfflineApplicationInfoDetailAttachment.IUser = User.Identity.Name;
                        wfm_OfflineApplicationInfoDetailAttachment.IDate = DateTime.Now;
                        entity.WFM_OfflineApplicationInfoDetailAttachment.Add(wfm_OfflineApplicationInfoDetailAttachment);
                    }
                    else
                    {
                        wfm_OfflineApplicationInfoDetailAttachment.OfflineApplicationInfoId = model.Id;
                        wfm_OfflineApplicationInfoDetailAttachment.EUser = User.Identity.Name;
                        wfm_OfflineApplicationInfoDetailAttachment.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                            _wfmCommonService.WFMUnit.OfflineApplicationInfoDetailAttachmentRepository.Add(wfm_OfflineApplicationInfoDetailAttachment);
                        }
                        else
                        {
                            _wfmCommonService.WFMUnit.OfflineApplicationInfoDetailAttachmentRepository.Update(wfm_OfflineApplicationInfoDetailAttachment);

                        }
                    }
                    _wfmCommonService.WFMUnit.OfflineApplicationInfoDetailAttachmentRepository.SaveChanges();
                }
            }
            #endregion

            return entity;
        }

        [HttpPost]
        public ActionResult AddAttachemnt([Bind(Exclude = "Attachment")] OfflineWelfareFundApplicationInformationViewModel model)
        {
            HttpFileCollectionBase files = Request.Files;
            string name = string.Empty;
            byte[] fileData = null;

            foreach (string fileTagName in files)
            {

                // byte[] fileData = null;
                HttpPostedFileBase file = Request.Files[fileTagName];
                if (file.ContentLength > 0)
                {
                    // Due to the limit of the max for a int type, the largest file can be
                    // uploaded is 2147483647, which is very large anyway.
                    int size = file.ContentLength;
                    name = file.FileName;
                    int position = name.LastIndexOf("\\");
                    name = name.Substring(position + 1);
                    string contentType = file.ContentType;
                    fileData = new byte[size];
                    file.InputStream.Read(fileData, 0, size);
                }
            }

            List<OfflineWelfareFundApplicationInformationViewModel> list = new List<OfflineWelfareFundApplicationInformationViewModel>();

            var attList = Session["attachmentList"] as List<OfflineWelfareFundApplicationInformationViewModel>;

            var obj = new OfflineWelfareFundApplicationInformationViewModel
            {
                Title = model.Title,
                FileName = name,
                Attachment = fileData
            };
            list.Add(obj);
            model.AttachmentFilesList = list;
            attList = list;

            return PartialView("_DetailAtt", model);
        }

        private void BindApprovalHistory(OfflineWelfareFundApplicationInformationViewModel model)
        {
            if (model.Id > 0)
            {
                int approvalProcessId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "WFM").FirstOrDefault().Id;
                var history = _prmCommonService.PRMUnit.ApprovalHistoryRepository.Get(q => q.ApplicationId == model.Id && q.ApprovalProcesssId == approvalProcessId).DefaultIfEmpty().OfType<vwApvApplicationWiseApprovalStatu>().ToList();

                var approvalHistory = new List<ApprovalHistoryViewModel>();

                foreach (var item in history)
                {
                    var historyObj = new ApprovalHistoryViewModel
                    {
                        ApprovalStepName = item.StepSequence,
                        ApproverComment = item.ApproverComments == null ? string.Empty : item.ApproverComments,
                        ApprovalStatus = item.ApprovalStatusName,
                        ApproverIdAndName = item.ApproverIdAndName,
                    };
                    approvalHistory.Add(historyObj);
                }

                model.ApprovalHistory = approvalHistory;
            }
        }

        [NoCache]
        public JsonResult GetApplicantInfo(OfflineWelfareFundApplicationInformationViewModel model)
        {
            var obj = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
            var item = _prmCommonService.PRMUnit.EmployeePhotoGraphRepository.Get(x => x.IsPhoto == false && x.EmployeeId == obj.Id).Select(s => s.PhotoSignature).FirstOrDefault();
            var signature = string.Empty;
            if (item != null)
            {
                signature = Convert.ToBase64String(item);
            }
            return Json(new
            {
                EmpID = obj.EmpID,
                EmployeeName = obj.FullName,
                Designation = obj.PRM_Designation.Name,
                Department = obj.PRM_Division == null ? string.Empty : obj.PRM_Division.Name,
                Phone = obj.TelephoneOffice,
                Email = obj.EmialAddress,
                JoiningDate = obj.DateofJoining.ToString("yyyy-MM-dd"),
                ConfirmationDate = obj.DateofConfirmation == null ? string.Empty : (Convert.ToDateTime(obj.DateofConfirmation)).ToString("yyyy-MM-dd"),
                ServiceDuration = CalculateServiceDuration(obj.DateofJoining, DateTime.Now),
                Signature = signature
            });
        }


        [NoCache]
        public JsonResult LoadReason(int welfareFundId)
        {
            var list = _wfmCommonService.WFMUnit.ReasonOfWelfareCategoryRepository.Fetch().Where(t => t.WelfareFundCategoryId == welfareFundId).FirstOrDefault();
            var amount = _wfmCommonService.WFMUnit.WelfareFundPolicyRepository.Fetch().Where(t => t.WelfareFundCategoryId == welfareFundId).FirstOrDefault();

            return Json(new
            {
                Reason = list == null ? string.Empty : list.Reason,
                Amount = amount == null ? 0 : amount.MaxAmount
            });

        }

        [NoCache]
        public JsonResult GetSignatoryEmployeeInfo(string employeeId)
        {
            int empId = 0;
            int.TryParse(employeeId, out empId);
            string empIdStr = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(empId).EmpID;
            var employeeList = _prmCommonService.PRMUnit.FunctionRepository.GetApproverByApplicant(empIdStr, "WFM");
            var empInfoList = new List<EmployeeInfo>();
            foreach (var d in employeeList)
            {
                empInfoList.Add(new EmployeeInfo
                {
                    Id = (Int32)d.Id,
                    FullName = d.FullName,
                });
            }
            return Json(empInfoList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Method

        public void populateDropdown(OfflineWelfareFundApplicationInformationViewModel model)
        {
            dynamic ddlList;

            #region Welfare Fund Category ddl

            ddlList = _wfmCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll().Where(q => q.IsActive == true).OrderBy(x => x.Name).ToList();
            model.WelfareFundCategoryList = Common.PopulateWelfareFundCategoryDDL(ddlList);

            #endregion

            ddlList = _empService.PRMUnit.BankNameRepository.GetAll().OrderBy(x => x.Name).ToList();
            model.BankNameList = Common.PopulateDllList(ddlList);

            ddlList = _empService.PRMUnit.BankBranchRepository.GetAll().OrderBy(x => x.Name).ToList();
            model.BranchNameList = Common.PopulateDllList(ddlList);

            ddlList = _empService.PRMUnit.Relation.GetAll().OrderBy(x => x.Name).ToList();
            model.RelationList = Common.PopulateDllList(ddlList);

            var approvalStatusList = _prmCommonService.PRMUnit.ApprovalStatusRepository.GetAll();
            approvalStatusList = approvalStatusList.Where(q => q.StatusName == "Draft" || q.StatusName == "Submit").ToList();
            model.ApplicationStatusList = Common.PopulateDdlApprovalStatus(approvalStatusList);

            var approvalProcessInfo = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(t => t.ProcessNameEnum == "WFM").FirstOrDefault();
            var employeeList = _prmCommonService.PRMUnit.FunctionRepository.GetApproverByApplicant(model.EmpID, "WFM");
            model.SignatoryList = Common.PopulateEmployeeDDL(employeeList);
        }
        public string CalculateServiceDuration(DateTime startDate, DateTime endDate)
        {
            if (startDate.Date > endDate.Date)
            {
                throw new ArgumentException("startDate cannot be higher then endDate", "startDate");
            }

            int years = endDate.Year - startDate.Year;
            int months = 0;
            int days = 0;

            // Check if the last year, was a full year.
            if (endDate < startDate.AddYears(years) && years != 0)
            {
                years--;
            }

            // Calculate the number of months.
            startDate = startDate.AddYears(years);

            if (startDate.Year == endDate.Year)
            {
                months = endDate.Month - startDate.Month;
            }
            else
            {
                months = (12 - startDate.Month) + endDate.Month;
            }

            // Check if last month was a complete month.
            if (endDate < startDate.AddMonths(months) && months != 0)
            {
                months--;
            }

            // Calculate the number of days.
            startDate = startDate.AddMonths(months);

            days = (endDate - startDate).Days;

            return years.ToString() + " Year(s) " + months.ToString() + " Month(s) " + days.ToString() + " Day(s)";
        }

        private OfflineWelfareFundApplicationInformationViewModel SetApplicationCyclePeriod(OfflineWelfareFundApplicationInformationViewModel model)
        {
            var cycleList = _wfmCommonService.WFMUnit.CycleRepository.GetAll();
            var cycleModelList = new List<CycleViewModel>();
            foreach (var item in cycleList)
            {
                cycleModelList.Add(new CycleViewModel
                {
                    Id = item.Id,
                    FromMonth = item.FromMonth,
                    ToMonth = item.ToMonth,
                    StartMonthId = DateTime.ParseExact(item.FromMonth, "MMM", CultureInfo.InvariantCulture).Month,
                    EndMonthId = DateTime.ParseExact(item.ToMonth, "MMM", CultureInfo.InvariantCulture).Month,
                });
            }

            int applicationMonthId = model.ApplicationDate.Value.Month;
            var applicantCycle = cycleModelList.Where(q => q.StartMonthId <= applicationMonthId && q.EndMonthId >= applicationMonthId).FirstOrDefault();
            if (applicantCycle != null)
            {
                model.CycleId = applicantCycle.Id;
            }
            return model;
        }

        #endregion

        #region Attachment

        private int Upload(OfflineWelfareFundApplicationInformationViewModel model)
        {
            if (model.File == null)
                return 0;

            try
            {
                var uploadFile = model.File;

                byte[] data;
                using (Stream inputStream = uploadFile.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    data = memoryStream.ToArray();
                }
                model.Attachment = data;
                model.FileName = uploadFile.FileName;
                model.IsError = 0;

            }
            catch (Exception ex)
            {
                model.IsError = 1;
                model.ErrMsg = "Upload File Error!";
            }

            return model.IsError;
        }

        public void DownloadDoc(OfflineWelfareFundApplicationInformationViewModel model)
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

    public class EmployeeInfo
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string ApproverTypeName { get; set; }
    }


}