using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppPharmacy.Models.DTO;

namespace WebAppPharmacy.Models.VM
{
    public class UnitItemViewModel
    {
        // Информация о партии
        public long BatchId { get; set; }
        [DisplayName("Номер партии")]
        public string BatchNumber { get; set; } = null!;
        [DisplayName("Остаток")]
        public int RemainingQuantity { get; set; }

        // Информация о товаре
        [DisplayName("Название товара")]
        public string ProductTitle { get; set; } = null!;
        [DisplayName("Цена")]
        public decimal ProductPrice { get; set; }

        // Список маркированных единиц
        [DisplayName("Единицы в партии")]
        public List<UnitItemDto> UnitItems { get; set; } = new List<UnitItemDto>();

        // Выпадающие списки для продажи
        [DisplayName("Клиенты")]
        public List<SelectListItem> Clients { get; set; } = new List<SelectListItem>();
        [DisplayName("Сотрудники")]
        public List<SelectListItem> Employees { get; set; } = new List<SelectListItem>();
        [DisplayName("Статусы")]
        public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
    }
}
