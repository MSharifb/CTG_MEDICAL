using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Collections.ObjectModel;
using MFS_IWM.Web.Utility;
using System.Collections;
using System.Data.SqlClient;
using Lib.Web.Mvc.JQuery.JqGrid;
using MFS_IWM.Domain.PGM;
using MFS_IWM.DAL.PGM;
using MFS_IWM.DAL.PRM;
using MFS_IWM.Domain.PRM;
using MFS_IWM.Web.Resources;
using MFS_IWM.Web.Areas.PGM.Models.OtherAdjustDeduct;
using MFS_IWM.Web.Areas.PRM.ViewModel;
using MFS_IWM.Web.Areas.PRM.ViewModel.Employee;

namespace MFS_IWM.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class OtherAdjustDeductController : Controller
    {
        #region Fields

        private static string type;
        private static string sYear;
        private static string sMonth;
        private static int sEmpID; 
        private readonly PGMCommonService _pgmCommonservice;
        private readonly PRMCommonSevice _prmCommonservice;
        private readonly ResourceInfoService _RresourceInfoService;

        #endregion

        #region Constructor
  
        public OtherAdjustDeductController(PGMCommonService pgmCommonservice, PRMCommonSevice prmCommonservice, ResourceInfoService service)
        {
            this._pgmCommonservice = pgmCommonservice;
            this._prmCommonservice = prmCommonservice;
            this._RresourceInfoService = service;
        }

        #endregion

        #region Actions

        [NoCache]
        public ViewResult Index(string Id)
        {
            var model = new OtherAdjustDeductModel();
            type = Id;
            model.Type = Id;
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, OtherAdjustDeductModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<OtherAdjustDeductModel> list = (from tr in _pgmCommonservice.PGMUnit.OtherAdjustDeduction.GetAll()
                                          join emp in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() on tr.EmployeeId equals emp.Id
                                          join desig in _prmCommonservice.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals desig.Id
                                          join Sal in _prmCommonservice.PRMUnit.SalaryHeadRepository.GetAll() on tr.SalaryHeadId equals Sal.Id
                                          where (string.IsNullOrEmpty(model.SalaryMonth) || model.SalaryMonth == tr.SalaryMonth)
                                          && (string.IsNullOrEmpty(model.SalaryYear) || model.SalaryYear == tr.SalaryYear)
                                          && (string.IsNullOrEmpty(model.EmpID) || model.EmpID == emp.EmpID)
                                          && (string.IsNullOrEmpty(model.EmployeeInitial) || model.EmployeeInitial == emp.EmployeeInitial)
                                          && (string.IsNullOrEmpty(model.EmployeeName) || model.EmployeeName == emp.FullName)
                                          && (string.IsNullOrEmpty(model.EmployeeDesignation) || model.EmployeeDesignation == desig.Id.ToString ())
                                          && (tr.Type == type)
                                          select new OtherAdjustDeductModel()
                                       {
                                           Id = tr.Id,
                                           EmployeeId = tr.EmployeeId,
                                           SalaryMonth = tr.SalaryMonth,
                                           SalaryYear = tr.SalaryYear,
                                           Type = tr.Type,
                                           SalaryHeadId = tr.SalaryHeadId,
                                           SalaryHead = Sal.HeadName,
                                           EmpID = emp.EmpID,
                                           EmployeeInitial=emp.EmployeeInitial,
                                           EmployeeName = emp.FullName,
                                           EmployeeDesignation = desig.Name,
                                           Amount = tr.Amount
                                       }).OrderBy(x=>Convert.ToDateTime(x.SalaryYear+"-"+x.SalaryMonth+"-01")).ToList();

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.SalaryYear,
                    d.SalaryMonth,
                    d.EmpID,
                    d.EmployeeInitial,
                    d.EmployeeName,
                    d.EmployeeDesignation,   
                    d.SalaryHead,   
                    d.Amount,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public PartialViewResult AddDetail()
        {
            return PartialView("_Detail");
        }

        [NoCache]
        public ActionResult Create()
        {
            OtherAdjustDeductModel model = new OtherAdjustDeductModel();

            model.Type = type;
            PrepareModel(model);
           
           return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(OtherAdjustDeductModel model)
        {
            string errorList = string.Empty;
            type = model.Type;
            var entity = model.ToEntity();
            entity = GetInsertUserAuditInfo(entity);
            errorList = GetBusinessLogicValidation(entity);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    _pgmCommonservice.PGMUnit.OtherAdjustDeduction.Add(entity);
                    _pgmCommonservice.PGMUnit.OtherAdjustDeduction.SaveChanges();
                    return RedirectToAction("Index/" + type.ToString());
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    }
                    else
                    {
                        model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                    }
                }
            }
            else
            {
                model.IsError = 1;
                model.ErrMsg = errorList;
            }
            PrepareModel(model);

            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            var entity = _pgmCommonservice.PGMUnit.OtherAdjustDeduction.GetByID(id, "Id");

            PGM_OtherAdjustDeduct pgm_taxrate = _pgmCommonservice.PGMUnit.OtherAdjustDeduction.GetByID(id);

            sYear = entity.SalaryYear;
            sMonth = entity.SalaryMonth;
            sEmpID = entity.EmployeeId;

            OtherAdjustDeductModel model = pgm_taxrate.ToModel();

            PrepareModel(model);
            model.Mode = "Edit";
            model.Type = type;

            if (model.EmployeeId != 0)
            {
                var obj = _RresourceInfoService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);

                model.EmpID = obj.EmpID;
                model.EmployeeInitial = obj.EmployeeInitial;
                model.EmployeeName = obj.FullName;
                model.EmployeeDesignation = obj.PRM_Designation.Name;
            }

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(OtherAdjustDeductModel model)
        {
            type = model.Type;
            string errorList =string.Empty;
            PGM_OtherAdjustDeduct entity = model.ToEntity();
            ArrayList lstDetail = new ArrayList();

            entity.EUser = User.Identity.Name;
            entity.EDate = Common.CurrentDateTime;
            errorList = GetBusinessLogicValidationForEdit(sYear, sMonth, sEmpID);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {

                try
                {
                    _pgmCommonservice.PGMUnit.OtherAdjustDeduction.Update(entity);
                    _pgmCommonservice.PGMUnit.OtherAdjustDeduction.SaveChanges();
                    return RedirectToAction("Index/" + type.ToString());
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    }
                    else
                    {
                        model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                    }
                }
            }
            else
            {
                model.IsError = 1;
                model.ErrMsg = errorList;
            }

            PrepareModel(model);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult Delete(int id)
        {
            bool result;
            string errMsg = string.Empty;
            var OAWithheld = _pgmCommonservice.PGMUnit.OtherAdjustDeduction.GetByID(id);
            errMsg = GetBusinessLogicValidationForEdit(OAWithheld.SalaryYear, OAWithheld.SalaryMonth, OAWithheld.EmployeeId);

            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    _pgmCommonservice.PGMUnit.OtherAdjustDeduction.Delete(id);
                    _pgmCommonservice.PGMUnit.OtherAdjustDeduction.SaveChanges();
                    result = true;
                }
                catch (UpdateException ex)
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
                catch (Exception ex)
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }
            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

       #endregion Action

        #region Others
        [NoCache]
        private PGM_OtherAdjustDeduct GetInsertUserAuditInfo(PGM_OtherAdjustDeduct taxrate)
        {
            taxrate.IUser = User.Identity.Name;
            taxrate.IDate = DateTime.Now;

            return taxrate;
        }

        [NoCache]
        private PGM_OtherAdjustDeduct GetEditUserAuditInfo(PGM_OtherAdjustDeduct taxrate)
        {
            taxrate.EUser = User.Identity.Name;
            taxrate.EDate = DateTime.Now;

            return taxrate;
        }

        [NoCache]
        private void PrepareModel(OtherAdjustDeductModel model)
        {
            model.SalaryYearList = SalaryYearList();
            model.SalaryMonthList = SalaryMonthList();
            model.SalaryHeadList = SalaryHeadList();
        }

        [NoCache]
        public string GetBusinessLogicValidation(PGM_OtherAdjustDeduct entity)
        {
            string errorMessage = string.Empty;
            var salaryexist = (from tr in _pgmCommonservice.PGMUnit.SalaryMaster.GetAll()
                               where (string.IsNullOrEmpty(entity.SalaryMonth) || entity.SalaryMonth == tr.SalaryMonth)
                               && (string.IsNullOrEmpty(entity.SalaryYear) || entity.SalaryYear == tr.SalaryYear)
                               && (entity.EmployeeId == tr.EmployeeId)
                               select tr.EmployeeId).ToList();

            int totalRecords = salaryexist == null ? 0 : salaryexist.Count;
            if (totalRecords > 0)
            {
                errorMessage = "Salary has been processed for the month. Create is not acceptable.";
            }

            if (entity.Amount<=0)
            {
                errorMessage="Amount must be greater than zero.";
            }

            return errorMessage;

        }

        private string GetBusinessLogicValidationForEdit(string spYear, string spMonth, int spEmpID)
        {
            string errorMessage = string.Empty;

            var salaryexist = (from tr in _pgmCommonservice.PGMUnit.SalaryMaster.GetAll()
                               where (string.IsNullOrEmpty(spMonth) || spMonth == tr.SalaryMonth)
                               && (string.IsNullOrEmpty(spYear) || spYear == tr.SalaryYear)
                               && (spEmpID == tr.EmployeeId)
                               select tr.EmployeeId).ToList();

            int totalRecords = salaryexist == null ? 0 : salaryexist.Count;

            if (totalRecords > 0)
            {
                errorMessage = "Salary has been processed for the month. Update/Delete is not acceptable.";
            }

            return errorMessage;

        }

        [NoCache]
        private IList<SelectListItem> SalaryMonthList()
        {
            IList<SelectListItem> SalaryMonth = new List<SelectListItem>();
            SalaryMonth.Add(new SelectListItem() { Text = "January", Value = "January" });
            SalaryMonth.Add(new SelectListItem() { Text = "February", Value = "February" });
            SalaryMonth.Add(new SelectListItem() { Text = "March", Value = "March" });
            SalaryMonth.Add(new SelectListItem() { Text = "April", Value = "April" });
            SalaryMonth.Add(new SelectListItem() { Text = "May", Value = "May" });
            SalaryMonth.Add(new SelectListItem() { Text = "June", Value = "June" });

            SalaryMonth.Add(new SelectListItem() { Text = "July", Value = "July" });
            SalaryMonth.Add(new SelectListItem() { Text = "August", Value = "August" });
            SalaryMonth.Add(new SelectListItem() { Text = "September", Value = "September" });
            SalaryMonth.Add(new SelectListItem() { Text = "October", Value = "October" });
            SalaryMonth.Add(new SelectListItem() { Text = "November", Value = "November" });
            SalaryMonth.Add(new SelectListItem() { Text = "December", Value = "December" });

            return SalaryMonth;
        }

        [NoCache]
        public ActionResult GetSalaryMonthList()
        {
            Dictionary<string, string> SalaryMonth = new Dictionary<string, string>();
            SalaryMonth.Add("January", "January");
            SalaryMonth.Add("February", "February");
            SalaryMonth.Add("March", "March");
            SalaryMonth.Add("April", "April");
            SalaryMonth.Add("May", "May");
            SalaryMonth.Add("June", "June");

            SalaryMonth.Add("July", "July");
            SalaryMonth.Add("August", "August");
            SalaryMonth.Add("September", "September");
            SalaryMonth.Add("October", "October");
            SalaryMonth.Add("November", "November");
            SalaryMonth.Add("December", "December");
     
            ViewBag.SalaryMonthList = SalaryMonth;
            return PartialView("Select", SalaryMonth);
        }

        [NoCache]
        private IList<SelectListItem> SalaryYearList()
        {
            IList<SelectListItem> SalaryYear = new List<SelectListItem>();           
            SalaryYear = Common.PopulateYearList().DistinctBy(x => x.Value).ToList();
            return SalaryYear;
        }

        [NoCache]
        public ActionResult GetSalaryYearList()
        {
            Dictionary<string, string> SalaryYear = new Dictionary<string, string>();
            for (int i = DateTime.Now.Year; i >=2000 ; i--)
            {
                var iyFormat = i.ToString();
                SalaryYear.Add(iyFormat, iyFormat);
            }

           
            ViewBag.IncomeYearList = SalaryYear;
            return PartialView("Select", SalaryYear);
        }

        [NoCache]
        public ActionResult GetDesignationList()
        {
            IList<SelectListItem> Designation = new List<SelectListItem>();

            Designation = _prmCommonservice.PRMUnit.DesignationRepository.GetAll()
            .ToList()
            .Select(y => new SelectListItem()
            {
                Text = y.Name,
                Value = y.Id.ToString()
            }).ToList();

            return PartialView("_SelectDesignation", Designation);
        }

        [NoCache]
        public ActionResult GetSalaryHeadList()
        {
            string HeadType="";
            if (type=="Adjust")
            {
                HeadType="Addition";
            }
            else
            {
                HeadType="Deduction";
            }
            IList<SelectListItem> SalaryHead = new List<SelectListItem>();

            SalaryHead =_prmCommonservice.PRMUnit.SalaryHeadRepository.GetAll()
            .Where(x => x.HeadType == HeadType && x.IsGrossPayHead == false).ToList()
            .Select(y => new SelectListItem()
            {
                Text = y.HeadName,
                Value = y.Id.ToString()
            }).ToList();

            return PartialView("_SelectDesignation", SalaryHead);
        }

        [NoCache]
        private IList<SelectListItem> SalaryHeadList()
        {
            string HeadType = "";
            if (type == "Adjust")
            {
                HeadType = "Addition";
            }
            else
            {
                HeadType = "Deduction";
            }

            IList<SelectListItem> SalaryHead = new List<SelectListItem>();

            SalaryHead = _prmCommonservice.PRMUnit.SalaryHeadRepository.GetAll()
            .Where(x => x.HeadType == HeadType && x.IsGrossPayHead==false).ToList()
            .Select(y => new SelectListItem()
            {
                Text = y.HeadName,
                Value = y.Id.ToString()
            }).ToList();

            return SalaryHead;
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(ResourceInfoViewModel model)
        {
            string errorList = string.Empty;
            errorList = GetValidationChecking(model);

            if (string.IsNullOrEmpty(errorList))
            {
                try
                {
                    var obj = _RresourceInfoService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
                    if (obj != null)
                    {
                        return Json(new
                        {
                            EmpID = obj.EmpID,
                            EmployeeInitial = obj.EmployeeInitial,
                            EmployeeName = obj.FullName,
                            EmployeeDesignation = obj.PRM_Designation.Name
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
        [NoCache]
        private string GetValidationChecking(ResourceInfoViewModel model)
        {
            string errorMessage = string.Empty;
            var obj = _RresourceInfoService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
          
            if (obj.DateofInactive != null)
            {
                errorMessage = "InActiveEmployee";
            }

            return errorMessage;
        }
        #endregion Others
    }
}
