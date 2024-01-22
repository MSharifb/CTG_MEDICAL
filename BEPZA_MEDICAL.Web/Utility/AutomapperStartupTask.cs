using BEPZA_MEDICAL.Utility;
using AutoMapper;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class AutomapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            PRMMapper _prmMapper = new PRMMapper();
            FAMMapper _famMapper = new FAMMapper();
            PGMMapper _pgmMapper = new PGMMapper();
            CPFMapper _cpfMapper = new CPFMapper();
            INVMapper _invMapper = new INVMapper();
            PMIMapper _pmiMapper = new PMIMapper();
            WFMMapper _wfmMapper = new WFMMapper();
            FMSMapper _fmsMapper = new FMSMapper();
            FARMapper _farMapper = new FARMapper();
            AMSMapper _amsMapper = new AMSMapper();
            SMSMapper _smsMapper = new SMSMapper();

        }
        protected virtual void ViceVersa<T1, T2>()
        {
            Mapper.CreateMap<T1, T2>();
            Mapper.CreateMap<T2, T1>();
        }
    }
}