using SQLite;

namespace BankManagerApp.Models
{
    [Table("bank_accounts")]
    public class BankAccountDb
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int UserId { get; set; }

        [MaxLength(100), NotNull]
        public string Name { get; set; } = string.Empty;

        public decimal Balance { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
