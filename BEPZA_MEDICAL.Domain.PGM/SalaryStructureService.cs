using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPZA_MEDICAL.DAL.PGM;

namespace BEPZA_MEDICAL.Domain.PGM
{
    public class SalaryStructureService : PGMCommonService
    {
        public SalaryStructureService(PGM_UnitOfWork uow)
            : base(uow)
        {
        }

        public IList<PRM_SalaryStructureDetail> GetSalaryStrutureDetailsByGradeAndStepId(int gradeId, int stepId, out int salaryStructureId)
        {
            var query = from s in base.PGMUnit.SalaryStructureDetailRepository.Fetch()
                        where s.PRM_SalaryStructure.GradeId == gradeId && s.PRM_SalaryStructure.StepId == stepId
                        select s;

            var queryId = from q in query
                          select q.PRM_SalaryStructure.Id;

            var result = query.ToList();
            salaryStructureId = queryId.FirstOrDefault();

            return result;
        }

    }
}
