using AutoMapper;
using BEPZA_MEDICAL.DAL.PGM;

using BEPZA_MEDICAL.Web.Areas.PGM.Models.SalaryHead;
using BEPZA_MEDICAL.Web.Areas.PGM.Models.SalaryHeadGroup;
using BEPZA_MEDICAL.Web.Areas.PGM.Models.SalaryStructure;


namespace BEPZA_MEDICAL.Web.Utility
{
    public class PGMMapper
    {
        public PGMMapper()
        {

            //Salary Head Group
            Mapper.CreateMap<SalaryHeadGroupViewModel, PRM_SalaryHeadGroup>();
            Mapper.CreateMap<PRM_SalaryHeadGroup, SalaryHeadGroupViewModel>();

            //Salary Head
            Mapper.CreateMap<SalaryHeadViewModel, PRM_SalaryHead>();
            Mapper.CreateMap<PRM_SalaryHead, SalaryHeadViewModel>();

            //Salary Structure
            Mapper.CreateMap<PRM_SalaryStructure, SalaryStructureModel>();
            Mapper.CreateMap<SalaryStructureModel, PRM_SalaryStructure>();

            //Salary Structure Details
            Mapper.CreateMap<SalaryStructureDetailsModel, PRM_SalaryStructureDetail>();
            Mapper.CreateMap<PRM_SalaryStructureDetail, SalaryStructureDetailsModel>();

            //emp salary structure
            Mapper.CreateMap<SalaryStructureDetailsModel, PRM_EmpSalaryDetail>();
            Mapper.CreateMap<PRM_EmpSalaryDetail, SalaryStructureDetailsModel>();

            
        }
    }
}
