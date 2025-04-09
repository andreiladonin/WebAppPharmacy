using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppPharmacy.Models
{
    [Table("categories")]
    public class Category
    {
        public long Id { get; set; }
        [DisplayName("Название")]
        public string Title { get; set; } = null!;
    }

}
