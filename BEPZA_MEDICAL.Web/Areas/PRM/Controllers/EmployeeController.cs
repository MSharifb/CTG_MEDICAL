using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.Collections;
using System.IO;
using System.Web.Helpers;
using System.Data;
using System.Text;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using System.Web.Configuration;
using BEPZA_MEDICAL.DAL.PGM;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PGM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Employee;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.SecurityService;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Areas.PGM.Models.SalaryStructure;
using BEPZA_MEDICAL.Web.Controllers;
using PRM_EmpSalary = BEPZA_MEDICAL.DAL.PRM.PRM_EmpSalary;
using PRM_EmpSalaryDetail = BEPZA_MEDICAL.DAL.PRM.PRM_EmpSalaryDetail;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class EmployeeController : BaseController
    {
        #region Fields

        private readonly EmployeeService _empService;
        private readonly PGMCommonService _pgmCommonservice;
        private UserManagementServiceClient _userAgent;
        private readonly JobGradeService _JobGradeService;
        private readonly SalaryStructureService _salaryStructureService;

        #endregion

        #region Constructor
        public EmployeeController(EmployeeService empService, PGMCommonService pgmCommonservice, JobGradeService JobGradeService, SalaryStructureService salaryStructureService)
        {
            this._empService = empService;
            this._pgmCommonservice = pgmCommonservice;
            this._JobGradeService = JobGradeService;
            _userAgent = new UserManagementServiceClient();
            this._salaryStructureService = salaryStructureService;
        }
        #endregion

        #region Actions

        #region Search

        public ActionResult Index()
        {
            var model = new EmployeeSearchViewModel();
            model.ZoneInfoId = LoggedUserZoneInfoId;
            model.ActionName = "EmploymentInfoIndex";

            var indexView = View(model);
            indexView.MasterName = "~/Areas/PRM/Views/Shared/_Layout.cshtml";

            return indexView;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, EmployeeSearchViewModel viewModel, string st)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            if (string.IsNullOrEmpty(st))
                st = "";
            viewModel.ZoneInfoId = LoggedUserZoneInfoId;
            //LoginEmpId = Common.CheckPermission("PRM");

            #region User Own Info
            var groupList = _userAgent.GetGroupList();
            var userInfo = _userAgent.GetUserByLoginId(MyAppSession.User.LoginId);
            var groupName = groupList.Where(s => s.GroupId == userInfo.GroupId).Select(x => x.GroupName).FirstOrDefault();

            if (groupName.Contains("Admin"))
            {
                viewModel.EmpId = viewModel.EmpId;
            }
            else
            {
                viewModel.EmpId = MyAppSession.EmpId;
            }
            #endregion

            var list = _empService.GetPaged(
                 filterExpression.ToString(),
                 request.SortingName,
                 request.SortingOrder.ToString(),
                 request.PageIndex,
                 request.RecordsCount,
                 request.PagesCount.HasValue ? request.PagesCount.Value : 1,

                 viewModel.EmpId,
                 viewModel.EmpName,
                 viewModel.DesigName,
                 viewModel.EmpTypeId,
                 viewModel.DivisionName,
                 viewModel.JobLocName,
                 viewModel.GradeName,
                 viewModel.StaffCategoryId,
                //viewModel.ResourceLevelId,
                 viewModel.OrganogramLevelId,
                 viewModel.ZoneInfoId,
                 st.Equals("active") ? 1 : viewModel.EmployeeStatus,

                 out totalRecords

                 //LoginEmpId
                 );

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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetListForPGM(JqGridRequest request, EmployeeSearchViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            viewModel.ZoneInfoId = LoggedUserZoneInfoId;

            var empStatusFormData = form["EmployeeStatus"];
            if (string.IsNullOrEmpty(empStatusFormData))
                empStatusFormData = "";

            var isBonusEligibleFormData = form["IsBonusEligible"];
            var isOvertimeEligibleFormData = form["IsOvertimeEligible"];
            var isRefreshmentEligibleFormData = form["IsRefreshmentEligible"];


            if (request.Searching)
            {
                if (viewModel != null)
                {
                    filterExpression = viewModel.GetFilterExpression(empStatusFormData, isBonusEligibleFormData,
                        isOvertimeEligibleFormData, isRefreshmentEligibleFormData);
                }
            }


            var list = _empService.GetPagedForPGM(
                filterExpression,
                request.SortingName,
                request.SortingOrder.ToString(),
                request.PageIndex,
                request.RecordsCount,
                request.PagesCount.HasValue ? request.PagesCount.Value : 1,

                viewModel.EmpId,
                viewModel.EmpName,
                viewModel.DesigName,
                viewModel.EmpTypeId,
                viewModel.DivisionName,
                viewModel.JobLocName,
                viewModel.GradeName,
                viewModel.StaffCategoryId,
                viewModel.OrganogramLevelId,
                viewModel.ZoneInfoId,
                empStatusFormData.Equals("active") ? 1 : viewModel.EmployeeStatus,

                out totalRecords,
                LoggedUserZoneInfoId
                );

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
                    item.DesigName,
                    item.DivisionName,
                    item.JobLocName,
                    item.GradeName,                     
                    item.DateOfJoining.ToString(DateAndTime.GlobalDateFormat),
                    item.DateOfConfirmation.HasValue ? item.DateOfConfirmation.Value.ToString(DateAndTime.GlobalDateFormat) : null,
                    item.EmpTypeName,
                    item.StaffCatName,
                    item.EmpStatusName,
                    item.OrganogramLevelId,
                    item.IsBonusEligible,
                    item.IsOvertimeEligible,
                    item.IsRefreshmentEligible
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetActiveEmployeeList(JqGridRequest request, EmployeeSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _empService.GetActivePaged(
                filterExpression.ToString(),
                request.SortingName,
                request.SortingOrder.ToString(),
                request.PageIndex,
                request.RecordsCount,
                request.PagesCount.HasValue ? request.PagesCount.Value : 1,

                viewModel.EmpId,
                viewModel.EmpName,
                viewModel.DesigName,
                viewModel.EmpTypeId,
                viewModel.DivisionName,
                viewModel.JobLocName,
                viewModel.GradeName,
                viewModel.StaffCategoryId,
                //viewModel.ResourceLevelId,
                viewModel.EmployeeStatus,
                viewModel.SelectedEmployeeStatus,
                out totalRecords
                );

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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetInactiveEmployeeList(JqGridRequest request, EmployeeSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            dynamic empList = 0;

            //get inactive employee status start
            var empStatus = _empService.PRMUnit.EmploymentStatusRepository.Get(q => q.Name.ToLower() == "inactive").FirstOrDefault();
            if (empStatus != null)
            {
                viewModel.EmployeeStatus = empStatus.Id;
                viewModel.SelectedEmployeeStatus = empStatus.Id;
                ViewBag.SearchEmpType = empStatus.Name.ToLower();
            }
            //end

            var list = _empService.GetInactivePaged(
                filterExpression.ToString(),
                request.SortingName,
                request.SortingOrder.ToString(),
                request.PageIndex,
                request.RecordsCount,
                request.PagesCount.HasValue ? request.PagesCount.Value : 1,

                viewModel.EmpId,
                viewModel.EmpName,
                viewModel.DesigName,
                viewModel.EmpTypeId,
                viewModel.DivisionName,
                viewModel.JobLocName,
                viewModel.GradeName,
                viewModel.StaffCategoryId,
                //viewModel.ResourceLevelId,
                viewModel.EmployeeStatus,
                viewModel.SelectedEmployeeStatus,
                out totalRecords
                );

            var finalSettlementList = _pgmCommonservice.PGMUnit.FinalSettlementRepository.GetAll().ToList();

            if (viewModel.SelectedEmployeeStatus == 2)
            {
                if (finalSettlementList.Count > 0)
                {
                    list = list.Where(a => !finalSettlementList.Any(b => b.EmployeeId == a.ID)).ToList();
                    JqGridResponse response1 = new JqGridResponse()
                    {
                        TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                        PageIndex = request.PageIndex,
                        TotalRecordsCount = list.Count
                    };

                    foreach (var item in list)
                    {
                        response1.Records.Add(new JqGridRecord(Convert.ToString(item.ID), new List<object>()
                {
                    item.EmpName,
                    item.ID,
                    item.EmpId,                  
                    item.DesigName,
                    item.DivisionName,
                    item.JobLocName,
                    item.GradeName,                     
                    item.DateOfJoining.ToString(DateAndTime.GlobalDateFormat),
                    item.DateOfConfirmation.HasValue ? item.DateOfConfirmation.Value.ToString(DateAndTime.GlobalDateFormat) : null,

                }));
                    }
                    return new JqGridJsonResult() { Data = response1 };

                }
            }
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEmpList(JqGridRequest request, EmployeeSearchViewModel viewModel, int EmployeeStatus)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            viewModel.ZoneInfoId = LoggedUserZoneInfoId;

            var list = _empService.GetPaged(
                filterExpression.ToString(),
                request.SortingName,
                request.SortingOrder.ToString(),
                request.PageIndex,
                request.RecordsCount,
                request.PagesCount.HasValue ? request.PagesCount.Value : 1,

                viewModel.EmpId,
                viewModel.EmpName,
                viewModel.DesigName,
                viewModel.EmpTypeId,
                viewModel.DivisionName,
                viewModel.JobLocName,
                viewModel.GradeName,
                viewModel.StaffCategoryId,
                //viewModel.ResourceLevelId,
                viewModel.OrganogramLevelId,
                viewModel.ZoneInfoId,
                EmployeeStatus,
                out totalRecords
                );

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

        public ActionResult EmployeeSearch(string searchEmpType)
        {
            var model = new EmployeeSearchViewModel();
            model.SearchEmpType = searchEmpType;
            model.UseTypeEmpId = "1";
            return View("EmployeeSearch", model);
        }

        public ActionResult EmployeeSearchForPGM(string searchEmpType)
        {
            var model = new EmployeeSearchViewModel();
            model.SearchEmpType = searchEmpType;
            return View("EmployeeSearchForPGM", model);
        }

        public ActionResult ActiveEmployeeSearchList()
        {
            var model = new EmployeeSearchViewModel();
            return View("ActiveEmployeeSearch", model);
        }

        public ActionResult InActiveEmployeeSearchList()
        {
            var model = new EmployeeSearchViewModel();
            return View("InactiveEmployeeSearch", model);
        }

        /// <summary>
        /// new employee popup list for multiple use at at time 
        /// </summary>
        /// <param name="searchEmpType"></param>
        /// <returns></returns>
        public ActionResult EmployeeSearchTwo(string UseTypeEmpId)
        {
            var model = new EmployeeSearchViewModel();
            model.SearchEmpType = "active";
            model.UseTypeEmpId = UseTypeEmpId;
            return View("EmployeeSearchTwo", model);
        }

        #endregion

        #region Employee Basic Information

        #region Insert-----------------------------------

        public ActionResult EmploymentInfoIndex(int? id)
        {
            if (id.HasValue)
                return RedirectToAction("EditEmploymentInfo", "Employee", new { id = id });

            var parentModel = new EmployeeViewModel();
            parentModel.ViewType = "EmploymentInfo";

            populateDropdown(parentModel.EmploymentInfo);

            parentModel.EmploymentInfo.EmpID = _empService.GetNewEmployeeID();

            //Initially Active
            parentModel.EmploymentInfo.EmploymentStatusId = 1;
            parentModel.EmploymentInfo.EmploymentStatusName = _empService.PRMUnit.EmploymentStatusRepository.GetByID(parentModel.EmploymentInfo.EmploymentStatusId).Name;

            //Job Grade List
            parentModel.EmploymentInfo.IsSalaryStructureProcess = false;

            parentModel.EmploymentInfo.ActionType = "CreateEmploymentInfo";
            parentModel.EmploymentInfo.ButtonText = "Save";
            parentModel.EmploymentInfo.SelectedClass = "active";

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateEmploymentInfo(EmploymentInfoViewModel model)
        {
            var parentModel = new EmployeeViewModel();
            var employeeContractInfo = new PRM_EmpContractInfo();
            parentModel.ViewType = "EmploymentInfo";

            var error = CheckEmpInfoBusinessRule(model);

            if (ModelState.IsValid && error == string.Empty)
            {
                try
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = model.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;

                    //entity.EmpID = _empService.GetNewEmployeeID();
                    //entity.EmployeeInitial = model.EmployeeInitial.Trim().ToUpper();

                    _empService.PRMUnit.EmploymentInfoRepository.Add(entity);
                    _empService.PRMUnit.EmploymentInfoRepository.SaveChanges();

                    if (model.IsContractual == true)
                    {
                        employeeContractInfo.EmpoyeeId = entity.Id;
                        employeeContractInfo.ContractStartDate = model.DateofJoining;
                        employeeContractInfo.ContractEndDate = Convert.ToDateTime(model.ContractExpireDate);
                        employeeContractInfo.isExtension = false;
                        employeeContractInfo.IUser = User.Identity.Name;
                        employeeContractInfo.IDate = Common.CurrentDateTime;

                        _empService.PRMUnit.EmploymentContractInfoRepository.Add(employeeContractInfo);
                        _empService.PRMUnit.EmploymentContractInfoRepository.SaveChanges();
                    }
                    parentModel.Id = entity.Id;
                    parentModel.EmpId = entity.EmpID;

                    #region create user to security module
                    /*
                    // user will create for active employee
                    if (model.EmploymentStatusId == 1)
                    {

                        User objUser = new User();
                        objUser.LoginId = model.EmpID;

                        // generate rabdom password
                        string randomPass = GeneratePassword(3, 2, 3);

                        // encrypt the password
                        string pass = getMd5Hash(randomPass);

                        // assign pass word
                        objUser.Password = pass;

                        objUser.FirstName = string.Empty;
                        objUser.LastName = model.FullName;
                        objUser.EmailAddress = model.EmialAddress;
                        objUser.Phone = model.MobileNo;
                        objUser.NeverExperied = false;
                        objUser.CreatedBy = User.Identity.Name;
                        objUser.CreatedDate = DateTime.Now;

                        string usergroup = WebConfigurationManager.AppSettings["DefaultUserGroup"].ToLower();

                        objUser.GroupId = _userAgent.GetGroupList().Where(g => g.GroupName.ToLower().Contains(usergroup)).FirstOrDefault().GroupId;
                        objUser.EmpId = model.EmpID;
                        objUser.Status = true;

                        _userAgent.InsertUserData(objUser);

                        if (string.IsNullOrEmpty(model.EmialAddress) == false)
                        {
                            try
                            {
                                string url = WebConfigurationManager.AppSettings["url"];
                                var mailbody = "<html><h3>Your Credential for System</h3><body>User ID:<b>" + model.EmployeeInitial + "<b><br>, Password:<b>" + randomPass + "<b><br> Please chnage your password after login.<br><b>URL:" + url + "<b></body></html>";
                                // send email when email address is available
                                SendEmail(model.EmialAddress, "User Credential", mailbody);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    */
                    #endregion
                }
                catch (Exception ex)
                {
                    populateDropdown(model);

                    parentModel.EmploymentInfo = model;
                    parentModel.EmploymentInfo.ButtonText = "Save";
                    parentModel.EmploymentInfo.SelectedClass = "active";
                    parentModel.EmploymentInfo.ErrorClass = "failed";
                    parentModel.EmploymentInfo.Message = Resources.ErrorMessages.InsertFailed;
                    // parentModel.EmploymentInfo.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                    parentModel.EmploymentInfo.IsError = 1;
                    InitializationJobGradeAndDesignationForEdit(model);

                    return View("CreateOrEdit", parentModel);
                }
            }
            else
            {
                populateDropdown(model);

                parentModel.EmploymentInfo = model;
                parentModel.EmploymentInfo.ButtonText = "Save";
                parentModel.EmploymentInfo.SelectedClass = "active";
                parentModel.EmploymentInfo.ErrorClass = "failed";
                parentModel.EmploymentInfo.Message = Resources.ErrorMessages.InsertFailed + " " + error;
                parentModel.EmploymentInfo.IsError = 1;
                InitializationJobGradeAndDesignationForEdit(model);
                return View("CreateOrEdit", parentModel);
            }

            return RedirectToAction("EditEmploymentInfo", "Employee", new { id = parentModel.Id, type = "success" });
        }


        //private void SetDynamicLabel(EmploymentInfoViewModel model)
        //{
        //    var objLabel = _empService.OrganogramDynamicLabel();

        //    foreach (var item in objLabel)
        //    {
        //        if (item.TableIdName.Equals(PRMEnum.EmployeeOrganogramDynamicLabel.DivisionId.ToString()))
        //        {
        //            model.DivisionNameForLabel = item.OrganogramTypeName;
        //        }
        //        else if (item.TableIdName.Equals(PRMEnum.EmployeeOrganogramDynamicLabel.DisciplineId.ToString()))
        //        {
        //            model.DisciplineForLabel = item.OrganogramTypeName;
        //        }
        //        else if (item.TableIdName.Equals(PRMEnum.EmployeeOrganogramDynamicLabel.SectionId.ToString()))
        //        {
        //            model.SectionNameForLabel = item.OrganogramTypeName;
        //        }
        //        else if (item.TableIdName.Equals(PRMEnum.EmployeeOrganogramDynamicLabel.SubSectionId.ToString()))
        //        {
        //            model.SubSectionNameForLabel = item.OrganogramTypeName;
        //        }
        //    }
        //}

        #endregion

        #region Update--------------------------------------

        public ActionResult EditEmploymentInfo(int id, string type)
        {
            var parentModel = new EmployeeViewModel();
            parentModel.ViewType = "EmploymentInfo";

            var entity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(id);
            var model = entity.ToModel();
            model.OrganogramLevelName = entity.PRM_OrganogramLevel == null ? String.Empty : entity.PRM_OrganogramLevel.LevelName;
            model.DivisionName = entity.PRM_Division == null ? string.Empty : entity.PRM_Division.Name;
            model.DisciplineName = entity.PRM_Discipline == null ? string.Empty : entity.PRM_Discipline.Name;
            model.SectionName = entity.PRM_Section == null ? string.Empty : entity.PRM_Section.Name;
            model.SubSectionName = entity.PRM_SubSection == null ? string.Empty : entity.PRM_SubSection.Name;
            model.JobGradeName = entity.PRM_JobGrade == null ? string.Empty : entity.PRM_JobGrade.GradeName;
            model.SalaryScaleName = entity.PRM_SalaryScale == null ? string.Empty : entity.PRM_SalaryScale.SalaryScaleName;


            #region Job Grade ddl Or Textbox
            var salaryProcess = _empService.PRMUnit.EmpSalaryRepository.GetAll().Where(x => x.EmployeeId == id).ToList();
            if (salaryProcess.Count > 0)
            {
                model.IsSalaryStructureProcess = true;
            }
            else
            {
                model.IsSalaryStructureProcess = false;
            }
            #endregion

            if (model.DesignationId.HasValue)
            {
                model.IsEmpEditDesignation = true;
            }

            model.DesignationName = entity.PRM_Designation == null ? string.Empty : entity.PRM_Designation.Name;

            if (model.DateofInactive.HasValue)
            {
                model.DateofInactive = model.DateofInactive.Value.Date;
            }

            model.EmployeeClassName = entity.PRM_EmployeeClass == null ? string.Empty : entity.PRM_EmployeeClass.Name;

            populateDropdown(model);

            var salaryExisting = _empService.PRMUnit.EmpSalaryRepository.Get(x => x.EmployeeId == id).FirstOrDefault();
            if (salaryExisting != null)
            {
                model.isExist = true;
            }

            InitializationJobGradeAndDesignationForEdit(model);

            model.ActionType = "EditEmploymentInfo";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "active";

            parentModel.EmploymentInfo = model;
            parentModel.EmpId = model.EmpID;
            parentModel.Id = model.Id;

            parentModel.EmploymentInfo.EmploymentStatusName = _empService.PRMUnit.EmploymentStatusRepository.GetByID(model.EmploymentStatusId).Name;

            model.IsPhotoExist = Common.IsPhotoExist(model.Id, true, _empService);

            if (type == "success")
            {
                parentModel.EmploymentInfo.Message = Resources.ErrorMessages.InsertSuccessful;
                parentModel.EmploymentInfo.ErrorClass = "success";
                parentModel.EmploymentInfo.IsError = 0;
            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EditEmploymentInfo(EmploymentInfoViewModel model, FormCollection form)
        {
            var parentModel = new EmployeeViewModel();
            parentModel.ViewType = "EmploymentInfo";

            /// Is salary exist
            var salaryExisting = _empService.PRMUnit.EmpSalaryRepository.Get(x => x.EmployeeId == model.Id).FirstOrDefault();
            if (salaryExisting != null)
            {
                model.isExist = true;
            }

            #region Job Grade ddl Or Textbox
            var salaryProcess = _empService.PRMUnit.EmpSalaryRepository.GetAll().Where(x => x.EmployeeId == model.Id).ToList();
            if (salaryProcess.Count > 0)
            {
                model.IsSalaryStructureProcess = true;
            }
            else
            {
                model.IsSalaryStructureProcess = false;
            }
            #endregion


            model.IsPhotoExist = Common.IsPhotoExist(model.Id, true, _empService);

            var error = CheckEmpInfoBusinessRule(model);

            if (ModelState.IsValid && error == string.Empty)
            {
                try
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;
                    //entity.EmployeeInitial = model.EmployeeInitial.Trim().ToUpper();                   
                    _empService.PRMUnit.EmploymentInfoRepository.Update(entity, new Dictionary<Type, ArrayList>());
                    _empService.PRMUnit.EmploymentInfoRepository.SaveChanges();
                }
                catch (Exception ex)
                {
                    populateDropdown(model);
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "active";

                    parentModel.EmploymentInfo = model;
                    InitializationJobGradeAndDesignationForEdit(model);
                    return View("CreateOrEdit", parentModel);
                }

                populateDropdown(model);
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "active";

                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;

                parentModel.EmploymentInfo = model;
                parentModel.Id = model.Id;
                parentModel.EmpId = model.EmpID;

                InitializationJobGradeAndDesignationForEdit(model);

                if (model.DesignationId.HasValue)
                {
                    model.IsEmpEditDesignation = true;
                }

                #region Label Name from Database

                //SetDynamicLabel(model);

                #endregion

                return View("CreateOrEdit", parentModel);

                //return RedirectToAction("Index");
            }

            populateDropdown(model);
            //ModelState.AddModelError("updateFailed", "update failed !!");
            model.Message = Resources.ErrorMessages.UpdateFailed + " " + error;
            model.ErrorClass = "failed";
            model.IsError = 1;

            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "active";

            parentModel.EmploymentInfo = model;

            InitializationJobGradeAndDesignationForEdit(model);

            return View("CreateOrEdit", parentModel);
        }

        private EmploymentInfoViewModel InitializationJobGradeAndDesignationForEdit(EmploymentInfoViewModel model)
        {
            var desigList = (from JG in _empService.PRMUnit.OrganizationalSetupManpowerInfoRepository.Fetch()
                             join de in _empService.PRMUnit.DesignationRepository.Fetch() on JG.DesignationId equals de.Id
                             where JG.OrganogramLevelId == model.OrganogramLevelId
                             select de).OrderBy(o => o.Name).ToList();

            model.DesignationList = Common.PopulateDllList(desigList);

            return model;
        }

        #endregion

        #region Delete

        public ActionResult Delete(int id)
        {
            var parentModel = new EmployeeViewModel();
            parentModel.ViewType = "EmploymentInfo";

            var entity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(id);
            var model = entity.ToModel();
            if (ModelState.IsValid)
            {
                try
                {
                    _empService.PRMUnit.EmploymentContractInfoRepository.Delete(x => x.EmpoyeeId == id);
                    _empService.PRMUnit.EmploymentContractInfoRepository.SaveChanges();

                    _empService.PRMUnit.EmploymentInfoRepository.Delete(entity);
                    _empService.PRMUnit.EmploymentInfoRepository.SaveChanges();
                }
                catch (Exception ex)
                {

                    populateDropdown(model);
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.IsError = 1;
                    model.DeleteEnable = true;
                    model.ButtonText = "Update";

                    parentModel.EmploymentInfo = model;
                    return View("CreateOrEdit", parentModel);
                }
                //var searchModel = new EmployeeSearchViewModel();
                //searchModel.Message = Resources.ErrorMessages.DeleteSuccessful;
                //searchModel.ActionName = "EditEmploymentInfo";
                //return View("Index", searchModel);

                model.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.IsError = 0;
                model.ErrorClass = "success delete-emp";

                parentModel.EmploymentInfo = model;
                return View("CreateOrEdit", parentModel);
                //return RedirectToAction("Index");
            }

            model = entity.ToModel();
            populateDropdown(model);
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.IsError = 1;

            parentModel.EmploymentInfo = model;
            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region

        //[NoCache]
        //public JsonResult GetEmployeeDeptOfficeSecInfo(int organogramLevelId)
        //{
        //    var obj = _empService.GetEmployeeDeptOfficeSecInfoByOrgogramId(organogramLevelId);

        //    return Json(new
        //    {
        //        Id = organogramLevelId,
        //        DepartmentName = obj.DepartmentName == null ? string.Empty : obj.DepartmentName,
        //        OfficeName = obj.OfficeName == null ? string.Empty : obj.OfficeName,
        //        SectionName = obj.SectionName == null ? string.Empty : obj.SectionName,
        //        SubSectionName = obj.SubSectionName == null ? string.Empty : obj.SubSectionName
        //    });
        //}

        private string GenerateOrganogramDetail(BEPZA_MEDICAL.Domain.PRM.EmployeeService.OrganogramSearch orgSearch)
        {
            string lbl = "";
            string sep = " | ";

            if (!String.IsNullOrEmpty(orgSearch.DepartmentName)) lbl += "Dept: " + orgSearch.DepartmentName;
            if (!String.IsNullOrEmpty(orgSearch.OfficeName)) lbl += sep + "Office: " + orgSearch.OfficeName;
            if (!String.IsNullOrEmpty(orgSearch.SectionName)) lbl += sep + "Section: " + orgSearch.SectionName;
            if (!String.IsNullOrEmpty(orgSearch.SubSectionName)) lbl += sep + "Sub Section: " + orgSearch.SubSectionName;

            return lbl;
        }

        //[NoCache]
        //public JsonResult GetOrganogramHierarchyInfo(int organogramLevelId)
        //{
        //    var label = _empService.GetOrganogramHierarchyLabel(organogramLevelId);

        //    return Json(new
        //    {
        //        OrgLabel = label
        //    });
        //}

        #endregion

        #endregion

        #region Employment Contract Extension

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEmploymentContractInfoList(JqGridRequest request, int empId)
        {
            var list = _empService.GetEmpContractByEmpId(empId);

            var totalRecords = list.Count();

            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {
                int isEditable = 1;
                var lst = list.Where(q => q.EmpoyeeId == d.EmpoyeeId).ToList();
                if (lst.Count > 1)
                {
                    var item = lst.OrderByDescending(q => q.ContractEndDate).ThenByDescending(y => y.Id).FirstOrDefault();
                    isEditable = item.Id == d.Id ? 1 : 0;
                }

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    "View",
                    d.Id,
                    d.ContractStartDate.ToString(DateAndTime.GlobalDateFormat),
                    d.ContractEndDate.ToString(DateAndTime.GlobalDateFormat),

                    d.isExtension==false?"No":"Yes",
                    d.Remarks,
                    isEditable
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        #region Insert

        public ActionResult EmploymentContractPeriodIndex(int id, string type)
        {
            var parentModel = new EmployeeViewModel();
            parentModel.ViewType = "EmploymentContractInfo";

            var employee = _empService.PRMUnit.EmploymentInfoRepository.GetByID(id);

            var model = parentModel.EmploymentContractInfo;

            var entityList = _empService.PRMUnit.EmploymentContractInfoRepository.Get(q => q.EmpoyeeId == id).OrderByDescending(q => q.ContractEndDate).ToList();
            model.ActionType = "CreateEmploymentContractPeriod";
            model.ButtonText = "Save";
            model.EmpCode = employee.EmpID;
            model.EmpoyeeId = employee.Id;
            model.EmployeeInitial = employee.EmployeeInitial;
            model.FullName = employee.FullName;
            model.DateofInactive = employee.DateofInactive;
            model.IsContractual = employee.IsContractual;
            if (entityList.Count > 0)
            {
                model.isExtension = true;
                //model.ContractStartDate = entityList.FirstOrDefault().ContractEndDate.AddDays(1);
                //model.ContractEndDate = null;

                var prmEmpContractInfo = entityList.FirstOrDefault();
                if (prmEmpContractInfo != null)
                    model.ContractStartDate = prmEmpContractInfo.ContractEndDate.AddDays(1);
                model.ContractEndDate = null;
            }
            else
            {
                /// Add Provision Month to joining date and calculate ContractEndDate
                model.ContractStartDate = employee.DateofJoining;
                //model.ContractEndDate = employee.ProvisionMonth > 0 ? employee.DateofJoining.AddMonths(employee.ProvisionMonth) : employee.DateofJoining.AddMonths(0);
                if (model.IsContractual)
                {
                    model.ContractEndDate = employee.ContractExpireDate != null ? Convert.ToDateTime(employee.ContractExpireDate) : DateTime.Now;
                }
                else
                {
                    model.ContractEndDate = null;
                }
            }

            model.SelectedClass = "active";

            parentModel.EmploymentContractInfo = model;
            parentModel.Id = employee.Id;
            parentModel.EmpId = employee.EmpID;

            if (type == "success")
            {
                parentModel.EmploymentContractInfo.Message = TempData["SuccessMessage"] != null ? Convert.ToString(TempData["SuccessMessage"]) : string.Empty;
                parentModel.EmploymentContractInfo.ErrorClass = "success";
                parentModel.EmploymentContractInfo.IsError = 0;
            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateEmploymentContractPeriod([Bind(Exclude = "Attachment")]EmploymentContractPeriodViewModel model)
        {
            var parentModel = new EmployeeViewModel();
            parentModel.ViewType = "EmploymentContractInfo";
            var error = CheckEmpContractBusinessRule(model, "add");

            if (ModelState.IsValid && error == string.Empty)
            {
                try
                {
                    var entity = model.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;
                    entity.EmpoyeeId = model.EmpoyeeId;

                    #region Update Contract Extension

                    var employee = _empService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmpoyeeId);
                    employee.ContractExpireDate = model.ContractEndDate;

                    int d1 = Convert.ToDateTime(employee.DateofJoining).Day;
                    int d2 = Convert.ToDateTime(employee.ContractExpireDate).Day;
                    int d3 = Math.Abs(d2 - d1);
                    var duration = DateAndTime.DateDiff(DateAndTime.DateInterval.Month, Convert.ToDateTime(employee.DateofJoining), Convert.ToDateTime(employee.ContractExpireDate));
                    duration += d3 / 30;
                    employee.ContractDuration = duration;

                    #endregion

                    _empService.PRMUnit.EmploymentContractInfoRepository.Add(entity);
                    _empService.PRMUnit.EmploymentContractInfoRepository.SaveChanges();

                    #region Upload files

                    HttpFileCollectionBase files = Request.Files;
                    foreach (string fileTagName in files)
                    {

                        HttpPostedFileBase file = Request.Files[fileTagName];
                        if (file.ContentLength > 0)
                        {
                            int size = file.ContentLength;
                            string originalName = file.FileName;
                            int position = originalName.LastIndexOf("\\");
                            originalName = originalName.Substring(position + 1);
                            string contentType = file.ContentType;
                            byte[] fileData = new byte[size];
                            file.InputStream.Read(fileData, 0, size);

                            var usrFileName = (from dd in model.AttachmentFilesList.Where(x => x.OriginalName == originalName) select dd.UserFileName).FirstOrDefault();
                            EmpContractFileUtl.SaveFile(entity.Id, usrFileName, originalName, contentType, size, fileData);
                        }
                    }

                    #endregion

                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                    model.ActionType = "CreateEmploymentContractPeriod";
                    model.ButtonText = "Save";
                    model.SelectedClass = "active";
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    #region Getting Start date

                    var employee = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmpoyeeId);
                    var entityList = _empService.PRMUnit.EmploymentContractInfoRepository.Get(q => q.EmpoyeeId == model.EmpoyeeId).OrderByDescending(q => q.ContractEndDate).ToList();
                    if (entityList.Count > 0)
                    {
                        model.isExtension = true;
                        model.ContractStartDate = entityList.FirstOrDefault().ContractEndDate.AddDays(1);
                        model.ContractEndDate = null;
                    }
                    else
                    {
                        /// Add Provision Month to joining date and calculate ContractEndDate
                        model.ContractStartDate = employee.DateofJoining;
                        //model.ContractEndDate = employee.ProvisionMonth > 0 ? employee.DateofJoining.AddMonths(employee.ProvisionMonth) : employee.DateofJoining.AddMonths(0);
                        model.ContractEndDate = employee.ContractExpireDate != null ? Convert.ToDateTime(employee.ContractExpireDate) : DateTime.Now;
                    }

                    #endregion

                    parentModel.EmploymentContractInfo = model;
                    parentModel.Id = model.EmpoyeeId;
                    return View("CreateOrEdit", parentModel);
                }

            }
            else
            {
                model.Message = error;
                model.ActionType = "CreateEmploymentContractPeriod";
                model.ButtonText = "Save";
                model.SelectedClass = "active";
                model.ErrorClass = "failed";
                model.IsError = 1;

                parentModel.EmploymentContractInfo = model;
                parentModel.Id = model.EmpoyeeId;
                return View("CreateOrEdit", parentModel);
            }
            PropertyReflector.ClearProperties(model);
            ModelState.Clear();

            model.Message = Resources.ErrorMessages.InsertSuccessful;
            model.ErrorClass = "success";
            model.IsError = 0;

            model.ActionType = "CreateEmploymentContractPeriod";
            model.ButtonText = "Save";
            model.SelectedClass = "active";

            #region Getting Start date

            var employee1 = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmpoyeeId);
            var entityList1 = _empService.PRMUnit.EmploymentContractInfoRepository.Get(q => q.EmpoyeeId == model.EmpoyeeId).OrderByDescending(q => q.ContractEndDate).ToList();
            if (entityList1.Count > 0)
            {
                model.isExtension = true;
                model.ContractStartDate = entityList1.FirstOrDefault().ContractEndDate.AddDays(1);
                model.ContractEndDate = null;
            }
            else
            {
                /// Add Provision Month to joining date and calculate ContractEndDate
                model.ContractStartDate = employee1.DateofJoining;
                //model.ContractEndDate = employee.ProvisionMonth > 0 ? employee.DateofJoining.AddMonths(employee.ProvisionMonth) : employee.DateofJoining.AddMonths(0);
                model.ContractEndDate = employee1.ContractExpireDate != null ? Convert.ToDateTime(employee1.ContractExpireDate) : DateTime.Now;
            }

            #endregion

            parentModel.EmploymentContractInfo = model;
            parentModel.Id = model.EmpoyeeId;
            TempData["SuccessMessage"] = model.Message;
            return RedirectToAction("EmploymentContractPeriodIndex", "Employee", new { id = model.EmpoyeeId, type = model.ErrorClass });

            //return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Update
        public ActionResult EditEmploymentContractPeriod(int id)
        {
            var parentModel = new EmployeeViewModel();
            parentModel.ViewType = "EmploymentContractInfo";

            try
            {
                var entity = _empService.PRMUnit.EmploymentContractInfoRepository.GetByID(id);

                var lastRecord = _empService.PRMUnit.EmploymentContractInfoRepository.Get(x => x.EmpoyeeId == entity.EmpoyeeId).Max(x => x.Id);

                var employee = _empService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmpoyeeId);
                var model = entity.ToModel();

                model.EmpCode = employee.EmpID;
                model.EmployeeInitial = employee.EmployeeInitial;
                model.FullName = employee.FullName;
                model.DateofInactive = employee.DateofInactive;
                model.IsContractual = employee.IsContractual;

                if (lastRecord == entity.Id)
                {
                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                }
                else
                {
                    model.DeleteEnable = false;
                }
                model.ActionType = "EditEmploymentContractPeriod";
                model.SelectedClass = "active";

                #region Attachment Files Download
                model.AttachmentFilesList = new List<EmployeeContractAttachmentFiles>();

                var list = EmpContractFileUtl.GetAllFilesByEmpContactInfoId(model.Id);

                foreach (DataRow row in list.Rows)
                {
                    EmployeeContractAttachmentFiles childModel = new EmployeeContractAttachmentFiles();
                    childModel.Id = Convert.ToInt32(row["Id"]);
                    childModel.UserFileName = (string)row["UserFileName"];
                    childModel.OriginalName = (string)row["OriginalName"];
                    childModel.ContentType = (string)row["ContentType"];
                    childModel.Size = Convert.ToInt64(row["Size"]);
                    childModel.Data = (Byte[])row["Data"];
                    model.AttachmentFilesList.Add(childModel);
                }

                #endregion

                parentModel.EmploymentContractInfo = model;
                parentModel.Id = entity.EmpoyeeId;
            }
            catch (Exception ex)
            {
            }
            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EditEmploymentContractPeriod([Bind(Exclude = "Attachment")]EmploymentContractPeriodViewModel model)
        {
            var parentModel = new EmployeeViewModel();
            parentModel.ViewType = "EmploymentContractInfo";
            var error = CheckEmpContractBusinessRule(model, null);

            if (ModelState.IsValid && error == string.Empty)
            {
                try
                {
                    var entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    #region Update Contract Extension

                    var employee = _empService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmpoyeeId);
                    employee.ContractExpireDate = model.ContractEndDate;

                    int d1 = Convert.ToDateTime(employee.DateofJoining).Day;
                    int d2 = Convert.ToDateTime(employee.ContractExpireDate).Day;
                    int d3 = Math.Abs(d2 - d1);
                    var duration = DateAndTime.DateDiff(DateAndTime.DateInterval.Month, Convert.ToDateTime(employee.DateofJoining), Convert.ToDateTime(employee.ContractExpireDate));
                    duration += d3 / 30;
                    employee.ContractDuration = duration;

                    #endregion

                    _empService.PRMUnit.EmploymentContractInfoRepository.Update(entity, new Dictionary<Type, ArrayList>());
                    _empService.PRMUnit.EmploymentInfoRepository.SaveChanges();

                    #region Upload files

                    HttpFileCollectionBase files = Request.Files;
                    foreach (string fileTagName in files)
                    {

                        HttpPostedFileBase file = Request.Files[fileTagName];
                        if (file.ContentLength > 0)
                        {
                            int size = file.ContentLength;
                            string originalName = file.FileName;
                            int position = originalName.LastIndexOf("\\");
                            originalName = originalName.Substring(position + 1);
                            string contentType = file.ContentType;
                            byte[] fileData = new byte[size];
                            file.InputStream.Read(fileData, 0, size);

                            var child = (from dd in model.AttachmentFilesList.Where(x => x.OriginalName.Trim() == originalName.Trim())
                                         select dd).FirstOrDefault();
                            var dt = EmpContractFileUtl.GetAFile(child.Id);

                            if (child != null)
                            {
                                //if (Convert.ToInt32(dt.Rows[0]["Id"]) != child.Id)
                                if (dt.Rows.Count == 0)
                                {
                                    EmpContractFileUtl.SaveFile(entity.Id, child.UserFileName, originalName, contentType, size, fileData);
                                }
                                else
                                {
                                    EmpContractFileUtl.UpdateFile(Convert.ToInt32(dt.Rows[0]["Id"]), child.UserFileName, originalName, contentType, size, fileData);
                                }
                            }


                            //if (dt.Rows.Count > 0)
                            //{
                            //    if (Convert.ToInt32(dt.Rows[0]["Id"]) != child.Id)
                            //    {
                            //        EmpContractFileUtl.SaveFile(entity.Id, child.UserFileName, originalName, contentType, size, fileData);
                            //    }
                            //    else
                            //    {
                            //        EmpContractFileUtl.UpdateFile(Convert.ToInt32(dt.Rows[0]["Id"]), child.UserFileName, originalName, contentType, size, fileData);
                            //    }
                            //}
                        }
                    }

                    #endregion

                    model.Message = Resources.ErrorMessages.UpdateSuccessful;

                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();

                    model.ActionType = "CreateEmploymentContractPeriod";

                    model.ErrorClass = "success";
                    model.IsError = 0;

                    model.ButtonText = "Save";
                    model.DeleteEnable = false;
                    model.SelectedClass = "active";

                    #region Getting Start date

                    var employeeEdit = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmpoyeeId);
                    var entityListEdit = _empService.PRMUnit.EmploymentContractInfoRepository.Get(q => q.EmpoyeeId == model.EmpoyeeId).OrderByDescending(q => q.ContractEndDate).ToList();
                    if (entityListEdit.Count > 0)
                    {
                        model.isExtension = true;
                        model.ContractStartDate = entityListEdit.FirstOrDefault().ContractEndDate.AddDays(1);
                        model.ContractEndDate = null;
                    }
                    else
                    {
                        /// Add Provision Month to joining date and calculate ContractEndDate
                        model.ContractStartDate = employeeEdit.DateofJoining;
                        //model.ContractEndDate = employee.ProvisionMonth > 0 ? employee.DateofJoining.AddMonths(employee.ProvisionMonth) : employee.DateofJoining.AddMonths(0);
                        model.ContractEndDate = employeeEdit.ContractExpireDate != null ? Convert.ToDateTime(employeeEdit.ContractExpireDate) : DateTime.Now;
                    }

                    #endregion

                    parentModel.EmploymentContractInfo = model;
                    parentModel.Id = model.EmpoyeeId;
                    TempData["SuccessMessage"] = model.Message;
                    return RedirectToAction("EmploymentContractPeriodIndex", "Employee", new { id = model.EmpoyeeId, type = model.ErrorClass });

                    //return View("CreateOrEdit", parentModel);
                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ActionType = "EditEmploymentContractPeriod";
                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "active";
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    #region Getting Start date

                    var employeeEdit1 = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmpoyeeId);
                    var entityListEdit1 = _empService.PRMUnit.EmploymentContractInfoRepository.Get(q => q.EmpoyeeId == model.EmpoyeeId).OrderByDescending(q => q.ContractEndDate).ToList();
                    if (entityListEdit1.Count > 0)
                    {
                        model.isExtension = true;
                        model.ContractStartDate = entityListEdit1.FirstOrDefault().ContractEndDate.AddDays(1);
                        model.ContractEndDate = null;
                    }
                    else
                    {
                        /// Add Provision Month to joining date and calculate ContractEndDate
                        model.ContractStartDate = employeeEdit1.DateofJoining;
                        //model.ContractEndDate = employee.ProvisionMonth > 0 ? employee.DateofJoining.AddMonths(employee.ProvisionMonth) : employee.DateofJoining.AddMonths(0);
                        model.ContractEndDate = employeeEdit1.ContractExpireDate != null ? Convert.ToDateTime(employeeEdit1.ContractExpireDate) : DateTime.Now;
                    }

                    #endregion

                    parentModel.EmploymentContractInfo = model;
                    parentModel.Id = model.EmpoyeeId;
                    return View("CreateOrEdit", parentModel);
                }
            }

            model.Message = error;
            model.ActionType = "EditEmploymentContractPeriod";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "active";
            model.ErrorClass = "failed";
            model.IsError = 1;

            #region Getting Start date

            var employeeEdit2 = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmpoyeeId);
            var entityListEdit2 = _empService.PRMUnit.EmploymentContractInfoRepository.Get(q => q.EmpoyeeId == model.EmpoyeeId).OrderByDescending(q => q.ContractEndDate).ToList();
            if (entityListEdit2.Count > 0)
            {
                model.isExtension = true;
                model.ContractStartDate = entityListEdit2.FirstOrDefault().ContractEndDate.AddDays(1);
                model.ContractEndDate = null;
            }
            else
            {
                /// Add Provision Month to joining date and calculate ContractEndDate
                model.ContractStartDate = employeeEdit2.DateofJoining;
                //model.ContractEndDate = employee.ProvisionMonth > 0 ? employee.DateofJoining.AddMonths(employee.ProvisionMonth) : employee.DateofJoining.AddMonths(0);
                model.ContractEndDate = employeeEdit2.ContractExpireDate != null ? Convert.ToDateTime(employeeEdit2.ContractExpireDate) : DateTime.Now;
            }

            #endregion

            parentModel.EmploymentContractInfo = model;
            parentModel.Id = model.EmpoyeeId;
            return View("CreateOrEdit", parentModel);
        }
        #endregion

        #region Delete

        public ActionResult DeleteEmploymentContractPeriod(int id)
        {
            var parentModel = new EmployeeViewModel();
            parentModel.ViewType = "EmploymentContractInfo";

            var entity = _empService.PRMUnit.EmploymentContractInfoRepository.GetByID(id);


            var empContractList = (from tr in _empService.PRMUnit.EmploymentContractInfoRepository.GetAll()
                                   where tr.EmpoyeeId == entity.EmpoyeeId
                                   select tr).ToList();

            var model = entity.ToModel();

            if (!entity.isExtension)
            {
                model.Message = "This record would not be deleted";
                model.ErrorClass = "failed";
                model.IsError = 1;
                TempData["SuccessMessage"] = model.Message;
                return RedirectToAction("EditEmploymentContractPeriod", "Employee", new { id = entity.Id, type = model.ErrorClass });
            }

            if (ModelState.IsValid && entity != null)
            {
                try
                {
                    #region Update Contract Extension

                    if (empContractList.Count > 1)
                    {
                        var employee = _empService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmpoyeeId);
                        employee.ContractExpireDate = model.ContractStartDate.AddDays(-1);

                        int d1 = Convert.ToDateTime(employee.DateofJoining).Day;
                        int d2 = Convert.ToDateTime(employee.ContractExpireDate).Day;
                        int d3 = Math.Abs(d2 - d1);
                        var duration = DateAndTime.DateDiff(DateAndTime.DateInterval.Month,
                            Convert.ToDateTime(employee.DateofJoining), Convert.ToDateTime(employee.ContractExpireDate));
                        duration += d3 / 30;
                        employee.ContractDuration = duration;
                    }

                    #endregion

                    var allTypes = new List<Type> { typeof(PRM_EmpContactFiles) };
                    _empService.PRMUnit.EmploymentContractInfoRepository.Delete(id, allTypes);

                    //_empService.PRMUnit.EmploymentContractInfoRepository.Delete(entity);
                    _empService.PRMUnit.EmploymentContractInfoRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                }
                catch (Exception ex)
                {
                    model = entity.ToModel();
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex,
                        BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "active";

                    parentModel.EmploymentContractInfo = model;
                    parentModel.Id = model.EmpoyeeId;

                    return RedirectToAction("EditEmploymentContractPeriod", null, new { id = entity.Id });

                    //return View("CreateOrEdit", parentModel);
                }
                TempData["SuccessMessage"] = model.Message;
                return RedirectToAction("EmploymentContractPeriodIndex", null, new { id = entity.EmpoyeeId, type = "success" });
            }


            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.IsError = 1;

            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "active";

            parentModel.EmploymentContractInfo = model;
            parentModel.Id = model.EmpoyeeId;

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        public PartialViewResult AddDetail()
        {
            EmployeeContractAttachmentFiles model = new EmployeeContractAttachmentFiles();

            return PartialView("_AttachmentDetails", model);
        }

        public ActionResult DeleteAttachmentFile(int id)
        {
            var masterRow = EmpContractFileUtl.GetAFile(id);
            EmpContractFileUtl.DeleteFile(id);

            return RedirectToAction("EditEmploymentContractPeriod", "Employee", new { id = Convert.ToInt32(masterRow.Rows[0]["EmpContactInfoId"]) });
        }
        public ActionResult GetEmpAttachedFile(int id)
        {
            var entity = _empService.PRMUnit.AttachmentContactFilesRepository.GetByID(id);

            DataTable file = EmpContractFileUtl.GetAFile(id);
            DataRow row = file.Rows[0];

            string name = (string)row["OriginalName"];
            string contentType = (string)row["ContentType"];
            Byte[] data = (Byte[])row["Data"];

            // Send the file to the browser
            Response.AddHeader("Content-type", contentType);
            Response.AddHeader("Content-Disposition", "attachment; filename=" + name);
            Response.BinaryWrite(data);
            Response.Flush();
            Response.End();

            return View("_AttachmentDetails", entity.ToModel());

            //return View("_AttachmentDetails", data1.ToModel());
        }

        #endregion

        #region Employee Salary Structure

        #region Insert

        public ActionResult SalaryStructureIndex(int id, string type, string delType, string insertType)
        {
            var parentModel = new EmployeeViewModel();
            var model = parentModel.EmployeeSalary;
            var empSalary = _empService.PRMUnit.EmpSalaryRepository.GetByID(id, "EmployeeId");// == null ? new PRM_EmpSalary() : _empService.PRMUnit.EmpSalaryRepository.GetByID(id, "EmployeeId");
            var employee = _empService.PRMUnit.EmploymentInfoRepository.GetByID(id);

            model.EmployeeId = employee.Id;
            model.EmpCode = employee.EmpID;
            model.EmployeeInitial = employee.EmployeeInitial;
            model.FullName = employee.FullName;
            model.DateofInactive = employee.DateofInactive;


            if (empSalary != null)
            {
                model.GradeId = empSalary.GradeId;
                model.StepId = empSalary.StepId;
                model.isConsolidated = empSalary.isConsolidated;
                model.GrossSalary = empSalary.GrossSalary;
                model.OrgGrossSalary = empSalary.GrossSalary;
            }
            else
            {
                empSalary = new PRM_EmpSalary();
                if (employee.IsContractual)
                {
                    model.isConsolidated = true;

                    model.GradeId = employee.JobGradeId;
                    var step = (from tr in _empService.PRMUnit.JobGradeStepRepository.Get(p => p.JobGradeId == model.GradeId)
                                select tr.Id).FirstOrDefault();

                    if (step > 0)
                    {
                        model.StepId = step;
                    }
                    //model.StepId = 1; // deafult
                }
            }
            populateDropdown(model);

            GettingSalaryStructureDetailList(model);
            GetSalaryHeadAmountTypeSetting(model);

            #region Hide Code
            /*
            int gradeId = 193, stepId = 22;
            var salaryStructureDetails = _empService.GetSalaryStrutureDetailsByGradeAndStepId(gradeId, stepId);
            var salaryHeads = _empService.PRMUnit.SalaryHeadRepository.Fetch().ToList();

            foreach (var item in salaryStructureDetails)
            {
                var ssdModel = item.ToModel();
                ssdModel.HeadAmountTypeList = Common.GetAmountType().ToList();
                ssdModel.DisplayHeadName = salaryHeads.Find(x => x.Id == item.HeadId).HeadName;

                model.SalaryStructureDetail.Add(ssdModel);
            }
            
            model.TotalAddition = model.SalaryStructureDetail.Where(s=> s.HeadType =="Addition").Sum(x => x.Amount);
            model.TotalDeduction = model.SalaryStructureDetail.Where(s => s.HeadType == "Deduction").Sum(x => x.Amount);
            model.NetPay = model.TotalAddition - model.TotalDeduction;

            
            */
            #endregion

            #region Button and event----------------------------------

            model.SelectedClass = "active";
            if (empSalary.EmployeeId == 0)
            {
                model.ActionType = "CreateSalaryStructure";
                model.ButtonText = "Save";
            }
            //else if (empSalary.EmployeeId != 0 && model.SalaryStructureDetail.Count == 0)
            //{
            //    model.ActionType = "EditSalaryStructure";
            //    model.ButtonText = "Save";
            //}
            else
            {
                model.ActionType = "EditSalaryStructure";
                model.ButtonText = "Update";
                model.DeleteEnable = true;
            }

            if (type == "success")
            {
                model.Message = Resources.ErrorMessages.InsertSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;
            }

            if (delType == "success")
            {
                model.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;
            }

            if (insertType == "success")
            {
                model.Message = Resources.ErrorMessages.InsertSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;
            }
            #endregion

            #region Job Grade ddl Or Textbox
            var salaryProcess = _pgmCommonservice.PGMUnit.SalaryMasterRepository.Get(x => x.EmployeeId == model.EmployeeId).Any();
            if (salaryProcess)
            {
                model.IsSalaryProcess = true;
            }
            else
            {
                model.IsSalaryProcess = false;
            }
            #endregion


            parentModel.ViewType = "EmployeeSalaryStructure";
            parentModel.EmployeeSalary = model;
            parentModel.Id = employee.Id;
            parentModel.EmpId = employee.EmpID;

            return View("CreateOrEdit", parentModel);
        }

        private void GettingSalaryStructureDetailList(EmployeeSalaryStructureViewModel model)
        {
            var salaryStructureId = 0;

            IList<PRM_SalaryStructureDetail> salaryStructureDetails = null;
            IList<PRM_EmpSalaryDetail> empSalaryDetails = _empService.GetEmpSalaryDetailsByGradeAndStepId(model.GradeId, model.StepId, model.EmployeeId, out salaryStructureId);
            var salaryHeads = _empService.PRMUnit.SalaryHeadRepository.Fetch().ToList();

            salaryStructureDetails = _salaryStructureService.GetSalaryStrutureDetailsByGradeAndStepId(model.GradeId, model.StepId, out salaryStructureId);

            #region Adding new salary heads from Salary Structure Template -------------------------

            //var empSalaryHeadIdList = (from h in empSalaryDetails select h.HeadId).ToList();
            //var salaryAbsenceHeadList = (from ah in salaryStructureDetails
            //                             where !empSalaryHeadIdList.Contains(ah.HeadId)
            //                             select ah).ToList();

            //if (salaryAbsenceHeadList.Count > 0)
            //{
            //    foreach (var newSalaryHead in salaryAbsenceHeadList)
            //    {
            //        PRM_EmpSalaryDetail obj = new PRM_EmpSalaryDetail();
            //        obj.Id = newSalaryHead.Id;
            //        obj.HeadId = newSalaryHead.HeadId;
            //        obj.HeadType = newSalaryHead.HeadType;
            //        obj.EmployeeId = model.EmployeeId;
            //        obj.AmountType = newSalaryHead.AmountType;
            //        obj.Amount =0;
            //        obj.IsTaxable = newSalaryHead.IsTaxable;
            //        obj.IUser = newSalaryHead.IUser;
            //        obj.IDate = newSalaryHead.IDate;
            //        obj.EUser = newSalaryHead.EUser;
            //        obj.EDate = newSalaryHead.EDate;

            //        empSalaryDetails.Add(obj);
            //    }
            //}
            #endregion

            #region Salary structure Details-----------------------------


            if (empSalaryDetails.Count == 0) //new salary structure
            {
                foreach (var item in salaryStructureDetails)
                {
                    var ssdModel = item.ToModel();
                    ssdModel.AmountType = item.AmountType;

                    ssdModel.HeadAmountTypeList = Common.GetAmountType().ToList();

                    ssdModel.DisplayHeadName = salaryHeads.Find(x => x.Id == item.HeadId).HeadName;

                    ssdModel.IsGrossPayHead = salaryHeads.Find(x => x.Id == item.HeadId).IsGrossPayHead;

                    // check consolidated structure, if yes then amount type should be percent and amount must be zero
                    if (model.isConsolidated == true)
                    {
                        if (ssdModel.IsGrossPayHead == true)
                        {
                            ssdModel.AmountType = "Percent";
                            //ssdModel.Amount = Math.Round(Convert.ToDecimal(0), 2);                      

                            if (ssdModel.DisplayHeadName.Contains("Basic") || ssdModel.DisplayHeadName.Contains("basic"))
                            {
                                ssdModel.Amount = Math.Round(Convert.ToDecimal(60.00), 2);
                            }
                            else if (ssdModel.DisplayHeadName.Contains("House Rent") || ssdModel.DisplayHeadName.Contains("Houserent"))
                            {
                                ssdModel.Amount = Math.Round(Convert.ToDecimal(20.00), 2);
                            }
                            else if (ssdModel.DisplayHeadName.Contains("Medical") || ssdModel.DisplayHeadName.Contains("medical"))
                            {
                                ssdModel.Amount = Math.Round(Convert.ToDecimal(10.00), 2);
                            }
                            else if (ssdModel.DisplayHeadName.Contains("Conveyance") || ssdModel.DisplayHeadName.Contains("Conv"))
                            {
                                ssdModel.Amount = Math.Round(Convert.ToDecimal(10.00), 2);
                            }
                            else
                            {
                                ssdModel.Amount = Math.Round(Convert.ToDecimal(0), 2);
                            }
                        }
                        else
                        {
                            ssdModel.Amount = Math.Round(Convert.ToDecimal(0), 2);
                        }
                    }

                    model.SalaryStructureDetail.Add(ssdModel);
                }
            }
            else //existing salary structure-
            {

                foreach (BEPZA_MEDICAL.DAL.PRM.PRM_EmpSalaryDetail item in empSalaryDetails)
                {
                    var ssdModel = item.ToModel();

                    ssdModel.AmountType = item.AmountType;

                    ssdModel.HeadAmountTypeList = Common.GetAmountType().ToList();

                    ssdModel.DisplayHeadName = salaryHeads.Find(x => x.Id == item.HeadId).HeadName;

                    ssdModel.IsGrossPayHead = salaryHeads.Find(x => x.Id == item.HeadId).IsGrossPayHead;
                    ssdModel.cssSalaryHeadClass = "";

                    model.SalaryStructureDetail.Add(ssdModel);
                }

                #region Adding new salary heads from Salary Structure Template -------------------------

                var empSalaryHeadIdList = (from h in empSalaryDetails select h.HeadId).ToList();
                var salaryAbsenceHeadList = (from ah in salaryStructureDetails
                                             where !empSalaryHeadIdList.Contains(ah.HeadId)
                                             select ah).ToList();

                if (salaryAbsenceHeadList.Count > 0)
                {
                    foreach (var newSalaryHead in salaryAbsenceHeadList)
                    {
                        var obj = new SalaryStructureDetailsModel();
                        obj.Id = newSalaryHead.Id;
                        obj.HeadId = newSalaryHead.HeadId;
                        obj.HeadType = newSalaryHead.HeadType;
                        obj.EmployeeId = model.EmployeeId;
                        obj.AmountType = newSalaryHead.AmountType;

                        obj.HeadAmountTypeList = Common.GetAmountType().ToList();

                        obj.DisplayHeadName = salaryHeads.Find(x => x.Id == newSalaryHead.HeadId).HeadName;

                        obj.IsGrossPayHead = salaryHeads.Find(x => x.Id == newSalaryHead.HeadId).IsGrossPayHead;

                        obj.Amount = 0;
                        obj.IsTaxable = newSalaryHead.IsTaxable;
                        obj.cssSalaryHeadClass = "cssSalaryHeadClass";

                        model.SalaryStructureDetail.Add(obj);
                    }
                }
                #endregion
            }
            #endregion

            model.TotalAddition = model.SalaryStructureDetail.Where(s => s.HeadType == "Addition").Sum(x => x.Amount);
            model.TotalDeduction = model.SalaryStructureDetail.Where(s => s.HeadType == "Deduction").Sum(x => x.Amount);
            model.NetPay = model.TotalAddition - model.TotalDeduction;
            model.SalaryStructureId = salaryStructureId;
        }

        private void GetSalaryHeadAmountTypeSetting(EmployeeSalaryStructureViewModel model)
        {
            foreach (var item in model.SalaryStructureDetail)
            {
                var ssdModel = item;
                ssdModel.AmountType = item.AmountType;
                ssdModel.HeadAmountTypeList = Common.GetAmountType().ToList();

                // Select Amount Type
                if (ssdModel.HeadAmountTypeList.Count > 0)
                {
                    foreach (var Amttype in ssdModel.HeadAmountTypeList)
                    {
                        if (Amttype.Value == ssdModel.AmountType)
                            Amttype.Selected = true;
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult CreateSalaryStructure(EmployeeSalaryStructureViewModel model)
        {
            var parentModel = new EmployeeViewModel();
            parentModel.ViewType = "EmployeeSalaryStructure";
            model.ErrorClass = "Success";

            var errorList = CheckEmpSalaryBusinessRule(model);

            if (ModelState.IsValid)
            {
                if (errorList.Count > 0)
                {
                    populateDropdown(model);

                    model.Message = errorList.FirstOrDefault(); //Resources.ErrorMessages.InsertFailed;
                    model.ErrorClass = "failed";
                    model.IsError = 0;


                    model.ActionType = "CreateSalaryStructure";
                    model.ButtonText = "Save";
                    model.SelectedClass = "active";

                    parentModel.EmployeeSalary = model;
                    parentModel.Id = model.EmployeeId;

                    GetSalaryHeadAmountTypeSetting(model);

                    return View("CreateOrEdit", parentModel);

                }
                else
                {

                    #region Master
                    var empSalaryEntity = new PRM_EmpSalary()
                            {
                                EmployeeId = model.EmployeeId,
                                SalaryStructureId = model.SalaryStructureId,
                                SalaryScaleId = model.SalaryScaleId,
                                GradeId = model.GradeId,
                                StepId = model.StepId,
                                GrossSalary = Math.Round(model.GrossSalary),
                                isConsolidated = model.isConsolidated,

                                IUser = User.Identity.Name,
                                IDate = Common.CurrentDateTime,
                            };
                    #endregion

                    #region Details
                    var empSalaryDetailsEntityList = empSalaryEntity.PRM_EmpSalaryDetail;//new List<PRM_EmpSalaryDetail>();

                    foreach (var item in model.SalaryStructureDetail)
                    {
                        var empSalaryDetailsEntity = new PRM_EmpSalaryDetail()
                        {
                            EmployeeId = model.EmployeeId,
                            HeadId = item.HeadId,
                            HeadType = item.HeadType,
                            AmountType = item.AmountType,
                            IsTaxable = item.IsTaxable,
                            Amount = item.Amount,

                            IUser = User.Identity.Name,
                            IDate = Common.CurrentDateTime
                        };
                        empSalaryDetailsEntityList.Add(empSalaryDetailsEntity);
                    }
                    #endregion

                    #region Save
                    try
                    {
                        _empService.PRMUnit.EmpSalaryRepository.Add(empSalaryEntity);
                        //_empService.PRMUnit.EmpSalaryDetailRepository.Add(empSalaryDetailsEntity);

                        _empService.PRMUnit.EmpSalaryRepository.SaveChanges();
                        //_empService.PRMUnit.EmpSalaryDetailRepository.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                        model.ErrorClass = "failed";
                        model.IsError = 0;


                        model.ActionType = "CreateSalaryStructure";
                        model.ButtonText = "Save";
                        model.SelectedClass = "active";

                        parentModel.EmployeeSalary = model;
                        parentModel.Id = model.EmployeeId;

                        populateDropdown(model);
                        GetSalaryHeadAmountTypeSetting(model);

                        return View("CreateOrEdit", parentModel);

                    }
                    #endregion
                }
            }
            else
            {
                #region Process save failed

                model.Message = Resources.ErrorMessages.InsertFailed;
                model.ErrorClass = "failed";
                model.IsError = 0;


                model.ActionType = "CreateSalaryStructure";
                model.ButtonText = "Save";
                model.SelectedClass = "active";

                parentModel.EmployeeSalary = model;
                parentModel.Id = model.EmployeeId;

                populateDropdown(model);
                GetSalaryHeadAmountTypeSetting(model);

                return View("CreateOrEdit", parentModel);

                #endregion
            }

            #region Save success and repopulate the form

            return RedirectToAction("SalaryStructureIndex", "Employee", new { Id = model.EmployeeId, insertType = "success" });

            //model.Message = Resources.ErrorMessages.InsertSuccessful;
            //model.ErrorClass = "success";
            //model.IsError = 0;

            //model.ActionType = "CreateSalaryStructure";
            //model.ButtonText = "Update";
            //model.DeleteEnable = true;
            //model.SelectedClass = "selected";

            //parentModel.EmployeeSalary = model;
            //parentModel.Id = model.EmployeeId;

            //return View("CreateOrEdit", parentModel);

            #endregion
        }

        #endregion

        #region Update
        [HttpPost]
        public ActionResult EditSalaryStructure(EmployeeSalaryStructureViewModel model)
        {
            var parentModel = new EmployeeViewModel();
            parentModel.ViewType = "EmployeeSalaryStructure";
            model.ErrorClass = "Success";


            var errorList = CheckEmpSalaryBusinessRule(model);

            if (errorList.Count > 0)
            {
                populateDropdown(model);

                model.Message = errorList.FirstOrDefault(); //Resources.ErrorMessages.InsertFailed;
                model.ErrorClass = "failed";
                model.IsError = 0;


                model.ActionType = "EditSalaryStructure";
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "active";

                parentModel.EmployeeSalary = model;
                parentModel.Id = model.EmployeeId;

                GetSalaryHeadAmountTypeSetting(model);

                return View("CreateOrEdit", parentModel);
            }

            if (ModelState.IsValid)
            {
                #region Master
                var empSalaryEntity = _empService.PRMUnit.EmpSalaryRepository.GetByID(model.EmployeeId, "EmployeeId");

                empSalaryEntity.EmployeeId = model.EmployeeId;
                empSalaryEntity.SalaryStructureId = model.SalaryStructureId;
                empSalaryEntity.SalaryScaleId = model.SalaryScaleId;
                empSalaryEntity.GradeId = model.GradeId;
                empSalaryEntity.StepId = model.StepId;
                empSalaryEntity.GrossSalary = Math.Round(model.GrossSalary);
                empSalaryEntity.isConsolidated = model.isConsolidated;

                empSalaryEntity.EUser = User.Identity.Name;
                empSalaryEntity.EDate = Common.CurrentDateTime;

                #endregion

                #region Details
                var empSalaryDetailsEntityList = empSalaryEntity.PRM_EmpSalaryDetail.ToList();
                //delete all existing
                empSalaryDetailsEntityList.ForEach(x => _empService.PRMUnit.EmpSalaryDetailRepository.Delete(x.Id));

                //var id = 0;
                var lstChild = new ArrayList();
                foreach (var item in model.SalaryStructureDetail)
                {
                    var empSalaryDetailsEntity = new PRM_EmpSalaryDetail(); //empSalaryDetailsEntityList[id];

                    empSalaryDetailsEntity.EmployeeId = model.EmployeeId;
                    empSalaryDetailsEntity.HeadId = item.HeadId;
                    empSalaryDetailsEntity.HeadType = item.HeadType;
                    empSalaryDetailsEntity.AmountType = item.AmountType;
                    empSalaryDetailsEntity.IsTaxable = item.IsTaxable;
                    empSalaryDetailsEntity.Amount = item.Amount;

                    empSalaryDetailsEntity.IUser = User.Identity.Name;
                    empSalaryDetailsEntity.IDate = Common.CurrentDateTime;
                    empSalaryDetailsEntity.EUser = User.Identity.Name;
                    empSalaryDetailsEntity.EDate = Common.CurrentDateTime;

                    lstChild.Add(empSalaryDetailsEntity);
                    //id++;
                }

                var navigationList = new Dictionary<Type, ArrayList>();
                navigationList.Add(typeof(PRM_EmpSalaryDetail), lstChild);
                #endregion

                #region Update
                try
                {
                    _empService.PRMUnit.EmpSalaryRepository.Update(empSalaryEntity, "EmployeeId", navigationList);

                    _empService.PRMUnit.EmpSalaryRepository.SaveChanges();
                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";
                    model.IsError = 0;


                    model.ActionType = "CreateSalaryStructure";
                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "active";

                    parentModel.EmployeeSalary = model;
                    parentModel.Id = model.EmployeeId;

                    GetSalaryHeadAmountTypeSetting(model);

                    return View("CreateOrEdit", parentModel);

                }
                #endregion
            }
            else
            {
                #region Process save failed

                model.Message = Resources.ErrorMessages.InsertFailed;
                model.ErrorClass = "failed";
                model.IsError = 0;


                model.ActionType = "CreateSalaryStructure";
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "active";

                parentModel.EmployeeSalary = model;
                parentModel.Id = model.EmployeeId;

                GetSalaryHeadAmountTypeSetting(model);

                return View("CreateOrEdit", parentModel);

                #endregion
            }

            #region Save success and repopulate the form

            return RedirectToAction("SalaryStructureIndex", "Employee", new { Id = model.EmployeeId, type = "success" });

            //model.Message = Resources.ErrorMessages.InsertSuccessful;
            //model.ErrorClass = "success";
            //model.IsError = 0;

            //model.ActionType = "CreateSalaryStructure";
            //model.ButtonText = "Update";
            //model.DeleteEnable = true;
            //model.SelectedClass = "active";

            //parentModel.EmployeeSalary = model;
            //parentModel.Id = model.EmployeeId;

            //return View("CreateOrEdit", parentModel);

            #endregion
        }


        #endregion

        #region Delete
        public ActionResult DeleteSalaryStructure(int id)
        {
            //var empSalary = _empService.PRMUnit.EmpSalaryRepository.GetByID(id, "EmployeeId");
            //var empSalaryDetails = empSalary.PRM_EmpSalaryDetail;

            var empSalaryStatusChange = (from ss in _empService.PRMUnit.EmpStatusChangeRepository.GetAll()
                                         where ss.EmployeeId == id
                                         select ss).LastOrDefault();
            if (empSalaryStatusChange != null)
            {
                var parentModel = new EmployeeViewModel();
                var model = parentModel.EmployeeSalary;
                var empSalary = _empService.PRMUnit.EmpSalaryRepository.GetByID(id, "EmployeeId") == null ? new PRM_EmpSalary() : _empService.PRMUnit.EmpSalaryRepository.GetByID(id, "EmployeeId");
                var employee = _empService.PRMUnit.EmploymentInfoRepository.GetByID(id);

                model.EmployeeId = employee.Id;
                model.EmpCode = employee.EmpID;
                model.EmployeeInitial = employee.EmployeeInitial;
                model.FullName = employee.FullName;

                populateDropdown(model);

                model.GradeId = empSalary.GradeId;
                model.StepId = empSalary.StepId;
                model.isConsolidated = empSalary.isConsolidated;
                model.GrossSalary = empSalary.GrossSalary;

                if (employee.IsContractual)
                    model.isConsolidated = true;

                model.Message = "Employee promotion or increment or confirmation is already exist. So you can not delete it.";
                model.ErrorClass = "failed";
                model.IsError = 1;

                model.ActionType = "EditSalaryStructure";
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "active";

                GettingSalaryStructureDetailList(model);
                GetSalaryHeadAmountTypeSetting(model);

                parentModel.ViewType = "EmployeeSalaryStructure";
                parentModel.EmployeeSalary = model;
                parentModel.Id = employee.Id;
                parentModel.EmpId = employee.EmpID;

                return View("CreateOrEdit", parentModel);
            }

            try
            {
                var allTypes = new List<Type> { typeof(PRM_EmpSalaryDetail) };
                _empService.PRMUnit.EmpSalaryRepository.Delete(id, "EmployeeId", allTypes);
                _empService.PRMUnit.EmpSalaryRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                var parentModel = new EmployeeViewModel();
                var model = parentModel.EmployeeSalary;
                var empSalary = _empService.PRMUnit.EmpSalaryRepository.GetByID(id, "EmployeeId") == null ? new PRM_EmpSalary() : _empService.PRMUnit.EmpSalaryRepository.GetByID(id, "EmployeeId");
                var employee = _empService.PRMUnit.EmploymentInfoRepository.GetByID(id);

                model.EmployeeId = employee.Id;
                model.EmpCode = employee.EmpID;
                model.EmployeeInitial = employee.EmployeeInitial;
                model.FullName = employee.FullName;

                populateDropdown(model);

                model.GradeId = empSalary.GradeId;
                model.StepId = empSalary.StepId;
                model.isConsolidated = empSalary.isConsolidated;
                model.GrossSalary = empSalary.GrossSalary;

                if (employee.IsContractual)
                    model.isConsolidated = true;

                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                model.ErrorClass = "failed";
                model.IsError = 1;

                model.ActionType = "EditSalaryStructure";
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "active";

                GettingSalaryStructureDetailList(model);
                GetSalaryHeadAmountTypeSetting(model);

                parentModel.ViewType = "EmployeeSalaryStructure";
                parentModel.EmployeeSalary = model;
                parentModel.Id = employee.Id;
                parentModel.EmpId = employee.EmpID;

                return View("CreateOrEdit", parentModel);
            }
            return RedirectToAction("SalaryStructureIndex", "Employee", new { Id = id, delType = "success" });
        }
        #endregion

        #endregion

        #region Employee Photograph/Signature------------------------------------------

        public ActionResult EmployeePhotographIndex(int id, bool isPhoto, string message, bool? isSuccessful)
        {
            var parentModel = new EmployeeViewModel();
            var model = parentModel.EmployeePhotograph;

            var entity = _empService.GetEmployeePhoto(id, isPhoto);
            var employee = _empService.PRMUnit.EmploymentInfoRepository.GetByID(id);

            if (entity != null)
            {
                model = entity.ToModel();
                model.ActionType = "EmployeePhotographDelete";
            }
            else
            {
                model.ActionType = "EmployeePhotographCreate";
            }
            model.EmployeeId = id;
            model.IsPhoto = isPhoto;
            model.EmpCode = employee.EmpID;
            model.EmployeeInitial = employee.EmployeeInitial;
            model.FullName = employee.FullName;
            model.DateofInactive = employee.DateofInactive;

            if (isPhoto) model.SelectedClass = "active";

            parentModel.EmployeePhotograph = model;
            parentModel.Id = model.EmployeeId;

            parentModel.ViewType = "EmployeePhotograph";

            if (!String.IsNullOrEmpty(message))
            {
                model.IsSuccessful = Convert.ToBoolean(isSuccessful);
                model.Message = message;
            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EmployeePhotographCreate(EmployeePhotoGraphViewModel model, string btnSubmit)
        {
            var parentModel = new EmployeeViewModel();

            if (btnSubmit == "Upload")
            {
                model = UploadEmployeePhoto(model);
            }
            if (btnSubmit == "Save")
            {
                model = SaveEmployeePhoto(model);

                if (model.IsSuccessful)
                {
                    model.ActionType = "EmployeePhotographDelete";
                }

            }
            if (model.IsPhoto) model.SelectedClass = "active";

            parentModel.EmployeePhotograph = model;
            parentModel.Id = model.EmployeeId;
            parentModel.ViewType = "EmployeePhotograph";
            return View("CreateOrEdit", parentModel);
        }

        private EmployeePhotoGraphViewModel SaveEmployeePhoto(EmployeePhotoGraphViewModel model)
        {
            try
            {
                if (model.PhotoSignature != null)
                {
                    PRM_EmpPhoto entity = new PRM_EmpPhoto();
                    byte[] buf = model.PhotoSignature;
                    entity.EmployeeId = model.EmployeeId;
                    entity.PhotoSignature = buf;
                    entity.IsPhoto = model.IsPhoto;
                    _empService.PRMUnit.EmployeePhotoGraphRepository.Add(entity);
                    _empService.PRMUnit.EmployeePhotoGraphRepository.SaveChanges();
                    model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    model.IsSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }
            return model;
        }

        [HttpPost]
        public ActionResult EmployeePhotographDelete(EmployeePhotoGraphViewModel model)
        {
            var parentModel = new EmployeeViewModel();

            try
            {
                PRM_EmpPhoto entity = _empService.GetEmployeePhoto(model.EmployeeId, model.IsPhoto);
                if (entity != null)
                {
                    _empService.PRMUnit.EmployeePhotoGraphRepository.Delete(entity);
                    _empService.PRMUnit.EmployeePhotoGraphRepository.SaveChanges();
                    model.Message = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                    model.IsSuccessful = true;
                    if (model.IsSuccessful)
                    {
                        model.ActionType = "EmployeePhotographCreate";
                    }
                }

                if (model.IsPhoto)
                    model.SelectedClass = "active";


            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            }

            //if (model.IsSuccessful)
            //{
            //    return RedirectToAction("EmployeePhotographIndex", new { id = model.EmployeeId, isPhoto = model.IsPhoto, message = model.Message, isSuccessful = model.IsSuccessful });
            //}

            parentModel.EmployeePhotograph = model;
            parentModel.Id = model.EmployeeId;
            parentModel.ViewType = "EmployeePhotograph";
            return View("CreateOrEdit", parentModel);
        }


        public FileContentResult GetImage(int? id, bool? isPhoto)
        {
            PRM_EmpPhoto toDisplay = null;
            if (id != null && id != 0 && isPhoto != null)
            {
                toDisplay = _empService.GetEmployeePhoto((int)id, (bool)isPhoto);
            }
            if (toDisplay != null)
            {
                return File(toDisplay.PhotoSignature, "image/jpeg");
            }
            else
            {
                return null;
            }
        }

        private EmployeePhotoGraphViewModel UploadEmployeePhoto(EmployeePhotoGraphViewModel model)
        {
            try
            {
                //var image = WebImage.GetImageFromRequest();
                var image = GetCustomImageFromRequest();

                if (image != null)
                {
                    if (image.Width <= 400)
                    {
                        if (image.Height <= 400)
                        {
                            byte[] buf = image.GetBytes();
                            model.PhotoSignature = buf;

                            if (image.Width > 400)
                            {
                                image.Resize(400, ((400 * image.Height) / image.Width), true, false);
                            }

                            var filename = Path.GetFileName(image.FileName);
                            image.Save(Path.Combine("~/Content/TempFiles/", filename));
                            filename = Path.Combine("~/Content/TempFiles/", filename);
                            model.ImageUrl = Url.Content(filename);
                            model.ImageAltText = image.FileName.Substring(0, image.FileName.Length - 4);
                            model.Message = "Uploaded Successfully!";
                            model.IsSuccessful = true;
                        }
                        else
                        {
                            model.IsSuccessful = false;
                            model.Message = "Photo size must be at most (400px Χ 400px) .";
                        }
                    }
                    else
                    {
                        model.IsSuccessful = false;
                        model.Message = "Photo size must be at most (400px Χ 400px) .";
                    }
                }
            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = "Upload Failed!";
            }

            return model;
        }

        private WebImage GetCustomImageFromRequest()
        {
            var request = System.Web.HttpContext.Current.Request;

            if (request == null)
            {
                return null;
            }

            try
            {
                var postedFile = request.Files[0];
                var image = new WebImage(postedFile.InputStream)
                {
                    FileName = postedFile.FileName
                };
                return image;
            }
            catch
            {
                return null;
            }
        }


        //private EmployeePhotoGraphViewModel DeleteImage(EmployeePhotoGraphViewModel model)
        //{
        //    try
        //    {
        //        PRM_EmpPhoto entity = _empService.GetEmployeePhoto(model.EmployeeId, model.IsPhoto);
        //        if (entity != null)
        //        {
        //            _empService.PRMUnit.EmployeePhotoGraphRepository.Delete(entity);
        //            _empService.PRMUnit.EmployeePhotoGraphRepository.SaveChanges();
        //            model.ErrMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        model.IsError = 1;
        //        try
        //        {
        //            if (ex.InnerException != null && (ex.InnerException is SqlException || ex.InnerException is EntityCommandExecutionException))
        //            {
        //                SqlException sqlException = ex.InnerException as SqlException;
        //                model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            model.ErrMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
        //        }
        //    }
        //    return model;

        //}

        //[HttpPost]
        //public ActionResult CommonAction(string btnSubmit, EmployeePhotoGraphViewModel model)
        //{
        //    var parentModel = new EmployeeViewModel();

        //    if (btnSubmit == "Upload")
        //    {
        //        model = Upload(model);
        //    }
        //    if (btnSubmit == "Save")
        //    {
        //        model = Save(model);
        //    }
        //    if (btnSubmit == "Delete")
        //    {
        //        model = DeleteImage(model);
        //    }

        //    if (model.IsPhoto)
        //        model.SelectedClass = "active";

        //    parentModel.EmployeePhotograph = model;
        //    parentModel.Id = model.EmployeeId;
        //    parentModel.ViewType = "EmployeePhotograph";

        //    return View("CreateOrEdit", parentModel);
        //}




        #endregion

        #region Others action


        public JsonResult DuplicateCheck(string initial, string empId)
        {
            if (_empService.EmpInitialDuplicateCheck(initial, empId))
                return Json(new { data = true }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { data = false }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GenerateEmployeeEmail(string initial, string empId)
        {
            if (!_empService.EmpInitialDuplicateCheck(initial, empId))
            {
                return Json(new { email = initial.Trim().ToLower() + "@bepza.org.bd" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { email = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetConfirmationDate(string JoiningDate, decimal duration)
        {
            DateTime? confirmationDate = null;
            if (JoiningDate != string.Empty)
            {
                try
                {
                    return Json(new
                    {
                        confirmationDate = Convert.ToDateTime(JoiningDate).AddMonths(Convert.ToInt32(duration)).AddDays(-1).ToString(DateAndTime.GlobalDateFormat)
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                { }
            }
            return Json(new { confirmationDate = confirmationDate }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProbationaryPeriod(string JoiningDate, string ConfirmationDate)
        {
            decimal duration = default(decimal);
            if (JoiningDate != string.Empty)
            {
                try
                {
                    if (Convert.ToDateTime(ConfirmationDate) >= Convert.ToDateTime(JoiningDate))
                    {
                        int d1 = Convert.ToDateTime(JoiningDate).Day;
                        int d2 = Convert.ToDateTime(ConfirmationDate).Day;
                        int d3 = Math.Abs(d2 - d1);

                        duration = DateAndTime.DateDiff(DateAndTime.DateInterval.Month, Convert.ToDateTime(JoiningDate), Convert.ToDateTime(ConfirmationDate));
                        duration += d3 / 30;

                        return Json(new
                        {
                            duration = duration
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { duration = "msg" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                { }
            }
            return Json(new { duration = duration }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetContractEndDate(string JoiningDate, decimal duration)
        {
            DateTime? contractEndDate = null;
            if (JoiningDate != string.Empty)
            {
                try
                {
                    return Json(new
                    {
                        contractEndDate = Convert.ToDateTime(JoiningDate).AddMonths(Convert.ToInt32(duration)).AddDays(-1).ToString(DateAndTime.GlobalDateFormat)
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                { }
            }
            return Json(new { contractEndDate = contractEndDate }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetContractDuration(string JoiningDate, string ContractExpireDate)
        {
            decimal duration = default(decimal);
            if (JoiningDate != string.Empty)
            {
                try
                {
                    if (Convert.ToDateTime(ContractExpireDate) > Convert.ToDateTime(JoiningDate))
                    {
                        int d1 = Convert.ToDateTime(JoiningDate).Day;
                        int d2 = Convert.ToDateTime(ContractExpireDate).Day;
                        int d3 = Math.Abs(d2 - d1);

                        duration = DateAndTime.DateDiff(DateAndTime.DateInterval.Month, Convert.ToDateTime(JoiningDate), Convert.ToDateTime(ContractExpireDate));
                        duration += d3 / 30;

                        return Json(new
                        {
                            duration = duration
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { duration = "msg" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                { }
            }
            return Json(new { duration = duration }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRetirementDate(int? Id, string DOB)
        {
            DateTime? prlDate = null;
            DateTime? retirementDate = null;

            var rd = (from tr in _empService.PRMUnit.RetirementAgeInfoRepository.GetAll()
                      select tr).ToList();

            //check Freedom fighter
            var entityEmpPersonalInfo = _empService.PRMUnit.PersonalInfoRepository.GetByID(Id, "EmployeeId");
            int RetYear = Convert.ToInt32(rd.FirstOrDefault().RetirementAge);

            if (entityEmpPersonalInfo != null)
            {
                if (Convert.ToBoolean(entityEmpPersonalInfo.IsFreedomFighter) && entityEmpPersonalInfo != null)
                {
                    RetYear = Convert.ToInt32(rd.FirstOrDefault().FreedomFighterAge);
                }
            }

            if (DOB != string.Empty)
            {
                try
                {
                    if (rd != null && rd.Count > 0)
                    {
                        return Json(new
                        {
                            prlDate = Convert.ToDateTime(DOB).AddYears(RetYear - 1).AddDays(-1).ToString("yyyy-MM-dd"),
                            retirementDate = Convert.ToDateTime(DOB).AddYears(RetYear).AddDays(-1).ToString("yyyy-MM-dd")

                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception)
                { }
            }
            return Json(new { prlDate = prlDate, retirementDate = retirementDate }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSalaryStructureDetails(int gradeId, int stepId, int empId, bool IsConsolidated, string ErrorClass)
        {
            var model = new EmployeeSalaryStructureViewModel();
            var salaryStructureId = 0;

            IList<PRM_SalaryStructureDetail> salaryStructureDetails = null;
            IList<PRM_EmpSalaryDetail> empSalaryDetails = _empService.GetEmpSalaryDetailsByGradeAndStepId(gradeId, stepId, empId, out salaryStructureId);
            var salaryHeads = _empService.PRMUnit.SalaryHeadRepository.Fetch().ToList();
            salaryStructureDetails = _salaryStructureService.GetSalaryStrutureDetailsByGradeAndStepId(gradeId, stepId, out salaryStructureId);

            if (empSalaryDetails.Count == 0) //new
            {


                foreach (var item in salaryStructureDetails)
                {
                    var ssdModel = item.ToModel();
                    ssdModel.AmountType = item.AmountType;

                    ssdModel.HeadAmountTypeList = Common.GetAmountType().ToList();

                    ssdModel.DisplayHeadName = salaryHeads.Find(x => x.Id == item.HeadId).HeadName;

                    ssdModel.IsGrossPayHead = salaryHeads.Find(x => x.Id == item.HeadId).IsGrossPayHead;

                    // check consolidated structure, if yes then amount type should be percent and amount must be zero
                    if (ssdModel.IsGrossPayHead == true && IsConsolidated == true)
                    {
                        ssdModel.AmountType = "Percent";
                        ssdModel.Amount = Math.Round(Convert.ToDecimal(0), 2);
                    }

                    model.SalaryStructureDetail.Add(ssdModel);
                }
            }
            else //existing
            {

                foreach (var item in empSalaryDetails)
                {
                    var ssdModel = item.ToModel();
                    ssdModel.AmountType = item.AmountType;
                    ssdModel.HeadAmountTypeList = Common.GetAmountType().ToList();
                    ssdModel.DisplayHeadName = salaryHeads.Find(x => x.Id == item.HeadId).HeadName;
                    ssdModel.IsGrossPayHead = salaryHeads.Find(x => x.Id == item.HeadId).IsGrossPayHead;
                    ssdModel.cssSalaryHeadClass = "";
                    model.SalaryStructureDetail.Add(ssdModel);
                }

                #region Adding new salary heads from Salary Structure Template -------------------------

                var empSalaryHeadIdList = (from h in empSalaryDetails select h.HeadId).ToList();
                var salaryAbsenceHeadList = (from ah in salaryStructureDetails
                                             where !empSalaryHeadIdList.Contains(ah.HeadId)
                                             select ah).ToList();

                if (salaryAbsenceHeadList.Count > 0)
                {
                    foreach (var newSalaryHead in salaryAbsenceHeadList)
                    {
                        var obj = new SalaryStructureDetailsModel();
                        obj.Id = newSalaryHead.Id;
                        obj.HeadId = newSalaryHead.HeadId;
                        obj.HeadType = newSalaryHead.HeadType;
                        obj.EmployeeId = model.EmployeeId;
                        obj.AmountType = newSalaryHead.AmountType;

                        obj.HeadAmountTypeList = Common.GetAmountType().ToList();

                        obj.DisplayHeadName = salaryHeads.Find(x => x.Id == newSalaryHead.HeadId).HeadName;

                        obj.IsGrossPayHead = salaryHeads.Find(x => x.Id == newSalaryHead.HeadId).IsGrossPayHead;

                        obj.Amount = 0;
                        obj.IsTaxable = newSalaryHead.IsTaxable;
                        obj.cssSalaryHeadClass = "cssSalaryHeadClass";

                        model.SalaryStructureDetail.Add(obj);
                    }
                }
                #endregion
            }

            model.TotalAddition = model.SalaryStructureDetail.Where(s => s.HeadType == "Addition").Sum(x => x.Amount);
            model.TotalDeduction = model.SalaryStructureDetail.Where(s => s.HeadType == "Deduction").Sum(x => x.Amount);
            model.NetPay = model.TotalAddition - model.TotalDeduction;
            model.SalaryStructureId = salaryStructureId;

            return PartialView("_SalaryStructureDetail", model);
        }
        #endregion

        #region Dropdown

        [NoCache]
        public ActionResult GetDesignation()
        {
            var designations = _empService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();
            return PartialView("Select", Common.PopulateDllList(designations));
        }

        [NoCache]
        public ActionResult GetDivision()
        {
            var divisions = _empService.PRMUnit.DivisionRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Id).ToList();
            return PartialView("Select", Common.PopulateDllList(divisions));
        }

        [NoCache]
        public ActionResult GetJobLocation()
        {
            var jobLocations = _empService.PRMUnit.JobLocationRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(jobLocations));
        }

        [NoCache]
        public ActionResult GetGrade()
        {
            //var grades = _empService.PRMUnit.JobGradeRepository.GetAll().OrderBy(x => x.GradeName).ToList();
            //return PartialView("Select", Common.PopulateJobGradeDDL(grades));
            return PartialView("Select", Common.PopulateCurrentJobGradeDDL(_empService));
        }

        [NoCache]
        public ActionResult GetEmploymentType()
        {
            var grades = _empService.PRMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetStaffCategory()
        {
            var grades = _empService.PRMUnit.PRM_StaffCategoryRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetResource()
        {
            var grades = _empService.PRMUnit.ResourceLevelRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetEmployeeStatus()
        {

            var empStatus = _empService.PRMUnit.EmploymentStatusRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(empStatus));
        }

        [NoCache]
        private void populateDropdown(EmploymentInfoViewModel model)
        {
            model.ReligionList = Common.PopulateReligionDDL(_empService.PRMUnit.Religion.GetAll().OrderBy(x => x.SortOrder));

            #region Gender ddl

            model.GenderList = Common.PopulateGenderDDL(model.GenderList);

            #endregion

            #region Title
            var titleList = _empService.PRMUnit.NameTitleRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.TitleList = Common.PopulateDllList(titleList);
            #endregion

            #region StaffCategory

            var staffList = _empService.PRMUnit.PRM_StaffCategoryRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.StaffCategoryList = Common.PopulateDllList(staffList);

            #endregion

            #region Job Grade

            // Remove due to cascading load on salary scale
            //var jobGradeList = _empService.PRMUnit.JobGradeRepository.Fetch().ToList();
            //model.JobGradeList = Common.PopulateJobGradeDDL(jobGradeList);

            model.JobGradeList = Common.PopulateJobGradeDDL(_JobGradeService.GetLatestJobGrade());

            #endregion

            #region Salary Scale

            //var salScalelist = _empService.PRMUnit.SalaryScaleRepository.Fetch().ToList();
            //model.SalaryScaleList = Common.PopulateSalaryScaleDDL(salScalelist);

            #endregion

            #region Status Designation

            //var desigList = _empService.PRMUnit.DesignationRepository.Get(m => m.Id == model.DesignationId).OrderBy(x => x.Name).ToList();
            //model.DesignationList = Common.PopulateDllList(desigList);

            #endregion

            #region working Designation
            //IList<PRM_StatusDesignationInfo> itemList;
            //itemList = _empService.PRMUnit.StatusDesignationInfoRepository.Get().OrderBy(x => x.SortOrder).ToList();

            //var list = new List<SelectListItem>();
            //foreach (var item in itemList)
            //{
            //    list.Add(new SelectListItem()
            //    {
            //        Text = item.StatusDesignationName,
            //        Value = item.Id.ToString()
            //    });
            //}

            model.StatusDesignationList = Common.PopulateDllList(_empService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList());

            #endregion

            #region Division
            var divList = _empService.PRMUnit.DivisionRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Id).ToList();
            model.DivisionList = Common.PopulateDllList(divList);
            #endregion

            #region JobLocation
            var jobLocList = _empService.PRMUnit.JobLocationRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.JobLocationList = Common.PopulateDllList(jobLocList);

            #endregion

            #region Discipline
            var disList = _empService.PRMUnit.DisciplineRepository.Fetch().OrderBy(x => x.Id).ToList();
            model.DisciplineList = Common.PopulateDllList(disList);

            #endregion

            //#region ResourceLevel
            //var resList = _empService.PRMUnit.ResourceLevelRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            //model.ResourceLevelList = Common.PopulateDllList(resList);
            //#endregion

            //#region working shift
            //var shiftList = _empService.PRMUnit.ShiftNameRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            //model.ShiftList = Common.PopulateDllList(shiftList);
            //#endregion

            #region Employment type
            var empTypeList = _empService.PRMUnit.EmploymentTypeRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.EmploymentTypeList = Common.PopulateDllList(empTypeList);
            #endregion

            #region ContractualType
            model.ContractualTypeList = Common.PopulateContuctualTypeList();
            #endregion

            #region Bank Name
            
            var bankNameList = _pgmCommonservice.PGMUnit.AccBankInfo.Fetch().OrderBy(x => x.bankName).ToList();
            model.BankList = Common.PopulateDDLBankList(bankNameList);
            #endregion

            #region Branch Name
            var branchNameList = _pgmCommonservice.PGMUnit.AccBankBranchInfo.Fetch().OrderBy(x => x.branchName).ToList();
            model.BankBranchList = Common.PopulateDDLBankBranchList(branchNameList);
            #endregion

            #region Employment status
            var empStatusList = _empService.PRMUnit.EmploymentStatusRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.EmploymentStatusList = Common.PopulateDllList(empStatusList);
            #endregion

            #region Region

            //var regionList = _empService.PRMUnit.RegionRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            //model.RegionList = Common.PopulateDllList(regionList);

            model.TaxRegionList = _pgmCommonservice.PGMUnit.TaxRegionRuleRepository.GetAll().Where(x => x.IsActive == true).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.RegionName,
                    Value = y.Id.ToString()
                }).ToList();
            model.SalaryWithdrawFromList = _empService.PRMUnit.ZoneInfoRepository.GetAll().OrderBy(x => x.SortOrder).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ZoneName,
                    Value = y.Id.ToString()
                }).ToList();
            #endregion

            Dictionary<int, string> assesseeList = Common.GetEnumAsDictionary<PGMEnum.TaxAssesseeType>();
            foreach (KeyValuePair<int, string> item in assesseeList)
            {
                model.AssesseTypeList.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }


            // Quota Name
            model.QuotaList = Common.PopulateDllList(_empService.PRMUnit.QuotaNameRepository.GetAll().OrderBy(x => x.SortOrder));

            // Employee Class
            //  model.EmployeeClassList = Common.PopulateDllList(_empService.PRMUnit.EmployeeClassRepository.GetAll().OrderBy(x => x.Name));

            // Employee Process
            model.EmploymentProcessList = Common.PopulateDllList(_empService.PRMUnit.EmploymentProcessRepository.GetAll().OrderBy(x => x.SortOrder));

            //Organogram Level Detail
            //if (model.OrganogramLevelId != null)
            //{
            //    model.OrganogramLevelDetail = _empService.GetOrganogramHierarchyLabel(Convert.ToInt32(model.OrganogramLevelId));
            //}

        }

        [NoCache]
        private void populateDropdown(EmployeeSalaryStructureViewModel model)
        {
            var employeeInfos = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);

            #region Salary Scale

            model.SalaryScaleId = employeeInfos.SalaryScaleId;
            model.SalaryScale = (from JG in _empService.PRMUnit.SalaryScaleRepository.GetAll()
                                 where JG.Id == employeeInfos.SalaryScaleId
                                 select JG.SalaryScaleName).FirstOrDefault();
            //model.SalaryScaleList = Common.PopulateSalaryScaleDDL(_empService.PRMUnit.SalaryScaleRepository.GetAll().OrderBy(x => x.DateOfEffective).ToList());

            #endregion

            #region Job Grade
            model.JobGrade = (from JG in _empService.PRMUnit.JobGradeRepository.GetAll()
                              where JG.Id == employeeInfos.JobGradeId
                              select JG.GradeName).FirstOrDefault();

            model.GradeId = employeeInfos.JobGradeId;

            //int staffCategory = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId).StaffCategoryId;
            //model.GradeList = Common.PopulateJobGradeDDL(_empService.PRMUnit.JobGradeRepository.Get(q => q.StaffCategoryId == staffCategory && q.IsConsolidated == model.isConsolidated).ToList());
            #endregion

            #region Step Number
            dynamic ddlList;
            ddlList = _empService.PRMUnit.JobGradeStepRepository.Get(d => d.JobGradeId == employeeInfos.JobGradeId).OrderBy(x => x.Id).ToList();
            model.StepList = Common.PopulateStepList(ddlList);

            // if consolidated then first setp number shoud be selected
            if (model.isConsolidated == true)
            {
                if (model.GradeId > 0 && model.StepId > 0)
                {
                    foreach (var item in model.StepList)
                    {
                        if (item.Value == model.StepId.ToString()) item.Selected = true;

                    }
                }
            }

            #endregion
        }

        [NoCache]
        public ActionResult GetYesNoAsList()
        {
            return PartialView("Select", Common.PopulateYesNoDDLList());
        }


        #endregion

        #endregion

        #region Cascading DDL -----------------------

        public string GetJobGradesByStaffCategoryID(int id)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });

            var items = (from JG in _empService.PRMUnit.JobGradeRepository.Fetch()
                         //where JG.StaffCategoryId == id
                         select JG).OrderBy(o => o.GradeName).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    var listItem = new SelectListItem { Text = item.GradeName, Value = item.Id.ToString() };
                    listItems.Add(listItem);
                }
            }
            return new JavaScriptSerializer().Serialize(listItems);
        }

        public string GetEmployeeDesignationByGradeID(int id)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });

            var items = (from JG in _empService.PRMUnit.DesignationRepository.Fetch()
                         where JG.GradeId == id
                         select JG).OrderBy(o => o.Name).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    var listItem = new SelectListItem { Text = item.Name, Value = item.Id.ToString() };
                    listItems.Add(listItem);
                }
            }
            return new JavaScriptSerializer().Serialize(listItems);
        }

        public string GetDesignationByOrganogramLevelId(int id)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });

            var items = (from OrgLevel in _empService.PRMUnit.OrganizationalSetupManpowerInfoRepository.Fetch()
                         join de in _empService.PRMUnit.DesignationRepository.Fetch() on OrgLevel.DesignationId equals de.Id
                         where OrgLevel.OrganogramLevelId == id
                         select de).OrderBy(o => o.Name).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    var listItem = new SelectListItem { Text = item.Name, Value = item.Id.ToString() };
                    listItems.Add(listItem);
                }
            }
            return new JavaScriptSerializer().Serialize(listItems);
        }

        public string GetJobGradeByDesignaitonId(int id)
        {
            var items = (from de in _empService.PRMUnit.DesignationRepository.Fetch()
                         join JG in _empService.PRMUnit.JobGradeRepository.Fetch() on de.GradeId equals JG.Id
                         where de.Id == id
                         select new
                         {
                             GradeId = de.GradeId,
                             GradeName = JG.GradeName
                         }).FirstOrDefault();

            return new JavaScriptSerializer().Serialize(items);
        }

        public string GetEmployeeClassByDesignaitonId(int id)
        {
            var items = (from de in _empService.PRMUnit.DesignationRepository.Fetch()
                         join empClass in _empService.PRMUnit.EmployeeClassRepository.Fetch() on de.EmployeeClassId equals empClass.Id
                         where de.Id == id
                         select new
                         {
                             EmployeeClassId = de.EmployeeClassId,
                             EmployeeClassName = empClass.Name
                         }).FirstOrDefault();

            return new JavaScriptSerializer().Serialize(items);
        }

        public string GetSalaryScaleByJobGradeId(int id)
        {
            var items = (from JG in _empService.PRMUnit.JobGradeRepository.Fetch()
                         join SL in _empService.PRMUnit.SalaryScaleRepository.Fetch() on JG.SalaryScaleId equals SL.Id
                         where JG.Id == id
                         select new
                         {
                             SalaryScaleId = JG.SalaryScaleId,
                             SalaryScaleName = SL.SalaryScaleName
                         }).FirstOrDefault();

            return new JavaScriptSerializer().Serialize(items);
        }

        public string GetJobGradeBySalaryScaleId(int id)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });

            var items = (from JG in _empService.PRMUnit.JobGradeRepository.GetAll()
                         where JG.SalaryScaleId == id
                         //orderby Convert.ToInt32(JG.GradeName)
                         select JG).ToList();

            items = items.OrderBy(a => a.GradeName).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    var listItem = new SelectListItem { Text = item.GradeName, Value = item.Id.ToString() };
                    listItems.Add(listItem);
                }
            }
            return new JavaScriptSerializer().Serialize(listItems);
        }

        [NoCache]
        public string GetBankBranchByBankId(int id)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });

            var items = (from entity in _pgmCommonservice.PGMUnit.AccBankBranchInfo.Fetch()
                         where entity.bankId == id
                         select entity).OrderBy(o => o.branchName).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    listItems.Add(new SelectListItem
                    {
                        Text = item.branchName,
                        Value = item.id.ToString()
                    });
                }
            }

            return new JavaScriptSerializer().Serialize(listItems);
        }


        /*RH#01*/
        public string GetResourceLevelIDByGradeID(int id)
        {
            //Comment due to elimanate ResourceLevelId
            //var item = _empService.PRMUnit.JobGradeRepository.GetByID(id);

            //if (item == null)
            //    return "0";
            //else
            //    return item.ResourceLevelId.ToString();
            return "0";
        }
        /*End RH#01*/
        #endregion

        #region Utils

        //private IList<String> CheckBusinessRule(

        private List<string> CheckEmpSalaryBusinessRule(EmployeeSalaryStructureViewModel model)
        {
            var errorList = new List<string>();

            if (model.SalaryStructureDetail.Count == 0)
            {
                errorList.Add("No salary details found !!");

                return errorList;
            }

            if (model.isConsolidated && model.GrossSalary == 0)
            {
                errorList.Add("For consolidated structure, Gross salary must be entered by the user.");
            }
            //if (model.isConsolidated != _empService.PRMUnit.JobGradeRepository.GetByID(model.GradeId).IsConsolidated)
            //{
            //    if (model.isConsolidated) errorList.Add("Please select consolidated grade from grade list for this employee.");
            //    else errorList.Add("Please select non consolidated grade from grade list.");
            //}

            if (model.SalaryStructureDetail.Where(q => q.AmountType == "Percent" && q.Amount > 100).Count() > 0)
            {
                errorList.Add("Amount can't exceed 100 for amount type 'Percent'.");
            }

            if (model.SalaryStructureDetail.Where(q => _empService.PRMUnit.SalaryHeadRepository.GetByID(q.HeadId).IsGrossPayHead == true
                && q.AmountType == "Percent").Sum(q => q.Amount) > 100)
            {
                errorList.Add("Total Gross Pay Head Amount can't exceed 100%.");
            }


            //var summationOfGrossPayhead = _empService.GetSumOfGrossPayHeadByEmpId(model.SalaryStructureId);
            //if (Math.Round(summationOfGrossPayhead).CompareTo(Math.Round(model.GrossSalary)) != 0)

            if (model.isConsolidated)
            {
                if (Math.Round(model.OrgGrossSalary).CompareTo(Math.Round(model.GrossSalary)) != 0)
                    errorList.Add("Gross salary must be equal to the summation of all gross pay head (" + model.OrgGrossSalary + ").");
            }

            return errorList;
        }

        private string CheckEmpInfoBusinessRule(EmploymentInfoViewModel model)
        {
            if (model.DateofJoining != null && model.DateofBirth >= model.DateofJoining)
            {
                return "Date of Birth (" + Convert.ToDateTime(model.DateofBirth).ToString(DateAndTime.GlobalDateFormat) + ") should be lower than Joining Date(" + model.DateofJoining.ToString(DateAndTime.GlobalDateFormat) + ")";
            }

            if (model.DateofConfirmation.HasValue && model.DateofConfirmation.Value < model.DateofJoining)
                return "Date of confirmation must be grater than the date of joining.";

            if (model.IsContractual && model.DateofConfirmation.HasValue)
                return "Confirmation date is not allowed for contractual employee.";

            if ((model.IsContractual || model.EmploymentTypeId == 2) && model.ContractExpireDate == null)
                return "Please enter contract end date.";

            if ((model.IsContractual || model.EmploymentTypeId == 2) && model.ContractExpireDate != null)
            {
                if (model.DateofJoining != null && model.ContractExpireDate <= model.DateofJoining)
                {
                    return "Date of contract end date must be grater than the date of joining.";
                }
            }
            if (model.DateofPosition < model.DateofJoining)
                return "Date of present job position must be equal or greater than the date of joining.";

            //if (model.IsOvertimeEligible && model.OvertimeRate <= 0)
            //    return "overtime rate must be greater than 0.";

            if (model.IsContractual == true && model.DateofConfirmation != null)
            {
                return "Contractual employee has to no confirmation date.";
            }

            //if (model.IsContractual == false && model.IsConsultant == false && model.DateofConfirmation == null)
            //{
            //    return "Regular employee must have confirmation date.";
            //}
            if (model.SelectedEmploymentType.ToLower() == PRMEnum.EmploymentType.Permanent.ToString().ToLower() && model.DateofConfirmation == null)
            {
                return "Permanent employee must have confirmation date.";
            }

            //if (model.IsConsultant == true )
            //{
            //    if (model.BudgetRate <= 0 || model.BudgetRate == null)
            //    {
            //        return "Budget rate must be greater than zero (0) for consultant.";
            //    }
            //    if (model.ActualRate <= 0 || model.ActualRate == null)
            //    {
            //        return "Actual rate must be greater than zero (0) for consultant.";
            //    }
            //}

            // for update
            if (model.Id > 0)
            {
                /// Is salary exist
                var salaryExisting = _empService.PRMUnit.EmpSalaryRepository.Get(x => x.EmployeeId == model.Id).FirstOrDefault();
                if (salaryExisting != null)
                {
                    model.isExist = true;

                    if (salaryExisting.isConsolidated == true && model.EmploymentTypeId != 2) // 2=contructual employee
                    {
                        return "Contructual salary structure is assigned already! please, select appropriate employment type.";
                    }
                    else if (salaryExisting.isConsolidated == false && model.EmploymentTypeId == 2)
                    {
                        return "Regular salary structure is assigned already! please, select appropriate employment type.";
                    }
                }

                // Restricting change of salary zone if salary process already done.
                var currentSalaryZone = _pgmCommonservice.PGMUnit.FunctionRepository.GetEmployeeById(model.Id)
                    .SalaryWithdrawFromZoneId;
                var proposedSalaryZone = model.SalaryWithdrawFromZoneId;
                if (currentSalaryZone != proposedSalaryZone)
                {
                    string query = String.Empty;
                    query = @"SELECT * FROM PGM_Salary sal";
                    query += " WHERE sal.EmployeeId = " + model.Id;
                    query += " AND '" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MMM-dd") +
                             "' = Convert (Date, sal.SalaryYear + ' ' + sal.SalaryMonth + ' " + DateTime.Now.Day + "')";

                    var salaryChecking = _pgmCommonservice.PGMUnit.SalaryMasterRepository.GetWithRawSql(query)
                        .FirstOrDefault();

                    if (salaryChecking != null)
                    {
                        return "Salary has been processed for this month(" + salaryChecking.SalaryMonth + "/" + salaryChecking.SalaryYear + ")." + Environment.NewLine
                               + " You cannot not change salary zone this month. Please try next month or rollback salary first. ";
                    }
                }
            }

            return string.Empty;
        }

        private string CheckEmpContractBusinessRule(EmploymentContractPeriodViewModel model, string strMode)
        {
            if (model.ContractEndDate <= model.ContractStartDate)
                return "Contract end date must be greater than the date of contract start date.";

            var empJoiningDate = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmpoyeeId).DateofJoining;
            if (model.ContractStartDate < empJoiningDate)
                return "Contract start date must be equal to or greater than date of joining.";

            if (model.Id > 0)
            {
                var entityList = _empService.PRMUnit.EmploymentContractInfoRepository.Get(q => q.EmpoyeeId == model.EmpoyeeId && q.Id != model.Id).OrderByDescending(q => q.ContractEndDate).ToList();
                if (entityList.Count > 0)
                {
                    var dd = entityList.FirstOrDefault();
                    if (model.ContractStartDate <= dd.ContractEndDate)
                        return "Contract start date must be greater than previous contract end date.";
                }
            }
            else
            {
                var entityList = _empService.PRMUnit.EmploymentContractInfoRepository.Get(q => q.EmpoyeeId == model.EmpoyeeId).OrderByDescending(q => q.ContractEndDate).ToList();
                if (entityList.Count > 0)
                {
                    var dd = entityList.FirstOrDefault();
                    if (model.ContractStartDate <= dd.ContractEndDate)
                        return "Contract start date must be greater than previous contract end date.";
                }
            }

            if (_empService.GetEmployeeContactByDareRange(model.ContractStartDate, Convert.ToDateTime(model.ContractEndDate), model.Id, model.EmpoyeeId, strMode))
            {
                return "Contract period is not valid.";
            }

            return string.Empty;
        }

        [NoCache]
        [HttpGet]
        public string IsEmpIdAvailable(string empId)
        {
            bool isEmployeeExist = false;
            var emp = _empService.PRMUnit.EmploymentInfoRepository.Get(e => e.EmpID == empId);
            if (emp == null || !emp.Any())
            {
                isEmployeeExist = false;
            }
            else
            {
                isEmployeeExist = true;
            }

            return new JavaScriptSerializer().Serialize(isEmployeeExist.ToString());
        }

        #endregion

        #region password encryption
        public static string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
                //sBuilder.Append(Convert.ToString(data[i], 2).PadLeft(8, '0')); //Convert into binary
            }

            // Return the hexadecimal string.
            return sBuilder.ToString().ToLower();
        }

        // Verify a hash against a string.
        public static bool verifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = getMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public static string GeneratePassword(int lowercase, int uppercase, int numerics)
        {
            string lowers = "abcdefghijklmnopqrstuvwxyz";
            string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string number = "0123456789";

            Random random = new Random();

            string generated = "!";
            for (int i = 1; i <= lowercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    lowers[random.Next(lowers.Length - 1)].ToString()
                );

            for (int i = 1; i <= uppercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    uppers[random.Next(uppers.Length - 1)].ToString()
                );

            for (int i = 1; i <= numerics; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    number[random.Next(number.Length - 1)].ToString()
                );

            return generated.Replace("!", string.Empty);

        }
        #endregion

        #region email send
        public void SendEmail(string address, string subject, string message)
        {
            var smtp = WebConfigurationManager.AppSettings["smtp"];

            string email = WebConfigurationManager.AppSettings["FromEmail"];
            string password = WebConfigurationManager.AppSettings["FromEmailPassword"];

            var loginInfo = new NetworkCredential(email, password);
            var msg = new MailMessage();
            //var smtpClient = new SmtpClient("smtp.gmail.com", 587);

            var smtpClient = new SmtpClient(smtp);

            msg.From = new MailAddress(email);
            msg.To.Add(new MailAddress(address));
            msg.Subject = subject;
            msg.Body = message;
            msg.IsBodyHtml = true;

            smtpClient.EnableSsl = false;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = loginInfo;
            smtpClient.Send(msg);
        }
        #endregion
    }
}
