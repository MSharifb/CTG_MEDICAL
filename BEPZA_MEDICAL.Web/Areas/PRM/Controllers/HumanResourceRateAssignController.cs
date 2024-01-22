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
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.HumanResourceRateAssign;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class HumanResourceRateAssignController : Controller
    {
      private readonly PRMCommonSevice _prmCommonservice;

      public HumanResourceRateAssignController(PRMCommonSevice prmCommonService)
        {           
            this._prmCommonservice = prmCommonService;
        }

      [NoCache]
      [AcceptVerbs(HttpVerbs.Post)]
      public ActionResult GetList(JqGridRequest request, HumanResourceRateAssignSearchViewModel viewModel)
      {
          string filterExpression = String.Empty;
          int totalRecords = 0;

          if (request.Searching)
          {
              if (viewModel != null)
                  filterExpression = viewModel.GetFilterExpression();
          }

          totalRecords = _prmCommonservice.PRMUnit.HumanResourceRateAssignMasterRepository.GetCount(filterExpression);

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


          var list = _prmCommonservice.PRMUnit.HumanResourceRateAssignMasterRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString());


          if (viewModel.EffectiveDate != null && !viewModel.EffectiveDate.Date.ToString().Contains("01/01/0001"))
          {
              list = list.Where(x => x.EffectiveDate == viewModel.EffectiveDate);
          }

          foreach (PRM_HumanResourceRateAssignMaster d in list)
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
          var model = new HumanResourceRateAssignModel();
          return View(model);
      }

      [NoCache]
      [HttpGet]
      public ActionResult Create()
      {
          HumanResourceRateAssignModel model = new HumanResourceRateAssignModel();
          dynamic employmentType = _prmCommonservice.PRMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.Name).ToList();
          model.EmploymentTypeList = Common.PopulateDllList(employmentType);
          model.EffectiveDate = System.DateTime.Now;
          model.ActionType = CrudeAction.Create;

          return View(model);
      }

      [HttpPost]
      [NoCache]
      public ActionResult Create(HumanResourceRateAssignModel model)
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
                      _prmCommonservice.PRMUnit.HumanResourceRateAssignMasterRepository.Add(masterEntity);
                      _prmCommonservice.PRMUnit.HumanResourceRateAssignMasterRepository.SaveChanges();

                      foreach (var childEntity in model.HumanResourceRateAssignDetailList)
                      {
                          HumanResourceRateAssignDetailModel detailModel = new HumanResourceRateAssignDetailModel();
                          detailModel.Id = childEntity.Id;
                          detailModel.HRRateAssignMasterId = masterEntity.Id;
                          detailModel.EmployeeId = childEntity.EmployeeId;
                          detailModel.EmploymentTypeId = model.EmploymentTypeMasterId;
                          detailModel.ActualRate = childEntity.ActualRate;
                          detailModel.BudgetRate = childEntity.BudgetRate;

                          _prmCommonservice.PRMUnit.HumanResourceRateAssignDetailRepository.Add(detailModel.ToEntity());
                          _prmCommonservice.PRMUnit.HumanResourceRateAssignDetailRepository.SaveChanges();

                      }
                    
                      model.isSuccess = true;
                      model.message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                      return RedirectToAction("Edit", new { id = masterEntity.Id });
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
                      if (model.message == "Duplicate entry.")
                      {
                      }
                  }
                  else
                  {
                      model.message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                  }
              }
          }
          dynamic employmentType = _prmCommonservice.PRMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.Name).ToList();
          model.EmploymentTypeList = Common.PopulateDllList(employmentType);
          model.ActionType = CrudeAction.Create;

          return View(model);
      }

      [NoCache]
      public ActionResult Edit(int id)
      {
          HumanResourceRateAssignModel model = _prmCommonservice.PRMUnit.HumanResourceRateAssignMasterRepository.GetByID(id).ToModel();

          dynamic employmentType = _prmCommonservice.PRMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.Name).ToList();
          model.EmploymentTypeList = Common.PopulateDllList(employmentType);

          model.ActionType = CrudeAction.Edit;
          return View(model);
      }

      [HttpPost]
      [NoCache]
      public ActionResult Edit(HumanResourceRateAssignModel model)
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
                      _prmCommonservice.PRMUnit.HumanResourceRateAssignMasterRepository.Update(master);
                      _prmCommonservice.PRMUnit.HumanResourceRateAssignMasterRepository.SaveChanges();


                      foreach (var childEntity in model.HumanResourceRateAssignDetailList)
                      {
                          HumanResourceRateAssignDetailModel detailModel = new HumanResourceRateAssignDetailModel();
                          detailModel.Id = childEntity.Id;
                          detailModel.HRRateAssignMasterId = master.Id;
                          detailModel.EmployeeId = childEntity.EmployeeId;
                          detailModel.EmploymentTypeId = model.EmploymentTypeMasterId;
                          detailModel.ActualRate = childEntity.ActualRate;
                          detailModel.BudgetRate = childEntity.BudgetRate;

                          var entity = detailModel.ToEntity();
                          if (detailModel.Id > 0)
                          {
                              _prmCommonservice.PRMUnit.HumanResourceRateAssignDetailRepository.Update(entity);
                          }
                          else
                          {
                              _prmCommonservice.PRMUnit.HumanResourceRateAssignDetailRepository.Add(entity);
                          }
                          _prmCommonservice.PRMUnit.HumanResourceRateAssignMasterRepository.SaveChanges();

                      }

                      model.isSuccess = true;
                      model.message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                      //return RedirectToAction("Index");
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
          dynamic employmentType = _prmCommonservice.PRMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.Name).ToList();
          model.EmploymentTypeList = Common.PopulateDllList(employmentType);
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
              List<Type> allTypes = new List<Type> { typeof(PRM_HumanResourceRateAssignDetail) };
              _prmCommonservice.PRMUnit.HumanResourceRateAssignMasterRepository.Delete(id, allTypes);
              _prmCommonservice.PRMUnit.HumanResourceRateAssignMasterRepository.SaveChanges();
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

      public PartialViewResult GetDetailsTemplate(int Id, decimal factor, string actionType, string effectiveDate)
      {
          HumanResourceRateAssignModel master = new HumanResourceRateAssignModel();
          //var fctor = factor > 0 ? factor : 0;
          //var employeeList = (from tr in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll()
          //                    join rl in _prmCommonservice.PRMUnit.JobGradeRepository.GetAll() on tr.JobGradeId equals rl.Id
          //                    join ss in _prmCommonservice.PRMUnit.EmpSalaryRepository.GetAll() on tr.Id equals ss.EmployeeId
          //                    join res in _prmCommonservice.PRMUnit.ResourceLevelRepository.GetAll() on rl.ResourceLevelId equals res.Id
          //                    where tr.EmploymentTypeId == Id
          //                    select new
          //                    {
          //                        EmployeeId = tr.Id,
          //                        EmployeeInitial = tr.EmployeeInitial,
          //                        ResourceLevelId = rl.ResourceLevelId,
          //                        ResourceLevel = res.Name,
          //                        GrossSalary = ss.GrossSalary,
          //                        JobGradeId = tr.JobGradeId,
          //                        EmploymentTypeId = tr.EmploymentTypeId
          //                    }).ToList();

          //master.HumanResourceRateAssignDetailList = new List<HumanResourceRateAssignDetailModel>();
          //PRM_HumanResourceRateDetail objHRRDetail = new PRM_HumanResourceRateDetail();
          //var objHRRMaster = (from tr in _prmCommonservice.PRMUnit.HumanResourceRateMasterRepository.GetAll()
          //                        select tr).OrderBy(x => x.EffectiveDate).LastOrDefault();


          //if (actionType == CrudeAction.Create)
          //{
          //    if (employeeList.Count > 0)
          //    {
          //        foreach (var item in employeeList)
          //        {
          //            HumanResourceRateAssignDetailModel model = new HumanResourceRateAssignDetailModel();

          //            if (objHRRMaster != null)
          //            {
          //                objHRRDetail = (from tr in _prmCommonservice.PRMUnit.HumanResourceRateDetailRepository.GetAll()
          //                                    where tr.JobGradeId == item.JobGradeId && tr.HRRateMasterId == objHRRMaster.Id
          //                                    select tr).OrderBy(x => x.Id).LastOrDefault();
          //            }

          //            model.Id = default(int);
          //            model.HRRateAssignMasterId = default(int);
          //            model.EmployeeId = item.EmployeeId;
          //            model.EmployeeInitial = item.EmployeeInitial;
          //            //model.ResourceLevelId = item.ResourceLevelId;
          //            model.ResourceLevel = item.ResourceLevel;
          //            model.EmploymentTypeId = item.EmploymentTypeId;
          //            model.ActualRate = item.GrossSalary;
          //            model.BudgetRate = objHRRDetail != null ? Id == 1 ? Math.Round(Convert.ToDecimal(objHRRDetail.BudgetRate), 2) : Math.Round(model.ActualRate * factor, 2) : default(decimal);

          //            master.HumanResourceRateAssignDetailList.Add(model);
          //        }

          //        master.HumanResourceRateAssignDetailList = master.HumanResourceRateAssignDetailList.OrderBy(x => x.EmployeeInitial).ToList();
          //    }
          //}
          //else
          //{
          //    var objHRRAssignMaster = (from tr in _prmCommonservice.PRMUnit.HumanResourceRateAssignMasterRepository.GetAll()
          //                              where Convert.ToDateTime(tr.EffectiveDate) == Convert.ToDateTime(effectiveDate)
          //                              select tr).LastOrDefault();

          //    if (objHRRAssignMaster != null)
          //    {
          //        var objHRRAssignDetail = (from tr in _prmCommonservice.PRMUnit.HumanResourceRateAssignDetailRepository.GetAll()
          //                                  join EMP in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() on tr.EmployeeId equals EMP.Id
          //                                  where tr.HRRateAssignMasterId == objHRRAssignMaster.Id && tr.EmploymentTypeId == Id
          //                                  select new
          //                                  {
          //                                      Id = tr.Id,
          //                                      HRRateAssignMasterId = tr.HRRateAssignMasterId,
          //                                      EmployeeId = tr.EmployeeId,
          //                                      EmployeeInitial = EMP.EmployeeInitial,
          //                                      ResourceLevelId = EMP.ResourceLevelId,
          //                                      EmploymentTypeId = tr.EmploymentTypeId,
          //                                      ActualRate = tr.ActualRate,
          //                                      BudgetRate = tr.BudgetRate,
          //                                      JobGradeId = EMP.JobGradeId

          //                                  }).ToList();

          //        /// New Employee List
          //        var newHRRAssignDetailList = (from c in employeeList
          //                                      where !objHRRAssignDetail.Any(p => p.EmployeeId == c.EmployeeId)
          //                                      select c).ToList();

          //        if (objHRRAssignDetail.Count > 0)
          //        {
          //            foreach (var item in objHRRAssignDetail)
          //            {
          //                HumanResourceRateAssignDetailModel model = new HumanResourceRateAssignDetailModel();
          //                var objResourceLevel = (from tr in _prmCommonservice.PRMUnit.ResourceLevelRepository.GetAll()
          //                                        where tr.Id == item.ResourceLevelId
          //                                        select tr).LastOrDefault();
          //                model.Id = item.Id;
          //                model.HRRateAssignMasterId = item.HRRateAssignMasterId;
          //                model.EmployeeId = item.EmployeeId;
          //                model.EmployeeInitial = item.EmployeeInitial;
          //                model.ResourceLevelId = item.ResourceLevelId;
          //                model.ResourceLevel = objResourceLevel != null ? objResourceLevel.Name : "";
          //                model.EmploymentTypeId = item.EmploymentTypeId;
          //                model.ActualRate = Convert.ToDecimal(item.ActualRate);
          //                model.BudgetRate = Id == 1 ?Math.Round(Convert.ToDecimal(item.BudgetRate),2) : Math.Round(model.ActualRate * factor,2);

          //                master.HumanResourceRateAssignDetailList.Add(model);
          //            }
          //        }

          //        if (newHRRAssignDetailList.Count > 0)
          //        {
          //            foreach (var item in newHRRAssignDetailList)
          //            {
          //                HumanResourceRateAssignDetailModel model = new HumanResourceRateAssignDetailModel();
          //                if (objHRRMaster != null)
          //                {
          //                    objHRRDetail = (from tr in _prmCommonservice.PRMUnit.HumanResourceRateDetailRepository.GetAll()
          //                                        where tr.JobGradeId == item.JobGradeId && tr.HRRateMasterId == objHRRMaster.Id
          //                                        select tr).OrderBy(x => x.Id).LastOrDefault();
          //                }

          //                model.Id = default(int);
          //                model.HRRateAssignMasterId = objHRRAssignMaster.Id;
          //                model.EmployeeId = item.EmployeeId;
          //                model.EmployeeInitial = item.EmployeeInitial;
          //                model.ResourceLevelId = item.ResourceLevelId;
          //                model.ResourceLevel = item.ResourceLevel;
          //                model.EmploymentTypeId = item.EmploymentTypeId;
          //                model.ActualRate = item.GrossSalary;
          //                model.BudgetRate = objHRRDetail != null ? Id == 1 ?Math.Round(Convert.ToDecimal(objHRRDetail.BudgetRate),2) : Math.Round(model.ActualRate * factor,2) : default(decimal);

          //                master.HumanResourceRateAssignDetailList.Add(model);
          //            }
          //        }
          //    }

          //}

          return PartialView("_GradeDetailList", master);
      }

      private string BusinessValidation(HumanResourceRateAssignModel model)
      {
          string msg = string.Empty;

          if (model.Id > 0)
          {
              var hmr = _prmCommonservice.PRMUnit.HumanResourceRateAssignMasterRepository.Get(x => x.EffectiveDate == model.EffectiveDate  && x.Id != model.Id).ToList();
              if (hmr.Count > 0)
              {
                  msg = "Effective date can not duplicate.";
              }
          }

          else
          {
              var hmr = _prmCommonservice.PRMUnit.HumanResourceRateAssignMasterRepository.Get(x => x.EffectiveDate == model.EffectiveDate).ToList();
              if (hmr.Count > 0)
              {
                  msg = "Effective date can not duplicate.";
              }
          }

          return msg;
      }

    }
}
