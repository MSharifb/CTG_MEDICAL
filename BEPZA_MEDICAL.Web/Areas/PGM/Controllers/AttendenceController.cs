using ERP_BEPZA.DAL.PGM;
using ERP_BEPZA.Domain.PGM;
using ERP_BEPZA.Domain.PRM;
using ERP_BEPZA.Web.Areas.PGM.Models.Attendance;
using ERP_BEPZA.Web.Areas.PRM.ViewModel;
using ERP_BEPZA.Web.Controllers;
using ERP_BEPZA.Web.Resources;
using ERP_BEPZA.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.PGM.Controllers
{
    public class AttendenceController : BaseController
    {
        //
        // GET: /PGM/Attendence/
        private readonly PGMCommonService _pgmCommonservice;
        private readonly PRMCommonSevice _prmCommonservice;
        private readonly ERP_BEPZAPGMEntities _pgmContext;
       
        public AttendenceController(PGMCommonService pgmCommonService, PRMCommonSevice prmCommonService, ERP_BEPZAPGMEntities pgmContext)
        {
            this._pgmCommonservice = pgmCommonService;
            this._prmCommonservice = prmCommonService;
            this._pgmContext = pgmContext;
        }

        #region Action

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, AttendanceViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from a in _pgmCommonservice.PGMUnit.AttendanceRepository.GetAll()
                        join emp in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() on a.EmployeeId equals emp.Id
                        where emp.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId
                        select new AttendanceViewModel
                        {
                            AttYear = a.AttYear,
                            AttMonth = a.AttMonth,
                        }).DistinctBy(d => new { d.AttMonth, d.AttYear }).ToList();
            if (request.Searching)
            {
                if (!String.IsNullOrEmpty(model.EmpID))
                {
                    list = list.Where(t => t.EmpID == model.EmpID).ToList();
                }

                if (!String.IsNullOrEmpty(model.AttYear))
                {
                    list = list.Where(t => t.AttYear == model.AttYear).ToList();
                }

                if (!String.IsNullOrEmpty(model.AttMonth))
                {
                    list = list.Where(t => t.AttMonth == model.AttMonth).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region sorting

            if (request.SortingName == "AttYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AttYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AttYear).ToList();
                }
            }

            if (request.SortingName == "AttMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AttMonth).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AttMonth).ToList();
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
                response.Records.Add(new JqGridRecord(d.AttYear + "-" + d.AttMonth, new List<object>()
                {
                    d.AttYear+"-"+ d.AttMonth,
                    d.AttYear,
                    d.AttMonth,                    
                    "Details"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult GetDetailList(JqGridRequest request, string year, string month, AttendanceViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<AttendanceViewModel> list = (from a in _pgmCommonservice.PGMUnit.AttendanceRepository.GetAll()
                                               join emp in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() on a.EmployeeId equals emp.Id
                                               join deg in _prmCommonservice.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals deg.Id
                                               //join empHis in _pgmCommonservice.PGMUnit.FunctionRepository.Get(null, null, DateTime.Now, LoggedUserZoneInfoId, null, null, null, null, null, null, null, null, null, false) on emp.Id equals empHis.EmployeeId
                                               where a.AttYear.ToString() == year && a.AttMonth.ToString() == month && emp.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId
                                               select new AttendanceViewModel
                                               {
                                                   Id = a.Id,
                                                   EmpID = emp.EmpID,
                                                   EmployeeId = emp.Id,
                                                   EmployeeName = emp.FullName,
                                                   Designation = deg.Name,
                                                   AttYear = a.AttYear,
                                                   AttMonth = a.AttMonth,
                                                   CalenderDays = Convert.ToInt32(a.CalenderDays),
                                                   AttFromDate = a.AttFromDate,
                                                   AttToDate = a.AttToDate,
                                                   TotalPresent = Convert.ToDecimal(a.TotalPresent),
                                                   TotalCasualLeave = Convert.ToDecimal(a.TotalCasualLeave),
                                                   TotalEarnedLeave = Convert.ToDecimal(a.TotalEarnedLeave),
                                                   TotalOthersLeave = Convert.ToDecimal(a.TotalOthersLeave),
                                                   TotalAttendance = Convert.ToDecimal(a.TotalAttendance),
                                                   Remarks = a.Remark
                                               }).OrderBy(x => x.Id).ToList();

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            if (request.Searching)
            {
                if (!String.IsNullOrEmpty(model.EmpID))
                {
                    list = list.Where(t => t.EmpID == model.EmpID).ToList();
                }

                if (!String.IsNullOrEmpty(model.EmployeeName))
                {
                    list = list.Where(t => t.EmployeeName == model.EmployeeName).ToList();
                }

                //if (model.OTMonth != 0)
                //{
                //    list = list.Where(t => t.OTMonth == model.OTMonth).ToList();
                //}
            }

            #region sorting

            if (request.SortingName == "EmpID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmpID).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmpID).ToList();
                }
            }

            if (request.SortingName == "EmployeeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmployeeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmployeeName).ToList();
                }
            }

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.EmpID ,
                    d.EmployeeId ,
                    d.EmployeeName ,
                    d.Designation ,
                    d.AttYear,
                    d.AttMonth,
                    d.CalenderDays,
                    d.TotalPresent,
                    d.TotalAttendance,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new AttendanceViewModel();
            populateDropdown(model);
            model.strMode = "Create";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(AttendanceViewModel model)
        {
            string errorList = string.Empty;
            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    if (string.IsNullOrEmpty(model.AccountNo))
                    {
                        model.AccountNo = string.Empty;
                    }

                    var entity = model.ToEntity();
                    _pgmCommonservice.PGMUnit.AttendanceRepository.Add(entity);
                    _pgmCommonservice.PGMUnit.AttendanceRepository.SaveChanges();
                    model.IsError = 0;
                    model.errClass = "success";
                    model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.Message = Common.GetSqlExceptionMessage(sqlException.Number);
                    }
                    else
                    {
                        model.errClass = "failed";
                        model.IsError = 1;
                        model.Message = Common.GetCommomMessage(CommonMessage.InsertFailed);
                    }
                }
            }
            else
            {
                model.IsError = 1;
                model.errClass = "failed";
                model.Message = errorList;
            }

            populateDropdown(model);
            model.strMode = "Create";

            return View(model);
        }

        public ActionResult Edit(int Id)
        {
            var model = _pgmCommonservice.PGMUnit.AttendanceRepository.GetByID(Id).ToViewModel();

            var emp = _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);

            var designation = _prmCommonservice.PRMUnit.DesignationRepository.GetByID(emp.DesignationId);

            model.EmpID = emp == null ? "" : emp.EmpID;
            model.EmployeeName = emp.FullName;
            model.DesignationId = model.DesignationId;
            model.Designation = designation == null ? "" : designation.Name;
            model.AccountNo = emp.BankAccountNo;
            populateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(AttendanceViewModel model)
        {
            string errorList = string.Empty;
            string Message = string.Empty;
            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    if (string.IsNullOrEmpty(model.AccountNo))
                    {
                        model.AccountNo = string.Empty;
                    }

                    model.EUser = User.Identity.Name;
                    model.EDate = DateTime.Now;
                    var entity = model.ToEntity();

                    _pgmCommonservice.PGMUnit.AttendanceRepository.Update(entity);
                    _pgmCommonservice.PGMUnit.AttendanceRepository.SaveChanges();

                    model.IsError = 0;
                    model.errClass = "success";
                    model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.Message = Common.GetSqlExceptionMessage(sqlException.Number);
                        model.IsError = 1;
                        model.errClass = "failed";
                    }
                    else
                    {
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.Message = Common.GetCommomMessage(CommonMessage.InsertFailed);
                    }
                }
            }
            else
            {
                model.IsError = 1;
                model.errClass = "failed";
                model.Message = errorList;
            }

            populateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _pgmCommonservice.PGMUnit.AttendanceRepository.Delete(id);
                _pgmCommonservice.PGMUnit.AttendanceRepository.SaveChanges();

                return Json(new
                {
                    Success = 1,
                    Message = ErrorMessages.DeleteSuccessful
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    Success = 0,
                    Message = ErrorMessages.DeleteFailed
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GoToDetails(string idYearMonth)
        {
            var model = new AttendanceViewModel();

            string[] yearMonth = idYearMonth.Split('-');
            model.AttYear = yearMonth[0];
            model.AttMonth = yearMonth[1];
            return View("DetailsList", model);
        }

        #endregion

        #region Other

        private string GetBusinessLogicValidation(AttendanceViewModel model)
        {
            if (model.strMode == "Create")
            {
                var r = _pgmCommonservice.PGMUnit.AttendanceRepository.Get
                (t => t.EmployeeId == model.EmployeeId
                && t.AttYear == model.AttYear
                && t.AttMonth == model.AttMonth).ToList();

                if (r != null && r.Count() != 0)
                {
                    return "Attendance already exist for this month of this employee";
                }
            }
            else if (model.strMode == "Edit")
            {
                var r = _pgmCommonservice.PGMUnit.AttendanceRepository.Get
                (e => e.EmployeeId == model.EmployeeId
                && e.AttYear == model.AttYear
                && e.AttMonth == model.AttMonth && e.Id != model.Id).ToList();

                if (r != null && r.Count() != 0)
                {
                    return "Attendance already exist for this month of this employee";
                }
            }

            return string.Empty;
        }

        [NoCache]
        public ActionResult GetYear()
        {
            return PartialView("Select", pGetYear());
        }

        [NoCache]
        public ActionResult GetMonth()
        {
            return PartialView("Select", pGetMonth());
        }

        private void populateDropdown(AttendanceViewModel model)
        {
            model.YearList = pGetYear();
            model.MonthList = pGetMonth();
        }

        private IList<SelectListItem> pGetYear()
        {
            return Common.PopulateYearList();
        }

        private IList<SelectListItem> pGetMonth()
        {
            return Common.PopulateMonthList();
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(ResourceInfoViewModel model)
        {
            string errorList = string.Empty;
            //errorList = GetValidationChecking(model);

            var bs = (from S in _pgmCommonservice.PGMUnit.SalaryMasterRepository.GetAll()
                      join SD in _pgmCommonservice.PGMUnit.SalaryDetailsRepository.GetAll() on S.Id equals SD.SalaryId
                      join sh in _prmCommonservice.PRMUnit.SalaryHeadRepository.Fetch() on SD.HeadId equals sh.Id
                      where S.EmployeeId == model.EmployeeId && sh.IsBasicHead == true
                      select new
                      {
                          Basic = SD.HeadAmount
                      }).ToList();

            if (string.IsNullOrEmpty(errorList))
            {
                try
                {
                    var obj = _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);

                    if (obj != null)
                    {
                        return Json(new
                        {
                            EmpID = obj.EmpID,
                            EmployeeName = obj.FullName,
                            DesignationId = obj.PRM_Designation == null ? 0 : obj.PRM_Designation.Id,
                            Designation = obj.PRM_Designation == null ? "" : obj.PRM_Designation.Name,
                            Department = obj.PRM_Division == null ? "" : obj.PRM_Division.Name,
                            AccountNo = obj.BankAccountNo,
                            BasicSalary = (bs == null) ? 0M : bs.Select(x => x.Basic).FirstOrDefault(),
                        });
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
                        model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                        return Json(new { Result = false });
                    }
                    else
                    {
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                        return Json(new { Result = false });
                    }
                }
            }
            else
            {
                return Json(new
                {
                    Result = errorList
                });
            }
        }

        #endregion
	}
}