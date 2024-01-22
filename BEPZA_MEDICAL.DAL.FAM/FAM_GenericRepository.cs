using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects;
using BEPZA_MEDICAL.DAL.Infrastructure;
using BEPZA_MEDICAL.DAL.FAM;


namespace BEPZA_MEDICAL.DAL.FAM
{
    public class FAM_GenericRepository<T> : DataRepository<T> where T : class
    {
        public FAM_GenericRepository(ERP_BEPZAFAMEntities context): base(context)
        {
            //constructor
        }
    }
}