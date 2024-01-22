using BEPZA_MEDICAL.DAL.FMS;
using BEPZA_MEDICAL.Domain.FMS;
using BEPZA_MEDICAL.Domain.PGM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.FMS.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.Controllers
{
    public class FDRClosingController : BaseController
    {
        #region Fields
        private readonly FMSCommonService _fmsCommonService;
        private readonly FMS_ExecuteFunctions _fmsfunction;
        private readonly PGMCommonService _pgmCommonservice;
        private readonly ERP_BEPZAFMSEntities _fmsContext;
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public FDRClosingController(FMSCommonService fmsCommonService, FMS_ExecuteFunctions fmsfunction, PGMCommonService pgmCommonservice, ERP_BEPZAFMSEntities fmsContext, PRMCommonSevice prmCommonService)
        {
            this._fmsCommonService = fmsCommonService;
            this._fmsfunction = fmsfunction;
            this._pgmCommonservice = pgmCommonservice;
            this._fmsContext = fmsContext;
            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions
        // GET: FMS/FDRClosing
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, FDRClosingViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from fdrClosing in _fmsCommonService.FMSUnit.FDRClosingInfoRepository.GetAll()
                        where (fdrClosing.FMS_FixedDepositInfo.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"]))
                        select new FDRClosingViewModel()
                        {
                            Id = fdrClosing.Id,
                            FDRNo = fdrClosing.FMS_FixedDepositInfo.FDRNumber,
                            FDRDate = fdrClosing.FMS_FixedDepositInfo.FDRDate,
                            ClosingDate = fdrClosing.ClosingDate,
                            ClosingAmount = fdrClosing.ClosingAmount,
                            InterestRate = fdrClosing.InterestRate,
                            BankInfoId = fdrClosing.FMS_FixedDepositInfo.BankInfoId,
                            BankName = fdrClosing.FMS_FixedDepositInfo.FMS_BankInfo.BankName,
                            BankInfoBranchDetailId = fdrClosing.FMS_FixedDepositInfo.BankInfoBranchDetailId,
                            BranchName = fdrClosing.FMS_FixedDepositInfo.FMS_BankInfoBranchDetail.BranchName,
                            ProfitRecvId = fdrClosing.ProfitRecvId,
                            IsFDRSaved = fdrClosing.IsFDRSaved,
                            IsRenew = fdrClosing.IsRenew
                        }).OrderBy(x => x.FDRDate).ToList();

            if (request.Searching)
            {
                if ((viewModel.FDRDateFrom != null && viewModel.FDRDateFrom != DateTime.MinValue) && (viewModel.FDRDateTo != null && viewModel.FDRDateTo != DateTime.MinValue))
                {
                    list = list.Where(d => d.FDRDate >= viewModel.FDRDateFrom && d.FDRDate <= viewModel.FDRDateTo).ToList();
                }
                else if ((viewModel.FDRDateFrom != null && viewModel.FDRDateFrom != DateTime.MinValue))
                {
                    list = list.Where(d => d.FDRDate >= viewModel.FDRDateFrom).ToList();
                }
                else if ((viewModel.FDRDateTo != null && viewModel.FDRDateTo != DateTime.MinValue))
                {
                    list = list.Where(d => d.FDRDate <= viewModel.FDRDateTo).ToList();
                }


                if ((viewModel.ClosingDateFrom != null && viewModel.ClosingDateFrom != DateTime.MinValue) && (viewModel.ClosingDateTo != null && viewModel.ClosingDateTo != DateTime.MinValue))
                {
                    list = list.Where(d => d.MaturityDate >= viewModel.ClosingDateFrom && d.MaturityDate <= viewModel.ClosingDateTo).ToList();
                }
                else if ((viewModel.ClosingDateFrom != null && viewModel.ClosingDateFrom != DateTime.MinValue))
                {
                    list = list.Where(d => d.MaturityDate >= viewModel.ClosingDateFrom).ToList();
                }
                else if ((viewModel.ClosingDateTo != null && viewModel.ClosingDateTo != DateTime.MinValue))
                {
                    list = list.Where(d => d.MaturityDate <= viewModel.ClosingDateTo).ToList();
                }

                if ((viewModel.InterestRateFrom != null && viewModel.InterestRateFrom != 0) && (viewModel.InterestRateTo != null && viewModel.InterestRateTo != 0))
                {
                    list = list.Where(d => d.InterestRate >= viewModel.InterestRateFrom && d.InterestRate <= viewModel.InterestRateTo).ToList();
                }
                else if (viewModel.InterestRateFrom != null && viewModel.InterestRateFrom != 0)
                {
                    list = list.Where(d => d.InterestRate == viewModel.InterestRateFrom).ToList();
                }
                else if (viewModel.InterestRateTo != null && viewModel.InterestRateTo != 0)
                {
                    list = list.Where(d => d.InterestRate == viewModel.InterestRateTo).ToList();
                }

                if (viewModel.BankInfoId != null && viewModel.BankInfoId != 0)
                {
                    list = list.Where(d => d.BankInfoId == viewModel.BankInfoId).ToList();
                }

                if (viewModel.BankInfoBranchDetailId != null && viewModel.BankInfoBranchDetailId != 0)
                {
                    list = list.Where(d => d.BankInfoBranchDetailId == viewModel.BankInfoBranchDetailId).ToList();
                }

                if (viewModel.FDRNo != null && viewModel.FDRNo != string.Empty)
                {
                    list = list.Where(d => d.FDRNo.Contains(viewModel.FDRNo)).ToList();
                }

            }
            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "FDRNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FDRNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FDRNo).ToList();
                }
            }
            if (request.SortingName == "FDRDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FDRDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FDRDate).ToList();
                }
            }
            if (request.SortingName == "ClosingDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ClosingDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ClosingDate).ToList();
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

            if (request.SortingName == "FDRAmount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FDRAmount).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FDRAmount).ToList();
                }
            }
            if (request.SortingName == "BankName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BankName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BankName).ToList();
                }
            }

            if (request.SortingName == "BranchName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BranchName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BranchName).ToList();
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
                int IsFDRSaved = 1;
                IsFDRSaved = d.IsFDRSaved == true ? 0 : 1;

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.FDRNo,
                    Convert.ToDateTime(d.FDRDate).ToString(DateAndTime.GlobalDateFormat),
                    d.FDRDateFrom,
                    d.FDRDateTo,
                    Convert.ToDateTime(d.ClosingDate).ToString(DateAndTime.GlobalDateFormat),
                    d.ClosingDateFrom,
                    d.ClosingDateTo,
                    d.ClosingAmount,
                    d.InterestRate,
                    d.InterestRateFrom,
                    d.InterestRateTo,
                    d.BankInfoId,
                    d.BankName,
                    d.BankInfoBranchDetailId,
                    d.BranchName,
                    d.IsRenew,
                    d.ProfitRecvId,
                    IsFDRSaved
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }

        public ActionResult Create()
        {
            FDRClosingViewModel model = new FDRClosingViewModel();
            populateDropdown(model);
            GetFundType(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(FDRClosingViewModel model)
        {
            try
            {
                string errorList = string.Empty;

                if (ModelState.IsValid)
                {
                    //model.ErrMsg = ValidateClosgingDate(model, model.Id);
                    //if (string.IsNullOrEmpty(model.ErrMsg))
                    //{
                    if (model.ActionType == "Renew")
                    {
                        model.IsRenew = true;
                        model.IsFDRSaved = false;
                    }
                    else
                    {
                        model.IsFDRSaved = true;
                    }
                    if (model.WithdrawalAmount == null)
                    {
                        model.WithdrawalAmount = 0;
                    }
                    model = GetInsertUserAuditInfo(model, true);
                    var entity = model.ToEntity();
                    if (errorList.Length == 0)
                    {
                        _fmsCommonService.FMSUnit.FDRClosingInfoRepository.Add(entity);
                        _fmsCommonService.FMSUnit.FDRClosingInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                        if (model.ActionType == "Renew")
                        {
                            return RedirectToAction("Renew", "FixedDepositInfo", new { fdrCloseingId = entity.Id });
                        }
                        else if(Convert.ToInt32(Session["FDRTypeId"]) == 1)
                        {
                            return RedirectToAction("PrepareVoucher", new { id = entity.Id });
                        }

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
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
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

            var entity = _fmsCommonService.FMSUnit.FDRClosingInfoRepository.GetByID(id);
            var model = entity.ToModel();
            var fdrInfo = _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetByID(model.FixedDepositInfoId);
            model.BankInfoId = fdrInfo.BankInfoId;
            model.BankInfoBranchDetailId = fdrInfo.BankInfoBranchDetailId;
            model.strMode = "Edit";
            GetFundType(model);
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(FDRClosingViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                if (ModelState.IsValid)
                {
                    //model.ErrMsg = ValidateClosgingDate(model, model.Id);
                    //if (string.IsNullOrEmpty(model.ErrMsg))
                    //{
                    if (model.ActionType == "Renew")
                    {
                        model.IsRenew = true;
                    }
                    if (model.WithdrawalAmount == null)
                    {
                        model.WithdrawalAmount = 0;
                    }

                    model = GetInsertUserAuditInfo(model, false);
                    var entity = model.ToEntity();
                    if (errorList.Length == 0)
                    {
                        _fmsCommonService.FMSUnit.FDRClosingInfoRepository.Update(entity);
                        _fmsCommonService.FMSUnit.FDRClosingInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
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
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }
            populateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _fmsCommonService.FMSUnit.FDRClosingInfoRepository.Delete(id);
                _fmsCommonService.FMSUnit.FDRClosingInfoRepository.SaveChanges();

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

        #region Private
        private void populateDropdown(FDRClosingViewModel model)
        {
            dynamic ddl;
            HashSet<int> ClsFDRId = new HashSet<int>(_fmsCommonService.FMSUnit.FDRClosingInfoRepository.GetAll().Select(x => x.FixedDepositInfoId));

            model.FDRNoList = Common.PopulateFDRNoDDL(_fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll().Where(q => q.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"])).Where(s => !ClsFDRId.Contains(s.Id))
                              .OrderByDescending(x => x.FDRDate).DistinctBy(x => x.FDRNumber));

            model.ProfitRecvList = _pgmCommonservice.PGMUnit.AccChartOfAccountRepository.GetAll().OrderBy(x => x.accountName)
                                    .Where(s => s.accountGroup == "Income" && s.isControlhead == 0)
                                    .Select(y => new SelectListItem()
                                    {
                                        Text = y.accountName,
                                        Value = y.id.ToString()
                                    }).ToList();

            ddl = _fmsfunction.fnGetBankChequeNoList(LoggedUserZoneInfoId, model.ProfitRecvId);
            model.ChequeList = Common.PopulateChequeNoddl(ddl);

            #region bank ddl
            ddl = _fmsCommonService.FMSUnit.BankInfoRepository.GetAll().OrderBy(x => x.BankName).ToList();
            model.BankInfoList = Common.PopulateFDRBankDDL(ddl);
            #endregion

            #region branch ddl
            ddl = _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.GetAll().OrderBy(x => x.BranchName).ToList();
            model.BankInfoBranchDetailList = Common.PopulateFDRBankBranchDDL(ddl);
            #endregion

        }
        public void GetFundType(FDRClosingViewModel model)
        {
            int FundtypeId = Convert.ToInt32(Session["FDRTypeId"]);
            model.FundType = _fmsCommonService.FMSUnit.FDRTypeRepository.Get(x => x.Id == FundtypeId).Select(s => s.Name).FirstOrDefault();
        }

        private FDRClosingViewModel GetInsertUserAuditInfo(FDRClosingViewModel model, bool pAddEdit)
        {
            if (pAddEdit)
            {
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;

            }
            else
            {
                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
            }
            return model;
        }
        private string ValidateClosgingDate(FDRClosingViewModel model, int id)
        {
            string errorMessage = string.Empty;

            dynamic ECPE;
            if (id < 1)
            {
                ECPE = _fmsCommonService.FMSUnit.FixedDepositInfoRepository.Get(q => q.Id == model.FixedDepositInfoId).FirstOrDefault();

            }
            else
            {
                ECPE = (from fdr in _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll()
                        where fdr.Id == model.FixedDepositInfoId && fdr.Id != model.Id
                        select fdr).FirstOrDefault();
            }
            if (ECPE != null && ECPE.MaturityDate > model.ClosingDate)
            {
                model.errClass = "failed";
                return errorMessage = "Closing date must be greater than FDR Maturity date.";
            }
            return errorMessage;
        }
        #endregion

        [NoCache]
        public JsonResult GetFDRDestils(int id)
        {
            var obj = _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetByID(id);
            var WithdrawalAmt = _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.GetAll().Where(x => x.FixedDepositInfoId == id).ToList();
            var fundType = _fmsCommonService.FMSUnit.FDRTypeRepository.GetByID(obj.FDRTypeId);
            //DateTime dt1 = obj.FDRDate;
            //DateTime dt2 = obj.FDRDate.AddMonths(12);
            //TimeSpan difference = dt2 - dt1;
            //var totaldays = difference.TotalDays;
            //obj.du
            decimal? clsingAmount = 0;
            if (fundType.Name.Equals("BEPZA"))
            {
                clsingAmount = (obj.FDRAmount + obj.TotalProfit) - WithdrawalAmt.Sum(x => x.InterestReceiveAmount == null ? 0 : x.InterestReceiveAmount);
            }
            else
            {
                clsingAmount = obj.FDRAmount + obj.TotalProfit;
            }
            return Json(new
            {
                ProfitRecvId = obj.ProfitRecvId,
                InitialDeposit = obj.InitialDeposit,
                FDRAmount = obj.FDRAmount,
                InterestRate = obj.InterestRate,
                TAXRate = obj.TAXRate,
                Duration = obj.FDRDurationInMonth,
                TotalInterestAmount = obj.TotalInterestAmount,
                TotalTAXAmount = obj.TotalTAXAmount,
                TotalBankCharge = obj.TotalBankCharge,
                TotalProfit = obj.TotalProfit,
                ChequeId = obj.ChequeId,
                WithdrawalAmount = WithdrawalAmt.Sum(x => x.InterestReceiveAmount),
                ClosingAmount = clsingAmount,
                ClosingDate = obj.MaturityDate.ToString(DateAndTime.GlobalDateFormat)
            }, JsonRequestBehavior.AllowGet);

        }

        [NoCache]
        public JsonResult GetBankCheckNo(int id)
        {
            var tempList = Common.PopulateChequeNoddl(_fmsfunction.fnGetBankChequeNoList(LoggedUserZoneInfoId, id));

            var list = tempList.Select(x => new { Id = x.Value, Name = x.Text }).OrderBy(x => x.Name).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BankListView()
        {
            var list = Common.PopulateFDRBankDDL(_fmsCommonService.FMSUnit.BankInfoRepository.GetAll().OrderBy(x => x.BankName).ToList());
            return PartialView("Select", list);
        }


        public ActionResult BranchListView()
        {
            var list = Common.PopulateFDRBankBranchDDL(_fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.GetAll().OrderBy(x => x.BranchName).ToList());
            return PartialView("Select", list);
        }

        [NoCache]
        public JsonResult GetBankBranch(int id)
        {
            var list = (from branch in _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.GetAll().Where(q => q.BankInfoId == id)
                        select new
                        {
                            branchId = branch.Id,
                            branchName = branch.BranchName
                        }).OrderBy(q => q.branchName).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);

        }

        [NoCache]
        public JsonResult GetFDRNo(int id)
        {
            HashSet<int> ClsFDRId = new HashSet<int>(_fmsCommonService.FMSUnit.FDRClosingInfoRepository.GetAll().Select(x => x.FixedDepositInfoId));
            var ddlList = _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll()
                           .Where(q => q.ZoneInfoId == LoggedUserZoneInfoId && q.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"]) && q.BankInfoBranchDetailId == id).OrderByDescending(x => x.FDRDate)
                           .Where(s => !ClsFDRId.Contains(s.Id))
                           .DistinctBy(x => x.FDRNumber);

            var list = Common.PopulateFDRNoDDL(ddlList);

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #region History
        public ActionResult History(int id)
        {
            FDRClosingViewModel model = new FDRClosingViewModel();

            var obj = _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetByID(id);
            model.FDRNo = obj.FDRNumber;
            model.FDRAmount = obj.FDRAmount;
            model.Bank = obj.FMS_BankInfo.BankName;
            model.Branch = obj.FMS_BankInfoBranchDetail.BranchName;
            model.FDRDate = obj.FDRDate;
            model.FixedDepositInfoId = obj.Id;

            var list = (from fdrCls in _fmsCommonService.FMSUnit.FDRClosingInfoRepository.GetAll()
                        join fdi in _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll() on fdrCls.FixedDepositInfoId equals fdi.Id
                        join bnk in _fmsCommonService.FMSUnit.BankInfoRepository.GetAll() on fdi.BankInfoId equals bnk.Id
                        join brnch in _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.GetAll() on fdi.BankInfoBranchDetailId equals brnch.Id
                        where (fdi.FDRNumber.Contains(obj.FDRNumber))
                        select new FDRClosingViewModel()
                        {
                            FixedDepositInfoId = fdrCls.FixedDepositInfoId,
                            MaturityDate = fdi.MaturityDate,
                            Renew = fdi.RenewalNo,
                            FDRNo = fdi.FDRNumber,
                            FDRAmount = fdrCls.FDRAmount,
                            InterestRate = fdrCls.InterestRate,
                            InterestAmount = fdrCls.InterestAmount,
                            TaxAmount = fdrCls.TaxAmount,
                            BankCharge = fdrCls.BankCharge,
                            ProfitAmount = fdrCls.ProfitAmount,
                            ClosingAmount = fdrCls.ClosingAmount
                        }).ToList();


            model.FDRClosingHistoryList = list;

            return View(model);
        }

        [HttpPost]
        public ActionResult GetInstallationInfo(int id)
        {

            var model = new FDRClosingViewModel();
            List<FDRClosingViewModel> resultFrm = (from FDI in _fmsCommonService.FMSUnit.FixedDepositInfoInstallmentScheduleRepository.GetAll()
                                                   where (FDI.FixedDepositInfoId == id)
                                                   select new FDRClosingViewModel()
                                                   {
                                                       InsDate = FDI.InsDate.ToString(DateAndTime.GlobalDateFormat),
                                                       InsAmount = FDI.InsAmount,
                                                       TaxAmount = FDI.Tax,
                                                       BankCharge = FDI.BankCharge,
                                                       ProfitAmount = FDI.Profit

                                                   }).ToList();

            model.FDRInstallmentList = resultFrm;
            return PartialView("_InstallmentDetails", model);
        }
        #endregion

        public ActionResult VoucherPosing(int id)
        {
            string url = string.Empty;
            var sessionUser = MyAppSession.User;
            int UserID = 0;
            string password = "";
            string Username = "";
            string ZoneID = "";
            if (sessionUser != null)
            {
                UserID = sessionUser.UserId;
                password = sessionUser.Password;
                Username = sessionUser.LoginId;
            }
            if (MyAppSession.ZoneInfoId > 0)
            {
                ZoneID = MyAppSession.ZoneInfoId.ToString();
            }

            var obj = _fmsContext.FMS_uspVoucherPostingForClosingRN(id).FirstOrDefault();
            if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VPostingUrl"].ToString() + "/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=" + obj.FundControlId + "&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            }

            return Redirect(url);
        }

        public ActionResult Renew(int id)
        {
            return RedirectToAction("Renew", "FixedDepositInfo", new { fdrCloseingId = id });
        }

        public ActionResult PrepareVoucher(int id)
        {
            var entity = _fmsCommonService.FMSUnit.FDRClosingInfoRepository.GetByID(id);
            var model = entity.ToModel();
            var fdrInfo = _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetByID(model.FixedDepositInfoId);
            model.FDRNo = fdrInfo.FDRNumber;


            var list = (from fdrClosing in _fmsCommonService.FMSUnit.FDRClosingInfoRepository.GetAll()
                        where (fdrClosing.FMS_FixedDepositInfo.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"]))
                              && (fdrClosing.FMS_FixedDepositInfo.FDRNumber == model.FDRNo)
                        select new FDRClosingViewModel()
                        {
                            Id = fdrClosing.Id,
                            FDRNo = fdrClosing.FMS_FixedDepositInfo.FDRNumber,
                            FDRDate = fdrClosing.FMS_FixedDepositInfo.FDRDate,
                            ClosingDate = fdrClosing.ClosingDate,
                            ClosingAmount = fdrClosing.ClosingAmount,
                            InterestRate = fdrClosing.InterestRate,
                            BankName = fdrClosing.FMS_FixedDepositInfo.FMS_BankInfo.BankName,
                            BranchName = fdrClosing.FMS_FixedDepositInfo.FMS_BankInfoBranchDetail.BranchName,
                            FDRAmount = fdrClosing.FMS_FixedDepositInfo.InitialDeposit,
                            ProfitAmount = fdrClosing.ProfitAmount,
                            BankCharge = fdrClosing.BankCharge,
                            TaxAmount = fdrClosing.TaxAmount
                        }).OrderBy(x => x.FDRDate).ToList();

            model.InitialDeposit = entity.FMS_FixedDepositInfo.InitialDeposit;

            model.FDRAmount = entity.FDRAmount;
            model.ProfitAmount = list.Sum(x => x.ProfitAmount);
            model.BankCharge = list.Sum(x => x.BankCharge);
            model.TaxAmount = list.Sum(x => x.TaxAmount);

            model.Narration = string.Concat("FDR No. ", entity.FMS_FixedDepositInfo.FDRNumber, " Encashment form ");

            DDLForVoucher(model);
            return View("_VoucherInfo", model);
        }

        public ActionResult CreateVoucher(FDRClosingViewModel model)
        {
            return View("_VoucherInfo", model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetListforVoucher(JqGridRequest request, string fdrNo, FDRClosingViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from fdrClosing in _fmsCommonService.FMSUnit.FDRClosingInfoRepository.GetAll()
                        where (fdrClosing.FMS_FixedDepositInfo.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"]))
                              && (fdrClosing.FMS_FixedDepositInfo.FDRNumber == fdrNo)
                        select new FDRClosingViewModel()
                        {
                            Id = fdrClosing.Id,
                            FDRNo = fdrClosing.FMS_FixedDepositInfo.FDRNumber,
                            FDRDate = fdrClosing.FMS_FixedDepositInfo.FDRDate,
                            ClosingDate = fdrClosing.ClosingDate,
                            ClosingAmount = fdrClosing.ClosingAmount,
                            InterestRate = fdrClosing.InterestRate,
                            BankName = fdrClosing.FMS_FixedDepositInfo.FMS_BankInfo.BankName,
                            BranchName = fdrClosing.FMS_FixedDepositInfo.FMS_BankInfoBranchDetail.BranchName,
                            FDRAmount = fdrClosing.FMS_FixedDepositInfo.InitialDeposit,
                            ProfitAmount = fdrClosing.ProfitAmount,
                            BankCharge = fdrClosing.BankCharge,
                            TaxAmount = fdrClosing.TaxAmount
                        }).OrderBy(x => x.FDRDate).ToList();

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();


            foreach (var d in list)
            {
                int IsFDRSaved = 1;
                IsFDRSaved = d.IsFDRSaved == true ? 0 : 1;

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.FDRNo,
                    d.FDRAmount,
                    Convert.ToDateTime(d.FDRDate).ToString(DateAndTime.GlobalDateFormat),
                    Convert.ToDateTime(d.ClosingDate).ToString(DateAndTime.GlobalDateFormat),
                    d.ProfitAmount,
                    d.BankCharge,
                    d.TaxAmount,
                    d.ClosingAmount,
                    d.InterestRate,
                    d.BankName,
                    d.BranchName
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }

        public ActionResult FDRGridList(string fdrNo, FDRClosingViewModel viewModel)
        {
            var list = (from fdrClosing in _fmsCommonService.FMSUnit.FDRClosingInfoRepository.GetAll()
                        where (fdrClosing.FMS_FixedDepositInfo.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"]))
                              && (fdrClosing.FMS_FixedDepositInfo.FDRNumber == fdrNo)
                        select new FDRClosingViewModel()
                        {
                            Id = fdrClosing.Id,
                            FDRNo = fdrClosing.FMS_FixedDepositInfo.FDRNumber,
                            FDRDate = fdrClosing.FMS_FixedDepositInfo.FDRDate,
                            ClosingDate = fdrClosing.ClosingDate,
                            ClosingAmount = fdrClosing.ClosingAmount,
                            InterestRate = fdrClosing.InterestRate,
                            BankName = fdrClosing.FMS_FixedDepositInfo.FMS_BankInfo.BankName,
                            BranchName = fdrClosing.FMS_FixedDepositInfo.FMS_BankInfoBranchDetail.BranchName,
                            FDRAmount = fdrClosing.FDRAmount,
                            InterestAmount = fdrClosing.InterestAmount,
                            ProfitAmount = fdrClosing.ProfitAmount,
                            BankCharge = fdrClosing.BankCharge,
                            TaxAmount = fdrClosing.TaxAmount
                        }).OrderBy(x => x.FDRDate).ToList();
            return Json(new
            {
                list
            });
        }

        public ActionResult VoucherPosting(decimal FDRAmount, decimal ProfitAmount, decimal TaxAmount, decimal BankCharge, DateTime voucherDate, int? approverId, int? subLedgerId, string narration)
        {
            var result = string.Empty;
            var Message = new ObjectParameter("Message", typeof(string));
            try{
                 _fmsContext.FMS_AutoRVforFDRCls
                    (LoggedUserZoneInfoId, MyAppSession.UserID, approverId, 
                    voucherDate.ToString("dd-MM-yyyy"), narration, 
                    subLedgerId, FDRAmount, ProfitAmount, MyAppSession.EmpId, 
                    TaxAmount, BankCharge, Message);

                result = Message.Value.ToString();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return Json(new
            {
                result
            });
        }

        public void DDLForVoucher(FDRClosingViewModel model)
        {
            int employeeId = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == MyAppSession.EmpId).Select(x => x.Id).FirstOrDefault();
            var approverList = _fmsContext.ACC_getApproverListByZoneId(LoggedUserZoneInfoId, "AccVou", employeeId, null).ToList();
            var avlist = new List<SelectListItem>();
            foreach (var item in approverList)
            {
                avlist.Add(new SelectListItem()
                {
                    Text = item.FullName,
                    Value = item.Id.ToString()
                });
            }
            model.ApproverList = avlist;

            var subLedgerList = _fmsCommonService.FMSUnit.SubLedgerRepository.GetAll().ToList();
            var sllist = new List<SelectListItem>();
            foreach (var item in subLedgerList)
            {
                sllist.Add(new SelectListItem()
                {
                    Text = item.name,
                    Value = item.id.ToString()
                });
            }
            model.SubLedgerList = sllist;
        }
    }
}