namespace WebAppPharmacy.Models.VM
{
    public class SaleDetailViewModel
    {

        public long Id { get; set; }
        public DateTime SaleDate { get; set; }
        public string ClientName { get; set; }
        public string EmployeeName { get; set; }
        public string StatusName { get; set; }
        public decimal Total { get; set; }

        public List<SaleDetailItemViewModel> Items { get; set; } = new();
    }

}
