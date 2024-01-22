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

using BEPZA_MEDICAL.DAL.AMS;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.AMS;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.AMS.ViewModel.AnsarInfo;
using BEPZA_MEDICAL.Web.Areas.AMS.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.SecurityService;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Controllers;

namespace BEPZA_MEDICAL.Web.Areas.AMS.Controllers
{
    public class AnsarInfoController : BaseController
    {
        #region Fields

        private readonly AMSCommonService _amsCommonService;
        private readonly PRMCommonSevice _prmCommonService;

        #endregion

        #region Constructor
        public AnsarInfoController(PRMCommonSevice prmCommonService, AMSCommonService amsCommonService)
        {
            this._prmCommonService = prmCommonService;
            this._amsCommonService = amsCommonService;
        }
        #endregion

        #region Actions

        #region Ansar Search

        public ActionResult Index()
        {
            var model = new AnsarSearchViewModel();
            model.ZoneInfoId = LoggedUserZoneInfoId;
            model.ActionName = "EmploymentInfoIndex";

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, AnsarSearchViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            //if (request.Searching)
            //{
            //    if (viewModel != null)
            //    {
            //        filterExpression = viewModel.GetFilterExpression();
            //    }
            //}

            viewModel.ZoneInfoId = LoggedUserZoneInfoId;
            filterExpression = viewModel.GetFilterExpression();
                
            totalRecords = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetCount(filterExpression);

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling(totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            var list = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.NationalID))
                {
                    list = list.Where(x => x.AMS_AnsarPersonalInfo.Count() > 0 && x.AMS_AnsarPersonalInfo.FirstOrDefault().NationalID != null && x.AMS_AnsarPersonalInfo.FirstOrDefault().NationalID.Contains(viewModel.NationalID)).ToList();
                }
            }


            var serial = 1;
            foreach (var d in list)
            {
                var desigName = string.Empty;
                var NationalID = string.Empty;
                var status = string.Empty;

                if (d.AMS_DesignationInfo != null)
                {
                    desigName = d.AMS_DesignationInfo.DesignationName;
                }
                status = d.AMS_EmpStatus.Name;
                if(d.AMS_AnsarPersonalInfo.Count() > 0)
                {
                    NationalID = d.AMS_AnsarPersonalInfo.FirstOrDefault().NationalID;
                }

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    serial,
                    d.AnsarId,
                    d.FullName,
                    d.BEPZAId,
                    d.DesignationId,
                    desigName,
                    d.AnsarJoiningDate.HasValue ? d.AnsarJoiningDate.Value.ToString(DateAndTime.GlobalDateFormat) : null,
                    d.DateofJoining.ToString(DateAndTime.GlobalDateFormat),
                    d.StatusId,
                    status,
                    NationalID
                }));

                serial++;
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult AnsarSearch()
        {
            var model = new AnsarSearchViewModel();

            return View("AnsarSearch", model);
        }

        public ActionResult BlacklistSearch()
        {
            var model = new AnsarSearchViewModel();

            return View("BlacklistSearch", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetBlacklist(JqGridRequest request, AnsarSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _amsCommonService.AMSUnit.BlacklistRepository.GetAll().Where(x => x.IsRevoked == false && x.AMS_AnsarEmpInfo.ZoneInfoId == LoggedUserZoneInfoId).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.BEPZAId))
                {
                    list = list.Where(x => x.AMS_AnsarEmpInfo.BEPZAId.Contains(viewModel.BEPZAId)).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.FullName))
                {
                    list = list.Where(x => x.AMS_AnsarEmpInfo.FullName.ToLower().Contains(viewModel.FullName.Trim().ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.AnsarId))
                {
                    list = list.Where(x => x.AMS_AnsarEmpInfo.AnsarId.Contains(viewModel.AnsarId)).ToList();
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
                    d.AMS_AnsarEmpInfo.BEPZAId,
                    d.AMS_AnsarEmpInfo.FullName,
                    d.AMS_AnsarEmpInfo.AMS_DesignationInfo.DesignationName,
                    d.AMS_AnsarEmpInfo.AnsarId,
                    d.Reason,
                    d.Date.ToString(DateAndTime.GlobalDateFormat)
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        #endregion

        #region Ansar Basic Information

        #region Insert-----------------------------------

        public ActionResult EmploymentInfoIndex(int? id)
        {
            if (id.HasValue)
                return RedirectToAction("EditEmploymentInfo", "AnsarInfo", new { id = id });

            var parentModel = new AnsarViewModel();
            parentModel.ViewType = "EmploymentInfo";

            populateDropdown(parentModel.EmploymentInfo);

            parentModel.EmploymentInfo.BEPZAID = _amsCommonService.GetNewAnsarBEPZAID();

            //Initially Active
            parentModel.EmploymentInfo.StatusName = "Active";
            parentModel.EmploymentInfo.StatusId = _amsCommonService.AMSUnit.EmpStatusRepository.First(x => x.Name == "Active").Id;

            parentModel.EmploymentInfo.ActionType = "CreateEmploymentInfo";
            parentModel.EmploymentInfo.ButtonText = "Save";
            parentModel.EmploymentInfo.SelectedClass = "selected";

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateEmploymentInfo(EmploymentInfoViewModel model)
        {
            var parentModel = new AnsarViewModel();
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

                    _amsCommonService.AMSUnit.AnsarEmpInfoRepository.Add(entity);
                    _amsCommonService.AMSUnit.AnsarEmpInfoRepository.SaveChanges();

                    parentModel.Id = entity.Id;
                    parentModel.BEPZAId = entity.BEPZAId;

                }
                catch (Exception ex)
                {
                    populateDropdown(model);

                    parentModel.EmploymentInfo = model;
                    parentModel.EmploymentInfo.ButtonText = "Save";
                    parentModel.EmploymentInfo.SelectedClass = "selected";
                    parentModel.EmploymentInfo.ErrorClass = "failed";
                    parentModel.EmploymentInfo.Message = Resources.ErrorMessages.InsertFailed;
                    parentModel.EmploymentInfo.IsError = 1;
                    //InitializationJobGradeAndDesignationForEdit(model);

                    return View("CreateOrEdit", parentModel);
                }
            }
            else
            {
                populateDropdown(model);

                parentModel.EmploymentInfo = model;
                parentModel.EmploymentInfo.ButtonText = "Save";
                parentModel.EmploymentInfo.SelectedClass = "selected";
                parentModel.EmploymentInfo.ErrorClass = "failed";
                parentModel.EmploymentInfo.Message = Resources.ErrorMessages.InsertFailed + " " + error;
                parentModel.EmploymentInfo.IsError = 1;
                //InitializationJobGradeAndDesignationForEdit(model);
                return View("CreateOrEdit", parentModel);
            }

            return RedirectToAction("EditEmploymentInfo", "AnsarInfo", new { id = parentModel.Id, type = "success" });
        }

        #endregion

        #region Update--------------------------------------

        public ActionResult EditEmploymentInfo(int id, string type)
        {
            var parentModel = new AnsarViewModel();
            parentModel.ViewType = "EmploymentInfo";

            var entity = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetByID(id);
            var model = entity.ToModel();

            if (model.DesignationId.HasValue)
            {
                model.IsAnsarEditDesignation = true;
            }

            model.DesignationName = entity.AMS_DesignationInfo == null ? string.Empty : entity.AMS_DesignationInfo.DesignationName;

            if (model.InactiveDate.HasValue)
            {
                model.InactiveDate = model.InactiveDate.Value.Date;
            }

            populateDropdown(model);

            //InitializationJobGradeAndDesignationForEdit(model);

            model.ActionType = "EditEmploymentInfo";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.EmploymentInfo = model;
            parentModel.BEPZAId = model.BEPZAID;
            parentModel.Id = model.Id;

            model.IsPhotoExist = _amsCommonService.IsPhotoExist(model.Id, true);

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
            var parentModel = new AnsarViewModel();
            parentModel.ViewType = "EmploymentInfo";

            model.IsPhotoExist = _amsCommonService.IsPhotoExist(model.Id, true);

            var error = CheckEmpInfoBusinessRule(model);

            if (ModelState.IsValid && error == string.Empty)
            {
                try
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;
                
                    _amsCommonService.AMSUnit.AnsarEmpInfoRepository.Update(entity, new Dictionary<Type, ArrayList>());
                    _amsCommonService.AMSUnit.AnsarEmpInfoRepository.SaveChanges();
                }
                catch (Exception ex)
                {
                    populateDropdown(model);
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "selected";

                    parentModel.EmploymentInfo = model;
                    //InitializationJobGradeAndDesignationForEdit(model);
                    return View("CreateOrEdit", parentModel);
                }

                populateDropdown(model);
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "selected";

                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;

                parentModel.EmploymentInfo = model;
                parentModel.Id = model.Id;
                parentModel.BEPZAId = model.BEPZAID;

                //InitializationJobGradeAndDesignationForEdit(model);

                if (model.DesignationId.HasValue)
                {
                    model.IsAnsarEditDesignation = true;
                }

                return View("CreateOrEdit", parentModel);

            }

            populateDropdown(model);
            model.Message = Resources.ErrorMessages.UpdateFailed + " " + error;
            model.ErrorClass = "failed";
            model.IsError = 1;

            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.EmploymentInfo = model;

            //InitializationJobGradeAndDesignationForEdit(model);

            return View("CreateOrEdit", parentModel);
        }

        //private EmploymentInfoViewModel InitializationJobGradeAndDesignationForEdit(EmploymentInfoViewModel model)
        //{
        //    var desigList = (from JG in _empService.PRMUnit.OrganizationalSetupManpowerInfoRepository.Fetch()
        //                     join de in _empService.PRMUnit.DesignationRepository.Fetch() on JG.DesignationId equals de.Id
        //                     where JG.OrganogramLevelId == model.OrganogramLevelId
        //                     select de).OrderBy(o => o.SortingOrder).ToList();

        //    model.DesignationList = Common.PopulateDllList(desigList);

        //    return model;
        //}

        #endregion

        #region Delete

        public ActionResult Delete(int id)
        {
            var parentModel = new AnsarViewModel();
            parentModel.ViewType = "EmploymentInfo";

            var entity = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetByID(id);
            var model = entity.ToModel();
            if (ModelState.IsValid)
            {
                try
                {
                    _amsCommonService.AMSUnit.AnsarEmpInfoRepository.Delete(entity);
                    _amsCommonService.AMSUnit.AnsarEmpInfoRepository.SaveChanges();
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

                model.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.IsError = 0;
                model.ErrorClass = "success delete-emp";

                parentModel.EmploymentInfo = model;
                return View("CreateOrEdit", parentModel);
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


        #endregion

        #region Employee Photograph/Signature------------------------------------------

        public ActionResult AnsarPhotographIndex(int id, bool isPhoto, string message, bool? isSuccessful)
        {
            var parentModel = new AnsarViewModel();
            var model = parentModel.AnsarPhotograph;

            var entity = _amsCommonService.GetAnsarPhoto(id, isPhoto);
            var ansar = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetByID(id);

            if (entity != null)
            {
                model = entity.ToModel();
                model.ActionType = "AnsarPhotographDelete";
            }
            else
            {
                model.ActionType = "AnsarPhotographCreate";
            }
            model.EmployeeId = id;
            model.IsPhoto = isPhoto;
            model.EmpCode = ansar.AnsarId;
            model.FullName = ansar.FullName;
            model.InactiveDate = ansar.InactiveDate;

            if (isPhoto) model.SelectedClass = "selected";

            parentModel.AnsarPhotograph = model;
            parentModel.Id = model.EmployeeId;

            parentModel.ViewType = "AnsarPhotograph";

            if (!String.IsNullOrEmpty(message))
            {
                model.IsSuccessful = Convert.ToBoolean(isSuccessful);
                model.Message = message;
            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult AnsarPhotographCreate(AnsarPhotoGraphViewModel model, string btnSubmit)
        {
            var parentModel = new AnsarViewModel();

            if (btnSubmit == "Upload")
            {
                model = UploadAnsarPhoto(model);
            }
            if (btnSubmit == "Save")
            {
                model = SaveAnsarPhoto(model);

                if (model.IsSuccessful)
                {
                    model.ActionType = "AnsarPhotographDelete";
                }

            }
            if (model.IsPhoto) model.SelectedClass = "selected";

            parentModel.AnsarPhotograph = model;
            parentModel.Id = model.EmployeeId;
            parentModel.ViewType = "AnsarPhotograph";
            return View("CreateOrEdit", parentModel);
        }

        private AnsarPhotoGraphViewModel SaveAnsarPhoto(AnsarPhotoGraphViewModel model)
        {
            try
            {
                if (model.PhotoSignature != null)
                {
                    AMS_AnsarPhoto entity = new AMS_AnsarPhoto();
                    byte[] buf = model.PhotoSignature;
                    entity.EmployeeId = model.EmployeeId;
                    entity.PhotoSignature = buf;
                    entity.IsPhoto = model.IsPhoto;
                    _amsCommonService.AMSUnit.AnsarPhotographRepository.Add(entity);
                    _amsCommonService.AMSUnit.AnsarPhotographRepository.SaveChanges();
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
        public ActionResult AnsarPhotographDelete(AnsarPhotoGraphViewModel model)
        {
            var parentModel = new AnsarViewModel();

            try
            {
                AMS_AnsarPhoto entity = _amsCommonService.GetAnsarPhoto(model.EmployeeId, model.IsPhoto);
                if (entity != null)
                {
                    _amsCommonService.AMSUnit.AnsarPhotographRepository.Delete(entity);
                    _amsCommonService.AMSUnit.AnsarPhotographRepository.SaveChanges();
                    model.Message = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                    model.IsSuccessful = true;
                    if (model.IsSuccessful)
                    {
                        model.ActionType = "AnsarPhotographCreate";
                    }
                }

                if (model.IsPhoto)
                    model.SelectedClass = "selected";


            }
            catch (Exception)
            {
                model.IsSuccessful = false;
                model.Message = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            }

            parentModel.AnsarPhotograph = model;
            parentModel.Id = model.EmployeeId;
            parentModel.ViewType = "AnsarPhotograph";
            return View("CreateOrEdit", parentModel);
        }


        public FileContentResult GetImage(int? id, bool? isPhoto)
        {
            AMS_AnsarPhoto toDisplay = null;
            if (id != null && id != 0 && isPhoto != null)
            {
                toDisplay = _amsCommonService.GetAnsarPhoto((int)id, (bool)isPhoto);
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

        private AnsarPhotoGraphViewModel UploadAnsarPhoto(AnsarPhotoGraphViewModel model)
        {
            try
            {
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

                            System.IO.Directory.CreateDirectory(Server.MapPath("~/Content/TempFiles"));
                            var path = "~/Content/TempFiles/";

                            //deleting code starts here
                            var temp = Path.GetFileNameWithoutExtension(image.FileName) + ".*";
                            string[] files = System.IO.Directory.GetFiles(Server.MapPath(path), temp);
                            foreach (string f in files)
                            {
                                FileInfo fi = new FileInfo(f);
                                if (fi.IsReadOnly)
                                    fi.IsReadOnly = false; 
                                System.IO.File.Delete(f);
                            }
                            //deleting code ends here
                            
                            image.Save(Path.Combine(path, filename));
                            filename = Path.Combine(path, filename);
                            
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

        #endregion

        #region Personal Information
        public ActionResult PersonaInfoIndex(int id, bool IsMenu = false, string type = "")
        {
            var parentModel = new AnsarViewModel();
            parentModel.ViewType = "PersonalInfo";

            var ansarEntity = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetByID(id);

            var entity = _amsCommonService.AMSUnit.AnsarPersonalInfoRepository.GetAll().Where(x=> x.EmployeeId == id).FirstOrDefault();

            var model = parentModel.PersonalInfo;
            if (entity == null)
            {
                model.DateofBirth = ansarEntity.DateofBirth != null ? Convert.ToDateTime(ansarEntity.DateofBirth) : DateTime.Now;
                model.strMode = "add";
                model.EmployeeId = Convert.ToInt32(id);
                model.ButtonText = "Save";
            }
            else
            {
                model = entity.ToModel();
                model.DeleteEnable = true;
                model.strMode = "edit";
                model.ButtonText = "Update";
                model.DateofBirth = ansarEntity.DateofBirth != null ? Convert.ToDateTime(ansarEntity.DateofBirth) : DateTime.Now;
            }

            parentModel.EmployeeId = id;
            parentModel.Id = model.EmployeeId;
            if (ansarEntity != null)
            {
                model.InactiveDate = ansarEntity.InactiveDate;
                Common.PopulateAnsarTop(model.EmpTop, ansarEntity.Id, _amsCommonService);
            }

            parentModel.PersonalInfo = model;
            PopulateDropdownList(model);

           
            parentModel.PersonalInfo.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.PersonalInfo.Message = Resources.ErrorMessages.DeleteSuccessful;
                parentModel.PersonalInfo.errClass = "success";

            }
            return View("CreateOrEdit", parentModel);

        }

        [HttpPost]
        public ActionResult CreateOrEditPersonaInfo(PersonalInfoViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new AnsarViewModel();
            try
            {
                if (ModelState.IsValid)
                {
                    parentModel.ViewType = "PersonalInfo";
                    parentModel.EmployeeId = model.EmployeeId;
                    parentModel.Id = model.EmployeeId;
                    Common.PopulateAnsarTop(model.EmpTop, model.EmployeeId, _amsCommonService);

                    var pInfo = _amsCommonService.AMSUnit.AnsarPersonalInfoRepository.Get(x => x.EmployeeId == model.EmployeeId).FirstOrDefault();
                    if (pInfo != null)
                    {
                        model.strMode = "edit";
                    }
                    else
                    {
                        model.strMode = "add";
                    }

                    if (ModelState.IsValid)
                    {
                        var entity = model.ToEntity();
                        businessError = _amsCommonService.CheckBusinessLogic(entity);
                        entity.EUser = User.Identity.Name;
                        entity.EDate = Common.CurrentDateTime;

                        if (businessError != string.Empty)
                        {
                            model.Message = businessError;
                            model.errClass = "failed";
                            if (model.strMode == "add")
                            {
                                model.ButtonText = "Save";
                                model.DeleteEnable = false;
                            }
                            else
                            {
                                model.ButtonText = "Update";
                                model.DeleteEnable = true;
                            }
                            parentModel.PersonalInfo = model;

                            PopulateDropdownList(model);
                            parentModel.PersonalInfo.SideBarClassName = "selected";
                            return View("CreateOrEdit", parentModel);
                        }

                        if (model.strMode == "add")
                        {
                            entity.IUser = User.Identity.Name;
                            entity.IDate = Common.CurrentDateTime;
                            _amsCommonService.AMSUnit.AnsarPersonalInfoRepository.Add(entity);
                            model.Message = Resources.ErrorMessages.InsertSuccessful;
                            model.errClass = "success";
                            model.ButtonText = "Update";
                            model.strMode = "edit";
                            model.DeleteEnable = true;

                        }
                        else
                        {
                            var personalInfoId = _amsCommonService.AMSUnit.AnsarPersonalInfoRepository.Fetch().Where(x => x.EmployeeId == model.EmployeeId).FirstOrDefault().Id;
                            entity.Id = personalInfoId;

                            _amsCommonService.AMSUnit.AnsarPersonalInfoRepository.Update(entity);

                            model.Message = Resources.ErrorMessages.UpdateSuccessful;
                            model.errClass = "success";
                            model.DeleteEnable = true;
                            model.strMode = "edit";
                            model.ButtonText = "Update";
                        }
                        _amsCommonService.AMSUnit.AnsarPersonalInfoRepository.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("", Common.ValidationSummaryHead);
                        model.errClass = "failed";
                    }

                    parentModel.PersonalInfo = model;
                    PopulateDropdownList(model);
                    parentModel.PersonalInfo.SideBarClassName = "selected";
                    return View("CreateOrEdit", parentModel);
                }
            }
            catch
            {
                PopulateDropdownList(model);
                if (model.strMode == "add")
                {
                    model.Message = Resources.ErrorMessages.InsertFailed;
                    model.ButtonText = "Save";
                    model.errClass = "failed";
                    model.DeleteEnable = false;
                }
                else if (model.strMode == "edit")
                {
                    model.Message = Resources.ErrorMessages.UpdateFailed;
                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.errClass = "failed";
                }
            }
            parentModel.PersonalInfo.SideBarClassName = "selected";
            return View("CreateOrEdit", parentModel);
        }

        public ActionResult DeletePersonaInfo(int id)
        {
            var parentModel = new AnsarViewModel();
            parentModel.ViewType = "PersonalInfo";

            var entity = _amsCommonService.AMSUnit.AnsarPersonalInfoRepository.Fetch().Where(x => x.EmployeeId == id).FirstOrDefault();
            var model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && model != null)
                {
                    _amsCommonService.AMSUnit.AnsarPersonalInfoRepository.Delete(entity);
                    _amsCommonService.AMSUnit.AnsarPersonalInfoRepository.SaveChanges();

                    PopulateDropdownList(model);
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";
                    model.ButtonText = "Save";
                    return RedirectToAction("PersonaInfoIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
                }
            }
            catch
            {
                PopulateDropdownList(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";
                model.ButtonText = "Update";
            }

            parentModel.PersonalInfo = model;
            parentModel.PersonalInfo.SideBarClassName = "selected";
            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Accademic Qualification Info
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetAllAccademicQlfnInfoList(JqGridRequest request, AnsarSearchViewModel viewModel, int Id)
        {
            var list = _amsCommonService.GetAllAccademicQlfnInfoByAnsarEmpID(viewModel.ID.HasValue ? viewModel.ID.Value : 0).OrderByDescending(x => x.YearOfPassing).ToList();
            var totalRecords = list.Count();

            #region Sorting
            if (request.SortingName == "ID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Id).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Id).ToList();
                }
            }

            if (request.SortingName == "ExamLevel")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ExamLevel).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ExamLevel).ToList();
                }
            }
            if (request.SortingName == "UniversityOrBorard")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.UniversityOrBorard).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.UniversityOrBorard).ToList();
                }
            }

            if (request.SortingName == "YearOfPassing")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.YearOfPassing).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.YearOfPassing).ToList();
                }
            }


            if (request.SortingName == "AccademicGrade")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AccademicGrade).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AccademicGrade).ToList();
                }
            }

            if (request.SortingName == "InstituteName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.InstituteName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.InstituteName).ToList();
                }
            }

            //if (request.SortingName == "SubjectGroup")
            //{
            //    if (request.SortingOrder.ToString().ToLower() == "asc")
            //    {
            //        list = list.OrderBy(x => x.SubjectGroup).ToList();
            //    }
            //    else
            //    {
            //        list = list.OrderByDescending(x => x.SubjectGroup).ToList();
            //    }
            //}
            #endregion

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.Id,
                    item.ExamLevel,
                    item.InstituteName,                  
                    item.YearOfPassing,
                    item.AccademicGrade,
                    item.UniversityOrBorard
                    //item.SubjectGroup
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult AccademicQlfnInfoIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new AnsarViewModel();
            var model = parentModel.AccademicQlfnInfo;

            if (id.HasValue)
            {
                var entity = _amsCommonService.AMSUnit.AnsarDegreeRepository.GetByID(id.Value);

                if (IsMenu)
                {
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(id);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(id);
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        model = entity.ToModel();
                        model.DeleteEnable = true;
                        model.ButtonText = "Update";
                        model.strMode = "edit";
                    }
                }

                var empEntity = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : id);
                parentModel.EmployeeId = empEntity.Id;
                parentModel.Id = empEntity.Id;
                parentModel.ViewType = "AccademicQlfnInfo";
                if (empEntity != null)
                {
                    model.InactiveDate = empEntity.InactiveDate;
                    Common.PopulateAnsarTop(model.EmpTop, empEntity.Id, _amsCommonService);
                }
            }
            parentModel.AccademicQlfnInfo = model;
            PopulateDropdownListACC(model);
            parentModel.AccademicQlfnInfo.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.AccademicQlfnInfo.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.errClass = "success";

            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditAccademicQlfnInfo(AccademicQlfnInfoViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new AnsarViewModel();
            parentModel.EmployeeId = model.EmployeeId;
            parentModel.Id = model.EmployeeId;
            try
            {
                Common.PopulateAnsarTop(model.EmpTop, model.EmployeeId, _amsCommonService);
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();

                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    businessError = CheckBusinessLogicACC(entity);

                    if (businessError != string.Empty)
                    {
                        model.Message = businessError;
                        model.errClass = "failed";
                        parentModel.ViewType = "AccademicQlfnInfo";
                        parentModel.AccademicQlfnInfo = model;
                        PopulateDropdownListACC(model);
                        if (model.strMode == "add")
                        {
                            model.DeleteEnable = false;
                            model.ButtonText = "Save";
                        }
                        else
                        {
                            model.DeleteEnable = true;
                            model.ButtonText = "Update";
                        }
                        return View("CreateOrEdit", parentModel);
                    }
                    if (model.strMode == "add")
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = Common.CurrentDateTime;
                        _amsCommonService.AMSUnit.AnsarDegreeRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        //var ansarDegreeId = _amsCommonService.AMSUnit.AnsarDegreeRepository.Fetch().Where(x => x.EmployeeId == model.EmployeeId).FirstOrDefault().Id;
                        //entity.Id = ansarDegreeId;

                        _amsCommonService.AMSUnit.AnsarDegreeRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _amsCommonService.AMSUnit.AnsarDegreeRepository.SaveChanges();
                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();
                }
                else
                {
                    model.Message = Common.ValidationSummaryHead;
                    model.errClass = "failed";

                    ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
                }

                PopulateDropdownListACC(model);

                parentModel.ViewType = "AccademicQlfnInfo";
                parentModel.AccademicQlfnInfo = model;
                parentModel.AccademicQlfnInfo.SideBarClassName = "selected";

                return View("CreateOrEdit", parentModel);
            }
            catch
            {
                PopulateDropdownListACC(model);

                if (model.strMode == "add")
                {
                    model.Message = Resources.ErrorMessages.InsertFailed;
                    model.errClass = "failed";
                    model.ButtonText = "Save";
                }
                else if (model.strMode == "edit")
                {
                    model.Message = Resources.ErrorMessages.UpdateFailed;
                    model.errClass = "failed";
                    model.ButtonText = "Update";
                }

                parentModel.ViewType = "AccademicQlfnInfo";
                parentModel.AccademicQlfnInfo = model;
                parentModel.AccademicQlfnInfo.SideBarClassName = "selected";

                return View("CreateOrEdit", parentModel);
            }
        }

        public ActionResult DeleteAccademicQlfnInfo(int id)
        {
            var parentModel = new AnsarViewModel();
            parentModel.ViewType = "AccademicQlfnInfo";

            var entity = _amsCommonService.AMSUnit.AnsarDegreeRepository.GetByID(id);
            dynamic model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _amsCommonService.AMSUnit.AnsarDegreeRepository.Delete(entity);

                    _amsCommonService.AMSUnit.AnsarDegreeRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";
                    return RedirectToAction("AccademicQlfnInfoIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
                }
                else
                {
                    PopulateDropdownList(model);
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";
                    parentModel.AccademicQlfnInfo = model;
                }
            }
            catch
            {
                PopulateDropdownList(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.AccademicQlfnInfo = model;
            }

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Job Experience Information

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetJobExperienceList(JqGridRequest request, AnsarSearchViewModel viewModel, int Id)
        {
            var list = _amsCommonService.GetAllJobExperienceInfoByAnsarEmpID(viewModel.ID.HasValue ? viewModel.ID.Value : 0).ToList().OrderBy(x => x.EndDate).ToList();

            var totalRecords = list.Count();

            #region Sorting

            if (request.SortingName == "Organization1")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Organization1).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Organization1).ToList();
                }
            }

            if (request.SortingName == "OrganizationType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.OrganizationType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.OrganizationType).ToList();
                }
            }

            if (request.SortingName == "FromDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FromDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FromDate).ToList();
                }
            }

            if (request.SortingName == "EndDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EndDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EndDate).ToList();
                }
            }
            if (request.SortingName == "Duration")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Duration).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Duration).ToList();
                }
            }

            #endregion

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };


            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.Id,
                    item.Organization1,
                    item.OrganizationType,
                    Convert.ToDateTime(item.FromDate).ToString("dd-MMM-yyyy"),
                    Convert.ToDateTime(item.EndDate).ToString("dd-MMM-yyyy"),
                    item.Duration
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult JobExperienceInfoIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new AnsarViewModel();
            var model = parentModel.JobExperienceInfo;

            if (id.HasValue)
            {
                var entity = _amsCommonService.AMSUnit.AnsarExperienceRepository.GetByID(id.Value);

                if (IsMenu)
                {
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(id);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(id);
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        model = entity.ToModel();
                        var tempDuration = (model.EndDate - model.FromDate).TotalDays / 365;
                        model.Duration = Convert.ToDecimal(Math.Round(tempDuration, 2));
                        model.StrDuration = _amsCommonService.AMSUnit.FunctionRepository.fnGetServiceDuration(model.FromDate, model.EndDate);
                        model.DeleteEnable = true;
                        model.ButtonText = "Update";
                        model.strMode = "edit";
                    }
                }

                var empEntity = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : id);
                parentModel.EmployeeId = empEntity.Id;
                parentModel.Id = empEntity.Id;

                parentModel.ViewType = "JobExperienceInfo";

                if (empEntity != null)
                {
                    model.InactiveDate = empEntity.InactiveDate;
                    Common.PopulateAnsarTop(model.EmpTop, empEntity.Id, _amsCommonService);
                }
            }

            parentModel.JobExperienceInfo = model;
            PopulateDropdownListJOBEXP(model);
            parentModel.JobExperienceInfo.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.JobExperienceInfo.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.errClass = "success";

            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditJobExperienceInfo(JobExperienceInfoViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new AnsarViewModel();

            parentModel.EmployeeId = model.EmployeeId;
            parentModel.Id = model.EmployeeId;
            try
            {
                Common.PopulateAnsarTop(model.EmpTop, model.EmployeeId, _amsCommonService);

                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();

                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    businessError = _amsCommonService.CheckBusinessLogicJOBEXP(entity, model.strMode);

                    if (businessError != string.Empty)
                    {
                        model.Message = businessError;
                        model.errClass = "failed";
                        parentModel.ViewType = "JobExperienceInfo";
                        parentModel.JobExperienceInfo = model;
                        PopulateDropdownListJOBEXP(model);
                        if (model.strMode == "add")
                        {
                            model.DeleteEnable = false;
                            model.ButtonText = "Save";
                        }
                        else
                        {
                            model.DeleteEnable = true;
                            model.ButtonText = "Update";
                        }
                        return View("CreateOrEdit", parentModel);
                    }
                    if (model.strMode == "add")
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = Common.CurrentDateTime;
                        _amsCommonService.AMSUnit.AnsarExperienceRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        //var expId = _amsCommonService.AMSUnit.AnsarExperienceRepository.Fetch().Where(x => x.EmployeeId == model.EmployeeId).FirstOrDefault().Id;
                        //entity.Id = expId;

                        _amsCommonService.AMSUnit.AnsarExperienceRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _amsCommonService.AMSUnit.AnsarExperienceRepository.SaveChanges();
                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();
                }
                else
                {
                    model.Message = Common.ValidationSummaryHead;
                    model.errClass = "failed";

                    ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
                }

                PopulateDropdownListJOBEXP(model);

                parentModel.ViewType = "JobExperienceInfo";
                parentModel.JobExperienceInfo = model;
                parentModel.JobExperienceInfo.SideBarClassName = "selected";

                return View("CreateOrEdit", parentModel);
            }
            catch (Exception ex)
            {
                PopulateDropdownListJOBEXP(model);

                if (model.strMode == "add")
                {
                    model.Message = Resources.ErrorMessages.InsertFailed;
                    model.errClass = "failed";
                }
                else if (model.strMode == "edit")
                {
                    model.Message = Resources.ErrorMessages.UpdateFailed;
                    model.errClass = "failed";
                    model.ButtonText = "Update";
                }

                parentModel.ViewType = "JobExperienceInfo";
                parentModel.JobExperienceInfo = model;
                parentModel.JobExperienceInfo.SideBarClassName = "selected";

                return View("CreateOrEdit", parentModel);
            }
        }
        public ActionResult DeleteJobExperienceInfo(int id)
        {
            var parentModel = new AnsarViewModel();
            parentModel.ViewType = "JobExperienceInfo";

            var entity = _amsCommonService.AMSUnit.AnsarExperienceRepository.GetByID(id);

            dynamic model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _amsCommonService.AMSUnit.AnsarExperienceRepository.Delete(entity);

                    _amsCommonService.AMSUnit.AnsarExperienceRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";


                    return RedirectToAction("JobExperienceInfoIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
                }
                else
                {
                    PopulateDropdownListJOBEXP(model);
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";

                    parentModel.JobExperienceInfo = model;
                }

            }
            catch
            {
                PopulateDropdownListJOBEXP(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.JobExperienceInfo = model;
            }

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Service History


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetServiceHistoryList(JqGridRequest request, AnsarSearchViewModel viewModel, int Id)
        {
            var list = _amsCommonService.AMSUnit.AnsarServiceHistoryRepository.GetAll().Where(x => x.EmployeeId == viewModel.ID).OrderBy(x => x.PeriodFrom).ToList();

            var totalRecords = list.Count();

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.Id,
                    item.PRM_ZoneInfo.ZoneName,
                    Convert.ToDateTime(item.PeriodFrom).ToString(DateAndTime.GlobalDateFormat),
                    item.PeriodTo != null ? Convert.ToDateTime(item.PeriodTo).ToString(DateAndTime.GlobalDateFormat) : "Present",
                    item.Duration,
                    item.RemarkableWork,
                    item.DisciplinaryRecord
                  
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult ServiceHistoryIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new AnsarViewModel();
            var model = parentModel.EmpServiceHistoryViewModel;

            if (id.HasValue)
            {
                var entity = _amsCommonService.AMSUnit.AnsarServiceHistoryRepository.GetByID(id.Value);
                if (IsMenu)
                {
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(id);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(id);
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {

                        model = entity.ToModel();
                        model.StrDuration = _amsCommonService.AMSUnit.FunctionRepository.fnGetServiceDuration(model.PeriodFrom, model.PeriodTo ?? DateTime.Now);
                        model.DeleteEnable = true;
                        model.ButtonText = "Update";
                        model.strMode = "edit";
                    }
                }

                var empEntity = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : id);
                parentModel.EmployeeId = empEntity.Id;
                parentModel.Id = empEntity.Id;

                parentModel.ViewType = "EmpServiceHisotory";

                if (empEntity != null)
                {
                    model.InactiveDate = empEntity.InactiveDate;
                    Common.PopulateAnsarTop(model.EmpTop, empEntity.Id, _amsCommonService);
                }

            }

            parentModel.EmpServiceHistoryViewModel = model;
            PopulateDropdownListServiceHistory(model);
            parentModel.EmpServiceHistoryViewModel.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.EmpServiceHistoryViewModel.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.errClass = "success";

            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditServiceHistory(EmpServiceHistoryViewModel model)
        {

            string businessError = string.Empty;
            var parentModel = new AnsarViewModel();

            parentModel.EmployeeId = model.EmployeeId;
            parentModel.Id = model.EmployeeId;
            try
            {
                Common.PopulateAnsarTop(model.EmpTop, model.EmployeeId, _amsCommonService);

                if (ModelState.IsValid)
                {
                    model.IUser = User.Identity.Name;
                    model.IDate = Common.CurrentDateTime;
                    var entity = model.ToEntity();

                    businessError = CheckAnsarServiceHistoryBusinessRule(model);

                    if (businessError != string.Empty)
                    {
                        model.Message = businessError;
                        model.errClass = "failed";
                        parentModel.ViewType = "EmpServiceHisotory";
                        parentModel.EmpServiceHistoryViewModel = model;
                        PopulateDropdownListServiceHistory(model);
                        if (model.strMode == "add")
                        {
                            model.DeleteEnable = false;
                            model.ButtonText = "Save";
                        }
                        else
                        {
                            model.DeleteEnable = true;
                            model.ButtonText = "Update";
                        }
                        return View("CreateOrEdit", parentModel);
                    }

                    if (model.strMode == "add")
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = Common.CurrentDateTime;

                        _amsCommonService.AMSUnit.AnsarServiceHistoryRepository.Add(entity);

                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        entity.EUser = User.Identity.Name;
                        entity.EDate = Common.CurrentDateTime;

                        //var serviceHistoryId = _amsCommonService.AMSUnit.AnsarServiceHistoryRepository.Fetch().Where(x => x.EmployeeId == model.EmployeeId).FirstOrDefault().Id;
                        //entity.Id = serviceHistoryId;

                        _amsCommonService.AMSUnit.AnsarServiceHistoryRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _amsCommonService.AMSUnit.AnsarServiceHistoryRepository.SaveChanges();

                    if (entity.PeriodTo == null)
                    {
                        var ansarEmpEntity = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetByID(entity.EmployeeId);
                        ansarEmpEntity.ZoneInfoId = entity.ZoneId;
                        _amsCommonService.AMSUnit.AnsarEmpInfoRepository.Update(ansarEmpEntity);
                        _amsCommonService.AMSUnit.AnsarEmpInfoRepository.SaveChanges();
                    }

                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();
                }
                else
                {
                    model.Message = Common.ValidationSummaryHead;
                    model.errClass = "failed";

                    ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
                }

                PopulateDropdownListServiceHistory(model);

                parentModel.ViewType = "EmpServiceHisotory";
                parentModel.EmpServiceHistoryViewModel = model;
                parentModel.EmpServiceHistoryViewModel.SideBarClassName = "selected";

                return View("CreateOrEdit", parentModel);
            }
            catch (Exception ex)
            {
                PopulateDropdownListServiceHistory(model);

                if (model.strMode == "add")
                {
                    model.Message = Resources.ErrorMessages.InsertFailed;
                    model.errClass = "failed";
                }
                else if (model.strMode == "edit")
                {
                    model.Message = Resources.ErrorMessages.UpdateFailed;
                    model.errClass = "failed";
                    model.ButtonText = "Update";
                }
                parentModel.ViewType = "EmpServiceHisotory";
                parentModel.EmpServiceHistoryViewModel = model;
                parentModel.EmpServiceHistoryViewModel.SideBarClassName = "selected";

                return View("CreateOrEdit", parentModel);
            }
        }

        public ActionResult DeleteServiceHistory(int id)
        {
            var parentModel = new AnsarViewModel();
            parentModel.ViewType = "EmpServiceHisotory";

            var entity = _amsCommonService.AMSUnit.AnsarServiceHistoryRepository.GetByID(id);

            var model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _amsCommonService.AMSUnit.AnsarServiceHistoryRepository.Delete(entity);
                    _amsCommonService.AMSUnit.AnsarServiceHistoryRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";


                    return RedirectToAction("ServiceHistoryIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
                }
                else
                {
                    PopulateDropdownListServiceHistory(model);
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";

                    parentModel.EmpServiceHistoryViewModel = model;
                }

            }
            catch
            {
                PopulateDropdownListServiceHistory(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.EmpServiceHistoryViewModel = model;
            }

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Others action

        [NoCache]
        public JsonResult GetDateDiff(DateTime startDate, DateTime endDate)
        {
            var duration = string.Empty;

            duration = _amsCommonService.AMSUnit.FunctionRepository.fnGetServiceDuration(startDate, endDate);

            return Json(new { Duration = duration }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDuration(int? Id, string DOJ, string DOI)
        {
            DateTime? toDate = DateTime.Now;
            string duration = string.Empty;

            if (DOJ != string.Empty)
            {
                try
                {
                    if (DOI != string.Empty)
                    {
                        toDate = Convert.ToDateTime(DOI);
                    }

                    if (toDate > Convert.ToDateTime(DOJ))
                    {
                        var amsContext = new ERP_BEPZA_AMSEntities();
                        duration = amsContext.sp_AMS_GetServiceDuration(Convert.ToDateTime(DOJ), toDate).FirstOrDefault().Duration;
                    }
                    
                    return Json(new { duration = duration }, JsonRequestBehavior.AllowGet);
                    
                }
                catch (Exception ex)
                { 
                }
            }
            return Json(new { duration = duration}, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Dropdown

        [NoCache]
        public ActionResult GetDesignation()
        {
            var designations = _amsCommonService.AMSUnit.AnsarDesignationRepository.GetAll().OrderBy(x => x.SortOrder).ThenBy(x => x.DesignationName).ToList().Select(y =>
                                                    new SelectListItem()
                                                    {
                                                        Text = y.DesignationName,
                                                        Value = y.Id.ToString()
                                                    }).ToList();

            return PartialView("Select", designations);
        }

        [NoCache]
        public ActionResult GetStatus()
        {
            var empStatus = _amsCommonService.AMSUnit.EmpStatusRepository.GetAll().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();

            return PartialView("Select", Common.PopulateDllList(empStatus));
        }

        public ActionResult LoadDistrict(int countryId)
        {
            var districtList = from d in _prmCommonService.PRMUnit.DistrictRepository.Fetch()
                                      join
                                          cd in _prmCommonService.PRMUnit.CountryDivisionRepository.Fetch() on d.DivisionId equals cd.Id
                                      where cd.CountryId == countryId
                                      select d;

            var list = districtList.Select(x => new { Id = x.Id, DistrictName = x.DistrictName }).OrderBy(x => x.DistrictName).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult LoadThana(int districtId)
        {
            var list = _prmCommonService.PRMUnit.ThanaRepository.Fetch().Where(t => t.DistrictId == districtId).Select(x => new { Id = x.Id, Name = x.ThanaName }).OrderBy(x => x.Name).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
        }

        private void populateDropdown(EmploymentInfoViewModel model)
        {
            #region District
            model.DistrictList = Common.PopulateDistrictDDL(_prmCommonService.PRMUnit.DistrictRepository.GetAll().OrderBy(x => x.DistrictName));
            #endregion
            
            #region Religion
            model.ReligionList = Common.PopulateReligionDDL(_prmCommonService.PRMUnit.Religion.GetAll().OrderBy(x => x.SortOrder).ThenBy(x => x.Name));
            #endregion

            #region Category
            model.CategoryList = Common.PopulateDllList(_amsCommonService.AMSUnit.AnsarCategoryRepository.GetAll().OrderBy(x => x.SortOrder).ThenBy(x => x.Name));
            #endregion

            #region Gender
            model.GenderList = Common.PopulateGenderDDL(model.GenderList);
            #endregion

            #region Title
            var titleList = _prmCommonService.PRMUnit.NameTitleRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.TitleList = Common.PopulateDllList(titleList);
            #endregion

            #region Designation
            var desigList = _amsCommonService.AMSUnit.AnsarDesignationRepository.GetAll().OrderBy(x => x.SortOrder).ThenBy(x => x.DesignationName).ToList();
            model.DesignationList = desigList.Select(y =>
                                                    new SelectListItem()
                                                    {
                                                        Text = y.DesignationName,
                                                        Value = y.Id.ToString()
                                                    }).ToList();
            #endregion

            #region Status
            var statusList = _amsCommonService.AMSUnit.EmpStatusRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.StatusList = Common.PopulateDllList(statusList);
            #endregion
        }

        private void PopulateDropdownList(PersonalInfoViewModel model)
        {
            dynamic ddlList;

            #region Gender ddl

            // model.GenderList = Common.PopulateGenderDDL(model.GenderList);

            #endregion

            #region Marital Status ddl

            ddlList = _prmCommonService.PRMUnit.MaritalStatusRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.MaritalStatusList = Common.PopulateDllList(ddlList);

            #endregion

            #region Blood group ddl

            ddlList = _prmCommonService.PRMUnit.BloodGroupRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.BloodGroupList = Common.PopulateDllList(ddlList);

            #endregion

            #region Country of Birth ddl

            ddlList = _prmCommonService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.CountryofBirthList = Common.PopulateDllList(ddlList);

            #endregion

            #region Nationality ddl

            ddlList = _prmCommonService.PRMUnit.NationalityRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.NationalityList = Common.PopulateDllList(ddlList);

            #endregion

            #region Present Country ddl

            ddlList = _prmCommonService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.PresentCountryList = Common.PopulateDllList(ddlList);

            #endregion

            #region Present District ddl

            var districtListPresent = from d in _prmCommonService.PRMUnit.DistrictRepository.Fetch()
                               join
                                   cd in _prmCommonService.PRMUnit.CountryDivisionRepository.Fetch() on d.DivisionId equals cd.Id
                               where cd.CountryId == model.PresentCountryId
                               select d;

            ddlList = districtListPresent.OrderBy(x => x.DistrictName);
            model.PresentDistictList = Common.PopulateDistrictDDL(ddlList);

            #endregion

            #region Present Thana ddl

            ddlList = _prmCommonService.PRMUnit.ThanaRepository.Fetch().Where(t => t.DistrictId == model.PresentDistrictId).OrderBy(x => x.ThanaName).ToList();
            model.PresentThanaList = Common.PopulateThanDDL(ddlList);

            #endregion

            #region Permanent Country ddl

            ddlList = _prmCommonService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.PermanentCountryList = Common.PopulateDllList(ddlList);

            #endregion

            #region Permanent District ddl

            var districtListPermanent = from d in _prmCommonService.PRMUnit.DistrictRepository.Fetch()
                               join
                                   cd in _prmCommonService.PRMUnit.CountryDivisionRepository.Fetch() on d.DivisionId equals cd.Id
                               where cd.CountryId == model.PermanentCountryId
                               select d;
            ddlList = districtListPermanent.OrderBy(x => x.DistrictName);
            model.PermanentDistictList = Common.PopulateDistrictDDL(ddlList);

            #endregion

            #region Permanent Thana ddl

            ddlList = _prmCommonService.PRMUnit.ThanaRepository.Fetch().Where(t => t.DistrictId == model.PermanentDistrictId).OrderBy(x => x.ThanaName).ToList();
            model.PermanentThanaList = Common.PopulateThanDDL(ddlList);

            #endregion

            #region Profession
            ddlList = _prmCommonService.PRMUnit.ProfessionRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.FatherProfessionList = Common.PopulateDllList(ddlList);
            model.MotherProfessionList = Common.PopulateDllList(ddlList);
            #endregion
        }

        private void PopulateDropdownListACC(AccademicQlfnInfoViewModel model)
        {
            dynamic ddlList;

            #region Exam/Level ddl

            ddlList = _prmCommonService.PRMUnit.ExamDegreeLavelRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.ExamLavelList = Common.PopulateDllList(ddlList);

            #endregion

            #region Passing Year List

            model.YearOfPassingList = Common.PopulateYearList();

            #endregion

            #region Result ddl

            ddlList = _prmCommonService.PRMUnit.AcademicGradeRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.ResultList = Common.PopulateDllList(ddlList);

            #endregion

            #region Institute ddl

            ddlList = _prmCommonService.PRMUnit.UniversityAndBoardRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.UniversityAndBoardList = Common.PopulateDllList(ddlList);

            #endregion

            #region Subject/Group ddl

            //ddlList = _prmCommonService.PRMUnit.SubjectGroupRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            //model.SubjectGroupList = Common.PopulateDllList(ddlList);

            #endregion

            #region Country ddl

            ddlList = _prmCommonService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.CountryList = Common.PopulateDllList(ddlList);

            #endregion
        }

        private void PopulateDropdownListJOBEXP(JobExperienceInfoViewModel model)
        {
            dynamic ddlList;

            #region Organization Type ddl

            ddlList = _amsCommonService.AMSUnit.OrganizationTypeRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.OrganizationTypeList = Common.PopulateDllList(ddlList);

            #endregion
        }

        private void PopulateDropdownListServiceHistory(EmpServiceHistoryViewModel model)
        {
            #region Zone ddl

            var list = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().OrderBy(x => x.ZoneName).ToList();
            model.ZoneList = list.Select(y =>
                                                    new SelectListItem()
                                                    {
                                                        Text = y.ZoneName,
                                                        Value = y.Id.ToString()
                                                    }).ToList(); ;

            #endregion

        }

        #endregion

        #endregion

        #region Business Logic

        private string CheckEmpInfoBusinessRule(EmploymentInfoViewModel model)
        {
            string message = string.Empty;
            dynamic ansarIdInfo = null;
            //dynamic bepzaIdInfo = null;

            var blacklist = _amsCommonService.AMSUnit.BlacklistRepository.Get(x => x.AMS_AnsarEmpInfo.AnsarId == model.AnsarId && x.IsRevoked == false).FirstOrDefault();

            if (model.Id > 0)
            {
                ansarIdInfo = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.Get(x => x.AnsarId.ToLower().Trim() == model.AnsarId.ToLower().Trim() && x.AMS_EmpStatus.Name == "Active" && x.Id != model.Id).FirstOrDefault();
                //bepzaIdInfo = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.Get(x => x.BEPZAId.ToLower().Trim() == model.BEPZAID.ToLower().Trim() && x.Id != model.Id).FirstOrDefault();
            }
            else
            {
                ansarIdInfo = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.Get(x => x.AnsarId.ToLower().Trim() == model.AnsarId.ToLower().Trim() && x.AMS_EmpStatus.Name == "Active").FirstOrDefault();
                //bepzaIdInfo = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.Get(x => x.BEPZAId.ToLower().Trim() == model.BEPZAID.ToLower().Trim()).FirstOrDefault();
            }

            if (ansarIdInfo != null)
            {
                message += "Ansar ID Already exists.";
            }
            //if (bepzaIdInfo != null)
            //{
            //    message += "BEPZA ID Already exists.";
            //}
            if (blacklist != null)
            {
                message += "Blacklisted Ansar ID.";
            }

            return message;
        }
        public string CheckBusinessLogicACC(AMS_AnsarDegree obj)
        {
            string businessError = string.Empty;

            if (CheckGPASelected(obj))
            {
                if (obj.CGPA == 0)
                {
                    businessError = "The field CGPA must be greater than 0.";

                    return businessError;
                }
                if (obj.Scale == 0)
                {
                    businessError = "The field Scale must be greater than 0.";

                    return businessError;
                }
                if (obj.CGPA > obj.Scale)
                {
                    businessError = "The field Scale must be greater than the field CGPA.";

                    return businessError;
                }
            }

            return string.Empty;
        }

        public bool CheckGPASelected(AMS_AnsarDegree obj)
        {
            var resultList = _prmCommonService.PRMUnit.AcademicGradeRepository.Fetch();
            if (resultList != null)
            {
                foreach (var item in resultList)
                {
                    if (obj.AcademicGradeId == item.Id)
                    {
                        if (item.Name.ToUpper() == "GPA")
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        private string CheckAnsarServiceHistoryBusinessRule(EmpServiceHistoryViewModel model)
        {
            string message = string.Empty;

            if (model.PeriodTo != null && (model.PeriodFrom > model.PeriodTo || model.PeriodTo > DateTime.Now))
            {
                message += "The field Period To must be greater than Period From and less than or equal to current date.";

                return message;
            }

            var dateOfJoining = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetAll().Where(x => x.Id == model.EmployeeId).FirstOrDefault().DateofJoining;

            if (model.PeriodFrom < dateOfJoining)
            {
                message += "The field Period From must be equal or greater than the Date of Joining (" + dateOfJoining.ToString("dd-MMM-yyyy") + ").";

                return message;
            }

            if (ServiceHistoryDateRangeCheck(model.PeriodFrom, model.PeriodTo, model.Id, model.EmployeeId, model.strMode))
            {
                message += "Service period is not valid.";
                return message;
            }

            return message;
        }

        public bool ServiceHistoryDateRangeCheck(DateTime sDate, DateTime? eDate, int serviceHistoryID, int ansarEmpID, string strMode)
        {
            bool rv = false;

            if (sDate == null)
            {
                eDate = DateTime.Now;
            }

            if (strMode == "add")
            {
                rv = _amsCommonService.AMSUnit.AnsarServiceHistoryRepository.Fetch().Where(
                          x => (x.EmployeeId == ansarEmpID) &&
                              (
                                  (x.PeriodFrom <= sDate && sDate <= (x.PeriodTo ?? DateTime.Now)) ||
                                  (x.PeriodFrom <= eDate && eDate <= (x.PeriodTo ?? DateTime.Now)) ||
                                  (sDate < x.PeriodFrom && (x.PeriodTo ?? DateTime.Now) < eDate))
                              ).Any();
            }
            else
            {
                rv = _amsCommonService.AMSUnit.AnsarServiceHistoryRepository.Fetch().Where(
                          x => (x.EmployeeId == ansarEmpID && serviceHistoryID != x.Id) &&
                              (
                                  (x.PeriodFrom <= sDate && sDate <= (x.PeriodTo ?? DateTime.Now))
                                  || (x.PeriodFrom <= eDate && eDate <= (x.PeriodTo ?? DateTime.Now))
                                  || (sDate < x.PeriodFrom && (x.PeriodTo ?? DateTime.Now) < eDate))
                              ).Any();
            }

            return rv;
        }

        #endregion
        
    }
}
