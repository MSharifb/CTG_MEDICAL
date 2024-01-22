using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ApprovalFlowConfigurationController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Ctor
        public ApprovalFlowConfigurationController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonservice = prmCommonService;
        }
        #endregion

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ApprovalFlowConfigurationViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from APF in _prmCommonservice.PRMUnit.ApprovalFlowConfigurationRepository.GetAll()
                        select new ApprovalFlowConfigurationViewModel()
                        {
                            Id = APF.Id,
                            ApprovalProcesssId = APF.ApprovalProcesssId,
                            ApprovalProcessName = APF.APV_ApprovalProcess.ApprovalProcessName,
                            IsConfigurableApprovalFlow = APF.IsConfigurableApprovalFlow,
                            IsManualApprovalFlow = APF.IsManualApprovalFlow
                        }).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "ApprovalProcessName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ApprovalProcessName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ApprovalProcessName).ToList();
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
                    d.ApprovalProcesssId,
                    d.ApprovalProcessName,
                    d.IsConfigurableApprovalFlow,
                    d.IsManualApprovalFlow
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        public ActionResult GetApprovalProcesss()
        {
            var list = Common.PopulateDdlApprovalProcess(_prmCommonservice.PRMUnit.ApprovalProcessRepository.GetAll());
            return PartialView("Select", list);
        }


        // GET: PRM/ApprovalFlowConfiguration
        public ActionResult Index()
        {
            ApprovalFlowConfigurationViewModel model = new ApprovalFlowConfigurationViewModel();
            model.IsConfigurableApprovalFlow = false;
            model.IsManualApprovalFlow = true;
            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(ApprovalFlowConfigurationViewModel model)
        {
            try
            {
                string errorList = "";
                errorList = BusinessLogicValidation(model);
                if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
                {
                    if (model.Id == 0)
                    {
                        model.IUser = User.Identity.Name;
                        model.IDate = DateTime.Now;
                        var entity = model.ToEntity();
                        _prmCommonservice.PRMUnit.ApprovalFlowConfigurationRepository.Add(entity);
                        _prmCommonservice.PRMUnit.ApprovalFlowConfigurationRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    else
                    {
                        model.IUser = User.Identity.Name;
                        model.IDate = DateTime.Now;
                        model.EUser = User.Identity.Name;
                        model.EDate = DateTime.Now;
                        var entity = model.ToEntity();
                        _prmCommonservice.PRMUnit.ApprovalFlowConfigurationRepository.Update(entity);
                        _prmCommonservice.PRMUnit.ApprovalFlowConfigurationRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = Resources.ErrorMessages.InsertSuccessful;
                }
            }
            populateDropdown(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var entity = _prmCommonservice.PRMUnit.ApprovalFlowConfigurationRepository.GetByID(id);
            var model = entity.ToModel();
            model.strMode = "Edit";
            populateDropdown(model);
            return View("Index", model);
        }


        private void populateDropdown(ApprovalFlowConfigurationViewModel model)
        {
            var approvalProcessList = _prmCommonservice.PRMUnit.ApprovalProcessRepository.GetAll();
            model.ApprovalProcessList = Common.PopulateDdlApprovalProcess(approvalProcessList);
        }

        private string BusinessLogicValidation(ApprovalFlowConfigurationViewModel model)
        {
            string msg = string.Empty;
            var item = _prmCommonservice.PRMUnit.ApprovalFlowConfigurationRepository.GetAll().ToList();
            var chkList = new List<BEPZA_MEDICAL.DAL.PRM.APV_ApprovalFlowConfiguration>();
            if (model.Id == 0)
            {
                 chkList = item.Where(s => s.ApprovalProcesssId == model.ApprovalProcesssId).ToList();
            }
            else
            {
                 chkList = item.Where(s => s.Id != model.Id && s.ApprovalProcesssId == model.ApprovalProcesssId).ToList();
            }
            if (chkList.Count>0)
            {
                msg = "Duplicate Entry!";
                return msg;
            }
            return msg;
        }
    }
}