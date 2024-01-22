using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.DAL.INV;
using System.Data.SqlClient;
using System.Collections;
using BEPZA_MEDICAL.Web.Controllers;

namespace BEPZA_MEDICAL.Web.Areas.INV.Controllers
{
    public class ItemInfoController : BaseController
    {
        #region Fields

        private readonly INVCommonService _invCommonService;

        #endregion

        #region Constructor

        public ItemInfoController(INVCommonService invCommonservice)
        {
            _invCommonService = invCommonservice;
        }

        #endregion

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetItemInfo(JqGridRequest request, ItemInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _invCommonService.INVUnit.ItemInfoRepository.GetAll().ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.ItemName))
                {
                    list = list.Where(x => x.ItemName.Trim().ToLower().Contains(viewModel.ItemName.Trim().ToLower())).ToList();
                }
                if(viewModel.CategoryId > 0)
                {
                    list = list.Where(x => x.CategoryId == viewModel.CategoryId).ToList();
                }
                if (viewModel.AssetGroupId > 0)
                {
                    list = list.Where(x => x.AssetGroupId == viewModel.AssetGroupId).ToList();
                }
                if (viewModel.TypeId > 0)
                {
                    list = list.Where(x => x.TypeId == viewModel.TypeId).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "ItemName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ItemName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ItemName).ToList();
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
                var Type = string.Empty;
                if (d.TypeId != null)
                {
                    Type = d.INV_ItemType.ItemTypeName;
                }
            
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.ItemName,
                    Type,
                    d.TypeId,
                    d.CategoryId,
                    d.AssetGroupId,
                    d.Code
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            ItemInfoViewModel model = new ItemInfoViewModel();
            model.ActionType = "Create";

            //Radiogroup default value
            model.AssetGroupId = 1;

            PopulateList(model);

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(ItemInfoViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "add");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    try
                    {
                        INV_ItemInfo entity = model.ToEntity();

                        _invCommonService.INVUnit.ItemInfoRepository.Add(entity);
                        _invCommonService.INVUnit.ItemInfoRepository.SaveChanges();

                        // model.IsError = 0;                  
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                        //return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        model.IsError = 1;
                        try
                        {
                            if (ex.InnerException != null && (ex.InnerException is SqlException || ex.InnerException is EntityCommandExecutionException))
                            {
                                SqlException sqlException = ex.InnerException as SqlException;
                                model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                            }
                        }
                        catch (Exception)
                        {
                            model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                        }
                    }
                }
                else
                {
                    model.ErrMsg = dupErrMessage;
                    model.IsError = 1;
                }
            }

            model.ActionType = "Create";
            PopulateList(model);
            return View("CreateOrEdit", model);
        }

        public ActionResult Edit(int id)
        {
            INV_ItemInfo entity = _invCommonService.INVUnit.ItemInfoRepository.GetByID(id);
            ItemInfoViewModel model = entity.ToModel();
            model.ActionType = "Edit";
            PopulateList(model);


            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(ItemInfoViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "edit");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    INV_ItemInfo entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    try
                    {
                        _invCommonService.INVUnit.ItemInfoRepository.Update(entity);
                        _invCommonService.INVUnit.ItemInfoRepository.SaveChanges();
                        //   model.IsError = 0;
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

                        // return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        model.IsError = 1;
                        try
                        {
                            if (ex.InnerException != null && (ex.InnerException is SqlException || ex.InnerException is EntityCommandExecutionException))
                            {
                                SqlException sqlException = ex.InnerException as SqlException;
                                model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                            }
                        }
                        catch (Exception)
                        {
                            model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                        }
                    }
                }
                else
                {
                    model.ErrMsg = dupErrMessage;
                    model.IsError = 1;
                }
            }

            model.ActionType = "Edit";
            PopulateList(model);
            return View("CreateOrEdit", model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            bool result = false;
            try
            {
                INV_ItemInfo iteminfo = _invCommonService.INVUnit.ItemInfoRepository.GetByID(id);
                _invCommonService.INVUnit.ItemInfoRepository.Delete(iteminfo);
                _invCommonService.INVUnit.ItemInfoRepository.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex.InnerException != null && (ex.InnerException is SqlException || ex.InnerException is EntityCommandExecutionException))
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                    }
                    result = false;
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        private string CheckDuplicate(ItemInfoViewModel model, string strMode)
        {
            string message = string.Empty;
            dynamic nameCheckItem = null;
            dynamic codeCheckItem = null;

            if (strMode == "add")
            {
                nameCheckItem = _invCommonService.INVUnit.ItemInfoRepository.Get(x => x.ItemName == model.ItemName).FirstOrDefault();

                codeCheckItem = _invCommonService.INVUnit.ItemInfoRepository.GetAll().Where(x => !string.IsNullOrWhiteSpace(model.Code) && x.Code == model.Code).FirstOrDefault();
            }
            else
            {
                nameCheckItem = _invCommonService.INVUnit.ItemInfoRepository.Get(x => x.ItemName == model.ItemName && x.Id != model.Id).FirstOrDefault();

                codeCheckItem = _invCommonService.INVUnit.ItemInfoRepository.GetAll().Where(x => !string.IsNullOrWhiteSpace(model.Code) && x.Code == model.Code && x.Id != model.Id).FirstOrDefault();
            }

            if (nameCheckItem != null)
            {
                message += "Item: " + model.ItemName + " already exists.";
            }


            if (codeCheckItem != null)
            {
                message += "This code is already assigned for Item: " + codeCheckItem.ItemName + ".";
            }

            return message;

        }

        #region Others

        private void PopulateList(ItemInfoViewModel model)
        {
            model.ItemTypeList = TypeList();
            model.CategoryList = CategoryList();
            model.ColorList = Common.PopulateDllList(_invCommonService.INVUnit.ColorRepository.GetAll().OrderBy(x => x.Name));
            model.ManufacturerList = Common.PopulateDllList(_invCommonService.INVUnit.ManufacturerRepository.GetAll().OrderBy(x => x.Name));
            model.ModelList = Common.PopulateDllList(_invCommonService.INVUnit.ModelRepository.GetAll().OrderBy(x => x.Name));
            model.UnitList = Common.PopulateDllList(_invCommonService.INVUnit.UnitRepository.GetAll().OrderBy(x => x.Name));
            model.AssetGroupList = AssetGroupList();
        }
        private IList<SelectListItem> CategoryList()
        {
            return Common.PopulateDllList(_invCommonService.INVUnit.CategoryRepository.GetAll().OrderBy(x => x.Name));
        }

        private IList<SelectListItem> TypeList()
        {
            return _invCommonService.INVUnit.ItemTypeRepository.GetAll().Where(x => x.ParentId != null && x.IsGroup == false).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ItemTypeName,
                    Value = y.Id.ToString()
                }).ToList();
        }

        private IList<SelectListItem> AssetGroupList()
        {
            IList<SelectListItem> AssetGroupList = new List<SelectListItem>();
            AssetGroupList.Add(new SelectListItem() { Text = "Fixed Asset", Value = "1" });
            AssetGroupList.Add(new SelectListItem() { Text = "Consumable", Value = "2" });
            AssetGroupList.Add(new SelectListItem() { Text = "Non-Consumable", Value = "3" });
            return AssetGroupList;
        }

        [NoCache]
        public ActionResult GetCategoryList()
        {
            return PartialView("Select", CategoryList());
        }

        [NoCache]
        public ActionResult GetAssetGroupList()
        {
            return PartialView("Select", AssetGroupList());
        }

        [NoCache]
        public ActionResult GetTypeList()
        {
            return PartialView("Select", TypeList());
        }

        #endregion
    }
}
