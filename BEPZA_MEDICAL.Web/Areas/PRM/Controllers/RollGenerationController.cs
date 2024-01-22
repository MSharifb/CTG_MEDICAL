using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class RollGenerationController : Controller
    {
        #region Fields
        private readonly EmployeeService _empService;
        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Ctor
        public RollGenerationController(EmployeeService empService, PRMCommonSevice prmCommonService)
        {
            this._empService = empService;
            this._prmCommonservice = prmCommonService;
        }
        #endregion

        // GET: PRM/RollGeneration
        public ActionResult Index()
        {
            RollGenerationViewModel model = new RollGenerationViewModel();
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(RollGenerationViewModel model)
        {
            string errorList = "";
            string Message = "";

            try
            {
                int result = 0;
                if (ModelState.IsValid)
                {
                    try
                    {
                        result = _prmCommonservice.PRMUnit.FunctionRepository.RollNoGeneration
                            (
                                model.AdCode,
                                model.DesignationId,
                                model.RollNoFrom,
                                User.Identity.Name
                            );

                            model.IsError = 0;
                            model.ErrMsg = "Roll Generation Process has been completed successfully.";
                            model.errClass = "success";
                       
                    }
                    catch (Exception ex)
                    {
                        model.IsError = 1;
                        if (ex.InnerException != null && ex.InnerException is SqlException)
                        {
                            SqlException sqlException = ex.InnerException as SqlException;
                            model.ErrMsg = ex.InnerException.Message; // Common.GetSqlExceptionMessage(sqlException.Number);
                        }
                        else
                        {
                            model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                        }
                        model.errClass = "failed";
                    }
                }
               
            }
            catch (Exception ex)
            {
                model.IsError = 1;
                if (errorList == string.Empty)
                {
                    model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(Message) ? ErrorMessages.InsertFailed : Message) : Common.GetModelStateError(ModelState);
                }
                else
                {
                    model.ErrMsg = errorList;
                }
            }
            populateDropdown(model);
            return View("Index",model);
        }


        private void populateDropdown(RollGenerationViewModel model)
        {
            dynamic ddlList;

            #region job advertisement
            DateTime cDate = DateTime.Now;
            ddlList = _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll().OrderBy(x => x.AdCode).ToList();
            model.JobAdvertisementInfoList = Common.PopulateJobAdvertisementDDL(ddlList);

            #endregion

            var jobPostList = _prmCommonservice.PRMUnit.JobAdvertisementPostDetailRepository.GetAll().OrderBy(x => x.Id).ToList();
            model.JobPostList = Common.PopulateJobPostAdvertisementDDL(jobPostList);
        }

        public ActionResult GetDesignation(Int32 advertisementId)
        {
            var advertisemenPostList = _prmCommonservice.PRMUnit.JobAdvertisementPostDetailRepository.GetAll().Where(e=> e.JobAdvertisementInfoId == advertisementId).OrderBy(x => x.Id).ToList();

            return PartialView("SelectOp", Common.PopulateJobPostAdvertisementDDL(advertisemenPostList));
        }
    }
}