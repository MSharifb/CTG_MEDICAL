using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.FixedParameters;


namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class FixedParametersController : Controller
    {
        private readonly CPFCommonService _cpfCommonservice;
        private readonly PRMCommonSevice _prmCommonservice;

        public FixedParametersController(PRMCommonSevice prmCommonService, CPFCommonService cpfCommonservice)
        {
            _prmCommonservice = prmCommonService;
            _cpfCommonservice = cpfCommonservice;
        }

        #region Actions
        public ActionResult Index()
        {
            var model = new FixedParametersSetupModel();

            model.CPFContributionRateSetupModel = GetCPFContributionRateSetup();
            //model.GratuityInterestRateSetupModel = GetGratuityInterestRateSetup();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(FixedParametersSetupModel model)
        {
            string errorList = string.Empty;
            model.strMode = CrudeAction.AddNew;

            try
            {
                //CPF Contribution Rate Setup 
                var cpfRateEntity = model.CPFContributionRateSetupModel.ToEntity();
                if (model.CPFContributionRateSetupModel.Id > 0)
                {
                    cpfRateEntity.EUser = System.Web.HttpContext.Current.User.Identity.Name;
                    cpfRateEntity.EDate = DateTime.Now;
                    _cpfCommonservice.CPFUnit.CPF_ContributionRateSetupRepository.Update(cpfRateEntity);
                }
                else
                {
                    _cpfCommonservice.CPFUnit.CPF_ContributionRateSetupRepository.Add(cpfRateEntity);
                }
                _cpfCommonservice.CPFUnit.CPF_ContributionRateSetupRepository.SaveChanges();
                //

                //Gratuity Interest Rate Setup
                //var gratuityEntity = model.GratuityInterestRateSetupModel.ToEntity();
                //if (model.GratuityInterestRateSetupModel.Id > 0)
                //{
                //    gratuityEntity.EUser = System.Web.HttpContext.Current.User.Identity.Name;
                //    gratuityEntity.EDate = DateTime.Now;
                //    _cpfCommonservice.CPFUnit.CPF_GratuityInterestRateRepository.Update(gratuityEntity);
                //}
                //else
                //{
                //    _cpfCommonservice.CPFUnit.CPF_GratuityInterestRateRepository.Add(gratuityEntity);
                //}

                //_cpfCommonservice.CPFUnit.CPF_GratuityInterestRateRepository.SaveChanges();
                //

                model.IsError = 0;
                model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //Error logging

                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.Message = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }

                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }

            //PopulateDropdown(model.GratuityInterestRateSetupModel);

            return View("Index", model);
        }

        #endregion

        #region Methods
        //private void PopulateDropdown(GratuityInterestRateSetupModel model)
        //{
        //    var listItems = _cpfCommonservice.CPFUnit.ManagePeriodInformationRepository.GetAll().ToList();
        //    //if (model == null) model = new GratuityInterestRateSetupModel();

        //    model.PeriodList = Common.PopulatePfPeriodDllList(listItems);
        //}

        private CPFContributionRateSetupModel GetCPFContributionRateSetup()
        {
            var model = new CPFContributionRateSetupModel();
            var item = _cpfCommonservice.CPFUnit.CPF_ContributionRateSetupRepository.GetAll().FirstOrDefault();

            model = item.ToModel();

            return model;
        }

        //private GratuityInterestRateSetupModel GetGratuityInterestRateSetup()
        //{
        //    var model = new GratuityInterestRateSetupModel();

        //    var item = _cpfCommonservice.CPFUnit.CPF_GratuityInterestRateRepository.GetAll().FirstOrDefault();            

        //    model = item.ToModel();

        //    if (model == null) model = new GratuityInterestRateSetupModel();
        //    PopulateDropdown(model);

        //    return model;
        //}

        #endregion
    }
}