using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;
using System.Collections;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc;

using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Domain.FAM;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class BankNameController : BaseController
    {
        private readonly PRMCommonSevice _prmCommonservice;

        public BankNameController(PRMCommonSevice prmCommonservice)
        {
            this._prmCommonservice = prmCommonservice;
        }

        #region Actions ---------------------------------

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, BankNameViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = (from bank in _prmCommonservice.PRMUnit.BankNameRepository.GetAll()
                        where (bank.ZoneInfoId == LoggedUserZoneInfoId)
                        select new BankNameViewModel()
                        {
                            Id = bank.Id,
                            Name = bank.Name,
                            Remarks = bank.Remarks
                        }).OrderBy(x => x.Name).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.Name))
                {
                    list = list.Where(x => x.Name.Trim().ToLower().Contains(viewModel.Name.Trim().ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(viewModel.Remarks))
                {
                    list = list.Where(x => x.Remarks.Trim().ToLower().Contains(viewModel.Remarks.Trim().ToLower())).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "Name")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Name).ToList();
                }
            }

            if (request.SortingName == "Remarks")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Remarks).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Remarks).ToList();
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
                    d.Name,    
                    d.Remarks,
                    "Delete"
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            BankNameViewModel model = new BankNameViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(BankNameViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {                   
                    if (model.BankBranches == null || model.BankBranches.Count() <= 0)
                    {
                        model.IsSuccessful = false;
                        model.errClass = "failed";
                        model.Message = "Please Provide at least one Bank branch";
                        return View(model);
                    }

                    var checkoutBusinessLogic = CheckingBusinessLogicValidation(model);

                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        model = SetUserAuditInfo(model, true);
                        model.ZoneInfoId = LoggedUserZoneInfoId;
                        var master = model.ToEntity();

                        foreach (var c in model.BankBranches)
                        {
                            master.PRM_BankBranch.Add
                            (new PRM_BankBranch
                                {
                                    BankId = c.BankId,
                                    Name = c.Name,
                                    NameInBengali = c.NameInBengali,
                                    Address = c.Address,
                                    AddressInBengali = c.AddressInBengali,
                                    IUser = User.Identity.Name,
                                    IDate = DateTime.Now
                                }
                            );
                        }

                        _prmCommonservice.PRMUnit.BankNameRepository.Add(master);
                        _prmCommonservice.PRMUnit.BankNameRepository.SaveChanges();

                        model.IsSuccessful = true;
                        model.errClass = "success";
                        model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                        //return RedirectToAction("Index");
                    }
                    else
                    {
                        model.IsSuccessful = false;
                        model.Message = checkoutBusinessLogic;
                    }
                }
                catch (Exception ex)
                {
                    model.errClass = "failed";
                    model.IsSuccessful = false;
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate"))
                    {
                        model.Message = ErrorMessages.UniqueIndex;
                    }
                    else
                    {
                        model.Message = ErrorMessages.InsertFailed;
                    }
                }
            }
            else
            {
                model.IsSuccessful = false;
            }

            return View(model);
        }

        public ActionResult Edit(int id, string type)
        {
            var master = _prmCommonservice.PRMUnit.BankNameRepository.GetByID(id);

            var model = master.ToModel();
            model.strMode = "Edit";

            if (master.PRM_BankBranch != null)
            {
                model.BankBranches = new Collection<BankBranchViewModel>();

                foreach (var item in master.PRM_BankBranch)
                {
                    model.BankBranches.Add(item.ToModel());
                }
            }

            //PopulateDropDownforAccountMaster(model);
            //var mainhead = _prmCommonservice.PRMUnit.BudgetSubHead.GetByID(master.BudgetSubHeadId).MainHeadId;
            //model.BudgetMainHeadId = mainhead;
            if (type == "success")
            {
                model.errClass = "success";
                model.Message = Resources.ErrorMessages.UpdateSuccessful;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(BankNameViewModel model)
        {
            var checkoutBusinessLogic = string.Empty;

            try
            {
                if (model.BankBranches == null || model.BankBranches.Count() <= 0)
                {
                    model.IsSuccessful = false;
                    model.errClass = "failed";
                    model.Message = "Please Provide at least one Bank branch";
                    return View(model);
                }

                checkoutBusinessLogic = CheckingBusinessLogicValidation(model);

                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        model = SetUserAuditInfo(model, false);
                        model.ZoneInfoId = LoggedUserZoneInfoId;
                        var master = model.ToEntity();
                        ArrayList arrtyList = new ArrayList();

                        if (model.BankBranches != null)
                        {
                            foreach (var item in model.BankBranches)
                            {
                                var child = new PRM_BankBranch()
                                {
                                    Id = item.Id,
                                    BankId = item.BankId,
                                    Name = item.Name,
                                    NameInBengali = item.NameInBengali,
                                    Address = item.Address,
                                    AddressInBengali = item.AddressInBengali,
                                    Remarks = item.Remarks,

                                    IUser = string.IsNullOrEmpty(item.IUser) ? User.Identity.Name : item.IUser,
                                    IDate = item.IDate == null ? DateTime.Now : Convert.ToDateTime(item.IDate),

                                    EUser = User.Identity.Name,
                                    EDate = DateTime.Now,
                                };

                                // if old item then reflection will retrive old IUser & IDate
                                arrtyList.Add(child);
                            }
                        }

                        Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                        NavigationList.Add(typeof(PRM_BankBranch), arrtyList);
                        _prmCommonservice.PRMUnit.BankNameRepository.Update(master, NavigationList);
                        _prmCommonservice.PRMUnit.BankNameRepository.SaveChanges();

                        model.errClass = "success";
                        model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        //return RedirectToAction("Index");
                        return RedirectToAction("Edit", new { id = master.Id, type = "success" });
                    }
                    else
                    {
                        model.IsSuccessful = false;
                        model.Message = checkoutBusinessLogic;
                    }
                }
                else
                {
                    model.IsSuccessful = false;
                    model.Message = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                }
            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.errClass = "failed";
                if (ex.InnerException.Message.Contains("duplicate"))
                {
                    model.Message = ErrorMessages.UniqueIndex;
                }
                else
                {
                    model.Message = ErrorMessages.UpdateFailed;
                }
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                //  List<Type> allTypes = new List<Type> { typeof(PRM_BankBranch) };
                // _prmCommonservice.PRMUnit.BankNameRepository.Delete(id, allTypes);
                _prmCommonservice.PRMUnit.BankNameRepository.Delete(id);
                _prmCommonservice.PRMUnit.BankNameRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        [HttpPost, ActionName("DeleteBankBranch")]
        public JsonResult DeleteBankBranchConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _prmCommonservice.PRMUnit.BankBranchRepository.Delete(id);
                _prmCommonservice.PRMUnit.BankBranchRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        #endregion

        #region Utilities ------------------

        private BankNameViewModel SetUserAuditInfo(BankNameViewModel item, bool IsInsert)
        {
            if (IsInsert)
            {
                item.IUser = User.Identity.Name;
                item.IDate = DateTime.Now;

                foreach (var child in item.BankBranches)
                {
                    child.IUser = User.Identity.Name;
                    child.IDate = DateTime.Now;
                }
            }

            if (!IsInsert)
            {
                item.EUser = User.Identity.Name;
                item.EDate = DateTime.Now;

                foreach (var child in item.BankBranches)
                {
                    child.EUser = User.Identity.Name;
                    child.EDate = DateTime.Now;
                }
            }

            return item;
        }

        public PartialViewResult AddDetail(int BankId, string BranchName, string Address, string BranchNameInBengali, string AddressInBengali)
        {
            var master = new BankNameViewModel();
            master.BankBranches = new List<BankBranchViewModel>();

            //if (!string.IsNullOrEmpty(BranchName))
            //{
            var model = new BankBranchViewModel();

            model.BankId = BankId;
            model.Name = BranchName;
            model.NameInBengali = BranchNameInBengali;
            model.Address = Address;
            model.AddressInBengali = AddressInBengali;

            master.BankBranches.Add(model);
            return PartialView("_Detail", master);
            //}
            //else
            //{
            //    var masterModel = new BankNameViewModel();
            //    masterModel.Name = "Invalid";
            //    ModelState.Clear();
            //    return PartialView("_InvalidData", masterModel);
            //}
        }

        private string CheckingBusinessLogicValidation(BankNameViewModel model)
        {
            string message = string.Empty;

            //message = CheckDuplicate(model.AccountDetails.ToList());

            //if (!string.IsNullOrEmpty(message))
            //{
            //    return message;
            //}

            //var getBudgetAccountHead = (from m in _prmCommonservice.PIMUnit.BudgetAccountMaster.GetAll()
            //                            join d in _prmCommonservice.PIMUnit.BudgetAccountDetails.GetAll() on m.Id equals d.BudgetAccMasterId
            //                            where m.ProjectTypeId == model.ProjectTypeId
            //                            select new { d.Id, d.AccountHeadId }).ToList();

            //var detailList = model.AccountDetails.ToList();

            //if (getBudgetAccountHead.Count > 0)
            //{
            //    var existingItemList = (from c in getBudgetAccountHead
            //                            where !(from inner in detailList select inner.Id).Contains(c.Id)
            //                            select c);

            //    foreach (var itemOuter in existingItemList)
            //    {
            //        foreach (var itemInner in detailList)
            //        {
            //            if (itemOuter.AccountHeadId == itemInner.AccountHeadId)
            //            {
            //                string accCode = _famCommonservice.FAMUnit.ChartOfAccount.GetByID(itemInner.AccountHeadId).AccountHeadCode;

            //                return message = "The account head [" + accCode + "] already exist.";
            //            }
            //        }
            //    }
            //}
            return message;
        }

        //private string CheckDuplicate(List<BankNameViewModel> model)
        //{
        //    string msg = string.Empty;
        //    List<int> ItemList = new List<int>();

        //    foreach (var item in model)
        //    {
        //        int budgetHeadIds = new int();
        //        budgetHeadIds = item.AccountHeadId;
        //        ItemList.Add(budgetHeadIds);
        //    }

        //    if (ItemList.Distinct().Count() < model.Count())
        //        return "Budget account head must be identical.";

        //    return msg;
        //}

        public string GetBankBranchByBankId(int id)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });

            var items = (from entity in _prmCommonservice.PRMUnit.BankBranchRepository.Fetch()
                         where entity.BankId == id
                         select entity).OrderBy(o => o.Name).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    var listItem = new SelectListItem { Text = item.Name, Value = item.Id.ToString() };
                    listItems.Add(listItem);
                }
            }

            return new JavaScriptSerializer().Serialize(listItems);
        }

        #endregion
    }
}