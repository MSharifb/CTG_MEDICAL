using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects;
using BEPZA_MEDICAL.DAL.Infrastructure;


namespace BEPZA_MEDICAL.DAL.PRM
{
    public class PRM_GenericRepository<T> : DataRepository<T> where T : class
    {
        public PRM_GenericRepository(ERP_BEPZAPRMEntities context)
            : base(context)
        {
            //constructor
        }
    }
}