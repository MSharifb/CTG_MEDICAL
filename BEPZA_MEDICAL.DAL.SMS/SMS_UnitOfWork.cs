using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BEPZA_MEDICAL.DAL.SMS
{
    public class SMS_UnitOfWork
    {
        #region Fields

        SMS_ExecuteFunctions _functionRepository;
        //SMS_GenericRepository<SMS_SecurityInfo> _securityInfoRepository;

        #endregion

        #region Constactor

        public SMS_UnitOfWork(
            SMS_ExecuteFunctions functionRepository
            //SMS_GenericRepository<SMS_SecurityInfo> securityInfoRepository
            
            )
        {
            this._functionRepository = functionRepository;
            //this._securityInfoRepository = securityInfoRepository;
        }

        #endregion

        #region Properties

        public SMS_ExecuteFunctions FunctionRepository
        {
            get
            {
                return _functionRepository;
            }
        }

        //public SMS_GenericRepository<SMS_SecurityInfo> SecurityInfoRepository
        //{
        //    get
        //    {
        //        return _securityInfoRepository;
        //    }
        //}
        
        #endregion
    }


}