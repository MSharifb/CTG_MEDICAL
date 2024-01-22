using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Collections;
using System.Data.SqlClient;
using Lib.Web.Mvc.JQuery.JqGrid;

using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.ProfitInterestRate;
using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Controllers;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class ProfitInterestRateController : BaseController
    {
        #region Fields

        private readonly CPFCommonService _cpfCommonservice;
        private readonly PRMCommonSevice _prmCommonService;

        #endregion

        #region Constructor

        public ProfitInterestRateController(CPFCommonService cpfCommonservice, PRMCommonSevice prmCommonService)
        {
            _cpfCommonservice = cpfCommonservice;
            _prmCommonService = prmCommonService;
        }

        #endregion

        #region Actions

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ProfitInterestRateModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<ProfitInterestRateModel> list = (from hr in _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.GetAll()
                                                  select new ProfitInterestRateModel()
                                                  {
                                                      Id = hr.Id,
                                                      Year = hr.Year,
                                                      Month = hr.Month,
                                                      InterestRate = hr.InterestRate,
                                                  }).ToList();
            if (request.Searching)
            {
                if (model.InterestRate != null && model.InterestRate != 0)
                {
                    list = list.Where(t => t.InterestRate == model.InterestRate).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            #region Sorting
            if (request.SortingName == "ID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Id).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Id).ToList();
                }
            }
            if (request.SortingName == "InterestRate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.InterestRate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.InterestRate).ToList();
                }
            }

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.Year,
                    d.Month,
                    d.InterestRate,
                    "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ViewResult Details(int id)
        {
            var list = _cpfCommonservice.CPFUnit.CPF_GratuityInterestRateRepository.GetByID(id);

            return View(list);
        }

        public ActionResult Create()
        {
            var model = new ProfitInterestRateModel();
            
            string previousSetup = _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.GetAll().Select(x => x.PfPeriodDuration).FirstOrDefault();
            
            model.IsMonthly = false;
            if (!string.IsNullOrEmpty(previousSetup) && previousSetup == CPFEnum.ProfitRateType.Monthly.ToString())
            {
                model.IsMonthly = true;
            }

            model.strMode = CrudeAction.Create;
            PrepareModel(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ProfitInterestRateModel model)
        {
            try
            {
                model.strMode = CrudeAction.Create;
                string errorMessage = string.Empty;

                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = DateTime.Now;

                    errorMessage = GetBusinessLogicValidation(model);

                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.Add(entity);
                        _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.SaveChanges();
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        model.errClass = "success";
                        return View("Index", model);
                    }
                }

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    model.IsError = 1;
                    model.ErrMsg = errorMessage;
                    model.errClass = "failed";
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                model.errClass = "failed";
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }

            PrepareModel(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var entity = _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.GetByID(id);
            var model = entity.ToModel();

            model.strMode = CrudeAction.Edit;

            PrepareModel(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ProfitInterestRateModel model)
        {
            try
            {
                model.strMode = CrudeAction.Edit;
                string errorMessage = string.Empty;
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    ArrayList lstDetail = new ArrayList();

                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;
                    errorMessage = GetBusinessLogicValidation(model);
                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.Update(entity);
                        _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.SaveChanges();

                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        model.errClass = "success";
                        return View("Index", model);
                    }
                }

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    model.IsError = 1;
                    model.ErrMsg = errorMessage;
                    model.errClass = "failed";
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //Error logging
                model.IsError = 1;
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);

                model.errClass = "failed";
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }

            PrepareModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult Delete(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.Delete(id);
                _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.SaveChanges();
                result = true;
            }
            catch (UpdateException ex)
            {
                try
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //Error logging
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
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
            catch (Exception)
            {
                result = false;
            }

            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        #endregion Action

        #region Others

        private void PrepareModel(ProfitInterestRateModel model)
        {
            model.YearList = Common.PopulateYearList();
            model.MonthList = Common.PopulateMonthList();
        }

        public string GetBusinessLogicValidation(ProfitInterestRateModel model)
        {
            string errorMessage = string.Empty;
            string duration = model.PfPeriodDuration.Trim();
            var previousSetup = _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.GetAll().DefaultIfEmpty().OfType<CPF_ProfitInterestRate>().ToList();

            string previousMethod = string.Empty;
            previousMethod = previousSetup.Count > 0 ? previousSetup.Select(t => t.PfPeriodDuration).Distinct().FirstOrDefault() : string.Empty;

            if (string.IsNullOrWhiteSpace(previousMethod))
            {
                previousMethod = duration;
            }

            if (previousMethod.Equals(duration))
            {
                string strMode = model.strMode.Trim();
                switch (strMode)
                {
                    case "Create":
                        switch (duration)
                        {
                            case "Yearly":
                                var itemInfo = (from x in _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.Get(t => t.PfPeriodDuration == model.PfPeriodDuration)
                                                select x).DefaultIfEmpty().OfType<CPF_ProfitInterestRate>().ToList();
                                errorMessage = itemInfo.Count > 0 ? "Interest Rate for selected year already exists." : string.Empty;
                                break;
                            case "Monthly":
                                var itemInfoMonth = (from x in _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.Get(t => t.Month == model.Month && t.Year == model.Year)
                                                     select x).DefaultIfEmpty().OfType<CPF_ProfitInterestRate>().ToList();
                                errorMessage = itemInfoMonth.Count > 0 ? "Interest rate for selected Month already exists." : string.Empty;

                                if (string.IsNullOrEmpty(errorMessage))
                                    errorMessage = (model.Month == null || model.Month == string.Empty) ? "Please select month" : string.Empty;

                                break;
                        }
                        break;

                    case "Edit":
                        switch (duration)
                        {
                            case "Yearly":
                                var itemInfo = (from x in _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.Get(t => t.PfPeriodDuration == model.PfPeriodDuration && t.Id != model.Id)
                                                select x).DefaultIfEmpty().OfType<CPF_ProfitInterestRate>().ToList();
                                errorMessage = itemInfo.Count > 0 ? "Interest Rate for selected year already exists." : string.Empty;
                                break;
                            case "Monthly":
                                var itemInfoMonth = (from x in _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.Get(t => t.Month == model.Month && t.Id != model.Id)
                                                     select x).DefaultIfEmpty().OfType<CPF_ProfitInterestRate>().ToList();
                                errorMessage = itemInfoMonth.Count > 0 ? "Interest rate for selected Month already exists." : string.Empty;

                                if (string.IsNullOrEmpty(errorMessage))
                                    errorMessage = (model.Month == null || model.Month == string.Empty) ? "Please select month" : string.Empty;

                                break;
                        }
                        break;
                }
            }
            else
            {
                errorMessage = "Please select : " + previousMethod + " to set interest rate.";
            }

            return errorMessage;
        }

        [NoCache]
        public ActionResult GetYear()
        {
            var list = Common.PopulateYearList();
            return PartialView("_Select", list);
        }

        [NoCache]
        public ActionResult GetMonth()
        {
            var list = Common.PopulateMonthList();
            return PartialView("_Select", list);
        }


        public ActionResult GetPfInterestRateMethod()
        {
            var previousSetup = _cpfCommonservice.CPFUnit
                .CPF_ProfitInterestRateRepository
                .GetAll()
                .DefaultIfEmpty()
                .OfType<CPF_ProfitInterestRate>()
                .ToList();

            string previousMethod = string.Empty;
            
            previousMethod = previousSetup.Count > 0 ? previousSetup.Select(t => t.PfPeriodDuration).Distinct().FirstOrDefault() : string.Empty;
            
            return Json(previousMethod, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
