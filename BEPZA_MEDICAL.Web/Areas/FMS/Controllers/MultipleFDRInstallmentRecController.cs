using BEPZA_MEDICAL.Domain.FMS;
using BEPZA_MEDICAL.Web.Areas.FMS.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.Controllers
{
    public class MultipleFDRInstallmentRecController : BaseController
    {
        #region Fields
        private readonly FMSCommonService _fmsCommonService;
        #endregion

        #region Ctor
        public MultipleFDRInstallmentRecController(FMSCommonService fmsCommonService)
        {
            this._fmsCommonService = fmsCommonService;
        }
        #endregion

        // GET: FMS/MultipleFDRInstallmentRec
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            FDRInstallmentInformationViewModel model = new FDRInstallmentInformationViewModel();
            model.Date = DateTime.Now;
            model.Ratio = 100;
            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(FDRInstallmentInformationViewModel model)
        {
            string errorList = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {

                    foreach(var item in model.InstallmentList)
                    {
                        var entity = model.ToEntity();

                        if (item.IsCheckedFinal)
                        {
                            entity.Date = item.Date;
                            entity.FixedDepositInfoId = item.FixedDepositInfoId;
                            entity.FDRAmount = item.FDRAmount;
                            entity.InterestRate = item.InterestRate;
                            entity.ProfitRecvId = item.ProfitRecvId == null ? 0 : Convert.ToInt32(item.ProfitRecvId);
                            entity.InstallmentAmount = item.InstallmentAmount;
                            entity.BankCharge = item.BankCharge;
                            entity.TAXAmount = item.TAXAmount;
                            entity.Profit = item.Profit;
                            entity.IUser = User.Identity.Name;
                            entity.IDate = DateTime.Now;
                            entity.ZoneInfoId = LoggedUserZoneInfoId;
                            entity.InterestReceiveAmount = item.InterestReceiveAmount;

                            _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.Add(entity);
                            _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.SaveChanges();
                        }
                    }

                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                }
                catch
                {
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                }
            }
            else
            {
                model.IsError = 1;
                model.errClass = "failed";
                model.ErrMsg = errorList;
            }
            populateDropdown(model);
            return View(model);
        }
        private void populateDropdown(FDRInstallmentInformationViewModel model)
        {
            #region From Month ddl
            model.MonthList = Common.PopulateMonthList2();
            #endregion

            #region Year ddl
            model.YearList = Common.PopulateYearList();
            #endregion
        }

        [HttpGet]
        public PartialViewResult GetInstallmentInfo(DateTime dateFrom, DateTime dateTo, decimal ratio)
        {
            var model = new FDRInstallmentInformationViewModel();

            var ExistingList = _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.GetAll().Where(x => x.Date >= dateFrom && x.Date <= dateTo).Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).ToList();
                                
            List<FDRInstallmentInformationViewModel> InstallmentList = new List<FDRInstallmentInformationViewModel>();

            var list = (from fdi in _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll()
                       join fdiSdl in _fmsCommonService.FMSUnit.FixedDepositInfoInstallmentScheduleRepository.GetAll() on fdi.Id equals fdiSdl.FixedDepositInfoId
                       join bnk in _fmsCommonService.FMSUnit.BankInfoRepository.GetAll() on fdi.BankInfoId equals bnk.Id 
                       join brnch in _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.GetAll()  on fdi.BankInfoBranchDetailId equals brnch.Id
                       where (fdiSdl.InsDate >= dateFrom && fdiSdl.InsDate <= dateTo)
                       && (fdi.ZoneInfoId == LoggedUserZoneInfoId)
                       && (fdi.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"]))
                       select new FDRInstallmentInformationViewModel()
                       {
                           FixedDepositInfoId=fdiSdl.FixedDepositInfoId,
                           FDRNo=fdi.FDRNumber,
                           ProfitRecvId = fdi.ProfitRecvId,
                           FDRAmount = fdi.FDRAmount,
                           InterestRate = fdi.InterestRate,
                           InstallmentAmount = fdiSdl.InsAmount,
                           TAXAmount = fdiSdl.Tax,
                           BankCharge = fdiSdl.BankCharge,
                           Profit = fdiSdl.Profit,
                           InterestReceiveAmount = fdiSdl.Profit,
                           ChequeId = fdi.ChequeId,
                           Date = fdiSdl.InsDate,
                           BankName=bnk.BankName,
                           BranchName=brnch.BranchName
                       }).ToList();

            list = list.Where(n => !ExistingList.Select(x => x.Date.Month).Contains(n.Date.Month) || !ExistingList.Select(x => x.FixedDepositInfoId).Contains(n.FixedDepositInfoId)).ToList();

            foreach (var FDRIns in list)
            {
                //foreach (var item in ExistingList)
                //{
                //    if (FDRIns.Date.Month != item.Date.Month || FDRIns.FixedDepositInfoId != item.FixedDepositInfoId)
                //    {
                        var gridModel = new FDRInstallmentInformationViewModel
                        {
                            FixedDepositInfoId = FDRIns.FixedDepositInfoId,
                            ProfitRecvId = FDRIns.ProfitRecvId,
                            FDRAmount = FDRIns.FDRAmount,
                            InterestRate = FDRIns.InterestRate,
                            InstallmentAmount = FDRIns.InstallmentAmount,
                            InterestReceiveAmount = Math.Round(((FDRIns.Profit) * (ratio / 100)), 2),
                            TAXAmount = FDRIns.TAXAmount,
                            BankCharge = FDRIns.BankCharge,
                            Profit = FDRIns.Profit,
                            ChequeId = FDRIns.ChequeId,
                            Date = FDRIns.Date,
                            FDRNo = FDRIns.FDRNo,
                            BankName = FDRIns.BankName,
                            BranchName = FDRIns.BranchName

                        };
                        InstallmentList.Add(gridModel);
                //    }
                //}
            }
            model.InstallmentList = InstallmentList;

            return PartialView("_Details", model);
        }

    }
}