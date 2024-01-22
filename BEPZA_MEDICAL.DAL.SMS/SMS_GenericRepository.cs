using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BEPZA_MEDICAL.DAL.Infrastructure;
using BEPZA_MEDICAL.DAL.SMS;


namespace BEPZA_MEDICAL.DAL.SMS
{
    public class SMS_GenericRepository<T> : DataEF6Repository<T> where T : class
    {
        public SMS_GenericRepository(ERP_BEPZASMSEntities context)
            : base(context)
        {
            //constructor
        }
    }
}