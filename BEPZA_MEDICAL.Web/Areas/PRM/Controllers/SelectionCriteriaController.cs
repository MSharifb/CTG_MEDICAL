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
    public class SelectionCriteriaController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Constructor
        public SelectionCriteriaController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion
        //
        // GET: /PRM/SelectionCriteria/
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SelectionCriteriaViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<SelectionCriteriaViewModel> list = new List<SelectionCriteriaViewModel>();
            List<SelectionCriteriaViewModel> seclist = new List<SelectionCriteriaViewModel>();

            var listTemp = _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll().ToList();
            foreach (var item in listTemp)
            {
                if (item.IsSameCriteria)
                {
                  seclist = (from sel in _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll()
                            join ad in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll() on sel.JobAdvertisementInfoId equals ad.Id
                            join addtl in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll() on ad.Id equals addtl.JobAdvertisementInfoId
                            join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on addtl.DesignationId equals des.Id
                            where (model.JobAdvertisementInfoId == 0 || model.JobAdvertisementInfoId == ad.Id)
                            && (model.DesignationId == 0 || model.DesignationId == addtl.DesignationId || model.DesignationId == null)
                            && (sel.IsSameCriteria) && (sel.Id == item.Id)
                            &&(sel.ZoneInfoId==LoggedUserZoneInfoId)
                            select new SelectionCriteriaViewModel()
                            {
                                Id = sel.Id,
                                JobAdvertisementInfoId = ad.Id,
                                AdvertisementName = ad.AdCode,
                                DesignationId = des.Id,
                                Designation = des.Name
                            }).ToList();
                    foreach (var abc in seclist)
                    {
                        list.Add(abc);
                    }
                }
                else
                {
                    seclist = (from sel in _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll()
                               join ad in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll() on sel.JobAdvertisementInfoId equals ad.Id
                               join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on sel.DesignationId equals des.Id
                               where (model.JobAdvertisementInfoId == 0 || model.JobAdvertisementInfoId == ad.Id)
                               && (model.DesignationId == 0 || model.DesignationId == sel.DesignationId || model.DesignationId == null)
                               && (sel.Id == item.Id)
                               && (sel.ZoneInfoId == LoggedUserZoneInfoId)
                               select new SelectionCriteriaViewModel()
                                {
                                    Id = sel.Id,
                                    JobAdvertisementInfoId = ad.Id,
                                    AdvertisementName = ad.AdCode,
                                    DesignationId = des.Id,
                                    Designation = des.Name
                                }).ToList();
                    foreach (var abc in seclist)
                    {
                        list.Add(abc);
                    }
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "AdvertisementName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AdvertisementName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AdvertisementName).ToList();
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
                  d.AdvertisementName,
                  d.DesignationId,
                  d.Designation,
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
            var designations = (from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Fetch()
                                join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.Fetch() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                join des in _prmCommonService.PRMUnit.DesignationRepository.Fetch() on jobAdReq.DesignationId equals des.Id
                                select des).OrderBy(o => o.Name).ToList();

            var list = Common.PopulateDllList(designations);
            return PartialView("Select", list);
        }
        public ActionResult Create()
        {
            SelectionCriteriaViewModel model = new SelectionCriteriaViewModel();

            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create( SelectionCriteriaViewModel model)
        {

            try
            {
                string errorList = string.Empty;
                model.IsError = 1;
                GetPrepareModel(model);

                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, true);

                    _prmCommonService.PRMUnit.SelectionCriteriaRepository.Add(entity);
                    _prmCommonService.PRMUnit.SelectionCriteriaRepository.SaveChanges();
                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
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
            var SelectionCriteriaEntity = _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetByID(id);
            var parentModel = SelectionCriteriaEntity.ToModel();
            parentModel.strMode = "Edit";
            parentModel.IsInEditMode = true;

            if (parentModel.IsSameCriteria == false)
            {
                #region Designation
                var designations = (from des in _prmCommonService.PRMUnit.DesignationRepository.Fetch()
                                    where (des.Id == parentModel.DesignationId)
                                    select des).OrderBy(o => o.Name).ToList();
                parentModel.DesignationList = Common.PopulateDllList(designations);
                #endregion
            }

            List<SelectionCriteriaDetailViewModel> list = (from sel in _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll()
                                                           join selDtl in _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.GetAll() on sel.Id equals selDtl.SelectionCriteriaId
                                                           where (selDtl.SelectionCriteriaId == id)
                                                           select new SelectionCriteriaDetailViewModel()
                                                             {
                                                                 Id = selDtl.Id,
                                                                 SelectionCriteriaOrExamTypeId=selDtl.SelectionCriteriaOrExamTypeId,
                                                                 SelectionCriteriaOrExamName=selDtl.PRM_SelectionCritariaOrExamType.Name,
                                                                 FullMark=selDtl.FullMark,
                                                                 PassMark=selDtl.PassMark,
                                                                 Remarks=selDtl.Remarks,
                                                                 IsLastExam=Convert.ToBoolean(selDtl.IsLastExam)
                                                             }).ToList();

            parentModel.SelectionCriteriaDetailList = list;
            populateDropdown(parentModel);
            if (type == "success")
            {
                parentModel.IsError = 1;
                parentModel.errClass = "success";
                parentModel.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            return View("Edit", parentModel);
        }

        [HttpPost]
        public ActionResult Edit(SelectionCriteriaViewModel model)
        {
            try
            {
                GetPrepareModel(model);

                string errorList = "";
                if (ModelState.IsValid)
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, false);

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.SelectionCriteriaRepository.Update(entity);
                        _prmCommonService.PRMUnit.SelectionCriteriaRepository.SaveChanges();
                        model.errClass = "success";
                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        return RedirectToAction("Edit", new { id = model.Id, type = "success" });
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 0;
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.IsError = 0;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }

            populateDropdown(model);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetByID(id);
            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(PRM_SelectionCriteriaDetail) };
                    _prmCommonService.PRMUnit.SelectionCriteriaRepository.Delete(tempPeriod.Id, allTypes);
                    _prmCommonService.PRMUnit.SelectionCriteriaRepository.SaveChanges();
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
        [HttpPost, ActionName("DeleteSelectionCriteriaDetail")]
        public JsonResult DeleteSelectionCriteriafirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.Delete(id);
                _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.SaveChanges();
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
        [NoCache]
        public SelectionCriteriaViewModel GetPrepareModel(SelectionCriteriaViewModel model)
        {
            if (model.IsSameCriteria)
            {
                model.DesignationId = null;
            }
            else
            {
                return model;
            }
            return model;
        }
        private PRM_SelectionCriteria CreateEntity(SelectionCriteriaViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            foreach (var c in model.SelectionCriteriaDetailList)
            {
                var prm_SelectionCriteriaDetail = new PRM_SelectionCriteriaDetail();

                prm_SelectionCriteriaDetail.Id = c.Id;
                prm_SelectionCriteriaDetail.SelectionCriteriaOrExamTypeId = c.SelectionCriteriaOrExamTypeId;
                prm_SelectionCriteriaDetail.FullMark =(decimal) c.FullMark;
                prm_SelectionCriteriaDetail.PassMark = (decimal) c.PassMark;
                prm_SelectionCriteriaDetail.Remarks = c.Remarks;
                prm_SelectionCriteriaDetail.IsLastExam = c.IsLastExam;
                prm_SelectionCriteriaDetail.IUser = User.Identity.Name;
                prm_SelectionCriteriaDetail.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    prm_SelectionCriteriaDetail.IUser = User.Identity.Name;
                    prm_SelectionCriteriaDetail.IDate = DateTime.Now;
                    entity.PRM_SelectionCriteriaDetail.Add(prm_SelectionCriteriaDetail);
                }
                else
                {
                    prm_SelectionCriteriaDetail.SelectionCriteriaId = model.Id;
                    prm_SelectionCriteriaDetail.EUser = User.Identity.Name;
                    prm_SelectionCriteriaDetail.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.Add(prm_SelectionCriteriaDetail);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.Update(prm_SelectionCriteriaDetail);

                    }
                }
                _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.SaveChanges();
            }

            return entity;
        }
        private void populateDropdown(SelectionCriteriaViewModel model)
        {
            #region AdCode
            var adC = _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Fetch().Where(x=>x.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.AdCode).ToList();
            model.AdvetisementCodeList = Common.PopulateJobAdvertisementDDL(adC);
            #endregion

            #region Critaria
            var cri = _prmCommonService.PRMUnit.SelectionCritariaOrExamTypeRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.SelectionCriteriaOrExamList = Common.PopulateDllList(cri);
            #endregion
        }
      
        public ActionResult JobPostName(int adInfoId)
        {
            var designations = (from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Fetch()
                                join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.Fetch() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                join des in _prmCommonService.PRMUnit.DesignationRepository.Fetch() on jobAdReq.DesignationId equals des.Id
                                where(jobAd.Id==adInfoId)
                                select des).Concat(from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Fetch()
                                                   join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.Fetch() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                                   join des in _prmCommonService.PRMUnit.DesignationRepository.Fetch() on jobAdReq.DesignationId equals des.Id
                                                   where (jobAd.Id == adInfoId)
                                                   select des).OrderBy(o=>o.SortingOrder).ToList();

            var list = designations.Select(x => new { Id = x.Id, Name = x.Name }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
        }
	}
}