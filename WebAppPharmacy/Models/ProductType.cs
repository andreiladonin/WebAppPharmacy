using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppPharmacy.Models
{
    [Table("product_types")]
    public class ProductType
    {
        public long Id { get; set; }
        [DisplayName("Тип тоавра")]
        public string TypeName { get; set; } = null!;
    }
}
