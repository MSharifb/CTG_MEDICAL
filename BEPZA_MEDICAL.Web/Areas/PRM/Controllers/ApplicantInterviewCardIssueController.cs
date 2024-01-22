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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ApplicantInterviewCardIssueController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Constructor
        public ApplicantInterviewCardIssueController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion

        //
        // GET: /PRM/ApplicantInterviewCardIssue/
        public ActionResult Index()
        {
            return View();
        }

        #region JQ Grid
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ApplicantInterviewCardIssueViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ApplicantInterviewCardIssueViewModel> list = (from app in _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetAll()
                                                               join appDtl in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll() on app.AdvertisementInfoId equals appDtl.JobAdvertisementInfoId
                                                               join appInfo in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on appDtl.Id equals appInfo.CircularID
                                                               join examType in _prmCommonService.PRMUnit.SelectionCritariaOrExamTypeRepository.GetAll() on app.SelectionCriteriaOrExamTypeId equals examType.Id
                                                               where (model.AdvertisementInfoId == 0 ||  model.AdvertisementInfoId == appDtl.JobAdvertisementInfoId)
                                                               && (model.DesignationId == 0 || model.DesignationId == appDtl.DesignationId)
                                                               &&( model.ReferenceNo==null|| model.ReferenceNo=="" || model.ReferenceNo== app.ReferenceNo)
                                                               &&(model.ReferenceDate==DateTime.MinValue|| model.ReferenceDate==app.ReferenceDate)
                                                               && (model.InterviewDate == DateTime.MinValue || model.InterviewDate == app.InterviewDateAndTime)
                                                               //&&(appInfo.ZoneInfoId==LoggedUserZoneInfoId)
                                                               select new ApplicantInterviewCardIssueViewModel()
                                                        {
                                                            Id = app.Id,
                                                            AdvertisementInfoId = app.AdvertisementInfoId,
                                                            AdvertisementCode = appDtl.PRM_JobAdvertisementInfo.AdCode,
                                                            DesignationId = appDtl.DesignationId,
                                                            DesignationName = appDtl.PRM_Designation.Name,
                                                            ReferenceNo=app.ReferenceNo,
                                                            ReferenceDate=app.ReferenceDate,
                                                            InterviewDate = app.InterviewDateAndTime,
                                                            ExamTypeName=examType.Name,
                                                            Issue = app.IsIssue.ToString() == "True" ? "Yes" : "No",
                                                        }).DistinctBy(x=>x.Id).ToList();

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
            if (request.SortingName == "InterviewDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.InterviewDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.InterviewDate).ToList();
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
                  d.AdvertisementInfoId,
                  d.AdvertisementCode,
                  d.DesignationId,
                  d.DesignationName,
                  d.ReferenceNo,
                  ((DateTime)d.ReferenceDate).ToString(DateAndTime.GlobalDateFormat),
                  d.ExamTypeName,
                  ((DateTime)d.InterviewDate).ToString(DateAndTime.GlobalDateFormat),
                  d.Issue,
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
        #endregion

        public ActionResult Create()
        {
            ApplicantInterviewCardIssueViewModel model = new ApplicantInterviewCardIssueViewModel();
            model.ReferenceDate = DateTime.UtcNow;
            model.InterviewDate = DateTime.UtcNow;
            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        [NoCache]
        public ActionResult Create(ApplicantInterviewCardIssueViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                model.IsError = 1;
                errorList = BusinessLogicValidation(model);

                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    //var entity = CreateEntity(model, true);
                    var entity = model.ToEntity();
                    entity.Id = model.Id;

                    if (entity.Id <= 0)
                    {
                         entity.IDate = DateTime.Now;
                        _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.Add(entity);
                        _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.SaveChanges();
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

                            _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.Update(entity);
                            _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.SaveChanges();
                            model.strMode = "E";
                           // return RedirectToAction("Edit", new { id = model.Id, type = "success" });
                        }
                    }

                    if(model.IsEmail)
                    {
                        // Write Code Here
                        var list = (from appInt in _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetAll()
                                    join appDtl in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll() on appInt.AdvertisementInfoId equals appDtl.JobAdvertisementInfoId
                                    join applicant in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on appDtl.Id equals applicant.CircularID

                                    where (appInt.DesignationId == appDtl.DesignationId
                                    && applicant.RollNo >= model.FromRollNo && applicant.RollNo <= model.ToRollNo
                                    && appInt.Id == entity.Id
                                    )
                                    select new ApplicantInterviewCardIssueViewModel()
                                    {
                                        EmailId = applicant.Email,
                                        PhoneNo = applicant.MobilePhoneNo
                                    }).ToList();
                        if(list.Count() > 0)
                            SendEmail(list,model);
                    }

                    if (model.IsSms)
                    {
                        // Write Code Here
                        var list = (from appInt in _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetAll()
                                    join appDtl in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll() on appInt.AdvertisementInfoId equals appDtl.JobAdvertisementInfoId
                                    join applicant in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on appDtl.Id equals applicant.CircularID

                                    where (appInt.DesignationId == appDtl.DesignationId
                                    && applicant.RollNo >= model.FromRollNo && applicant.RollNo <= model.ToRollNo
                                    && appInt.Id == model.Id
                                    )
                                    select new ApplicantInterviewCardIssueViewModel()
                                    {
                                        EmailId = applicant.Email,
                                        PhoneNo = applicant.MobilePhoneNo
                                    }).ToList();
                    }
                    if(model.strMode == "E")
                        return RedirectToAction("Edit", new { id = model.Id, type = "success" });

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
                model.IsError = 1;
                model.errClass = "failed";
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                return View(model);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int Id, string type)
        {
            var entity = _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetByID(Id);
            if (entity.IsIssue)
            {
                return RedirectToAction("Index");
            }

            var model = entity.ToModel();
            model.InterviewDate = entity.InterviewDateAndTime;
            model.strMode = "Edit";
            model.IsInEditMode = true;
            model.IsChecked = true;

            #region Signatory
            var emp = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == model.SignatoryICNo).FirstOrDefault();
            model.EmpId = emp.EmpID;
            model.Name = emp.FullName;
            model.Designation = emp.PRM_Designation.Name;
            #endregion

            #region Designation

            var ddlList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().Where(x=>x.Id==model.DesignationId).OrderBy(x => x.Name).ToList();
            model.DesignationNameList = Common.PopulateDllList(ddlList);

            #endregion

            #region ExamType

            var exam = _prmCommonService.PRMUnit.SelectionCritariaOrExamTypeRepository.GetAll().Where(x => x.Id == model.SelectionCriteriaOrExamTypeId).OrderBy(x => x.Name).ToList();
            model.SelectionCriteriaList = Common.PopulateDllList(exam);

            #endregion

            #region Other
            //var obj = _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll().Where(x => x.DesignationId == model.DesignationId).Where( x => x.JobAdvertisementInfoId == model.AdvertisementInfoId).FirstOrDefault();
            //model.DepartmentName = obj.PRM_Division.Name;
            //model.SectionName =obj.SectionId==null? string.Empty:obj.PRM_Section.Name;
            //model.NoOfPost = obj.NumberOfClearancePosition;
            var item = _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll().Where(x => x.DesignationId == model.DesignationId && x.JobAdvertisementInfoId == model.AdvertisementInfoId).FirstOrDefault();
            model.DepartmentName = item.PRM_Division == null ? string.Empty : item.PRM_Division.Name;
            model.SectionName = item.SectionId == null ? string.Empty : item.PRM_Section.Name;
            model.NoOfPost = item.NumberOfPosition;

            #endregion

            //model.JobAdvertisementCode = entity.PRM_JobAdvertisementInfo.AdCode;

            //applicant shortlist Detail
            var list = (from appInt in _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetAll()
                        //join appIntDtl in _prmCommonService.PRMUnit.ApplicantInterviewCardIssueDetailRepository.GetAll() on appInt.Id equals appIntDtl.ApplicantInterviewCardIssueId
                        join appDtl in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll() on appInt.AdvertisementInfoId equals appDtl.JobAdvertisementInfoId 
                        join applicant in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on appDtl.Id equals applicant.CircularID

                        where (appInt.DesignationId == appDtl.DesignationId 
                        && applicant.RollNo >= model.FromRollNo && applicant.RollNo <= model.ToRollNo
                        && appInt.Id == model.Id
                        )
                        select new ApplicantInterviewCardIssueViewModel()
                        {
                            //Id = appIntDtl.Id,
                            ApplicantInfoId = applicant.intPK,
                            RollNo = applicant.RollNo,
                            ApplicantName = applicant.Name,
                            FatherName = applicant.FathersName,
                            DateOfBirth = applicant.DateOfBirth.ToString("dd-MM-yyyy"),
                            DesignationName = appDtl.PRM_Designation.Name,
                            NID = applicant.NationalId,
                            IsCheckedFinal = true
                        }).ToList();
            model.ApplicantInformationListDetail = list;

            #region Previous
            ////Job post information          
            //var jobPostList = (from appInt in _prmCommonService.PRMUnit.ApplicantInterviewCardIssueDetailRepository.GetAll()
            //                   join appInfo in _prmCommonService.PRMUnit.ApplicantInfoRepository.GetAll() on appInt.ApplicantInfoId equals appInfo.Id
            //                   join jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll() on appInfo.JobAdvertisementInfoId equals jobAd.Id
            //                   join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
            //                   join dept in _prmCommonService.PRMUnit.DivisionRepository.GetAll() on jobAdReq.DepartmentId equals dept.Id
            //                   join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on jobAdReq.DesignationId equals des.Id
            //                   join sec in _prmCommonService.PRMUnit.SectionRepository.GetAll() on jobAdReq.SectionId equals sec.Id
            //                   where (appInt.ApplicantInterviewCardIssueId == Id && appInfo.DesignationId==jobAdReq.DesignationId)
            //                   select new ApplicantInterviewCardIssueViewModel()
            //                   {
            //                       DesignationId = des.Id,
            //                       DesignationName = des.Name,
            //                       DepartmentId = dept.Id,
            //                       DepartmentName = dept.Name,
            //                       SectionId = sec.Id,
            //                       SectionName = sec.Name,
            //                       NoOfPost = jobAdReq.NumberOfPosition,
            //                       IsChecked = true,
            //                       strMode = "Edit"
            //                   }).ToList();

            //model.JobPostInformationList = jobPostList;
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

            var tempPeriod = _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetByID(id);
            if (tempPeriod.IsIssue)
            {
                return Json(new
                {
                    Message = "Sorry! Interview Card Already Issue."
                }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(PRM_ApplicantInterviewCardIssueDetail) };

                    _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.Delete(tempPeriod.Id, allTypes);
                    _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.SaveChanges();
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
                _prmCommonService.PRMUnit.ApplicantInterviewCardIssueDetailRepository.Delete(Id);
                _prmCommonService.PRMUnit.ApplicantInterviewCardIssueDetailRepository.SaveChanges();
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
        public string BusinessLogicValidation(ApplicantInterviewCardIssueViewModel model)
        {
            string errorMessage = string.Empty;
            if (model.strMode != "Edit")
            {
                var requInfo = _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetAll().Where(x => x.ReferenceNo == model.ReferenceNo).ToList();
                if (requInfo.Count > 0)
                {
                    return errorMessage = "Reference No. Already Exist";
                }
                if (model.ApplicantInformationListDetail.Count <= 0)
                {
                    return errorMessage = "Sorry! There is no Applicant list to Save.";
                }
                foreach (var item in model.ApplicantInformationListDetail)
                {
                    if (item.IsCheckedFinal)
                    {
                        var getRoll = _prmCommonService.PRMUnit.ApplicantInterviewCardIssueDetailRepository.GetAll().Where(x => x.RollNo == item.RollNo).ToList();
                        var examType = _prmCommonService.PRMUnit.ApplicantInterviewCardIssueDetailRepository.GetAll().Where(x => x.RollNo == item.SelectionCriteriaOrExamTypeId).ToList();
                        var appInfoId = _prmCommonService.PRMUnit.ApplicantInterviewCardIssueDetailRepository.GetAll().Where(x => x.RollNo == item.ApplicantInfoId).ToList();

                        if (getRoll.Count > 0 && examType.Count > 0 && appInfoId.Count > 0)
                        {
                            return errorMessage = "Employee Name's " + item.ApplicantName + " Roll No." + item.RollNo + " Already Exist";
                        }
                    }

                }
            }
            return errorMessage;

        }
        private PRM_ApplicantInterviewCardIssue CreateEntity([Bind(Exclude = "Attachment")] ApplicantInterviewCardIssueViewModel model, bool pAddEdit)
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
                var prm_ApplicantInterviewCardIssueDetail = new PRM_ApplicantInterviewCardIssueDetail();

                if (c.IsCheckedFinal)
                {
                    prm_ApplicantInterviewCardIssueDetail.Id = c.Id;
                    prm_ApplicantInterviewCardIssueDetail.ApplicantInfoId=c.ApplicantInfoId;
                    prm_ApplicantInterviewCardIssueDetail.RollNo =(int)c.RollNo;
                    prm_ApplicantInterviewCardIssueDetail.IUser = User.Identity.Name;
                    prm_ApplicantInterviewCardIssueDetail.IDate = DateTime.Now;

                    if (pAddEdit)
                    {
                        prm_ApplicantInterviewCardIssueDetail.IUser = User.Identity.Name;
                        prm_ApplicantInterviewCardIssueDetail.IDate = DateTime.Now;
                        entity.PRM_ApplicantInterviewCardIssueDetail.Add(prm_ApplicantInterviewCardIssueDetail);
                    }
                    else
                    {
                        prm_ApplicantInterviewCardIssueDetail.ApplicantInterviewCardIssueId = model.Id;
                        prm_ApplicantInterviewCardIssueDetail.EUser = User.Identity.Name;
                        prm_ApplicantInterviewCardIssueDetail.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                            var requInfo = _prmCommonService.PRMUnit.ApplicantInterviewCardIssueDetailRepository.GetAll().Where(x => x.ApplicantInfoId == c.ApplicantInfoId).ToList();
                            if (requInfo.Count == 0)
                            {
                                _prmCommonService.PRMUnit.ApplicantInterviewCardIssueDetailRepository.Add(prm_ApplicantInterviewCardIssueDetail);
                            }
                        }
                        else
                        {
                            _prmCommonService.PRMUnit.ApplicantInterviewCardIssueDetailRepository.Update(prm_ApplicantInterviewCardIssueDetail);

                        }
                    }
                    _prmCommonService.PRMUnit.ApplicantInterviewCardIssueDetailRepository.SaveChanges();
                }
            }
            #endregion

            return entity;
        }

        #region Previous
        ////get jobpost by job advertisement id
        //[HttpGet]
        //public PartialViewResult GetJobPost(int jobAdId)
        //{
        //    List<ApplicantInterviewCardIssueViewModel> list = new List<ApplicantInterviewCardIssueViewModel>();

        //    list = (from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll()
        //            join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
        //            join dept in _prmCommonService.PRMUnit.DivisionRepository.GetAll() on jobAdReq.DepartmentId equals dept.Id
        //            join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on jobAdReq.DesignationId equals des.Id
        //            join sec in _prmCommonService.PRMUnit.SectionRepository.GetAll() on jobAdReq.SectionId equals sec.Id
        //            where (jobAd.Id == jobAdId)
        //            select new ApplicantInterviewCardIssueViewModel
        //            {
        //                DesignationId = des.Id,
        //                DesignationName = des.Name,
        //                DepartmentId = dept.Id,
        //                DepartmentName = dept.Name,
        //                SectionId = sec.Id,
        //                SectionName = sec.Name,
        //                NoOfPost = jobAdReq.NumberOfPosition
        //            }).ToList();

        //    return PartialView("_JobPost", new ApplicantInterviewCardIssueViewModel { JobPostInformationList = list });

        //}
        #endregion

        //get applicant info by jobpost(designation) id
        [HttpPost]
        public PartialViewResult AddedApplicantInfo(List<ApplicantInterviewCardIssueDetailViewModel> jobPosts, string strMode, 
            int jobAdInfoId, int rollNoStartFrom, int rollNoTo, int examTypeId)
        {
            var model = new ApplicantInterviewCardIssueViewModel();

            List<ApplicantInterviewCardIssueViewModel> AssignmentList = new List<ApplicantInterviewCardIssueViewModel>();
            if (jobPosts != null)
            {
                List<EREC_tblgeneralinfo> list = new List<EREC_tblgeneralinfo>();

                #region Selected Applicant Info
                var selectedAppList = (from selLst in _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.GetAll().Where(x => x.JobAdvertisementInfoId == jobAdInfoId)
                                       join selLstDtl in _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.GetAll() on selLst.Id equals selLstDtl.SelectedApplicantInfoId
                                       join applicantInfo in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll().Where(x => jobPosts.Select(n => n.DesignationId).Contains(Convert.ToInt32( x.DesignationId))) on selLstDtl.ApplicantInfoId equals applicantInfo.intPK
                                       select selLst).ToList();
                
                if(selectedAppList.Count>0)
                {
                    foreach (var app in selectedAppList)
                    {
                        if (app.IsFinalExam == true)
                        {
                            return PartialView("_Details", model);
                        }
                    }

                    list = (from selLst in _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.GetAll().Where(x => x.JobAdvertisementInfoId == jobAdInfoId)
                            join selLstDtl in _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.GetAll().Where(x => x.SelectedForNextExamId == examTypeId) on selLst.Id equals selLstDtl.SelectedApplicantInfoId
                            join applicantInfo in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on selLstDtl.ApplicantInfoId equals applicantInfo.intPK
                            select applicantInfo).Where(x => jobPosts.Select(n => n.DesignationId).Contains( Convert.ToInt32( x.DesignationId))).ToList();
                }
                else
                {
                         list = (from postList in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll()
                                 join applicantInfo in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on postList.Id equals applicantInfo.CircularID
                                 where postList.JobAdvertisementInfoId == jobAdInfoId && applicantInfo.RollNo >= rollNoStartFrom && applicantInfo.RollNo <= rollNoTo
                                select applicantInfo).Where(x => jobPosts.Select(n => n.DesignationId).Contains(Convert.ToInt32(x.DesignationId))).ToList();
                }
                #endregion


               // int rollNo = rollNoStartFrom;

                foreach (var vmApplicant in list)
                {
                    var dupList = _prmCommonService.PRMUnit.ApplicantInterviewCardIssueDetailRepository.GetAll().Where(x => x.ApplicantInfoId == vmApplicant.intPK).ToList();   // for checking duplicate
                   
                    var jobPost = _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll().Where(e => e.Id == vmApplicant.CircularID).FirstOrDefault();

                    if (strMode == "Create")
                    {
                        
                        if (dupList.Count == 0)
                        {
                            
                            var gridModel = new ApplicantInterviewCardIssueViewModel
                            {
                                ApplicantInfoId = vmApplicant.intPK,
                                ApplicantName = vmApplicant.Name,
                                FatherName = vmApplicant.FathersName,
                                DateOfBirth = vmApplicant.DateOfBirth.ToString("dd-MM-yyyy"),
                                NID = vmApplicant.NationalId,
                                DesignationName = jobPost.PRM_Designation.Name,
                                RollNo = vmApplicant.RollNo

                            };
                            //rollNo++;
                            AssignmentList.Add(gridModel);
                        }
                        //else
                        //{
                        //    foreach (var item in dupList)
                        //    {
                        //        rollNo = item.RollNo;
                        //    }
                        //    var gridModel = new ApplicantInterviewCardIssueViewModel
                        //    {
                        //        ApplicantInfoId = vmApplicant.Id,
                        //        ApplicantName = vmApplicant.ApplicantNameE,
                        //        FatherName = vmApplicant.FatherName,
                        //        DateOfBirth = vmApplicant.DateOfBirth.ToString("dd-MM-yyyy"),
                        //        NID = vmApplicant.NationalID,
                        //        DesignationName = vmApplicant.PRM_Designation.Name,
                        //        RollNo=rollNo
                        //    };
                        //    AssignmentList.Add(gridModel);

                        //}
                    }
                    else
                    {
                        if (dupList.Count != 0)
                        {
                            //foreach (var item in dupList)
                            //{
                            //    rollNo = item.RollNo;
                            //}
                            var gridModel = new ApplicantInterviewCardIssueViewModel
                            {
                                ApplicantInfoId = vmApplicant.intPK,
                                ApplicantName = vmApplicant.Name,
                                FatherName = vmApplicant.FathersName,
                                DateOfBirth = vmApplicant.DateOfBirth.ToString("dd-MM-yyyy"),
                                NID = vmApplicant.NationalId,
                                DesignationName = jobPost.PRM_Designation.Name,
                                RollNo = vmApplicant.RollNo,
                                IsCheckedFinal = true,

                            };
                            AssignmentList.Add(gridModel);
                        }
                        else
                        {
                            var gridModel = new ApplicantInterviewCardIssueViewModel
                            {
                                ApplicantInfoId = vmApplicant.intPK,
                                ApplicantName = vmApplicant.Name,
                                FatherName = vmApplicant.FathersName,
                                DateOfBirth = vmApplicant.DateOfBirth.ToString("dd-MM-yyyy"),
                                NID = vmApplicant.NationalId,
                                DesignationName = jobPost.PRM_Designation.Name,
                                IsCheckedFinal = false,
                                RollNo = vmApplicant.RollNo
                            };
                            AssignmentList.Add(gridModel);

                        }

                    }
                }

                model.ApplicantInformationListDetail = AssignmentList;
            }
            return PartialView("_Details", model);
        }

        #region Populate Dropdown
        private void populateDropdown(ApplicantInterviewCardIssueViewModel model)
        {
            dynamic ddlList;

            #region job advertisement

            ddlList = _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.AdCode).ToList();
            model.AdvertisementCodeList = Common.PopulateJobAdvertisementDDL(ddlList);

            #endregion


        }

        #endregion

        [HttpPost]
        public ActionResult GetInterviewCard(int id, int? rollNo)
        {
            var obj = _prmCommonService.PRMUnit.ApplicantInfoRepository.GetByID(id);
            var model = obj.ToModel();
            model.RollNo = rollNo.ToString();
            var ZoneInfo = _prmCommonService.PRMUnit.ZoneInfoRepository.GetByID(model.ZoneInfoId);
            model.ZoneName = ZoneInfo.ZoneName;
            model.ZoneAddress = ZoneInfo.ZoneAddress;
            DownloadDoc(model);
            model.IsAddAttachment = true;
            return PartialView("_InterviewCard", model);
        }



        public ActionResult SelectJobPostType(int jobAdInfoId)
        {
           //var tempList = (from jobAdRe in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll()
           //                 join jobadInfo in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll() on jobAdRe.JobAdvertisementInfoId equals jobadInfo.Id
           //                 select jobAdRe).Where(x => x.JobAdvertisementInfoId == jobAdInfoId).ToList();

            var designations = (from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Fetch()
                                join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.Fetch() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                join des in _prmCommonService.PRMUnit.DesignationRepository.Fetch() on jobAdReq.DesignationId equals des.Id
                                where (jobAd.Id == jobAdInfoId)
                                select des).Concat(from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Fetch()
                                                   join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.Fetch() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                                   join des in _prmCommonService.PRMUnit.DesignationRepository.Fetch() on jobAdReq.DesignationId equals des.Id
                                                   where (jobAd.Id == jobAdInfoId)
                                                   select des).OrderBy(o => o.SortingOrder).ToList();

            var list = designations.Select(x => new { Id = x.Id, Name = x.Name }).ToList();

          //  var list = tempList.Select(x => new { Id = x.PRM_Designation.Id, Name = x.PRM_Designation.Name }).DistinctBy(x => x.Id).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult SelectionCriteriaOrExamType(int jobAdInfoId, int designationId)
        {
            var tempList = (from selectionCri in _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll()
                            join selectionCriDtl in _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.GetAll() on selectionCri.Id equals selectionCriDtl.SelectionCriteriaId
                            select selectionCriDtl).Where(x => x.PRM_SelectionCriteria.DesignationId == designationId).Where(x => x.PRM_SelectionCriteria.JobAdvertisementInfoId == jobAdInfoId).ToList();


            var list = tempList.Select(x => new { Id = x.PRM_SelectionCritariaOrExamType.Id, Name = x.PRM_SelectionCritariaOrExamType.Name }).OrderBy(x => x.Name).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
        }
         [NoCache]
        public JsonResult GetRequsionInfo(int designationId, int jobAdInfoId)
        {
            var obj = _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll().Where(x => x.DesignationId == designationId && x.JobAdvertisementInfoId == jobAdInfoId).FirstOrDefault();
            if (obj != null)
            {
                return Json(new
                {
                    department = obj.PRM_Division == null ? string.Empty : obj.PRM_Division.Name,
                    section = obj.SectionId == null ? string.Empty : obj.PRM_Section.Name,
                    noOfPost = obj.NumberOfClearancePosition
                });
            }
            else
            {
                var item = _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll().Where(x => x.DesignationId == designationId && x.JobAdvertisementInfoId == jobAdInfoId).FirstOrDefault();
              return Json(new
              {
                  department = item.PRM_Division == null ? string.Empty : item.PRM_Division.Name,
                  section = item.SectionId == null ? string.Empty : item.PRM_Section.Name,
                  noOfPost = item.NumberOfPosition
              });

            }

        }

         [NoCache]
         public JsonResult GetIsCardIssue(int jobAdInfoId, int designationId, int examTypeId)
         {
             var isIssue = 0;
             var obj = _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetAll()
                       .Where(x => x.DesignationId == designationId && x.AdvertisementInfoId==jobAdInfoId && x.SelectionCriteriaOrExamTypeId== examTypeId && x.IsIssue==true).FirstOrDefault();

             if (obj != null)
             {
                 isIssue = 1;
             }

             return Json(new
             {
                 IsIssue = isIssue
             });

         }

         #region Attachment Applicant Photo

         private int Upload(ApplicantInfoViewModel model)
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

         public void DownloadDoc(ApplicantInfoViewModel model)
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

         public void SendEmail(List<ApplicantInterviewCardIssueViewModel> model, ApplicantInterviewCardIssueViewModel emailSubject)
         {
             string FromEmailAddress = EmailConfig.MailSender;
             string FromEmailpassword = EmailConfig.MailSenderPassword;
             //string _Msg = emailSubject.Body;
           
             var loginInfo = new NetworkCredential(FromEmailAddress, FromEmailpassword);
             var msg = new MailMessage();

             var smtpClient = new SmtpClient(EmailConfig.MailServer);

             msg.From = new MailAddress(FromEmailAddress);
             foreach (var item in model)
             {
                 msg.To.Add(new MailAddress(item.EmailId));
                // msg.Bcc.Add(item.EmailId);
             }

             msg.Subject = emailSubject.Subject;
             msg.Body = emailSubject.Body;
             msg.IsBodyHtml = true;

             smtpClient.EnableSsl = false;
             smtpClient.UseDefaultCredentials = false;
             smtpClient.Credentials = loginInfo;

             // smtpClient.Credentials = (ICredentialsByHost)CredentialCache.DefaultNetworkCredentials;
             
             smtpClient.Send(msg);

         }


	}

    public static class EmailConfig
    {
        public static string MailSender { get { return System.Configuration.ConfigurationManager.AppSettings["mailSender"].ToString(); } }
        public static int SMTPPort { get { return int.Parse(System.Configuration.ConfigurationManager.AppSettings["SMTPPort"].ToString()); } }
        public static string MailServer { get { return System.Configuration.ConfigurationManager.AppSettings["mailServer"].ToString(); } }
        public static string MailSenderPassword { get { return System.Configuration.ConfigurationManager.AppSettings["mailSenderPassword"].ToString(); } }
    }

}