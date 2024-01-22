using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using System.Collections.ObjectModel;
using BEPZA_MEDICAL.Web.Utility;
using System.Collections;
using System.ComponentModel;
using System.Data.SqlClient;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PGM;
using BEPZA_MEDICAL.DAL.PGM;
using System.IO;
using System.Web;
using BEPZA_MEDICAL.Utility;
using MyNotificationLib.Operation;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class EmployeeSeperationController : Controller
    {
        #region Fields
        private readonly EmployeeSeperationService _Service;
        private readonly PGMCommonService _pgmCommonservice;
        #endregion

        #region Constructor

        public EmployeeSeperationController(EmployeeSeperationService service, PGMCommonService pgmCommonservice)
        {
            this._Service = service;
            this._pgmCommonservice = pgmCommonservice;
        }

        #endregion

        #region Actions

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult GetList(JqGridRequest request, EmployeeSeperationViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<EmployeeSeperationViewModel> list = (from empSeparation in _Service.PRMUnit.EmpSeperationRepository.GetAll()
                                                      join empInfo in _Service.PRMUnit.EmploymentInfoRepository.GetAll() on empSeparation.EmployeeId equals empInfo.Id
                                                      where 
                                                      (string.IsNullOrEmpty(viewModel.EmployeeName) || empInfo.FullName.Contains(viewModel.EmployeeName.ToUpper()))
                                                      && (viewModel.ApplicationDate == null || viewModel.ApplicationDate == empSeparation.ApplicationDate)
                                                      select new EmployeeSeperationViewModel()
                                                         {
                                                             Id = empSeparation.Id,
                                                             EmployeeId = empInfo.Id,
                                                             EmpId = empInfo.EmpID,
                                                             EmployeeName = empInfo.FullName,
                                                             Designation = empInfo.PRM_Designation.Name,
                                                             Type = empSeparation.Type,
                                                             ApplicationDate = empSeparation.ApplicationDate,
                                                             EffectiveDate = empSeparation.EffectiveDate
                                                         }).OrderBy(x => x.ApplicationDate).ToList();

            if (request.Searching)
            {
                if (viewModel.Type != "0")
                {
                    list = list.Where(d => d.Type == viewModel.Type).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.EmpId))
                {
                    list = list.Where(d => d.EmpId.Contains(viewModel.EmpId)).ToList();
                }
                if ((viewModel.EffectiveDateFrom != null && viewModel.EffectiveDateFrom != DateTime.MinValue) && (viewModel.EffectiveDateTo != null && viewModel.EffectiveDateTo != DateTime.MinValue))
                {
                    list = list.Where(d => d.EffectiveDateFrom >= viewModel.EffectiveDateFrom && d.EffectiveDateTo <= viewModel.EffectiveDateTo).ToList();
                }

                if ((viewModel.EffectiveDateFrom != null && viewModel.EffectiveDateFrom != DateTime.MinValue))
                {
                    list = list.Where(d => d.EffectiveDate >= viewModel.EffectiveDateFrom).ToList();
                }

                if ((viewModel.EffectiveDateTo != null && viewModel.EffectiveDateTo != DateTime.MinValue))
                {
                    list = list.Where(d => d.EffectiveDate <= viewModel.EffectiveDateTo).ToList();
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

                response.Records.Add(new JqGridRecord(Convert.ToString(d.EmployeeId), new List<object>()
                {   
                    d.Id,            
                    d.EmployeeId,
                    d.EmpId,
                    d.EmployeeName,
                    d.Designation,
                    d.Type,
                    d.ApplicationDate == null? "": Convert.ToDateTime(d.ApplicationDate).ToString("dd-MM-yyyy"),  
                    Convert.ToDateTime(d.EffectiveDate).ToString("dd-MM-yyyy"),  
                    "",
                    "",
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult TypeforView()
        {
            var itemlist = Common.PopulateSeparationTypeList().OrderBy(x => x.Text);
            return PartialView("Select", itemlist);
        }

        [NoCache]
        public JsonResult GetEmployeeInfoTemp(int empId)
        {
            var obj = _Service.PRMUnit.EmploymentInfoRepository.GetByID(empId);
            return Json(new
            {
                EmpId = obj.EmpID,
                Designation = obj.PRM_Designation.Name,
                EmployeeName = obj.FullName,
                DateofJoining = obj.DateofJoining.ToString(DateAndTime.GlobalDateFormat),
                IsContractual = obj.IsContractual,
                PreviousEmploymentStatusId = obj.EmploymentStatusId,
                PreviousEmploymentStatus = obj.PRM_EmploymentType.Name,
                MetterNo = string.Empty,
                DepartmentName =obj.PRM_Division == null? string.Empty: obj.PRM_Division.Name
            });
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(EmployeeSeperationViewModel model)
        {
            var obj = _Service.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
            return Json(new
            {
                EmpId = obj.EmpID,
                Designation = obj.PRM_Designation.Name,
                EmployeeName = obj.FullName,
                DateofJoining = obj.DateofJoining.ToString(DateAndTime.GlobalDateFormat),
                IsContractual = obj.IsContractual,
                PreviousEmploymentStatusId = obj.EmploymentStatusId,
                MetterNo = string.Empty,
                DepartmentName = obj.PRM_Division.Name
            });

        }

        public ActionResult Create()
        {
            EmployeeSeperationViewModel model = new EmployeeSeperationViewModel();
            model.isAddAttachment = true;
            model.ShortageDays = 0;
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] EmployeeSeperationViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                var attachment = Request.Files["attachment"];

                if (ModelState.IsValid)
                {
                    var obj = model.ToEntity();
                    obj.IUser = User.Identity.Name;
                    obj.IDate = Common.CurrentDateTime;

                    //model.IsError = 1;
                    //errorList = _Service.GetBusinessLogicValidation(obj).FirstOrDefault();

                    HttpFileCollectionBase files = Request.Files;
                    foreach (string fileTagName in files)
                    {
                        HttpPostedFileBase file = Request.Files[fileTagName];
                        if (file.ContentLength > 0)
                        {
                            // Due to the limit of the max for a int type, the largest file can be
                            // uploaded is 2147483647, which is very large anyway.
                            int size = file.ContentLength;
                            string name = file.FileName;
                            int position = name.LastIndexOf("\\");
                            name = name.Substring(position + 1);
                            string contentType = file.ContentType;
                            byte[] fileData = new byte[size];
                            file.InputStream.Read(fileData, 0, size);
                            obj.FileName = name;
                            obj.Attachment = fileData;
                            //EmpFileUtl.SaveFile(model.EmployeeId, model.AttachmentTypeId, model.FileName, model.Description, name, contentType, size, fileData, User.Identity.Name);
                        }
                    }

                    if (string.IsNullOrEmpty(errorList))
                    {
                        if (Upload(model) == 0)
                        {
                            try
                            {
                                _Service.PRMUnit.EmpSeperationRepository.Add(obj);
                                _Service.PRMUnit.EmpSeperationRepository.SaveChanges();

                                if (model.Type.Contains("Rejoin"))
                                {
                                    if (obj.EffectiveDate <= DateTime.Now.Date)
                                    {
                                        _Service.PRMUnit.EmploymentInfoRepository.Update(UpdateEmployeeInfoForRejoin(model));
                                        _Service.PRMUnit.EmploymentInfoRepository.SaveChanges();
                                    }
                                }
                                else
                                {
                                    if (obj.EffectiveDate <= DateTime.Now.Date)
                                    {
                                        _Service.PRMUnit.EmploymentInfoRepository.Update(UpdateEmployeeInfo(model));
                                        _Service.PRMUnit.EmploymentInfoRepository.SaveChanges();
                                    }
                                }
                                model.IsError = 0;
                                model.errClass = "success";
                                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                                // return RedirectToAction("Index");
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
                                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                                }
                            }

                            if (model.IsError == 0)
                            {
                                try
                                {
                                    Notification(model.EmployeeId, model.Type, model.EffectiveDate, model.NotifyTo);
                                }
                                catch (Exception) { }
                            }
                        }
                    }
                    else
                    {
                        model.ErrMsg = errorList;
                    }
                }
            }
            catch (Exception ex)
            {
                model.IsError = 1;
                model.ErrMsg = "failed";
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }

            populateDropdown(model);
            setEmployeeInfo(model, "I");
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            BEPZA_MEDICAL.DAL.PRM.PRM_EmpSeperation prm_resourceInfo = _Service.PRMUnit.EmpSeperationRepository.GetByID(id, "Id");
            var model = prm_resourceInfo.ToModel();
            DownloadDoc(model);
            populateDropdown(model);
            setEmployeeInfo(model, "E");
            setAuthorityEmployeeInfo(model);
            model.isAddAttachment = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Exclude = "Attachment")]EmployeeSeperationViewModel model)
        {
            string errorList = string.Empty;
            var attachment = Request.Files["attachment"];

            if (ModelState.IsValid)
            {
                // Set preious attachment if exist

                var obj = _Service.PRMUnit.EmpSeperationRepository.GetByID(model.Id, "Id");
                model.Attachment = obj.Attachment == null ? null : obj.Attachment;
                //

                BEPZA_MEDICAL.DAL.PRM.PRM_EmpSeperation entity = model.ToEntity();
                entity.EUser = User.Identity.Name;
                entity.EDate = Common.CurrentDateTime;
                ArrayList lstGradeSteps = new ArrayList();

                //model.IsError = 1;
                //model.ErrMsg = _Service.GetBusinessLogicValidation(obj).FirstOrDefault();

                HttpFileCollectionBase files = Request.Files;

                foreach (string fileTagName in files)
                {
                    HttpPostedFileBase file = Request.Files[fileTagName];
                    if (file.ContentLength > 0)
                    {
                        // Due to the limit of the max for a int type, the largest file can be
                        // uploaded is 2147483647, which is very large anyway.

                        int size = file.ContentLength;
                        string name = file.FileName;
                        int position = name.LastIndexOf("\\");
                        name = name.Substring(position + 1);
                        string contentType = file.ContentType;
                        byte[] fileData = new byte[size];
                        file.InputStream.Read(fileData, 0, size);
                        entity.FileName = name;
                        entity.Attachment = fileData;
                    }
                    else
                    {

                    }
                }

                if (string.IsNullOrEmpty(model.ErrMsg))
                {
                    if (Upload(model) == 0)
                    {
                        try
                        {
                            Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                            _Service.PRMUnit.EmpSeperationRepository.Update(entity, "Id", NavigationList);
                            _Service.PRMUnit.EmpSeperationRepository.SaveChanges();

                            if (model.Type.Contains("Rejoin"))
                            {
                                if (obj.EffectiveDate <= DateTime.Now.Date)
                                {
                                    _Service.PRMUnit.EmploymentInfoRepository.Update(UpdateEmployeeInfoForRejoin(model));
                                    _Service.PRMUnit.EmploymentInfoRepository.SaveChanges();
                                }
                            }
                            else
                            {
                                if (entity.EffectiveDate <= DateTime.Now.Date)
                                {
                                    _Service.PRMUnit.EmploymentInfoRepository.Update(UpdateEmployeeInfo(model));
                                    _Service.PRMUnit.EmploymentInfoRepository.SaveChanges();
                                }
                            }
                            model.IsError = 0;
                            model.errClass = "success";
                            model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                            //   return RedirectToAction("Index");
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
                                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                            }
                        }
                    }
                }
                else
                {
                    model.ErrMsg = errorList;
                }
            }
            populateDropdown(model);
            setEmployeeInfo(model, "E");
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;

            string errMsg = string.Empty;
            errMsg = GetBusinessLogicValidation(id);

            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    var empSepObj = _Service.PRMUnit.EmpSeperationRepository.GetByID(id, "Id");
                    var obj = _Service.PRMUnit.EmploymentInfoRepository.GetByID(empSepObj.EmployeeId);

                    _Service.PRMUnit.EmpSeperationRepository.Delete(id, "Id", new List<Type>());
                    _Service.PRMUnit.EmpSeperationRepository.SaveChanges();

                    // update employee info
                    obj.DateofInactive = null;
                    obj.EmploymentStatusId = 1; //active
                    //  obj.EmploymentStatusId = empSepObj.PreviousEmploymentStatusId;

                    _Service.PRMUnit.EmploymentInfoRepository.Update(obj);
                    _Service.PRMUnit.EmploymentInfoRepository.SaveChanges();

                    result = true;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                }
                catch (Exception ex)
                {

                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                    }
                }
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        #endregion

        #region  private method

        private string GetBusinessLogicValidation(int id)
        {
            string message = string.Empty;
            //var CheckOutGratuity = (from g in _pgmCommonservice.PGMUnit.GratuitySettlement.GetAll() where g.EmployeeId == id select g).ToList();
            //if (CheckOutGratuity.Count > 0)
            //{
            //    message = "Can't be deleted, because gratuity settlement has been completed.";
            //}

            var CheckOutFinalSettlement = (from f in _pgmCommonservice.PGMUnit.FinalSettlementRepository.GetAll() where f.EmployeeId == id select f).ToList();
            if (CheckOutFinalSettlement.Count > 0)
            {
                message = "Can't be deleted, because final settlement has been completed.";
            }
            return message;
        }

        private void populateDropdown(EmployeeSeperationViewModel model)
        {
            model.TypeList = Common.PopulateSeparationTypeList();
        }

        private void setEmployeeInfo(EmployeeSeperationViewModel model, string mode)
        {
            if (model.EmployeeId != 0)
            {
                var obj = _Service.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
                model.EmpId = obj.EmpID;
                if (mode == "I")
                {
                    model.PreviousEmploymentStatusId = obj.EmploymentStatusId;
                }

                model.Designation = obj.PRM_Designation.Name;
                model.EmployeeName = obj.FullName;
                model.DateofJoining = obj.DateofJoining;
                model.IsContractual = obj.IsContractual;
                model.PreviousEmploymentStatus = obj.PRM_EmploymentType.Name;
            }
        }

        private void setAuthorityEmployeeInfo(EmployeeSeperationViewModel model)
        {
            if (model.ApprovalEmployeeId != 0)
            {
                var obj = _Service.PRMUnit.EmploymentInfoRepository.GetByID(model.ApprovalEmployeeId);
                model.ApprovalEmpId = obj.EmpID;
                model.ApprovalEmpName = obj.FullName;
                model.ApprovalEmpDesignation = obj.PRM_Designation.Name;

            }
        }

        private PRM_EmploymentInfo UpdateEmployeeInfo(EmployeeSeperationViewModel model)
        {
            var obj = _Service.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
            obj.DateofInactive = model.EffectiveDate;
            var empStatus = _Service.PRMUnit.EmploymentStatusRepository.Get(q => q.Name.ToLower() == "inactive").FirstOrDefault();
            if (empStatus != null) obj.EmploymentStatusId = empStatus.Id;
            return obj;
        }

        private PRM_EmploymentInfo UpdateEmployeeInfoForRejoin(EmployeeSeperationViewModel model)
        {
            var obj = _Service.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
            obj.DateofInactive = null;
            obj.EmploymentStatusId = 1; //active
            return obj;
        }

        #region Attachment

        private int Upload(EmployeeSeperationViewModel model)
        {
            if (model.File == null)
                return 0;

            try
            {
                var uploadFile = model.File;

                byte[] data;
                using (Stream inputStream = uploadFile.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    data = memoryStream.ToArray();
                }
                model.Attachment = data;
                model.FileName = uploadFile.FileName;
                model.IsError = 0;

            }
            catch (Exception ex)
            {
                model.IsError = 1;
                model.ErrMsg = "Upload File Error!";
            }

            return model.IsError;
        }

        public void DownloadDoc(EmployeeSeperationViewModel model)
        {
            byte[] document = model.Attachment;
            if (document != null)
            {
                string strFilename = Url.Content("~/Content/" + User.Identity.Name + model.FileName);
                byte[] doc = document;
                WriteToFile(Server.MapPath(strFilename), ref doc);

                model.FilePath = strFilename;
            }
        }

        private void WriteToFile(string strPath, ref byte[] Buffer)
        {
            FileStream newFile = null;

            try
            {
                newFile = new FileStream(strPath, FileMode.Create);

                newFile.Write(Buffer, 0, Buffer.Length);
                newFile.Close();
            }
            catch (Exception ex)
            {
                if (newFile != null) newFile.Close();
            }
        }

        #endregion
        public JsonResult DiffCalulation(string AppDate, string EffecDate)
        {
            var diff = (Convert.ToDateTime(EffecDate) - Convert.ToDateTime(AppDate)).Days;
            return Json(diff, JsonRequestBehavior.AllowGet);
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

        #endregion

        private void Notification(int employeeId, String separationType, DateTime? effectiveDate, String notifyToEmps)
        {

            #region Notification
            var enumType = MyNotificationLibEnum.NotificationType.Employee_Separation;
            var redirectToUrl = String.Empty;

            // Declare Notification Variables
            var modules = new List<MyNotificationLibEnum.NotificationModule>();

            modules.Add(MyNotificationLibEnum.NotificationModule.Human_Resource_Management_System);

            var toEmployees = new List<int>();

            // Applicant info
            var applicant = _Service.PRMUnit.EmploymentInfoRepository.GetAll()
                .FirstOrDefault(e => e.Id == employeeId);
            var applicantInfo = applicant.FullName + ", " + (applicant.PRM_Designation.Name) + ", " +
                                applicant.EmpID;

            var customMessage = "A separation (" + separationType + ") has issued for " + applicantInfo +
                                " on " + Common.GetDate(effectiveDate).ToString(DateAndTime.GlobalDateFormat) + ".";

            #region Self Notification
            toEmployees.Clear();
            toEmployees.Add(employeeId);

            var notificationForApplicant = new SendGeneralPurposeNotification(
                modules,
                "Your " + separationType + " will be effective from " + Common.GetDate(effectiveDate).ToString(DateAndTime.GlobalDateFormat),
                String.Empty,
                toEmployees,
                MyAppSession.EmpId,
                enumType
            );
            notificationForApplicant.SendNotification();
            #endregion

            #region Notify To
            if (!String.IsNullOrEmpty(notifyToEmps))
            {
                toEmployees.Clear();

                // Notify to employees
                var notifyTo = notifyToEmps.Split(',');
                foreach (String empId in notifyTo)
                {
                    if (_Service.PRMUnit.EmploymentInfoRepository.GetAll()
                        .Any(e => e.EmpID == empId.Trim()))
                    {
                        toEmployees.Add(_Service.PRMUnit.EmploymentInfoRepository.GetAll()
                            .FirstOrDefault(e => e.EmpID == empId.Trim()).Id);
                    }
                }

                var generalPurposeNotification =
                    new SendGeneralPurposeNotification(
                        modules,
                        customMessage,
                        redirectToUrl,
                        toEmployees,
                        MyAppSession.EmpId,
                        enumType
                    );
                generalPurposeNotification.SendNotification();
            }
            #endregion

            #region From notification flow
            try
            {
                var notificationUtil = new SendNotificationByFlowSetup(
                    enumType
                    , employeeId
                    , String.Empty
                    , String.Empty
                    , Common.GetDate(effectiveDate)
                    , null
                    , redirectToUrl
                    , customMessage);
                notificationUtil.SendNotification();
            }
            catch (Exception) { }
            #endregion

            #endregion

        }
    }
}