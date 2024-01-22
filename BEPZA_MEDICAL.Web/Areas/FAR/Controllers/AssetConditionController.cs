using BEPZA_MEDICAL.Domain.FAR;
using BEPZA_MEDICAL.Web.Areas.FAR.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.Controllers
{
    public class AssetConditionController : Controller
    {
        #region Fields

        private readonly FARCommonService _farCommonService;
        #endregion

        #region Ctor
        public AssetConditionController(FARCommonService farCommonService)
        {
            this._farCommonService = farCommonService;
        }
        #endregion

        #region Actions
        //
        // GET: FAR/AssetCondition
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, AssetConditionViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from assetCondition in _farCommonService.FARUnit.AssetConditionRepository.GetAll()
                        select new AssetConditionViewModel()
                                                      {
                                                          Id = assetCondition.Id,
                                                          AssetStatusName=assetCondition.FAR_AssetStatus.Name,
                                                          Name = assetCondition.Name,
                                                          SortOrder = assetCondition.SortOrder,
                                                          Remarks = assetCondition.Remarks
                                                      }).OrderBy(x => x.SortOrder).ToList();

            if (request.Searching)
            {

                if ((viewModel.Name != null && viewModel.Name != ""))
                {
                    list = list.Where(d => d.Name == viewModel.Name).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "AssetStatusName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssetStatusName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssetStatusName).ToList();
                }
            }

            if (request.SortingName == "Name")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Name).ToList();
                }
            }

            if (request.SortingName == "SortOrder")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SortOrder).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SortOrder).ToList();
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
                    d.AssetStatusName,
                    d.Name,
                    d.SortOrder,
                    d.Remarks
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }

        public ActionResult Create()
        {
            AssetConditionViewModel model = new AssetConditionViewModel();
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(AssetConditionViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = "This name already exist.";
                        model.errClass = "failed";
                        populateDropdown(model);
                        return View(model);
                    }

                    model.IUser = User.Identity.Name;
                    model.IDate = DateTime.Now;
                    var entity = model.ToEntity();
                    _farCommonService.FARUnit.AssetConditionRepository.Add(entity);
                    _farCommonService.FARUnit.AssetConditionRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);


                }

                if (errorList.Count() > 0)
                {
                    model.errClass = "failed";
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.errClass = "failed";
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                //if (ex.InnerException != null && ex.InnerException is SqlException)
                //{
                //    SqlException sqlException = ex.InnerException as SqlException;
                //    model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                //}
                //else
                //{
                //    model.ErrMsg = Resources.ErrorMessages.InsertSuccessful;
                //}
            }
            populateDropdown(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var entity = _farCommonService.FARUnit.AssetConditionRepository.GetByID(id);
            var model = entity.ToModel();
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AssetConditionViewModel model)
        {
            try
            {
                string errorList = string.Empty;

                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = "This name already exist.";
                        model.errClass = "failed";
                        populateDropdown(model);
                        return View(model);
                    }

                    model.EUser = User.Identity.Name;
                    model.EDate = DateTime.Now;
                    var entity = model.ToEntity();
                    _farCommonService.FARUnit.AssetConditionRepository.Update(entity);
                    _farCommonService.FARUnit.AssetConditionRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;

                }

                if (errorList.Count() > 0)
                {
                    model.errClass = "failed";
                    model.ErrMsg = errorList;
                }

            }
            catch (Exception ex)
            {
                model.errClass = "failed";
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                //if (ex.InnerException != null && ex.InnerException is SqlException)
                //{
                //    SqlException sqlException = ex.InnerException as SqlException;
                //    model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                //}
                //else
                //{
                //    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                //}
            }
            populateDropdown(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _farCommonService.FARUnit.LocationRepository.Delete(id);
                _farCommonService.FARUnit.LocationRepository.SaveChanges();
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

        #region Private
        private void populateDropdown(AssetConditionViewModel model)
        {

            #region asset status ddl
            var ddlList = _farCommonService.FARUnit.AssetStatusRepository.GetAll().OrderBy(q => q.SortOrder).ToList();
            model.AssetStatusList = Common.PopulateDllList(ddlList);
            #endregion
        }
        private bool CheckDuplicateEntry(AssetConditionViewModel model, int Id)
        {
            if (Id < 1)
            {
                return _farCommonService.FARUnit.AssetConditionRepository.Get(q => q.Name.ToLower() == model.Name.ToLower()).Any();
            }

            else
            {
                return _farCommonService.FARUnit.AssetConditionRepository.Get(q => q.Name.ToLower() == model.Name.ToLower() && Id != q.Id).Any();
            }
        }
        #endregion
    }
}