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
    public class StatusDesignationInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonSevice;
        #endregion

        #region Constructor

        public StatusDesignationInfoController(PRMCommonSevice prmCommonSevice)
        {
            this._prmCommonSevice = prmCommonSevice;
        }

        #endregion

        #region Actions
        //
        // GET: /PRM/StatusDesignationInfo/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, StatusDesignationInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<StatusDesignationInfoViewModel> list = (from statusDesig in _prmCommonSevice.PRMUnit.StatusDesignationInfoRepository.GetAll()
                                                         where (string.IsNullOrEmpty(model.StatusDesignationName) || statusDesig.StatusDesignationName.Contains(model.StatusDesignationName))
                                                         && (statusDesig.ZoneInfoId == LoggedUserZoneInfoId)
                                                         select new StatusDesignationInfoViewModel()
                                                            {
                                                                Id = statusDesig.Id,
                                                                StatusDesignationName = statusDesig.StatusDesignationName,
                                                                SortOrder = statusDesig.SortOrder,
                                                            }).OrderBy(x => x.SortOrder).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "StatusDesignationName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.StatusDesignationName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.StatusDesignationName).ToList();
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
                  d.StatusDesignationName,
                  d.SortOrder                
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            StatusDesignationInfoViewModel model = new StatusDesignationInfoViewModel();          
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(StatusDesignationInfoViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;

            if (ModelState.IsValid)
            {
                model = GetInsertUserAuditInfo(model, true);
                model.ZoneInfoId = LoggedUserZoneInfoId;
                var entity = model.ToEntity();
                try
                {
                    _prmCommonSevice.PRMUnit.StatusDesignationInfoRepository.Add(entity);
                    _prmCommonSevice.PRMUnit.StatusDesignationInfoRepository.SaveChanges();
                    model.IsButtonHide = true;
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
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int Id, string type)
        {
            var entity = _prmCommonSevice.PRMUnit.StatusDesignationInfoRepository.GetByID(Id);
            var model = entity.ToModel();
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(StatusDesignationInfoViewModel model)
        {
            model.IsError = 1;
            model.ErrMsg = string.Empty;

            if (ModelState.IsValid)
            {
                model = GetInsertUserAuditInfo(model, false);
                model.ZoneInfoId = LoggedUserZoneInfoId;
                var entity = model.ToEntity();
                try
                {
                    _prmCommonSevice.PRMUnit.StatusDesignationInfoRepository.Update(entity);
                    _prmCommonSevice.PRMUnit.StatusDesignationInfoRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                }
                catch
                {
                    model.errClass = "failed";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                }
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _prmCommonSevice.PRMUnit.StatusDesignationInfoRepository.GetByID(id);
            try
            {
                _prmCommonSevice.PRMUnit.StatusDesignationInfoRepository.Delete(id);
                _prmCommonSevice.PRMUnit.StatusDesignationInfoRepository.SaveChanges();
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

        #endregion

        #region Private Method
        private StatusDesignationInfoViewModel GetInsertUserAuditInfo(StatusDesignationInfoViewModel model, bool pAddEdit)
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