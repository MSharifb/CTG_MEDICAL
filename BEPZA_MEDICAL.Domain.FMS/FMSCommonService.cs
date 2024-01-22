using BEPZA_MEDICAL.DAL.FMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.Domain.FMS
{
    public class FMSCommonService
    {
        #region Fields

        FMS_UnitOfWork _fmsUnit;

        #endregion

        #region Ctor
        public FMSCommonService(FMS_UnitOfWork unitOfWork)
        {
            _fmsUnit = unitOfWork;
        }
        #endregion
        
        #region Properties

        public FMS_UnitOfWork FMSUnit { get { return _fmsUnit; } }      

        #endregion
    }
}
