namespace WebAppPharmacy.Models.VM
{
    public class SaleItemViewModel
    {

        public long BatchId { get; set; }
        public long? UnitItemId { get; set; }
        public string QrCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsMarked { get; set; }
        public bool IsRecipe { get; set; }
        
    }
}
