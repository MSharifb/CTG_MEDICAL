using BEPZA_MEDICAL.DAL.FAR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.Domain.FAR
{
    public class FARCommonService
    {
        #region Fields

        FAR_UnitOfWork _farUnit;

        #endregion

        #region Ctor
        public FARCommonService(FAR_UnitOfWork unitOfWork)
        {
            _farUnit = unitOfWork;
        }
        #endregion
        
        #region Properties

        public FAR_UnitOfWork FARUnit { get { return _farUnit; } }      

        #endregion
    }
}
