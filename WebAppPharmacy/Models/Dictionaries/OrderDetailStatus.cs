using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppPharmacy.Models.Dictionaries
{
    [Table("order_detail_statuses")]
    public class OrderDetailStatus
    {
        public long Id { get; set; }

        [DisplayName("Название статуса")]
        public string StatusName { get; set; } = null!;
    }
}
