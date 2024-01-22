using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class RecruitmentQualificationInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonSevice;
        #endregion

        #region Constructor

        public RecruitmentQualificationInfoController(PRMCommonSevice prmCommonSevice)
        {
            this._prmCommonSevice = prmCommonSevice;
        }

        #endregion

        public ViewResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, RecruitmentQualificationInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<RecruitmentQualificationInfoViewModel> list = (from rec in _prmCommonSevice.PRMUnit.RecruitmentQualificationRepository.GetAll()
                                                                join dep in _prmCommonSevice.PRMUnit.DivisionRepository.GetAll() on rec.DepartmentId equals dep.Id 
                                                                join des in _prmCommonSevice.PRMUnit.DesignationRepository.GetAll() on rec.DesignationId equals des.Id
                                                                where (model.DepartmentId == 0 ||  model.DepartmentId ==rec.DepartmentId)
                                                                && (model.DesignationId == 0 || model.DesignationId == rec.DesignationId)
                                                                &&(rec.ZoneInfoId==LoggedUserZoneInfoId)
                                                                select new RecruitmentQualificationInfoViewModel()
                                                                  {
                                                                      Id = rec.Id,
                                                                      DepartmentId=dep.Id,
                                                                      DepartmentName=dep.Name,
                                                                      DesignationId=des.Id,
                                                                      DesignationName=des.Name,
                                                                      Recruitment = rec.IsNewRecruitment.ToString() == "True" ? "New Recruitment" : "Promotion",
                                                                  }).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "DepartmentName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DepartmentName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DepartmentName).ToList();
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
            if (request.SortingName == "Recruitment")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Recruitment).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Recruitment).ToList();
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
                  d.DepartmentId,
                  d.DepartmentName,
                  d.DesignationId,
                  d.DesignationName,
                  d.Recruitment
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }
        [NoCache]
        public ActionResult DepartmentforView()
        {
            var itemList = Common.PopulateDllList(_prmCommonSevice.PRMUnit.DivisionRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList());
            return PartialView("Select", itemList);
        }
        [NoCache]
        public ActionResult DesignationforView()
        {
            var itemList = Common.PopulateDllList(_prmCommonSevice.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", itemList);
        }
        public ActionResult Create()
        {
            RecruitmentQualificationInfoViewModel model = new RecruitmentQualificationInfoViewModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(RecruitmentQualificationInfoViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;

            if (ModelState.IsValid)
            {
                model.ZoneInfoId = LoggedUserZoneInfoId;
                if (model.Recruitment == "True") model.IsNewRecruitment = true;

                var entity = CreateEntity(model, true);
                try
                {
                    _prmCommonSevice.PRMUnit.RecruitmentQualificationRepository.Add(entity);
                    _prmCommonSevice.PRMUnit.RecruitmentQualificationRepository.SaveChanges();
                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    return RedirectToAction("Edit", new { id = entity.Id, type = "saveSuccess" });
                }
                catch
                {
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                }
            }
            return View(model);
        }
        private PRM_RecruitmentQualificationInfo CreateEntity(RecruitmentQualificationInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            foreach (var c in model.RecruitmentQualificationListDetails)
            {
                var prm_RecruitmentQualificationDetails = new PRM_RecruitmentQualificationDetails();
                prm_RecruitmentQualificationDetails.Id = c.Id;
                prm_RecruitmentQualificationDetails.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                prm_RecruitmentQualificationDetails.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                prm_RecruitmentQualificationDetails.EUser = c.EUser;
                prm_RecruitmentQualificationDetails.EDate = c.EDate;
                prm_RecruitmentQualificationDetails.Condition = c.Condition;

                if (pAddEdit)
                {
                    prm_RecruitmentQualificationDetails.IUser = User.Identity.Name;
                    prm_RecruitmentQualificationDetails.IDate = DateTime.Now;

                    entity.PRM_RecruitmentQualificationDetails.Add(prm_RecruitmentQualificationDetails);
                }
                else
                {
                    prm_RecruitmentQualificationDetails.RecruitmentId = model.Id;

                    prm_RecruitmentQualificationDetails.EUser = User.Identity.Name;
                    prm_RecruitmentQualificationDetails.EDate = DateTime.Now;


                    if (c.Id == 0)
                    {
                        _prmCommonSevice.PRMUnit.RecruitmentQualificationDetailRepository.Add(prm_RecruitmentQualificationDetails);
                    }
                    else
                    {
                        _prmCommonSevice.PRMUnit.RecruitmentQualificationDetailRepository.Update(prm_RecruitmentQualificationDetails);

                    }
                    _prmCommonSevice.PRMUnit.RecruitmentQualificationDetailRepository.SaveChanges();
                }
            }

            return entity;
        }

        public ActionResult AddRecruitmentQualificationInfo(int id)
        {
            RecruitmentQualificationDetailsViewModel model = new RecruitmentQualificationDetailsViewModel();
            model.Id = id;
            var educationList = (from eduDtl in _prmCommonSevice.PRMUnit.RecruitmentEducationalQualificationRepository.GetAll()
                                 where (eduDtl.RecruitmentQualificationDetailsId == id)
                                 select new RecruitmentQualificationEducationInfoViewModel
                                 {
                                     Id = eduDtl.Id,
                                     DegreeTypeId = eduDtl.DegreeTypeId,
                                     DegreeLevelId =eduDtl.DegreeLevelId ==null ? null :  eduDtl.DegreeLevelId,
                                     DivisionOrGradeId=eduDtl.DivisionOrGradeId == null ? null :  eduDtl.DivisionOrGradeId,
                                     GPAOrCGPA =eduDtl.GPAOrCGPA ==null ? null :  eduDtl.GPAOrCGPA,
                                     DegreeType = eduDtl.PRM_DegreeType == null ? string.Empty : eduDtl.PRM_DegreeType.Name,
                                     DegreeLevel = eduDtl.PRM_DegreeLevel == null ? string.Empty : eduDtl.PRM_DegreeLevel.Name,
                                     DivisionOrGrade =eduDtl.PRM_AcademicGrade==null ?string.Empty : eduDtl.PRM_AcademicGrade.Name
                                 }).ToList();
            IList<RecruitmentQualificationEducationInfoViewModel> finalEdulist = new List<RecruitmentQualificationEducationInfoViewModel>();
            foreach (var item in educationList)
            {
                var subject = _prmCommonSevice.PRMUnit.RecruitmentSubjectOrGroupRepository.GetAll().Where(q => q.RecruitmentEducationalQualificationId == item.Id).ToList();
                var sunjectName = _prmCommonSevice.PRMUnit.RecruitmentSubjectOrGroupRepository.GetAll().Where(q => q.RecruitmentEducationalQualificationId == item.Id)
                                  .Join(_prmCommonSevice.PRMUnit.SubjectGroupRepository.GetAll(), rQ => rQ.SubjectOrGroupId,
                                  rQD => rQD.Id, (rQ, rQD) => new { rQ, rQD }).Select(s=>s.rQD).ToList();

                              var sub =new RecruitmentQualificationEducationInfoViewModel
                                 {
                                     Id = item.Id,
                                     DegreeTypeId = item.DegreeTypeId,
                                     DegreeLevelId = item.DegreeLevelId,
                                     DivisionOrGradeId=item.DivisionOrGradeId,
                                     GPAOrCGPA = item.GPAOrCGPA,
                                     DegreeType = item.DegreeType,
                                     DegreeLevel = item.DegreeLevel,
                                     DivisionOrGrade = item.DivisionOrGrade,
                                     SubjectOrGroupId = string.Join(",", subject.Select(t => t.SubjectOrGroupId).ToArray()),
                                     SubjectOrGroup = string.Join(",", sunjectName.Select(t => t.Name).ToArray())
                                 };
                              finalEdulist.Add(sub);
            }

            var jobexpList = (from jobDtl in _prmCommonSevice.PRMUnit.RecruitmentJobExperienceRepository.GetAll()
                              where (jobDtl.RecruitmentQualificationDetailsId == id)
                                 select new RecruitmentQualificationJobExpInfoViewModel
                                 {
                                     Id = jobDtl.Id,
                                     YearOfExp = jobDtl.YearOfExp,
                                     OnBy = jobDtl.OnBy,
                                     JobGradeId = jobDtl.JobGradeId,
                                     PostId = jobDtl.PostId,
                                     ProfessionalCertificateId = jobDtl.ProfessionalCertificateId,
                                     OnType = jobDtl.PRM_Designation != null ? jobDtl.PRM_Designation.Name : (jobDtl.PRM_ProfessionalCertificate != null ? jobDtl.PRM_ProfessionalCertificate.Name: jobDtl.PRM_JobGrade.PayScale),
                                     TotalYearOfExp =jobDtl.TotalYearOfExp,
                                     Remarks= jobDtl.Remarks
                                 }).ToList();

            model.RecruitmentQualificationEducationList = finalEdulist;
            model.RecruitmentQualificationJobExpList = jobexpList;
            populateDropdown(model);
            return View("AddRecruitmentQualification", model);
        }

        [HttpPost]
        public ActionResult AddRecruitmentQualificationInfo(RecruitmentQualificationDetailsViewModel model)
        {
            var objAttach = _prmCommonSevice.PRMUnit.RecruitmentQualificationDetailRepository.Get(q => q.Id == model.Id).FirstOrDefault();

            try
            {
                    var educationList = model.RecruitmentQualificationEducationList.ToList();
                    var jobExpList = model.RecruitmentQualificationJobExpList.ToList();

                    var educationEntity = new List<PRM_RecruitmentEducationalQualification>();
                    var jobExpEntity = new List<PRM_RecruitmentJobExperience>();
                    var subjectEntity = new List<PRM_RecruitmentSubjectOrGroup>();

                    foreach(var item in educationList){
                        var prm_RecruitmentEducationalQualification = new PRM_RecruitmentEducationalQualification();

                        prm_RecruitmentEducationalQualification.Id = item.Id;
                        prm_RecruitmentEducationalQualification.RecruitmentQualificationDetailsId = model.Id;
                        prm_RecruitmentEducationalQualification.DegreeTypeId = item.DegreeTypeId;
                        prm_RecruitmentEducationalQualification.DegreeLevelId = item.DegreeLevelId;
                        prm_RecruitmentEducationalQualification.DivisionOrGradeId = item.DivisionOrGradeId;
                        prm_RecruitmentEducationalQualification.GPAOrCGPA = item.GPAOrCGPA;
                        prm_RecruitmentEducationalQualification.IUser = User.Identity.Name;
                        prm_RecruitmentEducationalQualification.IDate = DateTime.Now;
                        if (item.Id == 0)
                        {
                            _prmCommonSevice.PRMUnit.RecruitmentEducationalQualificationRepository.Add(prm_RecruitmentEducationalQualification);
                        }
                        else
                        {
                            _prmCommonSevice.PRMUnit.RecruitmentEducationalQualificationRepository.Update(prm_RecruitmentEducationalQualification);
                        }
                        if (item.SubjectOrGroupId != null && item.SubjectOrGroupId != string.Empty && item.SubjectOrGroupId != "null")
                        {
                            var subject = item.SubjectOrGroupId.Split(',').ToArray();
                            foreach (var sub in subject)
                            {
                                var prm_RecruitmentSubjectOrGroup = new PRM_RecruitmentSubjectOrGroup();
                                prm_RecruitmentSubjectOrGroup.RecruitmentEducationalQualificationId = prm_RecruitmentEducationalQualification.Id;
                                prm_RecruitmentSubjectOrGroup.SubjectOrGroupId =Convert.ToInt32(sub);
                                prm_RecruitmentSubjectOrGroup.IUser = User.Identity.Name;
                                prm_RecruitmentSubjectOrGroup.IDate = DateTime.Now;
                                if (item.Id == 0)
                                {
                                    _prmCommonSevice.PRMUnit.RecruitmentSubjectOrGroupRepository.Add(prm_RecruitmentSubjectOrGroup);
                                }
                                else
                                {
                                    var subgroId = _prmCommonSevice.PRMUnit.RecruitmentSubjectOrGroupRepository.Get(x => x.RecruitmentEducationalQualificationId == item.Id && x.SubjectOrGroupId == prm_RecruitmentSubjectOrGroup.SubjectOrGroupId).Select(s => s.Id).FirstOrDefault();
                                    if ( subgroId!=0)
                                    {
                                        prm_RecruitmentSubjectOrGroup.Id = subgroId;
                                        _prmCommonSevice.PRMUnit.RecruitmentSubjectOrGroupRepository.Update(prm_RecruitmentSubjectOrGroup);
                                    }
                                }
                            }
                        }
                    }

                    foreach (var job in jobExpList)
                    {
                        var prm_RecruitmentJobExperience = new PRM_RecruitmentJobExperience();
                        prm_RecruitmentJobExperience.Id = job.Id;
                        prm_RecruitmentJobExperience.RecruitmentQualificationDetailsId = model.Id;
                        prm_RecruitmentJobExperience.YearOfExp = job.YearOfExp;
                        prm_RecruitmentJobExperience.OnBy = job.OnBy;
                        prm_RecruitmentJobExperience.JobGradeId = job.JobGradeId;
                        prm_RecruitmentJobExperience.PostId = job.PostId;
                        prm_RecruitmentJobExperience.ProfessionalCertificateId = job.ProfessionalCertificateId;
                        prm_RecruitmentJobExperience.TotalYearOfExp = job.TotalYearOfExp;
                        prm_RecruitmentJobExperience.Remarks = job.Remarks;
                        prm_RecruitmentJobExperience.IUser = User.Identity.Name;
                        prm_RecruitmentJobExperience.IDate = DateTime.Now;
                        if (job.Id == 0)
                        {
                            _prmCommonSevice.PRMUnit.RecruitmentJobExperienceRepository.Add(prm_RecruitmentJobExperience);
                        }
                        else
                        {
                            _prmCommonSevice.PRMUnit.RecruitmentJobExperienceRepository.Update(prm_RecruitmentJobExperience);
                        }
                    }
                    _prmCommonSevice.PRMUnit.RecruitmentEducationalQualificationRepository.SaveChanges();
                    _prmCommonSevice.PRMUnit.RecruitmentSubjectOrGroupRepository.SaveChanges();
                    _prmCommonSevice.PRMUnit.RecruitmentJobExperienceRepository.SaveChanges();
            }
            catch
            {
                return RedirectToAction("Edit", "RecruitmentQualificationInfo", new { id = objAttach.RecruitmentId });
            }

            return RedirectToAction("Edit", "RecruitmentQualificationInfo", new { id = objAttach.RecruitmentId, type = "success"});
        }

        [NoCache]
        public ActionResult Edit(int Id, string type)
        {
           
            var entity = _prmCommonSevice.PRMUnit.RecruitmentQualificationRepository.GetByID(Id);
            var model = entity.ToModel();
            if (model.IsNewRecruitment == true)
            {
                model.Recruitment = "True";
            }
            else
            {
                model.Recruitment = "False";
            }
            model.OrganogramLevelName = entity.PRM_OrganogramLevel == null ? String.Empty : entity.PRM_OrganogramLevel.LevelName;

            var detEntity=(from  reqDtl in _prmCommonSevice.PRMUnit.RecruitmentQualificationDetailRepository.GetAll()
                           where (reqDtl.RecruitmentId == Id)
                           select new RecruitmentQualificationDetailsViewModel
                           {
                               Id = reqDtl.Id,
                               RecruitmentId=reqDtl.RecruitmentId,
                               Condition=reqDtl.Condition,
                               DesignationName=reqDtl.PRM_RecruitmentQualificationInfo.PRM_Designation.Name,
                               OrganogramLevelName=reqDtl.PRM_RecruitmentQualificationInfo.PRM_OrganogramLevel.LevelName
                           }
                         ).ToList();
            model.Mode = "Edit";
            model.RecruitmentQualificationListDetails = detEntity;

            #region Designation
                var desList = _prmCommonSevice.PRMUnit.DesignationRepository.Fetch().Where(x => x.Id == model.DesignationId).OrderBy(x => x.Name).ToList();
                model.DesignationList = Common.PopulateDllList(desList);
                #endregion

            if (type == "success")
            {
                model.IsError = 1;
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            else if (type == "saveSuccess")
            {
                model.IsError = 1;
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(RecruitmentQualificationInfoViewModel model)
        {
            model.IsError = 1;
            model.ErrMsg = string.Empty;

            if (ModelState.IsValid && (string.IsNullOrEmpty(model.ErrMsg)))
            {
                model.ZoneInfoId = LoggedUserZoneInfoId;
                if (model.Recruitment == "True") model.IsNewRecruitment = true;
                try
                {
                     var entity = CreateEntity(model, false);
                    _prmCommonSevice.PRMUnit.RecruitmentQualificationRepository.Update(entity);
                    _prmCommonSevice.PRMUnit.RecruitmentQualificationRepository.SaveChanges();

                    return RedirectToAction("Edit", new { id = model.Id, type = "success" });
                }
                catch
                {
                    model.IsError = 0;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                }
            }
            else
            {
                model.IsError = 0;
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
            }
            return View(model);
        }

        #region Delete Confirm
        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _prmCommonSevice.PRMUnit.RecruitmentQualificationRepository.GetByID(id);
            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(PRM_RecruitmentQualificationDetails) };
                    _prmCommonSevice.PRMUnit.RecruitmentQualificationRepository.Delete(tempPeriod.Id, allTypes);
                    _prmCommonSevice.PRMUnit.RecruitmentQualificationRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteRecruitmentDetail")]
        public JsonResult DeleteRecruitmentDetailConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            try
            {
                _prmCommonSevice.PRMUnit.RecruitmentQualificationDetailRepository.Delete(id);
                _prmCommonSevice.PRMUnit.RecruitmentQualificationDetailRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
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

        [HttpPost, ActionName("DeleteRecruitmentJobExp")]
        public JsonResult DeleteRecruitmentJobExpConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            try
            {
                _prmCommonSevice.PRMUnit.RecruitmentJobExperienceRepository.Delete(id);
                _prmCommonSevice.PRMUnit.RecruitmentJobExperienceRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
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

        [HttpPost, ActionName("DeleteRecruitmentEducation")]
        public JsonResult DeleteRecruitmentEducationConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;
            var tempPeriod = _prmCommonSevice.PRMUnit.RecruitmentEducationalQualificationRepository.GetByID(id);
            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(PRM_RecruitmentSubjectOrGroup) };
                    _prmCommonSevice.PRMUnit.RecruitmentEducationalQualificationRepository.Delete(tempPeriod.Id, allTypes);
                    _prmCommonSevice.PRMUnit.RecruitmentEducationalQualificationRepository.SaveChanges();
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

        #endregion

        private void populateDropdown(RecruitmentQualificationDetailsViewModel model)
        {
            #region Degree Type
            var degTList = _prmCommonSevice.PRMUnit.DegreeTypeRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.DegreeTypeList = Common.PopulateDllList(degTList);
            #endregion

            #region Degree Level
            var degLList = _prmCommonSevice.PRMUnit.ExamDegreeLavelRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.DegreeLevelList = Common.PopulateDllList(degLList);
            #endregion

            #region Grade
            var gradeTList = _prmCommonSevice.PRMUnit.AcademicGradeRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.AcademicGradeList = Common.PopulateDllList(gradeTList);
            #endregion

            #region Salary Scale
            var maxSalaryScale = _prmCommonSevice.PRMUnit.SalaryScaleRepository.Get(t => t.DateOfEffective <= DateTime.Now).OrderByDescending(t => t.DateOfEffective).FirstOrDefault();
            if (maxSalaryScale != null)
            {
                var salaryList = _prmCommonSevice.PRMUnit.JobGradeRepository.Get(x => x.SalaryScaleId == maxSalaryScale.Id).ToList();
                var salist = new List<SelectListItem>();
                foreach (var item in salaryList)
                {
                    salist.Add(new SelectListItem()
                    {
                        Text = item.PayScale,
                        Value = item.Id.ToString()
                    });
                }

                model.PayScaleList = salist;
            }
            #endregion

            #region Post
            var PostList = _prmCommonSevice.PRMUnit.DesignationRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.PostNameList = Common.PopulateDllList(PostList);
            #endregion

            #region On
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Pay Scale", Value = "PayScale", Selected = true });
            list.Add(new SelectListItem() { Text = "Post", Value = "Post" });
            list.Add(new SelectListItem() { Text = "Professional Certificate", Value = "Certificate" });
            model.OnList = list;
            #endregion

            #region Principle Subject
            var PSubjectList = _prmCommonSevice.PRMUnit.SubjectGroupRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.SubjectGroupList = Common.PopulateDllList(PSubjectList);
            #endregion

            #region Certificate
            var Certi = _prmCommonSevice.PRMUnit.ProfessionalCertificateRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.ProfessionalCertificateList = Common.PopulateDllList(Certi);
            #endregion

        }

        #region Cascading DDL

        #region Loading Designation
        public ActionResult LoadDesignation(int departmentId, int? officeOrsectionId)
        {
            if (officeOrsectionId != null && officeOrsectionId != 0)
            {
                if (_prmCommonSevice.PRMUnit.SectionRepository.GetAll().Where(x => x.Id == officeOrsectionId).ToList().Count > 0)
                {
                    var tempList = (from sce in _prmCommonSevice.PRMUnit.SectionRepository.GetAll()
                                    join orgManP in _prmCommonSevice.PRMUnit.OrganizationalSetupManpowerInfoRepository.GetAll() on sce.Id equals orgManP.OrganogramLevelId
                                    join des in _prmCommonSevice.PRMUnit.DesignationRepository.GetAll() on orgManP.DesignationId equals des.Id
                                    where (sce.Id == officeOrsectionId)
                                    select des
                                  ).ToList();

                    var list = tempList.Select(x => new { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();

                    return Json(list, JsonRequestBehavior.AllowGet
                    );
                }
                else
                {
                    var tempList = (from Discip in _prmCommonSevice.PRMUnit.DisciplineRepository.GetAll()
                                    join orgManP in _prmCommonSevice.PRMUnit.OrganizationalSetupManpowerInfoRepository.GetAll() on Discip.Id equals orgManP.OrganogramLevelId
                                    join des in _prmCommonSevice.PRMUnit.DesignationRepository.GetAll() on orgManP.DesignationId equals des.Id
                                    where (Discip.Id == officeOrsectionId)
                                    select des
                                    ).ToList();

                    var list = tempList.Select(x => new { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();

                    return Json(list, JsonRequestBehavior.AllowGet
                    );

                }
            }
            else
            {
                var tempList = (from div in _prmCommonSevice.PRMUnit.DivisionRepository.GetAll()
                                join orgManP in _prmCommonSevice.PRMUnit.OrganizationalSetupManpowerInfoRepository.GetAll() on div.Id equals orgManP.OrganogramLevelId
                                join des in _prmCommonSevice.PRMUnit.DesignationRepository.GetAll() on orgManP.DesignationId equals des.Id
                                where (div.Id == departmentId)
                                select des
              ).ToList();

                var list = tempList.Select(x => new { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();

                return Json(list, JsonRequestBehavior.AllowGet
                );

            }
        }

        #endregion

        public ActionResult LoadOfficeSection(int departmentId)
        {
            var items = (from  orgLevel in _prmCommonSevice.PRMUnit.OrganogramLevelRepository.GetAll()
                         where orgLevel.ParentId == departmentId
                         select new
                         {
                             Id = orgLevel.Id,
                             Name = orgLevel.LevelName
                         }).ToList();

            return Json(items, JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult LoadDegreeLevel(int degreeTypeId)
        {
            if (degreeTypeId != 0)
            {
                var items = (from degLevel in _prmCommonSevice.PRMUnit.DegreeLevelMappingRepository.GetAll()
                             join degLblDtl in _prmCommonSevice.PRMUnit.DegreeLevelMappingDetailRepository.GetAll() on degLevel.Id equals degLblDtl.DegreeLevelMappingId
                             join degree in _prmCommonSevice.PRMUnit.ExamDegreeLavelRepository.GetAll() on degLblDtl.DegreeLevelId equals degree.Id
                             where degLevel.DegreeTypeId == degreeTypeId
                             select new
                             {
                                 Id = degree.Id,
                                 Name = degree.Name
                             }).ToList();

                return Json(items, JsonRequestBehavior.AllowGet
                );
            }
            else
            {
                var items = (from degLevel in _prmCommonSevice.PRMUnit.DegreeLevelMappingRepository.GetAll()
                             join degLblDtl in _prmCommonSevice.PRMUnit.DegreeLevelMappingDetailRepository.GetAll() on degLevel.Id equals degLblDtl.DegreeLevelMappingId
                             join degree in _prmCommonSevice.PRMUnit.ExamDegreeLavelRepository.GetAll() on degLblDtl.DegreeLevelId equals degree.Id
                             select new
                             {
                                 Id = degree.Id,
                                 Name = degree.Name
                             }).ToList();

                return Json(items, JsonRequestBehavior.AllowGet
                );
            }
        }
        #endregion

    }
}