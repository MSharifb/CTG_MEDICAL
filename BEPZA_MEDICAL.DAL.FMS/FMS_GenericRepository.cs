using BEPZA_MEDICAL.DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.DAL.FMS
{
    public class FMS_GenericRepository<T> : DataEF6Repository<T> where T : class
    {
        public FMS_GenericRepository(ERP_BEPZAFMSEntities context)
            : base(context)
        {
            //constructor
        }
    }
}
