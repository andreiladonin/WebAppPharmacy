using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace WebAppPharmacy.Models
{
    [Table("unit_items")]
    [DisplayName("Маркированная единица")]
    public class UnitItem
    {
        public long Id { get; set; }

        [DisplayName("QR-код")]
        public string QrCode { get; set; } = null!;

        [DisplayName("Партия")]
        public long BatchId { get; set; }
        public bool IsSold { get; set; }
        // Навигация
        public Batch Batch { get; set; } = null!;
    }
}
