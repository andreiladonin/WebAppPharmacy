using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace WebAppPharmacy.Models
{
    [Table("suppliers")]
    [DisplayName("Поставщик")]
    public class Supplier
    {
        public long Id { get; set; }

        [DisplayName("Название компании")]
        public string Title { get; set; } = null!;

        [DisplayName("Адрес")]
        public string Address { get; set; } = null!;

        [DisplayName("Контактное лицо")]
        public string ContactPerson { get; set; } = null!;

        [DisplayName("Электронная почта")]
        public string Email { get; set; } = null!;

        [DisplayName("Телефон")]
        public long Phone { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<SupplierOrder> Orders { get; set; } = new List<SupplierOrder>();
    }
}
