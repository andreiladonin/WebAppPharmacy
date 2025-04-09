using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using WebAppPharmacy.Models.Dictionaries;

namespace WebAppPharmacy.Models
{
    [Table("sales")]
    [DisplayName("Продажа")]
    public class Sale
    {
        public long Id { get; set; }

        [DisplayName("Дата продажи")]
        public DateTime SaleDate { get; set; }

        [DisplayName("Сумма")]
        public decimal Total { get; set; }

        public long ClientId { get; set; }
        public long EmployeeId { get; set; }
        public long StatusId { get; set; }

        // Навигация
        public Client Client { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public SaleStatus Status { get; set; } = null!;
        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}
