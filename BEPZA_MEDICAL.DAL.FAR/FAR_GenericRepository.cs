using BEPZA_MEDICAL.DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.DAL.FAR
{
    public class FAR_GenericRepository<T> : DataEF6Repository<T> where T : class
    {
        public FAR_GenericRepository(ERP_BEPZAFAREntities context)
            : base(context)
        {
            //constructor
        }
    }
}
