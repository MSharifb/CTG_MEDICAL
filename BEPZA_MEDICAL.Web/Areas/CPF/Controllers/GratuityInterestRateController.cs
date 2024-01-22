using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Collections.ObjectModel;
using System.Collections;
using System.Data.SqlClient;
using Lib.Web.Mvc.JQuery.JqGrid;

using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.DAL.PGM;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.GratuityInterestRate;
using BEPZA_MEDICAL.Utility;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class GratuityInterestRateController : Controller
    {
        #region Fields

        private readonly CPFCommonService _cpfCommonservice;
        private readonly PRMCommonSevice _prmCommonService;

        #endregion

        #region Constructor

        public GratuityInterestRateController(CPFCommonService cpfCommonservice, PRMCommonSevice prmCommonService)
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
        public ActionResult GetList(JqGridRequest request, GratuityInterestRateModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<GratuityInterestRateModel> list = (from hr in _cpfCommonservice.CPFUnit.CPF_GratuityInterestRateRepository.GetAll()
                                       select new GratuityInterestRateModel()
                                       {
                                           Id = hr.Id,
                                           PeriodId = hr.PeriodId,
                                           Period =  "",
                                           InterestRate = hr.InterestRate,

                                       }).ToList();

            if (request.Searching)
            {
                if (model.PeriodId != null && model.PeriodId != 0)
                {
                    list = list.Where(t => t.PeriodId == model.PeriodId).ToList();
                }

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

            if (request.SortingName == "PeriodId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PeriodId).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PeriodId).ToList();
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
                    //d.PeriodId,
                    d.Period,
                    d.InterestRate,
                    "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public PartialViewResult AddDetail()
        {
            return PartialView("_Detail");
        }

        public ViewResult Details(int id)
        {
            var list = _cpfCommonservice.CPFUnit.CPF_GratuityInterestRateRepository.GetByID(id);

            return View(list);
        }

        public ActionResult Create()
        {
            var model = new GratuityInterestRateModel();
            model.strMode = CrudeAction.Create;
            PrepareModel(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(GratuityInterestRateModel model)
        {
            try
            {
                model.strMode = CrudeAction.Create;
                List<string> errorList = new List<string>();

                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = DateTime.Now;

                    errorList = GetBusinessLogicValidation(model);

                    if (errorList.Count == 0)
                    {
                        _cpfCommonservice.CPFUnit.CPF_GratuityInterestRateRepository.Add(entity);
                        _cpfCommonservice.CPFUnit.CPF_GratuityInterestRateRepository.SaveChanges();
                        //return RedirectToAction("Index");
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        model.errClass = "success";
                        return View("Index", model);
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = Common.ErrorListToString(errorList);
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
            var entity = _cpfCommonservice.CPFUnit.CPF_GratuityInterestRateRepository.GetByID(id);
            var model = entity.ToModel();

            model.strMode = CrudeAction.Edit;

            PrepareModel(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(GratuityInterestRateModel model)
        {
            try
            {
                model.strMode = CrudeAction.Edit;
                List<string> errorList = new List<string>();
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    ArrayList lstDetail = new ArrayList();

                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    // var NavigationList = new Dictionary<Type, ArrayList>();
                    //NavigationList.Add(typeof(PGM_HouseRentRuleDetail), lstDetail);
                    errorList = GetBusinessLogicValidation(model);
                    if (errorList.Count == 0)
                    {
                        _cpfCommonservice.CPFUnit.CPF_GratuityInterestRateRepository.Update(entity);
                        _cpfCommonservice.CPFUnit.CPF_GratuityInterestRateRepository.SaveChanges();

                        //return RedirectToAction("Index");
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        model.errClass = "success";
                        return View("Index", model);
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = Common.ErrorListToString(errorList);
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
                //List<Type> allTypes = new List<Type> { typeof(PGM_HouseRentRuleDetail) };
                _cpfCommonservice.CPFUnit.CPF_GratuityInterestRateRepository.Delete(id);
                _cpfCommonservice.CPFUnit.CPF_GratuityInterestRateRepository.SaveChanges();
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

       //private PGM_HouseRentRule GetInsertUserAuditInfo(PGM_HouseRentRule entity)
       //{
       //    entity.IUser = User.Identity.Name;
       //    entity.IDate = DateTime.Now;

       //    foreach (var child in entity.PGM_HouseRentRuleDetail)
       //    {
       //        child.IUser = User.Identity.Name;
       //        child.IDate = DateTime.Now;
       //    }
       //    return entity;
       //}

       //private PGM_HouseRentRule GetEditUserAuditInfo(PGM_HouseRentRule entity)
       // {
       //     entity.EUser = User.Identity.Name;
       //     entity.EDate = DateTime.Now;

       //     foreach (var child in entity.PGM_HouseRentRuleDetail)
       //     {
       //         child.EUser = User.Identity.Name;
       //         child.EDate = DateTime.Now;
       //     }
       //     return entity;
       // }

        private void PrepareModel(GratuityInterestRateModel model)
        {
            //var listItems = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.GetAll().ToList();
            //model.PeriodList = Common.PopulatePfPeriodDllList(listItems);
        }

        public List<string> GetBusinessLogicValidation(GratuityInterestRateModel model)
        {
            List<string> errorMessage = new List<string>();

            //var q = from hrr in _cpfCommonservice.PGMUnit.HouseRentRuleRepositoty.GetAll()
            //        where hrr.EffectiveDateFrom <= model.EffectiveDateFrom && model.EffectiveDateFrom <= hrr.EffectiveDateTo
            //               && (hrr.EffectiveDateFrom <= model.EffectiveDateTo && model.EffectiveDateTo <= hrr.EffectiveDateTo)
            //               && hrr.SalaryScaleId == model.SalaryScaleId
            //               && hrr.RegionId == model.RegionId
            //        select hrr;

            //if (model.Mode == CrudeAction.Edit)
            //{
            //    q = q.Where(t => t.Id != model.Id);
            //}

            //if (q.Count() > 0)
            //{
            //    errorMessage.Add("Same Salary scale, region and date range have another rule");
            //}

            //if (model.EffectiveDateFrom > model.EffectiveDateTo)
            //{
            //    errorMessage.Add("To date must be greater then from date");
            //}

            return errorMessage;
        }

        [NoCache]
        public ActionResult GetPeriodSearch()
        {
            //var list = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.GetAll().OrderBy(x => x.CPFPeriod).ToList();
            return PartialView("_Select");
        }

        #endregion
    }
}
