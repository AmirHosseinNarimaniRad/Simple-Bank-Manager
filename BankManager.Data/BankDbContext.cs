using BankManager.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankManager.Data
{
    public class BankDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<TransactionDb> Transactions { get; set; }

        private readonly string _databasePath;

        public BankDbContext()
        {
            // Default constructor for design-time tools
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _databasePath = Path.Combine(folder, "bankmanager.db");
        }

        public BankDbContext(string databasePath)
        {
            _databasePath = databasePath;
        }

        // Constructor for DI
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={_databasePath}");
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Data - Use static date to avoid PendingModelChangesWarning
            var seedDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    FirstName = "Test", 
                    LastName = "User", 
                    PhoneNumber = "09123456789", 
                    MonthlyBudget = 5000000,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );

            modelBuilder.Entity<Wallet>().HasData(
                new Wallet 
                { 
                    Id = 1, 
                    UserId = 1, 
                    Name = "کیف پول تست", 
                    Balance = 1000000,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );

            modelBuilder.Entity<TransactionDb>().HasData(
                new TransactionDb 
                { 
                    Id = 1, 
                    AccountId = 1, 
                    Type = "Income", 
                    Category = "حقوق",
                    IncomeType = "حقوق",
                    Amount = 1000000, 
                    Description = "تست اولیه", 
                    DateTime = seedDate,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is User || e.Entity is Wallet || e.Entity is TransactionDb);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    // Set CreatedAt via reflection or casting if we had a base interface
                    // For now, casting to known types or using dynamic
                    if (entry.Entity is User u) { u.CreatedAt = DateTime.UtcNow; u.UpdatedAt = DateTime.UtcNow; }
                    if (entry.Entity is Wallet w) { w.CreatedAt = DateTime.UtcNow; w.UpdatedAt = DateTime.UtcNow; }
                    if (entry.Entity is TransactionDb t) { t.CreatedAt = DateTime.UtcNow; t.UpdatedAt = DateTime.UtcNow; }
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is User u) u.UpdatedAt = DateTime.UtcNow;
                    if (entry.Entity is Wallet w) w.UpdatedAt = DateTime.UtcNow;
                    if (entry.Entity is TransactionDb t) t.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
