using BEPZA_MEDICAL.DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.DAL.PMI
{
    public class PMI_GenericRepository<T> : DataEF6Repository<T> where T : class
    {
        public PMI_GenericRepository(ERP_BEPZAPMIEntities context)
            : base(context)
        {
            //constructor
        }
    }
}
