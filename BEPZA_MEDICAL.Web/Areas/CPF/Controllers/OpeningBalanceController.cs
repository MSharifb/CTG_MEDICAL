using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.OpeningBalance;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.Data;
using System.Data.SqlClient;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.Domain.PGM;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class OpeningBalanceController : Controller
    {
        #region Fields

        private readonly PGMCommonService _pgmCommonservice;
        private readonly CPFCommonService _cpfCommonservice;
        private readonly EmployeeService _empService;

        #endregion

        #region Constructor

        public OpeningBalanceController(CPFCommonService cpfCommonservice, EmployeeService empService, PGMCommonService pgmCommonService)
        {
            this._cpfCommonservice = cpfCommonservice;
            this._empService = empService;
            this._pgmCommonservice = pgmCommonService;
        }

        #endregion

        #region Actions---------------------------

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, OpeningBalanceViewModel model)
        {
            string filterExpression = string.Empty;
            int totalRecords = 0;

            var OpList = _cpfCommonservice.CPFUnit.OpeningBalanceRepository.GetAll();
            var MemList = _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll();
            var empInfoList = _empService.PRMUnit.EmploymentInfoRepository.GetAll();
            var desigList = _empService.PRMUnit.DesignationRepository.GetAll();

            var list = (from Op in OpList
                        join Mem in MemList on Op.MembershipId equals Mem.Id
                        join empInfo in empInfoList on Mem.EmployeeId equals empInfo.Id
                        join desig in desigList on empInfo.DesignationId equals desig.Id
                        where
                        (string.IsNullOrEmpty(model.EmployeeName) || (empInfo.FullName ?? String.Empty).ToLower().Contains((model.EmployeeName ?? String.Empty).ToLower()))
                        // Mem.EmployeeId == model.EmployeeId
                        // where (model.EmployeeId == 0 || Mem.EmployeeId == model.EmployeeId)
                        //             && (model.MembershipIDName == string.Empty || Mem.MembershipID == model.MembershipIDName)
                        //             && (string.IsNullOrEmpty(model.EmployeeInitial) || empInfo.EmployeeInitial == model.EmployeeInitial)
                        //             && (model.PermanentDate==null||model.PermanentDate==DateTime.MinValue) || (Mem.PermanentDate >= Convert.ToDateTime(model.PermanentDate) )
                        //             && (model.DateOfOpening==null||model.DateOfOpening==DateTime.MinValue) || (Op.DateOfOpening >= Convert.ToDateTime(model.DateOfOpening) )
                        select new OpeningBalanceViewModel()
                        {
                            Id = Op.Id,
                            EmployeeId = Mem.EmployeeId,
                            MembershipIDName = Mem.MembershipID,
                            EmployeeName = Mem.EmployeeName,
                            DesignationName = desig.Name,
                            //EmployeeInitial = empInfo.EmployeeInitial,
                            DateOfOpening = Op.DateOfOpening,
                            ComTotalContrib = Op.ComTotal,
                            EmpTotalContrib = Op.EmpTotal,
                        }).ToList();

            if (request.Searching)
            {
                if (model.EmployeeId != 0)
                {
                    list = list.Where(d => d.EmployeeId == model.EmployeeId).ToList();
                }
                if (model.MembershipIDName != null && model.MembershipIDName != string.Empty)
                {
                    list = list.Where(d => d.MembershipIDName == model.MembershipIDName).ToList();
                }
                //if (model.EmployeeName != null && model.EmployeeName != string.Empty)
                //{
                //    list = list.Where(d => d.EmployeeName == model.EmployeeName).ToList();
                //}
                if (model.EmployeeInitial != null && model.EmployeeInitial != string.Empty)
                {
                    list = list.Where(d => d.EmployeeInitial == model.EmployeeInitial).ToList();
                }
                if (model.PermanentDate != null && model.PermanentDate != DateTime.MinValue)
                {
                    list = list.Where(d => d.PermanentDate >= model.PermanentDate).ToList();
                }
                if (model.PermanentDateTo != null && model.PermanentDateTo != DateTime.MinValue)
                {
                    list = list.Where(d => d.PermanentDate <= model.PermanentDateTo).ToList();
                }
                if (model.PeriodId != 0)
                {
                    list = list.Where(d => d.PeriodId == model.PeriodId).ToList();
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
                {     d.Id ,
                      d.EmployeeId ,
                      d.MembershipIDName,
                      d.EmployeeName ,
                      d.DesignationName,
                      d.PermanentDate!=null?Convert.ToDateTime(d.PermanentDate).ToString(DateAndTime.GlobalDateFormat):string.Empty ,
                      d.PermanentDateTo!=null?Convert.ToDateTime(d.PermanentDateTo).ToString(DateAndTime.GlobalDateFormat):string.Empty,
                      d.DateOfOpening!=null?Convert.ToDateTime(d.DateOfOpening).ToString(DateAndTime.GlobalDateFormat):string.Empty ,
                      d.ComTotalContrib,
                      d.EmpTotalContrib,
                      d.PeriodId,
                      d.ComTotalContrib + d.EmpTotalContrib,
                      "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Index()
        {
            OpeningBalanceViewModel model = new OpeningBalanceViewModel();
            return View(model);
        }

        [NoCache]
        public ActionResult Create()
        {
            var model = new OpeningBalanceViewModel();
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(OpeningBalanceViewModel model)
        {
            string errorList = string.Empty;

            model.IsError = 1;

            errorList = GetBusinessLogicValidation(model, "insert");

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                var obj = model.ToEntity();
                obj.IUser = User.Identity.Name;
                obj.IDate = Common.CurrentDateTime;

                try
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = "Duplicate Entry";
                        return View(model);
                    }

                    var membershipInfo = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(t => t.EmployeeId == model.EmployeeId).FirstOrDefault();
                    obj.EmpTotal = model.EmpTotalContrib + model.EmpTotalProfit;
                    obj.ComTotal = model.ComTotalContrib + model.ComTotalInterest;
                    obj.MembershipId = membershipInfo.Id;

                    _cpfCommonservice.CPFUnit.OpeningBalanceRepository.Add(obj);
                    _cpfCommonservice.CPFUnit.OpeningBalanceRepository.SaveChanges();
                    model.IsError = 0;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    return RedirectToAction("Index");
                    //return RedirectToAction("Index",model);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                    }
                    else
                    {
                        model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                    }
                }
            }
            else
            {
                model.ErrMsg = errorList;
            }

            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            CPF_OpeningBalance ObjOpening = _cpfCommonservice.CPFUnit.OpeningBalanceRepository.GetByID(id);

            OpeningBalanceViewModel model = ObjOpening.ToModel();
            if (ObjOpening != null)
            {
                var member = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(d => d.Id == ObjOpening.MembershipId).FirstOrDefault();
                if (member != null)
                {
                    var empInfo = _empService.PRMUnit.EmploymentInfoRepository.Get(d => d.Id == member.EmployeeId).FirstOrDefault();
                    model.EmployeeId = member.EmployeeId;
                    model.EmployeeCode = member.EmployeeCode;
                    model.EmployeeInitial = empInfo.EmployeeInitial;
                    model.EmployeeName = member.EmployeeName;
                    model.MembershipCode = member.MembershipID;
                    model.DateOfMembership = member.ApproveDate == null ? null : member.ApproveDate;
                    model.DesignationName = member.DesignationName;
                    model.JoiningDate = member.JoiningDate;
                }
            }

            model.Mode = "Update";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(OpeningBalanceViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;

            errorList = GetBusinessLogicValidation(model, "update");

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                CPF_OpeningBalance obj = model.ToEntity();
                obj.EUser = User.Identity.Name;
                obj.EDate = Common.CurrentDateTime;

                if (CheckDuplicateEntry(model, model.Id))
                {
                    model.strMessage = Resources.ErrorMessages.UniqueIndex;
                    model.errClass = "failed";
                    return View(model);
                }
                var membershipInfo = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(t => t.EmployeeId == model.EmployeeId).FirstOrDefault();
                //var pfPeriod = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.Get(d => d.IsActive == true).FirstOrDefault();
                //obj.PeriodId = pfPeriod.Id;
                obj.EmpTotal = model.EmpTotalContrib + model.EmpTotalProfit;
                obj.ComTotal = model.ComTotalContrib + model.ComTotalInterest;
                obj.MembershipId = membershipInfo.Id;

                _cpfCommonservice.CPFUnit.OpeningBalanceRepository.Update(obj);
                _cpfCommonservice.CPFUnit.OpeningBalanceRepository.SaveChanges();
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                return RedirectToAction("Index", model);
            }
            else
            {
                model.ErrMsg = errorList;
            }

            return View(model);
        }

        [NoCache]
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = "Error while deleting data!";

            try
            {

                //List<Type> allTypes = new List<Type> { typeof(CPF_OpeningBalance) };
                _cpfCommonservice.CPFUnit.OpeningBalanceRepository.Delete(id);
                _cpfCommonservice.CPFUnit.OpeningBalanceRepository.SaveChanges();
                result = true;



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
            catch (Exception)
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

        #region Utilities---------------------

        public ActionResult ActiveEmployeeSearchList()
        {
            var model = new MembershipSearchViewModel();
            return View("ActiveEmployeeSearch", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetActiveEmployeeList(JqGridRequest request, MembershipSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var division = _empService.PRMUnit.DivisionRepository.GetAll();

            var list = (from ma in _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll()
                        join empInfo in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on ma.EmployeeId equals empInfo.Id
                        select new MembershipSearch()
                        {
                            ID = empInfo.Id,
                            EmpId = empInfo.EmpID,
                            GradeId = empInfo.PRM_EmpSalary.GradeId,
                            GradeName = empInfo.PRM_EmpSalary.PRM_JobGrade.GradeName,
                            DateOfJoining = empInfo.DateofJoining,
                            DateOfConfirmation = empInfo.DateofConfirmation,
                            DesignationId = empInfo.DesignationId,
                            DesigName = empInfo.PRM_Designation.Name,
                            DivisionId = empInfo.DivisionId,
                            DivisionName = division.FirstOrDefault(d => d.Id == empInfo.DivisionId) != null ? division.FirstOrDefault(d => d.Id == empInfo.DivisionId).Name : String.Empty,  //empInfo.PRM_Division.Name,
                            EmpInitial = empInfo.EmployeeInitial,
                            EmpName = empInfo.FullName,
                            EmpTypeId = empInfo.EmploymentTypeId,
                            EmpTypeName = empInfo.PRM_EmploymentType.Name,
                            JobLocationId = empInfo.JobLocationId,
                            JobLocName = empInfo.PRM_JobLocation.Name,
                            ResourceLevelId = empInfo.ResourceLevelId,
                            StaffCategoryId = empInfo.StaffCategoryId,
                            DateOfInactive = empInfo.DateofInactive,
                            IsContractual = empInfo.IsContractual

                        }).ToList();


            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.ID), new List<object>()
                {
                    item.EmpName,
                    item.ID,
                    item.EmpId,
                    item.EmpInitial,

                    item.DesigName,
                    item.DivisionName,
                    item.JobLocName,
                    item.GradeName,
                    item.DateOfJoining.ToString(DateAndTime.GlobalDateFormat),
                    item.DateOfConfirmation.HasValue ? item.DateOfConfirmation.Value.ToString(DateAndTime.GlobalDateFormat) : null,

                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult GetMemberByEmployeeCode(string query)
        {
            query = query.Replace(" ", "");
            if (query.Length > 1)
            {
                int op = query.LastIndexOf(",");
                query = query.Substring(op + 1);
            }

            var users = (from u in _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll()
                         where u.EmployeeCode.Contains(query)
                         orderby u.EmployeeCode // optional
                         select u.EmployeeCode).Distinct().ToArray();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        private string GetBusinessLogicValidation(OpeningBalanceViewModel model, string action)
        {
            string errorMessage = string.Empty;

            var salary =
                _pgmCommonservice.PGMUnit.SalaryMasterRepository.Get(s => s.EmployeeId == model.EmployeeId)
                    .FirstOrDefault();
            //var pfPeriod = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.Get(d => d.IsActive == true).FirstOrDefault();
            var member = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(d => d.EmployeeId == model.EmployeeId).FirstOrDefault();

            if (member == null)
            {
                errorMessage = "This employee is not PF member yet.";
            }
            //else if (salary != null
            //    && (model.DateOfOpening > Convert.ToDateTime(salary.SalaryYear + "-" + salary.SalaryMonth + "-01")))
            //{
            //    errorMessage = "Date of opening balance must be less than first salary month." + salary.SalaryYear + "/" + salary.SalaryMonth;
            //}
            //else if (model.DateOfOpening < pfPeriod.StartDate || model.DateOfOpening > pfPeriod.EndDate)
            //{
            //    errorMessage = "Date of opening balance must be within active financial year.";
            //}
            //else if (!(model.ComCoreContribution > 0) || !(model.EmpCoreContribution > 0))
            //{
            //    errorMessage = "Core contribution must be greater than zero.";
            //}
            //else if (model.ComCoreContribution < model.ComProfit || model.EmpCoreContribution < model.EmpProfit)
            //{
            //    errorMessage = "Core contribution must be greater than share of interest";
            //}



            return errorMessage;
        }

        private bool CheckDuplicateEntry(OpeningBalanceViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _cpfCommonservice.CPFUnit.OpeningBalanceRepository.Get(q => q.MembershipId == model.MembershipId).Any();
            }

            else
            {
                return _cpfCommonservice.CPFUnit.OpeningBalanceRepository.Get(q => q.MembershipId == model.MembershipId && strMode != q.Id).Any();
            }
        }

        [NoCache]
        public JsonResult GetMembershipInfoByEmpId(string id)
        {
            //EmployeeCode=id in membership
            string empInitial = string.Empty;
            int cpfPeriodId = 0;
            string cpfPeriod = string.Empty;

            var member = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(d => d.EmployeeCode == id).FirstOrDefault();
            var openingBalance = _cpfCommonservice.CPFUnit.OpeningBalanceRepository.Get(d => d.MembershipId == member.Id).FirstOrDefault();
            if (openingBalance == null)
            {
                openingBalance = new CPF_OpeningBalance();
            }
            if (member == null)
            {
                member = new CPF_MembershipInfo();
            }
            else
            {
                var employee = _empService.PRMUnit.EmploymentInfoRepository.Get(d => d.Id == member.EmployeeId).FirstOrDefault();
                //var pfPeriod = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.Get(d => d.IsActive == true).FirstOrDefault();
                if (employee != null)
                {
                    empInitial = employee.EmployeeInitial;
                }
                //if (pfPeriod != null)
                //{
                //    cpfPeriodId = pfPeriod.Id;
                //    cpfPeriod = pfPeriod.CPFPeriod;
                //}


            }


            return Json(new
            {
                EmployeeId = member.EmployeeId,
                EmployeeCode = member.EmployeeCode,
                EmployeeInitial = empInitial,
                EmployeeName = member.EmployeeName,
                MembershipId = member.Id,
                MembershipCode = member.MembershipID,
                DateOfMembership = member.ApproveDate != null ? Convert.ToDateTime(member.ApproveDate).ToString(DateAndTime.GlobalDateFormat) : string.Empty,
                Designation = member.DesignationName,
                DateOfJoining = member.JoiningDate != null ? Convert.ToDateTime(member.JoiningDate).ToString(DateAndTime.GlobalDateFormat) : string.Empty,
                PeriodId = cpfPeriodId,
                CpfPeriod = cpfPeriod,
                // DateOfOpening = openingBalance.DateOfOpening != null ? Convert.ToDateTime(openingBalance.DateOfOpening).ToString(DateAndTime.GlobalDateFormat) : string.Empty,
                ComCoreContribution = openingBalance.ComCoreContribution,
                ComProfit = openingBalance.ComProfit,
                ComTotal = openingBalance.ComTotal,
                EmpCoreContribution = openingBalance.EmpCoreContribution,
                EmpProfit = openingBalance.EmpProfit,
                EmpTotal = openingBalance.EmpTotal


            }, JsonRequestBehavior.AllowGet);



        }

        [NoCache]
        public JsonResult GetMembershipInfoByMemberId(int id)
        {
            string empInitial = string.Empty;
            int cpfPeriodId = 0;
            string cpfPeriod = string.Empty;

            var member = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(d => d.Id == id).FirstOrDefault();

            var openingBalance = _cpfCommonservice.CPFUnit.OpeningBalanceRepository.Get(d => d.MembershipId == member.Id).FirstOrDefault();
            if (openingBalance == null)
            {
                openingBalance = new CPF_OpeningBalance();
            }
            if (member == null)
            {
                member = new CPF_MembershipInfo();
            }
            else
            {
                var employee = _empService.PRMUnit.EmploymentInfoRepository.Get(d => d.Id == member.EmployeeId).FirstOrDefault();
                if (employee != null)
                {
                    empInitial = employee.EmployeeInitial;
                }
            }


            return Json(new
            {
                EmployeeId = member.EmployeeId,
                EmployeeCode = member.EmployeeCode,
                EmployeeInitial = empInitial,
                EmployeeName = member.EmployeeName,
                MembershipId = member.Id,
                MembershipCode = member.MembershipID,
                DateOfMembership = member.ApproveDate != null ? Convert.ToDateTime(member.ApproveDate).ToString(DateAndTime.GlobalDateFormat) : string.Empty,
                Designation = member.DesignationName,
                DateOfJoining = member.JoiningDate != null ? Convert.ToDateTime(member.JoiningDate).ToString(DateAndTime.GlobalDateFormat) : string.Empty,
                PeriodId = cpfPeriodId,
                CpfPeriod = cpfPeriod,
                //DateOfOpening = openingBalance.DateOfOpening != null ? Convert.ToDateTime(openingBalance.DateOfOpening).ToString(DateAndTime.GlobalDateFormat) : string.Empty,
                ComCoreContribution = openingBalance.ComCoreContribution,
                ComProfit = openingBalance.ComProfit,
                ComTotal = openingBalance.ComTotal,
                EmpCoreContribution = openingBalance.EmpCoreContribution,
                EmpProfit = openingBalance.EmpProfit,
                EmpTotal = openingBalance.EmpTotal,

            }, JsonRequestBehavior.AllowGet);



        }

        [NoCache]
        public JsonResult GetCompanyPartColCaption()
        {
            var _customPropertiesService = new CustomPropertiesService();
            var companyPartColCaption = _customPropertiesService.RetriveDisplayName("CPFOpeningBalanceGridList", "CompanyPart");

            return Json(new
            {
                Caption = Common.GetString( companyPartColCaption)
            }, JsonRequestBehavior.AllowGet);

        }


        public class MembershipSearch
        {
            public Int32 ID { get; set; }
            public string EmpId { get; set; }
            public string EmpInitial { get; set; }
            public string EmpName { get; set; }
            public int? DesignationId { get; set; }
            public string DesigName { get; set; }
            public int EmpTypeId { get; set; }
            public string EmpTypeName { get; set; }
            public int? DivisionId { get; set; }
            public string DivisionName { get; set; }
            public int? JobLocationId { get; set; }
            public string JobLocName { get; set; }
            public int? GradeId { get; set; }
            public string GradeName { get; set; }
            public int StaffCategoryId { get; set; }
            public string StaffCatName { get; set; }
            public int? ResourceLevelId { get; set; }
            public string ResLevelName { get; set; }
            public DateTime DateOfJoining { get; set; }
            public DateTime? DateOfConfirmation { get; set; }
            public DateTime? DateOfInactive { get; set; }
            public bool IsContractual { get; set; }
        }

        #region Dropdown Population-------

        [NoCache]
        public ActionResult GetDesignation()
        {
            var designations = _empService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();

            return PartialView("_Select", Common.PopulateDllList(designations));
        }

        [NoCache]
        public ActionResult GetDivision()
        {
            var divisions = _empService.PRMUnit.DivisionRepository.GetAll().ToList();

            return PartialView("_Select", Common.PopulateDllList(divisions));
        }

        [NoCache]
        public ActionResult GetJobLocation()
        {
            var jobLocations = _empService.PRMUnit.JobLocationRepository.GetAll().ToList();

            return PartialView("_Select", Common.PopulateDllList(jobLocations));
        }

        [NoCache]
        public ActionResult GetGrade()
        {
            var grades = _empService.PRMUnit.JobGradeRepository.GetAll().OrderBy(x => x.GradeName).ToList();

            return PartialView("_Select", Common.PopulateJobGradeDDL(grades));
        }

        [NoCache]
        public ActionResult GetEmploymentType()
        {
            var grades = _empService.PRMUnit.EmploymentTypeRepository.GetAll().ToList();

            return PartialView("_Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetStaffCategory()
        {
            var grades = _empService.PRMUnit.PRM_StaffCategoryRepository.GetAll().ToList();

            return PartialView("_Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetResource()
        {
            var grades = _empService.PRMUnit.ResourceLevelRepository.GetAll().ToList();

            return PartialView("_Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetEmployeeStatus()
        {
            // return PartialView("Select", Common.PopulateEmployeeStatusList());
            var empStatus = _empService.PRMUnit.EmploymentStatusRepository.GetAll().ToList();

            return PartialView("_Select", Common.PopulateDllList(empStatus));
        }
        [NoCache]
        public ActionResult GetCPFPeriod()
        {
            //var periods = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.GetAll().OrderBy(x => x.CPFPeriod).ToList();

            return PartialView("_Select");
        }


        #endregion

        #endregion
    }
}
