using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.FAM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class JobRequisitionInfoController : BaseController
    {
        #region Fields
        private readonly EmployeeService _empService;
        private readonly PRMCommonSevice _prmCommonSevice;
        private readonly FAMCommonService _famCommonService;
        #endregion

        #region Constructor
        public JobRequisitionInfoController(PRMCommonSevice prmCommonSevice, FAMCommonService famCommonService, EmployeeService empService)
        {
            this._prmCommonSevice = prmCommonSevice;
            this._famCommonService = famCommonService;
            this._empService = empService;
        }
        #endregion
        //
        // GET: /PRM/JobRequisitionInfo/

        #region Index.......
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, JobRequisitionInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<JobRequisitionInfoViewModel> list = (from job in _prmCommonSevice.PRMUnit.JobRequisitionInfoRepository.GetAll()
                                                      join jobDtl in _prmCommonSevice.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on job.Id equals jobDtl.RequisitionInfoId
                                                      join fin in _famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll() on job.FinancialYearId equals fin.Id
                                                      join emp in _prmCommonSevice.PRMUnit.EmploymentInfoRepository.GetAll() on job.PreparedById equals emp.Id
                                                      join des in _prmCommonSevice.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                                                      where (model.Id == 0 || model.Id == job.Id)
                                                      && (model.DesignationId == 0 || model.DesignationId == jobDtl.DesignationId || model.DesignationId == null)
                                                      && (model.FinancialYearId == 0 || model.FinancialYearId == job.FinancialYearId)
                                                      && (model.RequisitionSubDate == null || model.RequisitionSubDate == job.RequisitionSubDate)
                                                      && (string.IsNullOrEmpty(model.RequisitionPreparedBy) || emp.FullName.Contains(model.RequisitionPreparedBy))
                                                      && (string.IsNullOrEmpty(model.Designation) || des.Name.Contains(model.Designation))   
                                                      &&(job.ZoneInfoId==LoggedUserZoneInfoId)
                                                      select new JobRequisitionInfoViewModel()
                                                 {
                                                     Id = job.Id,
                                                     RequisitionNo = job.RequisitionNo,
                                                     PreparedById = job.PreparedById,
                                                     RequisitionPreparedBy = emp.FullName,
                                                     Designation = des.Name,
                                                     FinancialYearId = job.FinancialYearId,
                                                     FinancialYear = fin.FinancialYearName,
                                                     RequisitionSubDate = job.RequisitionSubDate,
                                                     DesignationId = jobDtl.DesignationId
                                                 }).DistinctBy(x => x.RequisitionNo).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "RequisitionNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.RequisitionNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.RequisitionNo).ToList();
                }
            }


            if (request.SortingName == "RequisitionPreparedBy")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.RequisitionPreparedBy).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.RequisitionPreparedBy).ToList();
                }
            }
            if (request.SortingName == "Designation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Designation).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Designation).ToList();
                }
            }
            if (request.SortingName == "FinancialYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FinancialYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FinancialYear).ToList();
                }
            }
            if (request.SortingName == "RequisitionSubDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.RequisitionSubDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.RequisitionSubDate).ToList();
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
                  d.RequisitionNo,
                  //d.PreparedById,
                  d.RequisitionPreparedBy,
                  d.FinancialYearId,
                  d.Designation,
                  d.FinancialYear,
                  d.DesignationId,
                  ((DateTime)d.RequisitionSubDate).ToString(DateAndTime.GlobalDateFormat),
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult RequisitionNoforView()
        {
            var ddlList = _prmCommonSevice.PRMUnit.JobRequisitionInfoRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.RequisitionNo).ToList();

            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.RequisitionNo,
                    Value = item.Id.ToString()
                });
            }

            list.OrderBy(x => x.Text.Trim()).ToList();

            return PartialView("Select", list);
        }
        [NoCache]
        public ActionResult FinancialYearforView()
        {
            var list = Common.PopulateFinancialYearDllList(_famCommonService.FAMUnit.FinancialYearInformationRepository.GetAll().OrderBy(x => x.FinancialYearName).ToList());
            return PartialView("Select", list);
        }
        [NoCache]
        public ActionResult DesignationforView()
        {
            var list = Common.PopulateDllList(_prmCommonSevice.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }

        #endregion

        #region Create & Edit

        public ActionResult Create()
        {
            JobRequisitionInfoViewModel model = new JobRequisitionInfoViewModel();
            model = PrepareForLogin(model);
            model.RequisitionSubDate = DateTime.UtcNow;
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] JobRequisitionInfoViewModel model)
        {

            try
            {
                string errorList = string.Empty;
                model.IsError = 1;
                errorList = BusinessLogicValidation(model);

                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, true);

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

                    }
                    _prmCommonSevice.PRMUnit.JobRequisitionInfoRepository.Add(entity);
                    _prmCommonSevice.PRMUnit.JobRequisitionInfoRepository.SaveChanges();
                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                }
                else
                {
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = errorList;
                    populateDropdown(model);
                    return View(model);
                }
            }
            catch
            {
                model.IsError = 1;
                model.errClass = "failed";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
            }
            populateDropdown(model);
            return View(model);
        }
        public ActionResult Edit(int id, string type)
        {
            var JobRequisitionEntity = _prmCommonSevice.PRMUnit.JobRequisitionInfoRepository.GetByID(id);
            var parentModel = JobRequisitionEntity.ToModel();
            DownloadDoc(parentModel);
            parentModel.strMode = "Edit";
            parentModel.IsInEditMode = true;
            parentModel = PrepareForLogin(parentModel);

            List<JobRequisitionInfoDetailsViewModel> list = (from job in _prmCommonSevice.PRMUnit.JobRequisitionInfoRepository.GetAll()
                                                             join jobDtl in _prmCommonSevice.PRMUnit.JobRequisitionInfoDetailRepository.GetAll() on job.Id equals jobDtl.RequisitionInfoId
                                                             where (jobDtl.RequisitionInfoId == id)
                                                             select new JobRequisitionInfoDetailsViewModel()
                                                             {
                                                                 Id = jobDtl.Id,
                                                                 DepartmentId = jobDtl.DepartmentId,
                                                                 DepartmentName = jobDtl.PRM_Division.Name,
                                                                 SectionId =jobDtl.SectionId==null?null:jobDtl.SectionId,
                                                                 SectionName =jobDtl.SectionId==null?string.Empty:jobDtl.PRM_Section.Name,
                                                                 DesignationId = jobDtl.DesignationId,
                                                                 DesignationName = jobDtl.PRM_Designation.Name,
                                                                 EmploymentTypeId = jobDtl.EmploymentTypeId,
                                                                 SalaryScaleId = jobDtl.SalaryScaleId,
                                                                 SalaryScaleName = jobDtl.PRM_JobGrade.PRM_GradeStep.First().StepAmount.ToString()+ " - "+jobDtl.PRM_JobGrade.PRM_GradeStep.Last().StepAmount.ToString(),
                                                                 RequireDate = jobDtl.RequireDate.Value.Date,
                                                                 NumOfRequiredPost = jobDtl.NumOfRequiredPost,
                                                                 JobDescription = jobDtl.JobDescription,
                                                                 EduRequirement = jobDtl.EduRequirement,
                                                                 ExpRequirement = jobDtl.ExpRequirement,
                                                                 AdditionalRequirement = jobDtl.AdditionalRequirement

                                                             }).ToList();

            parentModel.JobRequisitionInfoDetail = list;
            populateDropdown(parentModel);
            if (type == "success")
            {
                parentModel.IsError = 1;
                parentModel.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            else if (type == "saveSuccess")
            {
                parentModel.IsError = 1;
                parentModel.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }
            return View("Edit", parentModel);
        }


        [HttpPost]
        public ActionResult Edit([Bind(Exclude = "Attachment")] JobRequisitionInfoViewModel model)
        {
            try
            {
                string errorList = "";
                var attachment = Request.Files["attachment"];
                if (ModelState.IsValid)
                {
                    // Set preious attachment if exist

                    var obj = _prmCommonSevice.PRMUnit.JobRequisitionInfoRepository.GetByID(model.Id);
                    model.Attachment = obj.Attachment == null ? null : obj.Attachment;
                    //
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, false);

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
                    }

                    if (errorList.Length == 0)
                    {
                        _prmCommonSevice.PRMUnit.JobRequisitionInfoRepository.Update(entity);
                        _prmCommonSevice.PRMUnit.JobRequisitionInfoRepository.SaveChanges();
                        return RedirectToAction("Edit", new { id = model.Id, type = "success" });
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }
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
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }

            populateDropdown(model);
            return View(model);
        }
        #endregion

        #region Delete.....

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _prmCommonSevice.PRMUnit.JobRequisitionInfoRepository.GetByID(id);
            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(PRM_JobRequisitionInfoDetail) };
                    _prmCommonSevice.PRMUnit.JobRequisitionInfoRepository.Delete(tempPeriod.Id, allTypes);
                    _prmCommonSevice.PRMUnit.JobRequisitionInfoRepository.SaveChanges();
                    result = true;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                }
            }
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }
            return Json(new
            {
                Success = result,
                Message = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("DeleteJobRequisitionDetail")]
        public JsonResult DeleteJobRequisitionDetailfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonSevice.PRMUnit.JobRequisitionInfoDetailRepository.Delete(id);
                _prmCommonSevice.PRMUnit.JobRequisitionInfoDetailRepository.SaveChanges();
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
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }


            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });

        }
        #endregion

        [NoCache]
        public string BusinessLogicValidation(JobRequisitionInfoViewModel model)
        {
            string errorMessage = string.Empty;
            var requInfo = _prmCommonSevice.PRMUnit.JobRequisitionInfoRepository.GetAll().Where(x => x.RequisitionNo == model.RequisitionNo).ToList();
            if (requInfo.Count > 0)
            {
                errorMessage = "Requisition No. Already Exist";
            }
            return errorMessage;
        }

        public JobRequisitionInfoViewModel PrepareForLogin(JobRequisitionInfoViewModel model)
        {
            if (model.strMode == "Create")
            {
                List<JobRequisitionInfoViewModel> list = (from emp in _prmCommonSevice.PRMUnit.EmploymentInfoRepository.GetAll()
                                                          join des in _prmCommonSevice.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                                                          where (emp.EmpID == model.IUser)
                                                          select new JobRequisitionInfoViewModel()
                                                          {
                                                              PreparedById = emp.Id,
                                                              RequisitionPreparedBy = emp.FullName,
                                                              Designation = des.Name
                                                          }).ToList();
                foreach (var item in list)
                {
                    model.PreparedById = item.PreparedById;
                    model.RequisitionPreparedBy = item.RequisitionPreparedBy;
                    model.Designation = item.Designation;
                }

            }
            else if (model.IsInEditMode == true)
            {
                List<JobRequisitionInfoViewModel> list = (from emp in _prmCommonSevice.PRMUnit.EmploymentInfoRepository.GetAll()
                                                          join des in _prmCommonSevice.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                                                          where (emp.Id == model.PreparedById)
                                                          select new JobRequisitionInfoViewModel()
                                                          {
                                                              PreparedById = emp.Id,
                                                              RequisitionPreparedBy = emp.FullName,
                                                              Designation = des.Name
                                                          }).ToList();
                foreach (var item in list)
                {
                    model.PreparedById = item.PreparedById;
                    model.RequisitionPreparedBy = item.RequisitionPreparedBy;
                    model.Designation = item.Designation;
                }

            }
            return model;
        }

        private PRM_JobRequisitionInfo CreateEntity(JobRequisitionInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            foreach (var c in model.JobRequisitionInfoDetail)
            {
                var prm_JobRequisitionInfoDetail = new PRM_JobRequisitionInfoDetail();

                prm_JobRequisitionInfoDetail.Id = c.Id;
                prm_JobRequisitionInfoDetail.DepartmentId = c.DepartmentId;
                prm_JobRequisitionInfoDetail.DesignationId = c.DesignationId;
                prm_JobRequisitionInfoDetail.SectionId = c.SectionId;
                prm_JobRequisitionInfoDetail.EmploymentTypeId = c.EmploymentTypeId;
                prm_JobRequisitionInfoDetail.SalaryScaleId = c.SalaryScaleId;
                prm_JobRequisitionInfoDetail.RequireDate = c.RequireDate.Value;
                prm_JobRequisitionInfoDetail.NumOfRequiredPost = c.NumOfRequiredPost;
                prm_JobRequisitionInfoDetail.JobDescription = c.JobDescription;
                prm_JobRequisitionInfoDetail.EduRequirement = c.EduRequirement;
                prm_JobRequisitionInfoDetail.ExpRequirement = c.ExpRequirement;
                prm_JobRequisitionInfoDetail.AdditionalRequirement = c.AdditionalRequirement;
                prm_JobRequisitionInfoDetail.IUser = User.Identity.Name;
                prm_JobRequisitionInfoDetail.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    prm_JobRequisitionInfoDetail.IUser = User.Identity.Name;
                    prm_JobRequisitionInfoDetail.IDate = DateTime.Now;
                    //entity.PRM_JobRequisitionInfoDetail.Add(prm_JobRequisitionInfoDetail);
                }
                else
                {
                    prm_JobRequisitionInfoDetail.RequisitionInfoId = model.Id;
                    prm_JobRequisitionInfoDetail.EUser = User.Identity.Name;
                    prm_JobRequisitionInfoDetail.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _prmCommonSevice.PRMUnit.JobRequisitionInfoDetailRepository.Add(prm_JobRequisitionInfoDetail);
                    }
                    else
                    {
                        _prmCommonSevice.PRMUnit.JobRequisitionInfoDetailRepository.Update(prm_JobRequisitionInfoDetail);

                    }
                    _prmCommonSevice.PRMUnit.JobRequisitionInfoDetailRepository.SaveChanges();
                }
            }

            return entity;
        }

        #region Polulate Dropdown
        private void populateDropdown(JobRequisitionInfoViewModel model)
        {
            #region Department
            var div = _prmCommonSevice.PRMUnit.DivisionRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.DepartmentList = Common.PopulateDllList(div);
            #endregion

            #region Office
            var office = _prmCommonSevice.PRMUnit.DisciplineRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.OfficeList = Common.PopulateDllList(office);
            #endregion

            #region Section
            var sec = _prmCommonSevice.PRMUnit.SectionRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.SectionList = Common.PopulateDllList(sec);
            #endregion

            #region Salary Scale
            //var sal = _prmCommonSevice.PRMUnit.SalaryScaleRepository.Fetch().OrderByDescending(x => x.SalaryScaleName).ToList();
            //model.SalaryScaleList = Common.PopulateSalaryScaleDDL(sal);
            #endregion

            #region Employment Type
            var emp = _prmCommonSevice.PRMUnit.EmploymentTypeRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.EmploymentTypeList = Common.PopulateDllList(emp);
            #endregion

            #region Financial Year
            var year = _famCommonService.FAMUnit.FinancialYearInformationRepository.Fetch().Where(p=>p.IsActive==true).OrderBy(x => x.FinancialYearName).ToList();
            model.FinancialYearList = Common.PopulateFinancialYearDllList(year);
            #endregion

        }
        #endregion

        #region Sanctioned post
        [NoCache]
        public JsonResult GetRequsionInfo(JobRequisitionInfoViewModel model)
        {
            var directRec = _prmCommonSevice.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.DesignationId == model.DesignationId).Where(x => x.EmploymentProcessId == 1).ToList();
            var promotion = _prmCommonSevice.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.DesignationId == model.DesignationId).Where(x => x.EmploymentProcessId != 1).ToList();
            var directRecuri = directRec.Count();
            var Promoted = promotion.Count();

            var SanctionPost = _prmCommonSevice.PRMUnit.OrganizationalSetupManpowerInfoRepository.GetAll().Where(x => x.DesignationId == model.DesignationId && x.OrganogramLevelId == model.OrganogramLevelId).FirstOrDefault();
            int post =SanctionPost ==null? 0:SanctionPost.SanctionedPost;

            return Json(new
            {
                SancTotal = post,

                FillDirect = directRecuri,
                FillPro = Promoted,
                FillTotal = directRecuri + Promoted,

                VacantTotal = post - (directRecuri + Promoted)
            });

        }
        #endregion

        public ActionResult SalaryScale(int designationId)
        {
            var items = (from de in _empService.PRMUnit.DesignationRepository.Fetch()
                         join JG in _empService.PRMUnit.JobGradeRepository.Fetch() on de.GradeId equals JG.Id
                         where de.Id == designationId
                         select new
                         {
                             GradeId = de.GradeId,
                             GradeName = JG.GradeName
                         }).FirstOrDefault();

            //var tempList = _prmCommonSevice.PRMUnit.EmploymentInfoRepository.Fetch().Where(t => t.DesignationId == designationId).ToList();
            int jobGradeId = items.GradeId;

            var list = _prmCommonSevice.PRMUnit.JobGradeRepository.Fetch().Where(t => t.Id == jobGradeId).Select(x => new { Id = x.Id, Name = x.PayScale }).OrderBy(x => x.Name).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
        }

        public JsonResult GetFirstAndLastStep(int designationId)
        {
            var items = (from de in _empService.PRMUnit.DesignationRepository.Fetch()
                         join JG in _empService.PRMUnit.JobGradeRepository.Fetch() on de.GradeId equals JG.Id
                         where de.Id == designationId
                         select new
                         {
                             GradeId = de.GradeId,
                             GradeName = JG.GradeName
                         }).FirstOrDefault();

            int jobGradeId = items.GradeId;
            var firstStep = _prmCommonSevice.PRMUnit.JobGradeStepRepository.GetAll().Where(x => x.StepName == 1 && x.JobGradeId == jobGradeId).FirstOrDefault();
            var LastStep = _prmCommonSevice.PRMUnit.JobGradeStepRepository.GetAll().Where(x =>x.JobGradeId == jobGradeId).LastOrDefault();

            var step1 = firstStep.StepAmount == null ? "0" : firstStep.StepAmount.ToString();
            var y = LastStep.StepAmount == null ? "0" : LastStep.StepAmount.ToString();
            var amount = step1 + " - " + y;

            return Json(new
            {
                amount = amount
            });
        }

        #region Loading Designation
        public ActionResult LoadDesignation(int departmentId, int? sectionId, int? officeId)
        {
            if (sectionId != null && sectionId != 0)
            {
                var tempList = (from sec in _prmCommonSevice.PRMUnit.SectionRepository.GetAll()
                                join orgManP in _prmCommonSevice.PRMUnit.OrganizationalSetupManpowerInfoRepository.GetAll() on sec.Id equals orgManP.OrganogramLevelId
                                join des in _prmCommonSevice.PRMUnit.DesignationRepository.GetAll() on orgManP.DesignationId equals des.Id
                                where (sec.Id == sectionId)
                                select des
                              ).ToList();

                var list = tempList.Select(x => new { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();

                return Json(list, JsonRequestBehavior.AllowGet
                );
            }
            else if (officeId != null && officeId != 0)
            {
                var tempList = (from ofi in _prmCommonSevice.PRMUnit.DisciplineRepository.GetAll()
                                join orgManP in _prmCommonSevice.PRMUnit.OrganizationalSetupManpowerInfoRepository.GetAll() on ofi.Id equals orgManP.OrganogramLevelId
                                join des in _prmCommonSevice.PRMUnit.DesignationRepository.GetAll() on orgManP.DesignationId equals des.Id
                                where (ofi.Id == officeId)
                                select des
              ).ToList();

                var list = tempList.Select(x => new { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();

                return Json(list, JsonRequestBehavior.AllowGet
                );

            }
            else
            {
                var tempList = (from div in _prmCommonSevice.PRMUnit.DivisionRepository.GetAll()
                                join orgManP in _prmCommonSevice.PRMUnit.OrganizationalSetupManpowerInfoRepository.GetAll() on div.Id equals orgManP.OrganogramLevelId
                                join des in _prmCommonSevice.PRMUnit.DesignationRepository.GetAll() on orgManP.DesignationId equals des.Id
                                where (div.Id == departmentId)
                                select des
              ).ToList();

                var list = tempList.Select(x => new { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();

                return Json(list, JsonRequestBehavior.AllowGet
                );

            }
        }
        #endregion

        #region Attachment

        private int Upload(JobRequisitionInfoViewModel model)
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

        public void DownloadDoc(JobRequisitionInfoViewModel model)
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

        [NoCache]
        public JsonResult GetOrganogramInfo(int organogramLevelId)
        {
            var obj = _empService.GetEmpDepartmentOfficeSectionSubSection(organogramLevelId);

            return Json(new
            {
                DepId = obj.DepartmentId,
                OfficeId = obj.OfficeId,
                SecId = obj.SectionId,
                //Department = obj.DivisionId == null ? string.Empty : obj.PRM_Division.Name,
                //Section = obj.PRM_Section == null ? string.Empty : obj.PRM_Section.Name
            });
        }
    }
}