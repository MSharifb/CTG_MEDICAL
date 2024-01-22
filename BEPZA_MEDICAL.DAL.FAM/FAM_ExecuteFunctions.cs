using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using BEPZA_MEDICAL.DAL.FAM;

namespace BEPZA_MEDICAL.DAL.FAM
{
    public class FAM_ExecuteFunctions
    {
        #region Fields
        private readonly ERP_BEPZAFAMEntities _context;
        #endregion

        #region Ctor

        public FAM_ExecuteFunctions(ERP_BEPZAFAMEntities context)
        {
            this._context = context;
        }

        #endregion

        #region Funtions
        public IList<CommonConfigGet> GetAllCommonConfig(string displayName,
                                                                int id,
                                                                string name,
                                                                string sortBy,
                                                                string sortType,
                                                                int rowIndex,
                                                                int maxRows,
                                                                out int totalRows
            )
        {
            var numTotalRows = new ObjectParameter("numTotalRows", typeof(int));
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var query = _context.CommonConfigGet(displayName, id, name, sortBy, sortType, rowIndex, maxRows, numTotalRows, numErrorCode, strErrorMsg);

            totalRows = Convert.ToInt32(numTotalRows.Value);

            return query.ToList();
        }

        public int CommonConfigTypeDML(string displayName, int id, string name, int sortOrder, string remarks, string user, string mode)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var query = _context.CommonConfigTypeDML(displayName, id, name, sortOrder, remarks, user, mode, numErrorCode, strErrorMsg).FirstOrDefault();

            return (int)query;
        }

        //public int TrialBalanceReport(DateTime voucherDate, int companyId, int displayLevel)
        //{
        //    var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
        //    var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

        //    var query = _context.uspFAMTrialBalanceDetailedALLChartOfAccount(voucherDate, companyId, displayLevel);

        //    return (int)query;
        //}

        public int BalanceSheetReport(int companyId,string periodCode ,DateTime voucherDate, string displayMode )
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var query = _context.uspFAMBalanceSheetbyControlHeadComparision(companyId,periodCode,voucherDate,displayMode);

            return (int)query;
        }

        public  string[] CurrentBalanceOfAccountHead(int companyId, int myAccountHeadId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            string[] query = _context.uspFAMCurrentBalanceOfAccountHead(companyId, myAccountHeadId).ToArray<string>();

            return query;
        }

        #endregion
    }
}
