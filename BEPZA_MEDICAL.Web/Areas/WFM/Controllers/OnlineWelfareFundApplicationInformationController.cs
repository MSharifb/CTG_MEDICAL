using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.WFM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Domain.WFM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
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
    public class OnlineWelfareFundApplicationInformationController : BaseController
    {

        #region Fields
        private readonly EmployeeService _empService;
        private readonly WFMCommonService _wfmCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public OnlineWelfareFundApplicationInformationController(EmployeeService empService, WFMCommonService wfmCommonService, PRMCommonSevice prmCommonService)
        {
            _empService = empService;
            _wfmCommonService = wfmCommonService;
            _prmCommonService = prmCommonService;
        }
        #endregion

        #region Action

        // GET: WFM/OnlineWelfareFundApplicationInformation
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, OnlineWelfareFundApplicationInformationViewModel viewModel)
        {
            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);

            string filterExpression = String.Empty;
            var applicationSatusList = _prmCommonService.PRMUnit.ApprovalStatusRepository.GetAll();
            int totalRecords = 0;
            var list = (from onlApp in _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.GetAll()
                        join wfc in _wfmCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll() on onlApp.WelfareFundCategoryId equals wfc.Id
                        where (viewModel.WelfareFundCategoryId == onlApp.WelfareFundCategoryId || viewModel.WelfareFundCategoryId == 0) && onlApp.EmployeeId == loginUser.ID
                        select new OnlineWelfareFundApplicationInformationViewModel()
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

        #region Search Employee

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEmpList(JqGridRequest request, EmployeeSearchViewModel viewModel, string st)
        {

            string filterExpression = String.Empty, LoginEmpId = "";
            int totalRecords = 0;
            if (string.IsNullOrEmpty(st))
                st = "";
            if (viewModel.ZoneInfoId == 0 && request.Searching == false)
            {
                viewModel.ZoneInfoId = LoggedUserZoneInfoId;
            }

            var list = _empService.GetPaged(
                filterExpression.ToString(),
                request.SortingName,
                request.SortingOrder.ToString(),
                request.PageIndex,
                request.RecordsCount,
                request.PagesCount.HasValue ? request.PagesCount.Value : 1,

                viewModel.EmpId,
                viewModel.EmpName,
                viewModel.DesigName,
                viewModel.EmpTypeId,
                viewModel.DivisionName,
                viewModel.JobLocName,
                viewModel.GradeName,
                viewModel.StaffCategoryId,
                //viewModel.ResourceLevelId,
                viewModel.OrganogramLevelId,
                viewModel.ZoneInfoId,
                st.Equals("active") ? 1 : viewModel.EmployeeStatus,

                out totalRecords

                //LoginEmpId
                );

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.ID), new List<object>()
                {
                    item.ZoneInfoId,
                    item.EmpName,
                    item.ID,
                    item.EmpId,
                    item.DesigName,
                    item.DivisionName,
                    item.JobLocName,
                    item.GradeName,                     
                    item.DateOfJoining.ToString(DateAndTime.GlobalDateFormat),
                    item.DateOfConfirmation.HasValue ? item.DateOfConfirmation.Value.ToString(DateAndTime.GlobalDateFormat) : null,
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        #endregion

        public ActionResult Create()
        {
            OnlineWelfareFundApplicationInformationViewModel model = new OnlineWelfareFundApplicationInformationViewModel();

            #region Login Info
            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            if (loginUser != null)
            {
                model.EmployeeId = loginUser.ID;
                model.EmpID = loginUser.EmpId;
                model.Name = loginUser.EmpName;
                model.Department = loginUser.Department;
                model.Designation = loginUser.DesignationName;
                model.JoiningDate = loginUser.JoiningDate.ToString("yyyy-MM-dd");
                model.ConfirmationDate = loginUser.ConfirmationDate == null ? string.Empty : (Convert.ToDateTime(loginUser.ConfirmationDate)).ToString("yyyy-MM-dd");
                model.ServiceDuration = CalculateServiceDuration(loginUser.JoiningDate, DateTime.Now);

                model.SignatureAttachment = _prmCommonService.PRMUnit.EmployeePhotoGraphRepository.Get(x => x.IsPhoto == false && x.EmployeeId == model.EmployeeId).Select(s => s.PhotoSignature).FirstOrDefault();
            }
            else
            {
                return View("Index");
            }
            model.strMode = "Create";
            model.AppTo = "Secretary";
            model.Subject = "For financial support from BEPZA Welfare Fund.";
            model.Body = "With the honor to state that, @@ApplicantName @@ApplicantID @@Designation is the employee of EPZ apply for financial support from BEPZA Welfare Fund. He/She provide all Attachment of his/her @@Reason. He/She spend  @@AppliedAmount TK for @@WelfareFundCategory.";

            

            #endregion

            #region Application No

            var preFix = "ON";
            var Number = 1;
            var appNo = string.Empty;

            var applicationNo = _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.GetAll().LastOrDefault();
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
            var apvInfo = _prmCommonService.PRMUnit.ApprovalFlowConfigurationRepository.GetAll().Where(x=>x.APV_ApprovalProcess.ProcessNameEnum.Contains("WFM")).FirstOrDefault();
            if(apvInfo != null)
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
        public ActionResult Create([Bind(Exclude = "Attachment")] OnlineWelfareFundApplicationInformationViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                model.IsError = 1;
                var attachment = Request.Files["Attachment"];
                errorList = BusinessLogicValidation(model, model.Id);

                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    var catName = _wfmCommonService.WFMUnit.WelfareFundCategoryRepository.GetByID(model.WelfareFundCategoryId);

                    model.Body = model.Body.Replace("@@ApplicantName", model.Name);
                    model.Body = model.Body.Replace("@@ApplicantID", model.EmpID);
                    model.Body = model.Body.Replace("@@Designation", model.Designation);
                    model.Body = model.Body.Replace("@@Reason", model.Reason);
                    model.Body = model.Body.Replace("@@AppliedAmount", model.AppliedAmount.ToString());
                    model.Body = model.Body.Replace("@@WelfareFundCategory", catName.Name);

                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    SetApplicationCyclePeriod(model);
                    var entity = CreateEntity(model, true);
                    if (entity.Id <= 0)
                    {
                        _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.Add(entity);
                        _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        populateDropdown(model);
                    }
                    else
                    {
                        if (errorList.Length == 0)
                        {
                            entity.EUser = User.Identity.Name;
                            entity.EDate = DateTime.Now;

                            _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.Update(entity);
                            _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.SaveChanges();
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
            var AdInfoEntity = _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.GetByID(id);
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
            List<OnlineWelfareFundApplicationInformationViewModel> aList = (from ada in _wfmCommonService.WFMUnit.OnlineApplicationInfoDetailAttachmentRepository.GetAll()
                                                                            where (ada.OnlineApplicationInfoId == id)
                                                                            select new OnlineWelfareFundApplicationInformationViewModel()
                                                                            {
                                                                                Id = ada.Id,
                                                                                Title = ada.Title,
                                                                                FileName = ada.FileName,
                                                                                Attachment = ada.Attachment
                                                                            }).ToList();

            var aNewlist = new List<OnlineWelfareFundApplicationInformationViewModel>();
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

            #region Approval History

            BindApprovalHistory(parentModel);



            #endregion

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

        #endregion

        private void BindApprovalHistory(OnlineWelfareFundApplicationInformationViewModel model)
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

        #region Delete

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.GetByID(id);

            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(WFM_OnlineApplicationInfoDetailAttachment) };

                    _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.Delete(tempPeriod.Id, allTypes);
                    _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.SaveChanges();
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
                _wfmCommonService.WFMUnit.OnlineApplicationInfoDetailAttachmentRepository.Delete(Id);
                _wfmCommonService.WFMUnit.OnlineApplicationInfoDetailAttachmentRepository.SaveChanges();
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
        public string BusinessLogicValidation(OnlineWelfareFundApplicationInformationViewModel model, int id)
        {
            var exist = false;
            var ApplicationNo = false;
            string errorMessage = string.Empty;
            if (id < 1)
            {
                ApplicationNo = _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.Get(q => q.ApplicationNo == model.ApplicationNo).Any();

                if (ApplicationNo)
                {
                    return errorMessage = "Duplicate Application No.";
                }
            }
            else
            {
                ApplicationNo = _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.Get(q => q.ApplicationNo == model.ApplicationNo && id != q.Id).Any();

                if (ApplicationNo)
                {
                    return errorMessage = "Duplicate Application No.";
                }


            }
            return errorMessage;

        }

        private WFM_OnlineApplicationInfo CreateEntity([Bind(Exclude = "Attachment")] OnlineWelfareFundApplicationInformationViewModel model, bool pAddEdit)
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
                    var wfm_OnlineApplicationInfoDetailAttachment = new WFM_OnlineApplicationInfoDetailAttachment();

                    wfm_OnlineApplicationInfoDetailAttachment.FileName = c.FileName;
                    wfm_OnlineApplicationInfoDetailAttachment.Attachment = c.Attachment;
                    wfm_OnlineApplicationInfoDetailAttachment.Id = c.Id;
                    wfm_OnlineApplicationInfoDetailAttachment.Title = c.Title;
                    wfm_OnlineApplicationInfoDetailAttachment.IUser = User.Identity.Name;
                    wfm_OnlineApplicationInfoDetailAttachment.IDate = DateTime.Now;

                    if (pAddEdit)
                    {
                        wfm_OnlineApplicationInfoDetailAttachment.IUser = User.Identity.Name;
                        wfm_OnlineApplicationInfoDetailAttachment.IDate = DateTime.Now;
                        entity.WFM_OnlineApplicationInfoDetailAttachment.Add(wfm_OnlineApplicationInfoDetailAttachment);
                    }
                    else
                    {
                        wfm_OnlineApplicationInfoDetailAttachment.OnlineApplicationInfoId = model.Id;
                        wfm_OnlineApplicationInfoDetailAttachment.EUser = User.Identity.Name;
                        wfm_OnlineApplicationInfoDetailAttachment.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                            _wfmCommonService.WFMUnit.OnlineApplicationInfoDetailAttachmentRepository.Add(wfm_OnlineApplicationInfoDetailAttachment);
                        }
                        else
                        {
                            _wfmCommonService.WFMUnit.OnlineApplicationInfoDetailAttachmentRepository.Update(wfm_OnlineApplicationInfoDetailAttachment);

                        }
                    }
                    _wfmCommonService.WFMUnit.OnlineApplicationInfoDetailAttachmentRepository.SaveChanges();
                }
            }
            #endregion

            return entity;
        }

        [HttpPost]
        public ActionResult AddAttachemnt([Bind(Exclude = "Attachment")] OnlineWelfareFundApplicationInformationViewModel model)
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

            List<OnlineWelfareFundApplicationInformationViewModel> list = new List<OnlineWelfareFundApplicationInformationViewModel>();

            var attList = Session["attachmentList"] as List<OnlineWelfareFundApplicationInformationViewModel>;

            var obj = new OnlineWelfareFundApplicationInformationViewModel
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

        [NoCache]
        public JsonResult GetEmployeeInfo(int employeeId)
        {
            try
            {
                var obj = _empService.PRMUnit.EmploymentInfoRepository.GetByID(employeeId);

                return Json(new
                {
                    EmpID = obj.EmpID,
                    EmployeeName = obj.FullName,
                    Designation = obj.PRM_Designation.Name,
                    Phone = obj.TelephoneOffice,
                    Email = obj.EmialAddress
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }


        [NoCache]
        public JsonResult LoadAmount(int welfareFundId)
        {
            var amount = _wfmCommonService.WFMUnit.WelfareFundPolicyRepository.Fetch().Where(t => t.WelfareFundCategoryId == welfareFundId).FirstOrDefault();

            return Json(new
            {
                Amount = amount == null ? 0 : amount.MaxAmount
            });
        }

        public ActionResult LoadReasonDDList(int welfareFundId)
        {
            var list = _wfmCommonService.WFMUnit.ReasonOfWelfareCategoryRepository.Fetch().Where(t => t.WelfareFundCategoryId == welfareFundId).Select(x => new { Id = x.Reason, Name = x.Reason }).OrderBy(x => x.Name).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
        }



        #region Method

        public void populateDropdown(OnlineWelfareFundApplicationInformationViewModel model)
        {
            dynamic ddlList;

            #region Welfare Fund Category ddl

            ddlList = _wfmCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll().Where(q => q.IsActive == true).OrderBy(x => x.Name).ToList();
            model.WelfareFundCategoryList = Common.PopulateWelfareFundCategoryDDL(ddlList);

            var approvalStatusList = _prmCommonService.PRMUnit.ApprovalStatusRepository.GetAll();
            approvalStatusList = approvalStatusList.Where(q => q.StatusName == "Draft" || q.StatusName == "Submit").ToList();
            model.ApplicationStatusList = Common.PopulateDdlApprovalStatus(approvalStatusList);

            var employeeList = _prmCommonService.PRMUnit.FunctionRepository.GetApproverByApplicant(model.EmpID, "WFM");

            model.SignatoryList = Common.PopulateEmployeeDDL(employeeList);

            #endregion

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

        public ActionResult EmployeeSearch(string UseTypeEmpId)
        {
            var model = new EmployeeSearchViewModel();
            model.SearchEmpType = "active";
            model.UseTypeEmpId = UseTypeEmpId;
            return View(model);
        }

        private OnlineWelfareFundApplicationInformationViewModel SetApplicationCyclePeriod(OnlineWelfareFundApplicationInformationViewModel model)
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

        #region Grid Dropdown list
        [NoCache]
        public ActionResult GetZoneInfo()
        {
            var zoneList = _empService.PRMUnit.ZoneInfoRepository.GetAll();
            return PartialView("Select", Common.PopulateDdlZoneList(zoneList));
        }

        [NoCache]
        public ActionResult GetDesignation()
        {
            var designations = _empService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.SortingOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(designations));
        }

        [NoCache]
        public ActionResult GetDivision()
        {
            var divisions = _empService.PRMUnit.DivisionRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList();
            return PartialView("Select", Common.PopulateDllList(divisions));
        }

        [NoCache]
        public ActionResult GetJobLocation()
        {
            var jobLocations = _empService.PRMUnit.JobLocationRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(jobLocations));
        }

        [NoCache]
        public ActionResult GetGrade()
        {
            //var grades = _empService.PRMUnit.JobGradeRepository.GetAll().OrderBy(x => x.Id).ToList();
            //return PartialView("Select", Common.PopulateJobGradeDDL(grades));
            return PartialView("Select", Common.PopulateCurrentJobGradeDDL(_empService));
        }

        [NoCache]
        public ActionResult GetEmploymentType()
        {
            var grades = _empService.PRMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetStaffCategory()
        {
            var grades = _empService.PRMUnit.PRM_StaffCategoryRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetEmployeeStatus()
        {
            var empStatus = _empService.PRMUnit.EmploymentStatusRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(empStatus));
        }

        #endregion

        #endregion

        #region Attachment

        private int Upload(OnlineWelfareFundApplicationInformationViewModel model)
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

        [HttpPost]
        public ActionResult ViewApplicantInfo(int id, string type)
        {

            var model = new OfflineWelfareFundApplicationInformationViewModel();
            var resultFrm =  new OfflineWelfareFundApplicationInformationViewModel();
            var aList = new List<OfflineWelfareFundApplicationInformationViewModel>();

            if (type == "online")
            {
                resultFrm = (from OnApp in _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.GetAll()
                             join emp in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on OnApp.SignatoryId equals emp.Id
                             join applicant in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on OnApp.EmployeeId equals applicant.Id
                             join cate in _wfmCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll() on OnApp.WelfareFundCategoryId equals cate.Id
                             where (OnApp.Id == id)
                             select new OfflineWelfareFundApplicationInformationViewModel()
                             {
                                 Subject = OnApp.Subject,
                                 AppTo = OnApp.AppTo,
                                 Body = OnApp.Body,
                                 ApplicationDate = OnApp.ApplicationDate,
                                 AppliedAmount = OnApp.AppliedAmount,
                                 SignatoryEmpId = emp.EmpID,
                                 SignatoryEmpName = emp.FullName,
                                 SignatoryEmpDesignation = emp.PRM_Designation.Name,
                                 SignatoryEmpPhone = emp.TelephoneOffice,
                                 Reason = OnApp.Reason,
                                 WelfareFundCategoryName = cate.Name,
                                 ApplicantName = applicant.FullName,
                                 EmpID = applicant.EmpID,
                                 Designation = applicant.PRM_Designation.Name,
                                 EmployeeId = applicant.Id

                             }).FirstOrDefault();

                 aList = (from ada in _wfmCommonService.WFMUnit.OnlineApplicationInfoDetailAttachmentRepository.GetAll()
                            where (ada.OnlineApplicationInfoId == id)
                            select new OfflineWelfareFundApplicationInformationViewModel()
                            {
                                Title = ada.Title

                            }).ToList();

            }
            else
            {
                resultFrm = (from offApp in _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.GetAll()
                             join emp in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on offApp.SignatoryId equals emp.Id
                             join applicant in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on offApp.EmployeeId equals applicant.Id
                             join cate in _wfmCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll() on offApp.WelfareFundCategoryId equals cate.Id
                             where (offApp.Id == id)
                             select new OfflineWelfareFundApplicationInformationViewModel()
                             {
                                 Subject = offApp.Subject,
                                 AppTo = offApp.AppTo,
                                 Body = offApp.Body,
                                 ApplicationDate = offApp.ApplicationDate,
                                 AppliedAmount = offApp.AppliedAmount,
                                 SignatoryEmpId = emp.EmpID,
                                 SignatoryEmpName = emp.FullName,
                                 SignatoryEmpDesignation = emp.PRM_Designation.Name,
                                 SignatoryEmpPhone = emp.TelephoneOffice,
                                 Reason = offApp.Reason,
                                 WelfareFundCategoryName = cate.Name,
                                 ApplicantName = applicant.FullName,
                                 EmpID = applicant.EmpID,
                                 Designation = applicant.PRM_Designation.Name,
                                 EmployeeId = applicant.Id

                             }).FirstOrDefault();

                aList = (from ada in _wfmCommonService.WFMUnit.OfflineApplicationInfoDetailAttachmentRepository.GetAll()
                         where (ada.OfflineApplicationInfoId == id)
                         select new OfflineWelfareFundApplicationInformationViewModel()
                         {
                             Title = ada.Title

                         }).ToList();

            }
            model.ApplicationDate = resultFrm.ApplicationDate;
            model.SignatoryEmpId = resultFrm.SignatoryEmpId;
            model.SignatoryEmpName = resultFrm.SignatoryEmpName;
            model.SignatoryEmpPhone = resultFrm.SignatoryEmpPhone;
            model.SignatoryEmpDesignation = resultFrm.SignatoryEmpDesignation;
            model.AppTo = resultFrm.AppTo;
            model.Subject = resultFrm.Subject;
            model.Reason = resultFrm.Reason;
            model.WelfareFundCategoryName = resultFrm.WelfareFundCategoryName;
            model.EmployeeId = resultFrm.EmployeeId;
            model.AttachmentFilesList = aList;

            model.Body = resultFrm.Body.Replace("@@ApplicantName", resultFrm.ApplicantName);
            model.Body = model.Body.Replace("@@ApplicantID", resultFrm.EmpID);
            model.Body = model.Body.Replace("@@Designation", resultFrm.Designation);
            model.Body = model.Body.Replace("@@Reason", resultFrm.Reason);
            model.Body = model.Body.Replace("@@AppliedAmount", resultFrm.AppliedAmount.ToString());
            model.Body = model.Body.Replace("@@WelfareFundCategory", resultFrm.WelfareFundCategoryName);
            model.SignatureAttachment = _prmCommonService.PRMUnit.EmployeePhotoGraphRepository.Get(x => x.IsPhoto == false && x.EmployeeId == model.EmployeeId).Select(s => s.PhotoSignature).FirstOrDefault();

            return PartialView("_ApplicationView", model);
        }


    }
}