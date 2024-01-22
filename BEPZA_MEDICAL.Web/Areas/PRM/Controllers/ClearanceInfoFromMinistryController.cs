using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.FAM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
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
    public class ClearanceInfoFromMinistryController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly FAMCommonService _famCommonService;
        #endregion

        #region Constructor
        public ClearanceInfoFromMinistryController(PRMCommonSevice prmCommonService, FAMCommonService famCommonService)
        {
            this._prmCommonService = prmCommonService;
            this._famCommonService = famCommonService;
        }
        #endregion

        //
        // GET: /PRM/ClearanceInfoFromMinistry/
        public ActionResult Index()
        {
            Session["ReqAddedList"] = null;
            Session["ReqList"] = null;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ClearanceInfoFromMinistryViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ClearanceInfoFromMinistryViewModel> list = (from clrInfo in _prmCommonService.PRMUnit.ClearanceInfoFromMinistryRepository.GetAll()
                                                              join clrInfoDtl in _prmCommonService.PRMUnit.ClearanceInfoFromMinistryDetailRepository.GetAll() on clrInfo.Id equals clrInfoDtl.ClearanceInfoFromMinistryId
                                                              join reqSum in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll() on clrInfo.JobRequisitionInfoSummaryId equals reqSum.Id
                                                              join finan in _famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll() on reqSum.FinancialYearId equals finan.Id
                                                             where (model.JobRequisitionInfoSummaryId == 0 || model.JobRequisitionInfoSummaryId == clrInfo.JobRequisitionInfoSummaryId)
                                                             &&(model.ReferenceDate==null|| Convert.ToDateTime(model.ReferenceDate)==reqSum.ReferenceDate)
                                                             && (model.ApproveDate == null || model.ApproveDate == clrInfoDtl.PRM_JobRequisitionInfoApprovalDetail.PRM_JobRequisitionInfoApproval.ApproveDate)
                                                             &&(model.ClearanceDate==null || model.ClearanceDate==clrInfo.ClearanceDate)
                                                             select new ClearanceInfoFromMinistryViewModel()
                                                              {
                                                                  Id = clrInfo.Id,
                                                                  JobRequisitionInfoSummaryId = clrInfo.JobRequisitionInfoSummaryId,
                                                                  ReferenceNo = clrInfo.PRM_JobRequisitionInfoSummary.ReferenceNo,
                                                                  ReferenceDate = clrInfo.PRM_JobRequisitionInfoSummary.ReferenceDate.ToString(DateAndTime.GlobalDateFormat),
                                                                  ApproveDate=clrInfoDtl.PRM_JobRequisitionInfoApprovalDetail.PRM_JobRequisitionInfoApproval.ApproveDate,
                                                                  ClearanceDate = clrInfo.ClearanceDate,
                                                                  FinancialYear = finan.FinancialYearName,
                                                                  Status = clrInfo.Status
                                                              }).DistinctBy(x => x.ReferenceNo).ToList();

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
                  d.JobRequisitionInfoSummaryId,
                  d.ReferenceNo,
                  d.ReferenceDate,
                  ((DateTime)d.ApproveDate).ToString(DateAndTime.GlobalDateFormat),
                  ((DateTime)d.ClearanceDate).ToString(DateAndTime.GlobalDateFormat),
                  d.Status,
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }
        [NoCache]
        public ActionResult ReferenceNoforView()
        {
            var ddlList = _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll().OrderBy(x => x.ReferenceNo).ToList();

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
        public ActionResult Create()
        {
            ClearanceInfoFromMinistryViewModel model = new ClearanceInfoFromMinistryViewModel();
            populateDropdown(model);
            model.ClearanceDate = DateTime.UtcNow;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create([Bind(Exclude = "Attachment")] ClearanceInfoFromMinistryViewModel model)
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
                        _prmCommonService.PRMUnit.ClearanceInfoFromMinistryRepository.Add(entity);
                        _prmCommonService.PRMUnit.ClearanceInfoFromMinistryRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    else
                    {
                        // Set preious attachment if exist

                        var obj = _prmCommonService.PRMUnit.ClearanceInfoFromMinistryRepository.GetByID(model.Id);
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
                            _prmCommonService.PRMUnit.ClearanceInfoFromMinistryRepository.Update(entity);
                            _prmCommonService.PRMUnit.ClearanceInfoFromMinistryRepository.SaveChanges();
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
            var ClearanceFromMinistryEntity = _prmCommonService.PRMUnit.ClearanceInfoFromMinistryRepository.GetByID(id);
            if (ClearanceFromMinistryEntity.Status == "Approved")
            {
                return RedirectToAction("Index");
            }
            var parentModel = ClearanceFromMinistryEntity.ToModel();
            DownloadDoc(parentModel);
            parentModel.strMode = "Edit";
            parentModel.IsInEditMode = true;

            #region RequisitionInfo
            List<ClearanceInfoFromMinistryViewModel> requisition = (from req in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll()
                                                                    join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on req.PreparedById equals emp.Id
                                                                    join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                                                                    join fin in _famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll() on req.FinancialYearId equals fin.Id
                                                                    where (req.Id == parentModel.JobRequisitionInfoSummaryId)
                                                                    select new ClearanceInfoFromMinistryViewModel()
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
                parentModel.FinancialYearId = item.FinancialYearId;
            }

            #endregion

            //Job Requisition Info Detail
            List<ClearanceInfoFromMinistryViewModel> list = (from clrInfo in _prmCommonService.PRMUnit.ClearanceInfoFromMinistryRepository.GetAll()
                                                              join clrInfoDtl in _prmCommonService.PRMUnit.ClearanceInfoFromMinistryDetailRepository.GetAll() on clrInfo.Id equals clrInfoDtl.ClearanceInfoFromMinistryId
                                                              join jobDtl in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.GetAll() on clrInfoDtl.JobRequisitionInfoApprovalDetailId equals jobDtl.Id
                                                              join jobSummDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on jobDtl.JobRequisitionInfoSummaryDtlId equals jobSummDtl.Id
                                                              join jobInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on jobSummDtl.RequisitionInfoDetailId equals jobInfoDtl.Id
                                                             where (clrInfoDtl.ClearanceInfoFromMinistryId == id)
                                                             select new ClearanceInfoFromMinistryViewModel()
                                                              {
                                                                  Id = clrInfoDtl.Id,
                                                                  DesignationId = jobInfoDtl.DesignationId,
                                                                  DepartmentName = jobInfoDtl.PRM_Division.Name,
                                                                  SectionName =jobInfoDtl.SectionId==null?string.Empty : jobInfoDtl.PRM_Section.Name,
                                                                  Designation = jobInfoDtl.PRM_Designation.Name,
                                                                  PayScale = jobInfoDtl.PRM_JobGrade.PayScale,
                                                                  NumberOfRequiredPost = jobInfoDtl.NumOfRequiredPost,
                                                                  RecommendPost = (int)jobSummDtl.NumOfRecommendedPost,
                                                                  RequisitionNo = jobInfoDtl.PRM_JobRequisitionInfo.RequisitionNo,
                                                                  ApprovedPost = jobDtl.ApprovedPost,
                                                                  ClearancePost=clrInfoDtl.ClearancePost,
                                                                  RequisitionInfoApprovalDetailId = jobDtl.Id,
                                                                  IsCheckedFinal = true
                                                              }).ToList();
            parentModel.JobRequisitionClearanceList = list;

            //Job Requisition Info
            var infolist = (from clrInfo in _prmCommonService.PRMUnit.ClearanceInfoFromMinistryRepository.GetAll()
                            join clrInfoDtl in _prmCommonService.PRMUnit.ClearanceInfoFromMinistryDetailRepository.GetAll() on clrInfo.Id equals clrInfoDtl.ClearanceInfoFromMinistryId
                            join jobDtl in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.GetAll() on clrInfoDtl.JobRequisitionInfoApprovalDetailId equals jobDtl.Id
                            join jobSummDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on jobDtl.JobRequisitionInfoSummaryDtlId equals jobSummDtl.Id
                            join jobInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on jobSummDtl.RequisitionInfoDetailId equals jobInfoDtl.Id
                            join rec in _prmCommonService.PRMUnit.JobRequisitionInfoRepository.GetAll() on jobInfoDtl.RequisitionInfoId equals rec.Id
                            join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on rec.PreparedById equals emp.Id
                            join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                            where (clrInfoDtl.ClearanceInfoFromMinistryId == id) && (rec.Id == jobInfoDtl.RequisitionInfoId)
                            select new ClearanceInfoFromMinistryDetailViewModel
                            {
                                RequisionId = rec.Id,
                                RequisitionSummaryId = jobSummDtl.JobRequisitionInfoSummaryId,
                                RequisitionApproveId = jobDtl.JobRequisitionInfoApprovalId,
                                RequisitionNo = rec.RequisitionNo,
                                ReqPreparedBy = emp.FullName,
                                Designation = des.Name,
                                SubmissionDate = rec.RequisitionSubDate.ToString("yyyy-MM-dd"),
                                IsChecked = true,
                                strMode = "Edit"
                            }
                        ).DistinctBy(x => x.RequisitionNo).ToList();

            parentModel.RequsitionClearanceDetailList = infolist;

            populateDropdown(parentModel);

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

            var tempPeriod = _prmCommonService.PRMUnit.ClearanceInfoFromMinistryRepository.GetByID(id);
            if (tempPeriod.Status == "Approved")
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
                    List<Type> allTypes = new List<Type> { typeof(PRM_ClearanceInfoFromMinistryDetail) };
                    _prmCommonService.PRMUnit.ClearanceInfoFromMinistryRepository.Delete(tempPeriod.Id, allTypes);
                    _prmCommonService.PRMUnit.ClearanceInfoFromMinistryRepository.SaveChanges();
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
                _prmCommonService.PRMUnit.ClearanceInfoFromMinistryDetailRepository.Delete(Id);
                _prmCommonService.PRMUnit.ClearanceInfoFromMinistryDetailRepository.SaveChanges();
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

        private PRM_ClearanceInfoFromMinistry  CreateEntity(ClearanceInfoFromMinistryViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            if (model.strMode == "Edit")
            {
                pAddEdit = false;
            }

            foreach (var c in model.JobRequisitionClearanceList )
            {
                var prm_ClearanceInfoFromMinistryDetail = new PRM_ClearanceInfoFromMinistryDetail();

                if (c.IsCheckedFinal)
                {
                    prm_ClearanceInfoFromMinistryDetail.Id = c.Id;
                    prm_ClearanceInfoFromMinistryDetail.JobRequisitionInfoApprovalDetailId = c.RequisitionInfoApprovalDetailId;
                    prm_ClearanceInfoFromMinistryDetail.ClearancePost = c.ClearancePost;
                    prm_ClearanceInfoFromMinistryDetail.IUser = User.Identity.Name;
                    prm_ClearanceInfoFromMinistryDetail.IDate = DateTime.Now;

                    if (pAddEdit)
                    {
                        prm_ClearanceInfoFromMinistryDetail.IUser = User.Identity.Name;
                        prm_ClearanceInfoFromMinistryDetail.IDate = DateTime.Now;
                        entity.PRM_ClearanceInfoFromMinistryDetail.Add(prm_ClearanceInfoFromMinistryDetail);
                    }
                    else
                    {
                        prm_ClearanceInfoFromMinistryDetail.ClearanceInfoFromMinistryId = model.Id;
                        prm_ClearanceInfoFromMinistryDetail.EUser = User.Identity.Name;
                        prm_ClearanceInfoFromMinistryDetail.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                            var requInfo = _prmCommonService.PRMUnit.ClearanceInfoFromMinistryDetailRepository.GetAll().Where(x => x.JobRequisitionInfoApprovalDetailId == c.RequisitionInfoApprovalDetailId).ToList();
                            if (requInfo.Count == 0)
                            {
                                _prmCommonService.PRMUnit.ClearanceInfoFromMinistryDetailRepository.Add(prm_ClearanceInfoFromMinistryDetail);
                            }
                        }
                        else
                        {
                            _prmCommonService.PRMUnit.ClearanceInfoFromMinistryDetailRepository.Update(prm_ClearanceInfoFromMinistryDetail);

                        }
                    }
                }
                _prmCommonService.PRMUnit.ClearanceInfoFromMinistryDetailRepository.SaveChanges();

            }

            return entity;
        }

        private void populateDropdown(ClearanceInfoFromMinistryViewModel model)
        {
            #region Reference No
            var ddlList =(from jobre in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.GetAll()
                          join jobreSum in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll() on jobre.JobRequisitionInfoSummaryId equals jobreSum.Id
                          where(jobre.Status=="Approved")
                          select jobreSum
                         ).OrderBy(x => x.ReferenceNo).ToList();

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
            var requisitionInfo = (from reqAprv in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.GetAll() 
                                   join reqSum in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll() on reqAprv.JobRequisitionInfoSummaryId equals reqSum.Id
                                   join reqSumDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on reqSum.Id equals reqSumDtl.JobRequisitionInfoSummaryId
                                   join reqInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on reqSumDtl.RequisitionInfoDetailId equals reqInfoDtl.Id
                                   join reqInfo in _prmCommonService.PRMUnit.JobRequisitionInfoRepository.GetAll() on reqInfoDtl.RequisitionInfoId equals reqInfo.Id
                                   join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on reqInfo.PreparedById equals emp.Id
                                   where (reqSum.Id == referenceNoId && reqSum.IsSubmit == true && reqAprv.Status == "Approved")
                                   select new ClearanceInfoFromMinistryDetailViewModel
                                   {
                                       RequisionId = reqInfo.Id,
                                       RequisitionSummaryId = reqSumDtl.JobRequisitionInfoSummaryId,
                                       RequisitionApproveId = reqAprv.PRM_JobRequisitionInfoApprovalDetail.Select(x=>x.Id).FirstOrDefault(),
                                       RequisitionNo = reqInfo.RequisitionNo,
                                       ReqPreparedBy = emp.FullName,
                                       Designation = emp.PRM_Designation.Name,
                                       SubmissionDate = reqInfo.RequisitionSubDate.ToString("yyyy-MM-dd")

                                   }).DistinctBy(x => x.RequisitionNo).ToList();

            return PartialView("_ReqList", new ClearanceInfoFromMinistryViewModel { RequsitionClearanceDetailList = requisitionInfo });
        }

        [HttpPost]
        public PartialViewResult AddedRequisitionInfo(List<ClearanceInfoFromMinistryDetailViewModel> RequisitionCodes, string ModeIs) //for getting requisition detail info
        {
            var model = new ClearanceInfoFromMinistryViewModel();

            List<ClearanceInfoFromMinistryViewModel> AssignmentList = new List<ClearanceInfoFromMinistryViewModel>();
            if (RequisitionCodes != null)
            {
                var list = (from jobReqAprDtl in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.GetAll()
                            join jobReqSumDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on jobReqAprDtl.JobRequisitionInfoSummaryDtlId equals jobReqSumDtl.Id
                            join jobReInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on jobReqSumDtl.RequisitionInfoDetailId equals jobReInfoDtl.Id
                            select jobReqAprDtl).Where(x => RequisitionCodes.Select(n => n.RequisionId).Contains(x.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.RequisitionInfoId)).ToList();

                foreach (var vmEmp in list)
                {
                    var dupList = _prmCommonService.PRMUnit.ClearanceInfoFromMinistryDetailRepository.GetAll().Where(x => x.JobRequisitionInfoApprovalDetailId == vmEmp.Id).ToList();   // for checking duplicate

                    if (ModeIs == "Create")
                    {
                        if (dupList.Count == 0)
                        {
                            var gridModel = new ClearanceInfoFromMinistryViewModel
                            {
                                RequisitionInfoApprovalDetailId = vmEmp.Id,
                                DesignationId = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.DesignationId,
                                Designation = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Designation.Name,
                                NumberOfRequiredPost = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.NumOfRequiredPost,
                                PayScale = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_JobGrade.PayScale,
                                DepartmentName = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Division.Name,
                                SectionName =vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.SectionId==null?string.Empty : vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Section.Name,
                                RecommendPost = vmEmp.PRM_JobRequisitionInfoSummaryDetail.NumOfRecommendedPost,
                                ApprovedPost = vmEmp.ApprovedPost,
                                ClearancePost = vmEmp.ApprovedPost,
                                RequisitionNo = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_JobRequisitionInfo.RequisitionNo
                            };
                            AssignmentList.Add(gridModel);
                        }
                    }
                    else
                    {
                        if (dupList.Count != 0)
                        {
                            var gridModel = new ClearanceInfoFromMinistryViewModel
                            {
                                RequisitionInfoApprovalDetailId = vmEmp.Id,
                                DesignationId = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.DesignationId,
                                Designation = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Designation.Name,
                                NumberOfRequiredPost = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.NumOfRequiredPost,
                                PayScale = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_JobGrade.PayScale,
                                DepartmentName = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Division.Name,
                                SectionName = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.SectionId == null ? string.Empty : vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Section.Name,
                                RecommendPost = vmEmp.PRM_JobRequisitionInfoSummaryDetail.NumOfRecommendedPost,
                                ApprovedPost = vmEmp.ApprovedPost,
                                ClearancePost = vmEmp.ApprovedPost,
                                RequisitionNo = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_JobRequisitionInfo.RequisitionNo,
                                IsCheckedFinal = true
                            };
                            AssignmentList.Add(gridModel);
                        }
                        else
                        {
                            var gridModel = new ClearanceInfoFromMinistryViewModel
                            {
                                RequisitionInfoApprovalDetailId = vmEmp.Id,
                                DesignationId = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.DesignationId,
                                Designation = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Designation.Name,
                                NumberOfRequiredPost = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.NumOfRequiredPost,
                                PayScale = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_JobGrade.PayScale,
                                DepartmentName = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Division.Name,
                                SectionName = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.SectionId == null ? string.Empty : vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Section.Name,
                                RecommendPost = vmEmp.PRM_JobRequisitionInfoSummaryDetail.NumOfRecommendedPost,
                                ApprovedPost = vmEmp.ApprovedPost,
                                ClearancePost = vmEmp.ApprovedPost,
                                RequisitionNo = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_JobRequisitionInfo.RequisitionNo,
                                IsCheckedFinal = false
                            };
                            AssignmentList.Add(gridModel);

                        }

                    }
                }

                model.JobRequisitionClearanceList = AssignmentList;
            }
            return PartialView("_Details", model);
        }

        [NoCache]
        public JsonResult SummaryOfRequisitionInfo(int referenceNoId)
        {
            List<ClearanceInfoFromMinistryViewModel> requisition = (from req in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll()
                                                                     join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on req.PreparedById equals emp.Id
                                                                     join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                                                                     join fin in _famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll() on req.FinancialYearId equals fin.Id
                                                                     where (req.Id == referenceNoId)
                                                                     select new ClearanceInfoFromMinistryViewModel()
                                                                     {
                                                                         ReferenceDate = req.ReferenceDate.ToString("yyyy-MM-dd"),
                                                                         PreparedBy = emp.FullName,
                                                                         Designation = des.Name,
                                                                         FinancialYear = fin.FinancialYearName,
                                                                         FinancialYearId = req.FinancialYearId
                                                                     }).ToList();
            var date = string.Empty;
            var name = string.Empty;
            var desig = string.Empty;
            var finan = string.Empty;
            var financialYearId = 0;
            foreach (var item in requisition)
            {
                date = item.ReferenceDate;
                name = item.PreparedBy;
                desig = item.Designation;
                finan = item.FinancialYear;
                financialYearId = item.FinancialYearId;
            }
            return Json(new
            {
                RefDate = date,
                Name = name,
                Designation = desig,
                FinancialYear = finan
            });
        }

        #region Attachment

        private int Upload(ClearanceInfoFromMinistryViewModel model)
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
                model.IsError = 1;

            }
            catch (Exception ex)
            {
                model.IsError = 0;
                model.ErrMsg = "Upload File Error!";
            }

            return model.IsError;
        }

        public void DownloadDoc(ClearanceInfoFromMinistryViewModel model)
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