using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class DistrictWiseApplicantInfoController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonSevice;
        private readonly ERP_BEPZAPRMEntities _prmContext;
        #endregion

        #region Constructor

        public DistrictWiseApplicantInfoController(PRMCommonSevice prmCommonSevice, ERP_BEPZAPRMEntities prmContext)
        {
            this._prmCommonSevice = prmCommonSevice;
            this._prmContext = prmContext;
        }

        #endregion

        // GET: PRM/DistrictWiseApplicantInfo
        public ActionResult Index()
        {
            DistrictWiseApplicantInfoViewModel model = new DistrictWiseApplicantInfoViewModel();
            PopulateDDL(model);
            return View(model);
        }
        //[NonAction]
        //private QueryAnalyzerViewModel ConstructQuery(QueryAnalyzerViewModel model)
        //{
        //    //var dsResult = new DataSet();
        //    //model.ReportDate = DateTime.Now;
        //    //model.DtReport = DynamicQueryAnalyzerRepository.GetEmpInfo(model.EmpId, model.EmployeeName, model.DepartmentId, model.SectionId, model.DesignationId, model.EmpCategoryId, model.EmpStatusId, model.MobileNo);
        //    //return model;
        //}

        [HttpPost]
        public ActionResult Index(DistrictWiseApplicantInfoViewModel model)
        {
            List<ReportTemp> list = new List<ReportTemp>();
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            //var data = _prmContext.EREC_uspReportDistrictWiseApplicants(model.JobAdvertisementId, 0).ToList();
            //foreach(var item  in data){
            //    var dis = new ReportTemp
            //    {
            //        Name = item.Name,
            //        ID = item.id,
            //        TotalNo = Convert.ToInt32(item.TotalNo)
            //    };
            //    list.Add(dis);
            //}
            model.reportTemp = list;
            PopulateDDL(model);
            return View(model);
        }

        public void PopulateDDL(DistrictWiseApplicantInfoViewModel model)
        {
            dynamic List;

            #region Post
            List = _prmCommonSevice.PRMUnit.DesignationRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.DesignationList = Common.PopulateDllList(List);
            #endregion

            #region JobAd
            List = _prmCommonSevice.PRMUnit.JobAdvertisementInfoRepository.GetAll().ToList();
            model.JobAdvertisementList = Common.PopulateJobAdvertisementDDL(List);
            #endregion
        }
    }
}