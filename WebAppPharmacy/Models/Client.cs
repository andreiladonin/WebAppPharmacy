using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppPharmacy.Models
{
    [Table("clients")]
    public class Client
    {
        public long Id { get; set; }
        [DisplayName("Имя клиента")]
        public string FullName { get; set; } = null!;
        [DisplayName("Дата рождения")]
        public DateOnly DateBirthday { get; set; }
        [DisplayName("Адрес эл. почты")]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [DisplayName("Номер телефона")]
        public long Phone { get; set; }

        // Навигация
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
