using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;

using BEPZA_MEDICAL.DAL.AMS;


namespace BEPZA_MEDICAL.DAL.AMS
{
    public class AMS_ExecuteFunctions
    {
        #region Fields
        private readonly ERP_BEPZA_AMSEntities _context;
        #endregion

        #region Ctor

        public AMS_ExecuteFunctions(ERP_BEPZA_AMSEntities context)
        {
            this._context = context;
        }

        #endregion

        #region Funtions
        public string fnGetServiceDuration(DateTime startDate, DateTime endDate)
        {
            return _context.sp_AMS_GetServiceDuration(startDate, endDate).FirstOrDefault().Duration;
        }
        

        #endregion
    }
}
