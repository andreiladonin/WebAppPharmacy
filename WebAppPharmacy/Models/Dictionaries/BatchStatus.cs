using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppPharmacy.Models.Dictionaries
{
    [Table("batch_statuses")]
    public class BatchStatus
    {
        public long Id { get; set; }

        [DisplayName("Название статуса")]
        public string StatusName { get; set; } = null!;
    }

}
