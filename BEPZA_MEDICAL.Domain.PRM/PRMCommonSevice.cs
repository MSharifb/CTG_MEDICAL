using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPZA_MEDICAL.DAL.PRM;

namespace BEPZA_MEDICAL.Domain.PRM
{
    public class PRMCommonSevice
    {      
        #region Fields

         PRM_UnitOfWork _prmUnit { get; set; }      
     
        #endregion

        #region Ctor

        public PRMCommonSevice(PRM_UnitOfWork uow)
        {
            _prmUnit =uow;
        }      
        
        #endregion

        public PRM_UnitOfWork PRMUnit { get { return _prmUnit; } }
     
    }
}
