using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.DAL.WFM
{
    public class WFM_UnitOfWork
    {
        #region Fields
        WFM_ExecuteFunctions _functionRepository;
        WFM_GenericRepository<WFM_WelfareFundCategory> _welfareFundCategoryRepository;
        WFM_GenericRepository<WFM_OnlineApplicationInfo> _onlineApplicationInfoRepository;
        WFM_GenericRepository<WFM_OnlineApplicationInfoDetailAttachment> _onlineApplicationInfoDetailAttachmentRepository;
        WFM_GenericRepository<WFM_OfflineApplicationInfo> _offlineApplicationInfoRepository;
        WFM_GenericRepository<WFM_OfflineApplicationInfoDetailAttachment> _offlineApplicationInfoDetailAttachmentRepository;
        WFM_GenericRepository<WFM_ReasonOfWelfareCategory> _reasonOfWelfareCategoryRepository;
        WFM_GenericRepository<WFM_WelfareFundPolicy> _welfareFundPolicyRepository;
        WFM_GenericRepository<WFM_CycleInfo> _cycleRepository;
        WFM_GenericRepository<WFM_ApprovalWelfareFundInfo> _approvalWelfareFundInfoRepository;
        WFM_GenericRepository<WFM_ApprovalWelfareFundInfoCommittee> _approvalWelfareFundInfoCommitteeRepository;
        WFM_GenericRepository<WFM_ApprovalWelfareFundInfoEmployeeDetails> _approvalWelfareFundInfoEmployeeDetailsRepository;
        WFM_GenericRepository<WFM_PaymentInfo> _paymentInfoRepository;
        WFM_GenericRepository<WFM_PaymentInfoEmployeeDetails> _paymentInfoEmployeeDetailsRepository;
        WFM_GenericRepository<WFM_VerifyTheApplication> _verifyTheApplicationRepository;
        WFM_GenericRepository<WFM_VerifyTheApplicationDetails> _verifyTheApplicationDetailsRepository;
        WFM_GenericRepository<vwWfmApplicationInformation> _vwApplicationRepository;
        //WFM_GenericRepository<acc_ChartofAccounts> _chartofAccountsRepository;

        #endregion

        #region Ctor
        public WFM_UnitOfWork(
            WFM_ExecuteFunctions functionRepository,
            WFM_GenericRepository<WFM_WelfareFundCategory> welfareFundCategoryRepository,
            WFM_GenericRepository<WFM_OnlineApplicationInfo> onlineApplicationInfoRepository,
            WFM_GenericRepository<WFM_OnlineApplicationInfoDetailAttachment> onlineApplicationInfoDetailAttachmentRepository,
            WFM_GenericRepository<WFM_OfflineApplicationInfo> offlineApplicationInfoRepository,
            WFM_GenericRepository<WFM_OfflineApplicationInfoDetailAttachment> offlineApplicationInfoDetailAttachmentRepository,
            WFM_GenericRepository<WFM_ReasonOfWelfareCategory> reasonOfWelfareCategoryRepository,
            WFM_GenericRepository<WFM_WelfareFundPolicy> welfareFundPolicyRepository,
            WFM_GenericRepository<WFM_CycleInfo> cycleRepository,
            WFM_GenericRepository<WFM_ApprovalWelfareFundInfo> approvalWelfareFundInfoRepository,
            WFM_GenericRepository<WFM_ApprovalWelfareFundInfoCommittee> approvalWelfareFundInfoCommitteeRepository,
            WFM_GenericRepository<WFM_ApprovalWelfareFundInfoEmployeeDetails> approvalWelfareFundInfoEmployeeDetailsRepository,
            WFM_GenericRepository<WFM_PaymentInfo> paymentInfoRepository,
            WFM_GenericRepository<WFM_PaymentInfoEmployeeDetails> paymentInfoEmployeeDetailsRepository,
            WFM_GenericRepository<WFM_VerifyTheApplication> verifyTheApplicationRepository,
            WFM_GenericRepository<WFM_VerifyTheApplicationDetails> verifyTheApplicationDetailsRepository,
            WFM_GenericRepository<vwWfmApplicationInformation> vwApplicationRepository
            // WFM_GenericRepository<acc_ChartofAccounts> chartofAccountsRepository

            )
        {
            this._functionRepository = functionRepository;
            this._welfareFundCategoryRepository = welfareFundCategoryRepository;
            this._onlineApplicationInfoRepository = onlineApplicationInfoRepository;
            this._onlineApplicationInfoDetailAttachmentRepository = onlineApplicationInfoDetailAttachmentRepository;
            this._offlineApplicationInfoRepository = offlineApplicationInfoRepository;
            this._offlineApplicationInfoDetailAttachmentRepository = offlineApplicationInfoDetailAttachmentRepository;
            this._reasonOfWelfareCategoryRepository = reasonOfWelfareCategoryRepository;
            this._welfareFundPolicyRepository = welfareFundPolicyRepository;
            this._cycleRepository = cycleRepository;
            this._approvalWelfareFundInfoRepository = approvalWelfareFundInfoRepository;
            this._approvalWelfareFundInfoCommitteeRepository = approvalWelfareFundInfoCommitteeRepository;
            this._approvalWelfareFundInfoEmployeeDetailsRepository = approvalWelfareFundInfoEmployeeDetailsRepository;
            this._paymentInfoRepository = paymentInfoRepository;
            this._paymentInfoEmployeeDetailsRepository = paymentInfoEmployeeDetailsRepository;
            this._verifyTheApplicationRepository = verifyTheApplicationRepository;
            this._verifyTheApplicationDetailsRepository = verifyTheApplicationDetailsRepository;
            this._vwApplicationRepository = vwApplicationRepository;
            // this._chartofAccountsRepository = chartofAccountsRepository;

        }
        #endregion

        #region Properties

        public WFM_ExecuteFunctions FunctionRepository
        {
            get
            {
                return _functionRepository;
            }
        }
        public WFM_GenericRepository<WFM_WelfareFundCategory> WelfareFundCategoryRepository
        {
            get
            {
                return _welfareFundCategoryRepository;
            }
        }
        public WFM_GenericRepository<WFM_OnlineApplicationInfo> OnlineApplicationInfoRepository
        {
            get
            {
                return _onlineApplicationInfoRepository;
            }
        }
        public WFM_GenericRepository<WFM_OnlineApplicationInfoDetailAttachment> OnlineApplicationInfoDetailAttachmentRepository
        {
            get
            {
                return _onlineApplicationInfoDetailAttachmentRepository;
            }
        }
        public WFM_GenericRepository<WFM_OfflineApplicationInfo> OfflineApplicationInfoRepository
        {
            get
            {
                return _offlineApplicationInfoRepository;
            }
        }
        public WFM_GenericRepository<WFM_OfflineApplicationInfoDetailAttachment> OfflineApplicationInfoDetailAttachmentRepository
        {
            get
            {
                return _offlineApplicationInfoDetailAttachmentRepository;
            }
        }

        public WFM_GenericRepository<WFM_ReasonOfWelfareCategory> ReasonOfWelfareCategoryRepository
        {
            get
            {
                return _reasonOfWelfareCategoryRepository;
            }
        }

        public WFM_GenericRepository<WFM_WelfareFundPolicy> WelfareFundPolicyRepository
        {
            get
            {
                return _welfareFundPolicyRepository;
            }
        }

        public WFM_GenericRepository<WFM_CycleInfo> CycleRepository
        {
            get
            {
                return _cycleRepository;
            }
        }


        public WFM_GenericRepository<WFM_ApprovalWelfareFundInfo> ApprovalWelfareFundInfoRepository
        {
            get
            {
                return _approvalWelfareFundInfoRepository;
            }
        }

        public WFM_GenericRepository<WFM_ApprovalWelfareFundInfoCommittee> ApprovalWelfareFundInfoCommitteeRepository
        {
            get
            {
                return _approvalWelfareFundInfoCommitteeRepository;
            }
        }

        public WFM_GenericRepository<WFM_ApprovalWelfareFundInfoEmployeeDetails> ApprovalWelfareFundInfoEmployeeDetailsRepository
        {
            get
            {
                return _approvalWelfareFundInfoEmployeeDetailsRepository;
            }
        }
        public WFM_GenericRepository<WFM_PaymentInfo> PaymentInfoRepository
        {
            get
            {
                return _paymentInfoRepository;
            }
        }

        public WFM_GenericRepository<WFM_PaymentInfoEmployeeDetails> PaymentInfoEmployeeDetailsRepository
        {
            get
            {
                return _paymentInfoEmployeeDetailsRepository;
            }
        }
        public WFM_GenericRepository<WFM_VerifyTheApplication> VerifyTheApplicationRepository
        {
            get
            {
                return _verifyTheApplicationRepository;
            }
        }
        public WFM_GenericRepository<WFM_VerifyTheApplicationDetails> VerifyTheApplicationDetailsRepository
        {
            get
            {
                return _verifyTheApplicationDetailsRepository;
            }
        }

        public WFM_GenericRepository<vwWfmApplicationInformation> VwApplicationRepository
        {
            get
            {
                return _vwApplicationRepository;
            }
        }

        //public WFM_GenericRepository<acc_ChartofAccounts> ChartofAccountsRepository
        //{
        //    get
        //    {
        //        return _chartofAccountsRepository;
        //    }
        //}

        #endregion
    }
}
