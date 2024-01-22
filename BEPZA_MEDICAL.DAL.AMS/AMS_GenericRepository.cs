using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BEPZA_MEDICAL.DAL.Infrastructure;
using BEPZA_MEDICAL.DAL.AMS;


namespace BEPZA_MEDICAL.DAL.AMS
{
    public class AMS_GenericRepository<T> : DataEF6Repository<T> where T : class
    {
        public AMS_GenericRepository(ERP_BEPZA_AMSEntities context)
            : base(context)
        {
            //constructor
        }
    }
}