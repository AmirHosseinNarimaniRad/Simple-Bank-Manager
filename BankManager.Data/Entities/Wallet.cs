using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankManager.Data.Entities
{
    [Table("bank_accounts")]
    public class Wallet
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Balance { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public User User { get; set; } = null!;
        
        public ICollection<TransactionDb> Transactions { get; set; } = new List<TransactionDb>();
    }
}
