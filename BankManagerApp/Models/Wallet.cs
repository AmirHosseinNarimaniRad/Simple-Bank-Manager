using SQLite;

namespace BankManagerApp.Models
{
    [Table("bank_accounts")]
    public class Wallet
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; } // e.g., Cash, Card

        public decimal Balance { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
