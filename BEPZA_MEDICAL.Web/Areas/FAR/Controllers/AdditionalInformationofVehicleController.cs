using BEPZA_MEDICAL.DAL.FAR;
using BEPZA_MEDICAL.Domain.FAR;
using BEPZA_MEDICAL.Web.Areas.FAR.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.Controllers
{
    public class AdditionalInformationofVehicleController : BaseController
    {
        #region Fields
        private readonly FARCommonService _farCommonService;
        #endregion

        #region Ctor
        public AdditionalInformationofVehicleController(FARCommonService farCommonService)
        {
            this._farCommonService = farCommonService;
        }
        #endregion

        // GET: FAR/AdditionalInformationofVehicle
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, AdditionalInformationofVehicleViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from loc in _farCommonService.FARUnit.AdditionalInfoofVehicleRepository.GetAll()
                        join FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll() on loc.FIxedAssetId equals FA.Id
                        select new AdditionalInformationofVehicleViewModel()
                        {
                            Id = loc.Id,
                            FIxedAssetId = FA.Id,
                            AssetCode = FA.AssetCode,
                        }).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting
            if (request.SortingName == "AssetCode")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssetCode).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssetCode).ToList();
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
                    d.FIxedAssetId,
                    d.AssetCode
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }

        [NoCache]
        public ActionResult AssetCodeforView()
        {
            var itemList = Common.PopulateAssetCodeDDL(_farCommonService.FARUnit.FixedAssetRepository.GetAll().ToList());
            return PartialView("Select", itemList);
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            AdditionalInformationofVehicleViewModel model = new AdditionalInformationofVehicleViewModel();
            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(AdditionalInformationofVehicleViewModel model)
        {

            try
            {
                string errorList = string.Empty;
                model.IsError = 1;

                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, true);

                    _farCommonService.FARUnit.AdditionalInfoofVehicleRepository.Add(entity);
                    _farCommonService.FARUnit.AdditionalInfoofVehicleRepository.SaveChanges();
                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    return RedirectToAction("Edit", new { id = entity.Id, type = "SaveSuccess" });

                }
                else
                {
                    populateDropdown(model);
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = errorList;
                    return View(model);
                }
            }
            catch
            {
                model.IsError = 1;
                model.errClass = "failed";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
            }

            return View(model);
        }

        public ActionResult Edit(int id, string type)
        {
            var SelectionCriteriaEntity = _farCommonService.FARUnit.AdditionalInfoofVehicleRepository.GetByID(id);
            var parentModel = SelectionCriteriaEntity.ToModel();
            parentModel.strMode = "Edit";
            parentModel.IsInEditMode = true;

            List<AdditionalInformationofVehicleDetailViewModel> list = (from sel in _farCommonService.FARUnit.AdditionalInfoofVehicleRepository.GetAll()
                                                                        join selDtl in _farCommonService.FARUnit.AdditionalInfoofVehicleDetailRepository.GetAll() on sel.Id equals selDtl.AdditionalInfoofVehicleId
                                                                        join SP in _farCommonService.FARUnit.SparePartInformationRepository.GetAll() on selDtl.SparePartInfoId equals SP.Id
                                                                        where (selDtl.AdditionalInfoofVehicleId == id)
                                                                        select new AdditionalInformationofVehicleDetailViewModel()
                                                                        {
                                                                         Id = selDtl.Id,
                                                                         SparePartId = selDtl.SparePartInfoId,
                                                                         Quantity = selDtl.Quantity,
                                                                         SparePartName = SP.SparePart
                                                                         }).ToList();

            parentModel.AdditionalInfoVehicleDetailList = list;
            populateDropdown(parentModel);
            if (type == "success")
            {
                parentModel.IsError = 1;
                parentModel.errClass = "success";
                parentModel.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            else if (type == "SaveSuccess")
            {
                parentModel.IsError = 1;
                parentModel.errClass = "success";
                parentModel.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }
            return View("Edit", parentModel);
        }

        [HttpPost]
        public ActionResult Edit(AdditionalInformationofVehicleViewModel model)
        {
            try
            {
                string errorList = "";
                if (ModelState.IsValid)
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, false);

                    if (errorList.Length == 0)
                    {
                        _farCommonService.FARUnit.AdditionalInfoofVehicleRepository.Update(entity);
                        _farCommonService.FARUnit.AdditionalInfoofVehicleRepository.SaveChanges();
                        model.errClass = "success";
                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        return RedirectToAction("Edit", new { id = model.Id, type = "success" });
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 0;
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.IsError = 0;
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

            populateDropdown(model);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _farCommonService.FARUnit.AdditionalInfoofVehicleRepository.GetByID(id);
            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(FAR_AdditionalInfoofVehicleDetail) };
                    _farCommonService.FARUnit.AdditionalInfoofVehicleRepository.Delete(tempPeriod.Id, allTypes);
                    _farCommonService.FARUnit.AdditionalInfoofVehicleRepository.SaveChanges();
                    result = true;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                }
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
        [HttpPost, ActionName("DeleteAssetMaintenanceInformationDetail")]
        public JsonResult DeleteAssetMaintenanceInfoDltfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _farCommonService.FARUnit.AdditionalInfoofVehicleDetailRepository.Delete(id);
                _farCommonService.FARUnit.AdditionalInfoofVehicleDetailRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (UpdateException ex)
            {
                try
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        // if (ex.InnerException.Message.Contains("REFERENCE constraint"))
                        // "The user has related information and cannot be deleted."
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
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }


            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });

        }
        private FAR_AdditionalInfoofVehicle CreateEntity(AdditionalInformationofVehicleViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            foreach (var c in model.AdditionalInfoVehicleDetailList)
            {
                var far_AdditionalInfoofVehicleDetail = new FAR_AdditionalInfoofVehicleDetail();

                far_AdditionalInfoofVehicleDetail.Id = c.Id;
                far_AdditionalInfoofVehicleDetail.SparePartInfoId = (int)c.SparePartId;
                far_AdditionalInfoofVehicleDetail.Quantity = c.Quantity;
                far_AdditionalInfoofVehicleDetail.IUser = User.Identity.Name;
                far_AdditionalInfoofVehicleDetail.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    far_AdditionalInfoofVehicleDetail.IUser = User.Identity.Name;
                    far_AdditionalInfoofVehicleDetail.IDate = DateTime.Now;
                    entity.FAR_AdditionalInfoofVehicleDetail.Add(far_AdditionalInfoofVehicleDetail);
                }
                else
                {
                    far_AdditionalInfoofVehicleDetail.AdditionalInfoofVehicleId = model.Id;
                    far_AdditionalInfoofVehicleDetail.EUser = User.Identity.Name;
                    far_AdditionalInfoofVehicleDetail.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {

                        _farCommonService.FARUnit.AdditionalInfoofVehicleDetailRepository.Add(far_AdditionalInfoofVehicleDetail);
                    }
                    else
                    {
                        _farCommonService.FARUnit.AdditionalInfoofVehicleDetailRepository.Update(far_AdditionalInfoofVehicleDetail);

                    }
                }
                _farCommonService.FARUnit.AdditionalInfoofVehicleDetailRepository.SaveChanges();
            }

            return entity;
        }

        public void populateDropdown(AdditionalInformationofVehicleViewModel model)
        {
            dynamic ddl;

            ddl = _farCommonService.FARUnit.SparePartInformationRepository.GetAll();
            model.SparePartList = Common.PopulateSparePartDDL(ddl);

            ddl = _farCommonService.FARUnit.FixedAssetRepository.GetAll();
            model.AssetCodeList = Common.PopulateAssetCodeDDL(ddl);
        }

    }
}