using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.Withdraw;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Domain.PGM;
using System.Data.SqlClient;
using System.Collections;
using System.Data;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.DAL.CPF;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class WithdrawController : Controller
    {
      
        #region Fields
        private readonly CPFCommonService _cpfCommonservice;
        private readonly PGMCommonService _pgmCommonservice;
        private readonly PRMCommonSevice _prmCommonservice;
        private readonly EmployeeService _empService;
        private ERP_BEPZACPFEntities cpfContext = new ERP_BEPZACPFEntities();
        #endregion

        #region Constructor

        public WithdrawController(CPFCommonService cpfCommonservice, EmployeeService empService, PGMCommonService pgmCommonService, PRMCommonSevice prmCommonService)
        {
            this._cpfCommonservice = cpfCommonservice;
            this._empService = empService;
            this._pgmCommonservice = pgmCommonService;
            this._prmCommonservice = prmCommonService;
        }

        #endregion

        #region Actions

        [NoCache]
        public ActionResult Index()
        {
            WithdrawViewModel model = new WithdrawViewModel();
            //populateDropdown(model);
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, WithdrawViewModel model)
        {
            string filterExpression = string.Empty;
            int totalRecords = 0;
            var settlementList = _cpfCommonservice.CPFUnit.WithdrawRepository.GetAll();
            var MemList = _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll();
            var empInfoList = _empService.PRMUnit.EmploymentInfoRepository.GetAll();
            var list = (from st in settlementList
                        join Mem in MemList on st.MembershipId equals Mem.Id
                        join empInfo in empInfoList on Mem.EmployeeId equals empInfo.Id
                        select new WithdrawViewModel()
                        {
                            Id = st.Id,
                            EmployeeCode = Mem.EmployeeCode,
                            MembershipId = Mem.Id,
                            MembershipCode = Mem.MembershipID,
                            EmployeeName = Mem.EmployeeName,
                            //EmployeeInitial = empInfo.EmployeeInitial,
                            WithdrawNo = st.WithdrawNo,
                            WithdrawDate = st.WithdrawDate,
                            WithdrawToDate = st.WithdrawDate,
                            EmpPortionBalance = st.EmpPortionBalance,
                            WithdrawAmount = st.WithdrawAmount,
                        }).ToList();

            if (request.Searching)
            {
                if (model.EmployeeCode != null)
                {
                    list = list.Where(d => d.EmployeeCode == model.EmployeeCode.Trim()).ToList();
                }
                if (model.MembershipCode != null && model.MembershipCode.Trim() != string.Empty)
                {
                    list = list.Where(d => d.MembershipCode == model.MembershipCode.Trim()).ToList();
                }
                if (model.EmployeeName != null && model.EmployeeName.Trim() != string.Empty)
                {
                    list = list.Where(d => d.EmployeeName == model.EmployeeName.Trim()).ToList();
                }
                //if (model.EmployeeInitial != null && model.EmployeeInitial.Trim() != string.Empty)
                //{
                //    list = list.Where(d => d.EmployeeInitial == model.EmployeeInitial.Trim()).ToList();
                //}
                if (model.WithdrawDate != null && model.WithdrawDate != DateTime.MinValue)
                {
                    list = list.Where(d => d.WithdrawDate >= model.WithdrawDate).ToList();
                }
                if (model.WithdrawToDate != null && model.WithdrawToDate != DateTime.MinValue)
                {
                    list = list.Where(d => d.WithdrawDate <= model.WithdrawToDate).ToList();
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
                {        
                    d.Id ,  
                    d.EmployeeCode,
                    d.MembershipCode,
                    d.EmployeeName,                         
                    //d.EmployeeInitial ,
                    d.WithdrawNo,
                    d.PeriodId ,
                    d.WithdrawDate !=null?Convert.ToDateTime(d.WithdrawDate).ToString(DateAndTime.GlobalDateFormat):string.Empty ,
                    d.WithdrawDate !=null?Convert.ToDateTime(d.WithdrawDate).ToString(DateAndTime.GlobalDateFormat):string.Empty , 
                    d.WithdrawToDate !=null?Convert.ToDateTime(d.WithdrawToDate).ToString(DateAndTime.GlobalDateFormat):string.Empty , 
                    d.EmpPortionBalance ,
                    d.WithdrawAmount ,                        
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
       
        [NoCache]
        public ActionResult Create()
        {
            WithdrawViewModel model = new WithdrawViewModel();
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(WithdrawViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            if (CheckDuplicateEntry(model, model.Id))
            {
                model.ErrMsg = "Duplicate Entry";
                return View(model);
            }
            errorList = GetBusinessLogicValidation(model, "insert");

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                var obj = model.ToEntity();
                
                try
                {
                    _cpfCommonservice.CPFUnit.WithdrawRepository.Add(obj);
                    _cpfCommonservice.CPFUnit.WithdrawRepository.SaveChanges();
                    model.IsError = 0;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    return RedirectToAction("Index");
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
                model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(errorList) ? ErrorMessages.InsertFailed : errorList) : Common.GetModelStateError(ModelState);
            }

            ModelState.Clear();
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {

            CPF_Withdraw ObjWithdraw = _cpfCommonservice.CPFUnit.WithdrawRepository.GetByID(id);

            WithdrawViewModel model = ObjWithdraw.ToModel();
            if (ObjWithdraw != null)
            {
                var member = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(d => d.Id == ObjWithdraw.MembershipId).FirstOrDefault();

                if (member != null)
                {
                    var empInfo = _empService.PRMUnit.EmploymentInfoRepository.Get(d => d.Id == member.EmployeeId).FirstOrDefault();
                    //var period = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.Get(d => d.Id == ObjWithdraw.PeriodId).FirstOrDefault();
                    model.EmployeeCode = member.EmployeeCode;
                    //model.EmployeeInitial = empInfo.EmployeeInitial;
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
        public ActionResult Edit(WithdrawViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            if (CheckDuplicateEntry(model, model.Id))
            {
                model.ErrMsg = "Duplicate Entry";
                return View(model);
            }
            errorList = GetBusinessLogicValidation(model, "update");

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    CPF_Withdraw obj = model.ToEntity();

                    _cpfCommonservice.CPFUnit.WithdrawRepository.Update(obj);
                    _cpfCommonservice.CPFUnit.WithdrawRepository.SaveChanges();
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    return RedirectToAction("Index");
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
                        model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                    }
                }
            }
            else
            {
                model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(errorList) ? ErrorMessages.InsertFailed : errorList) : Common.GetModelStateError(ModelState);
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
                _cpfCommonservice.CPFUnit.WithdrawRepository.Delete(id);
                _cpfCommonservice.CPFUnit.WithdrawRepository.SaveChanges();
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

        #endregion

        #region Utilities

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

        [HttpPost]
        public JsonResult GetWithdrawnInformation(string id, bool isEmpcode)
        {
            string empInitial = string.Empty;
            WithdrawViewModel objWithdraw = new WithdrawViewModel();
            objWithdraw = GetWithdrawInfo(objWithdraw, id, isEmpcode);
            return Json(new
            {
                //EmployeeInitial = objWithdraw.EmployeeInitial,
                EmployeeName = objWithdraw.EmployeeName,
                EmployeeCode = objWithdraw.EmployeeCode,
                MembershipId = objWithdraw.MembershipId,
                PeriodId = objWithdraw.PeriodId,
                CpfPeriod = objWithdraw.CpfPeriod,
                MembershipCode = objWithdraw.MembershipCode,
                WithdrawNo = objWithdraw.WithdrawNo,
                AlreadyWithdrawnAmount = objWithdraw.AlreadyWithdrawnAmount,
                WithdrawAmount = objWithdraw.WithdrawAmount,
                EmpPortionBalance = objWithdraw.EmpPortionBalance,
                WithdrawDate = objWithdraw.WithdrawDate,
                RequestDate = objWithdraw.RequestDate,
                Reason = objWithdraw.Reason
            }, JsonRequestBehavior.AllowGet);
        }

        //[NoCache]
        //public ActionResult GetCPFPeriod()
        //{
        //    var periods = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.GetAll().ToList();

        //    return PartialView("_Select", Common.PopulatePfPeriodDllList(periods));
        //}

        private int getRandomNo()
        {
            Random r = new Random();
            return r.Next(10000, 99999);
        }

        private WithdrawViewModel GetWithdrawInfo(WithdrawViewModel viewData, string id, bool isEmpcode)
        {
            var member = new CPF_MembershipInfo();
            if (isEmpcode)
            {
                viewData.EmployeeCode = id;
                member = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(d => d.EmployeeCode == viewData.EmployeeCode).FirstOrDefault();
            }
            else
            {
                int memberId = 0;
                int testId = Convert.ToInt32(id);
                if (int.TryParse(id, out memberId))
                {
                    member = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(d => d.Id == memberId).FirstOrDefault();
                }
            }
            if (member == null)
            {
                member = new CPF_MembershipInfo();
            }
            else
            {
                var employee = _empService.PRMUnit.EmploymentInfoRepository.Get(d => d.Id == member.EmployeeId).FirstOrDefault();
                //var pfPeriod = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.Get(d => d.IsActive == true).FirstOrDefault();
                if (employee != null)
                {
                    //viewData.EmployeeInitial = employee.EmployeeInitial;
                    viewData.EmployeeName = member.EmployeeName;
                    viewData.EmployeeCode = member.EmployeeCode;
                    viewData.MembershipId = member.Id;
                    viewData.MembershipCode = member.MembershipID;
                    viewData.MembershipDate = member.JoiningDate;
                    viewData.WithdrawNo =employee.EmpID+"-"+ getRandomNo().ToString();

                    viewData.AlreadyWithdrawnAmount = _cpfCommonservice.CPFUnit.WithdrawRepository.Get(d => d.MembershipId == member.Id).Sum(m => m.WithdrawAmount);
                    
                    #region Calculate Employee Portion Balance--------------

                    //viewData.EmpPortionBalance = EmployeeBalanceCalculation(pfPeriod, member);

                    #region Hide Code------
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
                    //        var EmpContributionInPeriod = cpfContext.GetContributionDuringtheYear(pfPeriod.StartDate, pfPeriod.EndDate, member.EmployeeId).Sum(d => d.HeadAmount);
                            
                    //        var specialAccount = _cpfCommonservice.CPFUnit.SpecialAccountHeadRepository.GetAll().OrderByDescending(d => d.Id).FirstOrDefault();
                    //        viewData.EmpPortionBalance = Convert.ToDecimal(profitDistribution.ComClosingBalance) + Convert.ToDecimal(EmpContributionInPeriod);
                    //        if (specialAccount != null)
                    //        {
                    //            var EmpProftInPeriod = profitDistribution.ComClosingBalance * specialAccount.OutgoingMemProfitRate / 100;
                    //            viewData.EmpPortionBalance = viewData.EmpPortionBalance + Convert.ToDecimal(EmpProftInPeriod);

                    //        }
                    //    }
                    //}
                    #endregion
                    #endregion

                }
            }
            return viewData;
        }

        private decimal EmployeeBalanceCalculation(CPF_MembershipInfo member)
        {
            decimal EmployeeContributionProfit = 0;
            decimal empCoreContributionOpening = 0;
            decimal empCoreContributionSalary = 0;
            if ( member != null)
            {
                //var profitDistribution =
                //    (from prM in _cpfCommonservice.CPFUnit.ProfitDistributionRepository.GetAll()
                //     join prD in _cpfCommonservice.CPFUnit.ProfitDistributionDetailRepository.GetAll() on prM.Id equals prD.ProfitDistributionId
                //     where prM.PeriodId == period.Id && prD.MembershipId == member.Id
                //     select new
                //     {
                //         prM.Id,
                //         prM.PeriodId,
                //         prD.EmpContributionInPeriod,
                //         prD.ComClosingBalance,
                //         prD.EmpClosingBalance
                //     });

                var profitDistribution =
                    (from prM in _cpfCommonservice.CPFUnit.ProfitDistributionRepository.GetAll()
                     join prD in _cpfCommonservice.CPFUnit.ProfitDistributionDetailRepository.GetAll() on prM.Id equals prD.ProfitDistributionId
                     where prD.MembershipId == member.Id
                     select new
                     {
                         prM.Id,
                         //prM.PeriodId,
                         prD.EmpContributionInPeriod
                     });

                if (profitDistribution != null)
                {
                    EmployeeContributionProfit += Convert.ToDecimal(profitDistribution.Sum(x => x.EmpContributionInPeriod));
                }

                //var openinBalanceContri = (from op in _cpfCommonservice.CPFUnit.OpeningBalanceRepository.GetAll() where op.PeriodId == period.Id && op.MembershipId == member.Id select op).LastOrDefault();
                var openinBalanceContri = (from op in _cpfCommonservice.CPFUnit.OpeningBalanceRepository.GetAll() 
                                           where op.MembershipId == member.Id select op).LastOrDefault();
                
                if (openinBalanceContri != null)
                {
                    empCoreContributionOpening += openinBalanceContri.EmpCoreContribution;
                }

                var specialHeadForContribution = _prmCommonservice.PRMUnit
                    .SalaryHeadRepository.GetAll()
                    .FirstOrDefault(s => s.IsPFCompanyContributionHead == true);
                if (specialHeadForContribution != null)
                {
                    var empCPFFromPGM = (from SM in _pgmCommonservice.PGMUnit.SalaryMasterRepository.GetAll()
                                         join SD in _pgmCommonservice.PGMUnit.SalaryDetailsRepository.GetAll() on SM.Id equals SD.SalaryId
                                              where SM.EmployeeId == member.EmployeeId && SD.HeadId == specialHeadForContribution.Id
                                              select SD).ToList();
                    if (empCPFFromPGM != null)
                    {
                        empCoreContributionSalary += empCPFFromPGM.Sum(s => s.HeadAmount);
                    }
                }
            }

            return Math.Round(empCoreContributionSalary + empCoreContributionOpening + EmployeeContributionProfit, 2);
        }

        private bool CheckDuplicateEntry(WithdrawViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _cpfCommonservice.CPFUnit.WithdrawRepository.Get(q => q.MembershipId == model.MembershipId && q.WithdrawNo == model.WithdrawNo).Any();
            }

            else
            {
                return _cpfCommonservice.CPFUnit.WithdrawRepository.Get(q => q.MembershipId == model.MembershipId  && q.WithdrawNo == model.WithdrawNo && strMode != q.Id).Any();
            }
        }

        private string GetBusinessLogicValidation(WithdrawViewModel model, string action)
        {
            string errorMessage = string.Empty;
            var member = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(d => d.EmployeeCode == model.EmployeeCode).FirstOrDefault();
            
            decimal withdrawepercent = ((model.AlreadyWithdrawnAmount + model.WithdrawAmount) * 100) / model.EmpPortionBalance;
            
            if (model.WithdrawAmount > model.EmpPortionBalance)
            {
                errorMessage = "Withdrawn amount cannot exceed balance of employee portion.";
            }
            else if (model.MembershipDate > model.WithdrawDate)
            {
                errorMessage = "Date of withdrawal must be higher than date of membership.";
            }
            else if (withdrawepercent > 80)
            {
                errorMessage = "Employee can withdraw at most 80% of own contribution.";
            }
            else if (model.WithdrawDate < model.RequestDate)
            {
                errorMessage = " Withdraw date must be higher than date of Request.";
            }

            return errorMessage;
        }


        #endregion
    }
}
