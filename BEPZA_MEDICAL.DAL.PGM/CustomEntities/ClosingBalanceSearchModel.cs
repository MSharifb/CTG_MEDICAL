namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public  class ClosingBalanceSearchModel
    {
       public int intLeaveYearID { get; set; }
       public string strYearTitle { get; set; }
       public int intLeaveTypeID { get; set; }
       public string EmployeeId { get; set; }
       public double fltCB { get; set; }
    }
}
