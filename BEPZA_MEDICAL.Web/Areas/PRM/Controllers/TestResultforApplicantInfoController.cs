using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class TestResultforApplicantInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Constructor
        public TestResultforApplicantInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion
        //
        // GET: /PRM/TestResultforApplicantInfo/
        public ActionResult Index()
        {
            return View();
        }

        #region JQ Grid List
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, TestResultforApplicantInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<TestResultforApplicantInfoViewModel> list = (from appTst in _prmCommonService.PRMUnit.TestResultforApplicantInfoRepository.GetAll()
                                                              join appTstDtl in _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.GetAll() on appTst.Id equals appTstDtl.TestResultforApplicantInfoId
                                                              join appInfo in _prmCommonService.PRMUnit.ApplicantInfoRepository.GetAll() on appTstDtl.ApplicantInfoId equals appInfo.Id
                                                              where (model.JobAdvertisementInfoId == 0 || model.JobAdvertisementInfoId == appInfo.PRM_JobAdvertisementInfo.Id)
                                                               && (model.DesignationId == 0 || model.DesignationId == null || model.DesignationId == appInfo.PRM_Designation.Id)
                                                               && (model.SelectionCriteriaOrExamTypeId == 0  || model.SelectionCriteriaOrExamTypeId == appTst.SelectionCriteriaOrExamTypeId)
                                                               &&(appInfo.ZoneInfoId==LoggedUserZoneInfoId)
                                                              select new TestResultforApplicantInfoViewModel()
                                                               {
                                                                   Id = appTst.Id,
                                                                   JobAdvertisementInfoId = appInfo.PRM_JobAdvertisementInfo.Id,
                                                                   AdvertisementCode = appInfo.PRM_JobAdvertisementInfo.AdCode,
                                                                   DesignationId = appInfo.PRM_Designation.Id,
                                                                   DesignationName = appInfo.PRM_Designation.Name,
                                                                   SelectionCriteriaOrExamTypeId=appTst.SelectionCriteriaOrExamTypeId,
                                                                   ExamTypeName=appTst.PRM_SelectionCritariaOrExamType.Name
                                                               }).Concat(from appTst in _prmCommonService.PRMUnit.TestResultforApplicantInfoRepository.GetAll()
                                                                         join appTstDtl in _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.GetAll() on appTst.Id equals appTstDtl.TestResultforApplicantInfoId
                                                                         join appInfo in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on appTstDtl.ApplicantInfoId equals appInfo.intPK
                                                                         join desi in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on appInfo.DesignationId equals desi.Id
                                                                         where (model.JobAdvertisementInfoId == 0 || model.JobAdvertisementInfoId == appTst.PRM_JobAdvertisementInfo.Id)
                                                                          && (model.DesignationId == 0 || model.DesignationId == null || model.DesignationId == appInfo.DesignationId)
                                                                          && (model.SelectionCriteriaOrExamTypeId == 0 || model.SelectionCriteriaOrExamTypeId == appTst.SelectionCriteriaOrExamTypeId)
                                                                          //&& (appInfo.ZoneInfoId == LoggedUserZoneInfoId)
                                                                         select new TestResultforApplicantInfoViewModel()
                                                                         {
                                                                             Id = appTst.Id,
                                                                             JobAdvertisementInfoId = appTst.JobAdvertisementInfoId,
                                                                             AdvertisementCode = appTst.PRM_JobAdvertisementInfo.AdCode,
                                                                             DesignationId = desi.Id,
                                                                             DesignationName = desi.Name,
                                                                             SelectionCriteriaOrExamTypeId = appTst.SelectionCriteriaOrExamTypeId,
                                                                             ExamTypeName = appTst.PRM_SelectionCritariaOrExamType.Name
                                                                         }).DistinctBy(x => x.Id).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "AdvertisementCode")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AdvertisementCode).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AdvertisementCode).ToList();
                }
            }
            if (request.SortingName == "DesignationName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DesignationName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DesignationName).ToList();
                }
            }
            if (request.SortingName == "ExamTypeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ExamTypeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ExamTypeName).ToList();
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
                  d.JobAdvertisementInfoId,
                  d.AdvertisementCode,
                  d.DesignationId,
                  d.DesignationName,
                  d.SelectionCriteriaOrExamTypeId,
                  d.ExamTypeName,
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult AdCodeforView()
        {
            var list = Common.PopulateJobAdvertisementDDL(_prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.AdCode).ToList());
            return PartialView("Select", list);
        }
        [NoCache]
        public ActionResult DesignationforView()
        {
            var list = Common.PopulateDllList(_prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }
        [NoCache]
        public ActionResult ExamTypeforView()
        {
            var list = Common.PopulateDllList(_prmCommonService.PRMUnit.SelectionCritariaOrExamTypeRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }
        #endregion

        public ActionResult Create()
        {
            TestResultforApplicantInfoViewModel model = new TestResultforApplicantInfoViewModel();
            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        [NoCache]
        public ActionResult Create(TestResultforApplicantInfoViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                model.IsError = 1;

                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    var entity = CreateEntity(model, true);
                    if (entity.Id <= 0)
                    {
                        _prmCommonService.PRMUnit.TestResultforApplicantInfoRepository.Add(entity);
                        _prmCommonService.PRMUnit.TestResultforApplicantInfoRepository.SaveChanges();
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

                            _prmCommonService.PRMUnit.TestResultforApplicantInfoRepository.Update(entity);
                            _prmCommonService.PRMUnit.TestResultforApplicantInfoRepository.SaveChanges();
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
        public ActionResult Edit(int Id, string type)
        {
            var entity = _prmCommonService.PRMUnit.TestResultforApplicantInfoRepository.GetByID(Id);

            var model = entity.ToModel();
            model.strMode = "Edit";
            model.IsInEditMode = true;

            //applicant shortlist Detail
            var list = (from selCri in _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll()
                        join selCriDtl in _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.GetAll() on selCri.Id equals selCriDtl.SelectionCriteriaId
                        join appInt in _prmCommonService.PRMUnit.TestResultforApplicantInfoRepository.GetAll() on selCri.JobAdvertisementInfoId equals appInt.JobAdvertisementInfoId
                        join appIntDtl in _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.GetAll() on appInt.Id equals appIntDtl.TestResultforApplicantInfoId
                        join applicant in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on appIntDtl.ApplicantInfoId equals applicant.intPK
                        where (appIntDtl.TestResultforApplicantInfoId == Id && selCriDtl.SelectionCriteriaOrExamTypeId==model.SelectionCriteriaOrExamTypeId)
                        && (selCri.DesignationId == applicant.DesignationId)
                        select new TestResultforApplicantInfoViewModel()
                        {
                            Id = appIntDtl.Id,
                            ApplicantInfoId = appIntDtl.ApplicantInfoId,
                            RollNo = applicant.RollNo,
                            FullMark = selCriDtl.FullMark,
                            PassMark=selCriDtl.PassMark,
                            ObtainedMarks=appIntDtl.ObtainedMarks,
                            Notes=appIntDtl.Notes,
                            ApplicantName = applicant.Name,
                            DateOfBirth = applicant.DateOfBirth.ToString("dd-MM-yyyy"),
                            IsCheckedFinal = true

                        }).DistinctBy(x=>x.ApplicantInfoId).ToList();
            model.ApplicantInformationListDetail = list;

            #region Exam Type

            var ddlList = _prmCommonService.PRMUnit.SelectionCritariaOrExamTypeRepository.GetAll().Where(x=>x.Id==model.SelectionCriteriaOrExamTypeId).OrderBy(x => x.Name).ToList();
            model.SelectionCriteriaList = Common.PopulateDllList(ddlList);
            #endregion

            #region Designation
            var test = _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.GetAll().Where(x => x.TestResultforApplicantInfoId == model.Id).FirstOrDefault();
            var obj = _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll().Where(x => x.intPK == test.ApplicantInfoId).FirstOrDefault();
            var des = _prmCommonService.PRMUnit.DesignationRepository.GetAll().Where(x => x.Id == obj.DesignationId).OrderBy(x => x.Name).ToList();
            model.DesignationNameList = Common.PopulateDllList(des);
            model.DesignationId = obj.DesignationId;
            #endregion

            #region Other
            var other = _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll().Where(x => x.DesignationId == model.DesignationId).Where(x => x.JobAdvertisementInfoId == model.JobAdvertisementInfoId).FirstOrDefault();
            if (other != null)
            {
                model.DepartmentName = other.PRM_Division == null ? string.Empty : other.PRM_Division.Name;
                model.SectionName = other.SectionId == null ? string.Empty : other.PRM_Section.Name;
                model.NoOfPost = other.NumberOfClearancePosition;
            }
            else
            {
                var newList = _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll().Where(x => x.DesignationId == model.DesignationId).Where(x => x.JobAdvertisementInfoId == model.JobAdvertisementInfoId).FirstOrDefault();
                if (newList != null)
                {
                    model.DepartmentName = newList.PRM_Division == null ? string.Empty : newList.PRM_Division.Name;
                    model.SectionName = newList.SectionId == null ? string.Empty : newList.PRM_Section.Name;
                    model.NoOfPost = newList.NumberOfPosition;
                }

            }
            #endregion

            populateDropdown(model);
            if (type == "success")
            {
                model.IsError = 1;
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            return View("Create", model);
        }

        #region Delete

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _prmCommonService.PRMUnit.TestResultforApplicantInfoRepository.GetByID(id);

            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(PRM_TestResultforApplicantInfoDetail) };

                    _prmCommonService.PRMUnit.TestResultforApplicantInfoRepository.Delete(tempPeriod.Id, allTypes);
                    _prmCommonService.PRMUnit.TestResultforApplicantInfoRepository.SaveChanges();
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
                _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.Delete(Id);
                _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.SaveChanges();
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

        private PRM_TestResultforApplicantInfo CreateEntity(TestResultforApplicantInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            if (model.strMode == "Edit")
            {
                pAddEdit = false;
            }

            #region Requisition
            foreach (var c in model.ApplicantInformationListDetail)
            {
                var prm_TestResultforApplicantInfoDetail = new PRM_TestResultforApplicantInfoDetail();

                if (c.IsCheckedFinal)
                {
                    prm_TestResultforApplicantInfoDetail.Id = c.Id;
                    prm_TestResultforApplicantInfoDetail.ApplicantInfoId = c.ApplicantInfoId;
                    prm_TestResultforApplicantInfoDetail.ObtainedMarks = (int)c.ObtainedMarks;
                    prm_TestResultforApplicantInfoDetail.Notes = c.Notes;
                    prm_TestResultforApplicantInfoDetail.IUser = User.Identity.Name;
                    prm_TestResultforApplicantInfoDetail.IDate = DateTime.Now;

                    if (pAddEdit)
                    {
                        prm_TestResultforApplicantInfoDetail.IUser = User.Identity.Name;
                        prm_TestResultforApplicantInfoDetail.IDate = DateTime.Now;
                        entity.PRM_TestResultforApplicantInfoDetail.Add(prm_TestResultforApplicantInfoDetail);
                    }
                    else
                    {
                        prm_TestResultforApplicantInfoDetail.TestResultforApplicantInfoId = model.Id;
                        prm_TestResultforApplicantInfoDetail.EUser = User.Identity.Name;
                        prm_TestResultforApplicantInfoDetail.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                            var requInfo = _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.GetAll().Where(x => x.ApplicantInfoId == c.ApplicantInfoId).ToList();
                            if (requInfo.Count == 0)
                            {
                                _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.Add(prm_TestResultforApplicantInfoDetail);
                            }
                        }
                        else
                        {
                            _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.Update(prm_TestResultforApplicantInfoDetail);

                        }
                    }
                    _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.SaveChanges();
                }
            }
            #endregion

            return entity;
        }

        #region Populate Dropdown
        private void populateDropdown(TestResultforApplicantInfoViewModel model)
        {
            dynamic ddlList;

            #region job advertisement
                  ddlList =(from test in _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.Fetch()
                            join jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Fetch() on test.AdvertisementInfoId equals jobAd.Id
                            select jobAd).Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).DistinctBy(o => o.AdCode).ToList();

            model.AdvertisementCodeList = Common.PopulateJobAdvertisementDDL(ddlList);

            #endregion


        }
        #endregion

        public ActionResult SelectionCriteriaOrExamType(int jobAdInfoId)
        {
            var tempList = (from selectionCri in _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll()
                            join selectionCriDtl in _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.GetAll() on selectionCri.Id equals selectionCriDtl.SelectionCriteriaId
                            select selectionCriDtl).Where(x => x.PRM_SelectionCriteria.JobAdvertisementInfoId == jobAdInfoId).ToList();


            var list = tempList.Select(x => new { Id = x.PRM_SelectionCritariaOrExamType.Id, Name = x.PRM_SelectionCritariaOrExamType.Name }).OrderBy(x => x.Name).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
        }

        #region Previous
        ////get jobpost by job advertisement id
        //[HttpGet]
        //public PartialViewResult GetJobPost(int jobAdId)
        //{
        //    List<TestResultforApplicantInfoViewModel> list = new List<TestResultforApplicantInfoViewModel>();

        //    list = (from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll()
        //            join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
        //            join dept in _prmCommonService.PRMUnit.DivisionRepository.GetAll() on jobAdReq.DepartmentId equals dept.Id
        //            join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on jobAdReq.DesignationId equals des.Id
        //            join sec in _prmCommonService.PRMUnit.SectionRepository.GetAll() on jobAdReq.SectionId equals sec.Id
        //            where (jobAd.Id == jobAdId)
        //            select new TestResultforApplicantInfoViewModel
        //            {
        //                DesignationId = des.Id,
        //                DesignationName = des.Name,
        //                DepartmentId = dept.Id,
        //                DepartmentName = dept.Name,
        //                SectionId = sec.Id,
        //                SectionName = sec.Name,
        //                NoOfPost = jobAdReq.NumberOfPosition
        //            }).ToList();

        //    return PartialView("_JobPost", new TestResultforApplicantInfoViewModel { JobPostInformationList = list });

        //}
        #endregion

        //get applicant info by jobpost(designation) id
        [HttpPost]
        public PartialViewResult AddedApplicantInfo(int designationId, string strMode, int exapTypeId, int jobAdInfoId)
        {
            var model = new TestResultforApplicantInfoViewModel();
            //var fromRollNo = _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetAll().Where(x=>x.AdvertisementInfoId == jobAdInfoId)
            List<TestResultforApplicantInfoViewModel> AssignmentList = new List<TestResultforApplicantInfoViewModel>();
            if (designationId != 0)
            {
                var list = (from selCri in _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll()
                            join selCriDtl in _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.GetAll() on selCri.Id equals selCriDtl.SelectionCriteriaId
                            join appCadr in _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetAll().Where(x => x.DesignationId == designationId).Where(x => x.SelectionCriteriaOrExamTypeId == exapTypeId) on selCri.JobAdvertisementInfoId equals appCadr.AdvertisementInfoId
                            join appInfo in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on designationId equals appInfo.DesignationId
                            where (selCri.JobAdvertisementInfoId == jobAdInfoId)
                            && (designationId == Convert.ToInt32(selCri.DesignationId))
                            && (selCriDtl.SelectionCriteriaOrExamTypeId == exapTypeId)
                            && (appInfo.RollNo >= appCadr.FromRollNo && appInfo.RollNo <= appCadr.ToRollNo)
                            select new TestResultforApplicantInfoViewModel()
                            {
                                ApplicantInfoId = appInfo.intPK,
                                RollNo = appInfo.RollNo,
                                FullMark = selCriDtl.FullMark,
                                PassMark = selCriDtl.PassMark,
                                ApplicantName = appInfo.Name,
                                DateOfBirth = appInfo.DateOfBirth.ToString("dd-MM-yyyy")
                            }).DistinctBy(x => x.ApplicantInfoId).ToList();



                foreach (var vmApplicant in list)
                {
                    var dupList = _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.GetAll().Where(x => x.ApplicantInfoId == vmApplicant.Id).ToList();   // for checking duplicate

                    if (strMode == "Create")
                    {
                        if (dupList.Count == 0)
                        {
                            var gridModel = new TestResultforApplicantInfoViewModel
                            {
                                ApplicantInfoId = vmApplicant.ApplicantInfoId,
                                RollNo = vmApplicant.RollNo,
                                FullMark = vmApplicant.FullMark,
                                PassMark = vmApplicant.PassMark,
                                ApplicantName = vmApplicant.ApplicantName,
                                DateOfBirth = vmApplicant.DateOfBirth,
                                DesignationName = vmApplicant.DesignationName

                            };
                            AssignmentList.Add(gridModel);
                        }
                    }
                    else
                    {
                        if (dupList.Count != 0)
                        {
                            var gridModel = new TestResultforApplicantInfoViewModel
                            {
                                ApplicantInfoId = vmApplicant.ApplicantInfoId,
                                RollNo=vmApplicant.RollNo,
                                FullMark=vmApplicant.FullMark,
                                PassMark=vmApplicant.PassMark,
                                ApplicantName = vmApplicant.ApplicantName,
                                DateOfBirth = vmApplicant.DateOfBirth,
                                DesignationName = vmApplicant.DesignationName,
                                IsCheckedFinal = true,

                            };
                            AssignmentList.Add(gridModel);
                        }
                        else
                        {
                            var gridModel = new TestResultforApplicantInfoViewModel
                            {
                                ApplicantInfoId = vmApplicant.ApplicantInfoId,
                                RollNo=vmApplicant.RollNo,
                                FullMark=vmApplicant.FullMark,
                                PassMark=vmApplicant.PassMark,
                                ApplicantName = vmApplicant.ApplicantName,
                                DateOfBirth = vmApplicant.DateOfBirth,
                                DesignationName = vmApplicant.DesignationName,
                                IsCheckedFinal = false
                            };
                            AssignmentList.Add(gridModel);

                        }

                    }
                }

                model.ApplicantInformationListDetail = AssignmentList;
            }
            return PartialView("_Details", model);
        }

        public ActionResult SelectJobPostType(int jobAdInfoId)
        {
            //var tempList = (from appStr in _prmCommonService.PRMUnit.ApplicantShortListApprovalRepository.GetAll()
            //                join appStrDtl in _prmCommonService.PRMUnit.ApplicantShortListApprovalDetailRepository.GetAll() on appStr.Id equals appStrDtl.ApplicantShortListApprovalId
            //                join appInfo in _prmCommonService.PRMUnit.ApplicantInfoRepository.GetAll() on appStrDtl.ApplicantInfoId equals appInfo.Id
            //                select appInfo).Where(x => x.JobAdvertisementInfoId == jobAdInfoId).ToList();
            //var list = tempList.Select(x => new { Id = x.PRM_Designation.Id, Name = x.PRM_Designation.Name }).DistinctBy(x => x.Id).ToList();

            var tempList = (from appInt in _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetAll().Where(x=>x.AdvertisementInfoId == jobAdInfoId)
                            join desi in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on appInt.DesignationId equals desi.Id
                            select desi).ToList();

            var list = tempList.Select(x => new { Id = x.Id, Name = x.Name }).DistinctBy(x => x.Id).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult SelectionCriteriaOrExam(int jobAdInfoId, int designationId)
        {
            var examList = _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetAll().Where(x => x.AdvertisementInfoId == jobAdInfoId).Where(y => y.DesignationId == designationId).Where(z=>z.IsIssue==true).ToList();
            var tempList = (from selectionCri in _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll()
                            join selectionCriDtl in _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.GetAll() on selectionCri.Id equals selectionCriDtl.SelectionCriteriaId
                            select selectionCriDtl)
                            .Where(x => x.PRM_SelectionCriteria.DesignationId == designationId)
                            .Where(x => x.PRM_SelectionCriteria.JobAdvertisementInfoId == jobAdInfoId)
                            .Where(x => examList.Select(n => n.SelectionCriteriaOrExamTypeId).Contains(x.SelectionCriteriaOrExamTypeId))
                            .ToList();


            var list = tempList.Select(x => new { Id = x.PRM_SelectionCritariaOrExamType.Id, Name = x.PRM_SelectionCritariaOrExamType.Name }).OrderBy(x => x.Name).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
        }
        [NoCache]
        public JsonResult GetRequsionInfo(int designationId)
        {
            var obj = _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll().Where(x => x.DesignationId == designationId).FirstOrDefault();

            return Json(new
            {
                department = obj.PRM_Division.Name,
                section = obj.PRM_Section.Name,
                noOfPost = obj.NumberOfClearancePosition
            });

        }

	}
}