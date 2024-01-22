using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.MyCPFBalance;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.Settlement;
using BEPZA_MEDICAL.Web.SecurityService;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class MyCPFBalanceController : Controller
    {
        #region Fields
        private readonly CPFCommonService _cpfCommonservice;
        private readonly EmployeeService _empService;
        private ERP_BEPZACPFEntities cpfContext = new ERP_BEPZACPFEntities();
        #endregion

        #region Constructor

        public MyCPFBalanceController(CPFCommonService cpfCommonservice, EmployeeService empService)
        {
            this._cpfCommonservice = cpfCommonservice;
            this._empService = empService;
        }
        #endregion

        #region Actions

        public ActionResult Index()
        {
            MyCPFBalanceViewModel model = new MyCPFBalanceViewModel();
            User user = MyAppSession.User;
            string empId = user.EmpId;
            
            var myCPF = _cpfCommonservice.CPFUnit.FunctionRepository.GetMyCpfSummary(empId).FirstOrDefault();
            var empInfo = (from m in _empService.PRMUnit.EmploymentInfoRepository.GetAll() where m.EmpID == user.EmpId select m).FirstOrDefault();
            
            if (myCPF != null)
            {
                model.EmpID = myCPF.EmpID;
                model.FullName = myCPF.EmployeeName;
                model.MembershipID = myCPF.MembershipID;
                model.BalanceOn = DateTime.Now.ToString(DateAndTime.GlobalDateFormat);
                model.MembershipLength = Math.Round(Convert.ToDecimal(myCPF.MembershipLength), 2);

                model.OwnContributionAmount = Math.Round(myCPF.OwnContribution, 2);
                model.OwnShareofProfit = Math.Round(Convert.ToDecimal(myCPF.EmpProfitInPeriod), 2);
                model.OwnTotal = Math.Round(Convert.ToDecimal(myCPF.OwnContribution), 2) + Math.Round(Convert.ToDecimal(myCPF.EmpProfitInPeriod), 2);

                model.ComContributionAmount = Math.Round(myCPF.CompanyContribution, 2);
                model.ComShareofProfit = Math.Round(Convert.ToDecimal(myCPF.ComProfitInPeriod), 2);
                model.ComTotal = Math.Round(myCPF.CompanyContribution + myCPF.ComProfitInPeriod, 2);

                model.ForfeitedAmount = Math.Round(Convert.ToDecimal(myCPF.ForfeitedAmount), 2);
                model.ComTotalWithoutForfietedAmount = Math.Round(Convert.ToDecimal(myCPF.CompanyContribution), 2) + Math.Round(Convert.ToDecimal(myCPF.ComProfitInPeriod), 2) - Math.Round(Convert.ToDecimal(myCPF.ForfeitedAmount), 2);

                model.PFTotal = Math.Round(model.OwnTotal + model.ComTotalWithoutForfietedAmount, 2);
                
                model.LoanAmount = Math.Round(Convert.ToDecimal(myCPF.LoanAmount), 2);
                model.PaidAmount = Math.Round(Convert.ToDecimal(myCPF.PaidLoan), 2);
                model.Dues = Math.Round(Convert.ToDecimal(myCPF.LoanDues), 2);

                model.LoanDues = Math.Round(Convert.ToDecimal(myCPF.LoanDues), 2);
                model.NetBalance = Math.Round(Convert.ToDecimal(myCPF.NetBalance), 2);// - Math.Round(Convert.ToDecimal(objSettlement.DueLoan), 2);
            }

            if (empInfo != null)
            {
                model.EmpID = empInfo.EmpID;
                model.FullName = empInfo.FullName;
                model.BalanceOn = DateTime.Now.ToString(DateAndTime.GlobalDateFormat);
            }

            var companyInfo = _empService.PRMUnit.CompanyInformation.GetAll().LastOrDefault();
            if (companyInfo != null)
            {
                model.CompanyName = companyInfo.CompanyName;
                model.CompanyAddress = companyInfo.Address;
            }

            return View(model);
        }

        private SettlementViewModel GetSettlementInfo(SettlementViewModel viewData, CPF_MembershipInfo member, string id)
        {
            var employee = _empService.PRMUnit.EmploymentInfoRepository.Get(d => d.Id == member.EmployeeId).FirstOrDefault();
            var pfPeriod = "";
            if (employee != null)
            {
                viewData.EmployeeInitial = employee.EmployeeInitial;
                viewData.EmployeeName = member.EmployeeName;
                viewData.EmployeeCode = member.EmployeeCode;
                viewData.MembershipId = member.Id;
                viewData.MembershipCode = member.MembershipID;

                TimeSpan days = DateTime.Now - Convert.ToDateTime(member.ApplicationDate);
                double MembershipLengthInDays = days.TotalDays;
                viewData.MembershipLengthInYear = Math.Round(Convert.ToDecimal(MembershipLengthInDays / 365), 2);

                #region   // Opening Balance Setting---------------------------
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

                //viewData.EmpContributionInPeriod = cpfContext.CPF_GetContributionDuringtheYear(pfPeriod.StartDate, pfPeriod.EndDate, member.EmployeeId, true).Sum(d => d.HeadAmount);
                //viewData.ComContributionInPeriod = cpfContext.CPF_GetContributionDuringtheYear(pfPeriod.StartDate, pfPeriod.EndDate, member.EmployeeId, false).Sum(d => d.HeadAmount);
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
                var forfeidRate = _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.Get(d => d.FromServiceLength >= viewData.MembershipLengthInYear && d.ToServiceLength <= viewData.MembershipLengthInYear).FirstOrDefault();
                if (forfeidRate != null)
                {
                    viewData.ForfeitedAmount = viewData.ComOpening + viewData.ComContributionInPeriod * Convert.ToDecimal(forfeidRate) / 100;
                }

                viewData.ComBalance = viewData.ComOpening + viewData.ComContributionInPeriod + viewData.ComProftInPeriod - viewData.ForfeitedAmount;
                viewData.GrandTotal = viewData.EmpBalance + viewData.ComBalance;
                //var objLoan = _cpfCommonservice.CPFUnit.LoanRepository.Get(d => d.IsActive == true && d.PeriodId == pfPeriod.Id).FirstOrDefault();
                var objLoan = _cpfCommonservice.CPFUnit.FunctionRepository.GetMyLoanSummary(employee.Id).LastOrDefault();
                if (objLoan != null)
                {
                    viewData.LoanAmount = objLoan.LoanAmount + objLoan.Interest;
                    viewData.LoanRefund = objLoan.Installment;
                    viewData.DueLoan = Convert.ToDecimal(objLoan.Balance);
                    viewData.NetPayable = viewData.GrandTotal - viewData.DueLoan - viewData.OtherDeduction;
                }

                #endregion // End Calculation
            }


            return viewData;
        }

        #endregion
    }
}
