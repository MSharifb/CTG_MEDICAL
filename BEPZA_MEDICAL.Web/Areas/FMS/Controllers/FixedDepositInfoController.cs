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
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.Controllers
{
    public class FixedDepositInfoController : BaseController
    {

        #region Fields

        private readonly FMSCommonService _fmsCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        private readonly FMS_ExecuteFunctions _fmsfunction;
        private readonly ERP_BEPZAFMSEntities _fmsContext;
        private readonly PGMCommonService _pgmCommonservice;

        #endregion

        #region Ctor
        public FixedDepositInfoController(FMSCommonService fmsfCommonService, PRMCommonSevice prmCommonService, FMS_ExecuteFunctions fmsfunction, ERP_BEPZAFMSEntities fmsContext, PGMCommonService pgmCommonservice)
        {
            this._fmsCommonService = fmsfCommonService;
            this._prmCommonService = prmCommonService;
            this._fmsfunction = fmsfunction;
            this._fmsContext = fmsContext;
            this._pgmCommonservice = pgmCommonservice;
        }
        #endregion

        #region Actions
        //
        // GET: FMS/FixedDepositInfo
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, FixedDepositInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from fixDeposit in _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId)
                        where (fixDeposit.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"]))
                        select new FixedDepositInfoViewModel()
                                                      {
                                                          Id = fixDeposit.Id,
                                                          FDRNumber = fixDeposit.FDRNumber,
                                                          FDRDate = fixDeposit.FDRDate,
                                                          MaturityDate = fixDeposit.MaturityDate,
                                                          FDRAmount = fixDeposit.FDRAmount,
                                                          InterestRate = fixDeposit.InterestRate,
                                                          BankInfoId = fixDeposit.BankInfoId,
                                                          BankName = fixDeposit.FMS_BankInfo.BankName,
                                                          BankInfoBranchDetailId = fixDeposit.BankInfoBranchDetailId,
                                                          BranchName = fixDeposit.FMS_BankInfoBranchDetail.BranchName
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


                if ((viewModel.MaturityDateFrom != null && viewModel.MaturityDateFrom != DateTime.MinValue) && (viewModel.MaturityDateTo != null && viewModel.MaturityDateTo != DateTime.MinValue))
                {
                    list = list.Where(d => d.MaturityDate >= viewModel.MaturityDateFrom && d.MaturityDate <= viewModel.MaturityDateTo).ToList();
                }
                else if ((viewModel.MaturityDateFrom != null && viewModel.MaturityDateFrom != DateTime.MinValue))
                {
                    list = list.Where(d => d.MaturityDate >= viewModel.MaturityDateFrom).ToList();
                }
                else if ((viewModel.MaturityDateTo != null && viewModel.MaturityDateTo != DateTime.MinValue))
                {
                    list = list.Where(d => d.MaturityDate <= viewModel.MaturityDateTo).ToList();
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


                if ((viewModel.FDRNumber != null && viewModel.FDRNumber != ""))
                {
                    list = list.Where(d => d.FDRNumber == viewModel.FDRNumber).ToList();

                }
                if (viewModel.BankInfoId != 0)
                {
                    list = list.Where(d => d.BankInfoId == viewModel.BankInfoId).ToList();
                }

                if (viewModel.BankInfoBranchDetailId != 0)
                {
                    list = list.Where(d => d.BankInfoBranchDetailId == viewModel.BankInfoBranchDetailId).ToList();
                }

            }
            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "FDRNumber")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FDRNumber).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FDRNumber).ToList();
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
            if (request.SortingName == "MaturityDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.MaturityDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.MaturityDate).ToList();
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
                int isEditable = 1;
                bool isUsed = _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.Get(q => q.FixedDepositInfoId == d.Id).Any();
                isEditable = isUsed == true ? 0 : 1;

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,                  
                    d.FDRNumber,
                    Convert.ToDateTime(d.FDRDate).ToString(DateAndTime.GlobalDateFormat),
                    d.FDRDateFrom,
                    d.FDRDateTo,
                    Convert.ToDateTime(d.MaturityDate).ToString(DateAndTime.GlobalDateFormat),
                    d.MaturityDateFrom,
                    d.MaturityDateTo,
                    d.FDRAmount,
                    d.InterestRate,
                    d.InterestRateFrom,
                    d.InterestRateTo,
                    d.BankInfoId,
                    d.BankName,
                    d.BankInfoBranchDetailId,
                    d.BranchName,
                    isEditable
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }

        public ActionResult Create()
        {
            FixedDepositInfoViewModel model = new FixedDepositInfoViewModel();
            populateDropdown(model);
            model.FDRName = "Bangladesh Export Processing Zones Authority";
            model.strMode = "Create";
            GetFDRType(model);
            return View(model);
        }

        public void GetFDRType(FixedDepositInfoViewModel model)
        {
            model.FDRTypeId = Convert.ToInt32(Session["FDRTypeId"]);
            model.FDRTypeName = _fmsCommonService.FMSUnit.FDRTypeRepository.Get(x => x.Id == model.FDRTypeId).Select(s => s.Name).FirstOrDefault();
        }

        [HttpPost]
        public ActionResult Create(FixedDepositInfoViewModel model)
        {
            try
            {
                string errorList = string.Empty;

                if (ModelState.IsValid)
                {
                    model.ErrMsg = ValidateFDRDate(model, model.Id);
                    if (string.IsNullOrEmpty(model.ErrMsg))
                    {
                        if (model.InstallmentSchedulList == null || model.InstallmentSchedulList.Count <= 0)
                        {
                            model.errClass = "failed";
                            model.ErrMsg = "Installment Schedule wouldn't be empty.";
                            model.strMode = "Create";
                            setDetailList(model);
                            populateDropdown(model);
                            return View(model);
                        }

                        if (CheckDuplicateEntry(model, model.Id) && model.strMode == "Create")
                        {
                            model.ErrMsg = "Duplicate FDR Number";
                            model.errClass = "failed";
                            populateDropdown(model);
                            #region bank ddl
                            var ddlList = _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.GetAll().OrderBy(x => x.BranchName).ToList();
                            model.BankInfoBranchDetailList = Common.PopulateFDRBankBranchDDL(ddlList);
                            #endregion
                            setDetailList(model);
                            model.strMode = "Create";
                            return View(model);
                        }


                        model.ZoneInfoId = LoggedUserZoneInfoId;
                        model = GetInsertUserAuditInfo(model, true);
                        var entity = CreateEntity(model, true);
                        if (!string.IsNullOrEmpty(model.RenewalNo))
                        {
                            var clsEntity = _fmsCommonService.FMSUnit.FDRClosingInfoRepository.GetByID(model.FDRCloseingId);
                            clsEntity.IsFDRSaved = true;
                            _fmsCommonService.FMSUnit.FDRClosingInfoRepository.Update(clsEntity);
                            _fmsCommonService.FMSUnit.FDRClosingInfoRepository.SaveChanges();
                        }
                        if (errorList.Length == 0)
                        {
                            _fmsCommonService.FMSUnit.FixedDepositInfoRepository.Add(entity);
                            _fmsCommonService.FMSUnit.FixedDepositInfoRepository.SaveChanges();
                            //model.errClass = "success";
                            //model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                            //return RedirectToAction("PrepareVoucher", new { id = entity.Id, type = "success" });

                            model = new FixedDepositInfoViewModel();
                            populateDropdown(model);
                            model.FDRName = "Bangladesh Export Processing Zones Authority";
                            model.strMode = "Create";
                            GetFDRType(model);
                            model.errClass = "success";
                            model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                            return View(model);
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
                //if (ex.InnerException != null && ex.InnerException is SqlException)
                //{
                //    SqlException sqlException = ex.InnerException as SqlException;
                //    model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                //}
                //else
                //{
                //    model.ErrMsg = Resources.ErrorMessages.InsertFailed;
                //}
            }

            setDetailList(model);
            populateDropdown(model);
            return View(model);
        }

        public ActionResult Renew(int fdrCloseingId)
        {
            FixedDepositInfoViewModel model = new FixedDepositInfoViewModel();
            var fdrClosingInfo = _fmsCommonService.FMSUnit.FDRClosingInfoRepository.GetByID(fdrCloseingId);

            if (fdrClosingInfo != null)
            {
                var entity = _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetByID(fdrClosingInfo.FixedDepositInfoId);
                model = entity.ToModel();
                model.strMode = "Renew";
                populateDropdown(model);
                //model.FDRAmount = ((fdrClosingInfo.ClosingAmount + fdrClosingInfo.ProfitAmount) - fdrClosingInfo.LossAmount);
                model.FDRAmount = fdrClosingInfo.ClosingAmount;
                model.FDRDate = entity.MaturityDate;
                model.RenewalNo = (Convert.ToInt32(entity.RenewalNo) + 1).ToString();
                model.StartDate = null;
                model.MaturityDate = null;
                model.Id = 0;
                model.FDRCloseingId = fdrCloseingId;

                GetFDRType(model);
            }

            return View("Create", model);
        }


        public ActionResult Edit(int id)
        {

            var entity = _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetByID(id);
            var model = entity.ToModel();
            model.tempTotalBankCharge = entity.TotalBankCharge;
            model.strMode = "Edit";
            populateDropdown(model);
            GetFDRType(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(FixedDepositInfoViewModel model)
        {
            try
            {
                string errorList = string.Empty;

                if (ModelState.IsValid)
                {
                    if (model.InstallmentSchedulList == null || model.InstallmentSchedulList.Count <= 0)
                    {
                        model.errClass = "failed";
                        model.ErrMsg = "Installment Schedule wouldn't be empty.";
                        model.strMode = "Edit";
                        populateDropdown(model);
                        return View(model);
                    }

                    //delete Installment schedule          
                    var list = _fmsCommonService.FMSUnit.FixedDepositInfoInstallmentScheduleRepository.GetAll().Where(q => q.FixedDepositInfoId == model.Id).ToList();

                    foreach (var item in list)
                    {
                        _fmsCommonService.FMSUnit.FixedDepositInfoInstallmentScheduleRepository.Delete(item.Id);
                    }
                    _fmsCommonService.FMSUnit.FixedDepositInfoInstallmentScheduleRepository.SaveChanges();

                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    model = GetInsertUserAuditInfo(model, false);
                    var entity = CreateEntity(model, false);
                    if (errorList.Length == 0)
                    {
                        _fmsCommonService.FMSUnit.FixedDepositInfoRepository.Update(entity);
                        _fmsCommonService.FMSUnit.FixedDepositInfoRepository.SaveChanges();
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
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                //if (ex.InnerException != null && ex.InnerException is SqlException)
                //{
                //    SqlException sqlException = ex.InnerException as SqlException;
                //    model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                //}
                //else
                //{
                //    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                //}
            }

            setDetailList(model);
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
                List<Type> allTypes = new List<Type> { typeof(FMS_FixedDepositInfoInstallmentSchedule) };
                _fmsCommonService.FMSUnit.FixedDepositInfoRepository.Delete(id, allTypes);
                _fmsCommonService.FMSUnit.FixedDepositInfoRepository.SaveChanges();
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

        //[HttpPost, ActionName("DeleteDetail")]
        //public JsonResult DeleteDetailConfirmed(int id)
        //{
        //    bool result;
        //    string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
        //    try
        //    {
        //        _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.Delete(id);
        //        _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.SaveChanges();
        //        result = true;
        //        errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
        //    }
        //    catch (UpdateException ex)
        //    {
        //        try
        //        {
        //            if (ex.InnerException != null && ex.InnerException is SqlException)
        //            {
        //                SqlException sqlException = ex.InnerException as SqlException;
        //                errMsg = Common.GetSqlExceptionMessage(sqlException.Number);
        //                // if (ex.InnerException.Message.Contains("REFERENCE constraint"))
        //                // "The user has related information and cannot be deleted."
        //                ModelState.AddModelError("Error", errMsg);
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("Error", errMsg);
        //            }
        //            result = false;
        //        }
        //        catch (Exception)
        //        {
        //            result = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //    }

        //    return Json(new
        //    {
        //        Success = result,
        //        Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
        //    });
        //}

        #endregion

        #region Private Method

        private void populateDropdown(FixedDepositInfoViewModel model)
        {
            dynamic ddlList;

            #region bank ddl
            ddlList = _fmsCommonService.FMSUnit.BankInfoRepository.GetAll().OrderBy(x => x.BankName).ToList();
            model.BankInfoList = Common.PopulateFDRBankDDL(ddlList);
            #endregion

            #region bank ddl
            ddlList = _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.GetAll().OrderBy(x => x.BranchName).ToList();
            if (model.strMode == "Edit" || model.strMode == "Renew")
            {
                model.BankInfoBranchDetailList = Common.PopulateFDRBankBranchDDL(ddlList);
            }

            #endregion

            #region FDR Type ddl
            ddlList = _fmsCommonService.FMSUnit.FixedDepositTypeInfoRepository.GetAll().OrderBy(x => x.FixedDepositType).ToList();
            model.FixedDepositTypeInfoList = Common.PopulateFDRTypeDDL(ddlList);
            #endregion

            #region bank account ddl
            ddlList = _fmsfunction.fnGetCOABudgetHeadList(LoggedUserZoneInfoId);
            model.BankAccountList = Common.PopulateBankAcNoddl(ddlList);
            #endregion

            #region profit receive ddl
            model.ProfitRecvList = _pgmCommonservice.PGMUnit.AccChartOfAccountRepository.GetAll().OrderBy(x => x.accountName)
                                    .Where(s => s.accountGroup == "Income" && s.isControlhead == 0)
                                    .Select(y => new SelectListItem()
                                    {
                                        Text = y.accountName,
                                        Value = y.id.ToString()
                                    }).ToList();
            #endregion

            #region checkno ddl
            ddlList = _fmsfunction.fnGetBankChequeNoList(LoggedUserZoneInfoId, model.BankAccountId);
            model.ChequeList = Common.PopulateChequeNoddl(ddlList);
            #endregion

            #region FDR duration ddl
            model.FDRDurationTypeList = Common.PopulateFDRDurationTypeList();
            #endregion

            #region Installment duration ddl
            model.InstallmentDurationTypeList = Common.PopulateFDRDurationTypeList();
            #endregion

            #region Source of Fund
            ddlList = _fmsCommonService.FMSUnit.SourceofFundRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            model.SourceofFundList = Common.PopulateDllList(ddlList);
            #endregion

            #region Acc Bank List
            model.AccBankList = _pgmCommonservice.PGMUnit.AccBankInfo.GetAll().OrderBy(x => x.bankName)
                                .Select(y => new SelectListItem()
                                {
                                    Text = y.bankName,
                                    Value = y.id.ToString()
                                }).ToList();
            #endregion

            #region Acc Branch List
            model.AccBranchList = _pgmCommonservice.PGMUnit.AccBankBranchInfo.GetAll().OrderBy(x => x.branchName)
                                .Select(y => new SelectListItem()
                                {
                                    Text = y.branchName,
                                    Value = y.id.ToString()
                                }).ToList();
            #endregion

        }

        private FixedDepositInfoViewModel GetInsertUserAuditInfo(FixedDepositInfoViewModel model, bool pAddEdit)
        {
            if (pAddEdit)
            {
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;
                foreach (var child in model.InstallmentSchedulList)
                {
                    child.IUser = User.Identity.Name;
                    child.IDate = DateTime.Now;
                }

            }
            else
            {
                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
                foreach (var child in model.InstallmentSchedulList)
                {
                    child.IUser = model.IUser;
                    child.IDate = model.IDate;

                    child.EUser = User.Identity.Name;
                    child.EDate = DateTime.Now;
                }
            }

            return model;
        }

        private FMS_FixedDepositInfo CreateEntity(FixedDepositInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();

            //branches
            foreach (var item in model.InstallmentSchedulList)
            {
                var fms_FixedDepositInfoInstallmentSchedule = new FMS_FixedDepositInfoInstallmentSchedule();

                fms_FixedDepositInfoInstallmentSchedule.Id = item.Id;
                fms_FixedDepositInfoInstallmentSchedule.InsDate = item.InsDate;
                fms_FixedDepositInfoInstallmentSchedule.InsAmount = item.InsAmount;
                fms_FixedDepositInfoInstallmentSchedule.Tax = item.Tax;
                fms_FixedDepositInfoInstallmentSchedule.BankCharge = item.BankCharge;
                fms_FixedDepositInfoInstallmentSchedule.Profit = item.Profit;
                fms_FixedDepositInfoInstallmentSchedule.IUser = User.Identity.Name;
                fms_FixedDepositInfoInstallmentSchedule.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    fms_FixedDepositInfoInstallmentSchedule.IUser = User.Identity.Name;
                    fms_FixedDepositInfoInstallmentSchedule.IDate = DateTime.Now;
                    entity.FMS_FixedDepositInfoInstallmentSchedule.Add(fms_FixedDepositInfoInstallmentSchedule);
                }
                else
                {
                    fms_FixedDepositInfoInstallmentSchedule.FixedDepositInfoId = model.Id;
                    fms_FixedDepositInfoInstallmentSchedule.EUser = User.Identity.Name;
                    fms_FixedDepositInfoInstallmentSchedule.EDate = DateTime.Now;

                    if (item.Id == 0)
                    {
                        _fmsCommonService.FMSUnit.FixedDepositInfoInstallmentScheduleRepository.Add(fms_FixedDepositInfoInstallmentSchedule);
                    }
                    else
                    {
                        _fmsCommonService.FMSUnit.FixedDepositInfoInstallmentScheduleRepository.Update(fms_FixedDepositInfoInstallmentSchedule);

                    }
                    _fmsCommonService.FMSUnit.FixedDepositInfoInstallmentScheduleRepository.SaveChanges();
                }

            }

            return entity;
        }

        private bool CheckDuplicateEntry(FixedDepositInfoViewModel model, int id)
        {
            if (id < 1)
            {
                return _fmsCommonService.FMSUnit.FixedDepositInfoRepository.Get(q => q.FDRNumber == model.FDRNumber).Any();
            }
            else
            {
                return _fmsCommonService.FMSUnit.FixedDepositInfoRepository.Get(q => q.FDRNumber == model.FDRNumber && id != q.Id).Any();
            }

        }

        private string ValidateFDRDate(FixedDepositInfoViewModel model, int id)
        {
            string errorMessage = string.Empty;

            dynamic ECPE;
            if (id < 1)
            {
                ECPE = (from fdr in _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll()
                        where fdr.FDRNumber == model.FDRNumber
                        select fdr).OrderBy(x => x.FDRDate).LastOrDefault();
            }
            else
            {
                ECPE = (from fdr in _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll()
                        where fdr.FDRNumber == model.FDRNumber && fdr.Id != model.Id
                        select fdr).OrderBy(x => x.FDRDate).LastOrDefault();
            }

            if (ECPE != null && ECPE.MaturityDate > model.FDRDate)
            {
                model.errClass = "failed";
                return errorMessage = "FDR date must be greater than previous FDR date.";
            }
            return errorMessage;
        }

        private void setDetailList(FixedDepositInfoViewModel model)
        {
            List<FixedDepositInfoInstallmentScheduleViewModel> list = new List<FixedDepositInfoInstallmentScheduleViewModel>();
            int sl = 0;
            foreach (var item in model.InstallmentSchedulList)
            {
                FixedDepositInfoInstallmentScheduleViewModel obj = new FixedDepositInfoInstallmentScheduleViewModel();
                obj.Id = obj.Id;
                obj.SLNo = (sl + 1);
                obj.InsDate = item.InsDate;
                obj.InsAmount = item.InsAmount;
                obj.Tax = item.Tax;
                obj.BankCharge = item.BankCharge;
                obj.Profit = item.Profit;
                list.Add(obj);
                sl = Convert.ToInt32(obj.SLNo);
            }
            model.InstallmentSchedulList = list;
            model.ShowRecord = "Show";
        }
        #endregion

        public ActionResult AddDetail(BankInfoViewModel model)
        {
            model.BankInfoBranchList = new List<BankInfoBranchDetailViewModel>();
            var branch = new BankInfoBranchDetailViewModel();
            branch.Id = 0;
            model.Id = 0;
            branch.BankInfoId = Convert.ToInt32(model.BankInfoId);
            branch.BranchName = model.BranchName;
            branch.BranchAddress = model.BranchAddress;
            branch.BranchContactNo = model.BranchContactNo;
            branch.BranchEmail = model.BranchEmail;
            branch.CountryId = Convert.ToInt32(model.CountryId);
            branch.CountryName = model.CountryName;
            model.BankInfoBranchList.Add(branch);
            return PartialView("_Details", model);

        }

        [NoCache]
        public JsonResult GetFDRDestils(int id)
        {
            var obj = _fmsCommonService.FMSUnit.FixedDepositTypeInfoRepository.GetByID(id);
            return Json(new
            {
                FDRDuration = obj.Duration,
                DurationType = obj.DurationType,
                InterestRate = obj.InterestRate,
                InstallmentIn = obj.InstallmentIn,
                InstallmentType = obj.InstallmentType,
                Tax = obj.Tax,
                BankChargeFix = obj.BankChargeFix
            }, JsonRequestBehavior.AllowGet);

        }

        [NoCache]
        public JsonResult GetBankBranchess(int id)
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
        public JsonResult GetBankCheckNo(int id)
        {
            var tempList = Common.PopulateChequeNoddl(_fmsfunction.fnGetBankChequeNoList(LoggedUserZoneInfoId, id));

            var list = tempList.Select(x => new { Id = x.Value, Name = x.Text }).OrderBy(x => x.Name).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public PartialViewResult AddInstallmentSchedule(FixedDepositInfoViewModel model, int NofoIns)
        {
            List<FixedDepositInfoInstallmentScheduleViewModel> list = new List<FixedDepositInfoInstallmentScheduleViewModel>();

            //ins. shedule 
            var insDuMonth = 0;
            if (model.InstallmentDurationType == "Month")
            {
                insDuMonth = Convert.ToInt32(model.InstallmentDuration);
            }
            else
            {
                insDuMonth = Convert.ToInt32(model.InstallmentDuration) * 12;
            }


            DateTime date = Convert.ToDateTime(model.StartDate);
            for (int i = 0; i < NofoIns; i++)
            {
                FixedDepositInfoInstallmentScheduleViewModel childObj = new FixedDepositInfoInstallmentScheduleViewModel();
                childObj.SLNo = (i + 1);
                childObj.InsDate = date;
                childObj.InsAmount = Convert.ToDecimal(model.InterestAmount);
                childObj.Tax = Convert.ToDecimal(model.TAXAmount == null ? 0 : model.TAXAmount);
                childObj.BankCharge = Convert.ToDecimal(model.BankCharge == null ? 0 : model.BankCharge);
                childObj.Profit = Convert.ToDecimal(model.InterestAmount - ((model.TAXAmount == null ? 0 : model.TAXAmount) + (model.BankCharge == null ? 0 : model.BankCharge)));
                list.Add(childObj);

                DateTime dtFirst = date;
                //int days = DateTime.DaysInMonth(date.Year, date.Month);
                //  date = date.AddMonths(insDuMonth).AddDays(days);
                date = date.AddMonths(insDuMonth);
                DateTime dtLast = date;
                //int months = (dtLast.Year - dtFirst.Year) * 12 + dtLast.Month - dtFirst.Month;
                //if (months >= 2)
                //{
                //    date = date.AddDays(-10);
                //}
            }
            model.InstallmentSchedulList = list;
            return PartialView("_Details", model);
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

        //Prepare Voucher

        public ActionResult PrepareVoucher(int id, string type)
        {
            var entity = _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetByID(id);
            var model = entity.ToModel();
            model.tempTotalBankCharge = entity.TotalBankCharge;
            model.strMode = "Edit";
            populateDropdown(model);
            GetFDRType(model);
            if (type == "success")
            {
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }
            return View(model);
        }
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

            var obj = _fmsContext.FMS_uspVoucherPosting(id).FirstOrDefault();
            if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VPostingUrl"].ToString() + "/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=" + obj.FundControlId + "&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            }

            return Json(new
            {
                redirectUrl = url
            });
        }
        [NoCache]
        public JsonResult GetAccBankBranch(int id)
        {
            var list = _pgmCommonservice.PGMUnit.AccBankBranchInfo.GetAll().OrderBy(x => x.branchName)
                        .Where(x=>x.bankId == id)
                        .Select(y => new SelectListItem()
                        {
                            Text = y.branchName,
                            Value = y.id.ToString()
                        }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        [NoCache]
        public JsonResult GetAccBankAccount(int id)
        {
            var list = _pgmCommonservice.PGMUnit.AccBankAccountInfo.GetAll().OrderBy(x => x.accountNumber)
                        .Where(x => x.bankBranchId == id && x.ZoneInfoId == LoggedUserZoneInfoId)
                        .Select(y => new SelectListItem()
                        {
                            Text = y.accountNumber,
                            Value = y.id.ToString()
                        }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult GetInitialDepositAmount(int id)
        {
            var amount = from CR in _pgmCommonservice.PGMUnit.AccChequeRegister.GetAll()
                                where CR.id == id
                                select CR.amount;
            return new JsonResult() { Data = amount, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

    }
}