using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace WebAppPharmacy.Models
{
    [Table("prescription_items")]
    [DisplayName("Позиция рецепта")]
    public class PrescriptionItem
    {
        public long Id { get; set; }

        [DisplayName("Рецепт")]
        public long PrescriptionId { get; set; }

        [DisplayName("Товар")]
        public long ProductId { get; set; }

        [DisplayName("Количество")]
        public int Quantity { get; set; }

        [DisplayName("Использовано")]
        public int? DispensedQuantity { get; set; }

        // Навигация
        public Prescription Prescription { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }   
}
