namespace WebAppPharmacy.Models.VM
{
    public class OrderDetailViewModel
    {
        public long Id { get; set; }
        public string ProductTitle { get; set; } = null!;
        public long OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string StatusName { get; set; } = null!;
    }

}
