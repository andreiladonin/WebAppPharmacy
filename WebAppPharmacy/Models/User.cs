using System.ComponentModel.DataAnnotations;

namespace WebAppPharmacy.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;
        [Required]
        public string PasswordHash { get; set; } = null!;
    }
}
