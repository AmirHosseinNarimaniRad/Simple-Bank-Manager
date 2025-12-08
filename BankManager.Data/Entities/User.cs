using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankManager.Data.Entities
{
    [Table("users")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        public string Email { get; set; } = string.Empty;
        
        public string Username { get; set; } = string.Empty;
        
        public string PasswordHash { get; set; } = string.Empty;
        
        public string? PasswordResetToken { get; set; }
        
        public DateTime? PasswordResetTokenExpiry { get; set; }
        
        public decimal MonthlyBudget { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        // Navigation property
        public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
    }
}
