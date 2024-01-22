using BEPZA_MEDICAL.Domain.WFM;
using BEPZA_MEDICAL.Web.Areas.WFM.ViewModel;
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
    public class ReasonOfWelfareCategoryController : Controller
    {
        #region Fields

        private readonly WFMCommonService _wpfCommonService;
        #endregion

        #region Ctor
        public ReasonOfWelfareCategoryController(WFMCommonService wpfCommonService)
        {
            this._wpfCommonService = wpfCommonService;
        }
        #endregion

        #region Actions
        //
        // GET: WFM/ReasonOfWelfareCategory
        public ActionResult Index()
        {
            ReasonOfWelfareCategoryViewModel model = new ReasonOfWelfareCategoryViewModel();
            populateDropdown(model);
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ReasonOfWelfareCategoryViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from wfc in _wpfCommonService.WFMUnit.ReasonOfWelfareCategoryRepository.GetAll()
                        select new ReasonOfWelfareCategoryViewModel()
                                                      {
                                                          Id = wfc.Id,
                                                          WelfareFundCategoryId = wfc.WelfareFundCategoryId,
                                                          WelfareFundCategoryName = wfc.WFM_WelfareFundCategory.Name,
                                                          Reason = wfc.Reason
                                                      }).OrderBy(x => x.WelfareFundCategoryName).ToList();


            if (request.Searching)
            {

                if ( viewModel.WelfareFundCategoryId != 0)
                {
                    list = list.Where(d => d.WelfareFundCategoryId == viewModel.WelfareFundCategoryId).ToList();
                }

                if ((viewModel.Reason != null && viewModel.Reason != ""))
                {
                    //list = list.Where(d => d.Reason == viewModel.Reason).ToList();
                    list = list.Where(d => d.Reason.Contains(viewModel.Reason)).ToList();
                }

            }
            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "WelfareFundCategoryName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.WelfareFundCategoryName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.WelfareFundCategoryName).ToList();
                }
            }

            if (request.SortingName == "Reason")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Reason).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Reason).ToList();
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
                    d.WelfareFundCategoryId,
                    d.WelfareFundCategoryName,
                    d.Reason
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }


        public ActionResult Create()
        {
            ReasonOfWelfareCategoryViewModel model = new ReasonOfWelfareCategoryViewModel();
            populateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Index(ReasonOfWelfareCategoryViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {
                    if (model.Id == 0)
                    {
                        model.IUser = User.Identity.Name;
                        model.IDate = DateTime.Now;
                        var entity = model.ToEntity();
                        _wpfCommonService.WFMUnit.ReasonOfWelfareCategoryRepository.Add(entity);
                        _wpfCommonService.WFMUnit.ReasonOfWelfareCategoryRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    else
                    {
                        model.EUser = User.Identity.Name;
                        model.EDate = DateTime.Now;
                        var entity = model.ToEntity();
                        _wpfCommonService.WFMUnit.ReasonOfWelfareCategoryRepository.Update(entity);
                        _wpfCommonService.WFMUnit.ReasonOfWelfareCategoryRepository.SaveChanges();
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
            var entity = _wpfCommonService.WFMUnit.ReasonOfWelfareCategoryRepository.GetByID(id);
            var model = entity.ToModel();
            model.strMode = "Edit";
            populateDropdown(model);
            return View("Index", model);
        }


        [HttpPost]
        public ActionResult Edit(ReasonOfWelfareCategoryViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    _wpfCommonService.WFMUnit.ReasonOfWelfareCategoryRepository.Update(entity);
                    _wpfCommonService.WFMUnit.ReasonOfWelfareCategoryRepository.SaveChanges();
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
                _wpfCommonService.WFMUnit.ReasonOfWelfareCategoryRepository.Delete(id);
                _wpfCommonService.WFMUnit.ReasonOfWelfareCategoryRepository.SaveChanges();
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


        private void populateDropdown(ReasonOfWelfareCategoryViewModel model)
        {
            dynamic ddlList;

            #region Welfare Fund Category ddl

            ddlList = _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll().Where(q => q.IsActive == true).OrderBy(x => x.Name).ToList();
            model.WelfareFundCategoryList = Common.PopulateWelfareFundCategoryDDL(ddlList);
            #endregion

        }



        #endregion

        public ActionResult WelfarefundCategoryListView()
        {
            var list = Common.PopulateWelfareFundCategoryDDL(_wpfCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll().Where(q => q.IsActive == true).OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }

    }
}