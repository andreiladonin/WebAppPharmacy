using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppPharmacy.Models.Dictionaries
{
    [Table("prescription_statuses")]
    public class PrescriptionStatus
    {
        public long Id { get; set; }

        [DisplayName("Название статуса рецепта")]
        public string StatusName { get; set; } = null!;
    }
}
