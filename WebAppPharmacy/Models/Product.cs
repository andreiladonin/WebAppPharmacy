using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using WebAppPharmacy.Models.Dictionaries;

namespace WebAppPharmacy.Models
{
    [Table("products")]
    [DisplayName("Товар")]
    public class Product
    {
        public long Id { get; set; }

        [DisplayName("Название")]
        public string Title { get; set; } = null!;

        [DisplayName("Описание")]
        public string Description { get; set; } = null!;

        [DisplayName("Лек. форма")]
        public string? DosageForm { get; set; }

        [DisplayName("Дозировка")]
        public string? Dosage { get; set; }

        [DisplayName("Требует рецепт")]
        public bool IsRecipe { get; set; }

        [DisplayName("Маркирован")]
        public bool IsMarked { get; set; }

        [DisplayName("Цена")]
        public decimal Price { get; set; }

        [DisplayName("Мин. кол-во")]
        public int MinQuantity { get; set; }

        [DisplayName("Ед. измерения")]
        public long MeasurementUnitId { get; set; }

        // Внешние ключи
        public long ManufacturerId { get; set; }
        public long CategoryId { get; set; }
        public long TypeId { get; set; }

        // Навигация
        public Supplier Manufacturer { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ProductType Type { get; set; } = null!;
        public MeasurementUnit MeasurementUnit { get; set; } = null!;
        public ICollection<Batch> Batches { get; set; } = new List<Batch>();
    }

}
