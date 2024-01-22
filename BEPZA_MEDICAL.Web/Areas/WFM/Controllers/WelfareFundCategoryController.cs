using BEPZA_MEDICAL.DAL.WFM;
using BEPZA_MEDICAL.Domain.WFM;
using BEPZA_MEDICAL.Web.Areas.WFM.ViewModel;
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

namespace BEPZA_MEDICAL.Web.Areas.WFM.Controllers
{
    public class WelfareFundCategoryController : BaseController
    {
        #region Fields
        private readonly WFMCommonService _wpfCommonService;
        private readonly WFM_ExecuteFunctions _wpffunction;
        #endregion

        #region Ctor
        public WelfareFundCategoryController(WFMCommonService wpfCommonService, WFM_ExecuteFunctions wpffunction)
        {
            this._wpfCommonService = wpfCommonService;
            this._wpffunction = wpffunction;
        }
        #endregion

        #region Actions
        //
        // GET: WFM/WelfareFundCategory
        public ActionResult Index()
        {
            WelfareFundCategoryViewModel model = new WelfareFundCategoryViewModel();
            populateDropdown(model);
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, WelfareFundCategoryViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from wfc in _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll()
                        join coa in _wpffunction.fnGetCOABudgetHeadList() on wfc.COAId equals coa.id
                        select new WelfareFundCategoryViewModel()
                                                      {
                                                          Id = wfc.Id,
                                                          Name = wfc.Name,
                                                          COAId = (int) wfc.COAId,
                                                          BudgetHeadName = coa.accountName
                                                      }).OrderBy(x => x.Name).ToList();

            if (request.Searching)
            {
                if ((viewModel.Name != null && viewModel.Name != ""))
                {
                    list = list.Where(d => d.Name == viewModel.Name).ToList();
                }
                if ( viewModel.COAId > 0)
                {
                    list = list.Where(d => d.COAId == viewModel.COAId).ToList();
                }

            }
            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

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

            if (request.SortingName == "BudgetHeadName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BudgetHeadName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BudgetHeadName).ToList();
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
                    d.Name,
                    d.COAId,
                    d.BudgetHeadName
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }

        public ActionResult Create()
        {
            WelfareFundCategoryViewModel model = new WelfareFundCategoryViewModel();
            populateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Index(WelfareFundCategoryViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = "Duplicate Category Name";
                        model.errClass = "failed";
                        populateDropdown(model);
                        model.strMode = "Create";
                        return View(model);
                    }
                    if (model.Id == 0)
                    {
                        model.IUser = User.Identity.Name;
                        model.IDate = DateTime.Now;
                        var entity = model.ToEntity();
                        _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.Add(entity);
                        _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    else
                    {
                        model.EUser = User.Identity.Name;
                        model.EDate = DateTime.Now;
                        var entity = model.ToEntity();
                        _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.Update(entity);
                        _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;
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
                    model.ErrMsg = Resources.ErrorMessages.InsertSuccessful;
                }
            }

            populateDropdown(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var entity = _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.GetByID(id);
            var model = entity.ToModel();
            model.strMode = "Edit";
            populateDropdown(model);
            return View("Index", model);
        }


        [HttpPost]
        public ActionResult Edit(WelfareFundCategoryViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = "Duplicate Category Name";
                        model.errClass = "failed";
                        populateDropdown(model);
                        model.strMode = "Edit";
                        return View(model);
                    }

                    var entity = model.ToEntity();
                    _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.Update(entity);
                    _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;

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
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
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
                _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.Delete(id);
                _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.SaveChanges();
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

        #region Private Method

        private bool CheckDuplicateEntry(WelfareFundCategoryViewModel model, int id)
        {
            if (id < 1)
            {
                return _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.Get(q => q.Name == model.Name).Any();
            }
            else
            {
                return _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.Get(q => q.Name == model.Name && id != q.Id).Any();
            }

        }

        private void populateDropdown(WelfareFundCategoryViewModel model)
        {
            dynamic ddlList;

            #region coa budget Head
            ddlList = _wpffunction.fnGetCOABudgetHeadList();
            model.BudgetHeadList = Common.PopulateCOABudgetHeatDDL(ddlList);
            #endregion

        }
        #endregion

        public ActionResult CoABudgetHeadListView()
        {          
            return PartialView("Select", Common.PopulateCOABudgetHeatDDL(_wpffunction.fnGetCOABudgetHeadList()));
        }
    }
}