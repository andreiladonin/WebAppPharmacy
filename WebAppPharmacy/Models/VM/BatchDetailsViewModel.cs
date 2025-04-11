using WebAppPharmacy.Models.DTO;

namespace WebAppPharmacy.Models.VM
{
    public class BatchDetailsViewModel
    {
        public long BatchId { get; set; }
        public string BatchNumber { get; set; } = null!;
        public string ProductTitle { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public int RemainingQuantity { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsMarked { get; set; }
        public List<UnitItemDto> UnitItems { get; set; } = new List<UnitItemDto>();
    }
}
