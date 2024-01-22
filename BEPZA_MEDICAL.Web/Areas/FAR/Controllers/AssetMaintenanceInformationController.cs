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
    public class AssetMaintenanceInformationController : BaseController
    {
        #region Fields
        private readonly FARCommonService _farCommonService;
        #endregion

        #region Ctor
        public AssetMaintenanceInformationController(FARCommonService farCommonService)
        {
            this._farCommonService = farCommonService;
        }
        #endregion

        // GET: FAR/AssetMaintenanceInformation
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, AssetMaintenanceInformationViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from loc in _farCommonService.FARUnit.AssetMaintenanceInformationRepository.GetAll()
                        join FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll() on loc.FIxedAssetId equals FA.Id
                        join MT in _farCommonService.FARUnit.MaintenanceTypeInformationRepository.GetAll() on loc.MaintenanceTypeInfoId equals MT.Id
                        where (viewModel.MaintenanceTypeInfoId==0 || viewModel.MaintenanceTypeInfoId==loc.MaintenanceTypeInfoId)
                        &&(viewModel.FIxedAssetId==0 || viewModel.FIxedAssetId==loc.FIxedAssetId)
                        select new AssetMaintenanceInformationViewModel()
                        {
                            Id = loc.Id,
                            FIxedAssetId=FA.Id,
                            AssetCode=FA.AssetCode,
                            MaintenanceTypeInfoId=MT.Id,
                            MaintenanceType = MT.MaintenanceType,
                            Amount=loc.Amount,
                            Date=loc.Date
                        }).ToList();

            if (request.Searching)
            {
                if ((viewModel.FromDate != null && viewModel.FromDate != DateTime.MinValue) && (viewModel.ToDate != null && viewModel.ToDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.Date >= viewModel.FromDate && d.Date <= viewModel.ToDate).ToList();
                }
                else if ((viewModel.FromDate != null && viewModel.FromDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.Date >= viewModel.FromDate).ToList();
                }
                else if ((viewModel.ToDate != null && viewModel.ToDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.Date <= viewModel.ToDate).ToList();
                }
            }

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

            if (request.SortingName == "MaintenanceType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.MaintenanceType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.MaintenanceType).ToList();
                }
            }
            if (request.SortingName == "Amount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Amount).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Amount).ToList();
                }
            }
            if (request.SortingName == "Date")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Date).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Date).ToList();
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
                    d.AssetCode,
                    d.MaintenanceTypeInfoId,
                    d.MaintenanceType,
                    d.Amount,
                    (Convert.ToDateTime(d.Date)).ToString(DateAndTime.GlobalDateFormat),
                    d.FromDate,
                    d.ToDate
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
        [NoCache]
        public ActionResult MaintenanceTypeforView()
        {
            var itemList = Common.PopulateMaintenanceTypeDDL(_farCommonService.FARUnit.MaintenanceTypeInformationRepository.GetAll().ToList());
            return PartialView("Select", itemList);
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            AssetMaintenanceInformationViewModel model = new AssetMaintenanceInformationViewModel();
            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(AssetMaintenanceInformationViewModel model)
        {

            try
            {
                string errorList = string.Empty;
                model.IsError = 1;

                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, true);
                    HttpFileCollectionBase files = Request.Files;
                    entity = ToAttachFile(entity, files);
                    model.Attachment = entity.Attachment;
                    model.FileName = entity.FileName;

                    _farCommonService.FARUnit.AssetMaintenanceInformationRepository.Add(entity);
                    _farCommonService.FARUnit.AssetMaintenanceInformationRepository.SaveChanges();
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
            populateDropdown(model);
            return View(model);
        }

        public ActionResult Edit(int id, string type)
        {
            var SelectionCriteriaEntity = _farCommonService.FARUnit.AssetMaintenanceInformationRepository.GetByID(id);
            var parentModel = SelectionCriteriaEntity.ToModel();
            parentModel.strMode = "Edit";
            parentModel.IsInEditMode = true;

            List<AssetMaintenanceInformationDetailViewModel> list = (from sel in _farCommonService.FARUnit.AssetMaintenanceInformationRepository.GetAll()
                                                                     join selDtl in _farCommonService.FARUnit.AssetMaintenanceInformationDetailRepository.GetAll() on sel.Id equals selDtl.AssetMaintenanceInfoId
                                                                     join SP in _farCommonService.FARUnit.SparePartInformationRepository.GetAll() on selDtl.SparePartInfoId equals SP.Id
                                                                     where (selDtl.AssetMaintenanceInfoId == id)
                                                                     select new AssetMaintenanceInformationDetailViewModel()
                                                                     {
                                                                      Id = selDtl.Id,
                                                                      SparePartId = selDtl.SparePartInfoId,
                                                                      Quantity = selDtl.Quantity,
                                                                      Rate = selDtl.Rate,
                                                                      SparePartName=SP.SparePart
                                                                     }).ToList();

            parentModel.AssetMaintenanceInformationDetailList = list;
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
        public ActionResult Edit(AssetMaintenanceInformationViewModel model)
        {
            try
            {
                string errorList = "";
                if (ModelState.IsValid)
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, false);

                    HttpFileCollectionBase files = Request.Files;
                    entity = ToAttachFile(entity, files);

                    if (files == null)
                    {
                        var existingObj = _farCommonService.FARUnit.AssetMaintenanceInformationRepository.GetByID(entity.Id);
                        entity.Attachment = existingObj.Attachment;
                    }

                    if (errorList.Length == 0)
                    {
                        _farCommonService.FARUnit.AssetMaintenanceInformationRepository.Update(entity);
                        _farCommonService.FARUnit.AssetMaintenanceInformationRepository.SaveChanges();
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

            var tempPeriod = _farCommonService.FARUnit.AssetMaintenanceInformationRepository.GetByID(id);
            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(FAR_AssetMaintenanceInformationDetail) };
                    _farCommonService.FARUnit.AssetMaintenanceInformationRepository.Delete(tempPeriod.Id, allTypes);
                    _farCommonService.FARUnit.AssetMaintenanceInformationRepository.SaveChanges();
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
                _farCommonService.FARUnit.AssetMaintenanceInformationDetailRepository.Delete(id);
                _farCommonService.FARUnit.AssetMaintenanceInformationDetailRepository.SaveChanges();
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
        private FAR_AssetMaintenanceInformation CreateEntity(AssetMaintenanceInformationViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            foreach (var c in model.AssetMaintenanceInformationDetailList)
            {
                var far_AssetMaintenanceInformationDetail = new FAR_AssetMaintenanceInformationDetail();

                far_AssetMaintenanceInformationDetail.Id = c.Id;
                far_AssetMaintenanceInformationDetail.SparePartInfoId = (int)c.SparePartId;
                far_AssetMaintenanceInformationDetail.Quantity =c.Quantity;
                far_AssetMaintenanceInformationDetail.Rate = (decimal)c.Rate;
                far_AssetMaintenanceInformationDetail.IUser = User.Identity.Name;
                far_AssetMaintenanceInformationDetail.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    far_AssetMaintenanceInformationDetail.IUser = User.Identity.Name;
                    far_AssetMaintenanceInformationDetail.IDate = DateTime.Now;
                    entity.FAR_AssetMaintenanceInformationDetail.Add(far_AssetMaintenanceInformationDetail);
                }
                else
                {
                    far_AssetMaintenanceInformationDetail.AssetMaintenanceInfoId = model.Id;
                    far_AssetMaintenanceInformationDetail.EUser = User.Identity.Name;
                    far_AssetMaintenanceInformationDetail.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {

                        _farCommonService.FARUnit.AssetMaintenanceInformationDetailRepository.Add(far_AssetMaintenanceInformationDetail);
                    }
                    else
                    {
                        _farCommonService.FARUnit.AssetMaintenanceInformationDetailRepository.Update(far_AssetMaintenanceInformationDetail);

                    }
                }
                _farCommonService.FARUnit.AssetMaintenanceInformationDetailRepository.SaveChanges();
            }

            return entity;
        }

        public void populateDropdown(AssetMaintenanceInformationViewModel model)
        {
            dynamic ddl;

            ddl = _farCommonService.FARUnit.SparePartInformationRepository.GetAll();
            model.SparePartList = Common.PopulateSparePartDDL(ddl);

            ddl = _farCommonService.FARUnit.MaintenanceTypeInformationRepository.GetAll();
            model.MaintenanceTypeList = Common.PopulateMaintenanceTypeDDL(ddl);          
            ddl = _farCommonService.FARUnit.FixedAssetRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).Select(q => new { Value = q.Id, Text = q.AssetCode + " [" + q.AssetName + " ]" }).OrderBy(x => x.Text).ToList(); ;
            model.AssetCodeList = Common.PopulateDDL(ddl);
        }

        public ActionResult DownloadFile(int id)
        {
            try
            {
                var adjustmentInfo = _farCommonService.FARUnit.AssetMaintenanceInformationRepository.GetByID(id);
                string fileName = string.Empty;
                byte[] fileData = null;
                if (adjustmentInfo != null)
                {
                    if (!string.IsNullOrEmpty(adjustmentInfo.FileName))
                    {
                        fileName = adjustmentInfo.FileName;
                        fileData = adjustmentInfo.Attachment;
                        string filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
                        string contentType = MimeMapping.GetMimeMapping(filePath);
                        var cd = new System.Net.Mime.ContentDisposition
                        {
                            FileName = fileName,
                            Inline = true,
                        };

                        Response.AppendHeader("Content-Disposition", cd.ToString());

                        return File(fileData, contentType);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private FAR_AssetMaintenanceInformation ToAttachFile(FAR_AssetMaintenanceInformation masterModel, HttpFileCollectionBase files)
        {
            foreach (string fileTagName in files)
            {
                HttpPostedFileBase file = Request.Files[fileTagName];
                if (file.ContentLength > 0)
                {
                    // Due to the limit of the max for a int type, the largest file can be
                    // uploaded is 2147483647, which is very large anyway.

                    int size = file.ContentLength;
                    string name = file.FileName;
                    int position = name.LastIndexOf("\\");
                    name = name.Substring(position + 1);
                    string contentType = file.ContentType;
                    byte[] fileData = new byte[size];
                    file.InputStream.Read(fileData, 0, size);
                    masterModel.Attachment = fileData;
                    masterModel.FileName = file.FileName;
                }
            }

            return masterModel;
        }


    }
}