namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public class TaxOpeningDetail
    {
        #region Standard Property

        public int Id { get; set; }

        public int TaxOpeningId { get; set; }

        public int IncomeHeadId { get; set; }

        public string IncomeHead { get; set; }

        public string HeadSource { get; set; }

        public decimal IncomeAmount { get; set; }

        #endregion
        
    }
}
