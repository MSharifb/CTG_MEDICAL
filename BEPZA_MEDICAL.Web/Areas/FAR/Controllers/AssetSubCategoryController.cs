using BEPZA_MEDICAL.Domain.FAR;
using BEPZA_MEDICAL.Web.Areas.FAR.ViewModel;
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
    public class AssetSubCategoryController : Controller
    {
        #region Fields

        private readonly FARCommonService _farCommonService;
        #endregion

        #region Ctor
        public AssetSubCategoryController(FARCommonService farCommonService)
        {
            this._farCommonService = farCommonService;
        }
        #endregion

        #region Actions
        //
        // GET: FAR/AssetCondition
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, AssetSubCategoryViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from subCateg in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll()
                        select new AssetSubCategoryViewModel()

                                                      {
                                                          Id = subCateg.Id,
                                                          SubCategoryCode = subCateg.SubCategoryCode,
                                                          SubCategoryName = subCateg.SubCategoryName,
                                                          AssetCategoryCode = subCateg.FAR_Catagory.CategoryCode,
                                                          CategoryName = subCateg.FAR_Catagory.CategoryName,
                                                          DepreciationRate=subCateg.DepreciationRate
                                                      }).OrderBy(x => x.SubCategoryName).ToList();

            if (request.Searching)
            {

                if ((viewModel.SubCategoryCode != null && viewModel.SubCategoryCode != ""))
                {
                    list = list.Where(d => d.SubCategoryCode == viewModel.SubCategoryCode).ToList();
                }

                if ((viewModel.SubCategoryName != null && viewModel.SubCategoryName != ""))
                {
                    list = list.Where(d => d.SubCategoryName == viewModel.SubCategoryName).ToList();
                }
                if ((viewModel.AssetCategoryCode != null && viewModel.AssetCategoryCode != ""))
                {
                    list = list.Where(d => d.AssetCategoryCode == viewModel.AssetCategoryCode).ToList();
                }
                if ((viewModel.CategoryName != null && viewModel.CategoryName != ""))
                {
                    list = list.Where(d => d.CategoryName == viewModel.CategoryName).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "SubCategoryCode")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SubCategoryCode).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SubCategoryCode).ToList();
                }
            }

            if (request.SortingName == "SubCategoryName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SubCategoryName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SubCategoryName).ToList();
                }
            }

            if (request.SortingName == "AssetCategoryCode")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssetCategoryCode).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssetCategoryCode).ToList();
                }
            }

            if (request.SortingName == "CategoryName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.CategoryName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.CategoryName).ToList();
                }
            }
            if (request.SortingName == "DepreciationRate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DepreciationRate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DepreciationRate).ToList();
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
                    d.SubCategoryName,
                    d.SubCategoryCode,
                    d.CategoryName,
                    d.AssetCategoryCode,
                    d.DepreciationRate
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }


        public ActionResult Create()
        {
            AssetSubCategoryViewModel model = new AssetSubCategoryViewModel();
            populateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(AssetSubCategoryViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                if (ModelState.IsValid)
                {

                    model.IUser = User.Identity.Name;
                    model.IDate = DateTime.Now;
                    var entity = model.ToEntity();
                    _farCommonService.FARUnit.AssetSubCategoryRepository.Add(entity);
                    _farCommonService.FARUnit.AssetSubCategoryRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);


                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.errClass = "failed";
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                //if (ex.InnerException != null && ex.InnerException is SqlException)
                //{
                //    SqlException sqlException = ex.InnerException as SqlException;
                //    model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                //}
                //else
                //{
                //    model.ErrMsg = Resources.ErrorMessages.InsertSuccessful;
                //}
            }
            populateDropdown(model);
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            var entity = _farCommonService.FARUnit.AssetSubCategoryRepository.GetByID(id);
            var model = entity.ToModel();
            populateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(AssetSubCategoryViewModel model)
        {
            try
            {
                string errorList = string.Empty;

                if (ModelState.IsValid)
                {

                    var entity = model.ToEntity();
                    _farCommonService.FARUnit.AssetSubCategoryRepository.Update(entity);
                    _farCommonService.FARUnit.AssetSubCategoryRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;

                }

                if (errorList.Count() > 0)
                {
                    model.errClass = "failed";
                    model.ErrMsg = errorList;
                }

            }
            catch (Exception ex)
            {
                model.errClass = "failed";
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                //if (ex.InnerException != null && ex.InnerException is SqlException)
                //{
                //    SqlException sqlException = ex.InnerException as SqlException;
                //    model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                //}
                //else
                //{
                //    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                //}
            }
            populateDropdown(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _farCommonService.FARUnit.AssetSubCategoryRepository.Delete(id);
                _farCommonService.FARUnit.AssetSubCategoryRepository.SaveChanges();
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


        #endregion

        #region Private
        private void populateDropdown(AssetSubCategoryViewModel model)
        {
            dynamic ddlList;
            #region Depreciation Method  ddl
            ddlList = _farCommonService.FARUnit.AssetCategoryRepository.GetAll().OrderBy(q => q.CategoryName).ToList();
            model.AssetCategoryList = Common.PopulateAssetCategoryDDL(ddlList);
            #endregion
        }

        #endregion
    }
}