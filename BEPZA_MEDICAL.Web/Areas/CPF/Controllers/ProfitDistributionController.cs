using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.DAL.CPF.CustomEntities;
using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Domain.PGM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.ProfitDistribution;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;


namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class ProfitDistributionController : BaseController
    {
        #region Fields

        private readonly PRMCommonSevice _prmCommonservice;
        private readonly CPFCommonService _cpfCommonservice;

        private readonly PGMCommonService _pgmCommonSevice;
        private readonly ERP_BEPZACPFEntities _cpfContext;
        #endregion

        #region Ctor

        public ProfitDistributionController(PRMCommonSevice prmCommonService, CPFCommonService cpfCommonservice, PGMCommonService pgmCommonSevice, ERP_BEPZACPFEntities context)
        {
            _prmCommonservice = prmCommonService;
            _cpfCommonservice = cpfCommonservice;

            _pgmCommonSevice = pgmCommonSevice;
            _cpfContext = context;
        }

        #endregion

        #region Message Properties
        public string Message { get; set; }

        #endregion

        #region Actions


        [NoCache]
        public ActionResult Index()
        {
            var model = new ProfitDistributionViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, ProfitDistributionViewModel model)
        {
            string filterExpression = string.Empty;
            int totalRecords = 0;

            List<ProfitDistributionViewModel> list = (from tr in _cpfCommonservice.CPFUnit.ProfitDistributionRepository.GetAll()
                                                      select new ProfitDistributionViewModel()
                                                      {
                                                          Id = tr.Id,
                                                          PeriodYear = tr.PeriodYear,
                                                          PeriodMonth = tr.PeriodMonth ,
                                                          ProfitRate = tr.ProfitRate,
                                                          Remarks = tr.Remarks
                                                      }).ToList();

            if (request.Searching)
            {
                if (model.PeriodYear != null && !String.IsNullOrEmpty(model.PeriodYear))
                {
                    list = list.Where(t => t.PeriodYear == model.PeriodYear).ToList();
                }
                if (model.PeriodMonth != null && !String.IsNullOrEmpty(model.PeriodMonth))
                {
                    list = list.Where(t => t.PeriodMonth == model.PeriodMonth).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "ID")
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
                    d.PeriodYear,
                    d.PeriodMonth,
                    d.ProfitRate,
                    d.Remarks,
                    "Detail",
                    "Rollback"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult GoToDetails(int id)
        {
            var model = (from tr in _cpfCommonservice.CPFUnit.ProfitDistributionRepository.GetAll()
                         
                         where tr.Id == id
                         select new ProfitDistributionViewModel
                         {
                             Id = tr.Id,
                             PeriodYear = tr.PeriodYear,
                             PeriodMonth = tr.PeriodMonth,
                             ProfitRate = tr.ProfitRate,
                             Remarks = tr.Remarks
                         }).FirstOrDefault();

            return View("ProfitDistributionDetails", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetDetailList(JqGridRequest request, ProfitDistributionViewModel model, String Id)
        {
            int masterId = Convert.ToInt32(Id);
            var list = (from pd in _cpfCommonservice.CPFUnit.ProfitDistributionDetailRepository.Get(p=> p.ProfitDistributionId == masterId)
                        join M in _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll() on pd.MembershipId equals M.Id
                        join E in _pgmCommonSevice.PGMUnit.FunctionRepository.GetEmployeeList() on M.EmployeeId equals E.Id

                        where (string.IsNullOrEmpty(model.EmpID) || model.EmpID == E.EmpID)

                        select new ProfitDistributionDetailModel
                        {
                            Id = pd.Id,
                            MembershipId =  pd.MembershipId,
                            EmpOpening = pd.EmpOpening,
                            EmpContributionInPeriod =pd.EmpContributionInPeriod,
                            EmpProfitInPeriod = pd.EmpProfitInPeriod,
                            EmpWithdrawnInPeriod = pd.EmpWithdrawnInPeriod,
                            EmpFinalPayment = pd.EmpFinalPayment,
                            EmpClosingBalance = pd.EmpClosingBalance,
                            ComOpening = pd.ComOpening,
                            ComContributionInPeriod = pd.ComContributionInPeriod,
                            ComProfitInPeriod = pd.ComProfitInPeriod,
                            ComWithdrawnInPeriod = pd.ComWithdrawnInPeriod,
                            ComFinalPayment = pd.ComFinalPayment,
                            ComForfeitedAmount = pd.ComForfeitedAmount,
                            ComClosingBalance = pd.ComClosingBalance,
                            TotalBalance = pd.TotalBalance,
                            strMembershipID = M.MembershipID,
                            FullName=  E.FullName,
                            EmployeeInitial = E.EmployeeInitial,
                            EmpID = E.EmpID,
                            DesigSortingOrder = E.SortingOrder
                        }).OrderBy(o=> o.DesigSortingOrder).ToList();


            int totalRecords = list == null ? 0 : list.ToList().Count;
            int pageSize = request.PagesCount.HasValue ? request.PagesCount.Value : 1;

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * pageSize).ToList();

            JqGridResponse response = new JqGridResponse
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
                   d.MembershipId,
                   d.EmpID,
                   d.FullName,

                   d.EmpOpening,
                   d.EmpContributionInPeriod,
                   d.EmpProfitInPeriod,
                   d.EmpWithdrawnInPeriod,
                   d.EmpFinalPayment,
                   d.EmpClosingBalance,

                   d.ComOpening,
                   d.ComContributionInPeriod,
                   d.ComProfitInPeriod,
                   d.ComWithdrawnInPeriod,
                   d.ComFinalPayment,
                   d.ComForfeitedAmount,
                   d.ComClosingBalance,

                   d.TotalBalance,
                   "Remove"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult CreateOrEdit()
        {
            var model = new ProfitDistributionViewModel();

            SetProfitRateType(model);

            PopulateDropdown(model);
            return PartialView("_CreateOrEdit", model);
        }

        private string GetProfitRateType()
        {
            string ProfitRateType = "";

            var profitRate = _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.GetAll().FirstOrDefault();

            if (profitRate != null)
            {
                ProfitRateType = profitRate.PfPeriodDuration;
            }

            return ProfitRateType;
        }

        private void SetProfitRateType(ProfitDistributionViewModel model)
        {
            model.ProfitRateType = GetProfitRateType();
        }

        [NoCache]
        public JsonResult ProfitDistributionProcess(string PeriodYear, string PeriodMonth, decimal ProfitRate, string Remarks)
        {
            int result = 0;
            List<string> Message = new List<string>();
            bool Success = true;
            List<string> errorList = new List<string>();

            errorList = GetBusinessLogicValidation(PeriodYear, PeriodMonth, ProfitRate);

            if ((ModelState.IsValid) && (errorList.Count == 0))
            {
                result = _cpfCommonservice.CPFUnit.FunctionRepository.ProfitDistributionProcess(PeriodYear, PeriodMonth, ProfitRate, Remarks, User.Identity.Name);

                if (result == 0)
                {
                    Message.Add("Profit distribution has been completed successfully.");
                }
                else
                {
                    Success = false;
                    Message.Add("Profit distribution Process is failed!");
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
        public JsonResult RollbackConfirmed(int periodId)
        {
            int rollbackResult = 0;
            bool result = false;
            string errMsg = string.Empty;
            string errorList = string.Empty;

            errorList = GetBusinessLogicValidationRollback(periodId);

            if ((ModelState.IsValid) && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    rollbackResult = _cpfCommonservice.CPFUnit.FunctionRepository.ProfitDistributionProcessRollback(periodId, User.Identity.Name);
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
                        errMsg = "Only last year allow to rollback.";
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
        public JsonResult RollbackIndividual(int id)
        {
            int rollbackResult = 0;
            bool result = false;
            string errMsg = string.Empty;
            string errMsgList = string.Empty;

            int membershipid = Convert.ToInt16((from tr in _cpfCommonservice.CPFUnit.ProfitDistributionDetailRepository.GetAll()
                                                where tr.Id == id
                                                select tr.MembershipId).FirstOrDefault());

            //int periodid = Convert.ToInt16((from tr in _cpfCommonservice.CPFUnit.ProfitDistributionDetailRepository.GetAll()
            //                                join m in _cpfCommonservice.CPFUnit.ProfitDistributionRepository.GetAll() on tr.ProfitDistributionId equals m.Id
            //                                where tr.Id == id
            //                                select m.PeriodId).FirstOrDefault());

            //errMsgList = GetBusinessLogicValidationRollbackIndividual(periodid, membershipid);


            if ((ModelState.IsValid) && (string.IsNullOrEmpty(errMsgList)))
            {
                try
                {
                    //rollbackResult = _cpfCommonservice.CPFUnit.FunctionRepository.ProfitDistributionProcessRollbackIndividual(periodid, id, User.Identity.Name);
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

        #endregion

        #region Utilities

        [NoCache]
        public ActionResult GetCPFYear()
        {
            
            return PartialView("_Select",Common.PopulateYearList());
        }
        [NoCache]
        public ActionResult GetCPFMonth()
        {

            return PartialView("_Select", Common.PopulateMonthList());
        }

        public JsonResult GetProfitRate(string PeriodYear, string PeriodMonth)
        {
            decimal? ProfitRate = 0;

            if (PeriodYear != string.Empty)
            {
                try
                {
                    var listProfitInterest = _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.GetAll().ToList();

                    if (PeriodMonth == "0")
                    {
                        listProfitInterest = listProfitInterest.Where(t => t.Year == PeriodYear).ToList();
                    }
                    else
                    {
                        listProfitInterest = listProfitInterest.Where(t => t.Year == PeriodYear && t.Month == PeriodMonth).ToList();
                    }

                    if (listProfitInterest != null)
                    {
                        var pi = listProfitInterest.FirstOrDefault();

                        ProfitRate = pi.InterestRate;
                    }

                    return Json(new { ProfitRate = ProfitRate }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(new { ProfitRate = ProfitRate }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { ProfitRate = ProfitRate }, JsonRequestBehavior.AllowGet);
        }

        //[NoCache]
        //public ActionResult GetYearList()
        //{

        //    Dictionary<string, string> periodList = new Dictionary<string, string>();

        //    foreach (var item in _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.GetAll())
        //    {
        //        periodList.Add(Convert.ToString(item.Id), item.CPFPeriod);
        //    }

        //    return PartialView("_Select", periodList);
        //}

        [NoCache]
        public ActionResult GetDivisionList()
        {
            var model = new ProfitDistributionViewModel();
            model.DivisionList = Common.PopulateCountryDivisionDDL(_prmCommonservice.PRMUnit.DivisionRepository.GetAll().ToList());
            return View("_Select", model.DivisionList);
        }

        [NoCache]
        private void PopulateDropdown(ProfitDistributionViewModel model)
        {
            model.PFYearList = Common.PopulateYearList();
            model.PFMonthList = Common.PopulateMonthList();
        }

        [NoCache]
        public List<string> GetBusinessLogicValidation(string PeriodYear, string PeriodMonth, decimal ProfitRate)
        {
            List<string> errorMessage = new List<string>();
            dynamic lastProcessedInterest = null;

            string lastPeriod = string.Empty, lastMonthName = string.Empty, lastYearName = string.Empty;

            string profitPeriodDuration = _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.GetAll().Select(t => t.PfPeriodDuration).Distinct().FirstOrDefault();

            // Validate salary process month.
            // ----------------------------------->>>>>>>>>
            // Step: 1 - Get last month interest information

            lastProcessedInterest = (from SM in _cpfCommonservice.CPFUnit.ProfitDistributionRepository.GetAll()
                                     select new
                                     {
                                         dtdate = Convert.ToDateTime(SM.PeriodYear + "-" + SM.PeriodMonth + "-01")
                                     }).Distinct().OrderBy(x => x.dtdate).ToList().LastOrDefault();


            if (lastProcessedInterest != null)
            {
                DateTime LastMonth = Convert.ToDateTime(lastProcessedInterest.dtdate);
                DateTime nextMonth = LastMonth.AddMonths(1);
                DateTime interestMonth = Convert.ToDateTime(PeriodYear + "-" + PeriodMonth + "-01");


                var existingInfo = new CPF_ProfitDistribution();


                switch (profitPeriodDuration)
                {
                    case "Yearly":
                        existingInfo = (from tr in _cpfCommonservice.CPFUnit.ProfitDistributionRepository.GetAll()
                                        where tr.PeriodYear == PeriodYear
                                        select tr).FirstOrDefault();
                        if (existingInfo != null)
                        {
                            if (PeriodYear == existingInfo.PeriodYear)
                            {
                                errorMessage.Add("This period has been already processed.");
                            }
                            //else if ((nextPeriodId > 0) && (periodId != nextPeriodId))
                            //{
                            //    errorMessage.Add("You can't skip the PF period. Please process in sequential order of PF period.Last processed period is " + lastPeriod);
                            //}
                        }
                        break;
                    case "Monthly":

                        existingInfo = (from tr in _cpfCommonservice.CPFUnit.ProfitDistributionRepository.GetAll()
                                        where tr.PeriodYear == PeriodYear && tr.PeriodMonth == PeriodMonth
                                        select tr).FirstOrDefault();
                        if (existingInfo != null)
                        {
                            errorMessage.Add("This period has been already processed.");
                        }
                        else
                        {
                            if (interestMonth > nextMonth)
                            {
                                errorMessage.Add("You can't skip the PF period." + Environment.NewLine +
                                                    "Please process in sequential order of PF period.Last processed period is " + LastMonth.ToString("MMM/yyyy"));
                            }
                            else if (interestMonth < LastMonth)
                            {
                                errorMessage.Add("You can't process previous month of last processed month." + Environment.NewLine +
                                                "Please process in sequential order of PF period. Last processed period is " + LastMonth.ToString("MMM/yyyy"));
                            }
                        }
                        break;
                }
            }

            string profitRateType = GetProfitRateType();

            if (profitRateType == CPFEnum.ProfitRateType.Monthly.ToString() && PeriodMonth == string.Empty)
            {
                errorMessage.Add("Please give month");
            }

            if (ProfitRate == 0)
            {
                errorMessage.Add("Profit rate not defined. Please define profit rate first.");
            }

            return errorMessage;
        }

        [NoCache]
        private string GetIncomeYear(string year, string month)
        {
            string incomeyear = string.Empty;
            DateTime dtDate = Convert.ToDateTime(year + "/" + month + "/01");

            if (dtDate.Month < 7)
            {
                incomeyear = (Convert.ToInt16(year) - 1).ToString() + "-" + year;
            }
            else
            {
                incomeyear = year + "-" + (Convert.ToInt16(year) + 1).ToString();
            }
            return incomeyear;
        }

        [NoCache]
        private string GetBusinessLogicValidationRollbackIndividual(int periodid, int membershipid)
        {
            string errMessage = string.Empty;

            var checkingLastPeriod = (from M in _cpfCommonservice.CPFUnit.ProfitDistributionRepository.GetAll()
                                      join D in _cpfCommonservice.CPFUnit.ProfitDistributionDetailRepository.GetAll()
                                               on M.Id equals D.ProfitDistributionId
                                      where D.MembershipId == membershipid
                                      select M).ToList();

            if (checkingLastPeriod.Count > 0)
            {
                errMessage = "Only Last PF year can be allowed for rollback.";
            }

            //var checkingOutInWithheldPayment = (from w in _pgmCommonservice.PGMUnit.WithheldSalaryPayment.GetAll() where (w.SalaryYear == year && w.SalaryMonth == month && w.EmployeeId == empID) select w).ToList();
            //if (checkingOutInWithheldPayment.Count > 0)
            //{
            //    errMessage = "Rollback is denied.Because salary is already paid.";
            //}
            return errMessage;
        }

        [NoCache]
        public string GetBusinessLogicValidationRollback(int periodId)
        {
            string errorMessage = string.Empty;
            CPF_ProfitDistribution lastObj = new CPF_ProfitDistribution();
            int LastPeriodId;
            //string lastPeriod;
            string lastMonthName = string.Empty;
            string lastYearName = string.Empty;
            string periodDuration = string.Empty;

            //find last processed period
            lastObj = (from tr in _cpfCommonservice.CPFUnit.ProfitDistributionRepository.GetAll()
                       select tr).OrderByDescending(x => x.Id).Take(1).FirstOrDefault();

            var profitInterestRateList = _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.GetAll();
            if (profitInterestRateList != null && profitInterestRateList.Count() > 0)
            {
                periodDuration = profitInterestRateList.Select(t => t.PfPeriodDuration).Distinct().FirstOrDefault();
            }

            if (lastObj != null)
            {

                LastPeriodId = lastObj.Id;

                //lastPeriod = (from tr in _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.GetAll()
                //              where tr.Id == lastObj.PeriodId
                //              select tr.CPFPeriod).FirstOrDefault().ToString();

                int lastMonthId = 0;
                //int.TryParse(lastObj.PeriodMonth.Value.ToString(), out lastMonthId);
                //lastMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(lastMonthId);

                if ((periodId != LastPeriodId))
                {
                    switch (periodDuration)
                    {
                        case "Yearly":
                            errorMessage = "You can rollback only last period.Last processed period is " + lastObj.PeriodYear;
                            break;
                        case "Monthly":
                            errorMessage = "You can rollback only last period.Last processed period is " + lastObj.PeriodMonth + " / " + lastObj.PeriodYear;
                            break;
                    }

                }
            }

            return errorMessage;
        }

        public ActionResult ProfitVoucherPosting(int id)
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

            var obj = _cpfContext.CPF_uspVoucherPostingForPFProfit(id).FirstOrDefault();
            if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VPostingUrl"].ToString() + "/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=" + obj.FundControlId + "&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            }

            return Json(new
            {
                redirectUrl = url
            });
        }

        #endregion
    }
}
