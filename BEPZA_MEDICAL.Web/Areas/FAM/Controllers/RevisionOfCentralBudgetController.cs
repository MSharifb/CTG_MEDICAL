using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.DAL.FAM;
using BEPZA_MEDICAL.Domain.FAM;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Resources;
using System.Data.SqlClient;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using System.Collections;
using System.Web.Services.Description;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.RevisionOfCentralBudget;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class RevisionOfCentralBudgetController : Controller
    {
        #region Fields
        private readonly FAMCommonService _famCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public RevisionOfCentralBudgetController(FAMCommonService famCommonService, PRMCommonSevice prmCommonService)
        {
            _famCommonService = famCommonService;
            _prmCommonService = prmCommonService; 
        }

        #endregion
        
        #region Actions

        public ActionResult Index()
        { 
            return View();
        }

        public ActionResult BackToList()
        {
            var model = new RevisionOfCentralBudgetModel();
            PrepareModel(model);
            return View("_Index", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, RevisionOfCentralBudgetModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            dynamic list = null;

            list = _famCommonService.GetRevisionOfCentralBudgetFYSearchedList(model.FinancialYearId);

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
                    d.FinancialYearName,
                    d.RevisionNo,
                    d.Remarks,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new RevisionOfCentralBudgetModel();
            model.FinancialYearList = Common.PopulateFinancialYearDllList(_famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll()).ToList();
            model.ApprovalPathList = Common.PopulateApprovalPathDllList(_famCommonService.FAMUnit.ApprovalPathMaster.GetAll()).ToList();
            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(RevisionOfCentralBudgetModel model)
        {
            string businessError = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    model.ApprovalStatus = "Draft";

                    var entity = model.ToEntity();

                    foreach (var item in model.CentralBudgetRevisionList)
                    {
                        var childEntity = item.ToEntityChild();
                        entity.FAM_CentralBudgetRevisionDetails.Add(childEntity);
                    }
                    _famCommonService.FAMUnit.RevisionCentralBudgetInformationRepository.Add(entity);
                    _famCommonService.FAMUnit.RevisionCentralBudgetInformationRepository.SaveChanges();
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

        public ActionResult Edit(int id)
        {
            var mainModel = new RevisionOfCentralBudgetModel();
            var entity = _famCommonService.FAMUnit.RevisionCentralBudgetInformationRepository.GetByID(id);

            var model = entity.ToModel();

            mainModel.FinancialYearId = model.FinancialYearId;
            mainModel.ApprovalPathId = model.ApprovalPathId;
            mainModel.CurrentApprovalNodeId = model.CurrentApprovalNodeId;
            mainModel.Remarks = model.Remarks;
            mainModel.ApprovalStatus = model.ApprovalStatus;
            mainModel.RevisionNo = model.RevisionNo;
            mainModel.Id = model.Id;
            //int revisionNo1 = _famCommonService.FAMUnit.RevisionCentralBudgetInformationRepository.GetAll().Where(z=> z.FinancialYearId == model.FinancialYearId).OrderBy(x=> x.RevisionNo).ThenByDescending(y=>y.RevisionNo).FirstOrDefault().RevisionNo;
            int revisionNo = _famCommonService.FAMUnit.RevisionCentralBudgetInformationRepository.GetAll().Where(x => x.FinancialYearId == model.FinancialYearId).OrderBy(y => y.RevisionNo).ThenByDescending(z => z.RevisionNo).FirstOrDefault().RevisionNo;

            var data = (from d in _famCommonService.FAMUnit.RevisionCentralBudgetInformationRepository.GetAll()
                        join dtl in _famCommonService.FAMUnit.RevisionCentralBudgetInformationDetailRepository.GetAll() on d.Id equals dtl.CentralBudgetRevisionId
                        join coa in _famCommonService.FAMUnit.ChartOfAccount.GetAll() on dtl.RevisionAccountHeadId equals coa.Id
                        where d.FinancialYearId == model.FinancialYearId && d.RevisionNo == revisionNo
                        select new
                        {
                            d.FinancialYearId,
                            dtl.RevisionAccountHeadId,
                            dtl.PreviousBudget,
                            dtl.RevisedBudget,
                            coa.AccountHeadCode,
                            coa.AccountHeadName,
                            coa.AccountHeadType,
                            dtl.Remarks
                        }).Distinct().ToList();

            foreach (var item in data)
            {
                var innerModel = new CentralBudgetRevision()
                {
                    RevisionAccountHeadId = item.RevisionAccountHeadId,
                    AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
                    PreviousBudget = item.PreviousBudget,
                    RevisedBudget = item.RevisedBudget,
                    AccountHeadType = item.AccountHeadType == "I" ? "Income" : "Expense",
                    Remarks = item.Remarks
                };

                mainModel.CentralBudgetRevisionList.Add(innerModel);
            }

            mainModel.FinancialYearList = Common.PopulateFinancialYearDllList(_famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll()).ToList();
            mainModel.ApprovalPathList = Common.PopulateApprovalPathDllList(_famCommonService.FAMUnit.ApprovalPathMaster.GetAll()).ToList();

            var empList = (from appM in _famCommonService.FAMUnit.ApprovalPathMaster.GetAll()
                           join appD in _famCommonService.FAMUnit.ApprovalPathDetails.GetAll() on appM.PathId equals appD.PathId
                           join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on appD.NodeEmpId equals emp.Id
                           where appM.PathId == mainModel.ApprovalPathId
                           select emp
                          ).Distinct().ToList();
            mainModel.NextApprovalNodeList = Common.PopulateEmployeeDDL(empList);

            mainModel.Mode = "Edit";
            return View("_CreateOrEdit", mainModel);
        }

        [HttpPost]
        public ActionResult Edit(RevisionOfCentralBudgetModel model)
        {
            string businessError = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {

                    var entity = model.ToEntity();
                    var navigationList = new Dictionary<Type, ArrayList>();
                    var childEntities = new ArrayList();
                    model.CentralBudgetRevisionList.ToList().ForEach(x => x.CentralBudgetRevisionId = model.Id);
                    model.CentralBudgetRevisionList.ToList().ForEach(x => childEntities.Add(x.ToEntityChild()));

                    navigationList.Add(typeof(FAM_CentralBudgetRevisionDetails), childEntities);
                    _famCommonService.FAMUnit.RevisionCentralBudgetInformationRepository.Update(entity, navigationList);
                    _famCommonService.FAMUnit.RevisionCentralBudgetInformationRepository.SaveChanges();
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
            var entity = _famCommonService.FAMUnit.RevisionCentralBudgetInformationRepository.GetByID(id);

            try
            {
                List<Type> allTypes = new List<Type> { typeof(FAM_CentralBudgetRevisionDetails) };
                _famCommonService.FAMUnit.RevisionCentralBudgetInformationRepository.Delete(entity.Id, allTypes);
                _famCommonService.FAMUnit.RevisionCentralBudgetInformationRepository.SaveChanges();

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
        
        private void PrepareModel(RevisionOfCentralBudgetModel model)
        {
            model.FinancialYearList = Common.PopulateFinancialYearDllList(_famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll()).ToList();
            model.ApprovalPathList = Common.PopulateApprovalPathDllList(_famCommonService.FAMUnit.ApprovalPathMaster.GetAll()).ToList();
        }

        [NoCache]
        public ActionResult FinancialYearforView()
        {
            var itemList = Common.PopulateFinancialYearDllList(_famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll()).ToList();
            return PartialView("_Select", itemList);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetEmpList(int id)
        {
            var empList = (from appM in _famCommonService.FAMUnit.ApprovalPathMaster.GetAll()
                           join appD in _famCommonService.FAMUnit.ApprovalPathDetails.GetAll() on appM.PathId equals appD.PathId
                           join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on appD.NodeEmpId equals emp.Id
                           where appM.PathId == id
                           select emp
                          ).Distinct().ToList();

            return PartialView("_Selectemp", Common.PopulateEmployeeDDL(empList));
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetBudgetByFinancialYearId(int Id)
        {
            var model = new RevisionOfCentralBudgetModel();

            var data = _famCommonService.GetRevisionCentralBudgetAllocationFYSearchList(Id);
            foreach (var item in data)
            {
                var innerModel = new CentralBudgetRevision()
                {
                    RevisionAccountHeadId = item.AccountHeadId,
                    //AccountHeadName = item.AccountHeadName,
                    AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
                    PreviousBudget = item.RevisedAmount,
                    //RevisedBudget = item.RevisedAmount,
                    AccountHeadType = item.AccountHeadType == "I" ? "Income" : "Expense"
                };
                model.CentralBudgetRevisionList.Add(innerModel);
            }
            return PartialView("_CentralBudgetRevisionList", model);
        }
        
        #endregion
    }
}
