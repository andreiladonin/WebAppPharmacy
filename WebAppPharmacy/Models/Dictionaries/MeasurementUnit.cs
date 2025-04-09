using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppPharmacy.Models.Dictionaries
{
    [Table("measurement_units")]
    public class MeasurementUnit
    {
        public long Id { get; set; }

        [DisplayName("Название")]
        public string UnitName { get; set; } = null!;

        [DisplayName("Сокращение")]
        public string Abbreviation { get; set; } = null!;
    }
}
