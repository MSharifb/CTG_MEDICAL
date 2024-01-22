using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.FAM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.JobAdvertisementInfo;
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
    public class JobAdvertisementInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly FAMCommonService _famCommonService;
        #endregion

        #region Constructor
        public JobAdvertisementInfoController(PRMCommonSevice prmCommonService, FAMCommonService famCommonService)
        {
            this._prmCommonService = prmCommonService;
            this._famCommonService = famCommonService;
        }
        #endregion
        //
        // GET: /PRM/JobAdvertisementInfo/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, JobAdvertisementInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<JobAdvertisementInfoViewModel> list = (from ad in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll()
                                                             where (model.Id == 0 || model.Id == ad.Id)
                                                             && ( model.AdDate == null || model.AdDate == ad.AdDate)
                                                             &&(ad.ZoneInfoId==LoggedUserZoneInfoId)
                                                          select new JobAdvertisementInfoViewModel()
                                                             {
                                                                 Id = ad.Id,
                                                                 AdDate=ad.AdDate,
                                                                 AdCode=ad.AdCode
                                                             }).DistinctBy(x => x.AdCode).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "AdCode")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AdCode).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AdCode).ToList();
                }
            }


            if (request.SortingName == "AdDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AdDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AdDate).ToList();
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
                  d.AdCode,
                  ((DateTime)d.AdDate).ToString(DateAndTime.GlobalDateFormat),
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult AdCodeforView()
        {
            var list = Common.PopulateJobAdvertisementDDL(_prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll().Where(x=>x.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.AdCode).ToList());
            return PartialView("Select", list);
        }
        public ActionResult Create()
        {
            JobAdvertisementInfoViewModel model = new JobAdvertisementInfoViewModel();
            model.AdDate = DateTime.UtcNow;
            model.AdvertisementExpDate = DateTime.UtcNow;
            model.AgeCalDate = DateTime.UtcNow;
            model.AppEndDate = DateTime.UtcNow;
            model.RollGenerationDate = DateTime.UtcNow;

            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create([Bind(Exclude = "Attachment")] JobAdvertisementInfoViewModel model)
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
                        _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Add(entity);
                        _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        //populateDropdown(model);
                        return RedirectToAction("Index");
                    }
                    else
                    {                    
                        if (errorList.Length == 0)
                        {
                            entity.EUser = User.Identity.Name;
                            entity.EDate = DateTime.Now;

                            _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Update(entity);
                            _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.SaveChanges();
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
                populateDropdown(model);
                model.IsError = 1;
                model.errClass = "failed";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                return View(model);
            }
            return View(model);
        }

        public ActionResult Edit(int id, string type)
        {
            var AdInfoEntity = _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetByID(id);
            var parentModel = AdInfoEntity.ToModel();
            parentModel.strMode = "Edit";
            parentModel.IsInEditMode = true;

            #region Advertisement Media
            List<JobAdvertisementInfoMediaViewModel> mList = (from adm in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailMediaRepository.GetAll()
                                                              join media in _prmCommonService.PRMUnit.MediaRepository.GetAll() on adm.AdvertisementMediaId equals media.Id
                                                              where (adm.JobAdvertisementInfoId == id)
                                                              select new JobAdvertisementInfoMediaViewModel()
                                                              {
                                                                  Id=adm.Id,
                                                                  AdvertisementMediaId=adm.AdvertisementMediaId,
                                                                  AdvertisementMediaName=media.Name,
                                                                  AdvertisementDate = adm.AdvertisementDate,
                                                                  Notes=adm.Notes,
                                                                  AdvertisementExpDate=adm.AdvertisementExpDate

                                                              }).ToList();
            parentModel.JobAdvertisementInfoMedia = mList;
            #endregion

            #region Post Detail
            List<JobAdvertisementPostDetailViewModel> pList = (from adm in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll()
                                                              //join media in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on adm.AdvertisementMediaId equals media.Id
                                                              where (adm.JobAdvertisementInfoId == id)
                                                               select new JobAdvertisementPostDetailViewModel()
                                                              {
                                                                  Id = adm.Id,
                                                                  EmployeeTypeId = adm.EmployeeTypeId,
                                                                   ApplicationFee = adm.ApplicationFee,
                                                                    JobAdvertisementInfoId = adm.JobAdvertisementInfoId,
                                                                     NumberOfPosition = adm.NumberOfPosition,
                                                                      SectionId = adm.SectionId,
                                                                       DepartmentId = adm.DepartmentId,
                                                                        OrgLevelId = Convert.ToInt32( adm.OrgLevelId),
                                                                         DesignationId = adm.DesignationId,
                                                                          DesignationName = adm.PRM_Designation== null? "":adm.PRM_Designation.Name

                                                              }).ToList();
            parentModel.JobAdvertisementPostDetail = pList;
            #endregion

            #region Post District 
            List<JobAdvertisementInfoDistrictsViewModel> dList = (from adm in _prmCommonService.PRMUnit.JobAdvertisementDestrictRepository.GetAll()
                                                               join dis in _prmCommonService.PRMUnit.ERECtblDistrictRepository.GetAll() on adm.DistrictId equals dis.intPK
                                                               where (adm.JobAdvertisementInfoId == id)
                                                                  select new JobAdvertisementInfoDistrictsViewModel()
                                                               {
                                                                   Id = adm.Id,
                                                                   JobAdvertisementInfoId = adm.JobAdvertisementInfoId,
                                                                   DesignationId = adm.DesignationId,
                                                                   DistrictId = adm.DistrictId,
                                                                   DistrictName = dis.Name,
                                                                   DesignationName = adm.PRM_Designation == null ? "" : adm.PRM_Designation.Name

                                                               }).ToList();
            parentModel.JobAdvertisementInfoDistricts = dList;
            #endregion

            #region Requisition
            //Job Requisition Info Detail
            //List<JobAdvertisementInfoViewModel> list = (from jobAdInfo in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll()
            //                                            join jobAdDtl in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll() on jobAdInfo.Id equals jobAdDtl.JobAdvertisementInfoId
            //                                            join jobDtl in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.GetAll() on jobAdDtl.JobRequisitionInfoApprovalDetailId equals jobDtl.Id
            //                                            join jobSummDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on jobDtl.JobRequisitionInfoSummaryDtlId equals jobSummDtl.Id
            //                                            join jobInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on jobSummDtl.RequisitionInfoDetailId equals jobInfoDtl.Id
            //                                            where (jobAdDtl.JobAdvertisementInfoId == id)
            //                                             select new JobAdvertisementInfoViewModel()
            //                                                 {
            //                                                     Id = jobAdDtl.Id,
            //                                                     JobRequisitionInfoApprovalDetailId=jobAdDtl.JobRequisitionInfoApprovalDetailId,
            //                                                     DesignationId = jobAdDtl.DesignationId,
            //                                                     DepartmentId=(int)jobAdDtl.DepartmentId,
            //                                                     SectionId=jobAdDtl.SectionId,
            //                                                     DepartmentName = jobInfoDtl.PRM_Division.Name,
            //                                                     SectionName =jobAdDtl.SectionId==null?string.Empty : jobInfoDtl.PRM_Section.Name,
            //                                                     Designation = jobInfoDtl.PRM_Designation.Name,
            //                                                     NumberOfRequiredPost = jobInfoDtl.NumOfRequiredPost,
            //                                                     RecommendPost = (int)jobSummDtl.NumOfRecommendedPost,
            //                                                     RequisitionNo = jobInfoDtl.PRM_JobRequisitionInfo.RequisitionNo,
            //                                                     ApprovedPost = jobDtl.ApprovedPost,
            //                                                     Category=jobInfoDtl.PRM_EmploymentType.Name,
            //                                                     RequireDate = ((DateTime)jobInfoDtl.RequireDate).ToString("yyyy-MM-dd"),
            //                                                     JobRequisitionInfoSummaryId = jobAdDtl.JobRequisitionInfoSummaryId,
            //                                                     ReferenceNo=jobAdDtl.PRM_JobRequisitionInfoSummary.ReferenceNo,
            //                                                     IsCheckedFinal = true
            //                                                 }).ToList();
            //parentModel.JobRequisitionInfoDetailList = list;

            ////Job Requisition Info
            //var infolist = (from jobAdInfo in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll()
            //                join jobAdDtl in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll() on jobAdInfo.Id equals jobAdDtl.JobAdvertisementInfoId
            //                join jobDtl in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.GetAll() on jobAdDtl.JobRequisitionInfoApprovalDetailId equals jobDtl.Id
            //                join jobSummDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on jobDtl.JobRequisitionInfoSummaryDtlId equals jobSummDtl.Id
            //                join jobInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on jobSummDtl.RequisitionInfoDetailId equals jobInfoDtl.Id
            //                join rec in _prmCommonService.PRMUnit.JobRequisitionInfoRepository.GetAll() on jobInfoDtl.RequisitionInfoId equals rec.Id
            //                join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on rec.PreparedById equals emp.Id
            //                join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
            //                where (jobAdDtl.JobAdvertisementInfoId == id) && (rec.Id == jobInfoDtl.RequisitionInfoId)
            //                select new JobAdvertisementInfoViewModel
            //                {
            //                    RequisionId = rec.Id,
            //                    JobRequisitionInfoApprovalDetailId = jobAdDtl.JobRequisitionInfoApprovalDetailId,
            //                    RequisitionNo = rec.RequisitionNo,
            //                    ReqPreparedBy = emp.FullName,
            //                    Designation = des.Name,
            //                    SubmissionDate = rec.RequisitionSubDate.ToString("yyyy-MM-dd"),
            //                    IsChecked = true,
            //                    strMode = "Edit"
            //                }
            //            ).DistinctBy(x => x.RequisitionNo).ToList();

            //parentModel.JobRequisitionInfoList = infolist;
            #endregion

            #region Attachment
            List<JobAdvertisementInfoViewModel> aList = (from ada in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailAttachmentRepository.GetAll()
                                                                   where (ada.JobAdvertisementId == id)
                                                         select new JobAdvertisementInfoViewModel()
                                                                  {
                                                                   Id = ada.Id,
                                                                   Title=ada.Title,
                                                                   FileName=ada.FileName,
                                                                   Attachment=ada.Attachment
                                                                  }).ToList();

            var aNewlist = new List<JobAdvertisementInfoViewModel>();
            foreach (var item in aList)
            {
                byte[] document =item.Attachment;
                if (document != null)
                {
                    string strFilename = Url.Content("~/Content/" + User.Identity.Name + item.FileName);
                    byte[] doc = document;
                    WriteToFile(Server.MapPath(strFilename), ref doc);

                    item.FilePath = strFilename;
                }
                aNewlist.Add(item);
            }
            //parentModel.JobAdvertisementInfoAttachment = aNewlist;
            parentModel.AttachmentFilesList = aNewlist;


            #endregion

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

            var tempPeriod = _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetByID(id);

            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(PRM_JobAdvertisementInfoDetailRequisition) };
                    allTypes.Add(typeof(PRM_JobAdvertisementInfoDetailMedia));
                    allTypes.Add(typeof(PRM_JobAdvertisementInfoDetailAttachment));
                    allTypes.Add(typeof(PRM_JobAdvertisementPostDetail));
                    allTypes.Add(typeof(PRM_JobAdvertisementInfoDistrict));

                    _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Delete(tempPeriod.Id, allTypes);
                    _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.SaveChanges();
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
                _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.Delete(Id);
                _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.SaveChanges();
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

       [HttpPost, ActionName("DeleteJobAdvertisementDistrict")]
        public JsonResult DeleteJobAdvertisementDistrict(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonService.PRMUnit.JobAdvertisementDestrictRepository.Delete(id);
                _prmCommonService.PRMUnit.JobAdvertisementDestrictRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteJobAdvertisementPostDetail")]
        public JsonResult DeleteJobAdvertisementPostDetail(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.Delete(id);
                _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteJobAdvertisementDetailMedia")]
        public JsonResult DeleteJobAdvertisementDetailfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonService.PRMUnit.JobAdvertisementInfoDetailMediaRepository.Delete(id);
                _prmCommonService.PRMUnit.JobAdvertisementInfoDetailMediaRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteJobAdvertisementDetailAttachment")]
        public JsonResult DeleteJobAdvertisementDetailAtth(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonService.PRMUnit.JobAdvertisementInfoDetailAttachmentRepository.Delete(id);
                _prmCommonService.PRMUnit.JobAdvertisementInfoDetailAttachmentRepository.SaveChanges();
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
        public string BusinessLogicValidation(JobAdvertisementInfoViewModel model)
        {
            string errorMessage = string.Empty;
            if (model.strMode != "Edit")
            {
                var requInfo = _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll().Where(x => x.AdCode == model.AdCode).ToList();
                if (requInfo.Count > 0)
                {
                    errorMessage = "Advertisement Code Already Exist";
                }
            }
            return errorMessage;

        }

        private PRM_JobAdvertisementInfo CreateEntity([Bind(Exclude = "Attachment")] JobAdvertisementInfoViewModel model, bool pAddEdit)
        {
            var tempPost = model.JobAdvertisementPostDetail;
            model.JobAdvertisementPostDetail = null;
            var tempDistrict = model.JobAdvertisementInfoDistricts;
            model.JobAdvertisementInfoDistricts = null;

            var entity = model.ToEntity();
            entity.Id = model.Id;
            if (model.strMode == "Edit")
            {
                pAddEdit = false;
            }

            model.JobAdvertisementPostDetail = tempPost;
            model.JobAdvertisementInfoDistricts = tempDistrict;

            #region Requisition
            //foreach (var c in model.JobRequisitionInfoDetailList)
            //{
            //    var prm_JobAdvertisementInfoDetailRequisition = new PRM_JobAdvertisementInfoDetailRequisition();

            //    if (c.IsCheckedFinal)
            //    {
            //        prm_JobAdvertisementInfoDetailRequisition.Id = c.Id;
            //        prm_JobAdvertisementInfoDetailRequisition.JobRequisitionInfoSummaryId = (int)c.JobRequisitionInfoSummaryId;
            //        prm_JobAdvertisementInfoDetailRequisition.JobRequisitionInfoApprovalDetailId= c.JobRequisitionInfoApprovalDetailId;
            //        prm_JobAdvertisementInfoDetailRequisition.NumberOfPosition = c.NumberOfRequiredPost;
            //        prm_JobAdvertisementInfoDetailRequisition.NumberOfClearancePosition = c.ApprovedPost;
            //        prm_JobAdvertisementInfoDetailRequisition.DesignationId = c.DesignationId;
            //        prm_JobAdvertisementInfoDetailRequisition.DepartmentId = c.DepartmentId;
            //        prm_JobAdvertisementInfoDetailRequisition.SectionId = c.SectionId;
            //        prm_JobAdvertisementInfoDetailRequisition.IUser = User.Identity.Name;
            //        prm_JobAdvertisementInfoDetailRequisition.IDate = DateTime.Now;

            //        if (pAddEdit)
            //        {
            //            prm_JobAdvertisementInfoDetailRequisition.IUser = User.Identity.Name;
            //            prm_JobAdvertisementInfoDetailRequisition.IDate = DateTime.Now;
            //            entity.PRM_JobAdvertisementInfoDetailRequisition.Add(prm_JobAdvertisementInfoDetailRequisition);
            //        }
            //        else
            //        {
            //            prm_JobAdvertisementInfoDetailRequisition.JobAdvertisementInfoId = model.Id;
            //            prm_JobAdvertisementInfoDetailRequisition.EUser = User.Identity.Name;
            //            prm_JobAdvertisementInfoDetailRequisition.EDate = DateTime.Now;

            //            if (c.Id == 0)
            //            {
            //                var requInfo = _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll().Where(x => x.JobRequisitionInfoApprovalDetailId == c.JobRequisitionInfoApprovalDetailId).ToList();
            //                if (requInfo.Count == 0)
            //                {
            //                    _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.Add(prm_JobAdvertisementInfoDetailRequisition);
            //                }
            //            }
            //            else
            //            {
            //                _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.Update(prm_JobAdvertisementInfoDetailRequisition);

            //            }
            //        }
            //        _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.SaveChanges();
            //    }
            //}
            #endregion

            #region Media
            foreach (var c in model.JobAdvertisementInfoMedia)
            {
                var prm_JobAdvertisementInfoDetailMedia = new PRM_JobAdvertisementInfoDetailMedia();

              
                    prm_JobAdvertisementInfoDetailMedia.Id = c.Id;
                    prm_JobAdvertisementInfoDetailMedia.AdvertisementMediaId = c.AdvertisementMediaId;
                    prm_JobAdvertisementInfoDetailMedia.AdvertisementDate = c.AdvertisementDate;
                    prm_JobAdvertisementInfoDetailMedia.Notes = c.Notes;
                    prm_JobAdvertisementInfoDetailMedia.AdvertisementExpDate = c.AdvertisementExpDate;
                    prm_JobAdvertisementInfoDetailMedia.IUser = User.Identity.Name;
                    prm_JobAdvertisementInfoDetailMedia.IDate = DateTime.Now;

                    if (pAddEdit)
                    {
                        prm_JobAdvertisementInfoDetailMedia.IUser = User.Identity.Name;
                        prm_JobAdvertisementInfoDetailMedia.IDate = DateTime.Now;
                        entity.PRM_JobAdvertisementInfoDetailMedia.Add(prm_JobAdvertisementInfoDetailMedia);
                    }
                    else
                    {
                        prm_JobAdvertisementInfoDetailMedia.JobAdvertisementInfoId = model.Id;
                        prm_JobAdvertisementInfoDetailMedia.EUser = User.Identity.Name;
                        prm_JobAdvertisementInfoDetailMedia.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                            _prmCommonService.PRMUnit.JobAdvertisementInfoDetailMediaRepository.Add(prm_JobAdvertisementInfoDetailMedia);                          
                        }
                        else
                        {
                            _prmCommonService.PRMUnit.JobAdvertisementInfoDetailMediaRepository.Update(prm_JobAdvertisementInfoDetailMedia);

                        }
                    }
                    _prmCommonService.PRMUnit.JobAdvertisementInfoDetailMediaRepository.SaveChanges();
            }
            #endregion

            #region Post Detail

            foreach (var item in model.JobAdvertisementPostDetail)
            {
                var postInfo = new PRM_JobAdvertisementPostDetail();


                postInfo.Id = item.Id;
                postInfo.JobAdvertisementInfoId = item.JobAdvertisementInfoId;
                postInfo.NumberOfPosition = item.NumberOfPosition;
                postInfo.OrgLevelId = item.DesignationId;
                postInfo.DepartmentId = item.DepartmentId;
                postInfo.SectionId = item.SectionId;
                postInfo.ApplicationFee = item.ApplicationFee;
                postInfo.EmployeeTypeId = item.EmployeeTypeId;
                postInfo.DesignationId = item.DesignationId;

                postInfo.IUser = User.Identity.Name;
                postInfo.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    postInfo.IUser = User.Identity.Name;
                    postInfo.IDate = DateTime.Now;
                    entity.PRM_JobAdvertisementPostDetail.Add(postInfo);
                }
                else
                {
                    postInfo.JobAdvertisementInfoId = model.Id;
                    postInfo.EUser = User.Identity.Name;
                    postInfo.EDate = DateTime.Now;

                    if (postInfo.Id == 0)
                    {
                        _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.Add(postInfo);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.Update(postInfo);

                    }
                }
                _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.SaveChanges();
            }
            #endregion

            #region Post District

            foreach (var distrinctItem in model.JobAdvertisementInfoDistricts)
            {
                var districtInfo = new PRM_JobAdvertisementInfoDistrict();


                districtInfo.Id = distrinctItem.Id;
                districtInfo.JobAdvertisementInfoId = model.Id;
                districtInfo.DesignationId = distrinctItem.DesignationId;
                districtInfo.DistrictId = distrinctItem.DistrictId;
                districtInfo.IUser = User.Identity.Name;
                districtInfo.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    districtInfo.IUser = User.Identity.Name;
                    districtInfo.IDate = DateTime.Now;
                    entity.PRM_JobAdvertisementInfoDistrict.Add(districtInfo);
                }
                else
                {
                    districtInfo.JobAdvertisementInfoId = model.Id;
                    districtInfo.EUser = User.Identity.Name;
                    districtInfo.EDate = DateTime.Now;

                    if (districtInfo.Id == 0)
                    {
                        _prmCommonService.PRMUnit.JobAdvertisementDestrictRepository.Add(districtInfo);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.JobAdvertisementDestrictRepository.Update(districtInfo);
                    }
                }
                _prmCommonService.PRMUnit.JobAdvertisementDestrictRepository.SaveChanges();
                
            }
            #endregion

            #region Attachment
            if (model.AttachmentFilesList != null)
            {
                foreach (var c in model.AttachmentFilesList)
                {
                    var prm_JobAdvertisementInfoDetailAttachment = new PRM_JobAdvertisementInfoDetailAttachment();

                    prm_JobAdvertisementInfoDetailAttachment.FileName = c.FileName;
                    prm_JobAdvertisementInfoDetailAttachment.Attachment = c.Attachment;
                    prm_JobAdvertisementInfoDetailAttachment.Id = c.Id;
                    prm_JobAdvertisementInfoDetailAttachment.Title = c.Title;
                    prm_JobAdvertisementInfoDetailAttachment.IUser = User.Identity.Name;
                    prm_JobAdvertisementInfoDetailAttachment.IDate = DateTime.Now;

                    if (pAddEdit)
                    {
                        prm_JobAdvertisementInfoDetailAttachment.IUser = User.Identity.Name;
                        prm_JobAdvertisementInfoDetailAttachment.IDate = DateTime.Now;
                        entity.PRM_JobAdvertisementInfoDetailAttachment.Add(prm_JobAdvertisementInfoDetailAttachment);
                    }
                    else
                    {
                        prm_JobAdvertisementInfoDetailAttachment.JobAdvertisementId = model.Id;
                        prm_JobAdvertisementInfoDetailAttachment.EUser = User.Identity.Name;
                        prm_JobAdvertisementInfoDetailAttachment.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                            _prmCommonService.PRMUnit.JobAdvertisementInfoDetailAttachmentRepository.Add(prm_JobAdvertisementInfoDetailAttachment);
                        }
                        else
                        {
                            _prmCommonService.PRMUnit.JobAdvertisementInfoDetailAttachmentRepository.Update(prm_JobAdvertisementInfoDetailAttachment);

                        }
                    }
                    _prmCommonService.PRMUnit.JobAdvertisementInfoDetailAttachmentRepository.SaveChanges();
                }
            }
            #endregion

            return entity;
        }
        private void populateDropdown(JobAdvertisementInfoViewModel model)
        {
            #region Reference No
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
            model.ReferenceNoList = list;

            #endregion

            #region Media
            var med = _prmCommonService.PRMUnit.MediaRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.AdvertisementMediaList = Common.PopulateDllList(med);

            #endregion

            #region Department
            var div = _prmCommonService.PRMUnit.DivisionRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.DepartmentList = Common.PopulateDllList(div);
            #endregion

            #region Section
            var sec = _prmCommonService.PRMUnit.SectionRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.SectionList = Common.PopulateDllList(sec);
            #endregion

            #region Employment Type
            var emp = _prmCommonService.PRMUnit.EmploymentTypeRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.EmploymentTypeList = Common.PopulateDllList(emp);
            #endregion

            #region District
            var districtList = _prmCommonService.PRMUnit.ERECtblDistrictRepository.GetAll().OrderBy(x => x.Name).ToList();
            model.DistrictList = Common.PopulateERECDistrictDDL(districtList);
            #endregion 

        }

        [HttpGet]
        public PartialViewResult GetRequisitionInfo(int referenceNoId)   //for getting requisition info
        {
            #region Previous
            //var requisitionInfo = (from clrInfo in _prmCommonService.PRMUnit.ClearanceInfoFromMinistryRepository.GetAll()
            //                       join clrInfoDtl in _prmCommonService.PRMUnit.ClearanceInfoFromMinistryDetailRepository.GetAll() on clrInfo.Id equals clrInfoDtl.ClearanceInfoFromMinistryId
            //                       join reqAprvDtl in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.GetAll() on clrInfoDtl.JobRequisitionInfoApprovalDetailId equals reqAprvDtl.Id
            //                       join reqSum in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll() on reqAprvDtl.PRM_JobRequisitionInfoApproval.JobRequisitionInfoSummaryId equals reqSum.Id
            //                       join reqSumDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on reqSum.Id equals reqSumDtl.JobRequisitionInfoSummaryId
            //                       join reqInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on reqSumDtl.RequisitionInfoDetailId equals reqInfoDtl.Id
            //                       join reqInfo in _prmCommonService.PRMUnit.JobRequisitionInfoRepository.GetAll() on reqInfoDtl.RequisitionInfoId equals reqInfo.Id
            //                       join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on reqInfo.PreparedById equals emp.Id
            //                       where (clrInfo.JobRequisitionInfoSummaryId == referenceNoId && reqSum.IsSubmit == true && reqAprvDtl.PRM_JobRequisitionInfoApproval.Status == "Approved")
            //                       select new JobAdvertisementInfoViewModel
            //                       {
            //                           RequisionId = reqInfo.Id,
            //                           RequisitionInfoClearanceId = clrInfoDtl.ClearanceInfoFromMinistryId,
            //                           RequisitionNo = reqInfo.RequisitionNo,
            //                           ReqPreparedBy = emp.FullName,
            //                           Designation = emp.PRM_Designation.Name,
            //                           SubmissionDate = reqInfo.RequisitionSubDate.ToString("dd/MM/yyyy")

            //                       }).DistinctBy(x=>x.RequisitionNo).ToList();
            #endregion
            var requisitionInfo = (from reqAprv in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalRepository.GetAll()
                                   join reqSum in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryRepository.GetAll() on reqAprv.JobRequisitionInfoSummaryId equals reqSum.Id
                                   join reqSumDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on reqSum.Id equals reqSumDtl.JobRequisitionInfoSummaryId
                                   join reqInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on reqSumDtl.RequisitionInfoDetailId equals reqInfoDtl.Id
                                   join reqInfo in _prmCommonService.PRMUnit.JobRequisitionInfoRepository.GetAll() on reqInfoDtl.RequisitionInfoId equals reqInfo.Id
                                   join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on reqInfo.PreparedById equals emp.Id
                                   where (reqSum.Id == referenceNoId && reqSum.IsSubmit == true && reqAprv.Status == "Approved")
                                   select new JobAdvertisementInfoViewModel
                                   {
                                       RequisionId = reqInfo.Id,
                                       RequisitionSummaryId = reqSumDtl.JobRequisitionInfoSummaryId,
                                       RequisitionNo = reqInfo.RequisitionNo,
                                       ReqPreparedBy = emp.FullName,
                                       Designation = emp.PRM_Designation.Name,
                                       SubmissionDate = reqInfo.RequisitionSubDate.ToString("yyyy-MM-dd")

                                   }).DistinctBy(x => x.RequisitionNo).ToList();

            return PartialView("_ReqList", new JobAdvertisementInfoViewModel { JobRequisitionInfoList = requisitionInfo });
        }

        [HttpPost]
        public PartialViewResult AddedRequisitionInfo(List<JobAdvertisementInfoRequisitionViewModel> RequisitionCodes, string ModeIs, int JobReqSumId) //for getting requisition detail info
        {
            var model = new JobAdvertisementInfoViewModel();

            List<JobAdvertisementInfoViewModel> AssignmentList = new List<JobAdvertisementInfoViewModel>();
            if (RequisitionCodes != null)
            {
                var list = (from jobReqAprDtl in _prmCommonService.PRMUnit.JobRequisitionInfoApprovalDetailRepository.GetAll()
                            join jobReqSumDtl in _prmCommonService.PRMUnit.JobRequisitionInfoSummaryDetailRepository.GetAll() on jobReqAprDtl.JobRequisitionInfoSummaryDtlId equals jobReqSumDtl.Id
                            join jobReInfoDtl in _prmCommonService.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on jobReqSumDtl.RequisitionInfoDetailId equals jobReInfoDtl.Id
                            select jobReqAprDtl).Where(x => RequisitionCodes.Select(n => n.RequisionId).Contains(x.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.RequisitionInfoId)).ToList();


                foreach (var vmEmp in list)
                {
                    var dupList = _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll().Where(x => x.JobRequisitionInfoApprovalDetailId == vmEmp.Id).ToList();   // for checking duplicate

                    if (ModeIs == "Create")
                    {
                        if (dupList.Count == 0)
                        {
                            var gridModel = new JobAdvertisementInfoViewModel
                            {
                                JobRequisitionInfoApprovalDetailId = vmEmp.Id,
                                JobRequisitionInfoSummaryId = vmEmp.PRM_JobRequisitionInfoApproval.JobRequisitionInfoSummaryId,
                                DesignationId = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.DesignationId,
                                Designation = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Designation.Name,
                                NumberOfRequiredPost = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.NumOfRequiredPost,
                                RequireDate = ((DateTime)vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.RequireDate).ToString("yyyy-MM-dd"),
                                PayScale = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_JobGrade.PayScale,
                                Category = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_EmploymentType.Name,
                                DepartmentId = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Division.Id,
                                SectionId = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.SectionId == null ? null : vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.SectionId,
                                DepartmentName = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Division.Name,
                                SectionName = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.SectionId == null ? string.Empty : vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Section.Name,
                                RecommendPost = vmEmp.PRM_JobRequisitionInfoSummaryDetail.NumOfRecommendedPost,
                                ApprovedPost = vmEmp.ApprovedPost,
                                RequisitionNo = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_JobRequisitionInfo.RequisitionNo
                            };
                            AssignmentList.Add(gridModel);
                        }
                    }
                    else
                    {
                        if (dupList.Count != 0)
                        {
                            var gridModel = new JobAdvertisementInfoViewModel
                            {
                                JobRequisitionInfoApprovalDetailId = vmEmp.Id,
                                JobRequisitionInfoSummaryId = vmEmp.PRM_JobRequisitionInfoApproval.JobRequisitionInfoSummaryId,
                                DesignationId = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.DesignationId,
                                Designation = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Designation.Name,
                                NumberOfRequiredPost = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.NumOfRequiredPost,
                                RequireDate = ((DateTime)vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.RequireDate).ToString("yyyy-MM-dd"),
                                PayScale = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_JobGrade.PayScale,
                                Category = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_EmploymentType.Name,
                                DepartmentId = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Division.Id,
                                SectionId = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.SectionId == null ? null : vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.SectionId,
                                DepartmentName = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Division.Name,
                                SectionName = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.SectionId == null ? string.Empty : vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Section.Name,
                                RecommendPost = vmEmp.PRM_JobRequisitionInfoSummaryDetail.NumOfRecommendedPost,
                                ApprovedPost = vmEmp.ApprovedPost,
                                RequisitionNo = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_JobRequisitionInfo.RequisitionNo,
                                IsCheckedFinal = true
                            };
                            AssignmentList.Add(gridModel);
                        }
                        else
                        {
                            var gridModel = new JobAdvertisementInfoViewModel
                            {
                                JobRequisitionInfoApprovalDetailId = vmEmp.Id,
                                JobRequisitionInfoSummaryId = vmEmp.PRM_JobRequisitionInfoApproval.JobRequisitionInfoSummaryId,
                                DesignationId = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.DesignationId,
                                Designation = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Designation.Name,
                                NumberOfRequiredPost = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.NumOfRequiredPost,
                                RequireDate = ((DateTime)vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.RequireDate).ToString("yyyy-MM-dd"),
                                PayScale = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_JobGrade.PayScale,
                                Category = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_EmploymentType.Name,
                                DepartmentId = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Division.Id,
                                SectionId = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.SectionId == null ? null : vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.SectionId,
                                DepartmentName = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Division.Name,
                                SectionName = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.SectionId == null ? string.Empty : vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_Section.Name,
                                RecommendPost = vmEmp.PRM_JobRequisitionInfoSummaryDetail.NumOfRecommendedPost,
                                ApprovedPost = vmEmp.ApprovedPost,
                                RequisitionNo = vmEmp.PRM_JobRequisitionInfoSummaryDetail.PRM_JobRequisitionInfoDetail.PRM_JobRequisitionInfo.RequisitionNo,
                                IsCheckedFinal = false
                            };
                            AssignmentList.Add(gridModel);

                        }

                    }
                    model.JobRequisitionInfoDetailList = AssignmentList;
                }
            }
            return PartialView("_Details", model);
        }

        #region Attachment

        private int Upload(JobAdvertisementInfoAttachmentViewModel model)
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

        public void DownloadDoc(JobAdvertisementInfoAttachmentViewModel model)
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
        public ActionResult AddAttachemnt([Bind(Exclude = "Attachment")] JobAdvertisementInfoViewModel model)
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

            List<JobAdvertisementInfoViewModel> list = new List<JobAdvertisementInfoViewModel>();

            var attList = Session["attachmentList"] as List<JobAdvertisementInfoViewModel>;

            var obj = new JobAdvertisementInfoViewModel
            {
                Title=model.Title,
                FileName = name,
                Attachment = fileData
            };
            list.Add(obj);
            model.AttachmentFilesList = list;
            attList = list;

            return PartialView("_DetailAtt", model);
        }

	}
}