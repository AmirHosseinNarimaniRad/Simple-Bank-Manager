using SQLite;

namespace BankManagerApp.Models
{
    [Table("users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(50), NotNull]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50), NotNull]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(15), Unique, NotNull]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Ignore]
        public string FullName => $"{FirstName} {LastName}";
    }
}
