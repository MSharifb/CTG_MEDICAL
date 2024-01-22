using BEPZA_MEDICAL.DAL.WFM;
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
    public class WelfareFundPolicyController : Controller
    {

        #region Fields
        private readonly WFMCommonService _wpfCommonService;
        private readonly WFM_ExecuteFunctions _wpffunction;
        #endregion

        #region Ctor
        public WelfareFundPolicyController(WFMCommonService wpfCommonService, WFM_ExecuteFunctions wpffunction)
        {
            this._wpfCommonService = wpfCommonService;
            this._wpffunction = wpffunction;
        }
        #endregion

        #region Actions
        //
        // GET: WFM/WelfareFundPolicy
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, WelfareFundPolicyViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from wfp in _wpfCommonService.WFMUnit.WelfareFundPolicyRepository.GetAll()
                        select new WelfareFundPolicyViewModel()
                                                      {
                                                          Id = wfp.Id,
                                                          WelfareFundCategoryId = wfp.WelfareFundCategoryId,
                                                          WelfareFundCategoryName = wfp.WFM_WelfareFundCategory.Name,
                                                          OrderNo = wfp.OrderNo,
                                                          EffectiveFromDate = wfp.EffectiveFromDate,
                                                          EffectiveToDate = wfp.EffectiveToDate,
                                                          EffectDateView = wfp.IsContinuous.ToString() == "True" ? "Continue" : Convert.ToDateTime(wfp.EffectiveToDate).ToString("dd-MM-yyyy"),
                                                          MinServiceYear = wfp.MinServiceYear,
                                                          MaxAmount = wfp.MaxAmount,
                                                          OtherMaxAmount = wfp.OtherMaxAmount
                                                      }).OrderBy(x => x.WelfareFundCategoryName).ToList();



            if (request.Searching)
            {
                if ((viewModel.EffectiveFromDate != null && viewModel.EffectiveFromDate != DateTime.MinValue) && (viewModel.EffectiveToDate != null && viewModel.EffectiveToDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.EffectiveFromDate >= viewModel.EffectiveFromDate && d.EffectiveToDate <= viewModel.EffectiveToDate).ToList();
                }

                if ((viewModel.EffectiveFromDate != null && viewModel.EffectiveFromDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.EffectiveFromDate >= viewModel.EffectiveFromDate).ToList();
                }

                if ((viewModel.EffectiveToDate != null && viewModel.EffectiveToDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.EffectiveToDate <= viewModel.EffectiveToDate || d.EffectiveToDate == null).ToList();
                }


                if ((viewModel.WelfareFundCategoryId != 0))
                {
                    list = list.Where(d => d.WelfareFundCategoryId == viewModel.WelfareFundCategoryId).ToList();
                }

                if ((viewModel.OrderNo != null && viewModel.OrderNo != ""))
                {
                    list = list.Where(d => d.OrderNo == viewModel.OrderNo).ToList();
                }
                if ((viewModel.MinServiceYear != 0 && viewModel.MinServiceYear != null))
                {
                    list = list.Where(d => d.MinServiceYear == viewModel.MinServiceYear).ToList();
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

            if (request.SortingName == "OrderNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.OrderNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.OrderNo).ToList();
                }
            }
            if (request.SortingName == "MinServiceYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.MinServiceYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.MinServiceYear).ToList();
                }
            }

            if (request.SortingName == "EffectiveFromDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EffectiveFromDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EffectiveFromDate).ToList();
                }
            }
            if (request.SortingName == "EffectiveToDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EffectiveToDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EffectiveToDate).ToList();
                }
            }
            if (request.SortingName == "MaxAmount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.MaxAmount).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.MaxAmount).ToList();
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
                    d.OrderNo,
                    d.MinServiceYear,
                    Convert.ToDateTime(d.EffectiveFromDate).ToString(DateAndTime.GlobalDateFormat),
                    Convert.ToDateTime(d.EffectiveToDate).ToString(DateAndTime.GlobalDateFormat),
                    d.EffectDateView,
                    d.MaxAmount,
                    d.OtherMaxAmount

                }));
            }
            return new JqGridJsonResult() { Data = response };

        }


        public ActionResult Create()
        {
            WelfareFundPolicyViewModel model = new WelfareFundPolicyViewModel();
            populateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(WelfareFundPolicyViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {
                    model.IUser = User.Identity.Name;
                    model.IDate = DateTime.Now;
                    var entity = model.ToEntity();
                    _wpfCommonService.WFMUnit.WelfareFundPolicyRepository.Add(entity);
                    _wpfCommonService.WFMUnit.WelfareFundPolicyRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

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
            var entity = _wpfCommonService.WFMUnit.WelfareFundPolicyRepository.GetByID(id);
            var model = entity.ToModel();
            populateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(WelfareFundPolicyViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    _wpfCommonService.WFMUnit.WelfareFundPolicyRepository.Update(entity);
                    _wpfCommonService.WFMUnit.WelfareFundPolicyRepository.SaveChanges();
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
                _wpfCommonService.WFMUnit.WelfareFundPolicyRepository.Delete(id);
                _wpfCommonService.WFMUnit.WelfareFundPolicyRepository.SaveChanges();
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


        private void populateDropdown(WelfareFundPolicyViewModel model)
        {
            dynamic ddlList;

            #region Welfare Fund Category

            ddlList = _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll().Where(q => q.IsActive == true).OrderBy(x => x.Name).ToList();
            model.WelfareFundCategoryList = Common.PopulateWelfareFundCategoryDDL(ddlList);
            #endregion

        }


        #endregion

        public ActionResult WelfarefundCategoryListView()
        {
            var list = _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll().Where(q => q.IsActive == true).OrderBy(q => q.Name).ToList();           
            return PartialView("Select", Common.PopulateWelfareFundCategoryDDL(list));
        }

    }
}