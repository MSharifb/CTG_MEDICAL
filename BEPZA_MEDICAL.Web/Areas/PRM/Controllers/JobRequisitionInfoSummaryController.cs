using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.FAM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class JobRequisitionInfoSummaryController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly FAMCommonService _famCommonService;
        #endregion

        #region Constructor
        public JobRequisitionInfoSummaryController(PRMCommonSevice prmCommonService, FAMCommonService famCommonService)
        {
            this._prmCommonService = prmCommonService;
            this._famCommonService = famCommonService;
        }
        #endregion
        //
        // GET: /PRM/JobRequisitionInfoSummary/
        public ActionResult Index()
        {
            Session["ReqAddedList"] = null;
            Session["ReqList"] = null;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, JobRequisitionInfoSummaryViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<JobRequisitionInfoSummaryViewModel> list = (from job in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll()
                                                             join jobDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on job.Id equals jobDtl.JobRequisitionInfoSummaryId
                                                             join jobInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on jobDtl.RequisitionInfoDetailId equals jobInfoDtl.Id 
                                                             join fin in _famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll() on job.FinancialYearId equals fin.Id
                                                             join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on job.PreparedById equals emp.Id
                                                             join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                                                      where (model.Id == 0 || model.Id == job.Id)
                                                      && (model.FinancialYearId == 0 || model.FinancialYearId == job.FinancialYearId)
                                                      && (model.ReferenceDate == null || model.ReferenceDate == job.ReferenceDate)
                                                      && (string.IsNullOrEmpty(model.PreparedBy) || emp.FullName.Contains(model.PreparedBy))
                                                      &&(job.ZoneInfoId==LoggedUserZoneInfoId)
                                                      select new JobRequisitionInfoSummaryViewModel()
                                                      {
                                                          Id = job.Id,
                                                          ReferenceNo = job.ReferenceNo,
                                                          PreparedById = job.PreparedById,
                                                          PreparedBy = emp.FullName,
                                                          Designation = des.Name,
                                                          FinancialYearId = job.FinancialYearId,
                                                          FinancialYear = fin.FinancialYearName,
                                                          ReferenceDate = job.ReferenceDate,
                                                          Status = job.IsSubmit.ToString() == "True" ? "Submitted" : "Pending",
                                                      }).DistinctBy(x => x.ReferenceNo).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "ReferenceNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ReferenceNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ReferenceNo).ToList();
                }
            }


            if (request.SortingName == "PreparedBy")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PreparedBy).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PreparedBy).ToList();
                }
            }
            if (request.SortingName == "Designation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Designation).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Designation).ToList();
                }
            }
            if (request.SortingName == "FinancialYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FinancialYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FinancialYear).ToList();
                }
            }
            if (request.SortingName == "ReferenceDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ReferenceDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ReferenceDate).ToList();
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
                  d.ReferenceNo,
                 ((DateTime)d.ReferenceDate).ToString(DateAndTime.GlobalDateFormat),
                  d.FinancialYearId,
                  d.PreparedBy,
                  d.Designation,
                  d.FinancialYear,
                  d.Status,
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult ReferenceNoforView()
        {
            var ddlList = _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.ReferenceNo).ToList();

            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ReferenceNo,
                    Value = item.Id.ToString()
                });
            }

            list.OrderBy(x => x.Text.Trim()).ToList();

            return PartialView("Select", list);
        }
        [NoCache]
        public ActionResult FinancialYearforView()
        {
            var list = Common.PopulateFinancialYearDllList(_famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll().OrderBy(x => x.FinancialYearName).ToList());
            return PartialView("Select", list);
        }

        public ActionResult Create()
        {
            JobRequisitionInfoSummaryViewModel model = new JobRequisitionInfoSummaryViewModel();
            model = PrepareForLogin(model);
            model.ReferenceDate = DateTime.UtcNow;
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create([Bind(Exclude = "Attachment")] JobRequisitionInfoSummaryViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                model.IsError = 1;
                var attachment = Request.Files["attachment"];
                errorList = BusinessLogicValidation(model);
                

                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, true);
                    if (entity.Id <= 0)
                    {
                        HttpFileCollectionBase files = Request.Files;
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
                                entity.FileName = name;
                                entity.Attachment = fileData;
                            }

                        }
                        _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.Add(entity);
                        _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.SaveChanges();

                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    else
                    {
                        // Set preious attachment if exist

                        var obj = _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetByID(model.Id);
                        model.Attachment = obj.Attachment == null ? null : obj.Attachment;
                        //

                        //var entity = CreateEntity(model, false);

                        HttpFileCollectionBase files = Request.Files;
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
                                entity.FileName = name;
                                entity.Attachment = fileData;
                            }
                        }

                        if (errorList.Length == 0)
                        {
                            _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.Update(entity);
                            _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.SaveChanges();
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
            catch
            {
                model.IsError = 1;
                model.errClass = "failed";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                return View(model);
            }
            return View(model);
        }

        public ActionResult Edit(int id, string type)
        {
            var JobRequisitionEntity = _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetByID(id);
            if (JobRequisitionEntity.IsSubmit)
            {
                return RedirectToAction("Index");
            }
            var parentModel = JobRequisitionEntity.ToModel();
            DownloadDoc(parentModel);
            parentModel.strMode = "Edit";
            parentModel.IsInEditMode = true;
            parentModel = PrepareForLogin(parentModel);

            //Job Requisition Info Detail
            List<JobRequisitionInfoSummaryViewModel> list = (from job in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll()
                                                             join jobDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on job.Id equals jobDtl.JobRequisitionInfoSummaryId
                                                             join jobInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on jobDtl.RequisitionInfoDetailId equals jobInfoDtl.Id
                                                             where (jobDtl.JobRequisitionInfoSummaryId == id)
                                                             select new JobRequisitionInfoSummaryViewModel()
                                                             {
                                                                 Id = jobDtl.Id,
                                                                 DesignationId = jobInfoDtl.DesignationId,
                                                                 DepartmentName = jobInfoDtl.PRM_Division.Name,
                                                                 SectionName =jobInfoDtl.SectionId==null?string.Empty : jobInfoDtl.PRM_Section.Name,
                                                                 Designation = jobInfoDtl.PRM_Designation.Name,
                                                                // PayScale = jobInfoDtl.PRM_JobGrade.PayScale,
                                                                 PayScale = jobInfoDtl.PRM_JobGrade.PRM_GradeStep.First().StepAmount.ToString() + " - " + jobInfoDtl.PRM_JobGrade.PRM_GradeStep.Last().StepAmount.ToString(),
                                                                 NumberOfRequiredPost=jobInfoDtl.NumOfRequiredPost,
                                                                 RecommendPost=(int)jobDtl.NumOfRecommendedPost,
                                                                 RequisitionInfoDetailId = jobInfoDtl.Id,
                                                                 RequisitionNo=jobInfoDtl.PRM_JobRequisitionInfo.RequisitionNo,
                                                                 IsCheckedFinal=true
                                                             }).ToList();
            parentModel.JobRequisitionInfoSummary = list;

            //Job Requisition Info
            var infolist = (from job in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll()
                           join jobDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on job.Id equals jobDtl.JobRequisitionInfoSummaryId
                           join jobInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on jobDtl.RequisitionInfoDetailId equals jobInfoDtl.Id
                           join rec in _prmCommonService.PRMUnit.JobRequisitionInfoRepository.GetAll() on jobInfoDtl.RequisitionInfoId equals rec.Id
                           join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on rec.PreparedById equals emp.Id
                           join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                           where (jobDtl.JobRequisitionInfoSummaryId==id)&&(jobDtl.RequisitionInfoDetailId==jobInfoDtl.Id) && (rec.Id==jobInfoDtl.RequisitionInfoId) 
                        select new RequisitionInfoSummaryDetail
                        {
                            RequisionId = rec.Id,
                            RequisitionNo = rec.RequisitionNo,
                            ReqPreparedBy = emp.FullName,
                            Designation = des.Name,
                            SubmissionDate = rec.RequisitionSubDate.ToString("yyyy-MM-dd"),
                            IsChecked = true
                        }
                        ).DistinctBy(x=>x.RequisitionNo).ToList();

            parentModel.RequsitionInformationList = infolist;

            populateDropdown(parentModel);
            if (type == "success")
            {
                parentModel.IsError = 1;
                parentModel.errClass = "success";
                parentModel.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            return View("Create", parentModel);
        }
       

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetByID(id);
            if (tempPeriod.IsSubmit)
            {
                return Json(new
                {
                    Message ="Sorry! Requisition Already Submitted."
                }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(PRM_JobRequisitionInfoSummaryDetail)};
                    _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.Delete(tempPeriod.Id, allTypes);
                    _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.SaveChanges();
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
                _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.Delete(Id);
                _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.SaveChanges();
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

        private PRM_JobRequisitionInfoSummary CreateEntity(JobRequisitionInfoSummaryViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            if (model.strMode == "Edit")
            {
                pAddEdit = false;
            }

            foreach (var c in model.JobRequisitionInfoSummary)
            {
                var prm_JobRequisitionInfoSummaryDetail = new PRM_JobRequisitionInfoSummaryDetail();

                if (c.IsCheckedFinal)
                {
                    prm_JobRequisitionInfoSummaryDetail.Id = c.Id;
                    prm_JobRequisitionInfoSummaryDetail.RequisitionInfoDetailId = c.RequisitionInfoDetailId;
                    prm_JobRequisitionInfoSummaryDetail.NumOfRecommendedPost = c.RecommendPost;
                    prm_JobRequisitionInfoSummaryDetail.IUser = User.Identity.Name;
                    prm_JobRequisitionInfoSummaryDetail.IDate = DateTime.Now;

                    if (pAddEdit)
                    {
                        prm_JobRequisitionInfoSummaryDetail.IUser = User.Identity.Name;
                        prm_JobRequisitionInfoSummaryDetail.IDate = DateTime.Now;
                        entity.PRM_JobRequisitionInfoSummaryDetail.Add(prm_JobRequisitionInfoSummaryDetail);
                    }
                    else
                    {
                        prm_JobRequisitionInfoSummaryDetail.JobRequisitionInfoSummaryId = model.Id;
                        prm_JobRequisitionInfoSummaryDetail.EUser = User.Identity.Name;
                        prm_JobRequisitionInfoSummaryDetail.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                             var requInfo = _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll().Where(x => x.RequisitionInfoDetailId == c.RequisitionInfoDetailId).ToList();
                             if (requInfo.Count == 0)
                             {
                                 _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.Add(prm_JobRequisitionInfoSummaryDetail);
                             }
                        }
                        else
                        {
                            _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.Update(prm_JobRequisitionInfoSummaryDetail);

                        }
                        _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.SaveChanges();
                    }
                }
            }

            return entity;
        }

        [NoCache]
        public string BusinessLogicValidation(JobRequisitionInfoSummaryViewModel model)
        {
            string errorMessage = string.Empty;
            if (model.strMode != "Edit")
            {
                var requInfo = _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll().Where(x => x.ReferenceNo == model.ReferenceNo).ToList();
                if (requInfo.Count > 0)
                {
                    errorMessage = "Reference No. Already Exist";
                }
            }
            return errorMessage;

        }

        public JobRequisitionInfoSummaryViewModel PrepareForLogin(JobRequisitionInfoSummaryViewModel model)
        {
            if (model.strMode == "Create")
            {
                List<JobRequisitionInfoSummaryViewModel> list = (from emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll()
                                                                 join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                                                          where (emp.EmpID == model.IUser)
                                                                 select new JobRequisitionInfoSummaryViewModel()
                                                          {
                                                              PreparedById = emp.Id,
                                                              PreparedBy = emp.FullName,
                                                              Designation = des.Name
                                                          }).ToList();
                foreach (var item in list)
                {
                    model.PreparedById = item.PreparedById;
                    model.PreparedBy = item.PreparedBy;
                    model.Designation = item.Designation;
                }

            }
            else if (model.IsInEditMode == true)
            {
                List<JobRequisitionInfoSummaryViewModel> list = (from emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll()
                                                                 join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                                                          where (emp.Id == model.PreparedById)
                                                                 select new JobRequisitionInfoSummaryViewModel()
                                                          {
                                                              PreparedById = emp.Id,
                                                              PreparedBy = emp.FullName,
                                                              Designation = des.Name
                                                          }).ToList();
                foreach (var item in list)
                {
                    model.PreparedById = item.PreparedById;
                    model.PreparedBy = item.PreparedBy;
                    model.Designation = item.Designation;
                }

            }
            return model;
        }
        private void populateDropdown(JobRequisitionInfoSummaryViewModel model)
        {
            #region Financial Year
            var year = _famCommonService.FAMUnit.FinancialYearInformationRepository.Fetch().Where(p => p.IsActive == true).OrderBy(x => x.FinancialYearName).ToList();
            model.FinancialYearList = Common.PopulateFinancialYearDllList(year);
            #endregion

        }

        [HttpGet]
        public PartialViewResult GetRequisitionInfo(int financialId)    //Job Requisition Info
        {
            var list = (from rec in _prmCommonService.PRMUnit.JobRequisitionInfoRepository.GetAll()
                        join recDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on rec.Id equals  recDtl.RequisitionInfoId

                        join reqSumDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on recDtl.Id equals reqSumDtl.RequisitionInfoDetailId
                        into gReqSumDtl from subReqSumDtl in gReqSumDtl.DefaultIfEmpty()

                        join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on rec.PreparedById equals emp.Id
                        join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                        where (rec.FinancialYearId == financialId) && ((subReqSumDtl == null ? 0 : Convert.ToInt32(subReqSumDtl.RequisitionInfoDetailId)) !=recDtl.Id)
                        select new RequisitionInfoSummaryDetail
                        { 
                            RequisionId = rec.Id,
                            RequisitionNo=rec.RequisitionNo,
                            ReqPreparedBy=emp.FullName,
                            Designation=des.Name,
                            SubmissionDate = rec.RequisitionSubDate.ToString("yyyy-MM-dd")
                        }
                        ).DistinctBy(x=>x.RequisitionNo).ToList();

           

            var empSelected = Session["ReqAddedList"] as List<RequisitionInfoSummaryDetail>;
            if (empSelected != null)
            {
                foreach (var empVm in empSelected)
                {
                    var rData = list.Where(e => e.RequisionId == empVm.RequisionId).FirstOrDefault();
                    if (rData != null)
                        rData.IsChecked = true;
                }
            }
            var addedList = Session["ReqList"] as List<JobRequisitionInfoSummaryViewModel>;
            if (addedList != null)
            {
                foreach (var item in addedList)
                {
                    var obj = list.SingleOrDefault(s => s.RequisionId == item.RequisionId);
                    if (obj != null)
                        list.Remove(obj);
                }
            }
            return PartialView("_ReqList", new JobRequisitionInfoSummaryViewModel { RequsitionInformationList = list });
        }

        [HttpPost]
        public PartialViewResult AddedRequisitionInfo(List<RequisitionInfoSummaryDetail> RequisitionCodes, string ModeIs) //Job Requisition Info Details
        {
            Session["ReqAddedList"] = RequisitionCodes;
            var model = new JobRequisitionInfoSummaryViewModel();

            List<JobRequisitionInfoSummaryViewModel> AssignmentList = new List<JobRequisitionInfoSummaryViewModel>();
            if (RequisitionCodes != null)
            {
                var list = _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll().Where(x => RequisitionCodes.Select(n => n.RequisionId).Contains(x.RequisitionInfoId)).ToList();
                foreach (var vmEmp in list)
                {
                    var dupList = _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll().Where(x=>x.RequisitionInfoDetailId==vmEmp.Id).ToList();
                    if (ModeIs == "Create")
                    {
                        if (dupList.Count == 0)
                        {
                            var gridModel = new JobRequisitionInfoSummaryViewModel 
                            { 
                                RequisitionInfoDetailId = vmEmp.Id, 
                                DesignationId = vmEmp.DesignationId, 
                                Designation = vmEmp.PRM_Designation.Name, 
                                NumberOfRequiredPost = vmEmp.NumOfRequiredPost, 
                                //PayScale = vmEmp.PRM_JobGrade.PayScale, 
                                PayScale = vmEmp.PRM_JobGrade.PRM_GradeStep.First().StepAmount.ToString() + " - " + vmEmp.PRM_JobGrade.PRM_GradeStep.Last().StepAmount.ToString(),
                                DepartmentName = vmEmp.PRM_Division.Name, 
                                SectionName = vmEmp.SectionId == null ? string.Empty : vmEmp.PRM_Section.Name, 
                                RecommendPost = vmEmp.NumOfRequiredPost, 
                                RequisitionNo = vmEmp.PRM_JobRequisitionInfo.RequisitionNo 
                            };
                            gridModel.ReferenceNo = model.ReferenceNo;
                            AssignmentList.Add(gridModel);
                        }
                    }
                    else
                    {
                        if (dupList.Count != 0)
                        {
                            var gridModel = new JobRequisitionInfoSummaryViewModel 
                            { 
                                RequisitionInfoDetailId = vmEmp.Id,
                                DesignationId = vmEmp.DesignationId, 
                                Designation = vmEmp.PRM_Designation.Name, 
                                NumberOfRequiredPost = vmEmp.NumOfRequiredPost,
                                //PayScale = vmEmp.PRM_JobGrade.PayScale, 
                                PayScale = vmEmp.PRM_JobGrade.PRM_GradeStep.First().StepAmount.ToString() + " - " + vmEmp.PRM_JobGrade.PRM_GradeStep.Last().StepAmount.ToString(),
                                DepartmentName = vmEmp.PRM_Division.Name, 
                                SectionName = vmEmp.SectionId == null ? string.Empty : vmEmp.PRM_Section.Name,
                                RecommendPost = vmEmp.NumOfRequiredPost, 
                                RequisitionNo = vmEmp.PRM_JobRequisitionInfo.RequisitionNo, 
                                IsCheckedFinal = true 
                            };
                            gridModel.ReferenceNo = model.ReferenceNo;
                            AssignmentList.Add(gridModel);
                        }
                        else
                        {
                            var gridModel = new JobRequisitionInfoSummaryViewModel 
                            { 
                                RequisitionInfoDetailId = vmEmp.Id, 
                                DesignationId = vmEmp.DesignationId, 
                                Designation = vmEmp.PRM_Designation.Name, 
                                NumberOfRequiredPost = vmEmp.NumOfRequiredPost,
                                //PayScale = vmEmp.PRM_JobGrade.PayScale, 
                                PayScale = vmEmp.PRM_JobGrade.PRM_GradeStep.First().StepAmount.ToString() + " - " + vmEmp.PRM_JobGrade.PRM_GradeStep.Last().StepAmount.ToString(),
                                DepartmentName = vmEmp.PRM_Division.Name, 
                                SectionName = vmEmp.SectionId == null ? string.Empty : vmEmp.PRM_Section.Name, 
                                RecommendPost = vmEmp.NumOfRequiredPost, 
                                RequisitionNo = vmEmp.PRM_JobRequisitionInfo.RequisitionNo, 
                                IsCheckedFinal = false 
                            };
                            gridModel.ReferenceNo = model.ReferenceNo;
                            AssignmentList.Add(gridModel);
                        }
                    }
                }
                var addedList = Session["ReqList"] as List<JobRequisitionInfoSummaryViewModel>;
                if (addedList != null)
                {
                    foreach (var item in addedList)
                        AssignmentList.Add(item);
                }
                model.JobRequisitionInfoSummary = AssignmentList;
            }
            return PartialView("_Details", model);
        }

        [NoCache]
        public JsonResult GetRequsionInfo(int designationId, int requisitionId)
        {
            var orgLevelInfo = _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.Get(x => x.Id == requisitionId).Select(x => x.PRM_JobRequisitionInfo.OrganogramLevelId).FirstOrDefault();

            var directRec = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.DesignationId == designationId).Where(x => x.EmploymentProcessId == 1).ToList();
            var promotion = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.DesignationId == designationId).Where(x => x.EmploymentProcessId != 1).ToList();
            var directRecuri = directRec.Count();
            var Promoted = promotion.Count();

            var SanctionPost = _prmCommonService.PRMUnit.OrganizationalSetupManpowerInfoRepository.GetAll().Where(x => x.DesignationId == designationId && x.OrganogramLevelId == orgLevelInfo).FirstOrDefault();
            int post = SanctionPost == null ? 0 : SanctionPost.SanctionedPost;

            return Json(new
            {
                SancTotal = post,

                FillDirect = directRecuri,
                FillPro = Promoted,
                FillTotal = directRecuri + Promoted,

                VacantTotal = post - (directRecuri + Promoted)
            });
        }

        #region Attachment

        private int Upload(JobRequisitionInfoSummaryViewModel model)
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

        public void DownloadDoc(JobRequisitionInfoSummaryViewModel model)
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
}