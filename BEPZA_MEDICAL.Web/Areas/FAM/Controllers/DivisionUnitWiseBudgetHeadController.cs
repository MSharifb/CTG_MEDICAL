using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.DAL.FAM;
using BEPZA_MEDICAL.Domain.FAM;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.DivisionUnitWiseBudgetHead;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Resources;
using System.Data.SqlClient;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{ 
    public class DivisionUnitWiseBudgetHeadController : Controller
    {

        #region Fields
        private readonly FAMCommonService _famCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        
        #endregion

        #region Ctor
        public DivisionUnitWiseBudgetHeadController(FAMCommonService famCommonService, PRMCommonSevice prmCommonService)
        {
            _famCommonService = famCommonService;
            _prmCommonService = prmCommonService;
            
        }
        #endregion

        #region Actions
        public ViewResult Index()
        {
            return View();
        }

        public ActionResult BackToList()
        {
            var model = new DivisionUnitWiseBudgetHeadModel();
            PrepareModel(model);
            return View("_Index", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, DivisionUnitWiseBudgetHeadSearchModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            dynamic list = null;

            list = _famCommonService.GetDivisionSearchedList(model.DivisionUnitId);

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
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new DivisionUnitWiseBudgetHeadModel();
            //Get Account Head

            model.DivisionUnitList = Common.PopulateDllList(_prmCommonService.PRMUnit.DivisionRepository.GetAll().ToList());
            IEnumerable<FAM_ChartOfAccount> accountHead = _famCommonService.FAMUnit.ChartOfAccount.GetAll().ToList()
                .Where(x => (bool)x.IsPostingAccount && (x.AccountHeadType == "E" || x.AccountHeadType == "I")).OrderByDescending(at=> at.AccountHeadType);

            foreach (var item in accountHead)
            {
                var innerModel = new BudgetHeadAllocation()
                {
                    AccountHeadId = item.Id,
                    //AccountHeadName = item.AccountHeadName,
                    AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
                    AccountHeadType = item.AccountHeadType == "I"? "Income":"Expense",
                    IsSelected = false
                };

                model.BudgetHeadAllocationList.Add(innerModel);
            }
            

            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(DivisionUnitWiseBudgetHeadModel model)
        {
            string businessError = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();
                    if (model.BudgetHeadAllocationList.Where(x => x.IsSelected).Count() <= 0)
                    {
                        businessError = "Select Account Head.";
                    }
                    else
                    {
                        foreach (var item in model.BudgetHeadAllocationList)
                        {
                            var childEntity = item.ToEntity();
                            childEntity.DivisionUnitId = entity.DivisionUnitId;
                            if (item.IsSelected)
                            {
                                _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.Add(childEntity);
                            }
                        }
                        _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.SaveChanges();
                    }
                    
                }
                catch (Exception)
                {

                    return new JsonResult()
                    {
                        Data = ErrorMessages.InsertFailed
                    };
                }

                return new JsonResult()
                {
                    Data = string.IsNullOrEmpty(businessError) ? ErrorMessages.InsertSuccessful : businessError
                };
            }

            var errors = ModelState
                           .Where(x => x.Value.Errors.Count > 0)
                           .Select(x => new { x.Key, x.Value.Errors })
                           .ToArray();

            return new JsonResult()
            {
                Data = errors.Count() > 0 ? errors.First().Errors.First().ErrorMessage : ""
            };
        }

        public ActionResult Edit(int Id)
        {
            //Get Saved Budget Head
            var modeSaved = new DivisionUnitWiseBudgetHeadModel();
            var entity = _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.GetAll().Where(x => x.DivisionUnitId == Id).ToList();

            foreach (var item in entity)
            {
                modeSaved.BudgetHeadAllocationList.Add(item.ToChildModel());
            }

            //Get All Posting (Income/Expense) Account Head
            var modelAll = new DivisionUnitWiseBudgetHeadModel();
            modelAll.DivisionUnitId = Id;
            IEnumerable<FAM_ChartOfAccount> accountHead = _famCommonService.FAMUnit.ChartOfAccount.GetAll().ToList()
                 .Where(x => (bool)x.IsPostingAccount && (x.AccountHeadType == "E" || x.AccountHeadType == "I")).OrderByDescending(at=> at.AccountHeadType);

            foreach (var item in accountHead)
            {
                var innerModel = new BudgetHeadAllocation()
                {
                    AccountHeadId = item.Id,
                    AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
                    AccountHeadType = item.AccountHeadType == "I" ? "Income" : "Expense",
                    IsSelected = modeSaved.BudgetHeadAllocationList.Where(d => d.AccountHeadId == item.Id).FirstOrDefault() != null ? true : false
                    
                };
                
                modelAll.BudgetHeadAllocationList.Add(innerModel);
            }


            PrepareModel(modelAll);
            modelAll.Mode = "Edit";
            return View("_CreateOrEdit", modelAll);
        }

        [HttpPost]
        public ActionResult Edit(DivisionUnitWiseBudgetHeadModel model)
        {

            if (ModelState.IsValid)
            {
                string businessError = string.Empty;
                try
                {
                    if (model.BudgetHeadAllocationList.Where(x => x.IsSelected).Count() <= 0)
                    {
                        businessError = "Select Account Head.";
                    }
                    else
                    {
                        //Get Saved Budget Head
                        var modeSaved = new DivisionUnitWiseBudgetHeadModel();
                        var entitySaved = _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.GetAll().Where(x => x.DivisionUnitId == model.DivisionUnitId).ToList();
                        foreach (var item in entitySaved)
                        {
                            modeSaved.BudgetHeadAllocationList.Add(item.ToChildModel());
                        }


                        //Delete Budget Head that has been ommited in Edit
                        List<Type> allTypes = new List<Type> { typeof(FAM_DivisionUnitBudgetHead) };
                        var entity = _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.GetAll().Where(x => x.DivisionUnitId == model.DivisionUnitId).ToList();
                        foreach (var item in entity)
                        {
                            if (model.BudgetHeadAllocationList.Where(x => x.AccountHeadId == item.AccountHeadId && x.IsSelected == false).Count() > 0)
                            {
                                _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.Delete(item.Id);
                            }
                        }
                        _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.SaveChanges();


                        //Insert Budget Head that has been added in Edit
                        var entityAdd = model.ToEntity();
                        foreach (var item in model.BudgetHeadAllocationList)
                        {
                            var childEntity = item.ToEntity();
                            childEntity.DivisionUnitId = entityAdd.DivisionUnitId;
                            if (item.IsSelected)
                            {
                                if (modeSaved.BudgetHeadAllocationList.Where(x => x.AccountHeadId == item.AccountHeadId).Count() == 0)// .Contains(item))//.ToList().Exists(d => d.AccountHeadId != item.AccountHeadId))//).FirstOrDefault() != null)
                                {
                                    _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.Add(childEntity);
                                }
                            }
                        }
                        _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.SaveChanges();
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



        [HttpPost, ActionName("Delete")]
        public JsonResult Delete(int id)
        {
            //bool result;
            //string errMsg = "Error while deleting data!";
            string businessError = string.Empty;
            try
            {
                
                List<Type> allTypes = new List<Type> { typeof(FAM_DivisionUnitBudgetHead) };
                
                var entity = _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.GetAll().Where(x => x.DivisionUnitId == id).ToList();
                businessError = CheckBusinessLogicDivisionUnitWiseBudgetHead(id);
                if (string.IsNullOrEmpty(businessError))
                {
                    foreach (var item in entity)
                    {
                        _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.Delete(item.Id);
                    }

                    _famCommonService.FAMUnit.DivisionUnitBudgetHeadRepository.SaveChanges();

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
                Message = businessError
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Utils
        private void PrepareModel(DivisionUnitWiseBudgetHeadModel model)
        {
            model.DivisionUnitList = Common.PopulateDllList(_prmCommonService.PRMUnit.DivisionRepository.GetAll()).ToList();
        }

        [NoCache]
        public ActionResult DivisionforView()
        {

            var itemList = Common.PopulateDllList(_prmCommonService.PRMUnit.DivisionRepository.GetAll().ToList());
            return PartialView("_Select", itemList);
        }
        #endregion

        #region Business Logic Validation
        public string CheckBusinessLogicDivisionUnitWiseBudgetHead(int obj)
        {
            string businessError = string.Empty;

            int? divisionUnitId = (from c in _famCommonService.FAMUnit.DivisionUnitBudgetAllocationRepository.Fetch()
                                   where c.DivisionUnitId == obj
                                   select c.DivisionUnitId).FirstOrDefault();

            if (divisionUnitId.HasValue && (divisionUnitId>0))
            {
                businessError = "The Division cannot be deleted, because it has been used in Budget Allocation.";
                return businessError;
            }

            return string.Empty;
           
            
        }
        #endregion


    }
}