using BEPZA_MEDICAL.DAL.INV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.Domain.INV
{
    public class INVCommonService
    {

        #region Fields

        INV_UnitOfWork _invUnit;

        #endregion

        #region Ctor
        public INVCommonService(INV_UnitOfWork unitOfWork)
        {
            _invUnit = unitOfWork;
        }
        #endregion
        
        #region Properties

        public INV_UnitOfWork INVUnit { get { return _invUnit; } }      

        #endregion
    }


}
