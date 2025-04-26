namespace WebAppPharmacy.Models.VM
{
    public class SaleDetailItemViewModel
    {
        public string ProductTitle { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string QrCode { get; set; }
    }
}
