using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using WebAppPharmacy.Models.Dictionaries;

namespace WebAppPharmacy.Models
{
    [Table("supplier_orders")]
    [DisplayName("Заказ поставщику")]
    public class SupplierOrder
    {
        public long Id { get; set; }

        [DisplayName("Дата заказа")]
        public DateTime OrderDate { get; set; }

        [DisplayName("Дата доставки")]
        public DateTime? DeliveryDate { get; set; }

        [DisplayName("Сумма")]
        public decimal TotalAmount { get; set; }

        public long SupplierId { get; set; }
        public long StatusId { get; set; }

        // Навигация
        public Supplier Supplier { get; set; } = null!;
        public SupplierOrderStatus Status { get; set; } = null!;
    }
}
