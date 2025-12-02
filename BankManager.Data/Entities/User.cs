using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BankManager.Data.Entities
{
    [Table("users")]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        [Required]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(15)]
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public decimal MonthlyBudget { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
