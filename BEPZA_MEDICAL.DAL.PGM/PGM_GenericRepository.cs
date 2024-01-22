using BEPZA_MEDICAL.DAL.Infrastructure;

namespace BEPZA_MEDICAL.DAL.PGM
{
    public class PGM_GenericRepository<T> : DataRepository<T> where T : class
    {
        public PGM_GenericRepository(ERP_BEPZAPGMEntities context)
            : base(context)
        {
            //constructor
        }
    }
}
