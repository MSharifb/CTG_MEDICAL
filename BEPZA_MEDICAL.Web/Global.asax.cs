
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;

using BEPZA_MEDICAL.Web.Utility;
using Autofac;
using System.Reflection;
using Autofac.Integration.Mvc;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;

using BEPZA_MEDICAL.Web.Filters;


using BEPZA_MEDICAL.DAL.FAM;
using BEPZA_MEDICAL.Domain.FAM;
using BEPZA_MEDICAL.DAL.PGM;
using BEPZA_MEDICAL.Domain.PGM;
using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.DAL.AMS;
using BEPZA_MEDICAL.Domain.AMS;
using BEPZA_MEDICAL.DAL.SMS;
using BEPZA_MEDICAL.Domain.SMS;
using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.Domain.PMI;
using BEPZA_MEDICAL.DAL.WFM;
using BEPZA_MEDICAL.Domain.WFM;
using BEPZA_MEDICAL.DAL.FMS;
using BEPZA_MEDICAL.Domain.FMS;
using BEPZA_MEDICAL.DAL.FAR;
using BEPZA_MEDICAL.Domain.FAR;

//using BEPZA_MEDICAL.Domain.PMI;

//using ERP_BEPZA.Domain.ADC;

namespace BEPZA_MEDICAL.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            filters.Add(new AuthorizationFilterAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            /*  routes.MapRoute(
                  "Default", // Route name
                  "{controller}/{action}/{id}", // URL with parameters
                  new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                  new[] { AppConstant.ProjectName + ".Controllers" });*/

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Account", action = "LogOn", id = UrlParameter.Optional }, // Parameter defaults
                new[] { AppConstant.ProjectName + ".Controllers" });


            routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });

        }

        [System.Obsolete]
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            ModelBinders.Binders.DefaultBinder = new CustomModelBinder();

            BEPZA_MEDICAL.DB.Utility.strDBConnectionString = AppConstant.ConnectionString;    // ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            BEPZA_MEDICAL.DB.Utility.strDBProvider = AppConstant.Provider;

            #region IoC registration

            var builder = new ContainerBuilder();
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());


            #region DataLayer DI for AMS

            builder.Register(c => new ERP_BEPZA_AMSEntities()).InstancePerHttpRequest();
            builder.RegisterType<AMS_UnitOfWork>().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(AMS_GenericRepository<>)).InstancePerHttpRequest();
            builder.RegisterType<AMS_ExecuteFunctions>().InstancePerHttpRequest();

            #endregion

            #region DataLayer DI for SMS

            builder.Register(c => new ERP_BEPZASMSEntities()).InstancePerHttpRequest();
            builder.RegisterType<SMS_UnitOfWork>().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(SMS_GenericRepository<>)).InstancePerHttpRequest();
            builder.RegisterType<SMS_ExecuteFunctions>().InstancePerHttpRequest();

            #endregion

            #region DataLayer DI for PRM

            builder.Register(c => new ERP_BEPZAPRMEntities()).InstancePerHttpRequest();
            builder.RegisterType<PRM_UnitOfWork>().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(PRM_GenericRepository<>)).InstancePerHttpRequest();
            builder.RegisterType<PRM_ExecuteFunctions>().InstancePerHttpRequest();

            #endregion

            #region DataLayer DI for PIM

            //builder.Register(c => new IWM_MFSPIMEntities()).InstancePerHttpRequest();
            //builder.RegisterType<PIM_UnitOfWork>().InstancePerHttpRequest();
            //
            // builder.RegisterType<PIM_UnitOfWork>().As<IPIM_UnitOfWork>().InstancePerHttpRequest();


            //builder.RegisterGeneric(typeof(PIM_GenericRepository<>)).InstancePerHttpRequest();
            //builder.RegisterType<PIM_ExecuteFunctions>().InstancePerHttpRequest();

            #endregion

            #region DataLayer DI for FAM

            builder.Register(c => new ERP_BEPZAFAMEntities()).InstancePerHttpRequest();
            builder.RegisterType<FAM_UnitOfWork>().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(FAM_GenericRepository<>)).InstancePerHttpRequest();
            builder.RegisterType<FAM_ExecuteFunctions>().InstancePerHttpRequest();

            #endregion

            #region DataLayer DI for PGM
            builder.Register(c => new ERP_BEPZAPGMEntities()).InstancePerHttpRequest();
            builder.RegisterType<PGM_UnitOfWork>().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(PGM_GenericRepository<>)).InstancePerHttpRequest();
            builder.RegisterType<PGM_ExecuteFunctions>().InstancePerHttpRequest();
            #endregion

            #region DataLayer DI for INV

            builder.Register(c => new ERP_BEPZAINVEntities()).InstancePerHttpRequest();
            builder.RegisterType<INV_UnitOfWork>().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(INV_GenericRepository<>)).InstancePerHttpRequest();
            builder.RegisterType<INV_ExecuteFunctions>().InstancePerHttpRequest();

            #endregion

            #region DataLayer DI for PMI

            builder.Register(c => new ERP_BEPZAPMIEntities()).InstancePerHttpRequest();
            builder.RegisterType<PMI_UnitOfWork>().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(PMI_GenericRepository<>)).InstancePerHttpRequest();
            builder.RegisterType<PMI_ExecuteFunctions>().InstancePerHttpRequest();

            #endregion

            #region DataLayer DI for WFM

            builder.Register(c => new ERP_BEPZAWFMEntities()).InstancePerHttpRequest();
            builder.RegisterType<WFM_UnitOfWork>().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(WFM_GenericRepository<>)).InstancePerHttpRequest();
            builder.RegisterType<WFM_ExecuteFunctions>().InstancePerHttpRequest();

            #endregion

            #region DataLayer DI for FMS

            builder.Register(c => new ERP_BEPZAFMSEntities()).InstancePerHttpRequest();
            builder.RegisterType<FMS_UnitOfWork>().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(FMS_GenericRepository<>)).InstancePerHttpRequest();
            builder.RegisterType<FMS_ExecuteFunctions>().InstancePerHttpRequest();

            #endregion

            #region DataLayer DI for FAR

            builder.Register(c => new ERP_BEPZAFAREntities()).InstancePerHttpRequest();
            builder.RegisterType<FAR_UnitOfWork>().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(FAR_GenericRepository<>)).InstancePerHttpRequest();
            builder.RegisterType<FAR_ExecuteFunctions>().InstancePerHttpRequest();

            #endregion



            #region Service DI for PMI

            builder.RegisterType<PMICommonService>().InstancePerHttpRequest();

            #endregion

            #region Service DI for INV

            builder.RegisterType<INVCommonService>().InstancePerHttpRequest();

            #endregion

            #region Service DI for AMS

            builder.RegisterType<AMSCommonService>().InstancePerHttpRequest();

            #endregion

            #region Service DI for SMS

            builder.RegisterType<SMSCommonService>().InstancePerHttpRequest();

            #endregion

            #region Service DI for PRM

            builder.RegisterType<JobGradeService>().InstancePerHttpRequest();
            builder.RegisterType<SalaryStructureService>().InstancePerHttpRequest();
            builder.RegisterType<PRMCommonSevice>().InstancePerHttpRequest();
            builder.RegisterType<EmployeeService>().InstancePerHttpRequest();
            builder.RegisterType<PersonalInfoService>().InstancePerHttpRequest();
            builder.RegisterType<JobDesignationService>().InstancePerHttpRequest();
            builder.RegisterType<DivisionHeadMapingService>().InstancePerHttpRequest();
            builder.RegisterType<ResourceInfoService>().InstancePerHttpRequest();
            builder.RegisterType<ResourceCategoryService>().InstancePerHttpRequest();
            //builder.RegisterType<EmployeeTransferInfoService>().InstancePerHttpRequest();           
            builder.RegisterType<EmployeeConfirmIncrementPromotionService>().InstancePerHttpRequest();
            builder.RegisterType<EmployeeSeperationService>().InstancePerHttpRequest();

            builder.RegisterType<SalaryScaleService>().InstancePerRequest();


            #endregion

            #region Service DI for PIM

            //builder.RegisterType<PIMCommonService>().InstancePerHttpRequest();
            //builder.RegisterType<InvoicePlanSevice>().InstancePerHttpRequest();

            //builder.RegisterType<PIMBusinessRuleService>().InstancePerHttpRequest();

            #endregion

            #region Service DI for FAM

            builder.RegisterType<FAMCommonService>().InstancePerHttpRequest();
            builder.RegisterType<FAMFinancialYearService>().InstancePerHttpRequest();

            #endregion

            #region Service DI for PGM
            builder.RegisterType<PGMCommonService>().InstancePerHttpRequest();
            builder.RegisterType<PGMArrearAdjustmentService>().InstancePerHttpRequest();            
            #endregion

            #region Service DI for WFM

            builder.RegisterType<WFMCommonService>().InstancePerHttpRequest();

            #endregion

            #region Service DI for FMS

            builder.RegisterType<FMSCommonService>().InstancePerHttpRequest();

            #endregion

            #region Service DI for FMS

            builder.RegisterType<FARCommonService>().InstancePerHttpRequest();

            #endregion

            #region Datalayer DI for CPF

            builder.Register(c => new ERP_BEPZACPFEntities()).InstancePerHttpRequest();
            builder.RegisterType<CPF_UnitOfWork>().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(CPF_GenericRepository<>)).InstancePerHttpRequest();
            builder.RegisterType<CPF_ExecuteFunctions>().InstancePerHttpRequest();
            #endregion

            #region Service DI for CPF
            builder.RegisterType<CPFCommonService>().InstancePerHttpRequest();
            builder.RegisterType<ManagePeriodInformationService>().InstancePerHttpRequest();
            #endregion

            #region Datalayer DI for ADC
            //builder.Register(c => new IWM_MFSADCEntities()).InstancePerHttpRequest();
            //builder.RegisterType<ADC_UnitOfWork>().InstancePerHttpRequest();
            //builder.RegisterGeneric(typeof(ADC_GenericRepository<>)).InstancePerHttpRequest();
            //builder.RegisterType<ADC_ExecuteFunctions>().InstancePerHttpRequest();
            #endregion

            #region Service DI for ADC
            //builder.RegisterType<ADCCommonService>().InstancePerHttpRequest();
            #endregion

            #region Report

            builder.RegisterType<ReportBase>().InstancePerHttpRequest();

            #endregion

            ///**********Example: Dot not Delete **********************************
            //builder.RegisterType<PRM_UnitOfWork>().PropertiesAutowired().InstancePerHttpRequest();
            //builder.Register(x => new PRM_UnitOfWork(new PRM_MfsIwmEntities())).InstancePerHttpRequest();          
            //builder.RegisterType<PRM_GenericRepository<PRM_JobGrade>>().InstancePerHttpRequest();  
            //builder.RegisterGeneric(typeof(PRM_GenericRepository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();
            //builder.RegisterGeneric(typeof(DataRepository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();
            //// Automapper
            //builder.RegisterType<AutomapperStartupTask>().As<IStartupTask>().InstancePerHttpRequest();

            /////////////////////////////////////////////////////////////////////////


            //build container
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            #endregion

            #region AutoMapper

            var mapper = new AutomapperStartupTask();
            mapper.Execute();

            #endregion

            // Custom ModelMetadata for data annotation DisplayName
            ModelMetadataProviders.Current = new ConventionalModelMetadataProvider();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }


        #region All Enums

        public enum ContractType
        {
            Corporate = 1,
            Project = 2
        };

        public enum EnumDocumentApprovalInformation
        {
            Draft,
            Prepared,
            Submitted,
            Rejected,
            Approved,
            Reviewed,
            Recommended
        }

        public enum CPFMembershipStatus
        {
            Active,
            Inactive
        }

        public enum CPFApprovalStatus
        {
            Draft,
            Submitted,
            Reviewed,
            Approved,
            Rejected
        }

        public enum CPFTransactionType
        {
            Debit,
            Credit
        }

        public enum ADCBeneficiaryType
        {
            Employee,
            Department,
            Other
        }

        public enum PIMApprovalStatus
        {
            Draft = 1,
            Submitted = 2,
            Reviewed = 3,
            Recommended = 4,
            Approved = 5,
            Rejected = 6
        }

        public enum PIMEDLoginUser
        {
            EdLoginID = 1
        }

        public enum PIMActivityStatus
        {
            NotStartedYet = 1,
            InProgress = 2,
            Complete = 3
        }

        public enum PIMBudgetHeadType
        {
            Income,
            Expense
        }

        public enum ProjectCategory
        {
            Internal = 1,
            External = 2
        };

        #endregion
    }
}