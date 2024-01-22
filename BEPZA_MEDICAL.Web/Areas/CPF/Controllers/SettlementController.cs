using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.DAL.CPF.CustomEntities;
using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.OpeningBalance;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.Settlement;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using BEPZA_MEDICAL.Domain.PGM;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class SettlementController : Controller
    {

        #region Fields

        private readonly CPFCommonService _cpfCommonservice;
        private readonly EmployeeService _empService;
        private ERP_BEPZACPFEntities cpfContext = new ERP_BEPZACPFEntities();

        #endregion

        #region Constructor

        public SettlementController(CPFCommonService cpfCommonservice, EmployeeService empService)
        {
            this._cpfCommonservice = cpfCommonservice;
            this._empService = empService;
        }

        #endregion

        #region Actions

        [NoCache]
        public ActionResult Index()
        {
            SettlementViewModel model = new SettlementViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, SettlementViewModel model)
        {
            string filterExpression = string.Empty;
            int totalRecords = 0;
            var settlementList = _cpfCommonservice.CPFUnit.SettlementRepository.GetAll();
            var MemList = _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll();
            var empInfoList = _empService.PRMUnit.EmploymentInfoRepository.GetAll();
            var list = (from st in settlementList
                        join Mem in MemList on st.MembershipId equals Mem.Id
                        join empInfo in empInfoList on Mem.EmployeeId equals empInfo.Id

                        select new SettlementViewModel()
                        {
                            Id = st.Id,
                            EmployeeCode = Mem.EmployeeCode,
                            MembershipCode = Mem.MembershipID,
                            EmployeeName = Mem.EmployeeName,
                            PermanentDate = Mem.PermanentDate,
                            SettlementDate = st.SettlementDate,
                            SettlementToDate = st.SettlementDate,
                            EmpContributionInPeriod = st.EmpContributionInPeriod,
                            ComContributionInPeriod = st.ComContributionInPeriod,
                            NetPayable = st.NetPayable,
                        }).ToList();
            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(model.EmployeeCode))
                {
                    list = list.Where(d => d.EmployeeCode == model.EmployeeCode).ToList();
                }
                if (model.MembershipCode != null && model.MembershipCode != string.Empty)
                {
                    list = list.Where(d => d.MembershipCode == model.MembershipCode).ToList();
                }
                if (model.EmployeeName != null && model.EmployeeName != string.Empty)
                {
                    list = list.Where(d => d.EmployeeName == model.EmployeeName).ToList();
                }

                if (model.SettlementDate != null && model.SettlementDate != DateTime.MinValue)
                {
                    list = list.Where(d => d.SettlementDate >= model.SettlementDate).ToList();
                }
                if (model.SettlementToDate != null && model.SettlementToDate != DateTime.MinValue)
                {
                    list = list.Where(d => d.SettlementToDate <= model.SettlementToDate).ToList();
                }
                if (model.PeriodId != 0)
                {
                    list = list.Where(d => d.PeriodId == model.PeriodId).ToList();
                }
            }
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
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {         d.Id ,
                          d.EmployeeCode,
                          d.MembershipCode,
                          d.EmployeeName,
                          d.PeriodId ,
                          d.SettlementDate != null ? Convert.ToDateTime(d.SettlementDate).ToString(DateAndTime.GlobalDateFormat) : string.Empty,
                          d.SettlementToDate ,
                          d.EmpContributionInPeriod ,
                          d.ComContributionInPeriod ,
                          d.NetPayable ,

                           "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetActiveEmployeeList(JqGridRequest request, MembershipSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = (from ma in _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll()
                        join empInfo in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on ma.EmployeeId equals empInfo.Id
                        select new MembershipSearch()
                        {
                            ID = empInfo.Id,
                            EmpId = empInfo.EmpID,
                            GradeId = empInfo.PRM_EmpSalary.GradeId,
                            GradeName = empInfo.PRM_EmpSalary.PRM_JobGrade.GradeName,
                            DateOfJoining = empInfo.DateofJoining,
                            DateOfConfirmation = empInfo.DateofConfirmation,
                            DesignationId = empInfo.DesignationId,
                            DesigName = empInfo.PRM_Designation.Name,
                            DivisionId = empInfo.DivisionId,
                            DivisionName = empInfo.PRM_Division.Name,
                            EmpInitial = empInfo.EmployeeInitial,
                            EmpName = empInfo.FullName,
                            EmpTypeId = empInfo.EmploymentTypeId,
                            EmpTypeName = empInfo.PRM_EmploymentType.Name,
                            JobLocationId = empInfo.JobLocationId,
                            JobLocName = empInfo.PRM_JobLocation.Name,
                            ResourceLevelId = empInfo.ResourceLevelId,
                            StaffCategoryId = empInfo.StaffCategoryId,
                            DateOfInactive = empInfo.DateofInactive,
                            IsContractual = empInfo.IsContractual

                        }).ToList();


            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.ID), new List<object>()
                {
                    item.EmpName,
                    item.ID,
                    item.EmpId,
                    item.EmpInitial,
                    item.DesigName,
                    item.DivisionName,
                    item.JobLocName,
                    item.GradeName,
                    item.DateOfJoining.ToString(DateAndTime.GlobalDateFormat),
                    item.DateOfConfirmation.HasValue ? item.DateOfConfirmation.Value.ToString(DateAndTime.GlobalDateFormat) : null,

                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Create()
        {
            SettlementViewModel model = new SettlementViewModel();
            model.IsMonthly = _cpfCommonservice.CPFUnit.CPF_ProfitInterestRateRepository.GetAll().Where(x => x.PfPeriodDuration == "Monthly").Any();
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(SettlementViewModel model)
        {
            string errorList = string.Empty;

            model.IsError = 1;

            errorList = GetBusinessLogicValidation(model, "insert");

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                var obj = model.ToEntity();
                obj.IUser = User.Identity.Name;
                obj.IDate = Common.CurrentDateTime;

                try
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = "Duplicate Entry";

                        return View(model);

                    }

                    _cpfCommonservice.CPFUnit.SettlementRepository.Add(obj);
                    _cpfCommonservice.CPFUnit.SettlementRepository.SaveChanges();
                    model.IsError = 0;
                    //model.IsError = 3;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    model.errClass = "success";
                    //return RedirectToAction("PrepareVoucher", new { id = obj.Id, type = "success" });
                    return View("Index", model);
                }

                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                    }
                    else
                    {
                        model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                    }
                }
            }
            else
            {
                model.ErrMsg = errorList;
            }


            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            CPF_Settlement ObjSettlement = _cpfCommonservice.CPFUnit.SettlementRepository.GetByID(id);

            SettlementViewModel model = ObjSettlement.ToModel();
            if (ObjSettlement != null)
            {
                var member = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(d => d.Id == ObjSettlement.MembershipId).FirstOrDefault();

                if (member != null)
                {
                    var empInfo = _empService.PRMUnit.EmploymentInfoRepository.Get(d => d.Id == member.EmployeeId).FirstOrDefault();
                    //var period = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.Get(d => d.Id == ObjSettlement.PeriodId).FirstOrDefault();
                    model.EmployeeCode = member.EmployeeCode;
                    model.EmployeeInitial = empInfo.EmployeeInitial;
                    model.EmployeeName = member.EmployeeName;
                    //model.CpfPeriod = period.CPFPeriod;
                    model.MembershipCode = member.MembershipID;

                }

            }

            model.Mode = "Update";

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(SettlementViewModel model)
        {

            string errorList = string.Empty;
            model.IsError = 1;
            errorList = GetBusinessLogicValidation(model, "update");

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                CPF_Settlement obj = model.ToEntity();
                obj.EUser = User.Identity.Name;
                obj.EDate = Common.CurrentDateTime;

                if (CheckDuplicateEntry(model, model.Id))
                {

                    model.strMessage = Resources.ErrorMessages.UniqueIndex;
                    model.errClass = "failed";
                    return View(model);
                }

                Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                _cpfCommonservice.CPFUnit.SettlementRepository.Update(obj, NavigationList);
                _cpfCommonservice.CPFUnit.SettlementRepository.SaveChanges();
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

                return RedirectToAction("Index", model);
            }
            else
            {
                model.ErrMsg = errorList;
            }


            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = "Error while deleting data!";

            try
            {
                _cpfCommonservice.CPFUnit.SettlementRepository.Delete(id);
                _cpfCommonservice.CPFUnit.SettlementRepository.SaveChanges();
                result = true;
            }
            catch (UpdateException ex)
            {
                try
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
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
            catch (Exception)
            {
                result = false;
            }

            return Json(new
            {
                Success = result,
                Message = result ? "Information has been deleted successfully." : errMsg
            });
        }

        [NoCache]
        public JsonResult GetCompanyContributionColCaption()
        {
            var _customPropertiesService = new CustomPropertiesService();
            var companyContColCaption = _customPropertiesService.RetriveDisplayName("CPFFinalSattlementGridList", "CompanyContribution");

            return Json(new
            {
                Caption = Common.GetString(companyContColCaption)
            }, JsonRequestBehavior.AllowGet);

        }
        
        #endregion

        #region Utilities

        [HttpPost]
        public JsonResult GetSettlementInformation(string id, bool isEmpcode)
        {
            string empInitial = string.Empty;
            SettlementViewModel objSettlement = new SettlementViewModel();

            var member = new CPF_MembershipInfo();
            if (isEmpcode)
            {
                member = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(d => d.EmployeeCode == id).FirstOrDefault();
            }
            else
            {
                int mId = 0;
                int.TryParse(id, out mId);
                member = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(d => d.Id == mId).FirstOrDefault();
            }
            if (member != null)
            {
                var employee = _empService.PRMUnit.EmploymentInfoRepository.Get(d => d.Id == member.EmployeeId).FirstOrDefault();

                if (employee.DateofInactive != null)
                {
                    objSettlement = GetSettlementInfo(objSettlement, member, id, isEmpcode);

                    return Json(new
                    {
                        EmployeeCode = objSettlement.EmployeeCode,
                        EmployeeInitial = objSettlement.EmployeeInitial,
                        EmployeeName = objSettlement.EmployeeName,
                        MembershipId = objSettlement.MembershipId,
                        MembershipCode = objSettlement.MembershipCode,
                        PeriodId = objSettlement.PeriodId,
                        CpfPeriod = objSettlement.CpfPeriod,
                        ActiveDate = objSettlement.ActiveDate != null ? Convert.ToDateTime(objSettlement.ActiveDate).ToString("yyyy-MM-dd") : string.Empty,
                        InactiveDate = objSettlement.InactiveDate != null ? Convert.ToDateTime(objSettlement.InactiveDate).ToString("yyyy-MM-dd") : string.Empty,
                        OutgoingProfitRate = objSettlement.OutgoingProfitRate,
                        MembershipLengthInYear = objSettlement.MembershipLengthInYear,
                        EmpOpening = objSettlement.EmpOpening,
                        EmpContributionInPeriod = objSettlement.EmpContributionInPeriod,
                        EmpProftInPeriod = objSettlement.EmpProftInPeriod,
                        EmpWithdrawnInPeriod = objSettlement.EmpWithdrawnInPeriod,
                        EmpBalance = objSettlement.EmpBalance,
                        ComOpening = objSettlement.ComOpening,
                        ComContributionInPeriod = objSettlement.ComContributionInPeriod,
                        ComProftInPeriod = objSettlement.ComProftInPeriod,
                        ForfeitedAmount = objSettlement.ForfeitedAmount,
                        ComBalance = objSettlement.ComBalance,
                        LoanAmount = objSettlement.LoanAmount,
                        LoanRefund = objSettlement.LoanRefund,
                        DueLoan = objSettlement.DueLoan,
                        GrandTotal = objSettlement.GrandTotal,
                        OtherDeduction = objSettlement.OtherDeduction,
                        NetPayable = objSettlement.NetPayable,
                        LoanId = objSettlement.LoanId
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        Inactive = "Inactive"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    Inactive = "NotMember"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private SettlementViewModel GetSettlementInfo(SettlementViewModel viewData, CPF_MembershipInfo member, string id, bool isEmpcode)
        {
            var employee = _empService.PRMUnit.EmploymentInfoRepository.Get(d => d.Id == member.EmployeeId).FirstOrDefault();

            //var pfPeriod = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.Get(d => d.IsActive == true).FirstOrDefault();

            if (employee != null)
            {
                viewData.EmployeeInitial = employee.EmployeeInitial;
                viewData.EmployeeName = member.EmployeeName;
                viewData.EmployeeCode = member.EmployeeCode;
                viewData.MembershipId = member.Id;
                viewData.MembershipCode = member.MembershipID;
                viewData.InactiveDate = employee.DateofInactive;

                if (employee.DateofInactive != null && employee.DateofJoining != null)
                {
                    TimeSpan days = Convert.ToDateTime(employee.DateofInactive) - Convert.ToDateTime(employee.DateofJoining);
                    double MembershipLengthInDays = days.TotalDays;
                    viewData.MembershipLengthInYear = Math.Round(Convert.ToDecimal(MembershipLengthInDays / 365), 2);



                    #region   //Old Opening Balance Setting---------------------------
                    //var previousPeriod = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.Get(d => d.EndDate < pfPeriod.StartDate).OrderByDescending(m => m.EndDate).FirstOrDefault();
                    //if (previousPeriod != null)
                    //{
                    //    var profitDistribution =
                    //        (from pr in _cpfCommonservice.CPFUnit.ProfitDistributionRepository.GetAll()
                    //         join prD in _cpfCommonservice.CPFUnit.ProfitDistributionDetailRepository.GetAll() on pr.Id equals prD.ProfitDistributionId
                    //         where pr.PeriodId == previousPeriod.Id
                    //         select new
                    //         {
                    //             pr.Id,
                    //             pr.PeriodId,
                    //             prD.ComClosingBalance,
                    //             prD.EmpClosingBalance
                    //         }).FirstOrDefault();

                    //    if (profitDistribution != null)
                    //    {
                    //        viewData.EmpOpening = profitDistribution.EmpClosingBalance;
                    //        viewData.ComOpening = profitDistribution.ComClosingBalance;
                    //    }
                    //}
                    //else
                    //{

                    //    var cpfOpeningBalance = _cpfCommonservice.CPFUnit.OpeningBalanceRepository.Get(d => d.PeriodId == pfPeriod.Id && d.MembershipId == member.Id).FirstOrDefault();

                    //    viewData.EmpOpening = cpfOpeningBalance.EmpTotal;
                    //    viewData.ComOpening = cpfOpeningBalance.ComTotal;
                    //}

                    #endregion //OB


                    #region New for OB

                    var cpfOpeningBalance = _cpfCommonservice.CPFUnit.OpeningBalanceRepository.Get(d =>d.MembershipId == member.Id).FirstOrDefault();
                    var data = _cpfCommonservice.CPFUnit.FunctionRepository.GetMyCpfSummary(employee.EmpID).FirstOrDefault();
                    viewData.EmpOpening = cpfOpeningBalance.EmpTotal;
                    viewData.ComOpening = cpfOpeningBalance.ComTotal;

                    #endregion

                    //viewData.EmpContributionInPeriod = cpfContext.CPF_GetContributionDuringtheYear(pfPeriod.StartDate, pfPeriod.EndDate, member.EmployeeId, true).Sum(d => d.HeadAmount);
                    //viewData.ComContributionInPeriod = cpfContext.CPF_GetContributionDuringtheYear(pfPeriod.StartDate, pfPeriod.EndDate, member.EmployeeId, false).Sum(d => d.HeadAmount);

                    viewData.EmpContributionInPeriod = cpfContext.CPF_GetContributionOfPF(member.EmployeeId, true).Sum(d => d.HeadAmount);
                    viewData.ComContributionInPeriod = cpfContext.CPF_GetContributionOfPF(member.EmployeeId, false).Sum(d => d.HeadAmount);

                    var specialAccount = _cpfCommonservice.CPFUnit.SpecialAccountHeadRepository.GetAll().OrderByDescending(d => d.Id).FirstOrDefault();

                    var profitInfo = (from pd in _cpfCommonservice.CPFUnit.ProfitDistributionDetailRepository.GetAll()
                                      join p in _cpfCommonservice.CPFUnit.ProfitDistributionRepository.GetAll() on pd.ProfitDistributionId equals p.Id
                                      //join period in _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.Get(t => t.IsActive == true) on p.PeriodId equals period.Id
                                      join mbr in _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(t => t.Id == member.Id) on pd.MembershipId equals mbr.Id
                                      select pd).DefaultIfEmpty().OfType<CPF_ProfitDistributionDetail>().ToList();

                    viewData.EmpProftInPeriod = profitInfo.Sum(t => t.EmpProfitInPeriod);
                    viewData.ComProftInPeriod = profitInfo.Sum(t => t.ComProfitInPeriod);

                    if (specialAccount != null)
                    {
                        viewData.OutgoingProfitRate = specialAccount.OutgoingMemProfitRate;
                        viewData.EmpProftInPeriod = viewData.EmpOpening * specialAccount.OutgoingMemProfitRate / 100;
                        viewData.ComProftInPeriod = viewData.ComOpening * specialAccount.OutgoingMemProfitRate / 100;
                    }
                    //viewData.EmpWithdrawnInPeriod = _cpfCommonservice.CPFUnit.WithdrawRepository.Get(d => d.PeriodId == pfPeriod.Id).Sum(m => m.WithdrawAmount);

                    #region // Start Calculation--------------------
                    viewData.EmpBalance = viewData.EmpOpening + viewData.EmpContributionInPeriod + viewData.EmpProftInPeriod - viewData.EmpWithdrawnInPeriod;

                    var forfeidRate = _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.Get(d => d.FromServiceLength <= viewData.MembershipLengthInYear && d.ToServiceLength >= viewData.MembershipLengthInYear).FirstOrDefault();
                    if (forfeidRate != null)
                    {
                        viewData.ForfeitedAmount = viewData.ComOpening + viewData.ComContributionInPeriod * Convert.ToDecimal(forfeidRate.ForfeitedRate) / 100;
                    }

                    viewData.ComBalance = viewData.ComOpening + viewData.ComContributionInPeriod + viewData.ComProftInPeriod - viewData.ForfeitedAmount;
                    viewData.GrandTotal = viewData.EmpBalance + viewData.ComBalance;

                    #region Loan 
                     // PF Loan is now controlling from loan module
                    //var objLoan = _cpfCommonservice.CPFUnit.LoanRepository.Get(d => d.IsActive == true && d.PeriodId == pfPeriod.Id).FirstOrDefault();
                    //if (objLoan != null)
                    //{
                    //    viewData.LoanAmount = objLoan.LoanAmount;
                    //    viewData.LoanId = objLoan.Id;
                    //}

                    //var refund = (from loan in _cpfCommonservice.CPFUnit.LoanRepository.GetAll()
                    //              join loanDetail in _cpfCommonservice.CPFUnit.LoanDetailRepository.GetAll() on loan.Id equals loanDetail.LoanId
                    //              where loan.IsActive == true && loan.PeriodId == pfPeriod.Id && loanDetail.IsPaid == true
                    //              select new { loan.LoanAmount }).Sum(m => m.LoanAmount);
                    //if (refund != null)
                    //{
                    //    viewData.LoanRefund = refund;
                    //}
                    //viewData.DueLoan = viewData.LoanAmount - viewData.LoanRefund;
                    //viewData.NetPayable = viewData.GrandTotal - viewData.DueLoan - viewData.OtherDeduction;

                    viewData.LoanAmount = Math.Round(Convert.ToDecimal(data.LoanAmount), 2);
                    viewData.LoanRefund = Math.Round(Convert.ToDecimal(data.PaidLoan), 2);
                    viewData.DueLoan = Math.Round(Convert.ToDecimal(data.LoanDues), 2);
                    viewData.NetPayable = viewData.GrandTotal - viewData.DueLoan - viewData.OtherDeduction;

                    #endregion // End Loan

                    #endregion // End Calculation
                }
            }
            return viewData;
        }

        [NoCache]
        public ActionResult ActiveEmployeeSearchList()
        {
            var model = new MembershipSearchViewModel();
            return View("ActiveEmployeeSearch", model);
        }

        [NoCache]
        public ActionResult GetMemberByEmployeeCode(string query)
        {
            query = query.Replace(" ", "");
            if (query.Length > 1)
            {
                int op = query.LastIndexOf(",");
                query = query.Substring(op + 1);
            }

            var users = (from u in _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll()
                         where u.EmployeeCode.Contains(query)
                         orderby u.EmployeeCode // optional
                         select u.EmployeeCode).Distinct().ToArray();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        #region Business Validation Logics-----------------
        private string GetBusinessLogicValidation(SettlementViewModel model, string action)
        {
            string errorMessage = string.Empty;
            if (model.OtherDeduction > model.GrandTotal - model.DueLoan)
            {
                errorMessage = "Other Deduction Amount is higher than payable amount.";

            }
            return errorMessage;
        }

        private bool CheckDuplicateEntry(SettlementViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _cpfCommonservice.CPFUnit.SettlementRepository.Get(q => q.MembershipId == model.MembershipId).Any();
            }

            else
            {
                return _cpfCommonservice.CPFUnit.SettlementRepository.Get(q => q.MembershipId == model.MembershipId && strMode != q.Id).Any();
            }
        }
        #endregion

        #region Populate DDL for Prepared Model------------------------

        [NoCache]
        public ActionResult GetDesignation()
        {
            var designations = _empService.PRMUnit.DesignationRepository.GetAll().ToList();

            return PartialView("_Select", Common.PopulateDllList(designations));
        }

        [NoCache]
        public ActionResult GetDivision()
        {
            var divisions = _empService.PRMUnit.DivisionRepository.GetAll().ToList();

            return PartialView("_Select", Common.PopulateDllList(divisions));
        }

        [NoCache]
        public ActionResult GetJobLocation()
        {
            var jobLocations = _empService.PRMUnit.JobLocationRepository.GetAll().ToList();

            return PartialView("_Select", Common.PopulateDllList(jobLocations));
        }

        [NoCache]
        public ActionResult GetGrade()
        {
            var grades = _empService.PRMUnit.JobGradeRepository.GetAll().ToList();

            return PartialView("_Select", Common.PopulateJobGradeDDL(grades));
        }

        [NoCache]
        public ActionResult GetEmploymentType()
        {
            var grades = _empService.PRMUnit.EmploymentTypeRepository.GetAll().ToList();
            return PartialView("_Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetStaffCategory()
        {
            var grades = _empService.PRMUnit.PRM_StaffCategoryRepository.GetAll().ToList();

            return PartialView("_Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetResource()
        {
            var grades = _empService.PRMUnit.ResourceLevelRepository.GetAll().ToList();

            return PartialView("_Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetEmployeeStatus()
        {
            var empStatus = _empService.PRMUnit.EmploymentStatusRepository.GetAll().ToList();
            return PartialView("_Select", Common.PopulateDllList(empStatus));
        }
        [NoCache]
        public ActionResult GetCPFPeriod()
        {
            //var periods = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.GetAll().ToList();

            return PartialView("_Select");
        }


        #endregion

        public ActionResult SettlementVoucherPosting(int mId)
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

            var obj = cpfContext.CPF_uspVoucherPostingForFinalSettlement(mId).FirstOrDefault();
            if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            {
                url = "http://tvllap32/VistaGL/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=2&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            }

            return Json(new
            {
                redirectUrl = url
            });
        }

        //public ActionResult PrepareVoucher(int id, string type)
        //{
        //    var entity = _cpfCommonservice.CPFUnit.SettlementRepository.GetByID(id);
        //    var model = entity.ToModel();
        //    //model.tempTotalBankCharge = entity.TotalBankCharge;
        //    model.Mode = "Edit";
        //    if (model != null)
        //    {
        //        var member = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(d => d.Id == model.MembershipId).FirstOrDefault();

        //        if (member != null)
        //        {
        //            var empInfo = _empService.PRMUnit.EmploymentInfoRepository.Get(d => d.Id == member.EmployeeId).FirstOrDefault();
        //            var period = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.Get(d => d.Id == model.PeriodId).FirstOrDefault();
        //            model.EmployeeCode = member.EmployeeCode;
        //            model.EmployeeInitial = empInfo.EmployeeInitial;
        //            model.EmployeeName = member.EmployeeName;
        //            model.CpfPeriod = period.CPFPeriod;
        //            model.MembershipCode = member.MembershipID;

        //        }

        //    }
        //    if (type == "success")
        //    {
        //        model.errClass = "success";
        //        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
        //    }
        //    return View(model);
        //}

        #endregion
    }
}
