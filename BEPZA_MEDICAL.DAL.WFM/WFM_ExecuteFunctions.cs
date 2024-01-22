using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.DAL.WFM
{
    public class WFM_ExecuteFunctions
    {
        #region Fields
        private readonly ERP_BEPZAWFMEntities _context;
        #endregion

        #region Ctor

        public WFM_ExecuteFunctions(ERP_BEPZAWFMEntities context)
        {
            this._context = context;
        }

        #endregion

        #region Funtions

        public IList<SP_WFM_getCoABudgetHeatList_Result> fnGetCOABudgetHeadList()
        {
            return _context.SP_WFM_getCoABudgetHeatList().ToList();
        }

        public IList<SP_WFM_getCoABudgetHeadAmount_Result> fnGetCOABudgetHeadAmountList(int? coaId, int zoneInfoId)
        {
            return _context.SP_WFM_getCoABudgetHeadAmount(coaId, zoneInfoId).ToList();
        }  

        #endregion

    }
}
