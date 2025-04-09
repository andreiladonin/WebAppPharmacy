namespace WebAppPharmacy.Models.VM
{
    public class SaleDetailViewModel
    {
        public long Id { get; set; }
        public long SaleId { get; set; }
        public string BatchNumber { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
    }

}
