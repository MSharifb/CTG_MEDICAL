using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class OfficeEquipmentFurnitureInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Constructor

        public OfficeEquipmentFurnitureInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion


        //
        // GET: /PRM/OfficeEquipmentFurnitureInfo/
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, OfficeEquipmentFurnitureInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<OfficeEquipmentFurnitureInfoViewModel> list = (from offEqu in _prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.GetAll()
                                                                where (model.Id == 0 || model.Id == offEqu.Id)
                                                                &&(offEqu.ZoneInfoId == LoggedUserZoneInfoId)
                                                                select new OfficeEquipmentFurnitureInfoViewModel()
                                                                {
                                                                    Id=offEqu.Id,
                                                                    Name=offEqu.Name

                                                                }).ToList();


            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "Name")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Name).ToList();
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
                  d.Name,
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }
        [NoCache]
        public ActionResult NameforView()
        {
            var itemList = Common.PopulateDllList(_prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList());
            return PartialView("Select", itemList);
        }

        public ActionResult Create()
        {
            OfficeEquipmentFurnitureInfoViewModel model = new OfficeEquipmentFurnitureInfoViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(OfficeEquipmentFurnitureInfoViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            errorList = BusinessLogicValidation(model);

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                model.ZoneInfoId = LoggedUserZoneInfoId;
                var entity = model.ToEntity();
                try
                {
                    entity.IUser = User.Identity.Name;
                    entity.IDate = DateTime.Now;

                    _prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.Add(entity);
                    _prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.SaveChanges();

                    model.IsError = 0;
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
                return View(model);
            }

            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int Id)
        {
            var entity = _prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.GetByID(Id);
            var model = entity.ToModel();
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(OfficeEquipmentFurnitureInfoViewModel model)
        {
            model.IsError = 1;
            model.strMode = "Edit";
            model.ErrMsg = string.Empty;
            model.ErrMsg = BusinessLogicValidation(model);

            if (ModelState.IsValid && (string.IsNullOrEmpty(model.ErrMsg)))
            {
                model.ZoneInfoId = LoggedUserZoneInfoId;
                var entity = model.ToEntity();
                try
                {
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;

                    _prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.Update(entity);
                    _prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                }
                catch
                {
                    model.errClass = "failed";
                    model.IsError = 1;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                }
            }
            else
            {
                return View(model);
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.GetByID(id);
            try
            {
                _prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.Delete(id);
                _prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }
            return Json(new
            {
                Success = result,
                Message = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public string BusinessLogicValidation(OfficeEquipmentFurnitureInfoViewModel model)
        {
            string errorMessage = string.Empty;
            if (model.strMode != "Edit")
            {
                var requInfo = _prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.GetAll().Where(x=>x.Name==model.Name && x.ZoneInfoId==LoggedUserZoneInfoId).ToList();
                if (requInfo.Count > 0)
                {
                    errorMessage = "This Name Already Exist";
                }
            }
            return errorMessage;

        }

	}
}