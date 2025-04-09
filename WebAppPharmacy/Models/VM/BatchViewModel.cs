namespace WebAppPharmacy.Models.VM
{
    public class BatchViewModel
    {
        public long Id { get; set; }
        public string ProductTitle { get; set; } = null!;
        public string BatchNumber { get; set; } = null!;
        public DateTime SupplyDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int Quantity { get; set; }
        public int RemainingQuantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public string StatusName { get; set; } = null!;
    }

}
