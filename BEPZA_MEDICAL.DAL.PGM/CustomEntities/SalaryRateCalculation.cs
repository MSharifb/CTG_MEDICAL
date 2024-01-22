using System;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public  class SalaryRateCalculation
    {
       public SalaryRateCalculation()
       {
         
       }

       public int Id { get; set; }
       public int EmployeeId { get; set; }
       public Decimal Amount { get; set; }
       public Decimal Basic { get; set; }
       public Decimal GrossSalary { get; set; }
       public Decimal Rate { get; set; }
       public string HeadType { get; set; }
       public string AmountType { get; set; }
      
    }
    
}
