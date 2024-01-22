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
using BEPZA_MEDICAL.Web.Areas.FAM.Models.CentralBudgetInformation;
using BEPZA_MEDICAL.Web.SecurityService;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class CentralBudgetInformationController : Controller
    {
        #region Fields
        private readonly FAMCommonService _famCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        private readonly EmployeeService _employeeService;
        private UserManagementServiceClient _userAgent;
        private User user;
        private int userEmployeeId;
        #endregion

        #region Ctor
        public CentralBudgetInformationController(FAMCommonService famCommonService, PRMCommonSevice prmCommonService, EmployeeService employeeService)
        {
            _famCommonService = famCommonService;
            _prmCommonService = prmCommonService;
            _employeeService = employeeService;
            _userAgent = new UserManagementServiceClient();
        }
        #endregion

        #region Actions
        public ViewResult Index()
        {
            return View();
        }

        public ActionResult BackToList()
        {
            var model = new CentralBudgetInformationModel();
            PrepareModel(model);
            return View("_Index", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, CentralBudgetInformationModel model)
        {
            user = _userAgent.GetUserByLoginId(User.Identity.Name);
            userEmployeeId = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(c => c.EmpID == user.EmpId).FirstOrDefault().Id;

            string filterExpression = String.Empty;
            int totalRecords = 0;
            dynamic list = null;

            //if (model.PendingApproval == "1")
            //{
            //    list = _famCommonService.GetCentralBudgetFYRecommendApproveSearchedList(model.FinancialYearId, _employeeService.GetEmployeeInfoByEmployeeId(Common.User.EmpId).Id);
            //}
            //else
            //{
            list = _famCommonService.GetCentralBudgetFYSearchedList(model.FinancialYearId, User.Identity.Name, userEmployeeId);                
            //}
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
                    d.ApprovalStatus,
                    d.Remarks,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new CentralBudgetInformationModel();
            model.FinancialYearList = Common.PopulateFinancialYearDllList(_famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll()).ToList();
            model.ApprovalPathList = Common.PopulateApprovalPathDllList(_famCommonService.FAMUnit.ApprovalPathMaster.GetAll()).ToList();
            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(CentralBudgetInformationModel model)
        {
            PrepareModelForSave(model);
            string businessError = string.Empty;
            string strMessage = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    //model.ApprovalStatus = "Draft";
                    
                    var entity = model.ToEntity();

                    foreach (var item in model.CentralBudgetList)
                    {
                        item.RevisedAmount = item.OriginalAmount;
                        var childEntity = item.ToEntityChild();
                        entity.FAM_CentralBudgetAllocationDetails.Add(childEntity);
                    }
                    _famCommonService.FAMUnit.CentralBudgetInformationRepository.Add(entity);
                    _famCommonService.FAMUnit.CentralBudgetInformationRepository.SaveChanges();
                    strMessage = ErrorMessages.InsertSuccessful;
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

            return new JsonResult()
            {
                Data = strMessage
            };
            
        }

        public ActionResult Edit(int id)
        {
            var mainModel = new CentralBudgetInformationModel();
            var entity = _famCommonService.FAMUnit.CentralBudgetInformationRepository.GetByID(id);
            
            var model = entity.ToModel();

            mainModel.FinancialYearId = model.FinancialYearId;
            mainModel.ApprovalPathId = model.ApprovalPathId;
            mainModel.CurrentApprovalNodeId = model.CurrentApprovalNodeId;
            mainModel.Remarks = model.Remarks;
            mainModel.ApprovalStatus = model.ApprovalStatus;
            mainModel.Id = model.Id;

            user = _userAgent.GetUserByLoginId(User.Identity.Name);
            userEmployeeId = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(c => c.EmpID == user.EmpId).FirstOrDefault().Id;
            if (mainModel.CurrentApprovalNodeId == userEmployeeId)
            {
                //Check Recommended Or Approved for 'Recommend' Or 'Approve' Button View 
                mainModel.recommenderOrApprover = (from c in _famCommonService.FAMUnit.ApprovalPathDetails.GetAll()
                                               where c.NodeEmpId == model.CurrentApprovalNodeId & c.PathId == model.ApprovalPathId
                                               select c.ApprovalType).FirstOrDefault();
            }


            var data = (from d in _famCommonService.FAMUnit.CentralBudgetInformationRepository.GetAll()
                        join dtl in _famCommonService.FAMUnit.CentralBudgetInformationDetailRepository.GetAll() on d.Id equals dtl.CentralBudgetId
                        join coa in _famCommonService.FAMUnit.ChartOfAccount.GetAll() on dtl.AccountHeadId equals coa.Id
                        where d.FinancialYearId == model.FinancialYearId
                        select new
                        {
                            d.FinancialYearId,
                            dtl.AccountHeadId,
                            dtl.OriginalAmount,
                            dtl.RevisedAmount,
                            coa.AccountHeadCode,
                            coa.AccountHeadName,
                            coa.AccountHeadType,
                            dtl.Remarks
                        }).Distinct().ToList();

            foreach (var item in data)
            {
                var innerModel = new CentralBudget()
                {
                    AccountHeadId = item.AccountHeadId,
                    //AccountHeadName = item.AccountHeadName,
                    AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
                    OriginalAmount = item.OriginalAmount,
                    RevisedAmount = item.RevisedAmount,
                    AccountHeadType = item.AccountHeadType == "I" ? "Income" : "Expense",
                    Remarks = item.Remarks
                };

                mainModel.CentralBudgetList.Add(innerModel);
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
            if (model.CurrentApprovalNodeId != null && model.CurrentApprovalNodeId > 0)
            {
                mainModel.NodeApprovalType = _famCommonService.FAMUnit.ApprovalPathDetails.Get().Where(d => d.NodeEmpId == model.CurrentApprovalNodeId && d.PathId == model.ApprovalPathId).FirstOrDefault().ApprovalType;
            }

            mainModel.Mode = "Edit";
            return View("_CreateOrEdit", mainModel);
        }

        [HttpPost]
        public ActionResult Edit(CentralBudgetInformationModel model)
        {
            PrepareModelForSave(model);
            string businessError = string.Empty;
            
            if (ModelState.IsValid)
            {
                try
                {

                    //Recommended Or Approved----
                    if (model.recommenderOrApprover == "Recommender")
                    {
                        model.ApprovalStatus = "Recommended";
                    }
                    if (model.recommenderOrApprover == "Approver")
                    {
                        model.ApprovalStatus = "Approved";
                    }
                    if (model.recommenderOrApprover == "Reject")
                    {
                        model.ApprovalStatus = "Rejected";
                    }
                    if (model.recommenderOrApprover == null)
                    {
                        model.ApprovalStatus = "Draft";
                    }





                    var entity = model.ToEntity();

                    
                    var navigationList = new Dictionary<Type, ArrayList>();
                    var childEntities = new ArrayList();
                    model.CentralBudgetList.ToList().ForEach(x => x.CentralBudgetId = model.Id);
                    model.CentralBudgetList.ToList().ForEach(x => childEntities.Add(x.ToEntityChild()));

                    navigationList.Add(typeof(FAM_CentralBudgetAllocationDetails), childEntities);
                    _famCommonService.FAMUnit.CentralBudgetInformationRepository.Update(entity, navigationList);
                    _famCommonService.FAMUnit.CentralBudgetInformationRepository.SaveChanges();
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
            var entity = _famCommonService.FAMUnit.CentralBudgetInformationRepository.GetByID(id);

            if (entity.ApprovalStatus == "Draft")
            {
                try
                {
                    List<Type> allTypes = new List<Type> { typeof(FAM_CentralBudgetAllocationDetails) };
                    _famCommonService.FAMUnit.CentralBudgetInformationRepository.Delete(entity.Id, allTypes);
                    _famCommonService.FAMUnit.CentralBudgetInformationRepository.SaveChanges();

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

                //return Json(new
                //{
                //    Success = 1,
                //    Message = ErrorMessages.DeleteFailed
                //}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                    {
                        Success = 0,
                        Message = "Recommended Or Approved Central Budget cannot be deleted."
                    }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Utils

        private void PrepareModelForSave(CentralBudgetInformationModel model)
        {
                
            int? approvalNodeId = 0;

            if (model.recommenderOrApprover == "Reject")
            {
                var currentNodeOrder = (from c in _famCommonService.FAMUnit.ApprovalPathDetails.GetAll()
                                        where c.PathId == model.ApprovalPathId && c.NodeEmpId == model.CurrentApprovalNodeId
                                        select c).FirstOrDefault().NodeOrder;

                var approvalNode = (from c in _famCommonService.FAMUnit.ApprovalPathDetails.GetAll()
                                    where c.PathId == model.ApprovalPathId && c.NodeOrder < currentNodeOrder
                                    orderby c.NodeOrder descending
                                    select c).FirstOrDefault();
                if (approvalNode == null)
                {
                    approvalNodeId = null;
                }
                else
                {
                    approvalNodeId = approvalNode.NodeEmpId;
                }



                model.CurrentApprovalNodeId = approvalNodeId;
            }
            else
            {

                if (model.CurrentApprovalNodeId == null)
                {
                    var approvalNode = (from c in _famCommonService.FAMUnit.ApprovalPathDetails.GetAll()
                                        where c.PathId == model.ApprovalPathId
                                        orderby c.NodeOrder ascending
                                        select c).FirstOrDefault();
                    if (approvalNode != null)
                    {
                        approvalNodeId = approvalNode.NodeEmpId;
                    }
                    model.CurrentApprovalNodeId = approvalNodeId;
                }
                else
                {
                    if (model.recommenderOrApprover == "Recommender" || model.recommenderOrApprover == "Approver")
                    {
                        var currentNodeOrder = (from c in _famCommonService.FAMUnit.ApprovalPathDetails.GetAll()
                                                where c.PathId == model.ApprovalPathId && c.NodeEmpId == model.CurrentApprovalNodeId
                                                select c).FirstOrDefault().NodeOrder;

                        var approvalNode = (from c in _famCommonService.FAMUnit.ApprovalPathDetails.GetAll()
                                            where c.PathId == model.ApprovalPathId && c.NodeOrder > currentNodeOrder
                                            orderby c.NodeOrder ascending
                                            select c).FirstOrDefault();
                        if (approvalNode != null)
                        {
                            approvalNodeId = approvalNode.NodeEmpId;
                        }

                        model.CurrentApprovalNodeId = approvalNodeId;
                    }

                    if (model.ApprovalStatus == "Draft")
                    {
                        model.CurrentApprovalNodeId = model.CurrentApprovalNodeId;
                    }
                }
            }
        }

        private void PrepareModel(CentralBudgetInformationModel model)
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
        public ActionResult GetBudgetByFinancialYearId(int Id)
        {
            var model = new CentralBudgetInformationModel();
            //var data = (from d in _famCommonService.FAMUnit.DivisionUnitBudgetAllocationRepository.GetAll()
            //            join dtl in _famCommonService.FAMUnit.BudgetAllocationRepository.GetAll() on d.Id equals dtl.DivisionUnitBudgetId
            //            join coa in _famCommonService.FAMUnit.ChartOfAccount.GetAll() on dtl.AccountHeadId equals coa.Id
            //            where d.FinancialYearId == Id
            //            orderby coa.AccountHeadType descending
            //            select new
            //            {
            //                d.DivisionUnitId,
            //                dtl.AccountHeadId,
            //                dtl.Amount,
            //                coa.AccountHeadCode,
            //                coa.AccountHeadName,
            //                coa.AccountHeadType
            //            }).Distinct().ToList();

            //dynamic list = null;

            //list = _famCommonService.GetCentralBudgetFYSearchedList(model.FinancialYearId);

            var data = _famCommonService.GetCentralBudgetAllocationFYSearchList(Id);
            foreach (var item in data)
            {
                var innerModel = new CentralBudget()
                {
                    AccountHeadId = item.AccountHeadId,
                    //AccountHeadName = item.AccountHeadName,
                    AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
                    OriginalAmount = item.Amount,
                    AccountHeadType = item.AccountHeadType == "I" ? "Income" : "Expense"
                };
                model.CentralBudgetList.Add(innerModel);
            }
            return PartialView("_CentralBudgetList", model);
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

        #endregion



    }
}
