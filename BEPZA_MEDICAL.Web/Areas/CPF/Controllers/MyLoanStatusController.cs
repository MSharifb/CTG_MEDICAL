using System;
using System.Linq;
using System.Web.Mvc;
using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Domain.PGM;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.MyLoanStatus;
using System.Data;
using BEPZA_MEDICAL.Web.SecurityService;
using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.Web.Utility;


namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class MyLoanStatusController : Controller
    {
        #region Fields
        private readonly CPFCommonService _cpfCommonservice;
        private readonly EmployeeService _empService;
        //private readonly PIMCommonService _pimCommonservice;
        private readonly PGMCommonService _pgmCommonservice;
        private ERP_BEPZACPFEntities cpfContext = new ERP_BEPZACPFEntities();
        #endregion

        #region Constructor

        public MyLoanStatusController(CPFCommonService cpfCommonservice, EmployeeService empService, PGMCommonService pgmCommonService)
        {
            this._cpfCommonservice = cpfCommonservice;
            this._empService = empService;

            this._pgmCommonservice = pgmCommonService;

        }
        #endregion

        #region Actions

        public ActionResult Index()
        {
            MyLoanStatusViewModel model = new MyLoanStatusViewModel();
            User user = MyAppSession.User;
            var empInfo = (from m in _empService.PRMUnit.EmploymentInfoRepository.GetAll() where m.EmpID == user.EmpId select m).FirstOrDefault();
            if (empInfo != null)
            {
                var loanStatus = _cpfCommonservice.CPFUnit.FunctionRepository.GetMyLoanSummary(empInfo.Id).LastOrDefault();
                if (loanStatus != null)
                {
                    model.EmpID = loanStatus.EmpID;
                    model.EmployeeInitial = loanStatus.EmpID;
                    model.FullName = loanStatus.FullName;
                    model.MembershipID = loanStatus.MembershipID;
                    model.BalanceOn = DateTime.Now.ToString(DateAndTime.GlobalDateFormat);
                    model.MembershipLength = Math.Round(Convert.ToDecimal(loanStatus.MembershipLength), 2);
                    model.LoanNo = loanStatus.LoanNo;
                    model.LoanDate = loanStatus.LoanDate.ToString(DateAndTime.GlobalDateFormat);
                    model.LoanAmount = Math.Round(Convert.ToDecimal(loanStatus.LoanAmount), 2);
                    model.InterestRate = Math.Round(Convert.ToDecimal(loanStatus.InterestRate), 2);
                    model.InterestAmount = Math.Round(Convert.ToDecimal(loanStatus.Interest), 2);
                    model.TotalRepayment = Math.Round(Convert.ToDecimal(loanStatus.LoanAmount + loanStatus.Interest), 2);
                    model.NoOfInstallmentPrincipal = loanStatus.GrantedPrincipalInstallmentNo;
                    model.NoOfInstallmentInterest = loanStatus.GrantedInterestInstallmentNo;
                    model.TotalNoOfInstallment = Convert.ToInt32(loanStatus.GrantedInterestInstallmentNo + loanStatus.GrantedPrincipalInstallmentNo);
                    model.PrincipalInstallment = Math.Round(loanStatus.Installment, 2);
                    model.UnpaidInstallmentAmount = Math.Round(Convert.ToDecimal(loanStatus.Balance), 2);
                    model.PaidInstallmentAmount = Math.Round(Convert.ToDecimal(loanStatus.Installment), 2);
                    model.NoOfPaidInstallment = Convert.ToInt32(loanStatus.NoOfPrincipalPaid);
                    model.NoOfUnPaidInstallment = Convert.ToInt32(loanStatus.GrantedInterestInstallmentNo + loanStatus.GrantedPrincipalInstallmentNo - loanStatus.NoOfPrincipalPaid - loanStatus.NoOfInterestPaid);
                }
            }

            var companyInfo = _empService.PRMUnit.CompanyInformation.GetAll().LastOrDefault();
            if (companyInfo != null)
            {
                model.CompanyName = companyInfo.CompanyName;
                model.CompanyAddress = companyInfo.Address;
            }

            return View(model);
        }

        #endregion
    }
}
