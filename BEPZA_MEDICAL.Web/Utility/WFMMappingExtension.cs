using AutoMapper;
using BEPZA_MEDICAL.DAL.WFM;
using BEPZA_MEDICAL.Web.Areas.WFM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Utility
{
    public static class WFMMappingExtension
    {
        public static WelfareFundCategoryViewModel ToModel(this WFM_WelfareFundCategory gradeStep)
        {
            return Mapper.Map<WFM_WelfareFundCategory, WelfareFundCategoryViewModel>(gradeStep);
        }
        public static WFM_WelfareFundCategory ToEntity(this WelfareFundCategoryViewModel gradeStepModel)
        {
            return Mapper.Map<WelfareFundCategoryViewModel, WFM_WelfareFundCategory>(gradeStepModel);
        }

        //Reason of  Fund Category
        public static ReasonOfWelfareCategoryViewModel ToModel(this WFM_ReasonOfWelfareCategory gradeStep)
        {
            return Mapper.Map<WFM_ReasonOfWelfareCategory, ReasonOfWelfareCategoryViewModel>(gradeStep);
        }
        public static WFM_ReasonOfWelfareCategory ToEntity(this ReasonOfWelfareCategoryViewModel gradeStepModel)
        {
            return Mapper.Map<ReasonOfWelfareCategoryViewModel, WFM_ReasonOfWelfareCategory>(gradeStepModel);
        }

        //Welfare Fund Policy

        public static WelfareFundPolicyViewModel ToModel(this WFM_WelfareFundPolicy gradeStep)
        {
            return Mapper.Map<WFM_WelfareFundPolicy, WelfareFundPolicyViewModel>(gradeStep);
        }
        public static WFM_WelfareFundPolicy ToEntity(this WelfareFundPolicyViewModel gradeStepModel)
        {
            return Mapper.Map<WelfareFundPolicyViewModel, WFM_WelfareFundPolicy>(gradeStepModel);
        }


        //Cycle Setup
        public static CycleViewModel ToModel(this WFM_CycleInfo gradeStep)
        {
            return Mapper.Map<WFM_CycleInfo, CycleViewModel>(gradeStep);
        }
        public static WFM_CycleInfo ToEntity(this CycleViewModel gradeStepModel)
        {
            return Mapper.Map<CycleViewModel, WFM_CycleInfo>(gradeStepModel);
        }
        //Online application 
        public static OnlineWelfareFundApplicationInformationViewModel ToModel(this WFM_OnlineApplicationInfo entity)
        {
            return Mapper.Map<WFM_OnlineApplicationInfo, OnlineWelfareFundApplicationInformationViewModel>(entity);
        }
        public static WFM_OnlineApplicationInfo ToEntity(this OnlineWelfareFundApplicationInformationViewModel model)
        {
            return Mapper.Map<OnlineWelfareFundApplicationInformationViewModel, WFM_OnlineApplicationInfo>(model);
        }

        //Online application Detail
        public static OnlineWelfareFundApplicationInformationDetailViewModel ToModel(this WFM_OnlineApplicationInfoDetailAttachment entity)
        {
            return Mapper.Map<WFM_OnlineApplicationInfoDetailAttachment, OnlineWelfareFundApplicationInformationDetailViewModel>(entity);
        }
        public static WFM_OnlineApplicationInfoDetailAttachment ToEntity(this OnlineWelfareFundApplicationInformationDetailViewModel model)
        {
            return Mapper.Map<OnlineWelfareFundApplicationInformationDetailViewModel, WFM_OnlineApplicationInfoDetailAttachment>(model);
        }

        //Offline application 
        public static OfflineWelfareFundApplicationInformationViewModel ToModel(this WFM_OfflineApplicationInfo entity)
        {
            return Mapper.Map<WFM_OfflineApplicationInfo, OfflineWelfareFundApplicationInformationViewModel>(entity);
        }
        public static WFM_OfflineApplicationInfo ToEntity(this OfflineWelfareFundApplicationInformationViewModel model)
        {
            return Mapper.Map<OfflineWelfareFundApplicationInformationViewModel, WFM_OfflineApplicationInfo>(model);
        }

        //Offline application Detail
        public static OfflineWelfareFundApplicationInformationDetailViewModel ToModel(this WFM_OfflineApplicationInfoDetailAttachment entity)
        {
            return Mapper.Map<WFM_OfflineApplicationInfoDetailAttachment, OfflineWelfareFundApplicationInformationDetailViewModel>(entity);
        }
        public static WFM_OfflineApplicationInfoDetailAttachment ToEntity(this OfflineWelfareFundApplicationInformationDetailViewModel model)
        {
            return Mapper.Map<OfflineWelfareFundApplicationInformationDetailViewModel, WFM_OfflineApplicationInfoDetailAttachment>(model);
        }

        // Approval of Welfare Fund Information

        public static ApprovalWelfareFundInfoViewModel ToModel(this WFM_ApprovalWelfareFundInfo entity)
        {
            return Mapper.Map<WFM_ApprovalWelfareFundInfo, ApprovalWelfareFundInfoViewModel>(entity);
        }
        public static WFM_ApprovalWelfareFundInfo ToEntity(this ApprovalWelfareFundInfoViewModel model)
        {
            return Mapper.Map<ApprovalWelfareFundInfoViewModel, WFM_ApprovalWelfareFundInfo>(model);
        }


        public static ApprovalWelfareFundInfoCommitteeViewModel ToModel(this WFM_ApprovalWelfareFundInfoCommittee entity)
        {
            return Mapper.Map<WFM_ApprovalWelfareFundInfoCommittee, ApprovalWelfareFundInfoCommitteeViewModel>(entity);
        }
        public static WFM_ApprovalWelfareFundInfoCommittee ToEntity(this ApprovalWelfareFundInfoCommitteeViewModel model)
        {
            return Mapper.Map<ApprovalWelfareFundInfoCommitteeViewModel, WFM_ApprovalWelfareFundInfoCommittee>(model);
        }


        public static ApprovalWelfareFundInfoEmployeeDetailsViewModel ToModel(this WFM_ApprovalWelfareFundInfoEmployeeDetails entity)
        {
            return Mapper.Map<WFM_ApprovalWelfareFundInfoEmployeeDetails, ApprovalWelfareFundInfoEmployeeDetailsViewModel>(entity);
        }
        public static WFM_ApprovalWelfareFundInfoEmployeeDetails ToEntity(this ApprovalWelfareFundInfoEmployeeDetailsViewModel model)
        {
            return Mapper.Map<ApprovalWelfareFundInfoEmployeeDetailsViewModel, WFM_ApprovalWelfareFundInfoEmployeeDetails>(model);
        }
      

        // Payment Order Info
        public static PaymentInfoViewModel ToModel(this WFM_PaymentInfo entity)
        {
            return Mapper.Map<WFM_PaymentInfo, PaymentInfoViewModel>(entity);
        }
        public static WFM_PaymentInfo ToEntity(this PaymentInfoViewModel model)
        {
            return Mapper.Map<PaymentInfoViewModel, WFM_PaymentInfo>(model);
        }

        public static PaymentInfoEmployeeDetailsViewDetail ToModel(this WFM_PaymentInfoEmployeeDetails entity)
        {
            return Mapper.Map<WFM_PaymentInfoEmployeeDetails, PaymentInfoEmployeeDetailsViewDetail>(entity);
        }
        public static WFM_PaymentInfoEmployeeDetails ToEntity(this PaymentInfoEmployeeDetailsViewDetail model)
        {
            return Mapper.Map<PaymentInfoEmployeeDetailsViewDetail, WFM_PaymentInfoEmployeeDetails>(model);
        }

        // Verify the Application Info
        public static VerifytheApplicationInfoViewModel ToModel(this WFM_VerifyTheApplication entity)
        {
            return Mapper.Map<WFM_VerifyTheApplication, VerifytheApplicationInfoViewModel>(entity);
        }
        public static WFM_VerifyTheApplication ToEntity(this VerifytheApplicationInfoViewModel model)
        {
            return Mapper.Map<VerifytheApplicationInfoViewModel, WFM_VerifyTheApplication>(model);
        }

        // Verify the Application Info Detail
        public static VerifytheApplicationInfoDetailViewModel ToModel(this WFM_VerifyTheApplicationDetails entity)
        {
            return Mapper.Map<WFM_VerifyTheApplicationDetails, VerifytheApplicationInfoDetailViewModel>(entity);
        }
        public static WFM_VerifyTheApplicationDetails ToEntity(this VerifytheApplicationInfoDetailViewModel model)
        {
            return Mapper.Map<VerifytheApplicationInfoDetailViewModel, WFM_VerifyTheApplicationDetails>(model);
        }

    }
}