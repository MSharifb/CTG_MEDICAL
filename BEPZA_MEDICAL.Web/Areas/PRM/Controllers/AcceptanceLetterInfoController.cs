using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class AcceptanceLetterInfoController : BaseController
    {
        #region Fields
        private readonly EmployeeService _empService;
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public AcceptanceLetterInfoController(EmployeeService empService, PRMCommonSevice prmCommonService)
        {
            this._empService = empService;
            this._prmCommonService = prmCommonService;
        }
        #endregion


        //
        // GET: /PRM/AcceptanceLetterInfo/
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, AcceptanceLetterInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<AcceptanceLetterInfoViewModel> list = (from apLetter in _prmCommonService.PRMUnit.AcceptanceLetterInfoRepository.GetAll()
                                                         join applicantInfo in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on apLetter.ApplicantInfoId equals applicantInfo.intPK
                                                         join desig in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on applicantInfo.DesignationId equals desig.Id
                                                         where (viewModel.DesignationId == 0 || viewModel.DesignationId == null || desig.Id == viewModel.DesignationId)
                                                         && (viewModel.DateOfAcceptance == null || viewModel.DateOfAcceptance == apLetter.DateOfAcceptance)
                                                         && (viewModel.SelectedId == null || viewModel.SelectedId == 0 || apLetter.SelectedId == viewModel.SelectedId)
                                                         && (viewModel.ApplicantName == null || viewModel.ApplicantName == "" || applicantInfo.Name.Contains(viewModel.ApplicantName))
                                                         //&& (applicantInfo.ZoneInfoId==LoggedUserZoneInfoId)
                                                        select new AcceptanceLetterInfoViewModel()
                                                         {
                                                             Id = apLetter.Id,
                                                             ApplicantName = applicantInfo.Name,
                                                             DesignationId = applicantInfo.DesignationId,
                                                             ApplicantPosition = desig.Name,
                                                             SelectedId = apLetter.SelectedId,
                                                             DateOfAcceptance = apLetter.DateOfAcceptance,
                                                         }).OrderBy(x => x.ApplicantName).ToList();


            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "SelectedId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SelectedId).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SelectedId).ToList();
                }
            }
            if (request.SortingName == "ApplicantName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ApplicantName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ApplicantName).ToList();
                }
            }
            if (request.SortingName == "ApplicantPosition")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ApplicantPosition).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ApplicantPosition).ToList();
                }
            }
            if (request.SortingName == "DateOfAcceptance")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DateOfAcceptance).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DateOfAcceptance).ToList();
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
                    d.SelectedId, 
                    d.ApplicantName,
                    d.DesignationId,
                    d.ApplicantPosition,  
                    Convert.ToDateTime(d.DateOfAcceptance).ToString(DateAndTime.GlobalDateFormat),
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }

        [NoCache]
        public ActionResult DesignationListView()
        {
            var designations =((from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Fetch()
                                join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.Fetch() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                join des in _prmCommonService.PRMUnit.DesignationRepository.Fetch() on jobAdReq.DesignationId equals des.Id
                                select des).Concat(from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Fetch()
                                                   join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.Fetch() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                                   join des in _prmCommonService.PRMUnit.DesignationRepository.Fetch() on jobAdReq.DesignationId equals des.Id
                                                   select des)).DistinctBy(x => x.Id).OrderBy(o => o.SortingOrder).ToList();

            return PartialView("Select", Common.PopulateDllList(designations));
        }

        public ActionResult Create()
        {
            AcceptanceLetterInfoViewModel model = new AcceptanceLetterInfoViewModel();
            model.Subject = "Acceptance Letter";
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(AcceptanceLetterInfoViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {

                    var entity = model.ToEntity();

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.AcceptanceLetterInfoRepository.Add(entity);
                        _prmCommonService.PRMUnit.AcceptanceLetterInfoRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.IsError = 1;
                model.errClass = "failed";
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

            return View(model);
        }
        public ActionResult Edit(int id, string type)
        {
            var entity = _prmCommonService.PRMUnit.AcceptanceLetterInfoRepository.GetByID(id);
            var model = entity.ToModel();

            model.strMode = "Edit";
            model.ApplicantName = entity.EREC_tblgeneralinfo.Name;
            model.ApplicantPosition = _prmCommonService.PRMUnit.DesignationRepository.Get(x => x.Id == entity.EREC_tblgeneralinfo.DesignationId).Select(s => s.Name).FirstOrDefault();

            model.EmpId = entity.PRM_EmploymentInfo.EmpID;
            model.EmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.DesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;

            if (type == "success")
            {
                model.IsError = 1;
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            return View("Edit", model);
        }


        [HttpPost]
        public ActionResult Edit(AcceptanceLetterInfoViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {

                    var entity = model.ToEntity();
                    if (errorList.Length == 0)
                    {
                        entity.EUser = User.Identity.Name;
                        entity.EDate = DateTime.Now;
                        _prmCommonService.PRMUnit.AcceptanceLetterInfoRepository.Update(entity);
                        _prmCommonService.PRMUnit.AcceptanceLetterInfoRepository.SaveChanges();
                        return RedirectToAction("Edit", new { id = model.Id, type = "success" });
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 0;
                    model.ErrMsg = errorList;
                }

            }
            catch (Exception ex)
            {
                model.IsError = 0;
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

            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _prmCommonService.PRMUnit.AcceptanceLetterInfoRepository.GetByID(id);
            try
            {
                _prmCommonService.PRMUnit.AcceptanceLetterInfoRepository.Delete(id);
                _prmCommonService.PRMUnit.AcceptanceLetterInfoRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
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

	}
}