using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.DAL.FAM;
using BEPZA_MEDICAL.Domain.FAM;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.DivisionUnitBudgetRevision;
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
    public class DivisionUnitBudgetRevisionController : Controller
    {
        #region Fields
        private readonly FAMCommonService _famCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public DivisionUnitBudgetRevisionController(FAMCommonService famCommonService, PRMCommonSevice prmCommonSevice)
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
            var model = new DivisionUnitBudgetRevisionModel();
            PrepareModel(model);
            return View("_Index", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, DivisionUnitBudgetRevisionModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            dynamic list = null;

            list = _famCommonService.GetDivisionUnitFYRevisionSearchList(model.DivisionUnitId, model.FinancialYearId);

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
                    d.RevisionNo,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new DivisionUnitBudgetRevisionModel();
            
            model.DivisionUnitList = Common.PopulateDllList(_prmCommonService.PRMUnit.DivisionRepository.GetAll()).ToList();
            model.FinancialYearList = Common.PopulateFinancialYearDllList(_famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll()).ToList();

            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(DivisionUnitBudgetRevisionModel model)
        {
            string businessError = string.Empty;
            string strMessage = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    businessError = CheckBusinessLogicRevisionNoInsert(model);
                    if (string.IsNullOrEmpty(businessError))
                    {
                        var entity = model.ToEntity();
                        foreach (var item in model.DivisionUnitBudgetRevisionList)
                        {
                            var childEntity = item.ToEntityChild();
                            entity.FAM_DivisionUnitBudgetRevisionDetails.Add(childEntity);
                        }

                        //Update Revised Budget Amount of Division/Unit budget
                        UpdateDivisionUnitRevisedBudget(model);

                        _famCommonService.FAMUnit.DivisionUnitBudgetRevisionRepository.Add(entity);
                        _famCommonService.FAMUnit.DivisionUnitBudgetRevisionRepository.SaveChanges();



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

                return new JsonResult()
                {
                    Data = String.IsNullOrEmpty(businessError) ? ErrorMessages.InsertSuccessful : businessError
                };

            }
            else
            {
                strMessage = "Please check all revised budget amount.";
            }

            return new JsonResult()
            {
                Data = strMessage
            };
        }

        public ActionResult Edit(int id)
        {
            var mainModel = new DivisionUnitBudgetRevisionModel();
            var entity = _famCommonService.FAMUnit.DivisionUnitBudgetRevisionRepository.GetByID(id);

            var model = entity.ToModel();

            mainModel.RevisionNo = model.RevisionNo;
            mainModel.Remarks = model.Remarks;
            mainModel.DivisionUnitId = model.DivisionUnitId;
            mainModel.FinancialYearId = model.FinancialYearId;
            mainModel.Id = model.Id;


            var data = (from d in _famCommonService.FAMUnit.DivisionUnitBudgetRevisionRepository.GetAll()
                        join dtl in _famCommonService.FAMUnit.DivisionUnitBudgetRevisionDetailRepository.GetAll() on d.Id equals dtl.DivisionUnitBudgetRevisionId
                        join coa in _famCommonService.FAMUnit.ChartOfAccount.GetAll() on dtl.RevisionAccountHeadId equals coa.Id
                        where d.DivisionUnitId == model.DivisionUnitId && d.FinancialYearId == model.FinancialYearId && d.RevisionNo == model.RevisionNo
                        select new
                        {
                            d.DivisionUnitId,
                            dtl.RevisionAccountHeadId,
                            dtl.PreviousBudget,
                            dtl.RevisedBudget,
                            dtl.Remarks,
                            coa.AccountHeadCode,
                            coa.AccountHeadName,
                            coa.AccountHeadType
                        }).Distinct().ToList();

            foreach (var item in data)
            {
                var innerModel = new DivisionUnitBudgetRevision()
                {
                    RevisionAccountHeadId = item.RevisionAccountHeadId,
                    AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
                    PreviousBudget = item.PreviousBudget,
                    RevisedBudget = item.RevisedBudget,
                    Remarks = item.Remarks,
                    AccountHeadType = item.AccountHeadType == "I" ? "Income" : "Expense"
                };

                mainModel.DivisionUnitBudgetRevisionList.Add(innerModel);
            }

            mainModel.DivisionUnitList = Common.PopulateDllList(_prmCommonService.PRMUnit.DivisionRepository.GetAll()).ToList();
            mainModel.FinancialYearList = Common.PopulateFinancialYearDllList(_famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll()).ToList();
            mainModel.Mode = "Edit";
            return View("_CreateOrEdit", mainModel);
        }

        [HttpPost]
        public ActionResult Edit(DivisionUnitBudgetRevisionModel model)
        {
            string businessError = string.Empty;
            
            if (ModelState.IsValid)
            {
                try
                {
                    businessError = CheckBusinessLogicRevisionNoUpdate(model);
                    if (string.IsNullOrEmpty(businessError))
                    {
                        var entity = model.ToEntity();
                        var navigationList = new Dictionary<Type, ArrayList>();
                        var childEntities = new ArrayList();
                        model.DivisionUnitBudgetRevisionList.ToList().ForEach(x => x.DivisionUnitBudgetRevisionId = model.Id);
                        model.DivisionUnitBudgetRevisionList.ToList().ForEach(x => childEntities.Add(x.ToEntityChild()));

                        navigationList.Add(typeof(FAM_DivisionUnitBudgetRevisionDetails), childEntities);
                        _famCommonService.FAMUnit.DivisionUnitBudgetRevisionRepository.Update(entity, navigationList);
                        _famCommonService.FAMUnit.DivisionUnitBudgetRevisionRepository.SaveChanges();

                        //Update Revised Budget Amount of Division/Unit budget
                        UpdateDivisionUnitRevisedBudget(model);
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
            var entity = _famCommonService.FAMUnit.DivisionUnitBudgetRevisionRepository.GetByID(id);

            try
            {
                List<Type> allTypes = new List<Type> { typeof(FAM_DivisionUnitBudgetRevisionDetails) };
                _famCommonService.FAMUnit.DivisionUnitBudgetRevisionRepository.Delete(entity.Id, allTypes);
                _famCommonService.FAMUnit.DivisionUnitBudgetRevisionRepository.SaveChanges();

                return Json(new
                {
                    Success = 1,
                    Message = ErrorMessages.DeleteSuccessful
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new
                {
                    Success = 0,
                    Message = ErrorMessages.DeleteFailed
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Utils
        private void PrepareModel(DivisionUnitBudgetRevisionModel model)
        {
            model.DivisionUnitList = Common.PopulateDllList(_prmCommonService.PRMUnit.DivisionRepository.GetAll()).ToList();
            model.FinancialYearList = Common.PopulateFinancialYearDllList(_famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll()).ToList();
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

        //[AcceptVerbs(HttpVerbs.Get)]
        //public ActionResult GetBudgetHeadByDivisionId(int Id)
        //{
        //    var model = new DivisionUnitBudgetRevisionModel();
        //    var data = (from d in _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.GetAll()
        //                join coa in _famCommonService.FAMUnit.ChartOfAccount.GetAll() on d.AccountHeadId equals coa.Id
        //                where d.DivisionUnitId == Id
        //                orderby coa.AccountHeadType descending
        //                select new
        //                {
        //                    d.DivisionUnitId,
        //                    d.AccountHeadId,
        //                    coa.AccountHeadCode,
        //                    coa.AccountHeadName,
        //                    coa.AccountHeadType
        //                }).Distinct().ToList();

        //    foreach (var item in data)
        //    {
        //        var innerModel = new BudgetAllocation()
        //        {
        //            AccountHeadId = item.AccountHeadId,
        //            //AccountHeadName = item.AccountHeadName,
        //            AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
        //            Amount = 0,
        //            AccountHeadType = item.AccountHeadType == "I" ? "Income" : "Expense"
        //        };
        //        model.BudgetAllocationList.Add(innerModel);
        //    }
        //    return PartialView("_BudgetAllocationList", model);
        //}

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetDivisionUnitBudgetByDUFY( int? DivisionUnitId=0, int? FinancialYearId=0)
        {
            var model = new DivisionUnitBudgetRevisionModel();

            var data = _famCommonService.GetRevisionDivisionUnitBudgetByDUFYSearchList(DivisionUnitId,FinancialYearId);
            foreach (var item in data)
            {
                var innerModel = new DivisionUnitBudgetRevision()
                {
                    RevisionAccountHeadId = item.AccountHeadId,
                    AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
                    PreviousBudget = item.RevisedAmount,
                    //RevisedBudget = item.RevisedAmount,
                    AccountHeadType = item.AccountHeadType == "I" ? "Income" : "Expense"
                };
                model.DivisionUnitBudgetRevisionList.Add(innerModel);
            }
            return PartialView("_BudgetAllocationList", model);
        }

        private void UpdateDivisionUnitRevisedBudget(DivisionUnitBudgetRevisionModel model)
        {
            var dbDivisionUnitBudget = (from bm in _famCommonService.FAMUnit.DivisionUnitBudgetAllocationRepository.GetAll()
                                        join bd in _famCommonService.FAMUnit.BudgetAllocationRepository.GetAll() on bm.Id equals bd.DivisionUnitBudgetId
                                        where bm.FinancialYearId == model.FinancialYearId && bm.DivisionUnitId == model.DivisionUnitId
                                        select bd).ToList();

            foreach (var item in dbDivisionUnitBudget)
            {
                foreach (var revItem in model.DivisionUnitBudgetRevisionList)
                {
                    if (item.AccountHeadId == revItem.RevisionAccountHeadId)
                    {
                        item.RevisedAmount = revItem.RevisedBudget;
                    }
                }
            }

            foreach (var item in dbDivisionUnitBudget)
            {
                _famCommonService.FAMUnit.BudgetAllocationRepository.Update(item);
            }
            _famCommonService.FAMUnit.BudgetAllocationRepository.SaveChanges();
        }

        public string CheckBusinessLogicRevisionNoUpdate(DivisionUnitBudgetRevisionModel model)
        {
            string businessError = string.Empty;

            int? revisionNo = (from c in _famCommonService.FAMUnit.DivisionUnitBudgetRevisionRepository.Fetch()
                                   where c.DivisionUnitId==model.DivisionUnitId && c.FinancialYearId==model.FinancialYearId && c.RevisionNo > model.RevisionNo
                                   select c.RevisionNo).FirstOrDefault();

            if (revisionNo.HasValue && (revisionNo > 0))
            {
                businessError = "Budget Revision cannot be updated, because only last revision no is allowed to update.";
                return businessError;
            }

            return string.Empty;

        }

        public string CheckBusinessLogicRevisionNoInsert(DivisionUnitBudgetRevisionModel model)
        {
            string businessError = string.Empty;

            int? revisionNo = (from c in _famCommonService.FAMUnit.DivisionUnitBudgetRevisionRepository.Fetch()
                               where c.DivisionUnitId == model.DivisionUnitId && c.FinancialYearId == model.FinancialYearId && c.RevisionNo > model.RevisionNo
                               select c.RevisionNo).FirstOrDefault();

            if (revisionNo.HasValue && (revisionNo > 0))
            {
                businessError = "Budget Revision cannot be saved, because revision no cannot be smaller than existing revision no.";
                return businessError;
            }

            return string.Empty;
        }
        #endregion
    }
}
