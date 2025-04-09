using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppPharmacy.Models.Dictionaries
{
    [Table("supplier_order_statuses")]
    public class SupplierOrderStatus
    {
        public long Id { get; set; }

        [DisplayName("Название статуса")]
        public string StatusName { get; set; } = null!;
    }
}
