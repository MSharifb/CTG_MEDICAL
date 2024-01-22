using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
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
    public class DesignationHistoryController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Constructor
        public DesignationHistoryController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions

        //
        // GET: /PRM/DesignationHistory/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, DesignationHistoryViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<DesignationHistoryViewModel> list = (from desHis in _prmCommonService.PRMUnit.DesignationHistoryRepository.GetAll()
                                                      join oldSalaryScale in _prmCommonService.PRMUnit.SalaryScaleRepository.GetAll() on desHis.OldSalaryScaleId equals oldSalaryScale.Id
                                                      join newSalaryScale in _prmCommonService.PRMUnit.SalaryScaleRepository.GetAll() on desHis.NewSalaryScaleId equals newSalaryScale.Id
                                                      where (model.OldSalaryScaleId == 0 || model.OldSalaryScaleId == oldSalaryScale.Id)
                                                      && (model.NewSalaryScaleId == null || model.NewSalaryScaleId == 0 || model.NewSalaryScaleId == newSalaryScale.Id)
                                                      && (model.EffectiveDate == null || desHis.EffectiveDate == Convert.ToDateTime(model.EffectiveDate))

                                                      select new DesignationHistoryViewModel()
                                                             {
                                                                 Id = desHis.Id,
                                                                 OldSalaryScaleId = desHis.OldSalaryScaleId,
                                                                 OldSalaryScaleName = oldSalaryScale.SalaryScaleName,
                                                                 NewSalaryScaleId = desHis.NewSalaryScaleId,
                                                                 NewSalaryScaleName = newSalaryScale.SalaryScaleName,
                                                                 EffectiveDate = desHis.EffectiveDate
                                                             }).OrderBy(x => x.EffectiveDate).ToList();



            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "OldSalaryScaleName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.OldSalaryScaleName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.OldSalaryScaleName).ToList();
                }
            }

            if (request.SortingName == "NewSalaryScaleName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.NewSalaryScaleName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.NewSalaryScaleName).ToList();
                }
            }

            if (request.SortingName == "EffectiveDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EffectiveDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EffectiveDate).ToList();
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
                    d.OldSalaryScaleId,
                    d.OldSalaryScaleName,
                    d.NewSalaryScaleId,  
                    d.NewSalaryScaleName,  
                    Convert.ToDateTime(d.EffectiveDate).ToString(DateAndTime.GlobalDateFormat), 
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            DesignationHistoryViewModel model = new DesignationHistoryViewModel();
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DesignationHistoryViewModel model)
        {
            try
            {
                string errorList = "";
                if (ModelState.IsValid)
                {
                    model = GetInsertUserAuditInfo(model, true);
                    var entity = CreateEntity(model, true);

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.DesignationHistoryRepository.Add(entity);
                        _prmCommonService.PRMUnit.DesignationHistoryRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Resources.ErrorMessages.InsertSuccessful;
                        //  return RedirectToAction("Index");
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.IsError = 1;
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
            setDetailList(model);
            return View(model);
        }


        [NoCache]
        public ActionResult Edit(int id)
        {
            var entity = _prmCommonService.PRMUnit.DesignationHistoryRepository.GetByID(id);
            var model = entity.ToModel();

            model.strMode = "Edit";
            model.OldJobGradeList = Common.PopulateJobGradeDDL(_prmCommonService.PRMUnit.JobGradeRepository.Get(d => d.SalaryScaleId == entity.OldSalaryScaleId).OrderBy(x => x.Id).ToList());
            model.NewJobGradeList = Common.PopulateJobGradeDDL(_prmCommonService.PRMUnit.JobGradeRepository.Get(d => d.SalaryScaleId == entity.NewSalaryScaleId).OrderBy(x => x.Id).ToList());
            model.DesignationList = Common.PopulateDllList(_prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.SortingOrder).ToList());
            var list = (from desHisDtl in _prmCommonService.PRMUnit.DesignationHistoryDetailRepository.GetAll()
                        join desHis in _prmCommonService.PRMUnit.DesignationHistoryRepository.GetAll() on desHisDtl.DesignationHistoryId equals desHis.Id
                        join oldJobGrade in _prmCommonService.PRMUnit.JobGradeRepository.GetAll() on desHisDtl.OldJobGradeId equals oldJobGrade.Id
                        join newJobGrade in _prmCommonService.PRMUnit.JobGradeRepository.GetAll() on desHisDtl.NewJobGradeId equals newJobGrade.Id
                        where (desHis.Id == id)
                        select new DesignationHistoryViewModel
                        {
                            Id = desHisDtl.Id,
                            DesignationHistoryId = desHisDtl.DesignationHistoryId,
                            DesignationId = desHisDtl.DesignationId,
                            Designation = desHisDtl.PRM_Designation.Name,
                            DesignationSortOrder = desHisDtl.PRM_Designation.SortingOrder,
                            OldSalaryScaleId = desHisDtl.OldSalaryScaleId,
                            NewSalaryScaleId = desHisDtl.NewSalaryScaleId,
                            OldJobGradeId = desHisDtl.OldJobGradeId,
                            OldJobGradeName = oldJobGrade.GradeName,
                            NewJobGradeId = desHisDtl.NewJobGradeId,
                            NewJobGradeName = newJobGrade.GradeName
                        }).OrderBy(o => o.DesignationSortOrder).ToList();
            model.DesignationHistoryList = list;

            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(DesignationHistoryViewModel model)
        {
            try
            {
                string errorList = "";
                var attachment = Request.Files["attachment"];
                if (ModelState.IsValid)
                {

                    model = GetInsertUserAuditInfo(model, false);
                    var entity = CreateEntity(model, false);

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.DesignationHistoryRepository.Update(entity);
                        _prmCommonService.PRMUnit.DesignationHistoryRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;
                        // return RedirectToAction("Index");
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.IsError = 1;
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
            setDetailList(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                //_prmCommonService.PRMUnit.DesignationHistoryRepository.Delete(id);
                //_prmCommonService.PRMUnit.DesignationHistoryRepository.SaveChanges();
                List<Type> allTypes = new List<Type> { typeof(PRM_DesignationHistoryDetail) };
                _prmCommonService.PRMUnit.DesignationHistoryRepository.Delete(id, allTypes);
                _prmCommonService.PRMUnit.DesignationHistoryRepository.SaveChanges();
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
            }

            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        [HttpPost, ActionName("DeleteDetail")]
        public JsonResult DeleteDetailConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonService.PRMUnit.DesignationHistoryDetailRepository.Delete(id);
                _prmCommonService.PRMUnit.DesignationHistoryDetailRepository.SaveChanges();
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
            }

            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        #endregion

        #region Private
        private void setDetailList(DesignationHistoryViewModel model)
        {
            //var list = (from desHisDtl in _prmCommonService.PRMUnit.DesignationHistoryDetailRepository.GetAll()
            //            join desHis in _prmCommonService.PRMUnit.DesignationHistoryRepository.GetAll() on desHisDtl.DesignationHistoryId equals desHis.Id
            //            join oldJobGrade in _prmCommonService.PRMUnit.JobGradeRepository.GetAll() on desHisDtl.OldJobGradeId equals oldJobGrade.Id
            //            join newJobGrade in _prmCommonService.PRMUnit.JobGradeRepository.GetAll() on desHisDtl.NewJobGradeId equals newJobGrade.Id
            //            where (desHis.Id == model.Id)
            //            select new DesignationHistoryViewModel
            //            {
            //                Id = desHisDtl.Id,
            //                DesignationHistoryId = desHisDtl.DesignationHistoryId,
            //                DesignationId = desHisDtl.DesignationId,
            //                Designation = desHisDtl.PRM_Designation.Name,
            //                DesignationSortOrder = desHisDtl.PRM_Designation.SortingOrder,
            //                OldSalaryScaleId = desHisDtl.OldSalaryScaleId,
            //                NewSalaryScaleId = desHisDtl.NewSalaryScaleId,
            //                OldJobGradeId = desHisDtl.OldJobGradeId,
            //                OldJobGradeName = oldJobGrade.GradeName,
            //                NewJobGradeId = desHisDtl.NewJobGradeId,
            //                NewJobGradeName = newJobGrade.GradeName
            //            }).OrderBy(o => o.DesignationSortOrder).ToList();
            //model.DesignationHistoryList = list;
            model.ShowRecord = "Show";
            model.OldJobGradeList = Common.PopulateJobGradeDDL(_prmCommonService.PRMUnit.JobGradeRepository.Get(d => d.SalaryScaleId == model.OldSalaryScaleId).OrderBy(x => x.Id).ToList());
            model.NewJobGradeList = Common.PopulateJobGradeDDL(_prmCommonService.PRMUnit.JobGradeRepository.Get(d => d.SalaryScaleId == model.NewSalaryScaleId).OrderBy(x => x.Id).ToList());
            model.DesignationList = Common.PopulateDllList(_prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.SortingOrder).ToList());

            List<DesignationHistoryViewModel> list = new List<DesignationHistoryViewModel>();          
            foreach (var item in model.DesignationHistoryList)
            {
                DesignationHistoryViewModel obj = new DesignationHistoryViewModel();
                obj.Id = item.Id;
                obj.DesignationHistoryId = item.DesignationHistoryId;
                obj.DesignationId = item.DesignationId;
                obj.Designation = item.Designation;
                obj.OldSalaryScaleId = item.OldSalaryScaleId;
                obj.NewSalaryScaleId = item.NewSalaryScaleId;
                obj.OldJobGradeId = item.OldJobGradeId;
                obj.OldJobGradeName = item.OldJobGradeName;
                obj.NewJobGradeId = item.NewJobGradeId;
                obj.NewJobGradeName = item.NewJobGradeName;
                list.Add(obj);
            }
            model.DesignationHistoryList = list;

        }

        #endregion

        #region Populate Dropdown
        private void populateDropdown(DesignationHistoryViewModel model)
        {
            dynamic ddlList;

            #region Old Salary Scale
            ddlList = _prmCommonService.PRMUnit.SalaryScaleRepository.GetAll().ToList();
            model.OldSalaryScaleList = Common.PopulateSalaryScaleDDL(ddlList);

            #endregion

            #region New Salary Scale
            DateTime cDate = DateTime.Now;

            ddlList = _prmCommonService.PRMUnit.SalaryScaleRepository.GetAll()/*.Where(t => t.DateOfEffective <= cDate)*/.OrderBy(t => t.DateOfEffective).ToList();

            model.NewSalaryScaleList = Common.PopulateSalaryScaleDDL(ddlList);

            #endregion
        }

        #endregion


        private PRM_DesignationHistory CreateEntity(DesignationHistoryViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            #region HearingFixationInfo detail
            foreach (var c in model.DesignationHistoryList)
            {
                var prm_DesignationHistoryDetail = new PRM_DesignationHistoryDetail();

                prm_DesignationHistoryDetail.Id = c.Id;
                prm_DesignationHistoryDetail.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                prm_DesignationHistoryDetail.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                prm_DesignationHistoryDetail.EUser = c.EUser;
                prm_DesignationHistoryDetail.EDate = c.EDate;

                prm_DesignationHistoryDetail.OldSalaryScaleId = c.OldSalaryScaleId;
                prm_DesignationHistoryDetail.OldJobGradeId = Convert.ToInt32(c.OldJobGradeId);
                prm_DesignationHistoryDetail.NewSalaryScaleId = c.NewSalaryScaleId;
                prm_DesignationHistoryDetail.NewJobGradeId = c.NewJobGradeId;
                prm_DesignationHistoryDetail.DesignationId = Convert.ToInt32(c.DesignationId);

                if (pAddEdit)
                {
                    prm_DesignationHistoryDetail.IUser = User.Identity.Name;
                    prm_DesignationHistoryDetail.IDate = DateTime.Now;

                    //entity.PRM_DesignationHistoryDetail.Add(prm_DesignationHistoryDetail);
                    _prmCommonService.PRMUnit.DesignationHistoryDetailRepository.Add(prm_DesignationHistoryDetail);

                }
                else
                {
                    prm_DesignationHistoryDetail.DesignationHistoryId = model.Id;
                    prm_DesignationHistoryDetail.EUser = User.Identity.Name;
                    prm_DesignationHistoryDetail.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _prmCommonService.PRMUnit.DesignationHistoryDetailRepository.Add(prm_DesignationHistoryDetail);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.DesignationHistoryDetailRepository.Update(prm_DesignationHistoryDetail);
                    }

                }
                //_prmCommonService.PRMUnit.DesignationHistoryDetailRepository.SaveChanges();
            }

            #endregion

            return entity;
        }

        private DesignationHistoryViewModel GetInsertUserAuditInfo(DesignationHistoryViewModel model, bool pAddEdit)
        {
            if (pAddEdit)
            {
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;
            }
            else
            {

                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
            }

            return model;
        }

        public PartialViewResult GetOldAndNewSalaryScaleList(int oldSalarScaleId, int newSalaryScaleId)
        {
            var model = new DesignationHistoryViewModel();

            dynamic ddlList;

            #region Old job grade by old salary scaleId

            ddlList = _prmCommonService.PRMUnit.JobGradeRepository.GetAll().Where(q => q.SalaryScaleId == oldSalarScaleId).ToList();
            model.OldJobGradeList = Common.PopulateJobGradeDDL(ddlList);
            #endregion

            #region New job grade by new salary scaleId

            ddlList = _prmCommonService.PRMUnit.JobGradeRepository.GetAll().Where(q => q.SalaryScaleId == newSalaryScaleId).ToList();
            model.NewJobGradeList = Common.PopulateJobGradeDDL(ddlList);
            #endregion

            #region Designation
            var desigList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.SortingOrder).ToList();
            model.DesignationList = Common.PopulateDllList(desigList);
            #endregion

            return PartialView("_Details", model);
        }


        //search 

        public ActionResult OldSalaryScaleListView()
        {
            var list = Common.PopulateSalaryScaleDDL(_prmCommonService.PRMUnit.SalaryScaleRepository.GetAll().OrderBy(x => x.SalaryScaleName).ToList());
            return PartialView("Select", list);
        }

        [NoCache]
        public ActionResult NewSalaryScaleListView()
        {
            var list = Common.PopulateSalaryScaleDDL(_prmCommonService.PRMUnit.SalaryScaleRepository.GetAll().OrderBy(x => x.SalaryScaleName).ToList());
            return PartialView("Select", list);
        }



    }
}