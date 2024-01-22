using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.BankReconciliation;
using BEPZA_MEDICAL.Domain.FAM;
 
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Resources;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class BankReconciliationController : Controller
    {
        #region Fields

        private readonly FAMCommonService _famCommonservice;
        //private readonly PIMCommonService _pimCommonservice;
        private readonly PRMCommonSevice _prmCommonservice;

        #endregion

        #region Ctor

        public BankReconciliationController(FAMCommonService famCommonservice,
           //   
            PRMCommonSevice prmCommonService)
        {
            _famCommonservice = famCommonservice;
            // 
            _prmCommonservice = prmCommonService;
        }

        #endregion

        #region Properties

        public string Message { get; set; }

        #endregion

        #region Actions

        public ActionResult Index()
        {
            var model = new BankReconciliationModel();

            model.Mode = "Create";
            model.Debits = new List<DebitOrCredit>() { new DebitOrCredit() };
            model.Credits = new List<DebitOrCredit>() { new DebitOrCredit() };

            PrepareCreateModel(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(BankReconciliationModel model)
        {
            PrepareModelForSave(model);
            Message = CheckBusinessRule(model);
            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                try
                {
                    var selectedIds = model.Debits.Where(x => x.IsReconciled).Select(x => x.Id).Union(model.Credits.Where(x => x.IsReconciled).Select(x => x.Id)).Distinct().ToList();

                    var entities = (from vd in _famCommonservice.FAMUnit.voucherDetails.GetAll()
                                    where selectedIds.Contains(vd.Id)
                                    select vd).ToList();

                    foreach (var item in entities)
                    {
                        var foundItem = model.Debits.Where(x => x.IsReconciled && x.Id == item.Id).FirstOrDefault();
                        var reconcileDate = DateTime.Now;
                        var isFound = false;

                        if (foundItem == null)
                        {
                            foundItem = model.Credits.Where(x => x.IsReconciled && x.Id == item.Id).FirstOrDefault();
                            if (foundItem != null)
                            {
                                isFound = true;
                                reconcileDate = foundItem.ClearDate.HasValue ? foundItem.ClearDate.Value : DateTime.Now;
                            }
                        }
                        else
                        {
                            isFound = true;
                            reconcileDate = foundItem.ClearDate.HasValue ? foundItem.ClearDate.Value : DateTime.Now;
                        }

                        if (isFound)
                        {
                            item.IsReconciled = true;
                            item.ReconcileDate = reconcileDate;

                            _famCommonservice.FAMUnit.voucherDetails.Update(item);
                        }
                    }
                    _famCommonservice.FAMUnit.voucherDetails.SaveChanges();

                    Message = ErrorMessages.InsertSuccessful;
                    model.strClass = "success";
                }
                catch (Exception ex)
                {
#if DEBUG
                    Message = ErrorMessages.InsertFailed + ": " + ex.Message + " inner exception: " + ex.InnerException ?? ex.InnerException.Message;
#endif
#if RELEASE
                    Message = ErrorMessages.InsertFailed;         
#endif
                    model.strClass = "failed";
                }
            }
            else
            {
                Message = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(Message) ? ErrorMessages.InsertFailed : Message) : Common.GetModelStateError(ModelState);
                model.strClass = "failed";
            }

            model.strMessage = Message;
            return View("_Search", model);
            //return new JsonResult()
            //{
            //    Data = Message
            //};
        }

        public ActionResult Search(BankReconciliationModel model)
        {
            model.Mode = "Create";

            model.BankList = Common.PopulateDllList(_prmCommonservice.PRMUnit.BankNameRepository.GetAll().ToList());

            int totalRecords = 0;

            var list = _famCommonservice.FAMUnit.FunctionRepository.GetAllCommonConfig("Voucher Type", 0, "", "", "", 0, 100, out totalRecords);

            var vtId = list.Where(x => x.Name == "RV" || x.Name == "JV").Select(x => x.Id).ToList();

            model.Debits = (from vm in _famCommonservice.FAMUnit.voucherMaster.GetAll()
                            join vd in _famCommonservice.FAMUnit.voucherDetails.GetAll() on vm.Id equals vd.VoucherId
                            where !vm.IsCashOrBank && vtId.Contains(vm.VoucherTypeId) && vd.Debit != 0   //RV : JV
                            && (model.BankId == null || vm.BankId == model.BankId)
                            && (model.AccountId == null || vm.BankAccountId == model.AccountId)
                            && ((model.VoucherDateFrom.HasValue ? (model.VoucherDateFrom.Value <= vm.VoucherDate) : true) && (model.VoucherDateTo.HasValue ? (model.VoucherDateTo.Value >= vm.VoucherDate) : true))
                            && (model.IsReconciled ? (bool)vd.IsReconciled : true)

                            select new DebitOrCredit()
                            {
                                Id = vd.Id,
                                //Amount = 0,
                                Amount = vd.Debit,
                                ChequeDate = vm.ChequeDate.HasValue ? vm.ChequeDate.Value : DateTime.MinValue,
                                ChequeNo = vd.ChequeNumber,
                                VoucherDate = vm.VoucherDate,
                                VoucherNo = vm.VoucherNumber,
                                IsReconciled = vd.IsReconciled.HasValue ? vd.IsReconciled.Value : false,
                                ClearDate = vd.ReconcileDate.HasValue ? vd.ReconcileDate.Value : DateTime.MinValue
                            }).ToList();

            vtId = list.Where(x => x.Name == "PV" || x.Name == "JV").Select(x => x.Id).ToList();
            model.Credits = (from vm in _famCommonservice.FAMUnit.voucherMaster.GetAll()
                             join vd in _famCommonservice.FAMUnit.voucherDetails.GetAll() on vm.Id equals vd.VoucherId
                             where !vm.IsCashOrBank && vtId.Contains(vm.VoucherTypeId) && vd.Credit != 0 //PV :JV
                             && (model.BankId == null || vm.BankId == model.BankId)
                             && (model.AccountId == null || vm.BankAccountId == model.AccountId)
                             && ((model.VoucherDateFrom.HasValue ? (model.VoucherDateFrom.Value <= vm.VoucherDate) : true) && (model.VoucherDateTo.HasValue ? (model.VoucherDateTo.Value >= vm.VoucherDate) : true))
                             && (model.IsReconciled ? (bool)vd.IsReconciled : true)

                             select new DebitOrCredit()
                             {
                                 Id = vd.Id,
                                 //Amount = 0,
                                 Amount = vd.Credit,
                                 ChequeDate = vm.ChequeDate.HasValue ? vm.ChequeDate.Value : DateTime.MinValue,
                                 ChequeNo = vd.ChequeNumber,
                                 VoucherDate = vm.VoucherDate,
                                 VoucherNo = vm.VoucherNumber,
                                 IsReconciled = vd.IsReconciled.HasValue ? vd.IsReconciled.Value : false,
                                 ClearDate = vd.ReconcileDate.HasValue ? vd.ReconcileDate.Value : DateTime.MinValue
                             }).ToList();
            ModelState.Clear();
            return View("_Search", model);
        }

        #endregion

        #region Utils

        private void PrepareCreateModel(BankReconciliationModel model)
        {
            model.BankList = Common.PopulateDllList(_prmCommonservice.PRMUnit.BankNameRepository.GetAll().ToList());

            int totalRecords = 0;

            var list = _famCommonservice.FAMUnit.FunctionRepository.GetAllCommonConfig("Voucher Type", 0, "", "", "", 0, 100, out totalRecords);

            var vtId = list.Where(x => x.Name == "RV" || x.Name == "JV").Select(x => x.Id).ToList();

            model.Debits = (from vm in _famCommonservice.FAMUnit.voucherMaster.GetAll()
                            join vd in _famCommonservice.FAMUnit.voucherDetails.GetAll() on vm.Id equals vd.VoucherId
                            where !vm.IsCashOrBank && vtId.Contains(vm.VoucherTypeId) && vd.Debit != 0   //RV : JV
                            select new DebitOrCredit()
                            {
                                Id = vd.Id,
                                Amount = 0,
                                ChequeDate = vm.ChequeDate.HasValue ? vm.ChequeDate.Value : DateTime.MinValue,
                                ChequeNo = vd.ChequeNumber,
                                VoucherDate = vm.VoucherDate,
                                VoucherNo = vm.VoucherNumber,
                                IsReconciled = vd.IsReconciled.HasValue ? vd.IsReconciled.Value : false,
                                ClearDate = vd.ReconcileDate.HasValue ? vd.ReconcileDate.Value : DateTime.MinValue
                            }).ToList();

            vtId = list.Where(x => x.Name == "PV" || x.Name == "JV").Select(x => x.Id).ToList();
            model.Credits = (from vm in _famCommonservice.FAMUnit.voucherMaster.GetAll()
                             join vd in _famCommonservice.FAMUnit.voucherDetails.GetAll() on vm.Id equals vd.VoucherId
                             where !vm.IsCashOrBank && vtId.Contains(vm.VoucherTypeId) && vd.Credit != 0 //PV :JV
                             select new DebitOrCredit()
                             {
                                 Id = vd.Id,
                                 Amount = 0,
                                 ChequeDate = vm.ChequeDate.HasValue ? vm.ChequeDate.Value : DateTime.MinValue,
                                 ChequeNo = vd.ChequeNumber,
                                 VoucherDate = vm.VoucherDate,
                                 VoucherNo = vm.VoucherNumber,
                                 IsReconciled = vd.IsReconciled.HasValue ? vd.IsReconciled.Value : false,
                                 ClearDate = vd.ReconcileDate.HasValue ? vd.ReconcileDate.Value : DateTime.MinValue
                             }).ToList();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAccountNumber(int id)
        {
            var list = (from bm in _famCommonservice.FAMUnit.BankMaster.GetAll()
                        join bd in _famCommonservice.FAMUnit.BankDetails.GetAll() on bm.Id equals bd.BankBranchMapId
                        where bm.BankId == id
                        select (new SelectListItem() { Text = bd.BankAccountNo, Value = bd.Id.ToString() })).ToList();

            return PartialView("_Select", list);
        }

        private string CheckBusinessRule(BankReconciliationModel model)
        {
            return "";
        }

        private void PrepareModelForSave(BankReconciliationModel model)
        {

        }

        #endregion
    }
}
