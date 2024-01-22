using AutoMapper;
using BEPZA_MEDICAL.DAL.WFM;
using BEPZA_MEDICAL.Web.Areas.WFM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class WFMMapper
    {
        public WFMMapper()
        {
            //Welfare Fund Category
            Mapper.CreateMap<WelfareFundCategoryViewModel, WFM_WelfareFundCategory>();
            Mapper.CreateMap<WFM_WelfareFundCategory, WelfareFundCategoryViewModel>();

            //Reason of  Fund Category
            Mapper.CreateMap<ReasonOfWelfareCategoryViewModel, WFM_ReasonOfWelfareCategory>();
            Mapper.CreateMap<WFM_ReasonOfWelfareCategory, ReasonOfWelfareCategoryViewModel>();

            //Welfare Fund Policy
            Mapper.CreateMap<WelfareFundPolicyViewModel, WFM_WelfareFundPolicy>();
            Mapper.CreateMap<WFM_WelfareFundPolicy, WelfareFundPolicyViewModel>();

            //Cycle Setup
            Mapper.CreateMap<CycleViewModel, WFM_CycleInfo>();
            Mapper.CreateMap<WFM_CycleInfo, CycleViewModel>();

            // Online Application
            Mapper.CreateMap<OnlineWelfareFundApplicationInformationViewModel, WFM_OnlineApplicationInfo>();
            Mapper.CreateMap<WFM_OnlineApplicationInfo, OnlineWelfareFundApplicationInformationViewModel>();

            Mapper.CreateMap<OnlineWelfareFundApplicationInformationDetailViewModel, WFM_OnlineApplicationInfoDetailAttachment>();
            Mapper.CreateMap<WFM_OnlineApplicationInfoDetailAttachment, OnlineWelfareFundApplicationInformationDetailViewModel>();

            // Offline Application
            Mapper.CreateMap<OfflineWelfareFundApplicationInformationViewModel, WFM_OfflineApplicationInfo>();
            Mapper.CreateMap<WFM_OfflineApplicationInfo, OfflineWelfareFundApplicationInformationViewModel>();

            Mapper.CreateMap<OfflineWelfareFundApplicationInformationDetailViewModel, WFM_OfflineApplicationInfoDetailAttachment>();
            Mapper.CreateMap<WFM_OfflineApplicationInfoDetailAttachment, OfflineWelfareFundApplicationInformationDetailViewModel>();

            // Approval of Welfare Fund Information
            Mapper.CreateMap<ApprovalWelfareFundInfoViewModel, WFM_ApprovalWelfareFundInfo>();
            Mapper.CreateMap<WFM_ApprovalWelfareFundInfo, ApprovalWelfareFundInfoViewModel>();

            Mapper.CreateMap<ApprovalWelfareFundInfoCommitteeViewModel, WFM_ApprovalWelfareFundInfoCommittee>();
            Mapper.CreateMap<WFM_ApprovalWelfareFundInfoCommittee, ApprovalWelfareFundInfoCommitteeViewModel>();

            Mapper.CreateMap<ApprovalWelfareFundInfoEmployeeDetailsViewModel, WFM_ApprovalWelfareFundInfoEmployeeDetails>();
            Mapper.CreateMap<WFM_ApprovalWelfareFundInfoEmployeeDetails, ApprovalWelfareFundInfoEmployeeDetailsViewModel>();
           
            // Payment Order Info
            Mapper.CreateMap<PaymentInfoViewModel, WFM_PaymentInfo>();
            Mapper.CreateMap<WFM_PaymentInfo, PaymentInfoViewModel>();

            Mapper.CreateMap<PaymentInfoEmployeeDetailsViewDetail, WFM_PaymentInfoEmployeeDetails>();
            Mapper.CreateMap<WFM_PaymentInfoEmployeeDetails, PaymentInfoEmployeeDetailsViewDetail>();

            // Verify the Application Info
            Mapper.CreateMap<VerifytheApplicationInfoViewModel, WFM_VerifyTheApplication>();
            Mapper.CreateMap<WFM_VerifyTheApplication, VerifytheApplicationInfoViewModel>();

            Mapper.CreateMap<VerifytheApplicationInfoDetailViewModel, WFM_VerifyTheApplicationDetails>();
            Mapper.CreateMap<WFM_VerifyTheApplicationDetails, VerifytheApplicationInfoDetailViewModel>();

        }
    }
}