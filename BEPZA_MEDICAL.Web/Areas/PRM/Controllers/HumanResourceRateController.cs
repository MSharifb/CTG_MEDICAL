using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using System.Collections.ObjectModel;
using BEPZA_MEDICAL.Web.Utility;
using System.Collections;
using System.Data.SqlClient;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.HumanResourceRate;


namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class HumanResourceRateController : Controller
    {
        private readonly PRMCommonSevice _prmCommonservice;

        public HumanResourceRateController(PRMCommonSevice prmCommonService)
        {           
            this._prmCommonservice = prmCommonService;
        }

        [NoCache]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, HumanResourceRateSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            if (request.Searching)
            {
                if (viewModel != null)
                    filterExpression = viewModel.GetFilterExpression();
            }

            totalRecords = _prmCommonservice.PRMUnit.HumanResourceRateMasterRepository.GetCount(filterExpression);

            //Prepare JqGridData instance
            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            
            var list = _prmCommonservice.PRMUnit.HumanResourceRateMasterRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString());


            if (viewModel.EffectiveDate != null && !viewModel.EffectiveDate.Date.ToString().Contains("01/01/0001"))
            {
                list = list.Where(x => x.EffectiveDate == viewModel.EffectiveDate);
            }

            foreach (PRM_HumanResourceRateMaster d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.EffectiveDate.ToString("dd-MMM-yyyy"),           
                    d.Remarks,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Index()
        {
            var model = new HumanResourceRateViewModel();
            return View(model);
        }

        [NoCache]
        [HttpGet]
        public ActionResult Create()
        {
            HumanResourceRateViewModel model = new HumanResourceRateViewModel();
            //model.HumanResourceRateDetailList = new List<HumanResourceRateDetailViewModel>();
            //var objJobGradeList = _prmCommonservice.PRMUnit.JobGradeRepository.GetAll().OrderBy(x => x.GradeName).ToList();

            //if (objJobGradeList.Count > 0)
            //{
            //    foreach (var item in objJobGradeList)
            //    {
            //        HumanResourceRateDetailViewModel detailModel = new HumanResourceRateDetailViewModel();
            //        var objResourceLevel=_prmCommonservice.PRMUnit.ResourceLevelRepository.GetByID(item.ResourceLevelId);
            //        detailModel.Id = default(int);
            //        detailModel.HRRateMasterId = default(int);
            //        detailModel.JobGradeId = item.Id;
            //        detailModel.JobGrade = item.GradeName;
            //        detailModel.ResourceLevelId = item.ResourceLevelId;
            //        detailModel.ResourceLevel = objResourceLevel != null ? objResourceLevel.Name : "";
            //        detailModel.ActualRate = default(decimal);
            //        detailModel.BudgetRate = default(decimal);

            //        model.HumanResourceRateDetailList.Add(detailModel);
            //    }
            //}
            //else
            //{
            //    model.isSuccess = false;
            //    model.message = "There is no job grade for human resource rate.";
            //}
            //model.HumanResourceRateDetailList.OrderBy(x => x.JobGrade);
            //model.EffectiveDate = System.DateTime.Now;
            //model.ActionType = CrudeAction.Create;

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(HumanResourceRateViewModel model)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    message = BusinessValidation(model);
                    if (string.IsNullOrEmpty(message))
                    {
                        var masterEntity = model.ToEntity();
                        foreach (var childEntity in model.HumanResourceRateDetailList)
                        {
                            HumanResourceRateDetailViewModel detailModel = new HumanResourceRateDetailViewModel();
                            detailModel.Id = childEntity.Id;
                            detailModel.HRRateMasterId = childEntity.HRRateMasterId;
                            detailModel.JobGradeId = childEntity.JobGradeId;
                            detailModel.ActualRate = childEntity.ActualRate;
                            detailModel.BudgetRate = childEntity.BudgetRate;

                            masterEntity.PRM_HumanResourceRateDetail.Add(detailModel.ToEntity());
                        }
                        _prmCommonservice.PRMUnit.HumanResourceRateMasterRepository.Add(masterEntity);
                        _prmCommonservice.PRMUnit.HumanResourceRateMasterRepository.SaveChanges();

                        model.isSuccess = true;
                        model.message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        model.isSuccess = false;
                        model.message = message;
                    }
                }
                catch (Exception ex)
                {
                    model.isSuccess = false;
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.message = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        if (model.message=="Duplicate entry.")
                        { 
                        }
                    }
                    else
                    {
                        model.message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                    }
                }
            }

            model.ActionType = CrudeAction.Create;
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            HumanResourceRateViewModel model = _prmCommonservice.PRMUnit.HumanResourceRateMasterRepository.GetByID(id).ToModel();

            //model.HumanResourceRateDetailList = new List<HumanResourceRateDetailViewModel>();

            //var jobGradeList = _prmCommonservice.PRMUnit.JobGradeRepository.GetAll().ToList();
            //var humanResourceRateDetailList = _prmCommonservice.PRMUnit.HumanResourceRateDetailRepository.Get(x => x.HRRateMasterId == model.Id).ToList();

            //var newJobGradeList = (from c in jobGradeList
            //               where !humanResourceRateDetailList.Any(p => p.JobGradeId == c.Id)
            //               select c).ToList(); 

            //if (humanResourceRateDetailList.Count > 0)
            //{
            //    foreach (var item in humanResourceRateDetailList)
            //    {
            //        HumanResourceRateDetailViewModel detailModel = new HumanResourceRateDetailViewModel();
            //        var objJobGrade = _prmCommonservice.PRMUnit.JobGradeRepository.GetByID(item.JobGradeId);
            //        var objResourceLevel = _prmCommonservice.PRMUnit.ResourceLevelRepository.GetByID(objJobGrade != null ? objJobGrade.ResourceLevelId : 0);
            //        detailModel.Id = item.Id;
            //        detailModel.HRRateMasterId = item.HRRateMasterId;
            //        detailModel.JobGradeId = item.JobGradeId;
            //        detailModel.JobGrade = objJobGrade != null ? objJobGrade.GradeName : "";
            //        //detailModel.ResourceLevelId = objJobGrade.ResourceLevelId;
            //        detailModel.ResourceLevel = objResourceLevel != null ? objResourceLevel.Name : "";
            //        detailModel.ActualRate = Convert.ToDecimal(item.ActualRate);
            //        detailModel.BudgetRate =Convert.ToDecimal(item.BudgetRate);
            //        model.HumanResourceRateDetailList.Add(detailModel);
            //    }
            //}

            //if (newJobGradeList.Count > 0)
            //{
            //    foreach (var item in newJobGradeList)
            //    {
            //        HumanResourceRateDetailViewModel detailModel = new HumanResourceRateDetailViewModel();
            //        var objJobGrade = _prmCommonservice.PRMUnit.JobGradeRepository.GetByID(item.Id);
            //        var objResourceLevel = _prmCommonservice.PRMUnit.ResourceLevelRepository.GetByID(item.ResourceLevelId);
            //        detailModel.Id = default(int);
            //        detailModel.HRRateMasterId = model.Id;
            //        detailModel.JobGradeId = item.Id;
            //        detailModel.JobGrade = objJobGrade != null ? objJobGrade.GradeName : "";
            //        //detailModel.ResourceLevelId = item.ResourceLevelId;
            //        detailModel.ResourceLevel = objResourceLevel != null ? objResourceLevel.Name : "";
            //        detailModel.ActualRate = default(decimal);
            //        detailModel.BudgetRate = default(decimal);

            //        model.HumanResourceRateDetailList.Add(detailModel);
            //    }
            //}
            //model.HumanResourceRateDetailList=model.HumanResourceRateDetailList.OrderBy(x => x.JobGrade).ToList();
            //model.ActionType = CrudeAction.Edit;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(HumanResourceRateViewModel model)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    message = BusinessValidation(model);
                    if (string.IsNullOrEmpty(message))
                    {
                        var master = model.ToEntity();
                        ArrayList lstChilds = new ArrayList();

                        foreach (var childEntity in model.HumanResourceRateDetailList)
                        {
                            HumanResourceRateDetailViewModel detailModel = new HumanResourceRateDetailViewModel();
                            detailModel.Id = childEntity.Id;
                            detailModel.HRRateMasterId = childEntity.HRRateMasterId;
                            detailModel.JobGradeId = childEntity.JobGradeId;
                            detailModel.ResourceLevelId = childEntity.ResourceLevelId;
                            detailModel.ActualRate = childEntity.ActualRate;
                            detailModel.BudgetRate = childEntity.BudgetRate;

                            lstChilds.Add(detailModel.ToEntity());
                        }

                        Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                        NavigationList.Add(typeof(PRM_HumanResourceRateDetail), lstChilds);

                        _prmCommonservice.PRMUnit.HumanResourceRateMasterRepository.Update(master, NavigationList);
                        _prmCommonservice.PRMUnit.HumanResourceRateMasterRepository.SaveChanges();

                        model.isSuccess = true;
                        model.message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        model.isSuccess = false;
                        model.message = message;
                    }
                }
                catch (Exception ex)
                {
                    model.isSuccess = false;
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.message = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                    }
                    else
                    {
                        model.message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                    }
                }
            }
            else
            {
                model.isSuccess = false;
                model.message = Common.GetCommomMessage(CommonMessage.UpdateFailed);
            }

            model.ActionType = CrudeAction.Edit;
            return View(model);
        }

        [NoCache]
        [ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {

                List<Type> allTypes = new List<Type> { typeof(PRM_HumanResourceRateDetail) };
                _prmCommonservice.PRMUnit.HumanResourceRateMasterRepository.Delete(id, allTypes);
                _prmCommonservice.PRMUnit.HumanResourceRateMasterRepository.SaveChanges();
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

        private string BusinessValidation(HumanResourceRateViewModel model)
        {
            string msg = string.Empty;

            if (model.Id > 0)
            {
                var hmr = _prmCommonservice.PRMUnit.HumanResourceRateMasterRepository.Get(x => x.EffectiveDate == model.EffectiveDate && x.Id != model.Id).ToList();
                if (hmr.Count > 0)
                {
                    msg = "Effective date can not duplicate.";
                }
            }

            else
            {
                var hmr = _prmCommonservice.PRMUnit.HumanResourceRateMasterRepository.Get(x => x.EffectiveDate == model.EffectiveDate).ToList();
                if (hmr.Count > 0)
                {
                    msg = "Effective date can not duplicate.";
                }
            }

            return msg;
        }

    }
}
