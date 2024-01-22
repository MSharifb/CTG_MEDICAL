using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.DAL.FMS
{
    public class FMS_UnitOfWork
    {
        #region Fields
        FMS_ExecuteFunctions _functionRepository;
        FMS_GenericRepository<FMS_FixedDepositTypeInfo> _fixedDepositTypeInfoRepository;
        FMS_GenericRepository<FMS_BankInfo> _bankInfoRepository;
        FMS_GenericRepository<FMS_BankInfoBranchDetail> _bankInfoBranchDetailRepository;
        FMS_GenericRepository<FMS_FDRInstallmentInfo> _fDRInstallmentInfoRepository;
        FMS_GenericRepository<FMS_FixedDepositInfo> _fixedDepositInfoRepository;       
        FMS_GenericRepository<FMS_FixedDepositInfoInstallmentSchedule> _fixedDepositInfoInstallmentScheduleRepository;
        FMS_GenericRepository<FMS_FDRClosingInfo> _fDRClosingInfoRepository;
        FMS_GenericRepository<FMS_FDRType> _fDRTypeRepository;
        FMS_GenericRepository<FMS_SourceofFund> _sourceofFundRepository;
        FMS_GenericRepository<FMS_BankWiseOfferRate> _bankWiseOfferRateRepository;
        FMS_GenericRepository<acc_Cost_Centre_or_Institute_Information> _subLedgerRepository;
        FMS_GenericRepository<FMS_CPFInterestReceivable> _cpfInterestReceivableRepository;

        #endregion

        #region Ctor
        public FMS_UnitOfWork(
            FMS_ExecuteFunctions functionRepository,
            FMS_GenericRepository<FMS_FixedDepositTypeInfo> fixedDepositTypeInfoRepository,
            FMS_GenericRepository<FMS_BankInfo> bankInfoRepository,
            FMS_GenericRepository<FMS_BankInfoBranchDetail> bankInfoBranchDetailRepository,
            FMS_GenericRepository<FMS_FDRInstallmentInfo> fDRInstallmentInfoRepository,
            FMS_GenericRepository<FMS_FixedDepositInfo> fixedDepositInfoRepository,          
            FMS_GenericRepository<FMS_FixedDepositInfoInstallmentSchedule> fixedDepositInfoInstallmentScheduleRepository,
            FMS_GenericRepository<FMS_FDRClosingInfo> fDRClosingInfoRepository,
            FMS_GenericRepository<FMS_FDRType> fDRTypeRepository,
            FMS_GenericRepository<FMS_SourceofFund> sourceofFundRepository,
            FMS_GenericRepository<FMS_BankWiseOfferRate> bankWiseOfferRateRepository,
            FMS_GenericRepository<acc_Cost_Centre_or_Institute_Information> subLedgerRepository,
            FMS_GenericRepository<FMS_CPFInterestReceivable> cpfInterestReceivableRepository
            )
        {
            this._functionRepository = functionRepository;
            this._fixedDepositTypeInfoRepository = fixedDepositTypeInfoRepository;
            this._bankInfoRepository = bankInfoRepository;
            this._bankInfoBranchDetailRepository = bankInfoBranchDetailRepository;
            this._fDRInstallmentInfoRepository = fDRInstallmentInfoRepository;
            this._fixedDepositInfoRepository = fixedDepositInfoRepository;           
            this._fixedDepositInfoInstallmentScheduleRepository = fixedDepositInfoInstallmentScheduleRepository;
            this._fDRClosingInfoRepository = fDRClosingInfoRepository;
            this._fDRTypeRepository = fDRTypeRepository;
            this._sourceofFundRepository = sourceofFundRepository;
            this._bankWiseOfferRateRepository = bankWiseOfferRateRepository;
            this._subLedgerRepository = subLedgerRepository;
            this._cpfInterestReceivableRepository = cpfInterestReceivableRepository;
        }
        #endregion

        #region Properties

        public FMS_ExecuteFunctions FunctionRepository
        {
            get
            {
                return _functionRepository;
            }
        }
        public FMS_GenericRepository<FMS_FixedDepositTypeInfo> FixedDepositTypeInfoRepository
        {
            get
            {
                return _fixedDepositTypeInfoRepository;
            }
        }
        public FMS_GenericRepository<FMS_BankInfo> BankInfoRepository
        {
            get
            {
                return _bankInfoRepository;
            }
        }
        public FMS_GenericRepository<FMS_BankInfoBranchDetail> BankInfoBranchDetailRepository
        {
            get
            {
                return _bankInfoBranchDetailRepository;
            }
        }

        public FMS_GenericRepository<FMS_FDRInstallmentInfo> FDRInstallmentInfoRepository
        {
            get
            {
                return _fDRInstallmentInfoRepository;
            }
        }
        public FMS_GenericRepository<FMS_FixedDepositInfo> FixedDepositInfoRepository
        {
            get
            {
                return _fixedDepositInfoRepository;
            }
        }       
        public FMS_GenericRepository<FMS_FixedDepositInfoInstallmentSchedule> FixedDepositInfoInstallmentScheduleRepository
        {
            get
            {
                return _fixedDepositInfoInstallmentScheduleRepository;
            }
        }

        public FMS_GenericRepository<FMS_FDRClosingInfo> FDRClosingInfoRepository
        {
            get
            {
                return _fDRClosingInfoRepository;
            }
        }
        public FMS_GenericRepository<FMS_FDRType> FDRTypeRepository
        {
            get
            {
                return _fDRTypeRepository;
            }
        }
        public FMS_GenericRepository<FMS_SourceofFund> SourceofFundRepository
        {
            get
            {
                return _sourceofFundRepository;
            }
        }
        public FMS_GenericRepository<FMS_BankWiseOfferRate> BankWiseOfferRateRepository
        {
            get
            {
                return _bankWiseOfferRateRepository;
            }
        }
        public FMS_GenericRepository<acc_Cost_Centre_or_Institute_Information> SubLedgerRepository
        {
            get
            {
                return _subLedgerRepository;
            }
        }
        public FMS_GenericRepository<FMS_CPFInterestReceivable> CPFInterestReceivableRepository
        {
            get
            {
                return _cpfInterestReceivableRepository;
            }
        }

        #endregion

    }
}
