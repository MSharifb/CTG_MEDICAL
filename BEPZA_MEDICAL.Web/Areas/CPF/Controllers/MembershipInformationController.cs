using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.IO;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.Data.SqlClient;

using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.MembershipInformation;
using BEPZA_MEDICAL.Domain.PGM;

using BEPZA_MEDICAL.Web.SecurityService;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.DAL.PRM;

/*
Revision History (RH):
		SL	    : 01
		Author	: AMN
		Date	: 2015-May-10
        SCR     : Internal
		Desc	: Make nominee attachment nullable
 
		---------
 		SL	    : 02
		Author	: AMN
		Date	: 2015-May-10
        SCR     : ERP_BEPZA_CPF_SCR.doc (SCR#06)
		Desc	: Membership did not save due to duplicate employee found (Unique key define in DB) but not checking before save
		---------
*/

namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class MembershipInformationController : Controller
    {
        #region Fields
        private readonly CPFCommonService _cpfCommonservice;

        private readonly PRMCommonSevice _prmCommonservice;
        private readonly PGMCommonService _pgmCommonservice;
        //private readonly PIMCommonService _pimCommonservice;
        private readonly ResourceInfoService _RresourceInfoService;
        private readonly EmployeeService _empService;
        private readonly string ApprovalProcessEnum = "CPFMbr";

        #endregion

        #region Constructor

        public MembershipInformationController(CPFCommonService cpfCommonservice, PRMCommonSevice prmCommonService, PGMCommonService pgmCommonservice, ResourceInfoService service, EmployeeService empService)
        {
            _cpfCommonservice = cpfCommonservice;
            _prmCommonservice = prmCommonService;

            _pgmCommonservice = pgmCommonservice;
            this._RresourceInfoService = service;
            this._empService = empService;
        }
        #endregion

        #region Properties

        public string Message { get; set; }


        #endregion

        #region Actions

        [NoCache]
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, MembershipInformationViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var listMemberInfo = _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll();
            var listEmployee = _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll();
            var listDesignation = _prmCommonservice.PRMUnit.DesignationRepository.GetAll();
            var salaryWithdrawFromZone = _prmCommonservice.PRMUnit.ZoneInfoRepository.GetAll();

            List<MembershipInformationViewModel> list = new List<MembershipInformationViewModel>();

            if (listMemberInfo != null && listMemberInfo.Count() != 0)
            {
                list = (from M in listMemberInfo
                        join E in listEmployee on M.EmployeeId equals E.Id
                        join Z in salaryWithdrawFromZone on E.SalaryWithdrawFromZoneId equals Z.Id
                        //join D in listDesignation on E.DesignationId equals D.Id
                        where (string.IsNullOrEmpty(model.EmployeeCode) || M.EmployeeCode.Trim() == model.EmployeeCode.Trim())
                        && (string.IsNullOrEmpty(model.EmployeeName) || E.FullName.Trim().ToLower().Contains(model.EmployeeName.Trim().ToLower()))
                            //&& (string.IsNullOrEmpty(model.EmployeeInitial) || E.EmployeeInitial.Trim().ToLower() == model.EmployeeInitial.Trim().ToLower())
                        && (string.IsNullOrEmpty(model.MembershipID) || M.MembershipID.Trim() == model.MembershipID.Trim())
                        //&& (string.IsNullOrEmpty(model.MembershipStatus) || M.MembershipStatus == model.MembershipStatus)
                        //&& M.MembershipID != null
                        select new MembershipInformationViewModel()
                        {
                            Id = M.Id,
                            EmployeeCode = M.EmployeeCode,
                            MembershipID = M.MembershipID,
                            EmployeeName = M.EmployeeName,
                            EmployeeInitial = E.EmployeeInitial,
                            DepartmentName = M.DepartmentName,
                            DesignationName = M.DesignationName,
                            SectionName = M.SectionName,
                            //PermanentDate = Convert.ToDateTime(E.DateofConfirmation),
                            JoiningDate = E.DateofJoining,
                            PermanentDate = M.PermanentDate,
                            MembershipStatus = M.MembershipStatus,
                            ApprovalStatus = M.APV_ApprovalStatus.StatusName,
                            DateOfMemberShip = M.ApproveDate,
                            SalaryWithdrawFromZoneName = Z.ZoneCode
                        }).ToList();
            }

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(model.SalaryWithdrawFromZoneName))
                {
                    if (!model.SalaryWithdrawFromZoneName.Equals("0"))
                    {
                        list = list.Where(q => q.SalaryWithdrawFromZoneName.ToString() == model.SalaryWithdrawFromZoneName).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(model.MembershipStatus))
                {
                    if (!string.IsNullOrEmpty(model.MembershipStatus))
                    {
                        list = list.Where(q => q.MembershipStatus == model.MembershipStatus).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(model.EmployeeCode))
                {
                    if (!string.IsNullOrEmpty(model.EmployeeCode))
                    {
                        list = list.Where(q => q.EmployeeCode.ToString() == model.EmployeeCode).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(model.EmployeeName))
                {
                    if (!string.IsNullOrWhiteSpace(model.EmployeeName))
                    {
                        list = list.Where(q => q.EmployeeName != null && q.EmployeeName.Contains(model.EmployeeName)).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(model.MembershipID))
                {
                    if (!string.IsNullOrWhiteSpace(model.MembershipID))
                    {
                        list = list.Where(q => q.MembershipID.ToString() == model.MembershipID).ToList();
                    }
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "MembershipID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.MembershipID).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.MembershipID).ToList();
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
                    d.MembershipID!=null?d.MembershipID:string.Empty,
                    d.EmployeeCode,
                    d.EmployeeName,
                    d.DepartmentName,
                    d.DesignationName,
                    d.SectionName,
                    d.PermanentDate!=null?Convert.ToDateTime(d.PermanentDate).ToString(DateAndTime.GlobalDateFormat):string.Empty,
                    d.JoiningDate!=null?Convert.ToDateTime(d.JoiningDate).ToString(DateAndTime.GlobalDateFormat):string.Empty,
                    d.DateOfMemberShip!=null?Convert.ToDateTime(d.DateOfMemberShip).ToString(DateAndTime.GlobalDateFormat):string.Empty,
                    d.MembershipStatus,
                    d.ApprovalStatus,
                    d.SalaryWithdrawFromZoneName,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            MembershipInformationViewModel model = new MembershipInformationViewModel();
            User user = MyAppSession.User;
            var emp = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == user.EmpId).FirstOrDefault();

            model.ApproveDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            model.ApproverList = new List<SelectListItem>();


            var bankList = _prmCommonservice.PRMUnit.BankNameRepository.GetAll();
            model.BankList = Common.PopulateDllList(bankList);

            var bankBranchList = _prmCommonservice.PRMUnit.BankBranchRepository.Get();
            model.BankBranchList = Common.PopulateDllList(bankBranchList);
            model.ActionType = "Create";

            #region Approval Flow Info
            var apvInfo = _prmCommonservice.PRMUnit.ApprovalFlowConfigurationRepository.GetAll().Where(x => x.APV_ApprovalProcess.ProcessNameEnum.Contains("CPFMbr")).FirstOrDefault();
            if (apvInfo != null)
            {
                model.IsConfigurableApprovalFlow = apvInfo.IsConfigurableApprovalFlow;
            }
            #endregion



            return View(model);
        }


        [HttpPost]
        [NoCache]
        public ActionResult Create(MembershipInformationViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            model.MembershipStatus = Convert.ToString(BEPZA_MEDICAL.Web.MvcApplication.CPFMembershipStatus.Active);

            decimal presentBasicPay = 0m;
            decimal.TryParse(model.PresentBasicPay.ToString(), out presentBasicPay);

            if (ModelState.IsValid)
            {
                var obj = model.ToEntity();
                obj.IUser = User.Identity.Name;
                obj.IDate = Common.CurrentDateTime;
                obj.PresentBasicPay = presentBasicPay;
                User user = MyAppSession.User;
                var empInfo = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == user.EmpId).FirstOrDefault();

                errorList = _cpfCommonservice.CheckMembershipID(obj);

                if (string.IsNullOrEmpty(errorList))
                {
                    try
                    {
                        var apvApprovalStatus = _prmCommonservice.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Submit")).FirstOrDefault();
                        if (apvApprovalStatus != null)
                        {
                            obj.ApprovalStatusId = apvApprovalStatus.Id;
                        }

                        _cpfCommonservice.CPFUnit.MembershipInformationRepository.Add(obj);
                        _cpfCommonservice.CPFUnit.MembershipInformationRepository.SaveChanges();

                        int applicationId = obj.Id;
                        int approverId = Convert.ToInt32(obj.ApprovedById);
                        int isOnlineApplication = 0;
                        _prmCommonservice.PRMUnit.FunctionRepository.InitializeApprovalProcess("CPFMbr", model.EmployeeCode, applicationId, isOnlineApplication, approverId, obj.IUser);
                        return RedirectToAction("Index", model);
                    }

                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        if (ex.InnerException != null && ex.InnerException is SqlException)
                        {
                            SqlException sqlException = ex.InnerException as SqlException;
                            model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);

                            if (model.ErrMsg == "Duplicate entry.")
                            {
                                model.ErrMsg = "Record can not be duplicate";
                            }
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
            }
            else
            {
                model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(model.ErrMsg) ? ErrorMessages.InsertFailed : model.ErrMsg) : Common.GetModelStateError(ModelState);
            }



            PopulateDropDown(model);
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            var member = _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetByID(id);
            MembershipInformationViewModel model = member.ToModel();
            model.Mode = CrudeAction.Edit;

            ModelState.Clear();
            model.ActionType = "Edit";

            var membershipAppliedBy =
               _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(m => m.EmpID == member.IUser).FirstOrDefault();
            if (membershipAppliedBy != null)
            {
                var appliersDesignation = _prmCommonservice.PRMUnit.DesignationRepository.GetAll()
                    .FirstOrDefault(d => d.Id == membershipAppliedBy.DesignationId);

                String desigShortName = String.Empty;
                if (appliersDesignation != null)
                    desigShortName = appliersDesignation.ShortName;

                model.MembershipAppliedByInOffline = membershipAppliedBy.FullName + " (" + membershipAppliedBy.EmpID +
                                                     ") - " + desigShortName;
                model.MembershipAppliedDateInOffline = membershipAppliedBy.IDate.ToString(DateAndTime.GlobalDateFormat);
            }

            PopulateDropDown(model);
            return View(model);
        }


        private MembershipInformationViewModel PopulateDropDown(MembershipInformationViewModel model)
        {

            var bankList = _prmCommonservice.PRMUnit.BankNameRepository.GetAll();
            var bankBranchList = _prmCommonservice.PRMUnit.BankBranchRepository.Get();

            model.BankList = Common.PopulateDllList(bankList);
            model.BankBranchList = Common.PopulateDllList(bankBranchList);

            if (model.Id > 0)
            {
                var approverList = _prmCommonservice.PRMUnit.FunctionRepository.GetApproverByApplicant(model.EmployeeCode, "CPFMbr");
                model.ApproverList = Common.PopulateEmployeeDDL(approverList);
            }
            return model;
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(MembershipInformationViewModel model)
        {
            model.IsError = 0;
            string errorList = string.Empty;
            if (ModelState.IsValid)
            {
                var membershipInfo = _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetByID(model.Id);

                //model.MembershipStatus = membershipInfo.MembershipStatus;
                model.ApprovalStatusId = membershipInfo.ApprovalStatusId;
                model.MembershipID = model.MembershipID.Trim();
                var master = model.ToEntity();


                errorList = _cpfCommonservice.CheckMembershipID(master);

                int approvalProcessId = _prmCommonservice.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == ApprovalProcessEnum).FirstOrDefault().Id;
                var approvalInfo = _prmCommonservice.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApplicationId == master.Id && q.ApprovalProcessId == approvalProcessId).DefaultIfEmpty().OfType<APV_ApplicationInformation>().ToList();

                if (string.IsNullOrEmpty(errorList))
                {
                    try
                    {
                        if(model.MembershipStatus == "Inactive")
                        {
                            _cpfCommonservice.CPFUnit.MembershipInformationRepository.Update(master);
                            _cpfCommonservice.CPFUnit.MembershipInformationRepository.SaveChanges();
                        }
                        else if (approvalInfo == null || approvalInfo.Count <= 1)
                        {
                            // TODO: Need to use enum instead of string comparison
                            master.ApprovalStatusId = _prmCommonservice.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Submit")).FirstOrDefault().Id;
                            _cpfCommonservice.CPFUnit.MembershipInformationRepository.Update(master);
                            _cpfCommonservice.CPFUnit.MembershipInformationRepository.SaveChanges();

                            //_prmCommonservice.PRMUnit.FunctionRepository.InitializeApprovalProcess("CPFMbr", model.EmployeeCode, master.Id, 0, (int)master.ApprovedById, master.IUser);
                        }
                        //return RedirectToAction("Index");
                        model.IsError = 0;
                        model.IsSuccessful = true;
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        model.IsError = 1;
                        if (ex.InnerException != null && ex.InnerException is SqlException)
                        {
                            SqlException sqlException = ex.InnerException as SqlException;
                            model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                            if (model.ErrMsg == "Duplicate entry.")
                            {
                                model.ErrMsg = "Record can not be duplicate";
                            }
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
            }
            else
            {
                model.IsError = 1;
                model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(model.ErrMsg) ? ErrorMessages.UpdateFailed : model.ErrMsg) : Common.GetModelStateError(ModelState);
            }
            PopulateDropDown(model);
            model.Mode = CrudeAction.Edit;
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;
            errMsg = _cpfCommonservice.DeleteMembershipValidation(id);

            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    var resultDelete = _cpfCommonservice.CPFUnit.FunctionRepository.DeletePFMembeship(id);
                    if (resultDelete == 0)
                    {
                        _cpfCommonservice.CPFUnit.MembershipInformationRepository.SaveChanges();
                        result = true;
                        errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                    }
                    else
                    {
                        errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
                    }
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    result = false;
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                }
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  Utilities

        [NoCache]
        public byte[] ConvertToBytes(HttpPostedFileBase Image)
        {
            byte[] imageBytes = null;
            if (Image == null)
            {
                return null;
            }
            try
            {
                BinaryReader reader = new BinaryReader(Image.InputStream);
                imageBytes = reader.ReadBytes((int)Image.ContentLength);
                return imageBytes;
            }
            catch
            {
                return null;
            }
        }

        [NoCache]
        public HttpPostedFileBase GetAttachment(string FileName, string FileExtension, byte[] Photograph)
        {
            fileUploader obj = new fileUploader();

            return obj;
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(MembershipInformationViewModel model)
        {
            string errorList = string.Empty;
            errorList = GetValidationChecking(model);

            if (string.IsNullOrEmpty(errorList))
            {
                try
                {
                    var emp = _RresourceInfoService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);

                    if (emp != null)
                    {
                        var empInfoPersonal = _prmCommonservice.PRMUnit.PersonalInfoRepository.Get(d => d.EmployeeId == emp.Id).FirstOrDefault();
                        var empDptSectionInfo = _empService.GetEmpDepartmentOfficeSectionSubSection(Convert.ToInt32(emp.OrganogramLevelId));
                        decimal basicpay = 0;

                        if (empInfoPersonal != null)
                        {
                            var salaryInfo = _pgmCommonservice.GetBasicSalaryByEmployeeId(empInfoPersonal.EmployeeId);
                            if (salaryInfo != null)
                            {
                                decimal.TryParse(salaryInfo.Basic.ToString(), out basicpay);
                            }
                        }


                        return Json(new
                        {
                            EmployeeId = emp.EmpID,
                            EmployeeName = emp.FullName,
                            FatherName = empInfoPersonal != null && empInfoPersonal.FatherName != null ? empInfoPersonal.FatherName : string.Empty,
                            MotherName = empInfoPersonal != null && empInfoPersonal.MotherName != null ? empInfoPersonal.MotherName : string.Empty,
                            Department = empDptSectionInfo != null ? empDptSectionInfo.DepartmentName : string.Empty,
                            Designation = emp.PRM_Designation.Name,
                            Section = emp.PRM_Section != null ? emp.PRM_Section.Name : string.Empty,
                            JoiningDate = emp.DateofJoining.ToString(DateAndTime.GlobalDateFormat),
                            PermanentDate = Convert.ToDateTime(emp.DateofConfirmation).ToString(DateAndTime.GlobalDateFormat),

                            DateOfBirth = emp.DateofBirth.ToString(DateAndTime.GlobalDateFormat),
                            EmployeeCategory = emp.PRM_StaffCategory != null ? emp.PRM_StaffCategory.Name : string.Empty,
                            PresentPayScale = emp.PRM_SalaryScale != null ? emp.PRM_SalaryScale.SalaryScaleName : string.Empty,
                            Office = emp.PRM_JobLocation != null ? emp.PRM_JobLocation.Name : string.Empty,
                            Nationality = empInfoPersonal != null ? empInfoPersonal.PRM_Nationality.Name : string.Empty,

                            PresentBasicPay = basicpay,
                            PresentAddress = empInfoPersonal != null ? empInfoPersonal.PresentAddress1 : string.Empty,
                            PermanentAddress = empInfoPersonal != null ? empInfoPersonal.PermanentAddress1 : string.Empty,


                            BankId = emp.BankId,
                            BranchId = emp.BankBranchId,
                            AccountNo = emp.BankAccountNo
                        });
                    }
                    else
                    {
                        return Json(new { Result = false });
                    }
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        return Json(new { Result = false });

                    }
                    else
                    {
                        model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
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
        public ActionResult GetMembershipStatusList()
        {
            Dictionary<string, string> MemberhipStatus = new Dictionary<string, string>();

            MemberhipStatus.Add(Convert.ToString(BEPZA_MEDICAL.Web.MvcApplication.CPFMembershipStatus.Active), Convert.ToString(BEPZA_MEDICAL.Web.MvcApplication.CPFMembershipStatus.Active));
            MemberhipStatus.Add(Convert.ToString(BEPZA_MEDICAL.Web.MvcApplication.CPFMembershipStatus.Inactive), Convert.ToString(BEPZA_MEDICAL.Web.MvcApplication.CPFMembershipStatus.Inactive));

            return PartialView("_Select", MemberhipStatus);
        }

        [NoCache]
        public ActionResult GetMemberApprovalStatusList()
        {
            Dictionary<string, string> MemberApprovalStatus = new Dictionary<string, string>();
            var approvalStatusList = _cpfCommonservice.CPFUnit.ApprovalStatusRepository.GetAll().ToList();
            foreach (var item in approvalStatusList)
            {
                MemberApprovalStatus.Add(item.StatusName, item.StatusName);
            }
            return PartialView("_Select", MemberApprovalStatus);
        }

        [NoCache]
        private string GetValidationChecking(MembershipInformationViewModel model)
        {
            var CheckForMembershipAvailability = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Find(d => d.EmployeeId == model.EmployeeId);
            if (CheckForMembershipAvailability.Count() != 0)
            {
                return "ExistEmployee";
            }

            var CheckForInactiveOrConfirmEmployee = _RresourceInfoService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
            if (CheckForInactiveOrConfirmEmployee.DateofInactive != null)
            {
                return "InActiveEmployee";
            }
            if (CheckForInactiveOrConfirmEmployee.DateofConfirmation == null)
            {
                return "ConfirmEmployee";
            }

            var CheckForSalaryStructure =
                _RresourceInfoService.PRMUnit.EmpSalaryRepository.Get(s => s.EmployeeId == model.EmployeeId);
            if (!CheckForSalaryStructure.Any())
            {
                return "NoSalaryStructure";
            }

            return string.Empty;
        }

        #endregion

        #region Autocomplete For Membership ID

        public JsonResult AutoCompleteMembershipList(string term)
        {
            // TODO: Need to use enum instead of string comparison
            var approvedId = _cpfCommonservice.CPFUnit.ApprovalStatusRepository.Get(a => a.StatusName == "Approved").FirstOrDefault().Id;

            var result = (from r in _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll()
                          where r.MembershipID != null
                              && r.MembershipID.ToLower().StartsWith(term.ToLower())
                              && r.MembershipStatus == "Active"
                              && r.ApprovalStatusId == approvedId
                          select new { r.Id, r.MembershipID, r.EmployeeName }).Distinct().OrderBy(x => x.MembershipID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult GetMembershipInformation(string membershipID)
        {
            string msg = string.Empty;
            var obj = (from tr in _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll()
                       where tr.MembershipID == membershipID
                       select tr).FirstOrDefault();

            // TODO: Need to use enum instead of string comparison
            var approvedId = _cpfCommonservice.CPFUnit.ApprovalStatusRepository.Get(a => a.StatusName == "Approved").FirstOrDefault().Id;

            if (obj != null && obj.ApprovalStatusId != approvedId && obj.MembershipStatus != "Active")
            {
                msg = "Inactive";
            }
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    if (obj != null)
                    {
                        return Json(new
                        {
                            Id = obj.Id,
                            WitnessMembershipID = obj.MembershipID,
                            WitnessName = obj.EmployeeName,
                            WitnessAddress = obj.PresentAddress,
                            WitnessDesignation = obj.DesignationName
                        });
                    }
                    else
                    {
                        return Json(new { Result = false });
                    }
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
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
        #endregion

        #region Approval

        [NoCache]
        public JsonResult GetApproverInfo(string employeeId)
        {
            int empId = 0;
            int.TryParse(employeeId, out empId);
            string empIdStr = _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetByID(empId).EmpID;
            var employeeList = _prmCommonservice.PRMUnit.FunctionRepository.GetApproverByApplicant(empIdStr, "CPFMbr");
            var empInfoList = new List<PRM_EmploymentInfo>();

            foreach (var d in employeeList)
            {
                empInfoList.Add(new PRM_EmploymentInfo
                {
                    Id = (Int32)d.Id,
                    FullName = d.FullName,
                });
            }

            return Json(empInfoList, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
