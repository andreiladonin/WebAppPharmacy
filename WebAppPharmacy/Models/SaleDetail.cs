using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace WebAppPharmacy.Models
{
    [Table("sale_details")]
    [DisplayName("Деталь продажи")]
    public class SaleDetail
    {
        public long Id { get; set; }

        [DisplayName("Количество")]
        public int Quantity { get; set; }

        [DisplayName("Цена за единицу")]
        public decimal Price { get; set; }

        [DisplayName("Скидка")]
        public decimal? Discount { get; set; }

        public long SaleId { get; set; }
        public long BatchId { get; set; }

        // Навигация
        public Sale Sale { get; set; } = null!;
        public Batch Batch { get; set; } = null!;
    }
}
