using BEPZA_MEDICAL.DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPZA_MEDICAL.DAL.Infrastructure;

namespace BEPZA_MEDICAL.DAL.INV
{
    public class INV_GenericRepository<T> : DataEF6Repository<T> where T : class
    {
        public INV_GenericRepository(ERP_BEPZAINVEntities context)
            : base(context)
        {
            //constructor
        }
    }


}
