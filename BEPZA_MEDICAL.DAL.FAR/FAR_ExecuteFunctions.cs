using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.DAL.FAR
{
    public class FAR_ExecuteFunctions
    {
        #region Fields
        private readonly ERP_BEPZAFAREntities _context;
        #endregion

        #region Ctor

        public FAR_ExecuteFunctions(ERP_BEPZAFAREntities context)
        {
            this._context = context;
        }

        #endregion

        #region Funtions

        public int DepreciationProcess(int ZoneInfoId, int financialYearId, DateTime ProcessDate, string Remarks, string user)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.SP_FAR_ProcDepreciationCal(ZoneInfoId, financialYearId, ProcessDate, Remarks, user, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public int DepreciationRollbackProcess(int ZoneInfoId, int id, DateTime ProcessDate, string Remarks, string user)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.SP_FAR_ProcDepreciationCalRollback(ZoneInfoId,id, ProcessDate, Remarks, user, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public int DepreciationRollbackIndividualProcess(int ZoneInfoId, int DepreciationId, int DetailId, string user)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.SP_FAR_ProcDepreciationCalRollbackIndividual(ZoneInfoId,DepreciationId, DetailId, user, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }


        public SP_FAR_GetNextFinancialYear_Result getFinancialYear(int id)
        {
            return _context.SP_FAR_GetNextFinancialYear(id).FirstOrDefault();
        }

        public IList<SP_FAR_GetUniqueMRRList_Result> getUniqueMRRList(int zoneId)
        {
            return _context.SP_FAR_GetUniqueMRRList(zoneId).ToList();
        }

        #endregion
    }
}
