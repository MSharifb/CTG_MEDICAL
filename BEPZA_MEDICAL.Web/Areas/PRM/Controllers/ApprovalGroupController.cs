using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using System.Collections;
using System.Data.SqlClient;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalGroup;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Controllers;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ApprovalGroupController : Controller
    {

        #region Fields
        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        public ApprovalGroupController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonservice = prmCommonService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            ApprovalGroupViewModel model = new ApprovalGroupViewModel();
            model.ActionType = "Save";
            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Save(ApprovalGroupViewModel model)
        {
            SaveUpdate(model);
            if (model.IsError == 1)
            {
                return View("CreateOrEdit", model);
            }
            return View("Index", model);
        }

        public ActionResult Edit(int id)
        {
            var model = new ApprovalGroupViewModel();
            try
            {
                var obj = _prmCommonservice.PRMUnit.ApprovalGroupRepository.GetByID(id);

                if (obj != null)
                {
                    model = obj.ToModel();
                    model.ActionType = "Update";

                }
            }
            catch (Exception exception)
            {
                model.errClass = "failed";
                model.IsSuccessful = false;
                if (exception.InnerException != null && exception.InnerException.Message.Contains("duplicate"))
                {
                    model.Message = ErrorMessages.UniqueIndex;
                }
                else
                {
                    model.Message = ErrorMessages.DataNotFound;
                }
            }
            return View("CreateOrEdit", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ApprovalGroupViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = (from data in _prmCommonservice.PRMUnit.ApprovalGroupRepository.GetAll()
                        select new ApprovalGroupViewModel()
                        {
                            Id = data.Id,
                            ApprovalGroupName = data.ApprovalGroupName,
                            SortOrder = data.SortOrder,
                            Remarks = data.Remarks
                        }).OrderBy(x => x.SortOrder).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.ApprovalGroupName))
                {
                    list = list.Where(x => x.ApprovalGroupName.Trim().ToLower().Contains(viewModel.ApprovalGroupName.Trim().ToLower())).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "ApprovalGroupName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ApprovalGroupName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ApprovalGroupName).ToList();
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
                    d.ApprovalGroupName,
                    d.SortOrder,
                    d.Remarks,
                    "Delete"
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [HttpPost]
        public ActionResult Update(ApprovalGroupViewModel model)
        {
            SaveUpdate(model);
            if (model.IsError == 1)
            {
                return View("CreateOrEdit", model);
            }
            return View("Index", model);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _prmCommonservice.PRMUnit.ApprovalGroupRepository.Delete(id);
                _prmCommonservice.PRMUnit.ApprovalGroupRepository.SaveChanges();
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
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        #region Methods

        private ApprovalGroupViewModel SaveUpdate(ApprovalGroupViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var errorMessage = CheckBusinessValidation(model);
                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        var obj = model.ToEntity();
                        switch (model.ActionType)
                        {
                            case "Save":
                                _prmCommonservice.PRMUnit.ApprovalGroupRepository.Add(obj);
                                break;
                            case "Update":
                                obj.EUser = User.Identity.Name;
                                obj.EDate = DateTime.Now;
                                _prmCommonservice.PRMUnit.ApprovalGroupRepository.Update(obj);
                                break;
                        }
                        _prmCommonservice.PRMUnit.ApprovalGroupRepository.SaveChanges();
                        model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        model.IsError = 0;
                        model.errClass = "success";
                    }
                    else
                    {
                        model.ErrMsg = errorMessage;
                        model.IsError = 1;
                        model.errClass = "failed";
                    }
                }
                else
                {
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                    model.IsError = 1;
                    model.errClass = "failed";
                }

            }
            catch (Exception exception)
            {
                model.errClass = "failed";
                model.IsSuccessful = false;
                model.IsError = 1;
                if (exception.InnerException != null && exception.InnerException.Message.Contains("duplicate"))
                {
                    model.ErrMsg = ErrorMessages.UniqueIndex;
                }
                else
                {
                    model.ErrMsg = ErrorMessages.InsertFailed;
                }
            }
            return model;
        }

        private string CheckBusinessValidation(ApprovalGroupViewModel model)
        {
            string message = string.Empty;
            try
            {
                var list = _prmCommonservice.PRMUnit.ApprovalGroupRepository.GetAll();
                switch (model.ActionType)
                {
                    case "Save":
                        list = list.Where(t => t.ApprovalGroupName == model.ApprovalGroupName).ToList();
                        break;
                    case "Update":
                        list = list.Where(t => t.ApprovalGroupName == model.ApprovalGroupName && t.Id != model.Id).ToList();
                        break;
                }

                if (list != null && list.Count() > 0)
                {
                    message += "Approval Group " + model.ApprovalGroupName + " already exists.";
                }
                return message;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}