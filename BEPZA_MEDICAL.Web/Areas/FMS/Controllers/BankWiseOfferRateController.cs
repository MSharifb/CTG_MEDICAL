using BEPZA_MEDICAL.Domain.FMS;
using BEPZA_MEDICAL.Web.Areas.FMS.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.Controllers
{
    public class BankWiseOfferRateController : Controller
    {
        #region Fields

        private readonly FMSCommonService _fmsCommonService;

        #endregion

        #region Ctor
        public BankWiseOfferRateController(FMSCommonService fmsfCommonService)
        {
            this._fmsCommonService = fmsfCommonService;
        }
        #endregion

        // GET: FMS/BankWiseOfferRate
        public ActionResult Index(BankWiseOfferRateViewModel model, string type)
        {
            var existing = _fmsCommonService.FMSUnit.BankWiseOfferRateRepository.GetAll().Where(x=>x.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"])).ToList();
            if (existing.Count > 0)
            {
                foreach(var item in existing)
                {
                    BankWiseOfferRateViewModel nmodel = new BankWiseOfferRateViewModel();

                    nmodel = item.ToModel();
                    GenerateRow(nmodel);
                    model.BankWiseOfferRateList.Add(nmodel);
                }
            }
            else
            {
                GenerateRow(model);
            }

            if (type == "success")
            {
                model.IsError = 0;
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            else if(type == "fail")
            {
                model.IsError = 1;
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
            }
            return View(model);
        }
        private BankWiseOfferRateViewModel GenerateRow(BankWiseOfferRateViewModel model)
        {
            //BankWiseOfferRateViewModel nModel = new BankWiseOfferRateViewModel();
            dynamic ddlList;

            #region bank ddl
            ddlList = _fmsCommonService.FMSUnit.BankInfoRepository.GetAll().OrderBy(x => x.BankName).ToList();
            model.BankList = Common.PopulateFDRBankDDL(ddlList);
            #endregion

            #region branch ddl
            ddlList = _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.GetAll().OrderBy(x => x.BranchName).ToList();
            model.BranchList = Common.PopulateFDRBankBranchDDL(ddlList);
            #endregion

            #region FDR duration ddl
            model.TypeList = Common.PopulateFDRDurationTypeList();
            #endregion

            model.BankWiseOfferRateList.Add(model);

            model.FDRTypeId = Convert.ToInt32(Session["FDRTypeId"]);

            return model;
        }

        [HttpPost]
        public ActionResult Create(BankWiseOfferRateViewModel model)
        {
            try
            {

                var existing = _fmsCommonService.FMSUnit.BankWiseOfferRateRepository.GetAll().Where(x => x.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"])).ToList();
                var existingInModel = model.BankWiseOfferRateList.Where(q => q.Id > 0).DefaultIfEmpty().ToList();
                var deleted = (from exist in existing
                                       where !(existingInModel.Any(dt => dt.Id == exist.Id && dt.Id != 0))
                                       select exist).ToList();

                foreach (var item in deleted)
                {
                    _fmsCommonService.FMSUnit.BankWiseOfferRateRepository.Delete(item);
                }
                foreach (var item in model.BankWiseOfferRateList)
                {
                    var entity = item.ToEntity();
                    if (item.Id > 0)
                    {
                        entity.EUser = User.Identity.Name;
                        entity.EDate = DateTime.Now;
                        _fmsCommonService.FMSUnit.BankWiseOfferRateRepository.Update(entity);
                    }
                    else
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = DateTime.Now;
                        _fmsCommonService.FMSUnit.BankWiseOfferRateRepository.Add(entity);
                    }
                }
                _fmsCommonService.FMSUnit.BankWiseOfferRateRepository.SaveChanges();
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                return RedirectToAction("Index", new {type = "success" });
            }
            catch (Exception ex)
            {
                model.errClass = "fail";
                model.ErrMsg = ex.ToString();
                return RedirectToAction("Index", new { type = "fail" });
            }
        }

        [HttpPost]
        public ActionResult AddNew(BankWiseOfferRateViewModel model)
        {
            GenerateRow(model);
            return PartialView("_Detail", model);
        }

        [HttpPost]
        public ActionResult GetBranchInfo(int bankId)
        {
            try
            {
                #region branch ddl
                var list = Common.PopulateFDRBankBranchDDL(_fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.GetAll().Where(x=>x.BankInfoId == bankId).OrderBy(x => x.BranchName).ToList());
                #endregion

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}