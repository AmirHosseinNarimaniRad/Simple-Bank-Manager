using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankManager.Data.Enums;

namespace BankManager.Data.Entities
{
    [Table("categories")]
    public class Category
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public TransactionType Type { get; set; }
        
        public string? Icon { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        // Navigation property
        public ICollection<TransactionDb> Transactions { get; set; } = new List<TransactionDb>();
    }
}
