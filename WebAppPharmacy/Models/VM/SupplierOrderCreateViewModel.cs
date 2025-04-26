using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using WebAppPharmacy.Models.Dictionaries;

namespace WebAppPharmacy.Models.VM
{
    public class SupplierOrderCreateViewModel
    {
        [DisplayName("Дата создания")]
        public DateTime OrderDate { get; set; }
        [DisplayName("Дата поставки")]
        public DateTime? DeliveryDate { get; set; }
        [DisplayName("Сумма")]
        public decimal TotalAmount { get; set; }
        [DisplayName("Поставщик")]
        public long SupplierId { get; set; }
        [DisplayName("Статус")]
        public long StatusId { get; set; }

        public List<SupplierDetailCreateViewModel> Details { get; set; } = new();
    }

    public class SupplierDetailCreateViewModel
    {
        [DisplayName("Товар")]
        public long ProductId { get; set; }

        [DisplayName("Количество")]
        public int Quantity { get; set; }

        [DisplayName("Цена")]
        public decimal Price { get; set; }

        [DisplayName("Статус")]
        public long StatusId { get; set; }
    }
}
