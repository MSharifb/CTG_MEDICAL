using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class RetirementAgeInfoController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonSevice;
        #endregion

        #region Ctor
        public RetirementAgeInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonSevice = prmCommonService;
        }
        #endregion

        #region Action
        // GET: PRM/RetirementAgeInfo
        public ActionResult Index(string bText, string type)
        {
            RetirementAgeInfoViwModel model = new RetirementAgeInfoViwModel();
            model.btnText = "Save";
            model.strMode = "Create";
            var obj = _prmCommonSevice.PRMUnit.RetirementAgeInfoRepository.GetAll().FirstOrDefault();

            if (obj != null)
            {
                // childModel = obj.ToModel();   
                model.Id = obj.Id;
                model.RetirementAge = obj.RetirementAge;
                model.FreedomFighterAge = obj.FreedomFighterAge;
                model.strMode = "Edit";
                model.btnText = "Update";
            }
            else
            {
                model.RetirementAge = 0.0;
                model.FreedomFighterAge = 0.0;
            }

            if (bText == "Save" && type == "success")
            {
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }
            else if (bText == "Update" && type == "success")
            {
                model.btnText = "Update";
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(RetirementAgeInfoViwModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;

            if (ModelState.IsValid)
            {
                model = GetInsertUserAuditInfo(model, true);
                var entity = model.ToEntity();

                try
                {
                    if(entity.Id>0)
                    {
                        _prmCommonSevice.PRMUnit.RetirementAgeInfoRepository.Update(entity);
                        _prmCommonSevice.PRMUnit.RetirementAgeInfoRepository.SaveChanges();                                        
                        return RedirectToAction("Index", new { bText = model.btnText, type = "success" });
                    }
                    else
                    {
                        _prmCommonSevice.PRMUnit.RetirementAgeInfoRepository.Add(entity);
                        _prmCommonSevice.PRMUnit.RetirementAgeInfoRepository.SaveChanges();                                       
                        return RedirectToAction("Index", new { bText = model.btnText, type = "success" });
                    }
                  
                }
                catch
                {
                    if (entity.Id > 0)
                    {
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                    }
                    else
                    {
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                    }
                                  
                }
            }
            return View(model);
        }


        #endregion


        #region Private Method
        private RetirementAgeInfoViwModel GetInsertUserAuditInfo(RetirementAgeInfoViwModel model, bool pAddEdit)
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
        #endregion
    }
}