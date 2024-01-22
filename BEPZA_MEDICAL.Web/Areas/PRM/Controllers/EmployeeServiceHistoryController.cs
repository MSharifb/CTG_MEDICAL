using BEPZA_MEDICAL.Domain.PGM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Employee;
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

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class EmployeeServiceHistoryController : BaseController
    {
        #region Fields
        private readonly EmployeeSeperationService _Service;
        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Constructor
        public EmployeeServiceHistoryController(PRMCommonSevice prmCommonservice, EmployeeSeperationService service)
        {
            this._prmCommonservice = prmCommonservice;
            this._Service = service;
        }

        #endregion

        #region Actions

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, EmployeeServiceHistoryViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<EmployeeServiceHistoryViewModel> list = (from esh in _prmCommonservice.PRMUnit.EmployeeServiceHistoryRepository.GetAll()
                                                          join emp in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() on esh.EmployeeId equals emp.Id
                                                            select new EmployeeServiceHistoryViewModel()
                                                            {
                                                                Id= esh.Id,
                                                                Type = esh.Type,
                                                                EmployeeId = esh.EmployeeId,
                                                                OrderNo = esh.OrderNo,
                                                                OrderDate = esh.OrderDate,
                                                                EmpId = emp.EmpID,
                                                                EmployeeName = emp.FullName,
                                                                Designation = emp.PRM_Designation.Name
                                                            }).ToList();
            if (request.Searching)
            {
                if (viewModel.Type != "0")
                {
                    list = list.Where(d => d.Type == viewModel.Type).ToList();
                }
                if (viewModel.EmpId != null)
                {
                    list = list.Where(d => d.EmpId == viewModel.EmpId).ToList();
                }
                if (viewModel.EmployeeName != null)
                {
                    list = list.Where(d => d.EmployeeName.ToUpper().Contains(viewModel.EmployeeName.ToUpper())).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;
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
                     d.EmployeeId,
                     d.EmpId,
                     d.EmployeeName,
                     d.Designation,
                     d.Type,
                     d.OrderNo,
                     Convert.ToDateTime(d.OrderDate).ToString("dd-MM-yyyy"),
                     "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
                        
        }

        [HttpGet]
        public ActionResult Create()
        {
            EmployeeServiceHistoryViewModel model = new EmployeeServiceHistoryViewModel();
            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(EmployeeServiceHistoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.IUser = User.Identity.Name;
                model.IDate = Common.CurrentDateTime;
                var entity = model.ToEntity();


                _prmCommonservice.PRMUnit.EmployeeServiceHistoryRepository.Add(entity);
                _prmCommonservice.PRMUnit.EmployeeServiceHistoryRepository.SaveChanges();
                model.IsError = 0;
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }
            populateDropdown(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            EmployeeServiceHistoryViewModel model = new EmployeeServiceHistoryViewModel();
            var entity = _prmCommonservice.PRMUnit.EmployeeServiceHistoryRepository.GetByID(id);
            model = entity.ToModel();
            populateDropdown(model);
            setEmployeeInfo(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EmployeeServiceHistoryViewModel model)
        {

            if (ModelState.IsValid)
            {
                model.IUser = User.Identity.Name;
                model.IDate = Common.CurrentDateTime;

                var entity = model.ToEntity();

                entity.EUser = User.Identity.Name;
                entity.EDate = Common.CurrentDateTime;

                _prmCommonservice.PRMUnit.EmployeeServiceHistoryRepository.Update(entity);
                _prmCommonservice.PRMUnit.EmployeeServiceHistoryRepository.SaveChanges();
                model.IsError = 0;
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
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
                _prmCommonservice.PRMUnit.EmployeeServiceHistoryRepository.Delete(id);
                _prmCommonservice.PRMUnit.EmployeeServiceHistoryRepository.SaveChanges();
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
                errMsg = ex.Message;
            }
            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        #endregion

        #region Grid Dropdown List
        [NoCache]
        public ActionResult GetTypeView()
        {
            Dictionary<string, string> type = new Dictionary<string, string>();

            type.Add(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Confirmation.ToString(), "Confirmation");
            type.Add(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Increment.ToString(), "Increment"); 
            type.Add(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Promotion.ToString(), "Promotion"); 
            type.Add(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Demotion.ToString(), "Demotion"); 
            type.Add(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Transfer.ToString(), "Transfer"); 

            return PartialView("_Select", type);
        }
        #endregion

        #region private Methods
        private void populateDropdown(EmployeeServiceHistoryViewModel model)
        {
            model.FromDivisionList = Common.PopulateDllList(_prmCommonservice.PRMUnit.DivisionRepository.GetAll().ToList());
            model.ToDivisionList = Common.PopulateDllList(_prmCommonservice.PRMUnit.DivisionRepository.GetAll().ToList());
            model.FromDisciplineList = Common.PopulateDllList(_prmCommonservice.PRMUnit.DisciplineRepository.GetAll().ToList());
            model.ToDisciplineList = Common.PopulateDllList(_prmCommonservice.PRMUnit.DisciplineRepository.GetAll().ToList());
            model.ToZoneInfoList = Common.PopulateDdlZoneList(_prmCommonservice.PRMUnit.ZoneInfoRepository.GetAll().ToList());
            model.FromZoneInfoList = Common.PopulateDdlZoneList(_prmCommonservice.PRMUnit.ZoneInfoRepository.GetAll().ToList());
            model.FromEmploymentProcessList = Common.PopulateDllList(_prmCommonservice.PRMUnit.EmploymentProcessRepository.GetAll().OrderBy(x => x.Name).ToList());
            model.ToEmploymentProcessList = Common.PopulateDllList(_prmCommonservice.PRMUnit.EmploymentProcessRepository.GetAll().OrderBy(x => x.Name).ToList());
            model.FromEmploymentTypeList = Common.PopulateDllList(_prmCommonservice.PRMUnit.EmploymentTypeRepository.GetAll().ToList());
            model.ToEmploymentTypeList = Common.PopulateDllList(_prmCommonservice.PRMUnit.EmploymentTypeRepository.GetAll().ToList());
            model.FromSalaryScaleList = Common.PopulateSalaryScaleDDL(_prmCommonservice.PRMUnit.SalaryScaleRepository.GetAll().ToList());
            model.ToSalaryScaleList = Common.PopulateSalaryScaleDDL(_prmCommonservice.PRMUnit.SalaryScaleRepository.GetAll().ToList());
            model.FromStepList = Common.PopulateStepList(_prmCommonservice.PRMUnit.JobGradeStepRepository.GetAll().ToList());
            model.ToStepList = Common.PopulateStepList(_prmCommonservice.PRMUnit.JobGradeStepRepository.GetAll().ToList());
            model.FromGradeList = Common.PopulateStepList(_prmCommonservice.PRMUnit.PRM_GradeStepRepository.GetAll().ToList());
            model.ToGradeList = Common.PopulateStepList(_prmCommonservice.PRMUnit.PRM_GradeStepRepository.GetAll().ToList());
            model.FromDesignationList = Common.PopulateDllList(_prmCommonservice.PRMUnit.DesignationRepository.GetAll().ToList());
            model.ToDesignationList = Common.PopulateDllList(_prmCommonservice.PRMUnit.DesignationRepository.GetAll().ToList());


            List<SelectListItem> type = new List<SelectListItem>() {
                new SelectListItem(){
                    Text = BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Confirmation.ToString(),
                    Value = "Confirmation"
                },
                new SelectListItem(){
                    Text = BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Increment.ToString(),
                    Value = "Increment"
                },
                new SelectListItem(){
                    Text = BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Promotion.ToString(),
                    Value = "Promotion"
                },
                new SelectListItem(){
                    Text = BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Demotion.ToString(),
                    Value = "Demotion"
                },
                new SelectListItem(){
                    Text = BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Transfer.ToString(),
                    Value = "Transfer"
                },
            };
            model.TypeList = type;
        }

        #endregion

        #region Autocomplete Employee

        [NoCache]
        public JsonResult AutoCompleteEmployeeList(string term)
        {
            var result = (from r in _Service.PRMUnit.EmploymentInfoRepository.GetAll()
                          where r.EmpID.StartsWith(term) && r.DateofInactive == null
                          select new { r.EmpID, r.FullName }).Distinct().OrderBy(x => x.EmpID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult GetEmployeeInfoAutocomplete(string ICNO)
        {
            string msg = string.Empty;
            var obj = _Service.PRMUnit.EmploymentInfoRepository.Get(q => q.EmpID == ICNO).FirstOrDefault();
            if (obj != null && obj.DateofInactive != null)
            {
                msg = "Inactive Employee";
            }
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    if (obj != null)
                    {
                        return Json(new
                        {
                            EmployeeId = obj.Id,
                            EmpId = obj.EmpID,
                            EmployeeName = obj.FullName,
                            EmployeeDesignation = obj.PRM_Designation.Name
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Result = false });
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        return Json(new { Result = false });

                    }
                    else
                    {
                        return Json(new { Result = false });
                    }
                }
            }
            else
            {
                return Json(new
                {
                    Result = msg
                });
            }

        }


        private void setEmployeeInfo(EmployeeServiceHistoryViewModel model)
        {
            if (model.EmployeeId != 0)
            {
                var obj = _Service.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
                model.EmpId = obj.EmpID;
                model.Designation = obj.PRM_Designation.Name;
                model.EmployeeName = obj.FullName;
            }
        }

        #endregion
    }
}