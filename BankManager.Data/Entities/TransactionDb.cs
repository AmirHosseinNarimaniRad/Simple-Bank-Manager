using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankManager.Data.Enums;

namespace BankManager.Data.Entities
{
    [Table("transactions")]
    public class TransactionDb
    {
        [Key]
        public int Id { get; set; }
        
        public int AccountId { get; set; } // Foreign key to Wallet
        
        public TransactionType Type { get; set; } // Enum: Deposit or Withdraw
        
        public string Category { get; set; } = string.Empty; // Legacy category name
        
        public int? CategoryId { get; set; } // Foreign key to Category (nullable for now)
        
        public string? IncomeType { get; set; } // Legacy field, nullable
        
        public decimal Amount { get; set; }
        
        public string? Description { get; set; }
        
        public DateTime DateTime { get; set; } // Transaction Date
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public Wallet Wallet { get; set; } = null!;
        
        public Category? CategoryEntity { get; set; }
    }
}
