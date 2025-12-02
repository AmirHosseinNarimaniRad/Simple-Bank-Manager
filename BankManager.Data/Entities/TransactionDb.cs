using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankManager.Data.Entities
{
    [Table("transactions")]
    public class TransactionDb
    {
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; } // Now refers to WalletId
        public string Type { get; set; } // Deposit / Withdraw
        public string Category { get; set; } // e.g., Food, Taxi, etc.
        public string IncomeType { get; set; } // e.g., Salary, Cash, etc.
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; } // Transaction Date
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
