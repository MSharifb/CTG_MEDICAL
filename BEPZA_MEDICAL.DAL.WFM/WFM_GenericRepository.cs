using BEPZA_MEDICAL.DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.DAL.WFM
{
    public class WFM_GenericRepository<T> : DataEF6Repository<T> where T : class
    {
        public WFM_GenericRepository(ERP_BEPZAWFMEntities context) : base(context)
        {
            //constructor
        }

    }
}
