using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.DAL.FMS
{
    public class FMS_ExecuteFunctions
    {
        #region Fields
        private readonly ERP_BEPZAFMSEntities _context;
        #endregion

        #region Ctor

        public FMS_ExecuteFunctions(ERP_BEPZAFMSEntities context)
        {
            this._context = context;
        }

        #endregion

        #region Funtions
        public IList<SP_FMS_getBankAccount_Result> fnGetCOABudgetHeadList(int zoneId)
        {
            return _context.SP_FMS_getBankAccount(zoneId).ToList();
        }

        public IList<SP_FMS_getBankChequeNo_Result> fnGetBankChequeNoList(int zoneId, int? coaId)
        {
            return _context.SP_FMS_getBankChequeNo(zoneId,coaId).ToList();
        }

       
        #endregion
    }
}
