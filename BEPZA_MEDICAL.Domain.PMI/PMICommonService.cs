using BEPZA_MEDICAL.DAL.PMI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.Domain.PMI
{
    public class PMICommonService
    {
        #region Fields

        PMI_UnitOfWork _invUnit;

        #endregion

        #region Ctor
        public PMICommonService(PMI_UnitOfWork unitOfWork)
        {
            _invUnit = unitOfWork;
        }
        #endregion

        #region Properties

        public PMI_UnitOfWork PMIUnit { get { return _invUnit; } }

        #endregion
    }
}
