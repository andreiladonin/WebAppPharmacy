using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppPharmacy.Models
{
    [Table("employees")]
    public class Employee
    {
        public long Id { get; set; }
        [DisplayName("Имя сотрудника")]
        public string FullName { get; set; } = null!;
        [DisplayName("Должность")]
        public string Position { get; set; } = null!;

        public ICollection<Sale> Sales { get; set; } = new List<Sale>();


    }
}
