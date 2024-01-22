using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Domain.FAM;
using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Domain.PGM;
 
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.VoucherSearchModel;
 
using Lib.Web.Mvc.JQuery.JqGrid;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class VoucherSearchController : Controller
    {
        #region Fields
        private readonly FAMCommonService _famCommonservice;
        private readonly CPFCommonService _cpfCommonservice;
        private readonly PRMCommonSevice _prmCommonservice;
        //private readonly PIMCommonService _pimCommonservice;
        private readonly ResourceInfoService _RresourceInfoService;
        #endregion

        #region Constructor

        public VoucherSearchController(FAMCommonService famCommonService, CPFCommonService cpfCommonservice, PRMCommonSevice prmCommonService, /* */ PGMCommonService pgmCommonservice, ResourceInfoService service)
        {
            _famCommonservice = famCommonService;
            _cpfCommonservice = cpfCommonservice;
            _prmCommonservice = prmCommonService;
             
            this._RresourceInfoService = service;
        }
        #endregion
        
        #region Properties

        public string Message { get; set; }

        #endregion


        #region Actions---------

        public ActionResult Index()
        {
            return View();
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //[NoCache]
        //public ActionResult GetList(JqGridRequest request, VoucherSearchViewModel  model)
        //{
        //    string filterExpression = String.Empty;
        //    int totalRecords = 0;
        //    //int pLeaderId = default(int);
        //    //int pSupervisorId = default(int); 


        //    var projectList = _pimCommonservice.PIMUnit.ProjectInformation.GetAll().ToList();
        //    var projectTypeList = _pimCommonservice.PIMUnit.ProjectType.GetAll().ToList();
        //    var projectCategoryList = _pimCommonservice.PIMUnit.ProjectCatagory.GetAll().ToList();
        //    var employeeList = _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll().ToList();
        //    var divisionList = _prmCommonservice.PRMUnit.DivisionRepository.GetAll().ToList();

        //    var voucherList = _famCommonservice.FAMUnit.voucherMaster.GetAll().Where(c=> c.VoucherTypeId ==4).ToList();
        //    var voucherDetailList = _famCommonservice.FAMUnit.voucherDetails.GetAll().Where(c=> c.Debit > 0).ToList();

        //    #region PL/ PS ID Finding--------
        //    //if (!string.IsNullOrEmpty(model.ProjectLeaderInitial))
        //    //{
        //    //    pLeaderId = (from PL in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() where PL.EmployeeInitial == model.ProjectLeaderInitial select PL.Id).FirstOrDefault();
        //    //}

        //    //if (!string.IsNullOrEmpty(model.ProjectSupervisorInitial))
        //    //{
        //    //    pSupervisorId = (from PL in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() where PL.EmployeeInitial == model.ProjectSupervisorInitial select PL.Id).FirstOrDefault();
        //    //}

        //    #endregion

        //    List<VoucherSearchViewModel> list = new List<VoucherSearchViewModel>();

        //    if (voucherList != null && voucherList.Count() != 0)
        //    {
        //        list = (from v in voucherList
        //                join vd in voucherDetailList on v.Id equals vd.VoucherId
        //                join p in projectList on vd.ProjectId equals p.Id

        //                where (string.IsNullOrEmpty(model.VoucherNumber) || v.VoucherNumber == model.VoucherNumber.Trim())
        //                && (string.IsNullOrEmpty(model.ProjectNo) || p.ProjectNo == model.ProjectNo.Trim())
        //                && (string.IsNullOrEmpty(model.ProjectTitle) || p.ProjectTitle.ToLower().Contains(model.ProjectTitle.Trim().ToLower()))
                        


        //                select new VoucherSearchViewModel()
        //                {
        //                    Id = v.Id,
        //                    VoucherNumber = v.VoucherNumber,
        //                    VoucherDate = v.VoucherDate.ToString("dd-MMM-yyyy"),
        //                    ReferenceNumber = v.ReferenceNumber,
        //                    ProjectNo = p.ProjectNo,
        //                    ProjectId = vd.ProjectId,
        //                    Debit = vd.Debit,
        //                    ProjectTitle = p.ProjectTitle
        //                }).OrderBy(x => x.VoucherNumber).ToList();
                    
        //    }

        //    #region Searching-----------

        //    //if (model.SelectedStatus == 1)
        //    //{
        //    //    list = list.Where(q => q.ActualEndDate==null).ToList();
        //    //}
        //    //if (model.SelectedStatus == 2)
        //    //{
        //    //    list = list.Where(q => q.ActualEndDate != null).ToList();
        //    //}

        //    //if (pLeaderId > 0)
        //    //{
        //    //    list = list.Where(q => q.ProjectLeaderId == pLeaderId).ToList();
        //    //}
        //    //if (pSupervisorId > 0)
        //    //{
        //    //    list = list.Where(q => q.ProjectSupervisorId == pSupervisorId).ToList();
        //    //}

        //    #endregion

        //    totalRecords = list == null ? 0 : list.Count;

        //    JqGridResponse response = new JqGridResponse()
        //    {
        //        TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
        //        PageIndex = request.PageIndex,
        //        TotalRecordsCount = totalRecords
        //    };

        //    list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

        //    foreach (var d in list)
        //    {
        //        response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
        //        {
        //            d.Id,
        //            d.VoucherNumber,
        //            d.VoucherDate,
        //            d.ReferenceNumber,
        //            d.ProjectNo,
        //            d.ProjectTitle,    
        //            d.Debit,                                    
        //            "SelectedStatus"
        //        }));
        //    }
        //    return new JqGridJsonResult() { Data = response };
        //}

        [NoCache]
        public ActionResult SearchInvoiceVoucher()
        {
            var model = new VoucherSearchViewModel();
            return View("SearchVoucher", model);
        }

        #endregion

        #region Utilities-------

        [NoCache]
        public ActionResult GetDivision()
        {
            var divisions = _prmCommonservice.PRMUnit.DivisionRepository.GetAll().OrderBy(x=>x.Name).ToList();

            return PartialView("Select", Common.PopulateDllList(divisions));
        }

        //[NoCache]
        //public ActionResult GetProjectType()
        //{
        //    var pType = _pimCommonservice.PIMUnit.ProjectType.GetAll().OrderBy(x => x.ProjectType).ToList();

        //    return PartialView("Select", Common.PopulateProjectTypeDllList(pType));
        //}

        //[NoCache]
        //public ActionResult GetProjectCategory()
        //{
        //    var pCategory = _pimCommonservice.PIMUnit.ProjectCatagory.GetAll().OrderBy(x => x.Name).ToList();

        //    return PartialView("Select", Common.PopulateDllList(pCategory));
        //}

        #endregion

        #region Autocomplete for Project Searching ---------------

        //public JsonResult AutoCompleteForProposedProjectSearch(string projectNo)  
        //{
        //    var items = _pimCommonservice.PIMUnit.SpecialParameter.GetAll().DistinctBy(x => x.ProposedProjectTypeId).Select(d => d.ProposedProjectTypeId);

        //    var result = (from r in _pimCommonservice.PIMUnit.ProjectInformation.GetAll()
        //                  where r.ProjectNo.ToLower().StartsWith(projectNo.ToLower()) && r.ActualEndDate == null && items.Contains(r.ProjectTypeId)
        //                  select new
        //                  {
        //                      Id = r.Id,
        //                      ProjectNo = r.ProjectNo,
        //                      ProjectTitle = Convert.ToString(r.ProjectTitle).Length > 100 ? Convert.ToString(r.ProjectTitle).Substring(0, 100) + "..." : r.ProjectTitle
        //                  }).Distinct().OrderBy(x => x.ProjectNo);

        //    return Json(result, JsonRequestBehavior.AllowGet);

        //}

        //public JsonResult AutoCompleteForActiveProjectSearch(string projectNo) 
        //{
        //    var items = _pimCommonservice.PIMUnit.SpecialParameter.GetAll().DistinctBy(x => x.ActiveProjectTypeId).Select(d => d.ActiveProjectTypeId);

        //    var result = (from r in _pimCommonservice.PIMUnit.ProjectInformation.GetAll()
        //                  where r.ProjectNo.ToLower().StartsWith(projectNo.ToLower()) && r.ActualEndDate == null && items.Contains(r.ProjectTypeId)
        //                  select new {
        //                      Id = r.Id,
        //                      ProjectNo = r.ProjectNo,
        //                      ProjectTitle = Convert.ToString(r.ProjectTitle).Length > 100 ? Convert.ToString(r.ProjectTitle).Substring(0, 100) + "..." : r.ProjectTitle
        //                  }).Distinct().OrderBy(x => x.ProjectNo);

        //    return Json(result, JsonRequestBehavior.AllowGet);

        //}

        //public JsonResult AutoCompleteForProjectSearching(string projectNo) 
        //{
        //    var result = (from r in _pimCommonservice.PIMUnit.ProjectInformation.GetAll()
        //                  where r.ProjectNo.ToLower().StartsWith(projectNo.ToLower()) && r.ActualEndDate == null
        //                  select new {
        //                      Id = r.Id,
        //                      ProjectNo = r.ProjectNo,
        //                      ProjectTitle = Convert.ToString(r.ProjectTitle).Length > 100 ? Convert.ToString(r.ProjectTitle).Substring(0, 100) + "..." : r.ProjectTitle
        //                  }).Distinct().OrderBy(x => x.ProjectNo);

        //    return Json(result, JsonRequestBehavior.AllowGet);

        //}

        #endregion
    }
}
