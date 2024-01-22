using BEPZA_MEDICAL.Domain.FMS;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.FMS.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.Controllers
{
    public class HomeController : Controller
    {
       #region Fields
        private readonly FMSCommonService _fmsfCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public HomeController(FMSCommonService fmsfCommonService, PRMCommonSevice prmCommonService)
        {
            this._fmsfCommonService = fmsfCommonService;
            this._prmCommonService = prmCommonService;
        }
        #endregion

        // GET: FMS/Home
        [NoCache]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FDRType(FDRTypeViewModel model)
        {
            model.FDRTypeList = PopulateDllList(_fmsfCommonService.FMSUnit.FDRTypeRepository.GetAll().ToList());
            return View(model);
        }

        public bool SetFDRType(int Id)
        {
            System.Web.HttpContext.Current.Session["FDRTypeId"] = Id;
            return true;
        }

        public static IList<SelectListItem> PopulateDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public ActionResult SwitchZone(int id, string zoneSwitchURL)
        {
            MyAppSession.ZoneInfoId = id;
            MyAppSession.ZoneName = _prmCommonService.PRMUnit.ZoneInfoRepository.GetByID(id).ZoneName;
            return Redirect(zoneSwitchURL);
        }

        public ActionResult Unauthorized()
        {
            return View();
        }
    }
}