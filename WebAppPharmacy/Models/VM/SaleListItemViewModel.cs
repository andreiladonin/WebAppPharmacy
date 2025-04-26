namespace WebAppPharmacy.Models.VM
{
    public class SaleListItemViewModel
    {
        public long Id { get; set; }
        public string ClientName { get; set; }
        public string EmployeeName { get; set; }
        public string StatusName { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal Total { get; set; }
    }
}
