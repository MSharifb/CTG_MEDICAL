using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Collections;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Collections.Generic;

using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Domain.PGM;

using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.SecurityService;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.MembershipInformation;
using BEPZA_MEDICAL.DAL.CPF;

/*
Revision History (RH):
 		SL	    : 01
		Author	: AMN
		Date	: 2015-May-10
        SCR     : ERP_BEPZA_CPF_SCR.doc (SCR#08)
		Desc	: Make nominee attachment nullable and more (Describe in SCR)
		---------
*/
namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class MembershipApplicationController : Controller
    {
        #region Fields
        private readonly CPFCommonService _cpfCommonservice;

        private readonly PRMCommonSevice _prmCommonservice;
        private readonly PGMCommonService _pgmCommonservice;
        //private readonly PIMCommonService _pimCommonservice;
        private readonly ResourceInfoService _ResourceInfoService;
        #endregion

        #region Constructor

        public MembershipApplicationController(CPFCommonService cpfCommonservice, PRMCommonSevice prmCommonService, PGMCommonService pgmCommonservice, ResourceInfoService service)
        {
            this._cpfCommonservice = cpfCommonservice;
            this._prmCommonservice = prmCommonService;

            this._pgmCommonservice = pgmCommonservice;
            this._ResourceInfoService = service;
        }
        #endregion

        #region Properties

        public string Message { get; set; }

        #endregion

        #region Actions

        public ActionResult Index()
        {
            return View();
        }

        [NoCache]
        public RedirectResult MembershipApplicationPreview()
        {
            int membershipId = default(int);
            User user = MyAppSession.User;

            var emp = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == user.EmpId).FirstOrDefault();
            if (emp != null)
            {
                var memberModel = (from m in _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll() where m.EmployeeId == emp.Id select m).LastOrDefault();
                if (memberModel != null)
                {
                    membershipId = _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetByID(memberModel.Id).Id;
                }
            }
            return Redirect("~/Reports/CPF/viewers/MembershipPreviewForm.aspx?id=" + membershipId);
        }

        public ActionResult Create()
        {
            MembershipInformationViewModel model = new MembershipInformationViewModel();
            User user = MyAppSession.User;

            var emp = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == user.EmpId).FirstOrDefault();


            if (emp != null)
            {
                var memberModel = (from m in _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll() where m.EmployeeId == emp.Id select m).LastOrDefault();
                if (memberModel != null)
                {

                    return RedirectToAction("Edit", new { employeeId = emp.Id });

                }
                else
                {
                    model = GetApplicantInfo(user.EmpId);

                    PopulateDropDown(model);
                    model.Mode = CrudeAction.Create;
                    #region Approval Flow Info
                    var apvInfo = _prmCommonservice.PRMUnit.ApprovalFlowConfigurationRepository.GetAll().Where(x => x.APV_ApprovalProcess.ProcessNameEnum.Contains("CPFMbr")).FirstOrDefault();
                    if (apvInfo != null)
                    {
                        model.IsConfigurableApprovalFlow = apvInfo.IsConfigurableApprovalFlow;
                    }
                    #endregion


                }
            }

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(MembershipInformationViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;

            model.MembershipStatus = Convert.ToString(BEPZA_MEDICAL.Web.MvcApplication.CPFMembershipStatus.Active);
            var submit = Convert.ToString(BEPZA_MEDICAL.Web.MvcApplication.CPFApprovalStatus.Submitted);

            // TODO: Need to use enum instead of string comparison
            var approvalStatus = _prmCommonservice.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Submit")).FirstOrDefault();

            if (approvalStatus != null)
            {
                model.ApprovalStatusId = approvalStatus.Id;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var obj = model.ToEntity();
                    obj.IUser = User.Identity.Name;
                    obj.IDate = Common.CurrentDateTime;

                    errorList = String.Empty;

                    if (string.IsNullOrEmpty(errorList))
                    {

                        _cpfCommonservice.CPFUnit.MembershipInformationRepository.Add(obj);
                        _cpfCommonservice.CPFUnit.MembershipInformationRepository.SaveChanges();

                        _prmCommonservice.PRMUnit.FunctionRepository.InitializeApprovalProcess("CPFMbr", model.EmployeeCode, obj.Id, 1, (int)obj.ApprovedById, obj.IUser);

                        #region Old Code
                        /*
                        #region Approval Flow Setting-----------------------------
                        
                        CPF_ApprovalFlow objAppFlow = new CPF_ApprovalFlow();
                        objAppFlow.DocumentTypeId = docTypeId;
                        objAppFlow.DocumentId = memberApplication.Id;
                        objAppFlow.DocumentStatusId = docStatusId;
                        objAppFlow.ApprovalPathId = pathandDetail.PathId;
                        objAppFlow.ToStepId = pathandDetail.PathDetailId;
                        objAppFlow.FromAuthorId = obj.EmployeeId;

                        if (pathandDetail.MainAuthorId > 0)
                        {
                            objAppFlow.ToAuthorId = pathandDetail.MainAuthorId;
                        }

                        objAppFlow.StatusDate = DateTime.Now;
                        objAppFlow.Comments = "Please review and approve it.";

                        _cpfCommonservice.CPFUnit.ApprovalFlowRepository.Add(objAppFlow);
                        _cpfCommonservice.CPFUnit.LoanApplicationRepository.SaveChanges();
                        #endregion

                        #region Mail Sending-------------------------

                        var isSent = MembershipSubmitEmailNotification(model, pathandDetail.MainAuthorId, docStatusId, "Please review and approve it.");


                        #endregion
                        */
                        #endregion

                        model.IsError = 0;
                        
                    }
                    
                    else
                    {
                        model.ErrMsg = errorList;
                    }
                }
                catch (Exception ex)
                {
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
                model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(model.ErrMsg) ? ErrorMessages.InsertFailed : model.ErrMsg) : Common.GetModelStateError(ModelState);
            }

            PopulateDropDown(model);
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int employeeId)
        {
            MembershipInformationViewModel model = new MembershipInformationViewModel();

            CPF_MembershipInfo member = (from m in _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll() where m.EmployeeId == employeeId select m).LastOrDefault();
            model = member.ToModel();

            var membershipAppliedBy =
                _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(m => m.EmpID == member.IUser).FirstOrDefault();
            if (membershipAppliedBy != null)
            {
                var desigName = membershipAppliedBy.PRM_Designation != null
                    ? Common.GetString(membershipAppliedBy.PRM_Designation.ShortName)
                    : String.Empty;
                model.MembershipAppliedByInOffline = membershipAppliedBy.FullName + " (" + membershipAppliedBy.EmpID +
                                                     ") - " + desigName;
                model.MembershipAppliedDateInOffline = membershipAppliedBy.IDate.ToString(DateAndTime.GlobalDateFormat);
            }

            PopulateDropDown(model);
            return View(model);


        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(MembershipInformationViewModel model)
        {
            model.IsError = 0;
            string checkingMessage = string.Empty;
            if (ModelState.IsValid)
            {
                var memberApplication = _cpfCommonservice.CPFUnit.MembershipInformationRepository.Get(d => d.EmployeeId == model.EmployeeId).LastOrDefault();
                var master = model.ToEntity();

                ArrayList childList = new ArrayList();

                try
                {

                    Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();

                    _cpfCommonservice.CPFUnit.MembershipInformationRepository.Update(master, NavigationList);
                    _cpfCommonservice.CPFUnit.MembershipInformationRepository.SaveChanges();

                    #region Mail Sending Setting---------------------------------

                    var isSent = MembershipSubmitEmailNotification(model, 0, model.ApprovalStatusId, "Please review and approve it.");

                    //List<string> recepentList = new List<string>();
                    //var mailReceiveEmploye = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(d => d.Id == pathandDetail.MainAuthorId).FirstOrDefault();
                    //if (mailReceiveEmploye != null && mailReceiveEmploye.EmialAddress != null)
                    //{
                    //    recepentList.Add(mailReceiveEmploye.EmialAddress);
                    //    var emailconfigurationData = _pimCommonservice.PIMUnit.EmailConfigureDataRepository.GetAll().FirstOrDefault();
                    //    string[] w = new string[1];
                    //    Common.SendMail(recepentList,w, "Review Application", "Please Review may application", emailconfigurationData.ToModel());
                    //}
                    #endregion

                    // Path Flow --> end

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
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
                model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(model.ErrMsg) ? ErrorMessages.UpdateFailed : model.ErrMsg) : Common.GetModelStateError(ModelState);
            }

            model.Mode = CrudeAction.Edit;
            return View(model);
        }

        #endregion

        #region  Utilities
        [NoCache]
        public JsonResult GetEmployeeInfo(MembershipInformationViewModel model)
        {
            string errorList = string.Empty;

            if (string.IsNullOrEmpty(errorList))
            {
                try
                {
                    var emp = _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetByID(model.ApprovedById);

                    if (emp != null)
                    {
                        var empInfoPersonal = _prmCommonservice.PRMUnit.PersonalInfoRepository.Get(d => d.EmployeeId == emp.Id).FirstOrDefault();
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
                            Designation = emp.PRM_Designation.Name,
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

        private MembershipInformationViewModel PopulateDropDown(MembershipInformationViewModel model)
        {
            var bankList = _prmCommonservice.PRMUnit.BankNameRepository.GetAll();
            var bankBranchList = _prmCommonservice.PRMUnit.BankBranchRepository.Get();

            model.BankList = Common.PopulateDllList(bankList);
            model.BankBranchList = Common.PopulateDllList(bankBranchList);

            var approverList = _prmCommonservice.PRMUnit.FunctionRepository.GetApproverByApplicant(model.EmployeeCode, "CPFMbr");
            model.ApproverList = Common.PopulateEmployeeDDL(approverList);

            return model;
        }

        private MembershipInformationViewModel GetApplicantInfo(string EmpId)
        {
            MembershipInformationViewModel viewData = new MembershipInformationViewModel();
            var empInfo = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(d => d.EmpID == EmpId).FirstOrDefault();
            decimal basicSalary = 0;

            if (empInfo != null)
            {
                var empInfoPersonal = _prmCommonservice.PRMUnit.PersonalInfoRepository.Get(d => d.EmployeeId == empInfo.Id).FirstOrDefault();
                if (empInfoPersonal != null)
                {
                    var salaryInfo = _pgmCommonservice.GetBasicSalaryByEmployeeId(empInfoPersonal.EmployeeId);

                    if (salaryInfo != null)
                    {
                        decimal.TryParse(salaryInfo.Basic.ToString(), out basicSalary);
                    }
                }

                viewData.EmployeeCode = empInfo.EmpID;
                viewData.EmployeeId = empInfo.Id;
                viewData.EmployeeName = empInfo.FullName;
                viewData.DepartmentName = empInfo.PRM_Discipline != null ? empInfo.PRM_Discipline.Name : string.Empty;
                viewData.DesignationName = empInfo.PRM_Designation.Name;
                viewData.SectionName = empInfo.PRM_Section != null ? empInfo.PRM_Section.Name : string.Empty;

                viewData.EmployeeInitial = empInfo.EmployeeInitial;
                viewData.JoiningDate = empInfo.DateofJoining;
                viewData.PermanentDate = empInfo.DateofConfirmation != null ? Convert.ToDateTime(empInfo.DateofConfirmation) : DateTime.Now;


                viewData.DateOfBirth = empInfo.DateofBirth;
                viewData.EmployeeCategory = empInfo.PRM_StaffCategory != null ? empInfo.PRM_StaffCategory.Name : string.Empty;
                viewData.PresentPayScale = empInfo.PRM_SalaryScale != null ? empInfo.PRM_SalaryScale.SalaryScaleName : string.Empty;
                viewData.OfficeName = empInfo.PRM_JobLocation != null ? empInfo.PRM_JobLocation.Name : string.Empty;
                viewData.Nationality = empInfoPersonal != null ? empInfoPersonal.PRM_Nationality.Name : string.Empty;

                viewData.PresentBasicPay = basicSalary;
                viewData.PresentAddress = empInfoPersonal != null ? empInfoPersonal.PresentAddress1 : string.Empty;
                viewData.PermanentAddress = empInfoPersonal != null ? empInfoPersonal.PermanentAddress1 : string.Empty;
                viewData.MembershipID = empInfo.EmpID;
                //viewData.PermanentDate = empInfo.DateofConfirmation;
            }
            return viewData;
        }

        private bool MembershipSubmitEmailNotification(MembershipInformationViewModel model, Int32 ToEmpId, Int32 ActionTypeId, String Comments)
        {
            bool isSent = false;

            var toEmail = (from tr in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll()
                           where tr.Id == ToEmpId
                           select tr.EmialAddress).FirstOrDefault();

            if (string.IsNullOrEmpty(toEmail) == false)
            {
                try
                {
                    var submittedBy = (from tr in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll()
                                       where tr.Id == model.EmployeeId
                                       select tr).FirstOrDefault();

                    var MemberInfo = (from tr in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll()
                                      where tr.Id == model.EmployeeId
                                      select tr).FirstOrDefault();

                    var Division = (from tr in _prmCommonservice.PRMUnit.DivisionRepository.GetAll()
                                    where tr.Id == MemberInfo.DivisionId
                                    select tr.Name).FirstOrDefault();

                    var Designation = (from tr in _prmCommonservice.PRMUnit.DesignationRepository.GetAll()
                                       where tr.Id == MemberInfo.DesignationId
                                       select tr.Name).FirstOrDefault();

                    var smtp = WebConfigurationManager.AppSettings["smtp"];

                    string Fromemail = WebConfigurationManager.AppSettings["FromEmail"];
                    string password = WebConfigurationManager.AppSettings["FromEmailPassword"];

                    string url = WebConfigurationManager.AppSettings["url"];

                    var mailbody = "<html><h3>CPF Membership Form</h3><body>Initial: " + MemberInfo.EmployeeInitial + ", Employee Name: " + MemberInfo.FullName + ", Designation: " + Designation + ", Division: " + Division + "<br>Date of Joining: " + MemberInfo.DateofJoining.ToString("dd/MM/yyyy") + ", Date of Confirmation: " + Convert.ToDateTime(MemberInfo.DateofConfirmation).ToString("dd/MM/yyyy") + "<br>Submitted By: " + submittedBy.EmployeeInitial + ", Date of Submission: " + System.DateTime.Now.ToString("dd/MM/yyyy") + "";
                    mailbody = mailbody + "<br>Comments: " + Comments + "<br><b>URL:" + url + "<b></body></html>";

                    var subject = "New CPF membership form (Initial :" + MemberInfo.EmployeeInitial + ") is arrived for your review";

                    isSent = _cpfCommonservice.SendEmail(smtp, Fromemail, password, toEmail, subject, mailbody);

                    return isSent;
                }
                catch (Exception ex)
                {
                    var errmsg = ex.Message;

                    return isSent;
                }
            }

            return isSent;
        }

        #endregion
    }
}
