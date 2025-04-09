using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppPharmacy.Models.Dictionaries
{
    [Table("sale_statuses")]
    public class SaleStatus
    {
        public long Id { get; set; }

        [DisplayName("Название статуса")]
        public string StatusName { get; set; } = null!;
    }
}
