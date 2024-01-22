using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MFS_IWM.DAL.FAM;
using MFS_IWM.Domain.FAM;
using Lib.Web.Mvc.JQuery.JqGrid;
using MFS_IWM.Web.Areas.FAM.Models.VendorInfo;
using MFS_IWM.Web.Utility;
using System.Data.SqlClient;
using MFS_IWM.Web.Resources;

namespace MFS_IWM.Web.Areas.FAM.Controllers
{ 
    public class VendorInfoController : Controller
    {
        #region Fields
        private readonly FAMCommonService _famCommonService;
        #endregion


        #region Ctor
        public VendorInfoController(FAMCommonService famCommonService)
        {
            _famCommonService = famCommonService;
        }
        #endregion


        #region Actions
        
        public ViewResult Index()
        {
            return View();
        }

        public ActionResult BackToList()
        {
            var model = new VendorInformationModel();
            PrepareModel(model);
            return View("_Index", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, VendorInformationSearchModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            if (request.Searching)
            {
                if (model != null)
                    filterExpression = model.GetFilterExpression();
            }

            totalRecords = _famCommonService.FAMUnit.VendorInformationRepository.GetCount(filterExpression);

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            var list = _famCommonService.FAMUnit.VendorInformationRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1);

            foreach (FAM_VendorInformation d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.VendorName,
                    d.ContactPerson,
                    d.ContactNumber,
                    d.EMail,
                    d.Address,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new VendorInformationModel();
            PrepareModel(model);
            return View("_CreateOrEdit", model);
        }

        //[HttpPost]
        //public ActionResult Create(VendorInformationModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var obj = model.ToEntity();

        //        _famCommonService.FAMUnit.VendorInformationRepository.Add(obj);
        //        _famCommonService.FAMUnit.VendorInformationRepository.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(model);
        //}

        [HttpPost]
        public ActionResult Create(VendorInformationModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();
                    _famCommonService.FAMUnit.VendorInformationRepository.Add(entity);
                    _famCommonService.FAMUnit.VendorInformationRepository.SaveChanges();
                }
                catch (Exception)
                {

                    return new JsonResult()
                    {
                        Data = ErrorMessages.InsertFailed
                    };
                }

                return new JsonResult()
                {
                    Data = ErrorMessages.InsertSuccessful
                };
            }

            var errors = ModelState
                           .Where(x => x.Value.Errors.Count > 0)
                           .Select(x => new { x.Key, x.Value.Errors })
                           .ToArray(); 

            return new JsonResult()
            {
                Data = errors.Count()>0? errors.First().Errors.First().ErrorMessage : ""
            };
        }

        public ActionResult Edit(int id)
        {
            //FAM_VendorInformation obj = _famCommonService.FAMUnit.VendorInformationRepository.GetByID(id);
            //VendorInformationModel model = obj.ToModel();
            //return View(model);

            var entity = _famCommonService.FAMUnit.VendorInformationRepository.GetByID(id);
            var model = entity.ToModel();
            PrepareModel(model);
            model.Mode = "Edit";
            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(VendorInformationModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();
                    _famCommonService.FAMUnit.VendorInformationRepository.Update(entity);
                    _famCommonService.FAMUnit.VendorInformationRepository.SaveChanges();
                    //_famCommonservice.FAMUnit.BankMaster.Update(entity);
                    //_famCommonservice.FAMUnit.BankMaster.SaveChanges();
                }
                catch (Exception)
                {

                    return new JsonResult()
                    {
                        Data = ErrorMessages.UpdateFailed
                    };
                }

                return new JsonResult()
                {
                    Data = ErrorMessages.UpdateSuccessful
                };
            }
            return new JsonResult()
            {
                Data = ErrorMessages.UpdateFailed
            };
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = "Error while deleting data!";

            try
            {
                List<Type> allTypes = new List<Type> { typeof(FAM_VendorInformation) };
                
                _famCommonService.FAMUnit.VendorInformationRepository.Delete(id);
                _famCommonService.FAMUnit.VendorInformationRepository.SaveChanges();
                result = true;
            }
            catch (UpdateException ex)
            {
                try
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = Common.GetSqlExceptionMessage(sqlException.Number);
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
            }

            return Json(new
            {
                Success = result,
                Message = result ? "Information has been deleted successfully." : errMsg
            });
        }
        
        #endregion


        #region Utils
        private void PrepareModel(VendorInformationModel model)
        {

        }

        #endregion
    }
}