using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using WebAppPharmacy.Models.Dictionaries;

namespace WebAppPharmacy.Models
{

    [Table("order_details")]
    [DisplayName("Детали заказа поставщику")]
    public class OrderDetail
    {
        public long Id { get; set; }

        [DisplayName("Заказ")]
        public long OrderId { get; set; }

        [DisplayName("Товар")]
        public long ProductId { get; set; }

        [DisplayName("Количество")]
        public int Quantity { get; set; }

        [DisplayName("Цена")]
        public decimal Price { get; set; }

        [DisplayName("Статус")]
        public long StatusId { get; set; }

        // Навигационные свойства
        public SupplierOrder Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public OrderDetailStatus Status { get; set; } = null!;
    }
}
