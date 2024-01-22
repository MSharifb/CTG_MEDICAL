using BEPZA_MEDICAL.DAL.FAR;
using BEPZA_MEDICAL.Domain.FAR;
using BEPZA_MEDICAL.Web.Areas.FAR.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.Controllers
{
    public class DepreciationController : BaseController
    {

        #region Fields

        private readonly FARCommonService _farCommonService;
        private ERP_BEPZAFAREntities _farEntities = new ERP_BEPZAFAREntities();
        #endregion

        #region Ctor
        public DepreciationController(FARCommonService farCommonService)
        {
            this._farCommonService = farCommonService;
        }
        #endregion

        #region message Properties

        public string Message { get; set; }

        #endregion

        [NoCache]
        public ActionResult Index()
        {
            DepreciationViewModel model = new DepreciationViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, DepreciationViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<DepreciationViewModel> list = (from tr in _farCommonService.FARUnit.AssetDepreciationRepository.GetAll()
                                                where (tr.ZoneInfoId == LoggedUserZoneInfoId)
                                                select new DepreciationViewModel()
                                                {
                                                    Id = tr.Id,
                                                    FinancialYearId = tr.FinancialYearId,
                                                    FinYearName = tr.acc_Accounting_Period_Information.yearName,
                                                    FinYearEndDate=tr.acc_Accounting_Period_Information.periodEndDate,
                                                    ProcessDate = tr.ProcessDate,
                                                    Remarks = tr.Remarks
                                                }).OrderByDescending(q=>q.FinYearEndDate).ToList();

            if (request.Searching)
            {
                if (viewModel.FinancialYearId > 0)
                {

                    list = list.Where(d => d.FinancialYearId == viewModel.FinancialYearId).ToList();
                }

            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sort

            if (request.SortingName == "Id")
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

            if (request.SortingName == "FinYearName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FinYearName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FinYearName).ToList();
                }
            }


            if (request.SortingName == "ProcessDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ProcessDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ProcessDate).ToList();
                }
            }
            #endregion

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {

                // response.Records.Add(new JqGridRecord(d.strYear + "-" + d.strMonth, new List<object>()
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.FinancialYearId,
                    d.FinYearName,  
                    d.ProcessDate.ToString(DateAndTime.GlobalDateFormat),
                    d.Remarks ,
                    "Details",
                    "Rollback",
                    "PrepareVoucher", //added for voucher
                    d.Id //added for voucher
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }


        [NoCache]
        public ActionResult GoToDetails(int id)
        {
            var model = new DepreciationDetailViewModel();
            //string[] yearMonth = idYearMonth.Split('-');
            //model.strYear = yearMonth[0];
            //model.strMonth = yearMonth[1];
            model.DepreciationId = id;
            return View("DepreciationDetails", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetDetailList(JqGridRequest request, int depreciationId, DepreciationDetailViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<DepreciationDetailViewModel> list = (from dm in _farCommonService.FARUnit.AssetDepreciationRepository.GetAll()
                                                      join dd in _farCommonService.FARUnit.AssetDepreciationDetailRepository.GetAll() on dm.Id equals dd.DepreciationId
                                                      join ar in _farCommonService.FARUnit.FixedAssetRepository.GetAll() on dd.FixedAssetId equals ar.Id
                                                      join a in _farCommonService.FARUnit.AssetStatusRepository.GetAll() on dd.AssetStatusId equals a.Id
                                                      join ac in _farCommonService.FARUnit.AssetConditionRepository.GetAll() on dd.AssetConditionId equals ac.Id
                                                      join cat in _farCommonService.FARUnit.AssetCategoryRepository.GetAll() on ar.CategoryId equals cat.Id
                                                      where (dm.ZoneInfoId == LoggedUserZoneInfoId && dm.Id == depreciationId/* && dm.strMonth == month*/ && (model.CategoryId == 0 || ac.Id == model.CategoryId))
                                                      select new DepreciationDetailViewModel()
                                                      {
                                                          Id = dd.Id,
                                                          FinancialYearId = dm.FinancialYearId,
                                                          YearName = dm.acc_Accounting_Period_Information.yearName,
                                                          DepreciationId = dd.DepreciationId,
                                                          FixedAssetId = dd.FixedAssetId,
                                                          AssetStatusId = dd.AssetStatusId,
                                                          AssetConditionId = dd.AssetConditionId,
                                                          DepreciationRate = dd.DepreciationRate,
                                                          AssetCost = dd.AssetCost,
                                                          OBBookValue = dd.OBBookValue,
                                                          OBDepreciation = dd.OBDepreciation,
                                                          Depreciation = dd.Depreciation,
                                                          CBDepreciation = dd.CBDepreciation,
                                                          CBBookValue = dd.CBBookValue,
                                                          Remarks = dd.Remarks,
                                                          AssetName = ar.AssetName,
                                                          AssetCode = ar.AssetCode,
                                                          AssetStatusName = a.Name,
                                                          AssetCondition = ac.Name,
                                                          CategoryId = cat.Id,
                                                          AssetCategoryCode = cat.CategoryCode,
                                                          AssetCategory = cat.CategoryName
                                                      }).ToList();

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {
                //response.Records.Add(new JqGridRecord(Convert.ToString(d.Id + "-" + d.FixedAssetId + "-" + year + "-" + month), new List<object>()
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id + "-" + d.FixedAssetId + "-" + d.FinancialYearId), new List<object>()
                {   
                   //d.Id + "-" + d.DepreciationId+"-"+year+"-"+month,        
                   d.Id + "-" + d.DepreciationId+"-"+d.FinancialYearId, 
                   d.FinancialYearId,
                   d.YearName,
                   d.AssetCode,
                   d.AssetName,
                   d.CategoryId,
                   d.AssetCategory,
                   d.AssetStatusName,
                   d.DepreciationRate,
                   d.OBBookValue,                   
                   d.OBDepreciation,
                   d.Depreciation,
                   d.CBDepreciation,
                   d.CBBookValue,
                   "Remove"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }


        [NoCache]
        public ActionResult CreateOrEdit()
        {
            DepreciationViewModel model = new DepreciationViewModel();
            PopulateDropdown(model);
            //model.strYear = Convert.ToString(Common.CurrentDateTime.Year);
            //model.strMonth = Common.CurrentDateTime.ToString("MMMM");
            model.ProcessDate = System.DateTime.Now;
            return PartialView("_CreateOrEdit", model);
        }

        [NoCache]
        public JsonResult DepreciationProcess(int financialYearId, string remarks)
        {
            int result = 0;
            List<string> Message = new List<string>();
            bool Success = true;
            List<string> errorList = new List<string>();

            // errorList = GetBusinessLogicValidation(year, month);
            errorList = GetBusinessLogicValidation(financialYearId);

            if ((ModelState.IsValid) && (errorList.Count == 0))
            {
                result = _farCommonService.FARUnit.FunctionRepository.DepreciationProcess(LoggedUserZoneInfoId, financialYearId, DateTime.Now, remarks, User.Identity.Name);

                if (result == 0)
                {
                    Message.Add("Depreciation Calculation has been completed successfully.");
                }
                else
                {
                    Success = false;
                    Message.Add("Depreciation Calculation is failed.");
                }
            }
            else
            {
                Success = false;
                foreach (var msg in errorList)
                {
                    Message.Add(msg);
                }
            }

            return Json(new
            {
                Success = Success,
                Message = Message
            });
        }

        [HttpPost, ActionName("Rollback")]
        [NoCache]
        public JsonResult RollbackConfirmed(int id)
        {
            int rollbackResult = 0;
            //string[] salaryMonth = idYearMonth.Split('-');
            bool result = false;
            string errMsg = string.Empty;
            string errorList = string.Empty;

            errorList = GetBusinessLogicValidationRollback(id);

            if ((ModelState.IsValid) && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    rollbackResult = _farCommonService.FARUnit.FunctionRepository.DepreciationRollbackProcess(LoggedUserZoneInfoId, id, System.DateTime.Now, "", User.Identity.Name);
                    if (rollbackResult == 0)
                    {
                        result = true;
                        errMsg = "Rollback has been completed successfully.";
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                    }
                    else
                    {
                        errMsg = "Only last month allow to rollback.";
                    }
                }
            }
            else
            {
                errMsg = errorList;
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        [HttpPost, ActionName("RollbackIndividual")]
        [NoCache]
        public JsonResult RollbackIndividual(string idRollbackIndividual)
        {
            string[] ids = idRollbackIndividual.Split('-');

            int rollbackResult = 0;
            bool result = false;
            string errMsg = string.Empty;
            string errMsgList = string.Empty;

            errMsgList = GetBusinessLogicValidationRollbackIndividual(Convert.ToInt32(ids[2]), Convert.ToInt32(ids[1]));

            if ((ModelState.IsValid) && (string.IsNullOrEmpty(errMsgList)))
            {
                try
                {
                    rollbackResult = _farCommonService.FARUnit.FunctionRepository.DepreciationRollbackIndividualProcess(LoggedUserZoneInfoId, Convert.ToInt32(ids[1]), Convert.ToInt32(ids[0]), User.Identity.Name);
                    if (rollbackResult == 0)
                    {
                        result = true;
                        errMsg = "Rollback has been completed successfully.";
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                    }
                }
            }
            else
            {
                errMsg = errMsgList;
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        private string GetBusinessLogicValidationRollbackIndividual(int financialYearId, Int64 depreciationId)
        {
            string errMessage = string.Empty;

            int nextYearId = 0;
            var nextFinYearInfo = _farCommonService.FARUnit.FunctionRepository.getFinancialYear(financialYearId);

                nextYearId = Convert.ToInt32(nextFinYearInfo.FinancialYearId);

                var DepreciationExistInNextMonth = (from BM in _farCommonService.FARUnit.AssetDepreciationRepository.GetAll()
                                                    where BM.FinancialYearId==nextYearId
                                                    select BM).ToList();
                if (DepreciationExistInNextMonth.Count > 0)
                {
                    errMessage = "Rollback is failed. Because next financial year's depreciation exist.";
                }
            
            return errMessage;
        }


        #region Utilities
        [NoCache]
        private void PopulateDropdown(DepreciationViewModel model)
        {
            dynamic ddlList; 
            #region financial Year ddl
            ddlList = _farCommonService.FARUnit.FinancialYearRepository.GetAll().OrderByDescending(x => x.periodEndDate).ToList();
            model.FinancialYearList = Common.FinancialYearListDll(ddlList);
            #endregion
        }

        [NoCache]
        public List<string> GetBusinessLogicValidation(int financialYearId)
        {
            List<string> errorMessage = new List<string>();
            dynamic list = null;

            list = (from tr in _farCommonService.FARUnit.AssetDepreciationRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId)
                    select new
                    {
                        FinancialYearId = tr.FinancialYearId,
                        FinYearStartDate = tr.FinYearStartDate,
                        FinYearEndDate = tr.FinYearEndDate,  
                        FinYearName= tr.FinYearName
                    }).Distinct().OrderBy(x => x.FinYearEndDate).ToList().LastOrDefault();
            
            if (list != null)
            {
                int nextProcessYearId = 0;
                var nextFinYearInfo = _farCommonService.FARUnit.FunctionRepository.getFinancialYear(list.FinancialYearId);
                if (nextFinYearInfo != null)
                {
                    nextProcessYearId = Convert.ToInt32(nextFinYearInfo.FinancialYearId);
                }

                if (nextProcessYearId != financialYearId)
                {
                    errorMessage.Add("You can't skip the financial year. Please process the depreciation in sequential order. The last processed financial year - " + list.FinYearName);
                }
            }

            return errorMessage;
        }


        [NoCache]
        public string GetBusinessLogicValidationRollback(int depId)
        {
            string errorMessage = string.Empty;
            dynamic list = null;

            int deletefinYearId = _farCommonService.FARUnit.AssetDepreciationRepository.Get(q => q.Id == depId).FirstOrDefault().FinancialYearId;

            list = (from tr in _farCommonService.FARUnit.AssetDepreciationRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId)
                    select new
                    {
                        FinancialYearId = tr.FinancialYearId,
                        FinYearStartDate = tr.FinYearStartDate,
                        FinYearEndDate = tr.FinYearEndDate
                    }).Distinct().OrderBy(x => x.FinYearEndDate).ToList().LastOrDefault();

            if (list != null)
            {
                if (deletefinYearId != null && deletefinYearId != list.FinancialYearId)
                {
                    errorMessage = "Only last financial year allow to rollback.";
                }
            }
            return errorMessage;
        }


        [NoCache]
        public ActionResult GetYearList()
        {
            var list = _farCommonService.FARUnit.FinancialYearRepository.GetAll().OrderByDescending(x => x.periodEndDate).ToList();
            return PartialView("Select", Common.FinancialYearListDll(list));
        }
        
        [NoCache]
        public ActionResult GetCategoryList()
        {
            var list = Common.PopulateAssetCategoryDDL(_farCommonService.FARUnit.AssetCategoryRepository.GetAll().OrderBy(x => x.CategoryName).ToList());
            return PartialView("Select", list);
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

            var obj = _farEntities.FAR_uspVoucherPosting(id).FirstOrDefault();
            if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VPostingUrl"].ToString() + "/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=" + obj.FundControlId + "&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            
                //url = "http://tvllap32/VistaGL/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=2&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            }
            return Redirect(url);           
        }

    }
}