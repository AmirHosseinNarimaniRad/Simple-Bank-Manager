using SQLite;

namespace BankManagerApp.Models
{
    [Table("transactions")]
    public class TransactionDb
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int AccountId { get; set; } // Now refers to WalletId
        public string Type { get; set; } // Deposit / Withdraw
        public string Category { get; set; } // e.g., Food, Taxi, etc.
        public string IncomeType { get; set; } // e.g., Salary, Cash, etc.
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
    }
}
