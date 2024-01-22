using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BEPZA_MEDICAL.DAL.Infrastructure;
using BEPZA_MEDICAL.DAL.CPF;

namespace BEPZA_MEDICAL.DAL.CPF
{
    public class CPF_GenericRepository<T> : DataEF6Repository<T> where T : class

    {
        public CPF_GenericRepository(ERP_BEPZACPFEntities context): base(context)
        {
            //constructor
        }

    }
}
