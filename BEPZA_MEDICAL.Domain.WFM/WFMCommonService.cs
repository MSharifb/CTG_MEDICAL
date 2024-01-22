using BEPZA_MEDICAL.DAL.WFM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.Domain.WFM
{
    public class WFMCommonService
    {
        #region Fields

        WFM_UnitOfWork _wfmUnit;

        #endregion

        #region Ctor
        public WFMCommonService(WFM_UnitOfWork unitOfWork)
        {
            _wfmUnit = unitOfWork;
        }
        #endregion
        
        #region Properties

        public WFM_UnitOfWork WFMUnit { get { return _wfmUnit; } }      

        #endregion

    }
}
