using SQLite;

namespace BankManagerApp.Models
{
    [Table("transactions")]
    public class TransactionDb
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int AccountId { get; set; }

        [NotNull]
        public string Type { get; set; } = string.Empty; // "Deposit" or "Withdraw"

        public decimal Amount { get; set; }

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
