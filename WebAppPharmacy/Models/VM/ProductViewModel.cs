namespace WebAppPharmacy.Models.VM
{
    public class ProductViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string MeasurementUnit { get; set; } = null!;
        public string Manufacturer { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsRecipe { get; set; }
        public bool IsMarked { get; set; }
    }
}
