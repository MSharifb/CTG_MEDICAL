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
    public class JobRequisitionInfoApprovalController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly FAMCommonService _famCommonService;
        #endregion

        #region Constructor
        public JobRequisitionInfoApprovalController(PRMCommonSevice prmCommonService, FAMCommonService famCommonService)
        {
            this._prmCommonService = prmCommonService;
            this._famCommonService = famCommonService;
        }
        #endregion
        //
        // GET: /PRM/JobRequisitionInfoApproval/
        public ActionResult Index()
        {
            Session["ReqAddedList"] = null;
            Session["ReqList"] = null;
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, JobRequisitionInfoApprovalViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<JobRequisitionInfoApprovalViewModel> list = (from job in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.GetAll()
                                                             join jobDtl in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.GetAll() on job.Id equals jobDtl.JobRequisitionInfoApprovalId
                                                             join reqSum in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll() on job.JobRequisitionInfoSummaryId equals reqSum.Id
                                                             join finan in _famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll() on reqSum.FinancialYearId equals finan.Id
                                                             where (model.Id == 0 || model.Id == job.Id)
                                                             &&(model.JobRequisitionInfoSummaryId==0 || model.JobRequisitionInfoSummaryId==job.JobRequisitionInfoSummaryId)
                                                             &&(model.ReferenceDate==null|| Convert.ToDateTime(model.ReferenceDate)==reqSum.ReferenceDate)
                                                             &&(model.ApproveDate==null || model.ApproveDate==job.ApproveDate)
                                                             &&(job.PRM_JobRequisitionInfoSummary.ZoneInfoId==LoggedUserZoneInfoId)
                                                              select new JobRequisitionInfoApprovalViewModel()
                                                             {
                                                                 Id = job.Id,
                                                                 JobRequisitionInfoSummaryId=job.JobRequisitionInfoSummaryId,
                                                                 ReferenceNo=job.PRM_JobRequisitionInfoSummary.ReferenceNo,
                                                                 ReferenceDate=job.PRM_JobRequisitionInfoSummary.ReferenceDate.ToString(DateAndTime.GlobalDateFormat),
                                                                 ApproveDate=job.ApproveDate,
                                                                 FinancialYear = finan.FinancialYearName,
                                                                 Status=job.Status
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


            if (request.SortingName == "ApproveDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ApproveDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ApproveDate).ToList();
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
                  d.JobRequisitionInfoSummaryId,
                  d.ReferenceNo,
                  d.ReferenceDate,
                  ((DateTime)d.ApproveDate).ToString(DateAndTime.GlobalDateFormat),
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
            JobRequisitionInfoApprovalViewModel model = new JobRequisitionInfoApprovalViewModel();
            model = PrepareForLogin(model);
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create([Bind(Exclude = "Attachment")] JobRequisitionInfoApprovalViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                model.IsError = 1;
                var attachment = Request.Files["attachment"];

                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
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
                        _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.Add(entity);
                        _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    else
                    {
                        // Set preious attachment if exist

                        var obj = _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.GetByID(model.Id);
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
                            _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.Update(entity);
                            _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.SaveChanges();
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
            }
            return View(model);
        }

        public ActionResult Edit(int id, string type)
        {
            var JobRequisitionEntity = _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.GetByID(id);
            if (JobRequisitionEntity.Status == "Approved")
            {
                return RedirectToAction("Index");
            }
            var parentModel = JobRequisitionEntity.ToModel();
            DownloadDoc(parentModel);
            parentModel.strMode = "Edit";
            parentModel.IsInEditMode = true;
            parentModel = PrepareForLogin(parentModel);

            #region Prepared By
            List<JobRequisitionInfoApprovalViewModel> requisition = (from req in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll()
                                                                     join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on req.PreparedById equals emp.Id
                                                                     join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                                                                     join fin in _famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll() on req.FinancialYearId equals fin.Id
                                                                     where (req.Id == parentModel.JobRequisitionInfoSummaryId)
                                                                     select new JobRequisitionInfoApprovalViewModel()
                                                                     {
                                                                         ReferenceDate = req.ReferenceDate.ToString("yyyy-MM-dd"),
                                                                         PreparedBy = emp.FullName,
                                                                         Designation = des.Name,
                                                                         FinancialYear = fin.FinancialYearName,
                                                                         FinancialYearId = req.FinancialYearId
                                                                     }).ToList();

            foreach (var item in requisition)
            {
                parentModel.ReferenceDate = item.ReferenceDate;
                parentModel.PreparedBy = item.PreparedBy;
                parentModel.Designation = item.Designation;
                parentModel.FinancialYear = item.FinancialYear;
            }

            #endregion

            //Job Requisition Info Detail
            List<JobRequisitionInfoApprovalViewModel> list = (from job in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.GetAll()
                                                             join jobDtl in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.GetAll() on job.Id equals jobDtl.JobRequisitionInfoApprovalId
                                                             join jobSummDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on jobDtl.JobRequisitionInfoSummaryDtlId equals jobSummDtl.Id
                                                             join jobInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on jobSummDtl.RequisitionInfoDetailId equals jobInfoDtl.Id
                                                             where (jobDtl.JobRequisitionInfoApprovalId == id)
                                                              select new JobRequisitionInfoApprovalViewModel()
                                                             {
                                                                 Id = jobDtl.Id,
                                                                 DesignationId = jobInfoDtl.DesignationId,
                                                                 DepartmentName = jobInfoDtl.PRM_Division.Name,
                                                                 SectionName =jobInfoDtl.SectionId==null?string.Empty: jobInfoDtl.PRM_Section.Name,
                                                                 Designation = jobInfoDtl.PRM_Designation.Name,
                                                                 PayScale = jobInfoDtl.PRM_JobGrade.PayScale,
                                                                 NumberOfRequiredPost = jobInfoDtl.NumOfRequiredPost,
                                                                 RecommendPost = (int)jobSummDtl.NumOfRecommendedPost,
                                                                 RequisitionNo = jobInfoDtl.PRM_JobRequisitionInfo.RequisitionNo,
                                                                 ApprovedPost=jobDtl.ApprovedPost,
                                                                 RequisitionInfoSummaryDetailId=jobSummDtl.Id,
                                                                 IsCheckedFinal = true
                                                             }).ToList();
            parentModel.JobRequisitionApprovalList = list;

            //Job Requisition Info
            var infolist = (from job in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.GetAll()
                            join jobDtl in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.GetAll() on job.Id equals jobDtl.JobRequisitionInfoApprovalId
                            join jobSummDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on jobDtl.JobRequisitionInfoSummaryDtlId equals jobSummDtl.Id
                            join jobInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on jobSummDtl.RequisitionInfoDetailId equals jobInfoDtl.Id
                            join rec in _prmCommonService.PRMUnit.JobRequisitionInfoRepository.GetAll() on jobInfoDtl.RequisitionInfoId equals rec.Id
                            join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on rec.PreparedById equals emp.Id
                            join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                            where (jobDtl.JobRequisitionInfoApprovalId == id) && (rec.Id == jobInfoDtl.RequisitionInfoId)
                            select new JobRequisitionInfoApprovalDetailViewModel
                            {
                                RequisionId = rec.Id,
                                RequisitionSummaryId = jobSummDtl.JobRequisitionInfoSummaryId,
                                RequisitionNo = rec.RequisitionNo,
                                ReqPreparedBy = emp.FullName,
                                Designation = des.Name,
                                SubmissionDate = rec.RequisitionSubDate.ToString("yyyy-MM-dd"),
                                IsChecked = true,
                                strMode="Edit"
                            }
                        ).DistinctBy(x => x.RequisitionNo).ToList();

            parentModel.RequsitionApprovalDetailList = infolist;

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

            var tempPeriod = _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.GetByID(id);
            if (tempPeriod.Status=="Approved")
            {
                return Json(new
                {
                    Message = "Sorry! Requisition Already Approved."
                }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(PRM_JobRequisitionInfoApprovalDetail) };
                    _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.Delete(tempPeriod.Id, allTypes);
                    _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.SaveChanges();
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
                _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.Delete(Id);
                _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.SaveChanges();
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

        private PRM_JobRequisitionInfoApproval CreateEntity(JobRequisitionInfoApprovalViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            if (model.strMode == "Edit")
            {
                pAddEdit = false;
            }

            foreach (var c in model.JobRequisitionApprovalList)
            {
                var prm_JobRequisitionInfoApprovalDetail = new PRM_JobRequisitionInfoApprovalDetail();

                if (c.IsCheckedFinal)
                {
                    prm_JobRequisitionInfoApprovalDetail.Id = c.Id;
                    prm_JobRequisitionInfoApprovalDetail.JobRequisitionInfoSummaryDtlId = c.RequisitionInfoSummaryDetailId;
                    prm_JobRequisitionInfoApprovalDetail.ApprovedPost = c.ApprovedPost;
                    prm_JobRequisitionInfoApprovalDetail.IUser = User.Identity.Name;
                    prm_JobRequisitionInfoApprovalDetail.IDate = DateTime.Now;

                    if (pAddEdit)
                    {
                        prm_JobRequisitionInfoApprovalDetail.IUser = User.Identity.Name;
                        prm_JobRequisitionInfoApprovalDetail.IDate = DateTime.Now;
                        entity.PRM_JobRequisitionInfoApprovalDetail.Add(prm_JobRequisitionInfoApprovalDetail);
                    }
                    else
                    {
                        prm_JobRequisitionInfoApprovalDetail.JobRequisitionInfoApprovalId = model.Id;
                        prm_JobRequisitionInfoApprovalDetail.EUser = User.Identity.Name;
                        prm_JobRequisitionInfoApprovalDetail.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                            var requInfo = _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.GetAll().Where(x => x.JobRequisitionInfoSummaryDtlId == c.RequisitionInfoSummaryDetailId).ToList();
                            if (requInfo.Count == 0)
                            {
                                _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.Add(prm_JobRequisitionInfoApprovalDetail);
                            }
                        }
                        else
                        {
                            _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.Update(prm_JobRequisitionInfoApprovalDetail);

                        }
                        _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.SaveChanges();
                    }
                }
            }

            return entity;
        }

        public JobRequisitionInfoApprovalViewModel PrepareForLogin(JobRequisitionInfoApprovalViewModel model)
        {
            if (model.strMode == "Create")
            {
                List<JobRequisitionInfoApprovalViewModel> list = (from emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll()
                                                                 join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                                                                 where (emp.EmpID == model.IUser)
                                                                  select new JobRequisitionInfoApprovalViewModel()
                                                                 {
                                                                     ApprovedById = emp.Id,
                                                                     ApprovedByName = emp.FullName,
                                                                     ApprovedByDesignation = des.Name
                                                                 }).ToList();
                foreach (var item in list)
                {
                    model.ApprovedById = item.ApprovedById;
                    model.ApprovedByName = item.ApprovedByName;
                    model.ApprovedByDesignation = item.ApprovedByDesignation;
                }

            }
            else if (model.IsInEditMode == true)
            {
                List<JobRequisitionInfoApprovalViewModel> list = (from emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll()
                                                                 join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                                                                 where (emp.Id == model.ApprovedById)
                                                                  select new JobRequisitionInfoApprovalViewModel()
                                                                 {
                                                                     ApprovedById = emp.Id,
                                                                     ApprovedByName = emp.FullName,
                                                                     ApprovedByDesignation = des.Name
                                                                 }).ToList();
                foreach (var item in list)
                {
                    model.ApprovedById = item.ApprovedById;
                    model.ApprovedByName = item.ApprovedByName;
                    model.ApprovedByDesignation = item.ApprovedByDesignation;
                }

            }
            return model;
        }
        private void populateDropdown(JobRequisitionInfoApprovalViewModel model)
        {
            #region Reference No
            var ddlList = _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll().Where(x=>x.IsSubmit==true && x.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.ReferenceNo).ToList();

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
            model.ReferenceNoList = list;
            #endregion
        }

        [HttpGet]
        public PartialViewResult GetRequisitionInfo(int referenceNoId)   //for getting requisition info
        {
            var requisitionInfo = (from reqSum in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll()
                                 join reqSumDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on reqSum.Id equals reqSumDtl.JobRequisitionInfoSummaryId
                                 join reqInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on reqSumDtl.RequisitionInfoDetailId equals reqInfoDtl.Id
                                 join reqInfo in _prmCommonService.PRMUnit.JobRequisitionInfoRepository.GetAll() on reqInfoDtl.RequisitionInfoId equals reqInfo.Id
                                 join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on reqInfo.PreparedById equals emp.Id
                                 where (reqSum.Id == referenceNoId && reqSum.IsSubmit==true)
                                 select new JobRequisitionInfoApprovalDetailViewModel
                                 {
                                     RequisionId=reqInfo.Id,
                                     RequisitionSummaryId = reqSumDtl.Id,
                                     RequisitionNo=reqInfo.RequisitionNo,
                                     ReqPreparedBy = emp.FullName,
                                     Designation = emp.PRM_Designation.Name,
                                     SubmissionDate = reqInfo.RequisitionSubDate.ToString("yyyy-MM-dd")

                                 }).DistinctBy(x => x.RequisitionNo).ToList();

            return PartialView("_ReqList", new JobRequisitionInfoApprovalViewModel { RequsitionApprovalDetailList = requisitionInfo });

        }

        [HttpPost]
        public PartialViewResult AddedRequisitionInfo(List<JobRequisitionInfoApprovalDetailViewModel> RequisitionCodes,string ModeIs) //for getting requisition detail info
        {
            var model = new JobRequisitionInfoApprovalViewModel();

            List<JobRequisitionInfoApprovalViewModel> AssignmentList = new List<JobRequisitionInfoApprovalViewModel>();
            if (RequisitionCodes != null)
            {
                var list = (from jobReSumInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll()
                            join jobReInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on jobReSumInfoDtl.RequisitionInfoDetailId equals jobReInfoDtl.Id
                            select jobReSumInfoDtl).Where(x => RequisitionCodes.Select(n => n.RequisionId).Contains(x.PRM_JobRequisitionInfoDetail.RequisitionInfoId)).ToList();
                foreach (var vmEmp in list)
                {
                    var dupList = _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.GetAll().Where(x=>x.JobRequisitionInfoSummaryDtlId==vmEmp.Id).ToList();   // for checking duplicate

                    if (ModeIs == "Create")
                    {
                        if (dupList.Count == 0)
                        {
                            var gridModel = new JobRequisitionInfoApprovalViewModel
                            {
                                RequisitionInfoSummaryDetailId = vmEmp.Id,
                                DesignationId = vmEmp.PRM_JobRequisitionInfoDetail.DesignationId,
                                Designation = vmEmp.PRM_JobRequisitionInfoDetail.PRM_Designation.Name,
                                NumberOfRequiredPost = vmEmp.PRM_JobRequisitionInfoDetail.NumOfRequiredPost,
                                PayScale = vmEmp.PRM_JobRequisitionInfoDetail.PRM_JobGrade.PayScale,
                                DepartmentName = vmEmp.PRM_JobRequisitionInfoDetail.PRM_Division.Name,
                                SectionName =vmEmp.PRM_JobRequisitionInfoDetail.SectionId==null?string.Empty:vmEmp.PRM_JobRequisitionInfoDetail.PRM_Section.Name,
                                RecommendPost = vmEmp.NumOfRecommendedPost,
                                ApprovedPost = vmEmp.NumOfRecommendedPost,
                                RequisitionNo = vmEmp.PRM_JobRequisitionInfoDetail.PRM_JobRequisitionInfo.RequisitionNo
                            };
                            AssignmentList.Add(gridModel);
                        }
                    }
                    else
                    {
                        if (dupList.Count != 0)
                        {
                            var gridModel = new JobRequisitionInfoApprovalViewModel
                            {
                                RequisitionInfoSummaryDetailId = vmEmp.Id,
                                DesignationId = vmEmp.PRM_JobRequisitionInfoDetail.DesignationId,
                                Designation = vmEmp.PRM_JobRequisitionInfoDetail.PRM_Designation.Name,
                                NumberOfRequiredPost = vmEmp.PRM_JobRequisitionInfoDetail.NumOfRequiredPost,
                                PayScale = vmEmp.PRM_JobRequisitionInfoDetail.PRM_JobGrade.PayScale,
                                DepartmentName = vmEmp.PRM_JobRequisitionInfoDetail.PRM_Division.Name,
                                SectionName = vmEmp.PRM_JobRequisitionInfoDetail.SectionId == null ? string.Empty : vmEmp.PRM_JobRequisitionInfoDetail.PRM_Section.Name,
                                RecommendPost = vmEmp.NumOfRecommendedPost,
                                ApprovedPost = vmEmp.NumOfRecommendedPost,
                                RequisitionNo = vmEmp.PRM_JobRequisitionInfoDetail.PRM_JobRequisitionInfo.RequisitionNo,
                                IsCheckedFinal=true
                            };
                            AssignmentList.Add(gridModel);
                        }
                        else
                        {
                            var gridModel = new JobRequisitionInfoApprovalViewModel
                            {
                                RequisitionInfoSummaryDetailId = vmEmp.Id,
                                DesignationId = vmEmp.PRM_JobRequisitionInfoDetail.DesignationId,
                                Designation = vmEmp.PRM_JobRequisitionInfoDetail.PRM_Designation.Name,
                                NumberOfRequiredPost = vmEmp.PRM_JobRequisitionInfoDetail.NumOfRequiredPost,
                                PayScale = vmEmp.PRM_JobRequisitionInfoDetail.PRM_JobGrade.PayScale,
                                DepartmentName = vmEmp.PRM_JobRequisitionInfoDetail.PRM_Division.Name,
                                SectionName = vmEmp.PRM_JobRequisitionInfoDetail.SectionId == null ? string.Empty : vmEmp.PRM_JobRequisitionInfoDetail.PRM_Section.Name,
                                RecommendPost = vmEmp.NumOfRecommendedPost,
                                ApprovedPost = vmEmp.NumOfRecommendedPost,
                                RequisitionNo = vmEmp.PRM_JobRequisitionInfoDetail.PRM_JobRequisitionInfo.RequisitionNo,
                                IsCheckedFinal = false
                            };
                            AssignmentList.Add(gridModel);

                        }

                    }
                }
                model.JobRequisitionApprovalList = AssignmentList;
            }
            return PartialView("_Details", model);
        }


        [NoCache]
        public JsonResult SummaryOfRequisitionInfo(int referenceNoId)
        {
            List<JobRequisitionInfoApprovalViewModel> requisition = (from req in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll()
                                                                     join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on req.PreparedById equals emp.Id
                                                                     join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                                                                     join fin in _famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll() on req.FinancialYearId equals fin.Id
                                                                     where (req.Id == referenceNoId)
                                                                     select new JobRequisitionInfoApprovalViewModel()
                                                                     {
                                                                         ReferenceDate = req.ReferenceDate.ToString("yyyy-MM-dd"),
                                                                         PreparedBy=emp.FullName,
                                                                         Designation=des.Name,
                                                                         FinancialYear=fin.FinancialYearName,
                                                                         FinancialYearId=req.FinancialYearId
                                                                     }).ToList();
            var date = string.Empty;
            var name=string.Empty;
            var desig=string.Empty;
            var finan=string.Empty;
            var financialYearId = 0;
            foreach(var item in requisition)
            {
                date =item.ReferenceDate;
                name=item.PreparedBy;
                desig=item.Designation;
                finan=item.FinancialYear;
                financialYearId = item.FinancialYearId;
            }
            return Json(new
             {
                 RefDate = date,
                 Name = name,
                 Designation = desig,
                 FinancialYear=finan
             });
        }

        #region Attachment

        private int Upload(JobRequisitionInfoApprovalViewModel model)
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

        public void DownloadDoc(JobRequisitionInfoApprovalViewModel model)
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