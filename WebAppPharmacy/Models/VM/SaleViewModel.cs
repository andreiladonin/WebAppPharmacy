namespace WebAppPharmacy.Models.VM
{
    public class SaleViewModel
    {
        public long Id { get; set; }
        public string ClientName { get; set; } = null!;
        public string EmployeeName { get; set; } = null!;
        public DateTime SaleDate { get; set; }
        public decimal Total { get; set; }
        public string StatusName { get; set; } = null!;
    }
}
