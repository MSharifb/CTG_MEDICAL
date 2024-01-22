using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.PersonalInfo;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Utility;
using System.IO;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class TempPersonalInfoController : Controller
    {
        #region Fields

        private readonly EmployeeService _empService;
        private readonly PersonalInfoService _personalInfoService;

        #endregion

        #region Constructor

        public TempPersonalInfoController(EmployeeService empService, PersonalInfoService personalInfoService)
        {
            this._empService = empService;
            this._personalInfoService = personalInfoService;
        }

        #endregion


        //ReferenceGuarantorInfo
        #region Actions

        public ActionResult Index()
        {
            return View();
        }

        #region Reference Information

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetReferenceGuarantorList(JqGridRequest request, int empId, string controlType)
        {
            var list = _personalInfoService.PRMUnit.EmpReferenceGuarantorRepository.Fetch().Where(x => x.Type == controlType && x.EmployeeId == empId).ToList();

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

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.FullName,
                    item.Id,
                    item.Designation,
                    item.Relation,

                    item.MobileNo,
                    item.Email
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult CreateReferenceInfo(int? id, string insertResult, string updateResult, string deleteResult, string controlType)
        {
            var model = new ReferenceInfoViewModel();

            Common.PopulateEmployeeTop(model.EmpTop, id.Value, _empService);


            populateDropdown(model);
            model.EmployeeId = model.EmpTop.EmployeeId;
            model.ActionType = "CreateReferenceInfo";
            model.ButtonText = "Save";
            model.SelectedClass = "selected";
            model.Type = controlType;//BEPZA_MEDICAL.Web.Utility.Common.ReferenceGuarantorEnum.ReferenceInfo.ToString();
            //PrepareModel(model);

            if (insertResult == "success")
            {
                model.Message = Resources.ErrorMessages.InsertSuccessful;
                model.ErrorClass = "success";
            }

            if (updateResult == "success")
            {
                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
            }

            if (deleteResult == "success")
            {
                model.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.ErrorClass = "success";
            }

            return View("_ReferenceGuarantorInfo", model);
            //return View();
        }

        [HttpPost]
        public ActionResult CreateReferenceInfo([Bind(Exclude = "Photo")]ReferenceInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                entity.IUser = User.Identity.Name;
                entity.IDate = Common.CurrentDateTime;
                entity.Designation = model.DesignationRG;

                var attachment = Request.Files["Photo"];
                if (attachment != null && !string.IsNullOrEmpty(attachment.FileName))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        attachment.InputStream.CopyTo(ms);
                        entity.Photo = ms.GetBuffer();
                    }
                }

                //entity.EmpoyeeId = model.EmpoyeeId;
                _empService.PRMUnit.EmpReferenceGuarantorRepository.Add(entity);
                _empService.PRMUnit.EmpReferenceGuarantorRepository.SaveChanges();
            }
            else
            {
                populateDropdown(model);

                model.Message = Resources.ErrorMessages.InsertFailed;
                model.ErrorClass = "failed";
                model.ActionType = "CreateReferenceInfo";
                model.ButtonText = "Save";
                model.SelectedClass = "selected";

                return View("_ReferenceGuarantorInfo", model);
            }

            return RedirectToAction("CreateReferenceInfo", "TempPersonalInfo", new { id = model.EmployeeId, insertResult = "success", controlType = model.Type });
        }

        public ActionResult EditReferenceInfo(int id)
        {
            var entity = _personalInfoService.PRMUnit.EmpReferenceGuarantorRepository.GetByID(id);
            var model = entity.ToModel();
            model.DesignationRG = entity.Designation;

            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

            populateDropdown(model);


            model.ActionType = "EditReferenceInfo";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            return View("_ReferenceGuarantorInfo", model);
        }

        [HttpPost]
        public ActionResult EditReferenceInfo([Bind(Exclude = "Photo")]ReferenceInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                entity.EUser = User.Identity.Name;
                entity.EDate = Common.CurrentDateTime;
                entity.Designation = model.DesignationRG;

                var attachment = Request.Files["Photo"];
                if (attachment != null && !string.IsNullOrEmpty(attachment.FileName))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        attachment.InputStream.CopyTo(ms);
                        entity.Photo = ms.GetBuffer();
                    }
                }

                //entity.EmpoyeeId = model.EmpoyeeId;
                _empService.PRMUnit.EmpReferenceGuarantorRepository.Update(entity);
                _empService.PRMUnit.EmpReferenceGuarantorRepository.SaveChanges();
            }
            else
            {
                populateDropdown(model);
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

                model.Message = Resources.ErrorMessages.UpdateFailed;
                model.ErrorClass = "failed";

                model.DeleteEnable = true;
                model.ActionType = "EditReferenceInfo";
                model.ButtonText = "Update";
                model.SelectedClass = "selected";

                return View("_ReferenceGuarantorInfo", model);
            }
            //return RedirectToAction("CreateReferenceInfo", "TempPersonalInfo", new { id = model.EmployeeId, updateResult = "success" });

            populateDropdown(model);
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

            model.Message = Resources.ErrorMessages.UpdateSuccessful;
            model.ErrorClass = "success";

            model.DeleteEnable = true;
            model.ActionType = "EditReferenceInfo";
            model.ButtonText = "Update";
            model.SelectedClass = "selected";

            return View("_ReferenceGuarantorInfo", model);
        }

        public ActionResult DeleteReferenceInfo(int id)
        {
            var entity = _personalInfoService.PRMUnit.EmpReferenceGuarantorRepository.GetByID(id);

            if (ModelState.IsValid && entity != null)
            {
                _empService.PRMUnit.EmpReferenceGuarantorRepository.Delete(entity);
                _empService.PRMUnit.EmpReferenceGuarantorRepository.SaveChanges();

                return RedirectToAction("CreateReferenceInfo", null, new { id = entity.EmployeeId, deleteResult = "success", controlType = entity.Type });
            }

            var model = entity.ToModel();
            model.DesignationRG = entity.Designation ?? string.Empty;
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";

            model.ButtonText = "Update";
            model.ActionType = "EditReferenceInfo";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            return View("_ReferenceGuarantorInfo", model);
        }

        #endregion



        #region Visa Information

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetVisaList(JqGridRequest request, int empId, string controlType)
        {
             var list = _personalInfoService.PRMUnit.EmployeeVisaInfoRepository.Fetch().Where(x => x.Type == controlType && x.EmployeeId == empId).ToList();

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

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.Id,
                    item.VisaPassportFor,
                    item.VisaOwner,
                    item.VisaPassportNo,
                    item.IssuePlace,
                    item.PRM_Country.Name,
                    item.IssueDate.ToString("dd-MMM-yyyy"),
                    item.ExpireDate.ToString("dd-MMM-yyyy")
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult CreateVisaInfo(int? id, string insertResult, string updateResult, string deleteResult, string controlType)
        {
            var model = new VisaInfoViewModel();

            Common.PopulateEmployeeTop(model.EmpTop, id.Value, _empService);


            populateDropdown(model);
            model.EmployeeId = model.EmpTop.EmployeeId;
            model.ActionType = "CreateVisaInfo";
            model.ButtonText = "Save";
            model.SelectedClass = "selected";
            model.Type = controlType;
            model.ControlType = controlType;//BEPZA_MEDICAL.Web.Utility.Common.ReferenceGuarantorEnum.ReferenceInfo.ToString();
            //PrepareModel(model);

            if (insertResult == "success")
            {
                model.Message = Resources.ErrorMessages.InsertSuccessful;
                model.ErrorClass = "success";
            }

            if (updateResult == "success")
            {
                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
            }

            if (deleteResult == "success")
            {
                model.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.ErrorClass = "success";
            }

            return View("_EmpVisaInformation", model);
            //return View();
        }

        [HttpPost]
        public ActionResult CreateVisaInfo(VisaInfoViewModel model)
        {

            if (ModelState.IsValid)
            {
                
                if (model.VisaPassportFor == "Own")
                {
                    model.VisaOwner = model.Name;
                }
                else
                {
                    model.VisaOwner = _empService.PRMUnit.PersonalFamilyInformation.Fetch().Where(x => x.Id == model.FamilyMemberId).FirstOrDefault().FullName;
                }
                
                var entity = model.ToEntity();
                entity.IUser = User.Identity.Name;
                entity.IDate = Common.CurrentDateTime;
                //entity.Designation = model.DesignationRG;

                //var attachment = Request.Files["Photo"];
                //if (attachment != null && !string.IsNullOrEmpty(attachment.FileName))
                //{
                //    using (MemoryStream ms = new MemoryStream())
                //    {
                //        attachment.InputStream.CopyTo(ms);
                //        entity.Photo = ms.GetBuffer();
                //    }
                //}

                //entity.EmpoyeeId = model.EmpoyeeId;
                _empService.PRMUnit.EmployeeVisaInfoRepository.Add(entity);
                _empService.PRMUnit.EmployeeVisaInfoRepository.SaveChanges();
            }
            else
            {
                populateDropdown(model);

                model.Message = Resources.ErrorMessages.InsertFailed;
                model.ErrorClass = "failed";
                model.ActionType = "CreateVisaInfo";
                model.ButtonText = "Save";
                model.SelectedClass = "selected";

                return View("_EmpVisaInformation", model);
            }

            return RedirectToAction("CreateVisaInfo", "TempPersonalInfo", new { id = model.EmployeeId, insertResult = "success", controlType = model.Type });
        }

        public ActionResult EditVisaInfo(int id)
        {
            var entity = _personalInfoService.PRMUnit.EmployeeVisaInfoRepository.GetByID(id);
            var model = entity.ToModel();
            //model.DesignationRG = entity.Designation;

            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

            populateDropdown(model);


            model.ActionType = "EditVisaInfo";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";
            return View("_EmpVisaInformation", model);
        }

        [HttpPost]
        public ActionResult EditVisaInfo(VisaInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                //if (model.ControlType == "Visa")
                //{
                //    model.Type = "Visa";
                //}
                //else
                //{
                //    model.Type = "Passport";
                //}

                if (model.VisaPassportFor == "Own")
                {
                    model.VisaOwner = model.Name;
                }
                else
                {
                    model.VisaOwner = _empService.PRMUnit.PersonalFamilyInformation.Fetch().Where(x => x.Id == model.FamilyMemberId).FirstOrDefault().FullName;
                }

                var entity = model.ToEntity();
                entity.EUser = User.Identity.Name;
                entity.EDate = Common.CurrentDateTime;
                //entity.Designation = model.DesignationRG;

                //var attachment = Request.Files["Photo"];
                //if (attachment != null && !string.IsNullOrEmpty(attachment.FileName))
                //{
                //    using (MemoryStream ms = new MemoryStream())
                //    {
                //        attachment.InputStream.CopyTo(ms);
                //        entity.Photo = ms.GetBuffer();
                //    }
                //}

                //entity.EmpoyeeId = model.EmpoyeeId;
                _empService.PRMUnit.EmployeeVisaInfoRepository.Update(entity);
                _empService.PRMUnit.EmployeeVisaInfoRepository.SaveChanges();
            }
            else
            {
                populateDropdown(model);
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

                model.Message = Resources.ErrorMessages.UpdateFailed;
                model.ErrorClass = "failed";

                model.DeleteEnable = true;
                model.ActionType = "EditVisaInfo";
                model.ButtonText = "Update";
                model.SelectedClass = "selected";

                return View("_ReferenceGuarantorInfo", model);
            }
            //return RedirectToAction("CreateReferenceInfo", "TempPersonalInfo", new { id = model.EmployeeId, updateResult = "success" });

            populateDropdown(model);
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

            model.Message = Resources.ErrorMessages.UpdateSuccessful;
            model.ErrorClass = "success";

            model.DeleteEnable = true;
            model.ActionType = "EditReferenceInfo";
            model.ButtonText = "Update";
            model.SelectedClass = "selected";

            return View("_EmpVisaInformation", model);
        }

        public ActionResult DeleteVisaInfo(int id)
        {
            var entity = _personalInfoService.PRMUnit.EmployeeVisaInfoRepository.GetByID(id);

            if (ModelState.IsValid && entity != null)
            {
                _empService.PRMUnit.EmployeeVisaInfoRepository.Delete(entity);
                _empService.PRMUnit.EmployeeVisaInfoRepository.SaveChanges();

                return RedirectToAction("CreateVisaInfo", null, new { id = entity.EmployeeId, deleteResult = "success", controlType = entity.Type });
            }

            var model = entity.ToModel();
            //model.DesignationRG = entity.Designation ?? string.Empty;
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";

            model.ButtonText = "Update";
            model.ActionType = "EditVisaInfo";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            return View("_EmpVisaInformation", model);
        }


        #endregion


        #region Attachment

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEmpAttachmentList(JqGridRequest request, int empId, string controlType)
        {
            var list = _personalInfoService.PRMUnit.EmpAttachementRepository.Fetch().Where(x => x.EmployeeId == empId).ToList();
            var attachmentTypes = _empService.PRMUnit.EmpAttachementTypeRepository.Fetch().ToList();
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

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.FileName,
                    item.Id,
                    attachmentTypes.Find(x=> x.Id == item.AttachmentTypeId).Name,
                    item.Description
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult CreateAttachment(int? id, string insertResult, string updateResult, string deleteResult)
        {
            var model = new EmpAttachmentViewModel();

            Common.PopulateEmployeeTop(model.EmpTop, id.Value, _empService);

            populateDropdown(model);

            model.EmployeeId = model.EmpTop.EmployeeId;
            model.ActionType = "CreateAttachment";
            model.ButtonText = "Save";
            model.SelectedClass = "selected";

            if (insertResult == "success")
            {
                model.Message = Resources.ErrorMessages.InsertSuccessful;
                model.ErrorClass = "success";
            }

            if (updateResult == "success")
            {
                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
            }

            if (deleteResult == "success")
            {
                model.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.ErrorClass = "success";
            }

            return View("_EmpAttachment", model);
        }

        [HttpPost]
        public ActionResult CreateAttachment([Bind(Exclude = "Attachment")]EmpAttachmentViewModel model)
        {
            var attachment = Request.Files["Attachment"];

            if (ModelState.IsValid && attachment != null && !string.IsNullOrEmpty(attachment.FileName) /* && attachment.ContentType != model.a*/)
            {
                var entity = model.ToEntity();
                entity.IUser = User.Identity.Name;
                entity.IDate = Common.CurrentDateTime;


                if (attachment != null && !string.IsNullOrEmpty(attachment.FileName))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        attachment.InputStream.CopyTo(ms);
                        entity.Attachment = ms.GetBuffer();
                    }
                }

                //entity.EmpoyeeId = model.EmpoyeeId;
                _empService.PRMUnit.EmpAttachementRepository.Add(entity);
                _empService.PRMUnit.EmpAttachementRepository.SaveChanges();
            }
            else
            {
                populateDropdown(model);

                model.Message = Resources.ErrorMessages.InsertFailed;
                model.ErrorClass = "failed";
                model.ActionType = "CreateAttachment";
                model.ButtonText = "Save";
                model.SelectedClass = "selected";

                return View("_EmpAttachment", model);
            }

            return RedirectToAction("CreateAttachment", "TempPersonalInfo", new { id = model.EmployeeId, insertResult = "success" });
        }

        public ActionResult EditAttachment(int id)
        {
            var entity = _personalInfoService.PRMUnit.EmpAttachementRepository.GetByID(id);
            var attachmentTypes = _empService.PRMUnit.EmpAttachementTypeRepository.Fetch().ToList();

            var model = entity.ToModel();

            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

            populateDropdown(model);

            model.FileSize = Math.Round(Convert.ToDouble(entity.Attachment.LongLength) / 8) + "B";

            //try
            //{


            //    model.DownloadLink = Server.MapPath("~/Content/TempFiles/") + model.FileName + "." + attachmentTypes.Find(x => x.Id == entity.AttachmentTypeId).Name;

            //    //@"c:\data.dmp"
            //    System.IO.File.WriteAllBytes(model.DownloadLink, entity.Attachment);
            //}
            //catch (Exception)
            //{

            //    model.Message = "Failed to create file !!";
            //    model.ErrorClass = "failed";
            //}

            model.ActionType = "EditAttachment";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            return View("_EmpAttachment", model);
        }

        [HttpPost]
        public ActionResult EditAttachment([Bind(Exclude = "Attachment")]EmpAttachmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                entity.EUser = User.Identity.Name;
                entity.EDate = Common.CurrentDateTime;

                var attachment = Request.Files["Attachment"];
                if (attachment != null && !string.IsNullOrEmpty(attachment.FileName))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        attachment.InputStream.CopyTo(ms);
                        entity.Attachment = ms.GetBuffer();
                    }
                }
                else
                {
                    entity.Attachment = _empService.PRMUnit.EmpAttachementRepository.GetByID(model.Id).Attachment;
                }

                //entity.EmpoyeeId = model.EmpoyeeId;
                _empService.PRMUnit.EmpAttachementRepository.Update(entity);
                _empService.PRMUnit.EmpReferenceGuarantorRepository.SaveChanges();

                model.FileSize = Math.Round(Convert.ToDouble(entity.Attachment.LongLength) / 8) + "B";
            }
            else
            {
                populateDropdown(model);
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

                model.Message = Resources.ErrorMessages.UpdateFailed;
                model.ErrorClass = "failed";

                model.DeleteEnable = true;
                model.ActionType = "EditAttachment";
                model.ButtonText = "Update";
                model.SelectedClass = "selected";

                return View("_EmpAttachment", model);
            }
            //return RedirectToAction("CreateReferenceInfo", "TempPersonalInfo", new { id = model.EmployeeId, updateResult = "success" });

            populateDropdown(model);
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

            model.Message = Resources.ErrorMessages.UpdateSuccessful;
            model.ErrorClass = "success";

            model.DeleteEnable = true;
            model.ActionType = "EditAttachment";
            model.ButtonText = "Update";
            model.SelectedClass = "selected";

            return View("_EmpAttachment", model);
        }

        public ActionResult DeleteAttachment(int id)
        {
            var entity = _personalInfoService.PRMUnit.EmpAttachementRepository.GetByID(id);

            if (ModelState.IsValid && entity != null)
            {
                _empService.PRMUnit.EmpAttachementRepository.Delete(entity);
                _empService.PRMUnit.EmpAttachementRepository.SaveChanges();

                return RedirectToAction("CreateAttachment", null, new { id = entity.EmployeeId, deleteResult = "success" });
            }

            var model = entity.ToModel();
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";

            model.ButtonText = "Update";
            model.ActionType = "EditAttachment";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            return View("_EmpAttachment", model);
        }

        #endregion



        #endregion

        private void populateDropdown(ReferenceInfoViewModel model)
        {
            #region Title
            var titleList = _empService.PRMUnit.NameTitleRepository.Fetch().ToList();
            model.TitleList = Common.PopulateDllList(titleList);
            #endregion
        }

        private void populateDropdown(EmpAttachmentViewModel model)
        {
            #region Title
            var list = _empService.PRMUnit.EmpAttachementTypeRepository.Fetch().ToList();
            model.AttachmentTypeList = Common.PopulateDllList(list);
            #endregion
        }

        private void populateDropdown(VisaInfoViewModel model)
        {
            #region Country ddl

            var list = _empService.PRMUnit.CountryRepository.Fetch().ToList();
            model.CountryList = Common.PopulateDllList(list);

            #endregion

            #region VisaType ddl

            var listvt = _empService.PRMUnit.VisaTypeRepository.Fetch().ToList();
            model.VisaTypeList = Common.PopulateDllList(listvt);

            #endregion

            #region FamilyMember ddl

            var listfm = _empService.PRMUnit.PersonalFamilyInformation.Fetch().ToList();
            model.FamilyMemberList = Common.PopulateFamilyMemberList(listfm);

            #endregion
        }

        #region Utils

        public FileContentResult GetReferenceGuarantorImage(int id)
        {
            var data = _personalInfoService.PRMUnit.EmpReferenceGuarantorRepository.GetByID(id);
            if (data != null && data.Photo.Length != 0)
                return GetImage(data.Photo);
            else
                return null;
        }

        public FileContentResult GetEmpAttachedFile(int id)
        {
            var data = _personalInfoService.PRMUnit.EmpAttachementRepository.GetByID(id);
            var attachmentTypes = _empService.PRMUnit.EmpAttachementTypeRepository.Fetch().ToList();

            if (data != null && data.Attachment.Length != 0)
            {
                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = data.FileName,

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(data.Attachment, attachmentTypes.Find(x => x.Id == data.AttachmentTypeId).Name);
            }
            else
                return null;
        }

        private FileContentResult GetImage(byte[] photo)
        {
            if (photo.Length != 0)
                return File(photo, "image/jpeg");
            else
                return null;
        }

        #endregion
    }
}
