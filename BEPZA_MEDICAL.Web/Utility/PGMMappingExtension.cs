using System.Collections.Generic;
using AutoMapper;
using BEPZA_MEDICAL.DAL.PGM;

using BEPZA_MEDICAL.Web.Areas.PGM.Models.SalaryHead;
using BEPZA_MEDICAL.Web.Areas.PGM.Models.SalaryHeadGroup;
using BEPZA_MEDICAL.Web.Areas.PGM.Models.SalaryStructure;


namespace BEPZA_MEDICAL.Web.Utility
{
    public static class PGMMappingExtension
    {

        //Salary Head Group
        public static SalaryHeadGroupViewModel ToModel(this PRM_SalaryHeadGroup obj)
        {
            return Mapper.Map<PRM_SalaryHeadGroup, SalaryHeadGroupViewModel>(obj);
        }
        public static PRM_SalaryHeadGroup ToEntity(this SalaryHeadGroupViewModel obj)
        {
            return Mapper.Map<SalaryHeadGroupViewModel, PRM_SalaryHeadGroup>(obj);
        }

        //Salary Head
        public static SalaryHeadViewModel ToModel(this PRM_SalaryHead obj)
        {
            return Mapper.Map<PRM_SalaryHead, SalaryHeadViewModel>(obj);
        }
        public static PRM_SalaryHead ToEntity(this SalaryHeadViewModel obj)
        {
            return Mapper.Map<SalaryHeadViewModel, PRM_SalaryHead>(obj);
        }

        //Salary Structure

        public static SalaryStructureModel ToModel(this PRM_SalaryStructure obj)
        {
            return Mapper.Map<PRM_SalaryStructure, SalaryStructureModel>(obj);
        }
        public static PRM_SalaryStructure ToEntity(this SalaryStructureModel obj)
        {
            return Mapper.Map<SalaryStructureModel, PRM_SalaryStructure>(obj);
        }

        public static List<PRM_SalaryStructure> ToEntityList(this List<SalaryStructureModel> modellist)
        {
            List<PRM_SalaryStructure> list = new List<PRM_SalaryStructure>();
            foreach (var item in modellist)
            {
                list.Add(Mapper.Map<SalaryStructureModel, PRM_SalaryStructure>(item));
            }
            return list;
        }
        
        //Salary Structure Details
        public static SalaryStructureDetailsModel ToModel(this PRM_SalaryStructureDetail obj)
        {
            return Mapper.Map<PRM_SalaryStructureDetail, SalaryStructureDetailsModel>(obj);
        }
        public static PRM_SalaryStructureDetail ToEntity(this SalaryStructureDetailsModel obj)
        {
            return Mapper.Map<SalaryStructureDetailsModel, PRM_SalaryStructureDetail>(obj);
        }

        public static List<SalaryStructureDetailsModel> ToModelList(this List<PRM_SalaryStructureDetail> objlist)
        {
            List<SalaryStructureDetailsModel> list = new List<SalaryStructureDetailsModel>();
            foreach (var item in objlist)
            {
                list.Add(Mapper.Map<PRM_SalaryStructureDetail, SalaryStructureDetailsModel>(item));
            }

            return list;
        }
        public static List<PRM_SalaryStructureDetail> ToEntityList(this List<SalaryStructureDetailsModel> modellist)
        {
            List<PRM_SalaryStructureDetail> list = new List<PRM_SalaryStructureDetail>();
            foreach (var item in modellist)
            {
                list.Add(Mapper.Map<SalaryStructureDetailsModel, PRM_SalaryStructureDetail>(item));
            }

            return list;
        }

        //Employee Dalary Details
        public static SalaryStructureDetailsModel ToModel(this PRM_EmpSalaryDetail obj)
        {
            return Mapper.Map<PRM_EmpSalaryDetail, SalaryStructureDetailsModel>(obj);
        }

        
    }
}