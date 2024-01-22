using BEPZA_MEDICAL.Domain.WFM;
using BEPZA_MEDICAL.Web.Areas.WFM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.WFM.Controllers
{
    public class CycleController : Controller
    {

        #region Fields

        private readonly WFMCommonService _wpfCommonService;
        #endregion

        #region Ctor
        public CycleController(WFMCommonService wpfCommonService)
        {
            this._wpfCommonService = wpfCommonService;
        }
        #endregion

        #region Actions
        //
        // GET: WFM/Cycle
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, CycleViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from cycle in _wpfCommonService.WFMUnit.CycleRepository.GetAll()
                        select new CycleViewModel()
                                                      {
                                                          Id = cycle.Id,
                                                          CycleName = cycle.CycleName,
                                                          FromMonth = cycle.FromMonth,
                                                          ToMonth = cycle.ToMonth
                                                      }).OrderBy(x => x.Id).ToList();

            if (request.Searching)
            {

                if ((viewModel.CycleName != null && viewModel.CycleName != ""))
                {
                    list = list.Where(d => d.CycleName == viewModel.CycleName).ToList();
                }

                if ((viewModel.FromMonth != null && viewModel.FromMonth != "") && (viewModel.ToMonth != null && viewModel.ToMonth != ""))
                {
                    list = list.Where(d => d.FromMonth == viewModel.FromMonth && d.ToMonth == viewModel.ToMonth).ToList();
                }

                if (viewModel.FromMonth != null && viewModel.FromMonth != "")
                {
                    list = list.Where(d => d.FromMonth == viewModel.FromMonth).ToList();
                }

                if (viewModel.ToMonth != null && viewModel.ToMonth != "")
                {
                    list = list.Where(d => d.ToMonth == viewModel.ToMonth).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "CycleName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.CycleName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.CycleName).ToList();
                }
            }

            if (request.SortingName == "FromMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FromMonth).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FromMonth).ToList();
                }
            }

            if (request.SortingName == "ToMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ToMonth).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ToMonth).ToList();
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
                    d.CycleName,
                    d.FromMonth,
                    d.ToMonth
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }


        public ActionResult Create()
        {
            CycleViewModel model = new CycleViewModel();
            populateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(CycleViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                if (ModelState.IsValid)
                {
                    model.ErrMsg = validateCyclce(model, model.Id);
                    if (String.IsNullOrEmpty(model.ErrMsg))
                    {
                        model.IUser = User.Identity.Name;
                        model.IDate = DateTime.Now;
                        var entity = model.ToEntity();
                        _wpfCommonService.WFMUnit.CycleRepository.Add(entity);
                        _wpfCommonService.WFMUnit.CycleRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    
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
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }

            populateDropdown(model);
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            var entity = _wpfCommonService.WFMUnit.CycleRepository.GetByID(id);
            var model = entity.ToModel();
            populateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(CycleViewModel model)
        {
            try
            {
                string errorList = string.Empty;

                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    _wpfCommonService.WFMUnit.CycleRepository.Update(entity);
                    _wpfCommonService.WFMUnit.CycleRepository.SaveChanges();
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
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
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
                _wpfCommonService.WFMUnit.CycleRepository.Delete(id);
                _wpfCommonService.WFMUnit.CycleRepository.SaveChanges();
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

        #region Private Method


        private void populateDropdown(CycleViewModel model)
        {

            #region From Month ddl
            model.FromMonthList = Common.PopulateMonthListDDL();
            #endregion

            #region To Month ddl
            model.ToMonthList = Common.PopulateMonthListDDL();
            #endregion
        }



        #endregion

        #region Serch Grid List View
        public ActionResult FromMonthListView()
        {
            var list = Common.PopulateMonthList();
            return PartialView("Select", list);
        }

        public ActionResult ToMonth()
        {
            var list = Common.PopulateMonthList();
            return PartialView("Select", list);
        }
        #endregion


        public JsonResult GetCycleMonth(int Id)
        {
            var Months = string.Empty;
            var obj = _wpfCommonService.WFMUnit.CycleRepository.GetByID(Id);
            if (obj != null)
            {
                Months = obj.FromMonth + '-' + obj.ToMonth;
            }

            return Json(Months, JsonRequestBehavior.AllowGet);
        }

        private string validateCyclce(CycleViewModel model, int id)
        {
            string error = string.Empty;
            var lastCycle = _wpfCommonService.WFMUnit.CycleRepository.GetAll().OrderByDescending(q => q.Id).FirstOrDefault();
            if (lastCycle != null)
            {
                int lastMonth = DateTime.ParseExact(lastCycle.ToMonth, "MMM", CultureInfo.InvariantCulture).Month;

                int frommonthInDigit = DateTime.ParseExact(model.FromMonth, "MMM", CultureInfo.InvariantCulture).Month;
                int tomonthInDigit = DateTime.ParseExact(model.ToMonth, "MMM", CultureInfo.InvariantCulture).Month;
                if (lastMonth >= frommonthInDigit)
                {
                    model.errClass = "failed";
                    return error = "From Month must be greater than previous Cycle Month.";
                }
                if (lastMonth >= tomonthInDigit)
                {
                    model.errClass = "failed";
                    return error = "To Month must be greater than previous Cycle Month.";
                }
            }           
            return error;
        }
    }
}