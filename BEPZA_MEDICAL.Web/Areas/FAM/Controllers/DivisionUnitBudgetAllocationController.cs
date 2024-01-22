using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.DAL.FAM;
using BEPZA_MEDICAL.Domain.FAM;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.DivisionUnitBudgetAllocation;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Resources;
using System.Data.SqlClient;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using System.Collections;
using System.Web.Services.Description;


namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class DivisionUnitBudgetAllocationController : Controller
    {
        #region Fields
        private readonly FAMCommonService _famCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public DivisionUnitBudgetAllocationController(FAMCommonService famCommonService, PRMCommonSevice prmCommonSevice)
        {
            _famCommonService = famCommonService;
            _prmCommonService = prmCommonSevice;
        }
        #endregion

        #region Actions
        public ViewResult Index()
        {
            return View();
        }

        public ActionResult BackToList()
        {
            var model = new DivisionUnitBudgetAllocationModel();
            PrepareModel(model);
            return View("_Index", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, DivisionUnitBudgetAllocationModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            dynamic list = null;

            list = _famCommonService.GetDivisionFYSearchedList(model.DivisionUnitId, model.FinancialYearId);

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.DivisionUnitName,
                    d.FinancialYearName,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new DivisionUnitBudgetAllocationModel();
            //Get Account Head

            //var mainmodel = new DivisionUnitBudgetAllocationModel();
            //PrepareModel(mainmodel);
            model.DivisionUnitList = Common.PopulateDllList(_prmCommonService.PRMUnit.DivisionRepository.GetAll()).ToList();
            model.FinancialYearList = Common.PopulateFinancialYearDllList(_famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll().Where(c=> c.IsClose==false)).ToList();

            //var data = (from d in _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.GetAll()
            //            join coa in _famCommonService.FAMUnit.ChartOfAccount.GetAll() on d.AccountHeadId equals coa.Id
            //            where  d.DivisionUnitId == model.DivisionUnitId orderby coa.AccountHeadType descending
            //            select new
            //            {
            //                d.DivisionUnitId,
            //                d.AccountHeadId,
            //                coa.AccountHeadCode,
            //                coa.AccountHeadName,
            //                coa.AccountHeadType                            
            //            }).Distinct().ToList();

            //foreach (var item in data)
            //{
            //    var innerModel = new BudgetAllocation()
            //    {
            //        AccountHeadId = item.AccountHeadId,
            //        AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
            //        Amount = 0,
            //        AccountHeadType = item.AccountHeadType == "I" ? "Income" : "Expense"
            //    };

            //    model.BudgetAllocationList.Add(innerModel);
            //}
            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(DivisionUnitBudgetAllocationModel model)
        {
            string businessError = string.Empty;
            string strMessage = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    businessError = CheckBusinessLogicDivisionUnitBudgetAllocationInsert(model.FinancialYearId);
                    if (string.IsNullOrEmpty(businessError))
                    {
                        var entity = model.ToEntity();
                        foreach (var item in model.BudgetAllocationList)
                        {
                            item.RevisedAmount = item.Amount;
                            var childEntity = item.ToEntityChild();

                            //childEntity.DivisionUnitId = entity.DivisionUnitId;
                            entity.FAM_DivisionUnitBudgetAllocationDetails.Add(childEntity);
                        }
                        _famCommonService.FAMUnit.DivisionUnitBudgetAllocationRepository.Add(entity);
                        _famCommonService.FAMUnit.DivisionUnitBudgetAllocationRepository.SaveChanges();
                        strMessage = ErrorMessages.InsertSuccessful;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        strMessage = ErrorMessages.UniqueIndex;
                    }
                    else
                    {
                        strMessage = ErrorMessages.InsertFailed;
                    }
                }
            }
            else
            {
                strMessage = "Please check all amount.";
            }

            return new JsonResult()
            {
                //Data = strMessage
                
                Data = String.IsNullOrEmpty(businessError) ? strMessage : businessError
            };
        }

        public ActionResult Edit(int id)
        {
            var mainModel = new DivisionUnitBudgetAllocationModel();
            var entity = _famCommonService.FAMUnit.DivisionUnitBudgetAllocationRepository.GetByID(id);
            //var childEntities = entity.FAM_DivisionUnitBudgetAllocationDetails;

            var model = entity.ToModel();
            //childEntities.ToList().ForEach(x => model.BudgetAllocationList.Add(x.ToModelChild()));

            

            mainModel.DivisionUnitId = model.DivisionUnitId;
            mainModel.FinancialYearId = model.FinancialYearId;
            mainModel.Id = model.Id;

            var data = (from d in _famCommonService.FAMUnit.DivisionUnitBudgetAllocationRepository.GetAll()
                        join dtl in _famCommonService.FAMUnit.BudgetAllocationRepository.GetAll() on d.Id equals dtl.DivisionUnitBudgetId
                        join coa in _famCommonService.FAMUnit.ChartOfAccount.GetAll() on dtl.AccountHeadId equals coa.Id
                        where d.DivisionUnitId == model.DivisionUnitId && d.FinancialYearId == model.FinancialYearId
                        select new
                        {
                            d.DivisionUnitId,
                            dtl.AccountHeadId,
                            dtl.Amount,
                            dtl.RevisedAmount,
                            coa.AccountHeadCode,
                            coa.AccountHeadName,
                            coa.AccountHeadType
                        }).Distinct().ToList();

            foreach (var item in data)
            {
                var innerModel = new BudgetAllocation()
                {
                    AccountHeadId = item.AccountHeadId,
                    //AccountHeadName = item.AccountHeadName,
                    AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
                    Amount = item.Amount,
                    RevisedAmount = item.RevisedAmount,
                    AccountHeadType = item.AccountHeadType == "I" ? "Income" : "Expense"
                };

                mainModel.BudgetAllocationList.Add(innerModel);
            }

            mainModel.DivisionUnitList = Common.PopulateDllList(_prmCommonService.PRMUnit.DivisionRepository.GetAll()).ToList();
            mainModel.FinancialYearList = Common.PopulateFinancialYearDllList(_famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll()).ToList();
            mainModel.Mode = "Edit";
            return View("_CreateOrEdit", mainModel);
        }

        [HttpPost]
        public ActionResult Edit(DivisionUnitBudgetAllocationModel model)
        {
            string businessError = string.Empty;
            //Message = CheckBusinessRule(model);

            if (ModelState.IsValid)
            {
                try
                {
                    businessError = CheckBusinessLogicDivisionUnitBudgetAllocationEdit(model.FinancialYearId);
                    if(string.IsNullOrEmpty(businessError))
                    {
                        var entity = model.ToEntity();
                        var navigationList = new Dictionary<Type, ArrayList>();
                        var childEntities = new ArrayList();

                        model.BudgetAllocationList.ToList().ForEach(x => x.RevisedAmount = x.Amount);
                        model.BudgetAllocationList.ToList().ForEach(x => x.DivisionUnitBudgetId = model.Id);
                        model.BudgetAllocationList.ToList().ForEach(x => childEntities.Add(x.ToEntityChild()));

                        navigationList.Add(typeof(FAM_DivisionUnitBudgetAllocationDetails), childEntities);
                        _famCommonService.FAMUnit.DivisionUnitBudgetAllocationRepository.Update(entity, navigationList);
                        _famCommonService.FAMUnit.DivisionUnitBudgetAllocationRepository.SaveChanges();
                        //Message = ErrorMessages.UpdateSuccessful;
                    }
                }
                catch (Exception)
                {

                    return new JsonResult()
                    {
                        Data = ErrorMessages.UpdateFailed
                    };
                }

                return new JsonResult()
                {
                    Data = String.IsNullOrEmpty(businessError) ? ErrorMessages.UpdateSuccessful : businessError
                };
            }
            return new JsonResult()
            {
                Data = ErrorMessages.UpdateFailed
            };
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var entity = _famCommonService.FAMUnit.DivisionUnitBudgetAllocationRepository.GetByID(id);
            string businessError = string.Empty;
            try
            {
                businessError = CheckBusinessLogicDivisionUnitBudgetAllocationDelete(id);
                if (string.IsNullOrEmpty(businessError))
                {
                    List<Type> allTypes = new List<Type> { typeof(FAM_DivisionUnitBudgetAllocationDetails) };
                    _famCommonService.FAMUnit.DivisionUnitBudgetAllocationRepository.Delete(entity.Id, allTypes);
                    _famCommonService.FAMUnit.DivisionUnitBudgetAllocationRepository.SaveChanges();

                    return Json(new
                    {
                        Success = 1,
                        Message = ErrorMessages.DeleteSuccessful
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                return Json(new
                {
                    Success = 0,
                    Message = ErrorMessages.DeleteFailed
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Success = 1,
                Message = ErrorMessages.DeleteFailed
            }, JsonRequestBehavior.AllowGet);
        }
        
        #endregion

        #region Utils
        private void PrepareModel(DivisionUnitBudgetAllocationModel model)
        {
            model.DivisionUnitList = Common.PopulateDllList(_prmCommonService.PRMUnit.DivisionRepository.GetAll()).ToList();
            model.FinancialYearList = Common.PopulateFinancialYearDllList(_famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll().ToList());
        }

        [NoCache]
        public ActionResult DivisionforView()
        {

            var itemList = Common.PopulateDllList(_prmCommonService.PRMUnit.DivisionRepository.GetAll().ToList());
            return PartialView("_Select", itemList);
        }

        [NoCache]
        public ActionResult FinancialYearforView()
        {
            var itemList = Common.PopulateFinancialYearDllList(_famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll()).ToList();
            return PartialView("_Select", itemList);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetBudgetHeadByDivisionId(int Id)
        {
            var model = new DivisionUnitBudgetAllocationModel();
            var data = (from d in _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.GetAll()
                        join coa in _famCommonService.FAMUnit.ChartOfAccount.GetAll() on d.AccountHeadId equals coa.Id
                        where d.DivisionUnitId == Id
                        orderby coa.AccountHeadType descending
                        select new
                        {
                            d.DivisionUnitId,
                            d.AccountHeadId,
                            coa.AccountHeadCode,
                            coa.AccountHeadName,
                            coa.AccountHeadType
                        }).Distinct().ToList();

            foreach (var item in data)
            {
                var innerModel = new BudgetAllocation()
                {
                    AccountHeadId = item.AccountHeadId,
                    //AccountHeadName = item.AccountHeadName,
                    AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
                    Amount = 0,
                    AccountHeadType = item.AccountHeadType == "I" ? "Income" : "Expense"
                };
                model.BudgetAllocationList.Add(innerModel);
            }
            return PartialView("_BudgetAllocationList", model);
        }

        public string CheckBusinessLogicDivisionUnitBudgetAllocationDelete(int id)
        {
            string businessError = string.Empty;

            int divisionBudgetFYId = (from c in _famCommonService.FAMUnit.DivisionUnitBudgetAllocationRepository.Fetch()
                                      where c.Id == id
                                      select c.FinancialYearId).SingleOrDefault();
                 

            int? centralBudgetFinancialYearId = (from c in _famCommonService.FAMUnit.CentralBudgetInformationRepository.Fetch()
                                                 where c.FinancialYearId == divisionBudgetFYId
                                        select c.FinancialYearId).FirstOrDefault();

            if (centralBudgetFinancialYearId.HasValue && (centralBudgetFinancialYearId > 0))
            {
                businessError = "Can not delete budget information, because central budget has been created for the financial year.";
                return businessError;
            }

            return string.Empty;
        }

        public string CheckBusinessLogicDivisionUnitBudgetAllocationEdit(int id)
        {
            string businessError = string.Empty;

            int? centralBudgetFinancialYearId = (from c in _famCommonService.FAMUnit.CentralBudgetInformationRepository.Fetch()
                                                 where c.FinancialYearId == id
                                                 select c.FinancialYearId).FirstOrDefault();

            if (centralBudgetFinancialYearId.HasValue && (centralBudgetFinancialYearId > 0))
            {
                businessError = "Can not update budget information, because central budget has been created for the financial year.";
                return businessError;
            }

            return string.Empty;
        }

        public string CheckBusinessLogicDivisionUnitBudgetAllocationInsert(int id)
        {
            string businessError = string.Empty;

            int? centralBudgetFinancialYearId = (from c in _famCommonService.FAMUnit.CentralBudgetInformationRepository.Fetch()
                                                 where c.FinancialYearId == id
                                                 select c.FinancialYearId).FirstOrDefault();

            if (centralBudgetFinancialYearId.HasValue && (centralBudgetFinancialYearId > 0))
            {
                businessError = "Can not add budget information, because central budget has been created for the financial year.";
                return businessError;
            }

            return string.Empty;
        }
        #endregion
    }
}
