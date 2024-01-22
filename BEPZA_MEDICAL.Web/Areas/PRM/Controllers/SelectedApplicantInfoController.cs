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
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class SelectedApplicantInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Constructor
        public SelectedApplicantInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion
        //
        // GET: /PRM/SelectedApplicantInfo/

        #region Index 
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SelectedApplicantInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<SelectedApplicantInfoViewModel> list = (from selApp in _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.GetAll()
                                                         join selAppDtl in _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.GetAll() on selApp.Id equals selAppDtl.SelectedApplicantInfoId
                                                         join jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll() on selApp.JobAdvertisementInfoId equals jobAd.Id
                                                         join jobAdRe in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll() on jobAd.Id equals jobAdRe.JobAdvertisementInfoId
                                                         where (model.JobAdvertisementInfoId == 0 || model.JobAdvertisementInfoId == selApp.PRM_JobAdvertisementInfo.Id)
                                                               && (model.DesignationId == 0 || model.DesignationId == null || model.DesignationId == jobAdRe.PRM_Designation.Id)
                                                               && (model.DepartmentId == 0 || model.DepartmentId == null || model.DepartmentId == jobAdRe.PRM_Division.Id)
                                                               && (model.SectionId == 0 || model.SectionId == null || model.SectionId == jobAdRe.PRM_Section.Id)
                                                               && (selApp.ZoneInfoId == LoggedUserZoneInfoId)
                                                         select new SelectedApplicantInfoViewModel()
                                                         {
                                                             Id = selApp.Id,
                                                             JobAdvertisementInfoId = jobAdRe.PRM_JobAdvertisementInfo.Id,
                                                             AdvertisementCode = jobAdRe.PRM_JobAdvertisementInfo.AdCode,
                                                             DesignationId = jobAdRe.PRM_Designation.Id,
                                                             DesignationName = jobAdRe.PRM_Designation.Name,
                                                             DepartmentId = jobAdRe.PRM_Division == null ? Convert.ToInt32(null) : jobAdRe.PRM_Division.Id,
                                                             DepartmentName = jobAdRe.PRM_Division == null ? string.Empty : jobAdRe.PRM_Division.Name,
                                                             SectionId = jobAdRe.SectionId == null ? null : jobAdRe.SectionId,
                                                             SectionName = jobAdRe.SectionId == null ? string.Empty : jobAdRe.PRM_Section.Name,
                                                             ExamTypeName = selApp.PRM_SelectionCritariaOrExamType.Name
                                                         }).Concat(from selApp in _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.GetAll()
                                                                        join selAppDtl in _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.GetAll() on selApp.Id equals selAppDtl.SelectedApplicantInfoId
                                                                        join jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll() on selApp.JobAdvertisementInfoId equals jobAd.Id
                                                                        join jobAdRe in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll() on jobAd.Id equals jobAdRe.JobAdvertisementInfoId
                                                                        where (model.JobAdvertisementInfoId == 0 || model.JobAdvertisementInfoId == selApp.PRM_JobAdvertisementInfo.Id)
                                                                              && (model.DesignationId == 0 || model.DesignationId == null || model.DesignationId == jobAdRe.PRM_Designation.Id)
                                                                              && (model.DepartmentId == 0 || model.DepartmentId == null || model.DepartmentId == jobAdRe.PRM_Division.Id)
                                                                              && (model.SectionId == 0 || model.SectionId == null || model.SectionId == jobAdRe.PRM_Section.Id)
                                                                              && (selApp.ZoneInfoId == LoggedUserZoneInfoId)
                                                                        select new SelectedApplicantInfoViewModel()
                                                                        {
                                                                            Id = selApp.Id,
                                                                            JobAdvertisementInfoId = jobAdRe.PRM_JobAdvertisementInfo.Id,
                                                                            AdvertisementCode = jobAdRe.PRM_JobAdvertisementInfo.AdCode,
                                                                            DesignationId = jobAdRe.PRM_Designation.Id,
                                                                            DesignationName = jobAdRe.PRM_Designation.Name,
                                                                            DepartmentId =jobAdRe.PRM_Division == null ?Convert.ToInt32(null) : jobAdRe.PRM_Division.Id,
                                                                            DepartmentName = jobAdRe.PRM_Division == null ? string.Empty : jobAdRe.PRM_Division.Name,
                                                                            SectionId = jobAdRe.SectionId == null ? null : jobAdRe.SectionId,
                                                                            SectionName = jobAdRe.SectionId == null ? string.Empty : jobAdRe.PRM_Section.Name,
                                                                            ExamTypeName = selApp.PRM_SelectionCritariaOrExamType.Name
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
            if (request.SortingName == "SectionName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SectionName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SectionName).ToList();
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
                  d.DepartmentId,
                  d.DepartmentName,
                  d.SectionId,
                  d.SectionName,
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
        public ActionResult DepartmentNameforView()
        {
            var list = Common.PopulateDllList(_prmCommonService.PRMUnit.DivisionRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }
        [NoCache]
        public ActionResult SectionNameforView()
        {
            var list = Common.PopulateDllList(_prmCommonService.PRMUnit.SectionRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }

        #endregion

        #region  Create

        public ActionResult Create()
        {
            SelectedApplicantInfoViewModel model = new SelectedApplicantInfoViewModel();
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create([Bind(Exclude = "Attachment")] SelectedApplicantInfoViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                model.IsError = 1;

                errorList = BusinessLogicValidation(model);

                var attachment = Request.Files["attachment"];

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
                        _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.Add(entity);
                        _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    else
                    {
                        // Set preious attachment if exist

                        var obj = _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.GetByID(model.Id);
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
                            _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.Update(entity);
                            _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.SaveChanges();
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

        #endregion

        #region Edit

        public ActionResult Edit(int Id, string type)
        {
            var entity = _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.GetByID(Id);

            var model = entity.ToModel();
            DownloadDoc(model);
            model.strMode = "Edit";
            model.IsInEditMode = true;

            //applicant shortlist Detail

            var list = (from selApp in _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.GetAll()
                        join selAppDtl in _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.GetAll() on selApp.Id equals selAppDtl.SelectedApplicantInfoId
                        join applicant in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on selAppDtl.ApplicantInfoId equals applicant.intPK
                        join tstRslt in _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.GetAll() on applicant.intPK equals tstRslt.ApplicantInfoId
                        join selCriDtl in _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.GetAll() on selApp.SelectionCriteriaExamTypeId equals selCriDtl.SelectionCriteriaOrExamTypeId
                        where (selAppDtl.SelectedApplicantInfoId == Id)
                        select new SelectedApplicantInfoViewModel()
                        {
                                Id = selAppDtl.Id,
                                ApplicantInfoId = selAppDtl.ApplicantInfoId,
                                RollNo = applicant.RollNo,
                                FullMark = selCriDtl.FullMark,
                                PassMark = selCriDtl.PassMark,
                                ObtainedMarks=tstRslt.ObtainedMarks,
                                QuotaId = selAppDtl.QuotaId == null ? null : selAppDtl.QuotaId,
                              //  Quota = selAppDtl.QuotaId == null ? string.Empty : selAppDtl.PRM_QuotaName.Name,
                                ApplicantName = applicant.Name,
                                DateOfBirth = applicant.DateOfBirth.ToString("dd-MM-yyyy"),
                                DesignationName = _prmCommonService.PRMUnit.DesignationRepository.Get(x => x.Id == applicant.DesignationId).Select(s=>s.Name).FirstOrDefault(),
                                DesignationId = _prmCommonService.PRMUnit.DesignationRepository.Get(x => x.Id == applicant.DesignationId).Select(s => s.Id).FirstOrDefault(),
                                IsFinallySelected=Convert.ToBoolean(selAppDtl.IsFinalSelected),
                                SelectedId=selAppDtl.SelectedId,
                                IsCheckedFinal = true,
                                JobAdvertisementInfoId = selApp.JobAdvertisementInfoId,
                                SelectedForNextExamId = selAppDtl.SelectedForNextExamId,
                                IsFinalExam=selApp.IsFinalExam
                        }).DistinctBy(x=>x.RollNo).ToList();
            PopulateGridList(model);
            model.SelectedForNextExamList= PopulateNextExamType(model.JobAdvertisementInfoId, model.SelectionCriteriaExamTypeId);

            foreach (var item in list)
            {
                item.IsFinallySelectedList = model.IsFinallySelectedList;
                item.SelectedForNextExamList = model.SelectedForNextExamList;
            }
            model.ApplicantInformationListDetail = list;

            #region Job post information
            var jobPostList = (from selAppDtl in _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.GetAll()
                               join jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll() on selAppDtl.JobAdvertisementInfoId equals jobAd.Id
                               join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                               where (selAppDtl.Id == Id && jobAdReq.JobAdvertisementInfoId==model.JobAdvertisementInfoId)
                               select new SelectedApplicantInfoViewModel()
                               {
                                    DesignationId = jobAdReq.PRM_Designation.Id,
                                    DesignationName = jobAdReq.PRM_Designation.Name,
                                    DepartmentId = jobAdReq.PRM_Division == null ? Convert.ToInt32(null) : jobAdReq.PRM_Division.Id,
                                    DepartmentName = jobAdReq.PRM_Division == null ? string.Empty : jobAdReq.PRM_Division.Name,
                                    SectionId = jobAdReq.SectionId == null ? null : jobAdReq.SectionId,
                                    SectionName = jobAdReq.SectionId == null ?string.Empty : jobAdReq.PRM_Section.Name,
                                    NoOfPost = jobAdReq.NumberOfClearancePosition,
                                 //   IsChecked = true,
                                    strMode = "Edit"
                               }).Concat(from selAppDtl in _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.GetAll()
                                         join jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll() on selAppDtl.JobAdvertisementInfoId equals jobAd.Id
                                         join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                         where (selAppDtl.Id == Id && jobAdReq.JobAdvertisementInfoId == model.JobAdvertisementInfoId)
                                         select new SelectedApplicantInfoViewModel()
                                         {
                                             DesignationId = jobAdReq.PRM_Designation.Id,
                                             DesignationName = jobAdReq.PRM_Designation.Name,
                                             DepartmentId =jobAdReq.PRM_Division == null ? Convert.ToInt32(null) : jobAdReq.PRM_Division.Id,
                                             DepartmentName = jobAdReq.PRM_Division == null ? string.Empty : jobAdReq.PRM_Division.Name,
                                             SectionId = jobAdReq.SectionId == null ? null : jobAdReq.SectionId,
                                             SectionName = jobAdReq.SectionId == null ? string.Empty : jobAdReq.PRM_Section.Name,
                                             NoOfPost = jobAdReq.NumberOfPosition,
                                 //            IsChecked = true,
                                             strMode = "Edit"
                                         }).DistinctBy(x => x.DesignationId).ToList();

            model.JobPostInformationList = jobPostList;
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

        #endregion

        #region Delete

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.GetByID(id);

            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(PRM_SelectedApplicantInfoDetail) };

                    _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.Delete(tempPeriod.Id, allTypes);
                    _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.SaveChanges();
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
                _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.Delete(Id);
                _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.SaveChanges();
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


        private PRM_SelectedApplicantInfo CreateEntity(SelectedApplicantInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            if (model.strMode == "Edit")
            {
                pAddEdit = false;
            }

            foreach (var c in model.ApplicantInformationListDetail)
            {
                var prm_SelectedApplicantInfoDetail = new PRM_SelectedApplicantInfoDetail();

                if (c.IsCheckedFinal)
                {
                    prm_SelectedApplicantInfoDetail.Id = c.Id;
                    prm_SelectedApplicantInfoDetail.IsFinalSelected = c.IsFinallySelected;
                    prm_SelectedApplicantInfoDetail.QuotaId = c.QuotaId;
                    prm_SelectedApplicantInfoDetail.SelectedId = c.SelectedId;
                    prm_SelectedApplicantInfoDetail.ApplicantInfoId = c.ApplicantInfoId;
                    prm_SelectedApplicantInfoDetail.SelectedForNextExamId = c.SelectedForNextExamId;
                    prm_SelectedApplicantInfoDetail.IUser = User.Identity.Name;
                    prm_SelectedApplicantInfoDetail.IDate = DateTime.Now;

                    if (pAddEdit)
                    {
                        prm_SelectedApplicantInfoDetail.IUser = User.Identity.Name;
                        prm_SelectedApplicantInfoDetail.IDate = DateTime.Now;
                        entity.PRM_SelectedApplicantInfoDetail.Add(prm_SelectedApplicantInfoDetail);
                    }
                    else
                    {
                        prm_SelectedApplicantInfoDetail.SelectedApplicantInfoId = model.Id;
                        prm_SelectedApplicantInfoDetail.EUser = User.Identity.Name;
                        prm_SelectedApplicantInfoDetail.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                            var requInfo = _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.GetAll().Where(x => x.ApplicantInfoId == c.ApplicantInfoId).ToList();
                            if (requInfo.Count == 0)
                            {
                                _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.Add(prm_SelectedApplicantInfoDetail);
                            }
                        }
                        else
                        {
                            _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.Update(prm_SelectedApplicantInfoDetail);

                        }
                    }
                    _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.SaveChanges();

                }
            }

            return entity;
        }

        #region Populate Dropdown
        private void populateDropdown(SelectedApplicantInfoViewModel model)
        {
            dynamic ddlList;

            #region job advertisement
            ddlList = (from test in _prmCommonService.PRMUnit.TestResultforApplicantInfoRepository.Fetch()
                       join jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Fetch() on test.JobAdvertisementInfoId equals jobAd.Id
                       select jobAd).Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).DistinctBy(x => x.Id).OrderBy(o => o.AdCode).ToList();

            model.AdvertisementCodeList = Common.PopulateJobAdvertisementDDL(ddlList);
            #endregion

            #region Critaria
            var cri = _prmCommonService.PRMUnit.SelectionCritariaOrExamTypeRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.SelectionCriteriaExamTypeList = Common.PopulateDllList(cri);
            #endregion
        }
        public void PopulateGridList(SelectedApplicantInfoViewModel model)
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Yes", Value = "true"});
            list.Add(new SelectListItem() { Text = "No", Value = "false" });

            model.IsFinallySelectedList = list;
        }

        public List<SelectListItem> PopulateNextExamType(int jobAdId, int examTypeId)
        {
            var list = new List<SelectListItem>();

           var  items = (from degLevel in _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll()
                         join degLblDtl in _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.GetAll() on degLevel.Id equals degLblDtl.SelectionCriteriaId
                         join degree in _prmCommonService.PRMUnit.SelectionCritariaOrExamTypeRepository.GetAll() on degLblDtl.SelectionCriteriaOrExamTypeId equals degree.Id
                         where degLevel.JobAdvertisementInfoId == jobAdId && degLblDtl.SelectionCriteriaOrExamTypeId != examTypeId
                         select new
                         {
                             Id = degree.Id,
                             Name = degree.Name
                         }).ToList();

           foreach (var item in items)
           {
               list.Add(new SelectListItem()
               {
                   Text = item.Name.ToString(),
                   Value = item.Id.ToString()
               });
           }


          return list;
        }

        #endregion

        [NoCache]
        public string BusinessLogicValidation(SelectedApplicantInfoViewModel model)
        {
            string errorMessage = string.Empty;
            if (model.strMode != "Edit")
            {
                if (model.ApplicantInformationListDetail.Count <= 0)
                {
                    return errorMessage = "Sorry! There is no Applicant list to Save.";
                }
                foreach (var item in model.ApplicantInformationListDetail)
                {
                    if (item.IsCheckedFinal)
                    {
                        if (item.SelectedId != null)
                        {
                            var getRoll = _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.GetAll().Where(x => x.SelectedId == item.SelectedId).ToList();
                            if (getRoll.Count > 0)
                            {
                                return errorMessage = "Employee Name's " + item.ApplicantName + " Roll No." + item.RollNo + " Selected Id " + item.SelectedId + " Already Exist";
                            }
                        }
                    }

                }
            }
            return errorMessage;

        }


        //get jobpost by job advertisement id
        [HttpGet]
        public PartialViewResult GetJobPost(int jobAdId)
        {
            List<SelectedApplicantInfoViewModel> list = new List<SelectedApplicantInfoViewModel>();

            list = (from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll()
                    join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                    join appInt in _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetAll() on jobAdReq.JobAdvertisementInfoId equals appInt.AdvertisementInfoId
                    where (jobAd.Id == jobAdId) && (appInt.IsIssue==true)
                    select new SelectedApplicantInfoViewModel
                    {
                        DesignationId = jobAdReq.PRM_Designation.Id,
                        DesignationName = jobAdReq.PRM_Designation.Name,
                        DepartmentId = jobAdReq.PRM_Division.Id,
                        DepartmentName = jobAdReq.PRM_Division == null ? string.Empty : jobAdReq.PRM_Division.Name,
                        SectionId = jobAdReq.SectionId == null ? null : jobAdReq.SectionId,
                        SectionName = jobAdReq.SectionId == null ?string.Empty : jobAdReq.PRM_Section.Name,
                        NoOfPost = jobAdReq.NumberOfClearancePosition
                    }).Concat(from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll()
                              join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                              join appInt in _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetAll() on jobAdReq.JobAdvertisementInfoId equals appInt.AdvertisementInfoId
                              where (jobAd.Id == jobAdId) && (appInt.IsIssue == true)
                              select new SelectedApplicantInfoViewModel
                              {
                                  DesignationId = jobAdReq.PRM_Designation.Id,
                                  DesignationName = jobAdReq.PRM_Designation.Name,
                                  DepartmentId =jobAdReq.PRM_Division == null?Convert.ToInt32(null): jobAdReq.PRM_Division.Id,
                                  DepartmentName =jobAdReq.PRM_Division == null? string.Empty: jobAdReq.PRM_Division.Name,
                                  SectionId = jobAdReq.SectionId == null ? null : jobAdReq.SectionId,
                                  SectionName = jobAdReq.SectionId == null ? string.Empty : jobAdReq.PRM_Section.Name,
                                  NoOfPost = jobAdReq.NumberOfPosition
                              }).DistinctBy(x => x.DesignationId).ToList();

            return PartialView("_JobPost", new SelectedApplicantInfoViewModel { JobPostInformationList = list });
        }

        //get applicant info by jobpost(designation) id
        [HttpPost]
        public PartialViewResult AddedApplicantInfo(List<SelectedApplicantInfoDetailViewModel> jobPosts, string strMode, int jobAdInfoId, int examTypeId)
        {
            var model = new SelectedApplicantInfoViewModel();

            List<SelectedApplicantInfoViewModel> AssignmentList = new List<SelectedApplicantInfoViewModel>();
            if (jobPosts != null)
            {
                var list = (from tResult in _prmCommonService.PRMUnit.TestResultforApplicantInfoRepository.GetAll().Where(x => jobPosts.Select(n => n.DesignationId).Contains(x.DesignationId)).Where(x => x.JobAdvertisementInfoId == jobAdInfoId && x.SelectionCriteriaOrExamTypeId == examTypeId)
                            join tstRslt in _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.GetAll() on tResult.Id equals tstRslt.TestResultforApplicantInfoId
                            join appInfo in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on tstRslt.ApplicantInfoId equals appInfo.intPK
                            join selCri in _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll() on tResult.JobAdvertisementInfoId equals selCri.JobAdvertisementInfoId
                            join selCriDtl in _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.GetAll() on selCri.Id equals selCriDtl.SelectionCriteriaId
                            where ((jobPosts.Select(n => n.DesignationId).Contains(Convert.ToInt32(appInfo.DesignationId)))
                            && (jobPosts.Select(n => n.DesignationId).Contains(Convert.ToInt32( selCri.DesignationId))))
                            && (selCriDtl.SelectionCriteriaOrExamTypeId == examTypeId)
                            select new SelectedApplicantInfoViewModel()
                            {
                                ApplicantInfoId = appInfo.intPK,
                                RollNo = appInfo.RollNo,
                                FullMark = selCriDtl.FullMark,
                                PassMark = selCriDtl.PassMark,
                                ObtainedMarks = tstRslt.ObtainedMarks,
                                //QuotaId = appInfo.QuotaNameId == null ? null:appInfo.QuotaNameId,
                                //Quota = appInfo.QuotaNameId == null ? string.Empty : appInfo.PRM_QuotaName.Name,
                                DesignationName =  _prmCommonService.PRMUnit.DesignationRepository.Get(x => x.Id == appInfo.DesignationId).Select(s=>s.Name).FirstOrDefault(),
                                ApplicantName = appInfo.Name,
                                DateOfBirth = appInfo.DateOfBirth.ToString(DateAndTime.GlobalDateFormat),
                                JobAdvertisementInfoId = tResult.JobAdvertisementInfoId,
                                IsFinalExam = selCriDtl.IsLastExam
                            }).DistinctBy(x=>x.RollNo).ToList();

                PopulateGridList(model);

                model.SelectedForNextExamList = PopulateNextExamType(jobAdInfoId, examTypeId);

                foreach (var vmApplicant in list)
                {
                    var dupList = _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.GetAll().Where(x => x.ApplicantInfoId == vmApplicant.ApplicantInfoId && x.SelectedForNextExamId != examTypeId && x.IsFinalSelected != false).ToList();   // for checking duplicate

                    if (strMode == "Create")
                    {
                        if (dupList.Count == 0)
                        {
                            var gridModel = new SelectedApplicantInfoViewModel
                            {
                                ApplicantInfoId = vmApplicant.ApplicantInfoId,
                                RollNo = vmApplicant.RollNo,
                                FullMark = vmApplicant.FullMark,
                                PassMark = vmApplicant.PassMark,
                                ObtainedMarks=vmApplicant.ObtainedMarks,
                                QuotaId=vmApplicant.QuotaId,
                                Quota=vmApplicant.Quota,
                                ApplicantName = vmApplicant.ApplicantName,
                                DateOfBirth = vmApplicant.DateOfBirth,
                                DesignationName = vmApplicant.DesignationName,
                                JobAdvertisementInfoId=vmApplicant.JobAdvertisementInfoId,
                                DesignationId=vmApplicant.DesignationId,
                                IsFinalExam = vmApplicant.IsFinalExam

                            };

                            gridModel.IsFinallySelectedList = model.IsFinallySelectedList;
                            gridModel.SelectedForNextExamList = model.SelectedForNextExamList;
                            AssignmentList.Add(gridModel);
                        }
                    }
                    else
                    {
                        if (dupList.Count != 0)
                        {
                            var gridModel = new SelectedApplicantInfoViewModel
                            {
                                ApplicantInfoId = vmApplicant.ApplicantInfoId,
                                RollNo = vmApplicant.RollNo,
                                FullMark = vmApplicant.FullMark,
                                PassMark = vmApplicant.PassMark,
                                ObtainedMarks = vmApplicant.ObtainedMarks,
                                QuotaId = vmApplicant.QuotaId,
                                Quota = vmApplicant.Quota,
                                ApplicantName = vmApplicant.ApplicantName,
                                DateOfBirth = vmApplicant.DateOfBirth,
                                DesignationName = vmApplicant.DesignationName,
                                IsCheckedFinal = true,
                                JobAdvertisementInfoId = vmApplicant.JobAdvertisementInfoId,
                                DesignationId = vmApplicant.DesignationId,
                                IsFinalExam = vmApplicant.IsFinalExam
                            };
                            gridModel.IsFinallySelectedList = model.IsFinallySelectedList;
                            gridModel.SelectedForNextExamList = model.SelectedForNextExamList;
                            AssignmentList.Add(gridModel);
                        }
                        else
                        {
                            var gridModel = new SelectedApplicantInfoViewModel
                            {
                                ApplicantInfoId = vmApplicant.ApplicantInfoId,
                                RollNo = vmApplicant.RollNo,
                                FullMark = vmApplicant.FullMark,
                                PassMark = vmApplicant.PassMark,
                                ObtainedMarks = vmApplicant.ObtainedMarks,
                                QuotaId = vmApplicant.QuotaId,
                                Quota = vmApplicant.Quota,
                                ApplicantName = vmApplicant.ApplicantName,
                                DateOfBirth = vmApplicant.DateOfBirth,
                                DesignationName = vmApplicant.DesignationName,
                                IsCheckedFinal = false,
                                JobAdvertisementInfoId = vmApplicant.JobAdvertisementInfoId,
                                DesignationId = vmApplicant.DesignationId,
                                IsFinalExam = vmApplicant.IsFinalExam

                            };
                            gridModel.IsFinallySelectedList = model.IsFinallySelectedList;
                            gridModel.SelectedForNextExamList = model.SelectedForNextExamList;
                            AssignmentList.Add(gridModel);

                        }

                    }
                }
                
                model.ApplicantInformationListDetail = AssignmentList;
            }
            return PartialView("_Details", model);
        }


        #region Attachment

        private int Upload(SelectedApplicantInfoViewModel model)
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

        public void DownloadDoc(SelectedApplicantInfoViewModel model)
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
        public ActionResult ViewTestResult(int id, int jobAdvertisementInfoId, int? designationId)
        {

            var model = new SelectedApplicantInfoViewModel();
            List<SelectedApplicantInfoViewModel> resultFrm = (from tResult in _prmCommonService.PRMUnit.TestResultforApplicantInfoRepository.GetAll()
                                                              join tstRslt in _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.GetAll() on tResult.Id equals tstRslt.TestResultforApplicantInfoId
                                                              join selCri in _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll() on tResult.JobAdvertisementInfoId equals selCri.JobAdvertisementInfoId
                                                              join selCriDtl in _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.GetAll() on selCri.Id equals selCriDtl.SelectionCriteriaId
                                                              join examType in _prmCommonService.PRMUnit.SelectionCritariaOrExamTypeRepository.GetAll() on tResult.SelectionCriteriaOrExamTypeId equals examType.Id
                                                              where (tstRslt.ApplicantInfoId == id && selCri.JobAdvertisementInfoId == jobAdvertisementInfoId)
                                                              && (tResult.SelectionCriteriaOrExamTypeId == selCriDtl.SelectionCriteriaOrExamTypeId)
                                                              && (selCri.DesignationId == designationId)
                                                              select new SelectedApplicantInfoViewModel()
                                                              {
                                                                  TestName=examType.Name,
                                                                  FullMark=selCriDtl.FullMark,
                                                                  PassMark=selCriDtl.PassMark,
                                                                  ObtainedMarks = tstRslt.ObtainedMarks,
                                                                  ExamTypeId=tResult.SelectionCriteriaOrExamTypeId
                                                              }).DistinctBy(x=>x.ExamTypeId).ToList();
            model.TestResultList = resultFrm;
            return PartialView("_TestResult", model);
        }

        public ActionResult SelectionCriteriaExamType(int jobAdId)
        {
            var items = (from degLevel in _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll()
                         join degLblDtl in _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.GetAll() on degLevel.Id equals degLblDtl.SelectionCriteriaId
                         join degree in _prmCommonService.PRMUnit.SelectionCritariaOrExamTypeRepository.GetAll() on degLblDtl.SelectionCriteriaOrExamTypeId equals degree.Id
                         where degLevel.JobAdvertisementInfoId == jobAdId
                         select new
                         {
                             Id = degree.Id,
                             Name = degree.Name
                         }).DistinctBy(x=>x.Id).ToList();

            return Json(items, JsonRequestBehavior.AllowGet
            );
        }

        [NoCache]
        public JsonResult GetIsFinalExam(int selectionCriteriaExamTypeId, int jobAdvertisementId)
        {
            var obj = (from degLevel in _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll()
                       join degLblDtl in _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.GetAll() on degLevel.Id equals degLblDtl.SelectionCriteriaId
                       //join degree in _prmCommonService.PRMUnit.SelectionCritariaOrExamTypeRepository.GetAll() on degLblDtl.SelectionCriteriaOrExamTypeId equals degree.Id
                       where degLevel.JobAdvertisementInfoId == jobAdvertisementId && degLblDtl.SelectionCriteriaOrExamTypeId == selectionCriteriaExamTypeId
                       select new
                       {
                           IsLastExam = degLblDtl.IsLastExam,
                       }).FirstOrDefault();

            return Json(new
            {
                IsFinalExam = obj.IsLastExam
            });

        }


	}
}