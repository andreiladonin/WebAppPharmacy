namespace WebAppPharmacy.Models.VM
{
    public class ProductBatchViewModel
    {
        public long BatchId { get; set; }
        public long ProductId { get; set; }
        public string ProductTitle { get; set; } = null!;
        public string BatchNumber { get; set; } = null!; // Номер партии
        public DateTime? ExpirationDate { get; set; }
        public decimal Price { get; set; }
        public int RemainingQuantity { get; set; }
        public bool IsMarked { get; set; }
        public bool IsRecipe { get; set; }
        public List<string> AvailableQrCodes { get; set; } = new List<string>();
    }
}
