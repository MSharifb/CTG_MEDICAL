using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class DegreeLevelMappingController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonSevice;
        #endregion

        #region Constructor

        public DegreeLevelMappingController(PRMCommonSevice prmCommonSevice)
        {
            this._prmCommonSevice = prmCommonSevice;
        }
        #endregion

        // GET: PRM/DegreeLevelMapping
        public ActionResult Index(DegreeLevelMappingViewModel model)
        {
            populateDropdown(model);
            model.ActionType = "DegreeLevelMappingCreate";
            return View("Index", model);
        }

        [HttpPost]
        public string GetList(List<DegreeLevelMappingViewModel> degreeLevelId, int degreeTypeId)
        {
            var DegreeLevel = _prmCommonSevice.PRMUnit.DegreeLevelMappingRepository.GetAll().Where(x => x.DegreeTypeId == degreeTypeId).FirstOrDefault();

            if (DegreeLevel != null)
            {
                List<Type> allTypes = new List<Type> { typeof(PRM_DegreeLevelMappingDetail) };
                _prmCommonSevice.PRMUnit.DegreeLevelMappingRepository.Delete(DegreeLevel.Id, allTypes);
                _prmCommonSevice.PRMUnit.DegreeLevelMappingRepository.SaveChanges();
            }
            DegreeLevelMappingViewModel model = new DegreeLevelMappingViewModel();
            model.ZoneInfoId = LoggedUserZoneInfoId;
            model.DegreeTypeId = degreeTypeId;
            var entity = model.ToEntity();
            foreach (var c in degreeLevelId)
            {
                var prm_DegreeLevelMappingDetail = new PRM_DegreeLevelMappingDetail();
                prm_DegreeLevelMappingDetail.Id = c.Id;
                prm_DegreeLevelMappingDetail.DegreeLevelId = Convert.ToInt32(c.DegreeLevelId);
                prm_DegreeLevelMappingDetail.IUser = User.Identity.Name;
                prm_DegreeLevelMappingDetail.IDate = DateTime.Now;
                prm_DegreeLevelMappingDetail.EUser = User.Identity.Name;
                prm_DegreeLevelMappingDetail.EDate = DateTime.Now;
                _prmCommonSevice.PRMUnit.DegreeLevelMappingDetailRepository.Add(prm_DegreeLevelMappingDetail);
            }
            _prmCommonSevice.PRMUnit.DegreeLevelMappingRepository.Add(entity);
            _prmCommonSevice.PRMUnit.DegreeLevelMappingRepository.SaveChanges();

            model.IsError = 0;
            model.errClass = "success";
            model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

            return model.ErrMsg;
        }

        private void populateDropdown(DegreeLevelMappingViewModel model)
        {

            #region Degree Type
            var degList = _prmCommonSevice.PRMUnit.DegreeTypeRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.DegreeTypeList = Common.PopulateDllList(degList);
            #endregion

            #region Available Degree Level
            //var List = _prmCommonSevice.PRMUnit.ExamDegreeLavelRepository.Fetch().OrderBy(x => x.Name).ToList();
            //model.AvailableDegreeLevelList = Common.PopulateDllList(List);
            #endregion

        }

        public ActionResult AvailableDegreeLevel(int degreeTypeId)
        {
            var items = (from de in _prmCommonSevice.PRMUnit.DegreeLevelMappingRepository.Fetch()
                         join JG in _prmCommonSevice.PRMUnit.DegreeLevelMappingDetailRepository.Fetch() on de.Id equals JG.DegreeLevelMappingId
                         where de.DegreeTypeId == degreeTypeId
                         select new
                         {
                             Id = JG.DegreeLevelId,
                         }).ToList();

            if (items.Count > 0)
            {
                var list = _prmCommonSevice.PRMUnit.ExamDegreeLavelRepository.GetAll().Where(x => !items.Select(n => n.Id).Contains(x.Id)).Select(x => new { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();
                return Json(list, JsonRequestBehavior.AllowGet
                           );
            }
            else
            {
                var list = _prmCommonSevice.PRMUnit.ExamDegreeLavelRepository.GetAll().Select(x => new { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();
                return Json(list, JsonRequestBehavior.AllowGet
                           );
            }

        }
        public ActionResult DegreeLevel(int degreeTypeId)
        {
            var items = (from de in _prmCommonSevice.PRMUnit.DegreeLevelMappingRepository.Fetch()
                         join JG in _prmCommonSevice.PRMUnit.DegreeLevelMappingDetailRepository.Fetch() on de.Id equals JG.DegreeLevelMappingId
                         where de.DegreeTypeId == degreeTypeId
                         select new
                         {
                             Id = JG.DegreeLevelId,
                         }).ToList();

                var list = _prmCommonSevice.PRMUnit.ExamDegreeLavelRepository.GetAll().Where(x => items.Select(n => n.Id).Contains(x.Id)).Select(x => new { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();
                return Json(list, JsonRequestBehavior.AllowGet
                );
        }

    }
}