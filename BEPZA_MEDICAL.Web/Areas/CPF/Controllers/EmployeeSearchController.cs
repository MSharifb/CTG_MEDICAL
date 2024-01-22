using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Domain.CPF;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.EmployeeSearchViewModel;
using BEPZA_MEDICAL.Domain.PGM;


namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class EmployeeSearchController : Controller
    {

        #region Fields
        private readonly CPFCommonService _cpfCommonservice;

        private readonly PRMCommonSevice _prmCommonservice;
        private readonly PGMCommonService _pgmCommonservice;
        //private readonly PIMCommonService _pimCommonservice;
        private readonly ResourceInfoService _RresourceInfoService;
        #endregion

        #region Constructor

        public EmployeeSearchController(CPFCommonService cpfCommonservice, PRMCommonSevice prmCommonService, PGMCommonService pgmCommonservice, ResourceInfoService service)
        {
            _cpfCommonservice = cpfCommonservice;
            _prmCommonservice = prmCommonService;

            _pgmCommonservice = pgmCommonservice;
            this._RresourceInfoService = service;
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
        public ActionResult GetList(JqGridRequest request, EmployeeSearchViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var listMemberInfo = _cpfCommonservice.CPFUnit.MembershipInformationRepository.GetAll();
            var listEmployee = (from e in _prmCommonservice.PRMUnit.EmploymentInfoRepository.Fetch()
                                select new
                                {
                                    e.Id,
                                    e.EmpID,
                                    e.DivisionId,
                                    e.EmployeeInitial,
                                    e.DesignationId
                                }).ToList();
            var listDesignation = _prmCommonservice.PRMUnit.DesignationRepository.GetAll();

            var list = new List<EmployeeSearchViewModel>();

            if (listMemberInfo != null && listMemberInfo.Count() != 0)
            {
                list = (from M in listMemberInfo
                        join E in listEmployee on M.EmployeeId equals E.Id
                        join D in listDesignation on E.DesignationId equals D.Id
                        where (string.IsNullOrEmpty(model.EmpID) || E.EmpID == model.EmpID.Trim())
                        && (string.IsNullOrEmpty(model.EmployeeInitial) || E.EmployeeInitial.ToLower().Contains(model.EmployeeInitial.Trim().ToLower()))
                        && (string.IsNullOrEmpty(model.EmployeeName) || M.EmployeeName.ToLower().Contains(model.EmployeeName.Trim().ToLower()))
                        && (string.IsNullOrEmpty(model.MembershipID) || M.MembershipID == model.MembershipID.Trim())
                        && (model.DivisionId == 0 || E.DivisionId == model.DivisionId || model.DivisionId == null)
                        && M.MembershipID != null

                        select new EmployeeSearchViewModel()
                        {
                            Id = M.Id,
                            EmpID = E.EmpID,
                            MembershipID = M.MembershipID,
                            EmployeeName = M.EmployeeName,
                            Designation = D.Name,
                            DivisionId = E.DivisionId,
                            MembershipStatus = M.MembershipStatus,
                            SortOrder = D.SortingOrder
                        }).ToList();
            }

            if (model.SelectedEmployeeStatus == 1)
            {
                list = list.Where(q => q.MembershipStatus == Convert.ToString(BEPZA_MEDICAL.Web.MvcApplication.CPFMembershipStatus.Active)).ToList();
            }
            else if (model.SelectedEmployeeStatus == 2)
            {
                list = list.Where(q => q.MembershipStatus == Convert.ToString(BEPZA_MEDICAL.Web.MvcApplication.CPFMembershipStatus.Inactive)).ToList();
            }

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).OrderBy(x => x.SortOrder).ToList();

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.EmpID,
                    d.EmployeeName,                   
                    d.DivisionId,
                    d.Designation,
                    d.MembershipID,
                    "SelectedEmployeeStatus"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult EmployeeSearchList()
        {
            var model = new EmployeeSearchViewModel();
            return View("EmployeeSearchList", model);
        }

        #endregion

        #region Utilities-------

        [NoCache]
        public ActionResult GetDivision()
        {
            var divisions = _prmCommonservice.PRMUnit.DivisionRepository.GetAll().OrderBy(x => x.Name).ToList();

            return PartialView("Select", Common.PopulateDllList(divisions));
        }

        #endregion
    }
}
